using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using Mono.CSharp;

public class ShotDetection : BaseScript
{
        public ShotDetection()
    {
        Tick += OnTick;
    }
        private async Task OnTick()
        {
            if (API.IsPedShooting(Game.PlayerPed.Handle))
            {
                int X = (int)Game.PlayerPed.Position.X;
                int Y = (int)Game.PlayerPed.Position.Y;
                //Debug.WriteLine($"Ped is SHOOTING at {X},{Y}!");
                TriggerServerEvent("OnShotsFired", X, Y);
            }
        }

    [EventHandler("CreateShotBlip")]
    private async void CreateShotBlip(int X, int Y)
    {
        int radius = 100;
        int blip = API.AddBlipForRadius(X, Y, 0, radius);
        API.SetBlipColour(blip, 1);
        API.SetBlipAlpha(blip, 50);

        for (radius = 100; radius > 0; radius--)
        {
            API.SetBlipScale(blip, radius);
            await Delay(5); //speed of decay
        }
        API.RemoveBlip(ref blip);
        // radius = 100; is a nice size.
        //Debug.WriteLine($"removing a shots fired blip");
    }
}
