using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class GameCopyClass : BaseScript
    {
        static string currentClass = "emergency";
        public static bool forceChosenVehicleOnly = false;


        [EventHandler("sendClientCopyClassSettings")]
        public static void sendClientCopyClassSettings()
        {
            TriggerClientEvent("UpdateCopyClassSettings", currentClass, forceChosenVehicleOnly);
        }

        [EventHandler("sendClientCopyClassClass")]
        public static void sendClientCopyClassClass(string newCurrentClass)
        {
            currentClass = newCurrentClass;

            TriggerClientEvent("UpdateCopyClassClass", currentClass);
        }

        [EventHandler("changeGameCopyClassSetting")]
        public void changeGameCopyClassSetting(int source, List<object> args)
        {
            if (args.Count == 1){}
            else if (args.Count == 3) 
            {
                if (args[1].ToString() == "force" && bool.TryParse(args[2].ToString(),out forceChosenVehicleOnly)){}
                //else if (args[1].ToString() == "decayrate" && int.TryParse(args[2].ToString(),out decayRate));
            }
            else TriggerClientEvent(Players[source], "ShowErrorNotification", $"/settings copyclass (gives current state)\nAdd (force) to set force chosen vehicle only.");
            sendCurrentSettings(source);
            sendClientCopyClassSettings();
        }

                public void sendCurrentSettings(int source)
                {
                    TriggerClientEvent(Players[source], "ShowNotification", $"~b~CopyClass~w~\nForce specific vehicle: {forceChosenVehicleOnly}");
                }

    }
}