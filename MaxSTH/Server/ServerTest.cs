using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        [Command("test", Restricted = false)]
        async void TestCommand(int source, List<object> args, string raw)
        {
                // int cal = ServerMain.respawnLocationsDict.Count;
                // int res = ServerMain.maxzzzieCalloutsDict.Count;
                // TriggerClientEvent("chat:addMessage", new{color=new[]{255,255,255},args=new[]{$"test callouts: {cal} respawn: {res}."}});
                // TriggerEvent("addBlip", true, "respawnName", "coord", new Vector3(0, 0, 0), 0, 133, 47, true, false, true);
                // await Delay(5000);
                // TriggerEvent("addBlip", false, "respawnName", "coord", new Vector3(0, 0, 0), 0, 133, 47, true, false, true);

            //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Test command." } });
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
                TriggerEvent("sendClientModelNameForOutfit", source, source);
            }
        }
    }
}
