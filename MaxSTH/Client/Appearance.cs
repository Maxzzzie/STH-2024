using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class Appearance : BaseScript
    {
        string model = "null";
        static List<string> nonAnimalModel = new List<string>();
        bool isModelChanging = false;

        //List<PedHash> PedModels = Enum.GetValues(typeof(PedHash)).Cast<PedHash>().ToList();


        public Appearance()
        {
            API.RegisterKeyMapping("+SelectRandomModel", "Randomize ped model.", "keyboard", "F6");
        }


        [EventHandler("getNonAnimalModelList")]
        void getNonAnimalModel(string getModel)
        {
            nonAnimalModel.Add(getModel.Trim());
        }

        //sends f6 press to server
        [Command("+SelectRandomModel")]
        void randmodelHandler()
        {
            Vector3 speed = Game.PlayerPed.Velocity;
            if (Game.PlayerPed.IsAlive == false || IsPauseMenuActive() || isModelChanging)
            {
                return;
            }
            if (!Game.PlayerPed.IsOnFoot)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"You have to be on foot to change your appearance." } });
                return;
            }
            if (speed.X >= 0.2f || speed.Y >= 0.2f || speed.Z >= 0.2f)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Stand still to change your appearance." } });
                return;
            }
            TriggerEvent("changeRandomModel");
        }

        //does nothing but prevends an error msg upon release of f6
        [Command("-SelectRandomModel")]
        void randmodelHandler_() { } //add empty handler for -SelectRandomModel so it doesn't show up in chat. 

        [EventHandler("changingModel")]
        public async void changingModel(string getModel)
        {
            isModelChanging = true;
            int HealthBeforeChange = API.GetEntityHealth(API.PlayerPedId());
            int ArmourBeforeChange = API.GetPedArmour(API.PlayerPedId());
            int playerHandle = Game.Player.ServerId;
            model = getModel;
            bool didChange = false;
            while (!didChange)
            {
                didChange = await Game.Player.ChangeModel(new Model(model));
                await Delay(20);
            }
            SetPedDefaultComponentVariation(PlayerPedId());
            NotificationScript.ShowNotification($"You changed your player model to: {model}.");
            TriggerEvent("lastWeaponClass", true);
            TriggerServerEvent("updateServerModel", playerHandle, model);
            Health.SetPlayerStats(HealthBeforeChange, ArmourBeforeChange);
            await Delay(200);
            TriggerServerEvent("updatePlayerBlips");
            isModelChanging = false;

        }

        //gets triggered by pressing f6 and /model
        [EventHandler("changeRandomModel")]
        public static void changeRandomModel()
        {
            if (nonAnimalModel.Count != 0)
            {
                var rand = new Random();
                var index = rand.Next(0, nonAnimalModel.Count);
                TriggerEvent("changingModel", nonAnimalModel[index]);
            }
            else Debug.WriteLine($"nonAnimalModel list is empty");
        }

        [Command("model")]
        public void requestModel(int source, List<object> args, string raw)
        {
            Vector3 speed = Game.PlayerPed.Velocity;
            if (Game.PlayerPed.IsAlive == false || isModelChanging)
            {
                return;
            }
            if (!Game.PlayerPed.IsOnFoot)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"You have to be on foot to change your appearance." } });
                return;
            }
            if (speed.X >= 0.2f || speed.Y >= 0.2f || speed.Z >= 0.2f)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Stand still to change your appearance." } });
                return;
            }
            if (args.Count == 0)
            {
                TriggerEvent("changeRandomModel");
            }
            else if (args.Count == 1)
            {
                string input = args[0].ToString();
                bool modelIsInList = nonAnimalModel.Contains(input);


                if (modelIsInList == true)
                    TriggerEvent("changingModel", input);

                else if (input == "ems")
                {
                    string[] emsModel = new string[] { "csb_cop", "s_f_y_ranger_01", "s_m_m_snowcop_01", "s_m_y_cop_01", "s_m_y_hwaycop_01", "mp_m_fibsec_01", "mp_m_securoguard_01", "s_f_y_sheriff_01", "s_m_y_fireman_01", "s_m_m_paramedic_01", "s_m_m_doctor_01", "s_m_y_autopsy_01", "s_m_m_highsec_01", "s_m_m_highsec_01" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, emsModel.Length);
                    TriggerEvent("changingModel", emsModel[modelIndex]);
                }
                else if (input == "story")
                {
                    string[] storyModel = new string[] { "ig_lestercrest_2", "cs_lamardavis", "cs_jimmydisanto", "cs_fabien", "cs_floyd", "cs_martinmadrazo", "cs_mrsphillips", "cs_patricia", "cs_tracydisanto", "csb_chef", "csb_cletus", "csb_mp_agent14", "csb_prologuedriver", "csb_paige", "csb_tonya", "csb_agatha", "player_one", "player_two", "player_zero", "ig_lestercrest", "cs_davenorton", "cs_brad", "cs_amandatownley" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, storyModel.Length);
                    TriggerEvent("changingModel", storyModel[modelIndex]);
                }
                else if (input == "special")
                {
                    string[] specialModel = new string[] { "s_m_y_mime", "s_m_y_clown_01", "s_m_m_strperf_01", "cs_bradcadaver", "cs_hunter", "s_m_m_mariachi_01", "mp_m_bogdangoon", "cs_priest", "s_m_m_movalien_01", "s_m_m_movspace_0", "hc_driver", "hc_gunman", "u_m_o_filmnoir", "u_m_m_streetart_01", "u_m_y_zombie_01", "u_m_y_rsranger_01", "u_m_y_imporage", "u_m_y_pogo_01", "u_m_y_mani", "u_m_y_juggernaut_01", };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, specialModel.Length);
                    TriggerEvent("changingModel", specialModel[modelIndex]);
                }
                else if (input == "ed")
                {
                    string[] edModel = new string[] { "a_f_y_hipster_02", "g_f_y_vagos_01", "g_f_y_vagos_01", "g_f_y_vagos_01", "g_f_y_vagos_01", "cs_jewelass", "u_f_y_spyactress", "a_f_y_hipster_01", "cs_movpremf_01", "csb_bride", "ig_paige", "csb_screen_writer", "a_f_y_yoga_01", "a_f_y_hiker_01", "a_f_y_hippie_01", };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, edModel.Length);
                    TriggerEvent("changingModel", edModel[modelIndex]);
                }
                else if (input == "nasfi")
                {
                    string[] nasfiModel = new string[] { "g_m_m_armgoon_01", "mp_m_bogdangoon", "mp_m_fibsec_01", "s_m_m_highsec_01", "s_m_m_highsec_02" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, nasfiModel.Length);
                    TriggerEvent("changingModel", nasfiModel[modelIndex]);
                }
                else if (input == "boss")
                {
                    string[] bossModel = new string[] { "u_m_y_babyd", "g_m_m_chicold_01", "ig_hunter", "csb_cop", "g_m_y_ballasout_01", "g_m_y_famca_01" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, bossModel.Length);
                    TriggerEvent("changingModel", bossModel[modelIndex]);
                }
                else if (input == "max")
                {
                    string[] maxModel = new string[] { "u_f_y_dancerave_01" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, maxModel.Length);
                    TriggerEvent("changingModel", maxModel[modelIndex]);
                }
                else if (input == "gil")
                {
                    string[] maxModel = new string[] { "g_m_y_mexgoon" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, maxModel.Length);
                    TriggerEvent("changingModel", maxModel[modelIndex]);
                }
                else if (input == "fw")
                {
                    string[] fwModel = new string[] { "ig_johnnyklebitz" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, fwModel.Length);
                    TriggerEvent("changingModel", fwModel[modelIndex]);
                }
                else if (input == "oltl") //one lemon two limes
                {
                    string[] fwModel = new string[] { "a_m_y_acult_01" };
                    var rand = new Random();
                    int modelIndex = rand.Next(0, fwModel.Length);
                    TriggerEvent("changingModel", fwModel[modelIndex]);
                }
                else
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Oh no. Something went wrong!\nYou should do /model (\"modelname\"/ ems/ story/ special/ or keep it empty for a random model)" } });
                }
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Oh no. Something went wrong!\nYou should do /model (\"modelname\"/ ems/ story/ special/ or keep it empty for a random model)" } });
            }
            // Debug.WriteLine("model change updated stamina.");
        }
    }
}
