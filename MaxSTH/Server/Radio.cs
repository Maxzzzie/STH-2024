using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;


namespace STHMaxzzzie.Server
{
    public class Radio : BaseScript
    {
            public static bool isRadioSetAutomatic = true;
         [Command("toggleradio", Restricted = true)]
        public void radioCommand(int source, List<object> args, string raw)
        {
            int channel;
            int playerId = source;
            if(args.Count == 2 && int.TryParse(args[0].ToString(), out playerId) && int.TryParse(args[1].ToString(), out channel))
            {
                TriggerClientEvent(Players[playerId], "AddPlayerToRadio", channel);
                string text = $"Set {Players[playerId].Name}'s radio to channel: {channel}.";
                TriggerClientEvent(Players[source], "ShowNotification", text);
            } 
            else if (args.Count == 1 && int.TryParse(args[0].ToString(), out channel))
            {
                TriggerClientEvent(Players[playerId], "AddPlayerToRadio", channel);
                string text = $"Set your radio to channel: {channel}.";
                TriggerClientEvent(Players[source], "ShowNotification", text);
            }
            else if (args.Count == 1 && args[0].ToString() == "all")
            {
                TriggerClientEvent("AddPlayerToRadio", 80085);
                TriggerClientEvent("setRadioAutomatic", false);
                string text = $"Set everyones radio to the same channel.\nAnd radio's are not set automatically.";
                TriggerClientEvent(Players[source], "ShowNotification", text);
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
            }
            else 
            {
                TriggerClientEvent(Players[source], "ShowNotification", "~h~~r~Radio help~s~\nDo /radio {playerId} {channelNo}\nOr /radio auto (true/false is optional)\nOr /radio all (channel)");
            }
        }
    }
    }