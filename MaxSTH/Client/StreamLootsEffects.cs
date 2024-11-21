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
            Tick += OnTick;
        }

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
                GunJamEffect();
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
            if (type == "pebble")
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
                API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 0.0f, 80.0f, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, true);
            }
        }

        private async Task OnTick() //spotlight
        {
            UpdatePedTasks(); // Update NPC tasks regularly
            CheckReload();    // Check for reload to unjam gun
            if (isSpotlightOn)
            // Get the player's current position
            {
                Vector3 playerPosition = Game.PlayerPed.Position;

                // Define the spotlight parameters
                Vector3 lightPosition = playerPosition + new Vector3(0, 0, 15.0f); // 15 units above the player
                Vector3 direction = new Vector3(0, 0, -1); // Pointing straight down
                float distance = 100.0f; // Maximum distance the light reaches
                int r = 255, g = 255, b = 255; // White light color
                float brightness = 5.0f; // Intensity of the light
                float roundness = 1.0f; // Spot size and softness
                float radius = 25.0f; // Radius of the spotlight
                float fadeout = 5.0f; // How the light fades out at the edges

                // Draw the spotlight
                API.DrawSpotLightWithShadow(lightPosition.X, lightPosition.Y, lightPosition.Z, direction.X, direction.Y, direction.Z,
                                  r, g, b, distance, brightness, roundness, radius, fadeout, 1);

            }
            if (isGta1CamOn)
            {
                Vector3 playerPosition = Game.PlayerPed.Position;
                Vector3 cameraPosition = playerPosition + new Vector3(0, 0, 50.0f); // Camera above the player

                API.SetCamCoord(API.GetRenderingCam(), cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
                API.PointCamAtEntity(API.GetRenderingCam(), Game.PlayerPed.Handle, 0, 0, 0, true);
            }
            await Task.FromResult(0); // Yield control back to FiveM runtime
        }

        private bool isCameraLocked = false;
        private DateTime cameraLockEndTime;
        private void SetReverseCam()
        {

            if (isGta1CamOn)
            {
                Vector3 playerPosition = Game.PlayerPed.Position;
                Vector3 cameraPosition = playerPosition + new Vector3(0, 0, 50.0f); // Camera above the player

                // Create a new camera if it doesn't exist
                int camHandle = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                API.SetCamCoord(camHandle, cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
                API.PointCamAtEntity(camHandle, Game.PlayerPed.Handle, 0, 0, 0, true);

                API.RenderScriptCams(true, false, 0, true, false); // Activate the new camera
                API.DestroyCam(API.GetRenderingCam(), false); // Destroy the previous camera to avoid conflicts

                isCameraLocked = true;
                cameraLockEndTime = DateTime.Now.AddSeconds(15);
            }

            // Prevent camera switching for 15 seconds
            if (isCameraLocked && DateTime.Now < cameraLockEndTime)
            {
                API.DisableControlAction(0, 0x3C, true); // Disable camera switching input
            }
            else
            {
                isCameraLocked = false;
                API.EnableControlAction(0, 0x3C, true); // Enable camera switching after lock period
            }

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
        private int jamShotLimit = 0;
        private bool isGunJammed = false;

        void GunJamEffect()
        {
            if (!isGunJammed)
            {
                jamShotLimit = rand.Next(1, 7); // Set random shot limit
                shotsFired = 0; // Reset the shot counter
            }

            if (shotsFired < jamShotLimit)
            {
                API.TaskShootAtCoord(Game.PlayerPed.Handle,
                                     Game.PlayerPed.Position.X + rand.Next(-5, 5),
                                     Game.PlayerPed.Position.Y + rand.Next(-5, 5),
                                     Game.PlayerPed.Position.Z,
                                     1000,
                                     (uint)WeaponHash.Pistol);
                shotsFired++;
            }

            if (shotsFired >= jamShotLimit)
            {
                isGunJammed = true;
                API.SetAmmoInClip(Game.PlayerPed.Handle, (uint)WeaponHash.Pistol, 0); // Jam the gun
            }
        }

        void CheckReload()
        {
            if (isGunJammed && API.IsControlJustPressed(0, (int)Control.Reload)) // Check for reload key press
            {
                isGunJammed = false;
            }
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

        void UpdatePedTasks()
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
                Vector3 currentPosition = veh.Position;
                veh.Delete();
                Vehicle compactCar = await World.CreateVehicle(VehicleHash.Panto, currentPosition);
                Game.PlayerPed.Task.WarpIntoVehicle(compactCar, VehicleSeat.Driver);
            }
        }

        void ApplySpeedLimiter()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                veh.MaxSpeed = 20f; // Limit speed to 20 units
            }
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
    }
}