using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Mono.CSharp;

namespace STHMaxzzzie.Client
{
    //start
    public class Misc : BaseScript
    {
        bool isShootingFromVehicleAllowed = false;

        [EventHandler("Stamina")]
        private void Stamina()
        {
            API.SetPlayerMaxStamina(Game.Player.Handle, 100);
            Debug.WriteLine($"added 100 stamina");
        }

        [EventHandler("clear_vehicles")]
        void RemoveAllVehicles(bool shouldRemoveProps)
        {
            Vehicle[] allVeh = World.GetAllVehicles();
            foreach (Vehicle veh in allVeh)
            {
                veh.Delete();
            }
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"All vehicles are removed." } });
            if (shouldRemoveProps)
            {
                Prop[] allProp = World.GetAllProps();
                foreach (Prop prop in allProp)
                {
                    prop.Delete();
                }
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"All entities are removed too." } });
            }
        }

        //[EventHandler("disableCanPlayerShootFromVehicles")]
        void DisableCanPlayerShootFromVehicles(bool sfv)
        {
            isShootingFromVehicleAllowed = sfv;
            SetPlayerCanDoDriveBy(PlayerId(), isShootingFromVehicleAllowed);
        }
    }

    public class MapBounds : BaseScript
    //all circles get cleared everytime the circlelist updates on the server.
    //circles get a different (blip id) for each client that makes a circle. So they are stored localy in blipList
    {
        int blip = 0;
        List<Int32> blipList = new List<Int32>();

        //deleting circle
        [EventHandler("delCircle")]
        void delCircle()
        {
            foreach (int blip in blipList)
            {
                int allBlip = blip;
                RemoveBlip(ref allBlip);
            }
            blipList.Clear();
        }

        [EventHandler("updateCircle")]
        void updateCircle(List<object> argArray)
        {
            blip = AddBlipForRadius(float.Parse(argArray[0].ToString()), float.Parse(argArray[1].ToString()), 0, float.Parse(argArray[2].ToString()));
            SetBlipAlpha(blip, 40);//sets opacity of the mapbound circles
            SetBlipColour(blip, int.Parse(argArray[3].ToString()));
            blipList.Add(blip);
        }
    }
}


