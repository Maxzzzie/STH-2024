using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CitizenFX;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Client
{
    public class DelayMode : BaseScript
    {
        public static Player runPlayer;
        public static bool delayModeOn = false;
        public static int distanceToBlip = 400;
        public static bool runnerSeesDelayBlip = false;
        int delayBlipHandle = 0;

        [Command("delaymode", Restricted = true)] //normal restriction true 
        [EventHandler("delaymode")] //delaymode (playerID) (optional: distance) (optional: runner sees blip)
        public void delayMode(int source, List<object> args, string raw)
        {
            Player sourceHost = Players[source];
            if (args.Count == 1 && args[0].ToString() == "false" || args[0].ToString() == "true")
            {
                if (args[0].ToString() == "true")
                {
                    TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /delaymode (playerID) (optional: distance) (optional: runner sees blip)." } });
                }
                else
                {
                    delayModeOn = false;
                    TriggerClientEvent("updateBlipLocationOnMapForDelayMode", new Vector3(0, 0, 0), delayModeOn);
                }
            }
            else if (args.Count == 1)
            {
                runPlayer = Players[int.Parse(args[0].ToString())];
                delayModeOn = true;
                runnerSeesDelayBlip = false;
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"{sourceHost.Name} started a round of delay mode with {distanceToBlip}m distance. {runPlayer.Name} is the runner." } });
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 2)
            {
                runPlayer = Players[int.Parse(args[0].ToString())];
                distanceToBlip = int.Parse(args[1].ToString());
                runnerSeesDelayBlip = false;
                delayModeOn = true;
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"{sourceHost.Name} started a round of delay mode with {distanceToBlip}m distance. {runPlayer.Name} is the runner." } });
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 3)
            {
                bool isBool = bool.TryParse(args[2].ToString(), out isBool);
                if (isBool)
                {
                    runnerSeesDelayBlip = bool.Parse(args[2].ToString());
                    runPlayer = Players[int.Parse(args[0].ToString())];
                    distanceToBlip = int.Parse(args[1].ToString());
                    delayModeOn = true;
                    TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
                    if (runnerSeesDelayBlip)
                        TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"{sourceHost.Name} started a round of delay mode with {distanceToBlip}m distance. {runPlayer.Name} is the runner and does see their blip." } });
                    else
                        TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"{sourceHost.Name} started a round of delay mode with {distanceToBlip}m distance. {runPlayer.Name} is the runner." } });

                }
                else
                    TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /delaymode (playerID) (optional: distance) (optional: runner sees blip)." } });

            }
            else
            {
                TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /delaymode (playerID) (optional: distance) (optional: runner sees blip)." } });
            }
        }

        [EventHandler("updateDelayBlip")]
        public void updateDelayBlip(Vector3 newBlipPos, bool isDelayModeOn)
        {
            delayModeOn = isDelayModeOn;

            if (runnerSeesDelayBlip && isDelayModeOn)
            {
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos, delayModeOn);
            }
            else if (isDelayModeOn)
            {
                foreach (Player player in Players)
                {
                    int playerId = int.Parse(player.Handle);
                    if (playerId != int.Parse(runPlayer.Handle))
                    {
                        TriggerClientEvent(player, "updateBlipLocationOnMapForDelayMode", newBlipPos, delayModeOn);
                    }
                }
            }
            else //when delay mode is off
            {
                delayModeOn = false;
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos, delayModeOn);
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Delay mode is now concluded." } });
            }
        }
    }
}