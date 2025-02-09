using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Linq;


namespace STHMaxzzzie.Server
{
    public class Radio : BaseScript
    {
        public Radio()
        {
            TriggerClientEvent("AddPlayerToRadio", 69);
        }
        public static Dictionary<int, int> playerRadioDict = new Dictionary<int, int>();

        public static bool isRadioSetAutomatic = true;
        [Command("toggleradio", Restricted = false)]
        public void radioCommand(int source, List<object> args, string raw)
        {
            int channel;
            int playerId = source;
            if (args.Count == 2 && int.TryParse(args[0].ToString(), out playerId) && int.TryParse(args[1].ToString(), out channel))
            {
                if (playerId == 0) playerId = source;
                TriggerClientEvent(Players[playerId], "AddPlayerToRadio", channel);
                TriggerClientEvent(Players[source], "ShowNotification", $"Set {Players[playerId].Name}'s radio to channel: {channel}.");
            }
            else if (args.Count == 1 && int.TryParse(args[0].ToString(), out channel))
            {
                TriggerClientEvent(Players[playerId], "AddPlayerToRadio", channel);
                TriggerClientEvent(Players[source], "ShowNotification", $"Set your radio to channel: {channel}.");
            }
            else if (args.Count == 1 && args[0].ToString() == "all")
            {
                TriggerClientEvent("setRadioAutomatic", false);
                TriggerClientEvent("AddPlayerToRadio", 69);
                TriggerClientEvent(Players[source], "ShowNotification", $"Set everyones radio to the same channel.\n~r~Radio's are no longer set automatically.");
                isRadioSetAutomatic = false;
            }
            else if (args.Count == 2 && args[0].ToString() == "all" && int.TryParse(args[1].ToString(), out channel))
            {
                TriggerClientEvent("setRadioAutomatic", false);
                TriggerClientEvent("AddPlayerToRadio", channel);
                string text = $"Set everyones radio to the same channel.\nAnd radio's are not set automatically.";
                TriggerClientEvent(Players[source], "ShowNotification", text);
                isRadioSetAutomatic = false;
            }
            else if (args.Count == 2 && args[0].ToString() == "auto" && bool.TryParse(args[1].ToString(), out bool newAutomatic))
            {
                isRadioSetAutomatic = newAutomatic;
                TriggerClientEvent("setRadioAutomatic", isRadioSetAutomatic);
                if (!newAutomatic) TriggerClientEvent(Players[source], "ShowNotification", "~r~~h~Radio~s~\nRadio is now set manually.");
                else TriggerClientEvent(Players[source], "ShowNotification", "~r~~h~Radio~s~\nRadio is now set manually.");
            }
            else if (args.Count == 1 && args[0].ToString() == "auto")
            {
                isRadioSetAutomatic = !isRadioSetAutomatic;
                TriggerClientEvent("setRadioAutomatic", isRadioSetAutomatic);
                TriggerClientEvent(Players[source], "ShowNotification", $"~r~~h~Radio~s~\nDo radio's set automatically: {isRadioSetAutomatic}.");
            }
            else if (args.Count == 1 && args[0].ToString() == "get")
            {
                DisplayClientChannelList(source);
            }
            else
            {
                TriggerClientEvent(Players[source], "ShowNotification", "~h~~r~/toggleradio help~s~\nSee the client console (f8) for info.");
                TriggerClientEvent(Players[source], "displayClientDebugLine", "{Id} {channel}: sets the channel for player with Id.\n{channel}: sets the channel for you.\n\"auto\" (true/false optional): game handling takes control of setting and changing the radio again.\n\"all\" {channel}: sets the channel for everyone. Channel is optional, default will set to 80085.\n\"get\": Displays current radio channels of all players.");
            }
        }

        public async void DisplayClientChannelList(int source)
        {
            playerRadioDict.Clear();
            TriggerClientEvent("getClientRadioChannel");
            while (Players.Count() > playerRadioDict.Count)
            {
                await Delay(50);
            }
            string text = $"Clients radio channel status = {string.Join(", ", playerRadioDict.Select(kvp => $"{Players[kvp.Key].Name}{kvp.Key} - {kvp.Value}"))}";
            TriggerClientEvent(Players[source], "ShowNotification", $"~r~Clients radio channel status is shown in client console(~s~f8~r~).");
            TriggerClientEvent(Players[source], "displayClientDebugLine", text);
        }

        [EventHandler("updateServerRadioList")]
        void populatePlayerRadioDict(int ServerId, int radioChannel)
        {
            playerRadioDict[ServerId] = radioChannel;
        }
    }
}