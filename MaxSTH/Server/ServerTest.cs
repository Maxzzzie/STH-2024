using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        [Command("test", Restricted = false)]
        void TestCommand(int source, List<object> args, string raw)
        {
            //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Test command." } });
            if (args.Count == 1)
            {
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
                if (isArgs0Int)
                {
                    Debug.WriteLine($"test 1");
                    TriggerEvent("sendClientModelNameForOutfit", source, int.Parse(args[0].ToString()));
                }
                else
                {
                    Debug.WriteLine($"test 2");
                                    TriggerEvent("sendClientModelNameForOutfit", source, source);
                }
            }
            else
            {
                Debug.WriteLine($"test 3");
                TriggerEvent("sendClientModelNameForOutfit", source, source);
            }
        }
    }
}
