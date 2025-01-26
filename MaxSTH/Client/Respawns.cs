using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Mono.CSharp;
using STHMaxzzzie.Client;

namespace STHMaxzzzie.Client
{
    public class Spawns : BaseScript
    {
        static Dictionary<string, Vector4> respawnLocationsDict = new Dictionary<string, Vector4>();
        static Vector4 DefaultSpawnPosition = new Vector4(0,0,71.5f,0);
        public bool didDefaultSpawnPositionGetSet = false;
        bool didIAlreadySpawnOnce = false;
        string lastRespawnPoint = "null";
        string secondToLastRespawnPoint = "null";
        bool canUseRespawnCommand = true;
        bool isRespawnRunning = false;

        public Spawns()
        { }

        [EventHandler("setInitialRespawnOff")]
        void setInitialRespawnOff()
        {
        didIAlreadySpawnOnce = false;
        lastRespawnPoint = "null";
        secondToLastRespawnPoint = "null";
        }

        [EventHandler("getRespawnLocationsDict")]
        void getRespawnLocationsDict(string respawnLocationName, Vector4 respawnLocationsXYZH)
        {

            if (respawnLocationsDict != null && !respawnLocationsDict.ContainsKey(respawnLocationName))
            {
                respawnLocationsDict.Add(respawnLocationName, respawnLocationsXYZH);
                //Debug.WriteLine($"name {respawnLocationName} location xyz {respawnLocationsXYZH.ToString()}");
            }
        }

        [Tick]
        public async Task OnTick()
        {
            if (Spawn.SpawnLock == false && !Game.PlayerPed.IsAlive && !isRespawnRunning) respawnPlayerHandler();
        }


        [Command("respawn")]
        async void respawnCommand()
        {
            if (!canUseRespawnCommand || isRespawnRunning)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Wait" } });
                return;
            }
            canUseRespawnCommand = false;
            respawnPlayerHandler();
            await Delay(15000);
            canUseRespawnCommand = true;
        }

        [EventHandler("respawnPlayer")]
        async void respawnPlayerHandler()
        {
            if (isRespawnRunning)
            {
                while (isRespawnRunning)
                {
                    await Delay(5);
                }
            }
            isRespawnRunning = true;
            Vector3 pPos = Game.PlayerPed.Position;
            Vector4 SpawnPos = DefaultSpawnPosition;
            Debug.WriteLine("running spawn function");
            TriggerServerEvent("thisClientDiedForGameStateCheck", Game.Player.ServerId);

            //-------------------------------------------------- temp respawn code. Can use the bits for later on in the main code -------------------------------- below here including didIAlreadySpawnOnce bool up top of this funciton
            if (!didIAlreadySpawnOnce)
            {
                while (!didDefaultSpawnPositionGetSet)
                {
                    Debug.WriteLine("default spawnposition isn't set yet.");
                    await Delay(50);
                }
                await Spawn.SpawnPlayer(DefaultSpawnPosition.X, DefaultSpawnPosition.Y, DefaultSpawnPosition.Z, DefaultSpawnPosition.W);
                didIAlreadySpawnOnce = true;
                isRespawnRunning = false;
                await Delay(200);
                Appearance.changeRandomModel();
                await Delay(2000);
                NotificationScript.ShowMOTD();
                return;
            }
            else if (respawnLocationsDict.Count() != 0)
            {

                string closestRespawnPoint = null;
                float distanceToThatPoint = float.PositiveInfinity;
                foreach (var entry in respawnLocationsDict)
                {
                    float dist = GetDistanceBetweenCoords(entry.Value.X, entry.Value.Y, entry.Value.Z, pPos.X, pPos.Y, pPos.Z, true);
                    if (dist < distanceToThatPoint && entry.Key != lastRespawnPoint && entry.Key != secondToLastRespawnPoint)//makes you not spawn again in the same spot after spawning there previously
                    {
                        distanceToThatPoint = dist;
                        closestRespawnPoint = entry.Key;
                    }
                }

                if (!respawnLocationsDict.ContainsKey(closestRespawnPoint))
                {
                    Debug.WriteLine($"Key: {closestRespawnPoint} not found. Default respawn.");
                    //Spawn.SpawnPlayer(-1610f, -1055f, 13f, 318f);
                    //SetPlayerInvincible(Game.PlayerPed.Handle, true);
                    lastRespawnPoint = "null";
                    secondToLastRespawnPoint = "null";
                }
                else
                {
                    SpawnPos = respawnLocationsDict[closestRespawnPoint];
                    //Spawn.SpawnPlayer(spawn.X, spawn.Y, spawn.Z, spawn.W);
                    Debug.WriteLine($"Respawning at closest avalible spawnpoint. \"{closestRespawnPoint}\"");
                    secondToLastRespawnPoint = lastRespawnPoint;
                    lastRespawnPoint = closestRespawnPoint;
                }
            }
            else
            {
                Debug.WriteLine($"respawnLocationsDict not found. Default respawn.");
                //Spawn.SpawnPlayer(-1610f, -1055f, 13f, 318f);
                lastRespawnPoint = "null";
                secondToLastRespawnPoint = "null";
            }

            // Try pointing the camera at the killer if they exist
            API.StartScreenEffect("DeathFailMPDark", 0, false);
            string soundName = "Bed"; // 'Bed' is commonly used in "Wasted" events
            string soundSet = "WastedSounds"; // The specific set for "Wasted"
            API.PlaySoundFrontend(-1, soundName, soundSet, false);

            int killer = API.GetPedSourceOfDeath(Game.PlayerPed.Handle);
            if (killer != 0 && API.DoesEntityExist(killer))
            {
                Debug.WriteLine("Killer exists. Focusing camera on killer.");
                Vector3 killerPosition = API.GetEntityCoords(killer, true);

                // Create a camera
                int cam = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                API.SetCamCoord(cam, pPos.X, pPos.Y, pPos.Z + 1f); // Slightly above the player
                API.PointCamAtEntity(cam, killer, 0, 0, 0, true);

                // Activate the camera
                API.SetCamActive(cam, true);
                API.RenderScriptCams(true, false, 0, true, false);

                // Wait for a few seconds (e.g., 3 seconds)
                await Delay(8000);

                // Restore gameplay camera
                API.RenderScriptCams(false, false, 0, true, false);
                API.DestroyCam(cam, false);

            }
            else
            {
                Debug.WriteLine("No killer found. Skipping camera focus.");
                await Delay(8000);
            }

            AnimpostfxStop("DeathFailMPDark");
            await Spawn.SpawnPlayer(SpawnPos.X, SpawnPos.Y, SpawnPos.Z, SpawnPos.W);

            TriggerServerEvent("updatePlayerBlips");
            isRespawnRunning = false;

        }

        [EventHandler("updateDefaultSpawnLocation")]
        void updateDefaultSpawnLocation(Vector4 receivedSpawnPoint)
        {
            Debug.WriteLine($"setting default spawnlocation to {receivedSpawnPoint}.");
            DefaultSpawnPosition = receivedSpawnPoint;
            didDefaultSpawnPositionGetSet = true;
        }
    }
}


