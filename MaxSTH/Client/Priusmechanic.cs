using CitizenFX.Core;
using System.Collections.Generic;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class pinkPrius : BaseScript
    {
        [Command("pri")]
        private void Pri(int source, List<object> args, string raw)
        {
            TriggerServerEvent("pri-spawn-requested", VehicleHash.Dilettante, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading - 180);
        }

        [EventHandler("modPinkPri")]
        void modPinkPri(int vehicle)
        {
            Debug.WriteLine($"pre if pri");
                
               
             
                // Debug.WriteLine($"in if pri {vehicle}");
                //API.SetVehicleColours(vehicle, 135, 135);
                // API.SetVehicleColours(vehicle.Handle, color1, color2);
                // API.SetVehicleNumberPlateText(vehicle, $"{ownerID.Name}");
                // API.SetVehicleNeonLightEnabled(vehicle, 0, true);
                // API.SetVehicleNeonLightEnabled(vehicle, 1, true);
                // API.SetVehicleNeonLightEnabled(vehicle, 2, true);
                // API.SetVehicleNeonLightEnabled(vehicle, 3, true);
                // API.SetVehicleNeonLightsColour(vehicle, 204, 0, 204);
                // API.SetVehicleXenonLightsColor(vehicle, 10);
                // API.SetVehicleLightsMode(vehicle, 2);
                // API.SetVehicleEngineOn(vehicle, true, true, false);
          
             
            // Debug.WriteLine($"past if pri");

        }
    }
}

