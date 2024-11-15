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
            // Debug.WriteLine($"server delayMode {sourceHost.Name} {runPlayer.Name} {args.Count} ");
            int temp = 0;
            if (args.Count == 2 && int.TryParse(args[1].ToString(), out temp) == true)
            {   
            // Debug.WriteLine($"server delayMode 2args ");
                delayModeOn = true;
                runnerSeesDelayBlip = false;
                TriggerClientEvent(runPlayer, "getBlipLocationForDelayMode", delayModeOn, distanceToBlip);
            }
            else if (args.Count == 3 && int.TryParse(args[1].ToString(), out temp) == true && int.TryParse(args[2].ToString(), out temp) == true)
            {
                // Debug.WriteLine($"server delayMode 3args ");
                distanceToBlip = int.Parse(args[2].ToString());
                runnerSeesDelayBlip = false;
                delayModeOn = true;
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
        }

        // [EventHandler("updateDelayBlip")]
        // public void updateDelayBlip(Vector3 newBlipPos, bool isDelayModeOn)
        // {
        //     Debug.WriteLine($"server updateDelayBlip {newBlipPos.X},{newBlipPos.Y},{newBlipPos.Z} {isDelayModeOn}");
        //     delayModeOn = isDelayModeOn;

        //     if (runnerSeesDelayBlip && isDelayModeOn)
        //     {
        //         Debug.WriteLine($"server updateDelayBlip {runnerSeesDelayBlip} {isDelayModeOn}");
        //         TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos);
        //     }
        //     else if (isDelayModeOn)
        //     { Debug.WriteLine($"server updateDelayBlip {isDelayModeOn}");
        //         foreach (Player player in Players)
        //         {
        //             int playerId = int.Parse(player.Handle);
        //             if (playerId != int.Parse(runPlayer.Handle))
        //             {
        //                 TriggerClientEvent(player, "updateBlipLocationOnMapForDelayMode", newBlipPos);
        //             }
        //         }
        //     }
        //     else //when delay mode is off
        //     { 
        //         {
        //         Debug.WriteLine($"server updateDelayBlip else, endgame, hunter");}
        //         delayModeOn = false;
        //         TriggerEvent("endGame", "hunter");
        //         TriggerClientEvent("updateBlipLocationOnMapForDelayMode", newBlipPos);
        //     }
        // }
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
    }
}
    }
}