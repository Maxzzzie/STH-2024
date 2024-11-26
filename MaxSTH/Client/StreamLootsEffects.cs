using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Linq;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Client
{
    public class StreamLootsEffects : BaseScript
    {
        bool isSpotlightOn = false;
        bool isPlayerDrunk = false;
        bool isGta1CamOn = false;
        bool isReverseCamOn = false;
        public static bool isStarmodeOn = false; //to not automatically update player colours
        DateTime fameStartTime;
        DateTime carFameStartTime;
        DateTime electricalGlitchStartTime;
        DateTime starmodeEndTime;
        Random rand = new Random();

        public StreamLootsEffects()
        {
            Tick += Spotlight;
            Tick += MonitorWeaponAndShooting;
            Tick += UpdatePedTasks;
            Tick += Gta1Cam;
            //loopThroughSlowly();

        }

        //     int x = 1;

        // async void loopThroughSlowly()
        // {
        //     while (x == 1)
        //     {
        //     MonitorWeaponAndShooting();
        //     await Delay(500);
        //     }
        // }

        [EventHandler("StreamLootsEffect")] 
        void DoStreamLootsEvent(string type)
        {
            if (type == "cleartires")
            {
                ClearAllTires();
            }
            if (type == "spotlight")
            {
                isSpotlightOn = !isSpotlightOn;
            }
            if (type == "burstsome")
            {
                BreakSomePlayerTires();
            }
            if (type == "burstall")
            {
                BreakAllPlayersTires();
            }
            if (type == "drunk")
            {
                setPlayerDrunk();
            }
            if (type == "stop")
            {
                stopPlayerOnTheSpot();
            }
            if (type == "speed")
            {
                givePlayerLotsOfForwardMomentum();
            }
            if (type == "launch")
            {
                launchPlayerUp();
            }
            if (type == "gta1cam")
            {
                isGta1CamOn = !isGta1CamOn;
            }
            if (type == "reversecam")
            {
                SetReverseCam();
            }
            if (type == "bounce")
            {
                BounceVehicle();
            }
            if (type == "kickflip")
            {
                PerformKickflip();
            }
            if (type == "gunjam")
            {
                canGunJam = true;
                isGunJammed = false;
                maxShots = rand.Next(1, 15);
            }
            if (type == "fame")
            {
                fameStartTime = DateTime.Now;
                StartNPCsRunningTowardsPlayer();
            }
            if (type == "carfame")
            {
                carFameStartTime = DateTime.Now;
                StartCarsDrivingIntoPlayer();
            }
            if (type == "imponent")
            {
                SpawnImponentFollower();
            }
            if (type == "paint")
            {
                ChangeCarColor();
            }
            if (type == "paintall")
            {
                PaintAllNearbyVehicles();
            }
            if (type == "crack")
            {
                BreakAllVehicleWindows();
            }
            if (type == "electricalglitch")
            {
                electricalGlitchStartTime = DateTime.Now;
                StartElectricalGlitch();
            }
            if (type == "starmode")
            {
                StartRainbowCarEffect();
            }
            if (type == "carborrow")
            {
                TeleportIntoRandomNPCVehicle();
            }
            if (type == "compacted")
            {
                ChangeToCompactCar();
            }
            if (type == "speedlimiter")
            {
                ApplySpeedLimiter();
            }
            if (type == "carswap")
            {
                SwapCarsWithAnotherPlayer();
            }
            if (type == "shake")
            {
                shakeCam();
            }
        }

        void ClearAllTires()
        {

            Vehicle[] allVeh = World.GetAllVehicles();
            foreach (Vehicle veh in allVeh)
            {
                API.SetVehicleTyreBurst(veh.Handle, 0, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 1, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 2, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 3, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 4, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 5, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 6, true, 1000);
                API.SetVehicleTyreBurst(veh.Handle, 7, true, 1000);
            }
        }

        void BreakSomePlayerTires()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                var rand = new Random();
                var index = rand.Next(0, 7);
                API.SetVehicleTyreBurst(veh.Handle, index, true, 1000);
            }
        }

        void BreakAllPlayersTires()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                {
                    API.SetVehicleTyreBurst(veh.Handle, 0, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 1, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 2, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 3, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 4, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 5, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 6, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 7, true, 1000);
                }
            }
        }

        void setPlayerDrunk()
        // {
        // int handle = Game.PlayerPed.Handle;
        // API.SetAudioSpecialEffectMode(7);
        // API.SetPedIsDrunk(handle ,isPlayerDrunk);
        // API.
        {
            isPlayerDrunk = !isPlayerDrunk;
            if (isPlayerDrunk)
            {
                // Apply the timecycle modifier for a drunk effect
                API.SetTimecycleModifier("spectator5");

                // Increase the intensity of the effect (lower values = stronger effect)
                API.SetTimecycleModifierStrength(1.0f);

                // Apply camera shake for added realism
                API.ShakeGameplayCam("DRUNK_SHAKE", 1.0f);

                // Optionally, apply screen effects
                //API.StartScreenEffect("DrugsTrevorClownsFight", 0, true);
            }
            if (!isPlayerDrunk)
            {
                // Clear timecycle and camera effects
                API.ClearTimecycleModifier();
                API.StopGameplayCamShaking(true);
                //API.StopScreenEffect("DrugsTrevorClownsFight");
            }
        }

        private void stopPlayerOnTheSpot()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                API.SetEntityVelocity(veh.Handle, 0, 0, 0);
            }
        }

        private void givePlayerLotsOfForwardMomentum()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 150.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, true);
            }
        }

        private void launchPlayerUp()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 0.0f, 40.0f, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, true);
            }
        }

        private async Task Spotlight() //spotlight
        {

            if (isSpotlightOn)
            // Get the player's current position
            {
                Vector3 playerPosition = Game.PlayerPed.Position;

                // Define the spotlight parameters
                Vector3 lightPosition = playerPosition + new Vector3(0, 0, 15.0f); // 15 units above the player
                Vector3 direction = new Vector3(0, 0, -1); // Pointing straight down
                float distance = 100.0f; // Maximum distance the light reaches
                int r = 255, g = 255, b = 255; // White light color
                float brightness = 4.0f; // Intensity of the light
                float roundness = 1.0f; // Spot size and softness
                float radius = 25.0f; // Radius of the spotlight
                float fadeout = 8.0f; // How the light fades out at the edges

                // Draw the spotlight
                API.DrawSpotLightWithShadow(lightPosition.X, lightPosition.Y, lightPosition.Z, direction.X, direction.Y, direction.Z,
                                  r, g, b, distance, brightness, roundness, radius, fadeout, 1);
            }
            await Task.FromResult(0);
        }


        public Vector3 playerPreviousPosition = new Vector3(0, 0, 0);
        public float dynamicCamHeight = 2;
        private int gta1CamHandle = -1;

        private const float MovementLVL0 = 0.1f;
        private const float MovementLVL1 = 0.3f;
        private const float MovementLVL2 = 0.6f;
        private const float MovementLVL3 = 0.8f;

        private float CurrentCamHeight = 10f;
        private float smoothingFactor = 0.1f; // Lerp factor for smoother transitions
        private int tickBuffer = 0; // Buffer for threshold changes
        private int requiredTicks = 30; // Ticks required to commit to a height change

        private int obstructedTicks = 0; // Counter for obstruction
        private const int maxObstructedTicks = 30; // Threshold for lowering the camera
        private bool goBackUp = true;
        private async Task Gta1Cam()
        {
            var playerPed = Game.PlayerPed;

            if (isGta1CamOn && ((Game.PlayerPed.Weapons.Current.Hash == WeaponHash.Unarmed && Game.PlayerPed.IsOnFoot) || Game.PlayerPed.IsInVehicle()))
            {
                // Create the camera if it doesn't already exist
                if (gta1CamHandle == -1)
                {
                    gta1CamHandle = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                    API.SetCamActive(gta1CamHandle, true);
                    API.RenderScriptCams(true, false, 0, true, false);
                    API.SetCamFov(gta1CamHandle, 50);
                }

                // Get the player's position and speed
                Vector3 playerPosition = playerPed.Position;
                float maxCamHeight = CheckForRoof(playerPosition);
                float playerSpeed = Vector3.Distance(playerPosition, playerPreviousPosition);
                playerPreviousPosition = playerPosition;

                // Determine the target height based on speed with hysteresis
                float nextCamHeight = CurrentCamHeight;

                //Debug.WriteLine($"{CurrentCamHeight} - {maxCamHeight} - {goBackUp} - {obstructedTicks}");

                if (maxCamHeight > CurrentCamHeight + 3f && !goBackUp)
                {
                    obstructedTicks--; // Reset the counter if the player is visible
                    if (obstructedTicks == 20)
                    {
                        goBackUp = true;
                        //Debug.WriteLine($"{goBackUp}");
                        obstructedTicks = 0;
                    }
                }
                else if (maxCamHeight < CurrentCamHeight)
                {
                    if (obstructedTicks >= maxObstructedTicks)
                    {
                        nextCamHeight = maxCamHeight - 1; // Lower the camera if obstructed
                        goBackUp = false;
                        tickBuffer = requiredTicks;
                    }
                    else
                    {
                        obstructedTicks++; // Increment obstruction counter
                    }
                }
                else if (!goBackUp)
                {
                    //ends the setting height here if the cam isn't supposed to go higher yet.
                }
                else if (goBackUp && obstructedTicks != 0)
                {
                    obstructedTicks = 0;
                }
                else if (Game.PlayerPed.IsOnFoot || Game.PlayerPed.IsGettingIntoAVehicle || Game.PlayerPed.IsJumpingOutOfVehicle)
                {
                    nextCamHeight = 20f;
                }
                else if (playerSpeed < MovementLVL0 - 0.02f) // Adding hysteresis buffer
                {
                    nextCamHeight = 50f;
                }
                else if (playerSpeed >= MovementLVL0 && playerSpeed < MovementLVL1 - 0.05f)
                {
                    nextCamHeight = 65f;
                }
                else if (playerSpeed >= MovementLVL1 && playerSpeed < MovementLVL2 - 0.05f)
                {
                    nextCamHeight = 80f;
                }
                else if (playerSpeed >= MovementLVL2)
                {
                    nextCamHeight = 90f;
                }

                // Adjust target height only if it's been consistent for a few ticks
                if (Math.Abs(nextCamHeight - CurrentCamHeight) > 0.1f)
                {
                    tickBuffer++; // Increment buffer when there's a difference
                    if (tickBuffer >= requiredTicks)
                    {
                        CurrentCamHeight = nextCamHeight; // Commit to the new height
                        tickBuffer = 0; // Reset the buffer
                    }
                }
                else
                {
                    tickBuffer = 0; // Reset the buffer if speeds stay consistent
                }


                // Smoothly interpolate the camera height
                dynamicCamHeight = Lerp(dynamicCamHeight, CurrentCamHeight, smoothingFactor);

                // Update camera position and lock orientation to north
                Vector3 cameraPosition = playerPosition + new Vector3(0, 0, dynamicCamHeight);
                API.SetCamCoord(gta1CamHandle, cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
                API.SetCamRot(gta1CamHandle, -90f, -90f, Game.PlayerPed.Heading - 90f, 2); // North-bound orientation
                API.LockMinimapAngle((int)Math.Round(Game.PlayerPed.Heading)); // Sets the map to player heading. Make 0 for north-bound orientation.
            }
            else
            {
                // Turn off the GTA1 Camera and return to the default view
                if (gta1CamHandle != -1)
                {
                    API.UnlockMinimapAngle();
                    API.RenderScriptCams(false, true, 100, true, false);
                    API.DestroyCam(gta1CamHandle, false);
                    gta1CamHandle = -1;
                }
            }

            await Task.FromResult(0); // Yield control back to FiveM runtime
        }

        private float CheckForRoof(Vector3 playerPosition)
        {
            // Perform a raycast upward from the player's position
            Vector3 rayStart = playerPosition + new Vector3(0, 0, 1); // Starting just above the player's head
            Vector3 rayEnd = playerPosition + new Vector3(0, 0, 100f); // Ray cast upwards

            int rayHandle = API.StartShapeTestRay(rayStart.X, rayStart.Y, rayStart.Z, rayEnd.X, rayEnd.Y, rayEnd.Z, (int)IntersectOptions.Everything, Game.PlayerPed.Handle, 7);

            bool hit = false;
            Vector3 hitCoords = Vector3.Zero;
            Vector3 surfaceNormal = Vector3.Zero;
            int entityHit = 0;

            API.GetShapeTestResult(rayHandle, ref hit, ref hitCoords, ref surfaceNormal, ref entityHit);

            if (hit == true)
            {
                float maxCamHeight = hitCoords.Z - playerPosition.Z;
                // If a roof/obstruction is detected, return the hit height (roof height)
                return maxCamHeight;
            }

            // If no roof is detected, return a very high value (no roof)
            return 1000;
        }
        private float Lerp(float start, float end, float percent)
        {
            return start + (end - start) * percent;
        }



        private DateTime reverseCamLockEndTime;
        int previousCam;

        private void SetReverseCam()
        {
            if (!isReverseCamOn)
            {
                previousCam = API.GetFollowPedCamViewMode();
                API.SetFollowVehicleCamViewMode(4); // 4 = 'Cinematic-like' rear view in most cases
                isReverseCamOn = true;
                reverseCamLockEndTime = DateTime.Now.AddSeconds(15);
                API.DisableControlAction(0, 0x3C, true); // Disable camera switching
            }
            else
            {
                API.SetFollowVehicleCamViewMode(previousCam); // Resets to a normal view
                isReverseCamOn = false;
                API.EnableControlAction(0, 0x3C, true); // Re-enable camera controls
            }
        }

        private async void unlockReverseCam()
        {
            while (DateTime.Now < reverseCamLockEndTime)
            {
                await Delay(500);
            }
            isReverseCamOn = false;
            API.SetFollowVehicleCamViewMode(previousCam);
            API.EnableControlAction(0, 0x3C, true); // Re-enable camera controls
        }

        private void BounceVehicle()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null && veh.Exists())
                {
                    Vector3 velocity = veh.Velocity;
                    velocity = -velocity; // Reverse direction
                    veh.Velocity = velocity;

                    // Rotate 180 degrees
                    Vector3 rotation = veh.Rotation;
                    rotation.Z += 180.0f;
                    veh.Rotation = rotation;
                }
            }
        }

        private void PerformKickflip()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null && veh.Exists())
                {
                    Vector3 velocity = veh.Velocity;
                    velocity.Z = velocity.Z + 4;
                    veh.Velocity = velocity;
                    API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 0.0f, 3.6f, 1.8f, 0.0f, 0.0f, 0, true, true, true, false, true);
                }
            }
        }

private int shotsFired = 0;
private int maxShots = 10;
public bool canGunJam = false;
private bool isGunJammed = false;

private async Task MonitorWeaponAndShooting()
{
    var playerPed = Game.PlayerPed;

    // If the weapon is jammed, disable firing
    if (isGunJammed)
    { 
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"isjammed"}});
        API.DisablePlayerFiring(playerPed.Handle, true);

        // Allow unjamming by reloading
        if (API.IsPedReloading(playerPed.Handle)) // Check for reload key press
        {
            isGunJammed = false;
            shotsFired = 0; // Reset shots fired
            canGunJam = false;
            NotificationScript.ShowNotification("~h~~g~Weapon unjammed!~h~");
        }

        await Task.FromResult(0);
        return; // Exit early, gun is jammed
    }

    // Ensure firing is enabled if the gun is not jammed
    //API.DisablePlayerFiring(playerPed.Handle, true);

    // If gun jamming is disabled, return early
    if (!canGunJam)
    {
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"can't jam."}});
        return;
    }

    // Check if the player has a weapon equipped and is on foot
    if (Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed && playerPed.IsOnFoot)
    {
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"holds weapon."}});
        // Monitor for shooting
        if (API.IsPedShooting(Game.PlayerPed.Handle))
        {
           //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"shot"}});
            shotsFired++;
        }

        // Jam the weapon if shots exceed the allowed limit
        if (shotsFired >= maxShots)
        {
            isGunJammed = true;
            NotificationScript.ShowNotification("~h~~r~Weapon jammed!~h~~n~~s~Press R to reload.");
        }
    }
    else
    {
        //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"else."}});
        // Reset state when switching weapons or unequipping
        shotsFired = 0; // Reset the shot counter
        isGunJammed = false;
    }

    await Task.FromResult(0);
}



        private Dictionary<Ped, DateTime> pedTaskEndTimes = new Dictionary<Ped, DateTime>();

        void StartNPCsRunningTowardsPlayer()
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            Ped[] nearbyPeds = World.GetAllPeds()
                .Where(p => p != Game.PlayerPed && p.IsHuman && Vector3.Distance(p.Position, playerPos) < 50f)
                .ToArray();

            foreach (Ped npc in nearbyPeds)
            {
                if (npc != null && !npc.IsInVehicle() && npc.IsAlive)
                {
                    npc.Task.RunTo(playerPos);
                    pedTaskEndTimes[npc] = DateTime.Now.AddSeconds(20); // Store the time to stop tasking them
                }
            }
        }

        private async Task UpdatePedTasks()
        {
            Vector3 playerPos = Game.PlayerPed.Position;

            foreach (var entry in pedTaskEndTimes.ToList()) // Use ToList() to avoid modifying the dictionary during iteration
            {
                Ped npc = entry.Key;
                DateTime endTime = entry.Value;

                if (DateTime.Now < endTime && npc.IsAlive)
                {
                    npc.Task.RunTo(playerPos); // Continuously update task
                }
                else
                {
                    pedTaskEndTimes.Remove(npc); // Remove from dictionary after 20 seconds or if the ped is dead
                }
            }
            await Task.FromResult(0); // Yield control back to FiveM runtime
        }

        void StartCarsDrivingIntoPlayer()
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            Vehicle[] vehicles = World.GetAllVehicles().Where(v => Vector3.Distance(v.Position, playerPos) < 100f).ToArray();
            foreach (Vehicle veh in vehicles)
            {
                if (veh.Driver != null && veh.Driver.IsHuman && veh.Driver.IsAlive)
                {
                    veh.Driver.Task.DriveTo(veh, playerPos, 5f, 100f, 786603);
                }
            }
        }

        async void SpawnImponentFollower()
        {
            int respawnAttempts = 0;

            // Spawn the Imporate Rage NPC on foot near the player
            Ped impPed = await World.CreatePed(new Model("u_m_y_imporage"), Game.PlayerPed.Position + new Vector3(10f, 0f, 0f));

            // Set the NPC to run towards the player's vehicle
            impPed.Task.RunTo(Game.PlayerPed.CurrentVehicle.Position);

            Tick += async () =>
            {
                if (respawnAttempts < 5)
                {
                    if (!impPed.IsAlive) // If Imporate Rage dies, respawn them
                    {
                        respawnAttempts++;
                        impPed.Delete(); // Delete the dead NPC

                        // Spawn a new NPC at a different position
                        impPed = await World.CreatePed(new Model("u_m_y_imporage"), Game.PlayerPed.Position + new Vector3(10f, 0f, 0f));
                        impPed.Task.RunTo(Game.PlayerPed.CurrentVehicle.Position); // Run to the player's vehicle
                    }
                    else if (Vector3.Distance(impPed.Position, Game.PlayerPed.CurrentVehicle.Position) < 5f) // Close enough to the vehicle
                    {
                        // If the NPC reaches the vehicle, enter the driver's seat
                        impPed.Task.EnterVehicle(Game.PlayerPed.CurrentVehicle, VehicleSeat.Driver);

                        await Delay(500); // Wait for the NPC to get in

                        if (impPed.IsInVehicle(Game.PlayerPed.CurrentVehicle))
                        {
                            // If the player is still in the vehicle, drag them out
                            if (Game.PlayerPed.IsInVehicle(Game.PlayerPed.CurrentVehicle))
                            {
                                Game.PlayerPed.Task.LeaveVehicle(); // Force the player to exit the vehicle
                                await Delay(1000); // Wait for the player to leave
                            }

                            // Once the player is out, the NPC will drive the car away
                            impPed.Task.DriveTo(Game.PlayerPed.CurrentVehicle, Game.PlayerPed.Position + new Vector3(100f, 0f, 0f), 20f, 100, 512);
                        }
                    }

                    // If the NPC gets too far from the vehicle or the player, respawn the NPC
                    if (Vector3.Distance(impPed.Position, Game.PlayerPed.CurrentVehicle.Position) > 100f)
                    {
                        respawnAttempts++;
                        impPed.Delete(); // Clean up
                        impPed = await World.CreatePed(new Model("u_m_y_imporage"), Game.PlayerPed.Position + new Vector3(10f, 0f, 0f)); // Respawn the NPC
                        impPed.Task.RunTo(Game.PlayerPed.CurrentVehicle.Position); // Run towards the car again
                    }
                }
            };
        }

        void ChangeCarColor()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                int[] colorIndices = { 88, 89, 91, 92, 135, 136, 137, 138, 145, 41, 42, 55, 83, 81, 140 };
                int randomColor = colorIndices[rand.Next(colorIndices.Length)];
                veh.Mods.PrimaryColor = (VehicleColor)randomColor;
                veh.Mods.SecondaryColor = (VehicleColor)randomColor;
            }
        }

        void PaintAllNearbyVehicles()
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            int[] colorIndices = { 88, 89, 91, 92, 135, 136, 137, 138, 145, 41, 42, 55, 83, 81, 140 };
            int randomColor = colorIndices[rand.Next(colorIndices.Length)];
            foreach (Vehicle veh in vehicles)
            {
                veh.Mods.PrimaryColor = (VehicleColor)randomColor;
                veh.Mods.SecondaryColor = (VehicleColor)randomColor;
            }
        }

        void BreakAllVehicleWindows()
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            foreach (Vehicle veh in vehicles)
            {
                // Loop through all possible windows (0 to 7) and break each one
                for (int i = 0; i < 8; i++)
                {
                    API.SmashVehicleWindow(veh.Handle, i); // Break the window at index i
                }
            }

        }

        async void StartElectricalGlitch()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                for (int i = 0; i < 200; i++)
                {
                    var horn = rand.Next(0, 2);
                    var dooropen = rand.Next(0, 2);
                    API.SetVehicleLights(veh.Handle, rand.Next(0, 3));

                    if (dooropen == 0)
                    {
                        var doorId = rand.Next(0, 7);
                        bool isDoorOpen = API.IsVehicleDoorFullyOpen(veh.Handle, doorId);
                        if (isDoorOpen) API.SetVehicleDoorShut(veh.Handle, doorId, false);
                        if (!isDoorOpen) API.SetVehicleDoorOpen(veh.Handle, doorId, false, true);
                    }
                    if (horn == 1) API.StartVehicleHorn(veh.Handle, 250, 0, false);
                    await Delay(200);
                }
            }
        }

        async void StartRainbowCarEffect()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null && !isStarmodeOn)
            {
                isStarmodeOn = true;
                // int primary = -1;
                // int secondary = -1;
                // API.GetVehicleColours(veh.Handle, ref primary, ref secondary);

                int r = 0;
                int g = 0;
                int b = 0;
                int state = 0; //state: 0 r increases. 1 b decreases. 2 g increases. 3 r decreases. 4 b increases. 5 g decreases. repeat
                starmodeEndTime = DateTime.Now.AddSeconds(rand.Next(10, 20));
                Debug.WriteLine($"starmodeEndTime: {starmodeEndTime}, {DateTime.Now}");


                int rgb = rand.Next(0, 4); //sets a random start colour
                if (rgb == 0) { r = 240; b = 255; state = 0; }
                else if (rgb == 1) { r = 1; b = 255; state = 0; }
                else if (rgb == 2) { r = 255; g = 100; state = 2; }
                else if (rgb == 3) { g = 255; b = 30; state = 4; }
                else if (rgb == 4) { g = 240; b = 255; state = 5; }

                API.SetVehicleNeonLightEnabled(veh.Handle, 0, true);
                API.SetVehicleNeonLightEnabled(veh.Handle, 1, true);
                API.SetVehicleNeonLightEnabled(veh.Handle, 2, true);
                API.SetVehicleNeonLightEnabled(veh.Handle, 3, true);
                API.ToggleVehicleMod(veh.Handle, 22, true); //turns on xenons
                while (DateTime.Now < starmodeEndTime)
                {
                    if (state == 0)
                    {
                        //r++;
                        r = Math.Min(r + (r <= 245 ? 10 : 255 - r), 255);
                        if (r == 255) state = 1;
                    }
                    else if (state == 1)
                    {
                        //b--;
                        b = Math.Min(b - (b >= 10 ? 10 : b), 255);
                        if (b == 0) state = 2;
                    }
                    else if (state == 2)
                    {
                        //g++;
                        g = Math.Min(g + (g <= 245 ? 10 : 255 - g), 255);
                        if (g == 255) state = 3;
                    }
                    else if (state == 3)
                    {
                        //r--;
                        r = Math.Min(r - (r >= 10 ? 10 : r), 255);
                        if (r == 0) state = 4;
                    }
                    else if (state == 4)
                    {
                        //b++;
                        b = Math.Min(b + (b <= 245 ? 10 : 255 - b), 255);
                        if (b == 255) state = 5;
                    }
                    else if (state == 5)
                    {
                        //g--;
                        g = Math.Min(g - (g >= 10 ? 10 : g), 255);
                        if (g == 0) state = 0;
                    }
                    API.SetVehicleCustomPrimaryColour(veh.Handle, r, g, b);
                    API.SetVehicleCustomSecondaryColour(veh.Handle, r, g, b);
                    API.SetVehicleNeonLightsColour(veh.Handle, r, g, b);
                    API.SetVehicleXenonLightsCustomColor(veh.Handle, r, g, b);
                    await Delay(1);
                }
                API.SetVehicleNeonLightEnabled(veh.Handle, 0, false);
                API.SetVehicleNeonLightEnabled(veh.Handle, 1, false);
                API.SetVehicleNeonLightEnabled(veh.Handle, 2, false);
                API.SetVehicleNeonLightEnabled(veh.Handle, 3, false);
                API.ToggleVehicleMod(veh.Handle, 22, false); //turns off xenons
                API.ClearVehicleCustomPrimaryColour(veh.Handle);
                //API.SetVehicleColours(veh.Handle, primary, secondary);
                isStarmodeOn = false;
                VehiclePersistenceClient.lastVehicle = null;
            }
        }


        void TeleportIntoRandomNPCVehicle()
        {
            Vehicle[] vehicles = World.GetAllVehicles().Where(v => v.Driver == null).ToArray();
            if (vehicles.Length > 0)
            {
                Vehicle randomVehicle = vehicles[rand.Next(vehicles.Length)];
                Game.PlayerPed.Task.WarpIntoVehicle(randomVehicle, VehicleSeat.Driver);
            }
        }

        async void ChangeToCompactCar()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                int value = rand.Next(0, 7);
                string compacts = "panto,issi2,prairie,dilettante2,rhapsody,brioso,dilettante";
                string[] compactNames = compacts.Split(',');
                Vector3 playerSpeed = Game.PlayerPed.Velocity;
                Vector3 currentPosition = veh.Position;
                float heading = Game.PlayerPed.Heading;
                veh.Delete();
                var model = new Model(max_Vehicle.VehicleNameToHash[compactNames[value]]);
                Vehicle compactCar = await World.CreateVehicle(model, currentPosition, heading);
                API.SetVehicleEngineOn(compactCar.Handle, true, true, false);
                Game.PlayerPed.Task.WarpIntoVehicle(compactCar, VehicleSeat.Driver);
                bool temp = false;

                compactCar.Velocity = playerSpeed;
                //NotificationScript.ShowNotification($"spawned a " + compactNames[value]);
            }
        }

        bool isMaxSpeedReduced = false;
        Vehicle oldVehicle;
        void ApplySpeedLimiter()
        {
            // Vehicle veh = Game.PlayerPed.CurrentVehicle;
            // if (veh == null)
            // {
            //     isMaxSpeedReduced = false;
            // }
            // else if (oldVehicle == veh && isMaxSpeedReduced)
            // {
            //    veh.MaxSpeed = 200;
            // }
            // else 
            // {
            // isMaxSpeedReduced = true;
            // oldVehicle = veh;
            // veh.MaxSpeed = 0.1f;
            // }

        }

        void SwapCarsWithAnotherPlayer()
        {
            List<Player> otherPlayers = new List<Player>(Players.Where(p => p != Game.Player));
            if (otherPlayers.Count > 0)
            {
                Player randomPlayer = otherPlayers[rand.Next(otherPlayers.Count)];
                Vehicle playerVeh = Game.PlayerPed.CurrentVehicle;
                Vehicle otherVeh = randomPlayer.Character.CurrentVehicle;

                if (playerVeh != null && otherVeh != null)
                {
                    Vector3 tempPos = playerVeh.Position;
                    playerVeh.Position = otherVeh.Position;
                    otherVeh.Position = tempPos;

                    Game.PlayerPed.Task.WarpIntoVehicle(otherVeh, VehicleSeat.Driver);
                    randomPlayer.Character.Task.WarpIntoVehicle(playerVeh, VehicleSeat.Driver);
                }
            }
        }

        public static async void shakeCam() //currently does nothing
        {
            // Possible shake types (updated b617d):  
            // DEATH_FAIL_IN_EFFECT_SHAKE  
            // DRUNK_SHAKE  
            // FAMILY5_DRUG_TRIP_SHAKE  
            // HAND_SHAKE  
            // JOLT_SHAKE  
            // LARGE_EXPLOSION_SHAKE  
            // MEDIUM_EXPLOSION_SHAKE  
            // SMALL_EXPLOSION_SHAKE  
            // ROAD_VIBRATION_SHAKE  
            // SKY_DIVING_SHAKE  
            // VIBRATE_SHAKE  
            int cam = API.GetRenderingCam();

            API.ShakeCam(cam, "VIBRATE_SHAKE", 1);
            await Delay(5000);
            API.StopCamShaking(cam, true);
        }

    }
}