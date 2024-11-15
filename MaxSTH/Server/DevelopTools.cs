using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.IO;

namespace STHMaxzzzie.Server
{
    public class DevelopTools : BaseScript
    {
        bool displayRespawnBlips = false;
        bool displayCalloutBlips = false;

        public DevelopTools()
        {
            
        }

        public async void setBlips()
        {
            if (displayRespawnBlips)
            {
                placeRespawnBlips();
            }
            await Delay(150);
            if (displayCalloutBlips)
            {
                placeCalloutBlips();
            }
            await Delay(150);
            if (!displayRespawnBlips)
            {
                clearRespawnBlips();
            }
            await Delay(150);
            if (!displayCalloutBlips)
            {
                clearCalloutBlips();
            }  
        }

        [EventHandler("clearRespawnBlips")]
        void clearRespawnBlips()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            foreach (var kvp in ServerMain.respawnLocationsDict)
            {
                request.BlipsToRemove.Add($"respawn:{kvp.Key}");
            }
            BlipHandler.AddBlips(request);
            //CitizenFX.Core.Debug.WriteLine($"Respawn blips cleared.");
        }

        [EventHandler("placeRespawnBlips")]
        void placeRespawnBlips()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            foreach (var kvp in ServerMain.respawnLocationsDict)
            {
                string name = kvp.Key;
                BlipHandler.BlipData blip = new BlipHandler.BlipData($"respawn:{name}")
                {
                    Coords = new Vector3(kvp.Value.X, kvp.Value.Y, kvp.Value.Z),
                    Sprite = 84,
                    IsShortRange = true,
                    MapName = $"Respawn"
                };
                request.BlipsToAdd.Add(blip);
            }
            BlipHandler.AddBlips(request);
        }

        [EventHandler("clearCalloutBlips")]
        void clearCalloutBlips()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            foreach (var kvp in ServerMain.maxzzzieCalloutsDict)
            {
                request.BlipsToRemove.Add($"callout:{kvp.Key}");
            }
            BlipHandler.AddBlips(request);

            //CitizenFX.Core.Debug.WriteLine($"Callout blips cleared.");
        }

        [EventHandler("placeCalloutBlips")]
        void placeCalloutBlips()
        {
            //Debug.WriteLine($"placeCallutblips with {ServerMain.maxzzzieCalloutsDict.Count}");
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            foreach (var kvp in ServerMain.maxzzzieCalloutsDict)
            {
                string name = kvp.Key;
                BlipHandler.BlipData blip = new BlipHandler.BlipData($"callout:{name}")
                {
                    Coords = new Vector3(kvp.Value.X, kvp.Value.Y, kvp.Value.Z),
                    Sprite = 133,
                    IsShortRange = true,
                    MapName = $"Callout"
                };
                request.BlipsToAdd.Add(blip);
            }
            BlipHandler.AddBlips(request);
            //CitizenFX.Core.Debug.WriteLine($"Callout blips placed.");
        }

        [Command("reloadresource", Restricted = true)] //restriction default = true
        [EventHandler("reloadresource")]
        void reloadresource(int source, List<object> args, string raw)
        {
            TriggerEvent("reloadResources", source);
            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Reloaded resources." } });
        }

        [Command("devblip", Restricted = true)] //restriction default = true
        void devBlip(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                if (displayCalloutBlips || displayRespawnBlips)
                {
                    displayCalloutBlips = false;
                    displayRespawnBlips = false;
                }
                else
                {
                    displayCalloutBlips = true;
                    displayRespawnBlips = true;
                }
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
            }
            else
            {
                //CitizenFX.Core.Debug.WriteLine($"There was an issue with the command. Do /devblip (empty to toggle or all/none/callout/respawn/reload) It will toggle them.");
                CitizenFX.Core.Debug.WriteLine($"This isn't working for now unfortunately.");
            }
            setBlips();
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
        [Command("r", Restricted = true)] //restriction default = true
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

