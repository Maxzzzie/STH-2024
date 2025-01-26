using CitizenFX.Core;
using System.Threading.Tasks;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class FireControl : BaseScript
    {
        public FireControl()
        {
            Tick += OnTick;
        }

        bool defaultShouldFireBeControlled = false;
        float fireRange = 20.0f; // radius in meters
        private async Task OnTick()
        {
            Vector3 position = Game.PlayerPed.Position;
            if (!defaultShouldFireBeControlled && Game.PlayerPed.Weapons.Current.Hash != WeaponHash.FireExtinguisher)
            {
                return;
            }
            else if (Game.PlayerPed.Weapons.Current.Hash == WeaponHash.FireExtinguisher)
            {
                int extinguisherFireCount = API.GetNumberOfFiresInRange(position.X, position.Y, position.Z, 5);

                if (extinguisherFireCount > 0 && API.IsPedShooting(Game.PlayerPed.Handle))
                {
                    API.StopFireInRange(position.X, position.Y, position.Z,5);
                }
                Game.PlayerPed.Weapons.Current.InfiniteAmmo = true;
                return;
            }

            // Check if there are fires in the area
            int fireCount = API.GetNumberOfFiresInRange(position.X, position.Y, position.Z, fireRange);

            if (fireCount > 0)
            {
                // Stop the fire in the specified range
                API.StopFireInRange(position.X, position.Y, position.Z, fireRange);

                // Optionally remove fire particle effects
                //API.RemoveParticleFxInRange(position.X, position.Y, position.Z, fireRange);
            }

            await Task.FromResult(0); // Ensure the task completes
        }

        [EventHandler("toggleFire")]
        void toggleFs(bool newShouldFireBeControlled, int newFireRange)
        {
            defaultShouldFireBeControlled = newShouldFireBeControlled;
            fireRange = newFireRange;
        }

        [EventHandler("clearFire")]
        void clearFire()
        {
            Vector3 position = Game.PlayerPed.Position;

            // Check if there are fires in the area
            int fireCount = API.GetNumberOfFiresInRange(position.X, position.Y, position.Z, 1000);

            if (fireCount > 0)
            {
                API.StopFireInRange(position.X, position.Y, position.Z, 1000);
            }
        }
    }
}