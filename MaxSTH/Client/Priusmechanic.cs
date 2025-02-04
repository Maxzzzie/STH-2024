using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using System.Collections.Generic;
using CitizenFX.Core.Native;
using STHMaxzzzie.Client;

namespace STHMaxzzzie.Client
{
    public class pinkPrius : BaseScript
    {
        bool canSpawnNextPri = true;

        [Command("pri")]
        private async void Pri(int source, List<object> args, string raw)
        {
            if (canSpawnNextPri)
            {
                if (RoundHandling.thisClientIsTeam == 1)
                {
                    NotificationScript.ShowErrorNotification($"You are a runner you silly goose.");
                    return;
                }
            canSpawnNextPri = false;
            TriggerServerEvent("pri-spawn-requested", VehicleHash.Dilettante, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading - 180);
            await WaitForSeconds(20);//time between pri spawning to prevent spam, in seconds.
            canSpawnNextPri = true;
            }
            else
            {
                NotificationScript.ShowErrorNotification($"wait");
            }
        }

        private async Task WaitForSeconds(int seconds)
        {
            int targetTime = Environment.TickCount + (seconds * 1000); 
            while (Environment.TickCount < targetTime)
            {
                await Delay(1);
            }
        }
    }
}

