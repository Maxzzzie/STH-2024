using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
using STHMaxzzzie.Client;

namespace STHMaxzzzie.Client
{
    public class PlayerDeathHandler : BaseScript
    {
        public PlayerDeathHandler()
        {
            Tick += MonitorPlayerDeath;
        }

        private bool isDead = false;

        private async Task MonitorPlayerDeath()
        {
            var ped = Game.PlayerPed;

            // Check if the player is dead
            if (!isDead && ped.IsDead)
            {
                isDead = true;
                await Delay(100);
                await HandlePlayerDeath(ped);
            }
            else if (isDead && !ped.IsDead)
            {
                isDead = false; // Reset when the player respawns
            }

            await Task.FromResult(0);
        }

        private async Task HandlePlayerDeath(Ped ped)
        {
            int victimServerId = API.GetPlayerServerId(API.PlayerId());
            var victimLocation = ped.Position;

            var killerEntity = API.GetPedSourceOfDeath(ped.Handle);
            int killerEntityType = API.GetEntityType(killerEntity);

            int killerServerId = -1;
            int weaponHash = API.GetPedCauseOfDeath(ped.Handle);
            bool killerInVehicle = false;
            int vehicleHash = -1;
            Vector3 killerLocation = new Vector3();
            // Check if the killer is another player
            if (killerEntityType == 1) // Ped
            {
                // Get killer's position
                killerLocation = API.GetEntityCoords(killerEntity, true);

                // Check if the killer is a player
                for (int i = 0; i < 1000; i++)
                {
                    if (API.NetworkIsPlayerActive(i))
                    {
                        //Debug.WriteLine($"player {i} is active and playerPed = {API.GetPlayerPed(i)}.");

                        if (API.GetPlayerPed(i) == killerEntity)
                        {
                            killerServerId = API.GetPlayerServerId(i);

                            // Check if the killer was in a vehicle
                            if (API.IsPedInAnyVehicle(killerEntity, false))
                            {
                                killerInVehicle = true;
                                var vehicle = API.GetVehiclePedIsIn(killerEntity, false);
                                vehicleHash = API.GetEntityModel(vehicle);
                            }
                            break;
                        }
                    }
                }
                // Trigger server events with the gathered data
                if (killerServerId != -1 && killerServerId != victimServerId) // Killed by a player but not yourself
                {
                    TriggerServerEvent("OnPlayerKilled", victimServerId, killerServerId, victimLocation, killerLocation, weaponHash, killerInVehicle, vehicleHash);
                    //NotificationScript.ShowNotification($"OnPlayerKilled, VictimId: {API.GetPlayerServerId(API.PlayerId())}, killerId: {killerServerId}, victimLoc: {victimLocation}, killerLoc: {killerLocation}, weaponHash: {weaponHash}, killerInVeh {killerInVehicle}, vehHash: {vehicleHash}");
                }
                else if (killerServerId == victimServerId) //if you are the killer
                {
                    TriggerServerEvent("OnPlayerSuicide", victimServerId, victimLocation);
                }
                else // Killed by NPC or environment
                {
                    TriggerServerEvent("OnPlayerDied", victimServerId, victimLocation);
                    //NotificationScript.ShowNotification($"OnPlayerDied, VictimId: {API.GetPlayerServerId(API.PlayerId())}, victimLoc: {victimLocation}");
                }
            }
            else
            {
                TriggerServerEvent("OnPlayerDied", victimServerId, victimLocation);
            }
            Debug.WriteLine($"PlayerDeathHandler, VictimId: {victimServerId}, killerEntity: {killerEntity}, killerEntityType: {killerEntityType}, killerId: {killerServerId}, victimLoc: {victimLocation}, killerLoc: {killerLocation}, weaponHash: {weaponHash}, killerInVeh {killerInVehicle}, vehHash: {vehicleHash}");

            await Delay(50); // Optional delay to prevent double-processing
        }
    }
}

