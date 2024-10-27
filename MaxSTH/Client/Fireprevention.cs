using CitizenFX.Core;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
public class FireControl : BaseScript
{
    public FireControl()
    {
        Tick += OnTick;
    }

    private async Task OnTick()
    {
        // Define the position and range where you want to stop the fire spread
        Vector3 position = Game.PlayerPed.Position;
        float fireRange = 50.0f; // radius in meters

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
}
