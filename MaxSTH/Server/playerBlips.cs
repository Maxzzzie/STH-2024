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
        static public string playerBlipSetting = "on";
        public static List<int> showThisBlip = new List<int>();

        public PlayerBlips()
        {
            GetPlayersInfo();
        }

        [EventHandler("Server:updatePlayerBlipSettings")]
        static void UpdatePlayerBlipSettings()
        {
            string showThisBlipString = string.Join(",", showThisBlip);
            Debug.WriteLine($"updatePlayerBlipSettings {showThisBlipString} + {playerBlipSetting}");
            TriggerClientEvent("updatePlayerBlipSettings", playerBlipSetting, showThisBlipString);
        }


        //[EventHandler("updatePlayerBlips")]
        [Command("togglepb", Restricted = true)] //normal restriction true 
        public void playerBlipHandling(int source, List<object> args, string raw)
        {   //turns on blips for teammates (default)
            if ((args.Count == 1 && (args[0].ToString() == "true" || args[0].ToString() == "on")) || (args.Count == 0 && playerBlipSetting != "on"))
            {
                playerBlipSetting = "on";
                TriggerClientEvent("ShowNotification", $"Player blips are now: \"{playerBlipSetting}\".");
            }
            //turns off all blips (with no input. Defaults to on first)
            else if ((args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "off")) || (args.Count == 0 && playerBlipSetting == "on"))
            {
                playerBlipSetting = "off";
                TriggerClientEvent("ShowNotification", $"Player blips are now: \"{playerBlipSetting}\".");
            }
            //shows blips of people who are in showThisBlip list.
            else if (args.Count == 1 && args[0].ToString() == "custom")
            {
                playerBlipSetting = "custom";
                TriggerClientEvent("ShowNotification", $"Player blips are now: {playerBlipSetting}.");
                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nFor more info see console(f8).");
                TriggerClientEvent("displayClientDebugLine", "---   ---   ---   togglepb settings   ---   ---   ---\n/togglepb on - turns on blips for teammates (default)\n/togglepb off - turns off blips for everyone.\n/togglepb custom - turns on a specific set with blips.\n\nWhen set to custom:\n/togglepb set {playerId} {showBool} - Adds or removes blip to showThisBlip list, bool is optional.\n/togglepb show {playerId} - Adds player to the list.\n/togglepb hide {playerId} - Removes player from the list.\n/togglepb clear - Clears the list.\n/togglepb all - Adds everyone online to the list.\n---   ---   ---   togglepb settings   ---   ---   ---");
            }
            
            else if (args.Count == 3 && args[0].ToString() == "set" && int.TryParse(args[1].ToString(), out int playerId) && Players[playerId] != null && bool.TryParse(args[2].ToString(), out bool addOrRemove))
            {
                if (addOrRemove && !showThisBlip.Contains(playerId)) showThisBlip.Add(playerId);
                else if (!addOrRemove && showThisBlip.Contains(playerId)) showThisBlip.Remove(playerId);

                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nIf pb is set to \"Custom\"");
            }
            //adds blip to showThisBlip list /togglepb show {playerId}.
            else if (args.Count == 2 && args[0].ToString() == "show" && int.TryParse(args[1].ToString(), out playerId) && Players[playerId] != null)
            {
                if (!showThisBlip.Contains(playerId)) showThisBlip.Add(playerId);
                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nIf pb is set to \"Custom\"");
            }
            //removes blip from showThisBlip list /togglepb hide {playerId}.
            else if (args.Count == 2 && args[0].ToString() == "hide" && int.TryParse(args[1].ToString(), out playerId) && Players[playerId] != null)
            {
                if (showThisBlip.Contains(playerId)) showThisBlip.Remove(playerId);
                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nIf pb is set to \"Custom\"");
            }
            //toggles player blip from showThisBlip list /togglepb set {playerId}.
            else if (args.Count == 2 && args[0].ToString() == "set" && int.TryParse(args[1].ToString(), out playerId))
            {
                if (!showThisBlip.Contains(playerId)) showThisBlip.Add(playerId);
                else showThisBlip.Remove(playerId);
                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nIf pb is set to \"Custom\"");
            }
            //clears all player blips from showThisBlip list /togglepb clear.
            else if (args.Count == 1 && args[0].ToString() == "clear")
            {
                showThisBlip.Clear();
                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nIf pb is set to \"Custom\"");
            }
            //adds all player blips to showThisBlip list /togglepb all.
            else if (args.Count == 1 && args[0].ToString() == "all")
            {
                showThisBlip.Clear();
                foreach (Player player in Players) showThisBlip.Add(int.Parse(player.Handle));
                
                TriggerClientEvent(Players[source], "ShowNotification", $"Showing blips for {string.Join(", ", showThisBlip)}.\nIf pb is set to \"Custom\"");
            }
            UpdatePlayerBlipSettings();
        }

        private async void GetPlayersInfo()
        {
            while (true)
            {
                //Debug.WriteLine($"GPI running");
                
                List<string> playerBlipList = new List<string>();

                foreach (Player player in Players)
                {
                    if (player == null)
                    {
                        Debug.WriteLine($"Player {player.Name} has not loaded yet.");
                        continue;
                    }
                    if (player.Character == null)
                    {
                        Debug.WriteLine($"Player {player.Name} has no character loaded yet.");
                        continue;
                    }
                    string playerId = player.Handle;
                    string playerName = player.Name;
                    string positionString = player.Character != null ? $"{player.Character.Position.X},{player.Character.Position.Y},{player.Character.Position.Z},{player.Character.Heading}" : "0,0,0,0";

                    
                    string playerInfo = $"{playerName},{playerId},{positionString}";
                    playerBlipList.Add(playerInfo);
                    //Debug.WriteLine($"pb add {playerInfo}");
                }


                TriggerClientEvent("setPlayerBlips", playerBlipList);
                //Debug.WriteLine($"GPI done");
                await Delay(200);
            }
        }
    }
}
