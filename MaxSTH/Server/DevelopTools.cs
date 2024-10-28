using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.IO;

namespace STHMaxzzzie.Server
{
    public class DevelopTools : BaseScript
    {
        static List<string> respawnBlips = new List<string>();
        static List<string> calloutBlips = new List<string>();
        bool displayRespawnBlips = false;
        bool displayCalloutBlips = false;

        public DevelopTools()
        {
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"This is the develop build. Develop tools are on!" } });
        }

        //         [EventHandler("clearRespawnBlips")]
        //         void clearRespawnBlips()
        //         {
        //             foreach (string respawnName in respawnBlips)
        //             {
        //                 //TriggerEvent("addBlip", true, respawnName, "coord", new Vector3(-2000, 0, 0), 0, 0, 0, true, false, true);
        //             }
        //             CitizenFX.Core.Debug.WriteLine($"Respawn blips cleared.");
        //             //respawnBlips.Clear();
        //         }

        [EventHandler("placeRespawnBlips")]
        void placeRespawnBlips()
        {
            foreach (var kvp in ServerMain.respawnLocationsDict)
            {
                //CitizenFX.Core.Debug.WriteLine($"AddBlip in Place Respawn blips. {kvp.Key}.");
                //TriggerEvent("addBlip", false, kvp.Key, "coord", new Vector3(kvp.Value.X, kvp.Value.Y, kvp.Value.Z), 0, 84, 56, true, false, true);
                //respawnBlips.Add(kvp.Key);
                int blipId = API.AddBlipForCoord(kvp.Value.X, kvp.Value.Y, kvp.Value.Z);
                API.SetBlipSprite(blipId, 84);
            }
            CitizenFX.Core.Debug.WriteLine($"Respawn blips placed.");
        }

        //         [EventHandler("clearCalloutBlips")]
        //         void clearCalloutBlips()
        //         {
        //             foreach (string calloutName in calloutBlips)
        //             {
        //                 //TriggerEvent("addBlip", true, calloutName, "coord", new Vector3(0, 0, 0), 0, 133, 47, true, false, true);
        //             }
        //             CitizenFX.Core.Debug.WriteLine($"Callout blips cleared.");
        //             //calloutBlips.Clear();
        //         }

        [EventHandler("placeCalloutBlips")]
        void placeCalloutBlips()
        {
            Debug.WriteLine($"placeCallutblips with {ServerMain.maxzzzieCalloutsDict.Count}");
            foreach (var kvp in ServerMain.maxzzzieCalloutsDict)
            {
                //TriggerEvent("addBlip", false, kvp.Key, "coord", new Vector3(kvp.Value.X, kvp.Value.Y, kvp.Value.Z), 0, 133, 47, true, false, true);
                int blipId = API.AddBlipForCoord(kvp.Value.X, kvp.Value.Y, kvp.Value.Z);
                API.SetBlipSprite(blipId, 133);
                //calloutBlips.Add(kvp.Key);
            }
            CitizenFX.Core.Debug.WriteLine($"Callout blips placed.");
        }

        [Command("reloadresource", Restricted = false)] //restriction default = true
        [EventHandler("reloadresource")]
        void reloadresource(int source, List<object> args, string raw)
        {
            //             CitizenFX.Core.Debug.WriteLine($"reloadresource triggered from developtools. Respawns: {displayRespawnBlips} Callouts: {displayCalloutBlips}");
            //             //TriggerEvent("clearRespawnBlips");
            //             //TriggerEvent("clearCalloutBlips");
            TriggerEvent("reloadResources", source);
            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Reloaded resources." } });

            //             //if (displayRespawnBlips) TriggerEvent("placeRespawnBlips");
            //             //if (displayCalloutBlips) TriggerEvent("placeCalloutBlips");
        }

        [Command("devblip", Restricted = false)] //restriction default = true
        void devBlip(int source, List<object> args, string raw)
        {
            TriggerEvent("placeRespawnBlips");
            TriggerEvent("placeCalloutBlips");
            //             // if (args.Count == 0)
            //             // {
            //             //     {
            //             //         TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Reloaded blips and resources." } });
            //             //     }
            //             //     TriggerEvent("reloadresource");
            //             // }
            //             // else if (args[0].ToString() == "callout" || args[0].ToString() == "respawn" || args[0].ToString() == "all" || args.Count == 0 || args[0].ToString() == "none")
            //             // {
            //             //     if ((args.Count == 0 && (displayCalloutBlips || displayRespawnBlips)) || args[0].ToString() == "none")
            //             //     {
            //             //         displayCalloutBlips = false;
            //             //         displayRespawnBlips = false;
            //             //         TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned off all dev blips" } });
            //             //     }
            //             //     else if ((args.Count == 0 && (!displayCalloutBlips || !displayRespawnBlips)) || args[0].ToString() == "all")
            //             //     {
            //             //         displayCalloutBlips = true;
            //             //         displayRespawnBlips = true;
            //             //         TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned on all dev blips" } });
            //             //     }
            //             //     else if (args[0].ToString() == "callout")
            //             //     {
            //             //         displayCalloutBlips = !displayCalloutBlips;
            //             //         if (!displayCalloutBlips) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned off callout dev blips" } });
            //             //         else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned on callout dev blips" } });
            //             //     }
            //             //     else if (args[0].ToString() == "respawn")
            //             //     {
            //             //         displayRespawnBlips = !displayRespawnBlips;
            //             //         if (!displayRespawnBlips) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned off respawn dev blips" } });
            //             //         else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned on respawn dev blips" } });
            //             //     }

            //             //     TriggerEvent("reloadresource");
            //             // }
            //             // else
            //             {
            //                 //CitizenFX.Core.Debug.WriteLine($"There was an issue with the command. Do /devblip (empty to toggle or all/none/callout/respawn/reload) It will toggle them.");
            //                 CitizenFX.Core.Debug.WriteLine($"This isn't working for now unfortunately.");
            //             }
        }
    }

    public class SavePositions : BaseScript
    {
        // Save callout location
        [Command("c", Restricted = false)] //restriction default = true
        private void CalloutCommand(int source, List<object> args, string raw)
        {
            string calloutName = string.Join(" ", args);

            if (ServerMain.maxzzzieCalloutsDict.ContainsKey(calloutName))
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"The name {calloutName} already exists in the callout resource." } });
            }
            else if (args.Count != 0)
            {
                TriggerClientEvent(Players[source], "givePedLocationAndHeadingForDevMode", source, calloutName, "callout");
            }
            else
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Usage: /c \"Name\"" } });
            }
        }
        // Save respawn location
        [Command("r", Restricted = false)] //restriction default = true
        private void RespawnCommand(int source, List<object> args, string raw)
        {
            string calloutName = string.Join(" ", args);
            if (ServerMain.respawnLocationsDict.ContainsKey(calloutName))
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"The name {calloutName} already exists in the respawn resource." } });
            }
            else if (args.Count != 0)
            {
                TriggerClientEvent(Players[source], "givePedLocationAndHeadingForDevMode", source, calloutName, "respawn");
            }
            else
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Usage: /r \"Name\"" } });
            }
        }

        [EventHandler("saveCallout")]
        private void saveCallout(int source, string locationName, Vector4 locationData) //locationdata vector4 w = heading. xyz = respective coords
        {
            // Prepare the line to append
            string line = $"{locationName}, {locationData.X}, {locationData.Y}, {locationData.Z}";

            // Define path
            var pathToResource = API.GetResourcePath(API.GetCurrentResourceName());
            var calloutFilePath = $"{pathToResource}/Resources/MaxzzzieCallouts.txt";

            try
            {
                File.AppendAllText(calloutFilePath, line + Environment.NewLine);
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Callout \"{locationName}\" saved successfully. ({locationData.X},{locationData.Y},{locationData.Z})" } });
                TriggerEvent("reloadresource");
            }
            catch (Exception ex)
            {
                CitizenFX.Core.Debug.WriteLine($"Error saving callout: {ex.Message}");
            }
        }

        // Save respawn location
        [EventHandler("saveRespawn")]
        private void saveRespawn(int source, string locationName, Vector4 locationData) //locationdata vector4 w = heading. xyz = respective coords
        {
            // Prepare the line to append with heading included
            string line = $"{locationName}, {locationData.X}, {locationData.Y}, {locationData.Z}, {locationData.W}";

            // Define path
            var pathToResource = API.GetResourcePath(API.GetCurrentResourceName());
            var respawnFilePath = $"{pathToResource}/Resources/RespawnLocations.txt";

            try
            {
                File.AppendAllText(respawnFilePath, line + Environment.NewLine);
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Respawn location \"{locationName}\" saved successfully. ({locationData.X},{locationData.Y},{locationData.Z}, {locationData.W})" } });
                TriggerEvent("reloadresource");
            }
            catch (Exception ex)
            {
                CitizenFX.Core.Debug.WriteLine($"Error saving respawn location: {ex.Message}");
            }
        }
    }
}

