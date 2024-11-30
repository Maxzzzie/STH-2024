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
        bool didIAlreadySpawnOnce = false;
        string lastRespawnPoint = "null";
        string secondToLastRespawnPoint = "null";
        bool canUseRespawnCommand = true;

        public Spawns()
        { }

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
            if (Spawn.SpawnLock == false && !Game.PlayerPed.IsAlive) respawnPlayerHandler();
        }


        [Command("respawn")]
        async void respawnCommand()
        {
            if (!canUseRespawnCommand)
            {
                TriggerEvent("chat:addMessage", new{color=new[]{255,0,0},args=new[]{$"Wait"}});
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
            Debug.WriteLine("running spawn function");
            TriggerServerEvent("thisClientDiedForGameStateCheck", Game.Player.ServerId);

            //-------------------------------------------------- temp respawn code. Can use the bits for later on in the main code -------------------------------- below here including didIAlreadySpawnOnce bool up top of this funciton
            if (!didIAlreadySpawnOnce)
            {
                Spawn.SpawnPlayer(-1610f, -1055f, 13f, 318f);
                didIAlreadySpawnOnce = true;

            }
            else
            {
                if (respawnLocationsDict.Count() != 0)
                {


                    Vector3 pPos = Game.PlayerPed.Position;
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
                        Debug.WriteLine($"Key: {closestRespawnPoint} not found. Aborting respawn.");
                        Spawn.SpawnPlayer(-1610f, -1055f, 13f, 318f);
                        //SetPlayerInvincible(Game.PlayerPed.Handle, true);
                        lastRespawnPoint = "null";
                        secondToLastRespawnPoint = "null";
                    }
                    else
                    {
                        Vector4 spawn = respawnLocationsDict[closestRespawnPoint];
                        Spawn.SpawnPlayer(spawn.X, spawn.Y, spawn.Z, spawn.W);
                        Debug.WriteLine($"Respawning at closest avalible spawnpoint. \"{closestRespawnPoint}\"");
                        secondToLastRespawnPoint = lastRespawnPoint;
                        lastRespawnPoint = closestRespawnPoint;
                        //SetPlayerInvincible(Game.PlayerPed.Handle, true);
                    }
                }
                else
                {
                    Debug.WriteLine($"respawnLocationsDict not found. Default respawn.");
                    Spawn.SpawnPlayer(-1610f, -1055f, 13f, 318f);
                    SetPlayerInvincible(Game.PlayerPed.Handle, true);
                    lastRespawnPoint = "null";
                    secondToLastRespawnPoint = "null";
                }
                

                // await Delay(5000);
                // SetPlayerInvincible(Game.PlayerPed.Handle, false);
            }

            //-------------------------------------------------- end of temp respawn code ---------------------------------------------------------

            //-------------------------------------------- respawns where player tp's to peds. Currently broken -----------------------------------  below here         
            // Ped _ped = Game.PlayerPed; //player ped
            // Ped[] peds = World.GetAllPeds();
            // Ped near_ped = null;



            // for (int i = peds.Count() - 1; i > 0; i--)
            // {
            //     var p = peds[i];
            //     //filter out if z < -50 because we noticed sometimes z was exactly -100 
            //     if (!p.IsHuman || p.IsPlayer || !p.IsAlive || p.Position.Z < 0) continue;

            //     float distanceSquared = World.GetDistance(_ped.Position, p.Position);
            //     //filter out peds too close
            //     if (distanceSquared < 60f)
            //     {
            //         continue;
            //     }
            //     near_ped = p;
            //     break;
            // }
            // if (near_ped == null)
            // {
            //     Debug.WriteLine("Couldn't find nearby ped.");
            //     // TODO - spawn at closest hospital


            //     Spawn.SpawnPlayer(-1610f, -1055f, 13f, 318f);
            //     return;
            // }

            // var near_ped_pos = near_ped.Position;
            // Vector3 _ped_old_pos = _ped.Position;
            // Debug.WriteLine("Making new action");
            // Func<Task> during_spawn = new Func<Task>(async () =>
            // {
            //     await Delay(1);
            //     Game.PlayerPed.Position = near_ped.Position;
            //     var isPedInVeh = near_ped.IsInVehicle();
            //     Debug.WriteLine("near ped is in vehicle: " + isPedInVeh);
            //     if (isPedInVeh)
            //     {
            //         var veh = near_ped.LastVehicle;

            //         //if multiple peds are in vehicle, we replace the driver
            //         near_ped = veh.GetPedOnSeat(VehicleSeat.Driver);

            //         //try to delete the near_ped
            //         for (int i = 0; i < 5; i++)
            //         {
            //             Debug.WriteLine("trying to delete near_ped");
            //             near_ped.Position = new Vector3(0, 0, 0);
            //             if (!near_ped.IsInVehicle())
            //             {
            //                 near_ped.Delete();
            //                 Debug.WriteLine("Spawn: Succesfully removed driver.");
            //                 break;
            //             }
            //             await Delay(50);
            //         }

            //         //try to set player in near_peds vehicle
            //         for (int i = 0; i < 5; i++)
            //         {
            //             await Delay(0);
            //             Debug.WriteLine("trying to set player in vehicle");
            //             Game.PlayerPed.SetIntoVehicle(near_ped.LastVehicle, VehicleSeat.Driver);
            //             Debug.WriteLine("222");
            //             bool shouldbreak = Game.PlayerPed.LastVehicle.Handle == veh.Handle;
            //             Debug.WriteLine("shouldbreak" + shouldbreak);

            //             if (shouldbreak)
            //             {
            //                 Debug.WriteLine("Spawn: Succesfully put in vehicle.");
            //                 break;
            //             }
            //             Debug.WriteLine("222");
            //             await Delay(50);
            //         }
            //         Debug.WriteLine("end of task");
            //         // near_ped.Position = _ped.Position + new Vector3(0, 0, -10);
            //         //  near_ped.Position = _ped.Position + new Vector3(0, 0, -10);
            //     }
            //     else
            //     {
            //         //try to delete the near_ped
            //         for (int i = 0; i < 5; i++)
            //         {
            //             Debug.WriteLine("trying to delete near_ped");
            //             near_ped.Delete();
            //             if (!near_ped.Exists())
            //             {
            //                 Debug.WriteLine("Spawn: Succesfully removed pedestrian.");
            //                 break;
            //             }
            //             await Delay(50);
            //         }
            //     }

            //     // near_ped.Position = _ped_old_pos;
            //     // near_ped.Kill();
            // });
            // Spawn.SpawnPlayer(near_ped_pos.X, near_ped_pos.Y, near_ped_pos.Z, near_ped.Heading, during_spawn);

            // Debug.WriteLine($"{near_ped.Handle} | {near_ped.Position}");

            //-------------------------------------------- respawns where player tp's to peds. Currently broken -----------------------------------  above here
        }
    }
}


