using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CitizenFX;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class DelayMode : BaseScript
    {
        public static Player runPlayer;
        public static bool delayModeOn = false;
        public static int distanceToBlip = 400;
        public static bool runnerSeesDelayBlip = true;
        static int highSpeedSpeed = 10;
        static int highSpeedBlipDistanceSubtraction = 10;
        static int highSpeedBlipDistanceAddition = 10;
        static int highSpeedBlipTimeSubtractTrigger = 60;
        static int highSpeedBlipTimeAddTrigger = 45;
        static int highSpeedBlipTimeAdd = 2;
        static int highSpeedBlipTimeSubtract = 1;
        static int highSpeedBlipMinimumDistance = 80;

        [Command("dms", Restricted = false)] //normal restriction true 
        void delayModeSettings(int source, List<object> args, string raw)
        {
            if (args.Count == 2)
            {
                string set = args[0].ToString();
                int value = int.Parse(args[1].ToString());

                if (set == "speed") highSpeedSpeed = value;
                else if (set == "distsub") highSpeedBlipDistanceSubtraction = value;
                else if (set == "distadd") highSpeedBlipDistanceAddition = value;
                else if (set == "timeaddtrig") highSpeedBlipTimeAddTrigger = value;
                else if (set == "timesubtrig") highSpeedBlipTimeSubtractTrigger = value;
                else if (set == "timeadd") highSpeedBlipTimeAdd = value;
                else if (set == "timesub") highSpeedBlipTimeSubtract = value;
                else if (set == "mindist") highSpeedBlipMinimumDistance = value;
            }
            else if (args.Count == 1 && args[0].ToString() == "reset")
            {
                highSpeedSpeed = 10;
                highSpeedBlipDistanceSubtraction = 10;
                highSpeedBlipDistanceAddition = 10;
                highSpeedBlipTimeAddTrigger = 60;
                highSpeedBlipTimeSubtractTrigger = 45;
                highSpeedBlipTimeAdd = 2;
                highSpeedBlipTimeSubtract = 1;
                highSpeedBlipMinimumDistance = 80;
            }

            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"speed = {highSpeedSpeed}. distsub = {highSpeedBlipDistanceSubtraction}. distadd = {highSpeedBlipDistanceAddition}.\ntimeaddtrig = {highSpeedBlipTimeAddTrigger}. timesubtrig = {highSpeedBlipTimeSubtractTrigger}. timeadd = {highSpeedBlipTimeAdd}.\ntimesub = {highSpeedBlipTimeSubtract}. mindist = {highSpeedBlipMinimumDistance}" } });
            Debug.WriteLine($"speed = {highSpeedSpeed}. distsub = {highSpeedBlipDistanceSubtraction}. distadd = {highSpeedBlipDistanceAddition}.\ntimeaddtrig = {highSpeedBlipTimeAddTrigger}. timesubtrig = {highSpeedBlipTimeSubtractTrigger}. timeadd = {highSpeedBlipTimeAdd}.\ntimesub = {highSpeedBlipTimeSubtract}. mindist = {highSpeedBlipMinimumDistance}");
            updateClientsDelayModeSettings();
        }

        public static void updateClientsDelayModeSettings()
        {
            TriggerClientEvent("updateDelayModeSettings", highSpeedSpeed, highSpeedBlipDistanceSubtraction, highSpeedBlipDistanceAddition, highSpeedBlipTimeAddTrigger, highSpeedBlipTimeSubtractTrigger, highSpeedBlipTimeAdd, highSpeedBlipTimeSubtract, highSpeedBlipMinimumDistance);
        }


        //[Command("delaymode", Restricted = true)] //normal restriction true 
        //[EventHandler("delaymode")] //delaymode (playerID) (optional: distance) (optional: runner sees blip)
        public static void delayMode(Player sourceHost, Player runPlayer, List<object> args)
        {
            // Debug.WriteLine($"server delayMode {sourceHost.Name} {runPlayer.Name} {args.Count} ");
            int temp = 0;
            if (args.Count == 2 && int.TryParse(args[1].ToString(), out temp) == true)
            {
                // Debug.WriteLine($"server delayMode 2args ");
                delayModeOn = true;
                runnerSeesDelayBlip = false;
                TriggerEvent("startGame", "delay", int.Parse(args[1].ToString()));
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 3 && int.TryParse(args[1].ToString(), out temp) == true && int.TryParse(args[2].ToString(), out temp) == true)
            {
                // Debug.WriteLine($"server delayMode 3args ");
                distanceToBlip = int.Parse(args[2].ToString());
                runnerSeesDelayBlip = false;
                delayModeOn = true;
                TriggerEvent("startGame", "delay", int.Parse(args[1].ToString()));
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 4)
            {
                // Debug.WriteLine($"server delayMode 4args ");
                bool isBool = bool.TryParse(args[3].ToString(), out isBool);
                if (isBool)
                {
                    // Debug.WriteLine($"server delayMode 4args is bool {runPlayer}");
                    runnerSeesDelayBlip = bool.Parse(args[3].ToString());
                    distanceToBlip = int.Parse(args[2].ToString());
                    delayModeOn = true;
                    TriggerEvent("startGame", "delay", int.Parse(args[1].ToString()));
                    TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
                }
                else
                {
                    // Debug.WriteLine($"server delayMode 4args else");
                    TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /start delay (playerID) (optional: distance) (optional: runner sees blip)." } });
                }
            }
            else
            {
                // Debug.WriteLine($"server delayMode else ");
                TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /start delay (playerID) (optional: distance) (optional: runner sees blip)." } });
            }
            DelayMode.setOrRemoveDistanceBlipsForDelayMode();
        }

        [EventHandler("updateDelayBlip")]
        public void updateDelayBlip(Vector3 newBlipPos, bool isDelayModeOn)
        {
            // Debug.WriteLine($"server updateDelayBlip {newBlipPos.X},{newBlipPos.Y},{newBlipPos.Z} {isDelayModeOn}");
            delayModeOn = isDelayModeOn;

            if (runnerSeesDelayBlip && isDelayModeOn)
            {
                // Debug.WriteLine($"server updateDelayBlip {runnerSeesDelayBlip} {isDelayModeOn}");
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos);
            }
            else if (isDelayModeOn)
            {
                // Debug.WriteLine($"server updateDelayBlip {isDelayModeOn}");

                if (runPlayer == null || string.IsNullOrEmpty(runPlayer.Handle))
                {
                    Debug.WriteLine("Error: runPlayer is null or has an invalid Handle.");
                    return; // Handle the error as needed.
                }

                foreach (Player player in Players)
                {
                    if (player == null || string.IsNullOrEmpty(player.Handle))
                    {
                        Debug.WriteLine("Warning: A player is null or has an invalid Handle.");
                        continue; // Skip this player.
                    }

                    int playerId = int.Parse(player.Handle);
                    if (playerId != int.Parse(runPlayer.Handle))
                    {
                        TriggerClientEvent(player, "updateBlipLocationOnMapForDelayMode", newBlipPos);
                    }
                }
            }
            else // when delay mode is off
            {
                // Debug.WriteLine($"server updateDelayBlip else, endgame, hunter");
                delayModeOn = false;
                TriggerEvent("endGame", "hunter");
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos);
                setOrRemoveDistanceBlipsForDelayMode();
            }
        }
        public static void setOrRemoveDistanceBlipsForDelayMode()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            if (delayModeOn)
            {
                BlipHandler.BlipData delayModeCue1 = new BlipHandler.BlipData("delayModeCue2")
                {
                    Coords = new Vector3(-2500, -500, 0),
                    Sprite = 503,
                    IsShortRange = true,
                    Colour = 51,
                    MapName = "delay mode distance"
                };
                BlipHandler.BlipData delayModeCue2 = new BlipHandler.BlipData("delayModeCue1")
                {
                    Coords = new Vector3(-2500, -500 - distanceToBlip, 0),
                    Sprite = 502,
                    IsShortRange = true,
                    Colour = 51,
                    MapName = "delay mode distance"
                };
                request.BlipsToAdd.Add(delayModeCue1);
                request.BlipsToAdd.Add(delayModeCue2);
            }
            else
            {
                request.BlipsToRemove.Add("delayModeCue1");
                request.BlipsToRemove.Add("delayModeCue2");
            }
            BlipHandler.AddBlips(request);
        }
    }
}