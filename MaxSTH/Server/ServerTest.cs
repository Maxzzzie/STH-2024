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
            Debug.WriteLine($"This is the test command.");
            int X = 0;
            int Y = 0;
            TriggerClientEvent("CreateShotBlip", X, Y);
        }
    }
}
