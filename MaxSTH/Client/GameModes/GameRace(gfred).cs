using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class GameRace : BaseScript
    {
        static Random rand = new Random();
        int serverId = Game.Player.ServerId;
        private DateTime startTime;
        private List<TimeSpan> checkpointTimes = new List<TimeSpan>();
        public static Vector4 respawnLocation = new Vector4(0, 0, 71, 0);
        public static bool isGfredRunning = false;
        public static string lastGfredClass = "cycles";

        List<Vector4> checkpointList = new List<Vector4>{
            // new Vector4(907.72f, -44.73f, 77.79f, 15),//location
            // new Vector4(817.3f, -189.19f, 71.79f, 15),//
            // new Vector4(964.3f, -287.38f, 65.99f, 15),//
            // new Vector4(891.51f, -419.46f, 30.92f, 15),//
            // new Vector4(725.24f, -474.24f, 15.29f, 15),//
            // new Vector4(570.51f, -754.26f, 10.73f, 15),//
            //new Vector4(567.47f, -1125.37f, 9.14f, 15),//
            //new Vector4(635.97f, -1848.58f, 8.22f, 15),//
            //new Vector4(624.61f, -2101.63f, -0.67f, 30),// boat
            // new Vector4(617.04f, -2452.62f, -0.28f, 15),// plane
            // new Vector4(638.05f, -2684.1f, 5.69f, 15),//finish



            new Vector4(3365.5f, 5181.56f, 0, 15),//lighthouse 
            new Vector4(2718.59f, 4764.13f, 43.08f, 15),//first highway - muscle
            new Vector4(2860.17f, 3568.03f, 53.78f, 40),//highway before quarry - off-road
            new Vector4(2955.75f, 2793.06f, 39.2f, 20),//quarry - suvs
            new Vector4(1775.11f, 2107.99f, 64.13f, 15),//highway split - coupes
            new Vector4(951.37f, 249.7f, 79.32f, 15),//highway casino - compacts
            new Vector4(218.19f, -1281.76f, 28.3f, 25),//xero strawberry - super
            new Vector4(-1017.16f, -2599.56f, 37.76f, 13),//lsia carpark - sports
            new Vector4(218.19f, -1281.76f, 28.3f, 22),//xero strawberry - motorcycles
            new Vector4(1208.31f, 658.33f, 98.89f, 12),//supercar road - super
            new Vector4(225.02f, 3012.34f, 41.03f, 15),//just before railroad river jump - off-road
            new Vector4(500.3038f, 5596.246f, 791.92f, 15),//mount chilliad - suvs
            new Vector4(225.02f, 3012.34f, 41.03f, 15),//just after railroad river jump - super
            new Vector4(-423.63f, 1130.27f, 324.65f, 15),//observatory - coupes
            new Vector4(-1353.95f, 2582.26f, 16.87f, 15),//armybase bridge - super
            new Vector4(-2167.14f, 5191.61f, 16.76f, 15),//finish island
        };

        //List<string> vehicleClassList = new List<string> { "cycles", "sedans", "off-road", "suvs", "coupes", "compacts", "super", "sports", "motorcycles", "boats", "helicopters", "suvs", "super", "coupes", "super" };
        List<string> vehicleClassList = new List<string> { "cycles", "sedans", "off-road", "suvs", "coupes", "compacts", "super", "sports", "motorcycles", "super", "off-road", "suvs", "super", "coupes", "super" };

        [EventHandler("Gfred")]
        async void Gfred()
        {
            if (checkpointList.Count == 0)
            {
                NotificationScript.ShowErrorNotification("Checkpoint list is empty. Stopping.");
                return;
            }
            if (isGfredRunning)
            {
                NotificationScript.ShowErrorNotification("gfred is already running. Stopping.");
                return;
            }
            isGfredRunning = true;


            API.FreezeEntityPosition(Game.PlayerPed.Handle, true);
            SetPlayerWeaponsForRace();
            Health.SetPlayerStats(300, 100);

            await Delay(5000);
            VehicleHandler.SetPlayerIntoNewVehicle("cruiser");
            while (Game.PlayerPed.CurrentVehicle == null) await Delay(1);
            API.FreezeEntityPosition(Game.PlayerPed.CurrentVehicle.Handle, true);
            NotificationScript.ShowSpecialNotification($"Starting ~g~~h~Gfred~s~~w~ in 3 seconds.", "3_2_1", "HUD_MINI_GAME_SOUNDSET");
            await Delay(1000);
            NotificationScript.ShowSpecialNotification($"Starting ~g~~h~Gfred~s~~w~ in 2 seconds.", "3_2_1", "HUD_MINI_GAME_SOUNDSET");
            await Delay(1000);
            NotificationScript.ShowSpecialNotification($"Starting ~g~~h~Gfred~s~~w~ in 1 seconds.", "3_2_1", "HUD_MINI_GAME_SOUNDSET");
            await Delay(1000);
            NotificationScript.ShowSpecialNotification("~g~~h~Go Go Go Go!!!\n~s~~w~Gfred has started!", "GO", "HUD_MINI_GAME_SOUNDSET");
            API.FreezeEntityPosition(Game.PlayerPed.CurrentVehicle.Handle, false);
            API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
            bool isLastCheckpoint = false;
            int checkpointHandle;
            int currentCheckpointBlipHandle = 0;
            int nextCheckpointBlipHandle = 0;

            startTime = DateTime.Now;  // Start the timer
            checkpointTimes.Clear();

            string formattedTime = DateTime.UtcNow.ToString("o"); // "o" = ISO 8601 | Use ISO 8601 format (yyyy-MM-ddTHH:mm:ss.fffZ), which is universal and easy to parse.
            TriggerServerEvent("receiveStartTime", formattedTime);
            SetPlayerControl(serverId, true, 0);

            int currentIndex = -1;
            int lastIndexOfList = checkpointList.Count - 1;

            foreach (Vector4 cp in checkpointList)
            {
                int checkpointType = 0;
                currentIndex++;
                Vector4 nextCp = Vector4.Zero;

                if (currentIndex != lastIndexOfList)
                    nextCp = checkpointList[currentIndex + 1];
                else
                {
                    checkpointType = 4; // Finish line checkpoint
                    isLastCheckpoint = true;
                }

                //create checkpoint
                checkpointHandle = CreateCheckpoint(checkpointType, cp.X, cp.Y, cp.Z - 1.2f, nextCp.X, nextCp.Y, nextCp.Z, cp.W, 255, 194, 0, 40, 0);
                SetCheckpointCylinderHeight(checkpointHandle, 15, 25, 150);
                SetCheckpointIconHeight(checkpointHandle, 0.5f);

                //create current checkpoint blip
                currentCheckpointBlipHandle = AddBlipForCoord(cp.X, cp.Y, cp.Z);
                SetBlipScale(currentCheckpointBlipHandle, 1f);


                //create next checkpoint smaller and less visible blip
                if (!isLastCheckpoint)
                {
                    nextCheckpointBlipHandle = AddBlipForCoord(nextCp.X, nextCp.Y, nextCp.Z);
                    SetBlipScale(nextCheckpointBlipHandle, 0.8f);
                    SetBlipAlpha(nextCheckpointBlipHandle, 150);
                    SetBlipShrink(nextCheckpointBlipHandle, true);
                }

                //wait for the player to get close to the ckeckpoint
                bool didPlayerGetCheckpoint = false;
                int sendServerInfoAfter20loops = 0;
                while (!didPlayerGetCheckpoint && isGfredRunning)
                {
                    sendServerInfoAfter20loops++;
                    Vector3 pos = Game.PlayerPed.Position;
                    float distance = GetDistanceBetweenCoords(pos.X, pos.Y, pos.Z, cp.X, cp.Y, cp.Z, false);
                    float heightOffset = pos.Z - cp.Z;

                    if (heightOffset > -2 && heightOffset < 14 && distance < cp.W + 1)
                        didPlayerGetCheckpoint = true;

                    await Delay(50);

                    //update the server with info for current leaderboard. Does it every second.
                    if (sendServerInfoAfter20loops == 20)
                    {
                        TriggerServerEvent("updateFirstPlaceAndScoreboard", serverId, currentIndex, distance);
                        sendServerInfoAfter20loops = 0;
                    }
                }

                sendServerInfoAfter20loops = 0;
                respawnLocation = new Vector4(cp.X, cp.Y, cp.Z, Game.PlayerPed.Heading);
                // Calculate and log checkpoint time
                TimeSpan checkpointTime = DateTime.Now - startTime;

                checkpointTimes.Add(checkpointTime);
                string timeFormatted = checkpointTime.ToString(@"mm\:ss\:ff");

                //Debug.WriteLine($"Checkpoint {currentIndex + 1} reached at {timeFormatted}");
                if (!isLastCheckpoint) NotificationScript.ShowSpecialNotification($"Checkpoint {currentIndex + 1} | Time: {timeFormatted}", "CHALLENGE_UNLOCKED", "HUD_AWARDS");

                DeleteCheckpoint(checkpointHandle);
                RemoveBlip(ref currentCheckpointBlipHandle);
                RemoveBlip(ref nextCheckpointBlipHandle);

                checkpointHandle = 0;

                if (currentIndex != 0 && !isLastCheckpoint)
                {
                    string vehicleClass = vehicleClassList[currentIndex];
                    VehicleHandler.SetPlayerIntoNewVehicle(VehicleHandler.GetRandomVehicleFromClass(vehicleClass));
                    lastGfredClass = vehicleClass;
                }
            }

            // Final notification with all times
            string finalTime = (DateTime.Now - startTime).ToString(@"mm\:ss\:ff");
            string allCheckpointTimes = string.Join(" | ", checkpointTimes.Select(t => t.ToString(@"mm\:ss\:ff")));
            NotificationScript.displayClientDebugLine($"Total Time: {finalTime}\nTimes: {allCheckpointTimes}");
            PlaySoundFrontend(500, "BASE_JUMP_PASSED", "HUD_AWARDS", false);
            TriggerServerEvent("sendRaceResults", serverId, finalTime);
            isGfredRunning = false;
        }

        [EventHandler("tpPlayerOnFloor")] //Task cannot be event handler. Thus this functions purpose.
        async void tpPlayerOnFloor(Vector4 Position)
        {
            Debug.WriteLine($"eventhandler");
            await TpPlayerOnTheFloor(Position);
        }

        static async Task TpPlayerOnTheFloor(Vector4 Position)
        {
            Debug.WriteLine("starting tpPlayerOnFloor");
            Vector4 inputStartingPosition = new Vector4(Position.X, Position.Y, Position.Z, Position.W);
            Game.PlayerPed.Position = new Vector3(inputStartingPosition.X, inputStartingPosition.Y + 3, inputStartingPosition.Z); //put player near spawningpoint. Then check for the area's colision.
            await Delay(1);

            //cast a ray down from starting position. Check if there is floor beneeth.
            int rayHandle = API.StartShapeTestRay(inputStartingPosition.X, inputStartingPosition.Y, inputStartingPosition.Z, inputStartingPosition.X, inputStartingPosition.Y, -50, (int)IntersectOptions.Map, Game.PlayerPed.Handle, 7);

            bool hit = false;
            Vector3 hitCoords = Vector3.Zero;
            Vector3 surfaceNormal = Vector3.Zero;
            int entityHit = 0;
            int resultState = GetShapeTestResult(rayHandle, ref hit, ref hitCoords, ref surfaceNormal, ref entityHit);
            int i = 0;
            while (resultState == 1 && i < 1000) //1 try's for 1 second.
            {
                i++;
                await Delay(1);
            }
            if (resultState == 2) Debug.WriteLine($"Ground found for teleport. Continuing.");
            else { Debug.WriteLine($"No ground found for teleport. Timed out."); respawnLocation = inputStartingPosition; return; } //0 means it failed.


            Vector4 resultStartingPosition = Vector4.Zero;
            if (hit == true) resultStartingPosition = new Vector4(hitCoords.X, hitCoords.Y, hitCoords.Z, inputStartingPosition.W);
            else resultStartingPosition = new Vector4(inputStartingPosition.X, inputStartingPosition.Y, inputStartingPosition.Z, inputStartingPosition.W);

            Game.PlayerPed.Position = new Vector3(resultStartingPosition.X, resultStartingPosition.Y, resultStartingPosition.Z);
            Game.PlayerPed.Heading = resultStartingPosition.W;
            respawnLocation = resultStartingPosition;

        }

        void SetPlayerWeaponsForRace()
        {
            Game.PlayerPed.Weapons.RemoveAll();
            Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, true);
            Game.PlayerPed.Weapons.Give(WeaponHash.RayPistol, 1, false, false);
            Game.PlayerPed.Weapons.Give(WeaponHash.Flare, 100, false, false);
            Game.PlayerPed.Weapons.Give(WeaponHash.FireExtinguisher, 200, false, false);
        }
    }
}
