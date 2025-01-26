using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class Radio : BaseScript
    {
        int radioChannel = 1;
        public static bool setsAutomatically = true;
        public Radio()
        {
            //AddPlayerToRadio(1);
        }


        [EventHandler("AddPlayerToRadio")]
        public void AddPlayerToRadio(int channel)
        {
            radioChannel = channel;
            dynamic exports = Exports;
            exports["pma-voice"].setRadioChannel(radioChannel);
        }


        [EventHandler("setRadioAutomatic")]
        public void setRadioAutomatic(bool newSetsAutomatically)
        {
            setsAutomatically = newSetsAutomatically;
        }
    }
}