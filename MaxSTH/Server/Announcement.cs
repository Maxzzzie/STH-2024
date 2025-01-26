using System;
using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;

namespace STHMaxzzzie.Server
{
    public class Announcements : BaseScript
    {
        [Command("announce", Restricted = true)] // Restriction (default true)
        void announce(int source, List<object> args, string raw)
        {
            if (args.Count >= 3 && args[0].ToString() == "id" && Int32.TryParse(args[1].ToString(), out int target))
            {
                string message = string.Join(" ", args.Skip(2));
                TriggerClientEvent(Players[target], "ShowNotification", message);
            }
            else if (args.Count >= 3 && args[0].ToString() == "fake" && Int32.TryParse(args[1].ToString(), out int chatSource))
            {
                string message = string.Join(" ", args.Skip(2));
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { Players[chatSource].Name, message } });
            }
            else if (args.Count >= 2 && args[0].ToString() == "all")
            {
                string message = string.Join(" ", args.Skip(1));
                TriggerClientEvent("ShowNotification", message);
            }
            else if (args.Count > 0 && args[0].ToString() == "motd")
            {
                TriggerClientEvent("ShowMotD");
            }
            else if (args.Count > 0 && args[0].ToString() == "help")
            {
                
                TriggerClientEvent(Players[source], "ShowNotification", "Usage: /announce id [targetID] [message]\nOr /announce all [message]");
            }
            else
            {
                TriggerClientEvent(Players[source], "ShowNotification", "~r~Invalid usage. Type /announce help for more info.");
            }
        }
        [EventHandler("notifyEveryone")]
        void notifyEveryone(string text)
        {
            TriggerClientEvent("ShowNotification", text);
        }

    }
}
