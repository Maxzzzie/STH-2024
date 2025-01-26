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
    public class GameBounce : BaseScript
    {
        public static int radius = 450;
        public static bool runnerSeesCircleBlip = false;


        [EventHandler("updateClientBounceSettings")]
        public static void updateClientBounceSettings()
        {
            Debug.WriteLine($"updateClientBounceSettings {radius}, {runnerSeesCircleBlip}");
            TriggerClientEvent("updateBounceGameSettings", radius, runnerSeesCircleBlip);
        }

        [EventHandler("sendGameBounceBlip")]
        public void sendGameBounceBlip(Vector4 blipPosAndRadius, bool removeBlipOnly)
        {
            TriggerClientEvent("setGameBounceBlip", blipPosAndRadius, removeBlipOnly);
        }
    }
}