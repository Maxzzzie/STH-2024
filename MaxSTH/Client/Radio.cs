using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class Radio : BaseScript
    {
        int radioChannel = 69;
        public static bool setsAutomatically = true;
        static int serverId = Game.Player.ServerId;
        public Radio()
        {
            
        }

        [EventHandler("AddPlayerToRadio")]
        public void AddPlayerToRadio(int channel)
        {
            radioChannel = channel;
            dynamic exports = Exports;
            exports["pma-voice"].setRadioChannel(radioChannel);
            Debug.WriteLine($"Radio channel is now set to {radioChannel}.");
            TriggerServerEvent("updateServerRadioList", serverId, radioChannel);
        }

        async void UpdateRadio()
        {
            while (RoundHandling.teamAssignment.Count == 0 && RoundHandling.teamAssignment.ContainsKey(serverId))
            {
                await Delay(1000);
            }

            if (setsAutomatically)
            {
                int channel = RoundHandling.teamAssignment[serverId];
                if (channel == 0) channel = 69;
                AddPlayerToRadio(channel);
                radioChannel = channel;
            }
            else AddPlayerToRadio(radioChannel);
        }


        [EventHandler("setRadioAutomatic")]
        public void setRadioAutomatic(bool newSetsAutomatically)
        {
            setsAutomatically = newSetsAutomatically;
            if (setsAutomatically) UpdateRadio();
        }

        [EventHandler("getClientRadioChannel")]
        public void getClientRadioChannel()
        {
            TriggerServerEvent("updateServerRadioList", serverId, radioChannel);
        }
    }
}