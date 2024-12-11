using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class PlayerBlips : BaseScript
    {
        static public string lastBlipSetting = "all";
        static public int allButPlayer = -1;
        public PlayerBlips()
        {
        }

        [EventHandler("updatePlayerBlips")]
        public void updatePlayerBlips()
        {
            if (lastBlipSetting == "all") setPlayerBlipsForAll();
            else if (lastBlipSetting == "none") clearPlayerBlipsForAll();
            // else if (lastBlipSetting == "hunt") setPlayerBlipsForAllButRunner();
            // else if (lastBlipSetting == "allBut") setPlayerBlipsForAllBut();

        }

        [Command("pb", Restricted = true)] //normal restriction true 
        public void playerBlipHandling(int source, List<object> args, string raw)
        {
            if (args.Count == 1 && (args[0].ToString() == "all" || args[0].ToString() == "none"))
            {
                lastBlipSetting = args[0].ToString();
               
            }
            else if (args.Count == 0)
            {
                if (lastBlipSetting == "all")
                {
                    lastBlipSetting = "none";
                }
                else if (lastBlipSetting == "none")
                {
                    lastBlipSetting = "all";
                }
            }


            if (lastBlipSetting == "all")
            {
                TriggerClientEvent("ShowNotification", "Player blips are on.");
            }
            else if (lastBlipSetting == "none")
            {
                TriggerClientEvent("ShowNotification", "Player blips are off.");
            }
             updatePlayerBlips();
        }

        public void setPlayerBlipsForAll()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            foreach (Player player in Players)
            {
                int PlayerHandle = int.Parse(player.Handle.ToString());
                BlipHandler.BlipData playerblip = new BlipHandler.BlipData($"{player.Name}-{player.Handle}")
                {
                    Type = "player",
                    Colour = PlayerHandle + 5,
                    Shrink = false,
                    MapName = player.Name
                };
                request.BlipsToAdd.Add(playerblip);
            }
            BlipHandler.AddBlips(request);
            lastBlipSetting = "all";
        }


        public void clearPlayerBlipsForAll()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            foreach (Player player in Players)
            {
                request.BlipsToRemove.Add($"{player.Name}-{player.Handle}");
            }
            BlipHandler.AddBlips(request);
            lastBlipSetting = "none";
        }

        // public void setPlayerBlipsForAllButRunner()
        // {
        //     BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
        //     foreach (Player player in Players)
        //     {
        //         BlipHandler.BlipData playerblip = new BlipHandler.BlipData($"{player.Name}-{player.Handle}")
        //         {
        //             Type = "player",
        //             Colour = 7,
        //             Shrink = false,
        //             MapName = player.Name,
        //         };
        //         request.BlipsToAdd.Add(playerblip);
        //     }
        //     BlipHandler.AddBlips(request);

        // }

        // public void setPlayerBlipsForAllBut()
        // {
        //     BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
        //     foreach (Player player in Players)
        //     {
        //         BlipHandler.BlipData playerblip = new BlipHandler.BlipData($"{player.Name}-{player.Handle}")
        //         {
        //             Type = "player",
        //             Colour = 7,
        //             Shrink = false,
        //             MapName = player.Name,
        //         };
        //         request.BlipsToAdd.Add(playerblip);
        //     }
        //     BlipHandler.AddBlips(request);

        // }


        // public void setPlayerBlipsForAllButIsDifferent()
        // {
        //     BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
        //     foreach (Player player in Players)
        //     {
        //         BlipHandler.BlipData playerblip = new BlipHandler.BlipData($"{player.Name}-{player.Handle}")
        //         {
        //             Type = "player",
        //             Colour = 7,
        //             Shrink = false,
        //             MapName = player.Name,
        //         };
        //         request.BlipsToAdd.Add(playerblip);
        //     }
        //     BlipHandler.AddBlips(request);

        // }
    }
}
