using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
public class MugShot : BaseScript
    {
        [Command("mugshot", Restricted = false)]
        async void mugShot(int source, List<object> args, string raw)
        {
            if (args.Count == 1)
            {
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
                if (isArgs0Int)
                {
                    //Debug.WriteLine($"Test 1");
                    TriggerEvent("sendClientModelNameForOutfit", source, int.Parse(args[0].ToString()));
                }
                else
                {
                    //Debug.WriteLine($"test 2");
                                    TriggerEvent("sendClientModelNameForOutfit", source, source);
                }
            }
            else
            {
                //Debug.WriteLine($"test 3");
                if (RoundHandling.runnerThisGame != -1)
                {
                    TriggerEvent("sendClientModelNameForOutfit", source, RoundHandling.runnerThisGame);
                }
                else TriggerEvent("sendClientModelNameForOutfit", source, source);

            }
        }
    }
}