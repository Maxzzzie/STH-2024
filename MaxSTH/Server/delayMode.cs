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
        public static bool runnerSeesDelayBlip = false;
        int delayBlipHandle = 0;

        //[Command("delaymode", Restricted = true)] //normal restriction true 
        //[EventHandler("delaymode")] //delaymode (playerID) (optional: distance) (optional: runner sees blip)
        public static void delayMode(Player sourceHost, Player runPlayer, List<object> args)
        {
            int temp = 0;
            if (args.Count == 2 && int.TryParse(args[1].ToString(), out temp) == true)
            {
                delayModeOn = true;
                runnerSeesDelayBlip = false;
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 3 && int.TryParse(args[1].ToString(), out temp) == true && int.TryParse(args[2].ToString(), out temp) == true)
            {
                distanceToBlip = int.Parse(args[2].ToString());
                runnerSeesDelayBlip = false;
                delayModeOn = true;
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 4)
            {
                bool isBool = bool.TryParse(args[3].ToString(), out isBool);
                if (isBool)
                {
                    runnerSeesDelayBlip = bool.Parse(args[3].ToString());
                    distanceToBlip = int.Parse(args[2].ToString());
                    delayModeOn = true;
                    TriggerEvent("startGame", "delay", int.Parse(args[1].ToString()));
                    TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
                }
                else
                {
                    TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /start delay (playerID) (optional: distance) (optional: runner sees blip)." } });
                }
            }
            else
            {
                TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong, start a delay run with /start delay (playerID) (optional: distance) (optional: runner sees blip)." } });
            }
        }

        [EventHandler("updateDelayBlip")]
        public void updateDelayBlip(Vector3 newBlipPos, bool isDelayModeOn)
        {
            delayModeOn = isDelayModeOn;

            if (runnerSeesDelayBlip && isDelayModeOn)
            {
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos);
            }
            else if (isDelayModeOn)
            {
                foreach (Player player in Players)
                {
                    int playerId = int.Parse(player.Handle);
                    if (playerId != int.Parse(runPlayer.Handle))
                    {
                        TriggerClientEvent(player, "updateBlipLocationOnMapForDelayMode", newBlipPos);
                    }
                }
            }
            else //when delay mode is off
            {
                delayModeOn = false;
                TriggerEvent("endGame", "hunter");
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos);
            }
        }
    }
}