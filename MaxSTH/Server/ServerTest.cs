using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder; // chat gpt addition to prevent compile error

/*
namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        [Command("test", Restricted = false)]
        void TestCommand(int source, List<object> args, string raw)
        {
            Debug.WriteLine($"This is the test command.");

            // Display notification to the player
            API.SetNotificationTextEntry("STRING");
            API.AddTextComponentString("This is a test notification.");
            API.SetNotificationMessage("CHAR_CARSITE", "CHAR_CARSITE", true, 1, "Test Notification", "");
            API.DrawNotification(false, true);
        }
        void test(int source, List<object> args, string raw)
        {
            string message = "This is the test command.";
            string type = "CHAR_ABIGAIL";
            TriggerClientEvent("STH:ShowNotification", source, message, type);
        }
    }
}*/