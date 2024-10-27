using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.IO;

namespace STHMaxzzzie.Server
{
    public class DevelopTools : BaseScript
    {
        static List<int> respawnBlips = new List<int>();
        static List<int> calloutBlips = new List<int>();
        bool displayRespawnBlips = false;
        bool displayCalloutBlips = false;

        public DevelopTools()
        {
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"This is the develop build. Develop tools are on!" } });
        }

        [EventHandler("clearRespawnBlips")]
        void clearRespawnBlips()
        {
            foreach (int blipId in respawnBlips)
            {
                int blip = blipId; //for some reason i have to specify again it's an int. It doesn't like the one in the foreach loop.
                API.RemoveBlip(ref blip);
                //TriggerClientEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"foreach respawn {blip}"}});
                CitizenFX.Core.Debug.WriteLine($"Clearing respawn blip ID: {blipId}.");
            }
            CitizenFX.Core.Debug.WriteLine($"Respawn blips cleared.");
            respawnBlips.Clear();
        }

        [EventHandler("placeRespawnBlips")]
        void placeRespawnBlips()
        {
            foreach (var entry in ServerMain.respawnLocationsDict)
            {
                int blipId = API.AddBlipForCoord(entry.Value.X, entry.Value.Y, entry.Value.Z);
                API.SetBlipSprite(blipId, 84);
                respawnBlips.Add(blipId);
                CitizenFX.Core.Debug.WriteLine($"Respawn blip placed ID: {blipId}.");
            }
            CitizenFX.Core.Debug.WriteLine($"Respawn blips placed.");
        }

        [EventHandler("clearCalloutBlips")]
        void clearCalloutBlips()
        {
            foreach (int blipId in calloutBlips)
            {
                int blip = blipId; //for some reason i have to specify again it's an int. It doesn't like the one in the foreach loop.
                API.RemoveBlip(ref blip);
                CitizenFX.Core.Debug.WriteLine($"Clearing callout blip ID: {blipId}.");
            }
            CitizenFX.Core.Debug.WriteLine($"Callout blips cleared.");
            calloutBlips.Clear();
        }

        [EventHandler("placeCalloutBlips")]
        void placeCalloutBlips()
        {
            foreach (var entry in ServerMain.maxzzzieCalloutsDict)
            {
                int blipId = API.AddBlipForCoord(entry.Value.X, entry.Value.Y, entry.Value.Z);
                API.SetBlipSprite(blipId, 205);
                calloutBlips.Add(blipId);
                CitizenFX.Core.Debug.WriteLine($"Callout blip placed ID: {blipId}.");
            }
            CitizenFX.Core.Debug.WriteLine($"Callout blips placed.");
        }

        //[Command("reloadblips", Restricted = false)] //restriction default = true
        [EventHandler("reloadBlips")]
        void reloadBlips(int source, List<object> args, string raw)
        {

            CitizenFX.Core.Debug.WriteLine($"reloadBlips triggered from developtools. Respawns: {displayRespawnBlips} Callouts: {displayCalloutBlips}");
            TriggerEvent("clearRespawnBlips");
            TriggerEvent("clearCalloutBlips");
            TriggerEvent("reloadResources");
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Reloaded resources." } });
            //await Delay(500)
            if (displayRespawnBlips) TriggerEvent("placeRespawnBlips");
            if (displayCalloutBlips) TriggerEvent("placeCalloutBlips");
        }

        [Command("devblip", Restricted = false)] //restriction default = true
        void devBlip(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Reloaded blips and resources." } });
                }
                TriggerEvent("reloadBlips");
            }
            else if (args[0].ToString() == "callout" || args[0].ToString() == "respawn" || args[0].ToString() == "all" || args.Count == 0 || args[0].ToString() == "none")
            {
                if ((args.Count == 0 && (displayCalloutBlips || displayRespawnBlips)) || args[0].ToString() == "none")
                {
                    displayCalloutBlips = false;
                    displayRespawnBlips = false;
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned off all dev blips" } });
                }
                else if ((args.Count == 0 && (!displayCalloutBlips || !displayRespawnBlips)) || args[0].ToString() == "all")
                {
                    displayCalloutBlips = true;
                    displayRespawnBlips = true;
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned on all dev blips" } });
                }
                else if (args[0].ToString() == "callout")
                {
                    displayCalloutBlips = !displayCalloutBlips;
                    if (!displayCalloutBlips) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned off callout dev blips" } });
                    else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned on callout dev blips" } });
                }
                else if (args[0].ToString() == "respawn")
                {
                    displayRespawnBlips = !displayRespawnBlips;
                    if (!displayRespawnBlips) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned off respawn dev blips" } });
                    else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Turned on respawn dev blips" } });
                }

                TriggerEvent("reloadBlips");
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine($"There was an issue with the command. Do /devblip (empty to toggle or all/none/callout/respawn/reload) It will toggle them.");

            }
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
            TriggerEvent("reloadBlips");
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
                TriggerEvent("reloadBlips");
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
                TriggerEvent("reloadBlips");
            }
            catch (Exception ex)
            {
                CitizenFX.Core.Debug.WriteLine($"Error saving respawn location: {ex.Message}");
            }
        }
    }
}

