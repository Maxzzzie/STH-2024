using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Linq;
using STHMaxzzzie.Client;

namespace STHMaxzzzie.Client
{
    public class StreamLootsEffects : BaseScript
    {
        int SLItterateTime = 10;
        bool isSpotlightOn = false;
        bool isPlayerDrunk = false;
        public static bool isGta1CamOn = false;
        bool didGta1CamStartOnce = false;
        DateTime Gta1camEndTime;
        bool isReverseCamOn = false;
        public static bool isStarmodeOn = false; //to not automatically update player colours
        static bool isElectricalGlitchRunning = false;
        DateTime fameStartTime;
        DateTime carFameStartTime;
        static bool isShakeCamOn = false;
        Random rand = new Random();

        public StreamLootsEffects()
        {
            Tick += Spotlight;
            Tick += MonitorWeaponAndShooting;
            Tick += UpdatePedTasks;
            Tick += Gta1Cam;
            //loopThroughSlowly();
            ProcessEffectCue();
        }

        [EventHandler("UpdateSLItterateTime")]
        void UpdateSLItterateTime(int Time)
        {
            SLItterateTime = Time;
        }

        // A list to store effects in the order they were received
        private List<string> EffectCue = new List<string>();


        // Method to handle incoming effects
        [EventHandler("StreamLootsEffect")]
        public void ReceiveStreamLootsEffect(string effectName)
        {
            // Attempt to execute the effect immediately
            if (!Game.PlayerPed.IsAlive)
            {
                EffectCue.Add(effectName);
                Debug.WriteLine($"StreamLootsEvent {effectName} added to cue because player is dead.");
            }
            else if (DoStreamLootsEvent(effectName))
            {
                Debug.WriteLine($"Did {effectName} from the server.");
            }
            else
            {
                EffectCue.Add(effectName);
                Debug.WriteLine($"StreamLootsEvent {effectName} added to cue.");
            }
        }

        // Iterates over the EffectCue periodically, trying each effect
        async void ProcessEffectCue()
        {
            bool DidClearCueAfterRoundAlready = true; //this prevents cue from clearing if it's made after a round has ended alredy. It does it only once at the end of the round basically. This only works for the command added effects.
            while (true)
            {
                if (RoundHandling.gameMode == "none" && EffectCue.Count != 0 && !DidClearCueAfterRoundAlready)
                {
                    Debug.WriteLine($"Game ended. Effect cue cleared. {EffectCue.Count} effects were remaining.");
                    Debug.WriteLine($"Remaining effects: {string.Join(", ", EffectCue)}.");
                    EffectCue.Clear();
                    DidClearCueAfterRoundAlready = true;
                }
                List<string> ItterationEffectCue = EffectCue;
                for (int i = 0; i < ItterationEffectCue.Count; i++)
                {
                    string effect = EffectCue[i];

                    // Try to execute the effect
                    if (DoStreamLootsEvent(effect))
                    {
                        Debug.WriteLine($"Did {effect} from the cue.");
                        // Remove it from the queue if successful
                        EffectCue.RemoveAt(i);
                        break;
                    }
                }
                if (RoundHandling.gameMode != "none")
                {
                    DidClearCueAfterRoundAlready = false;
                }
                await Delay(SLItterateTime * 1000);
            }
        }


        bool DoStreamLootsEvent(string type)
        {
            bool didWork = false;
            if (type == "cleartires")
            {
                ClearAllTires();
                didWork = true;
            }
            // else if (type == "spotlight")
            // {
            //     isSpotlightOn = !isSpotlightOn;
            // }
            else if (type == "burstsome")
            {
                didWork = BreakSomePlayerTires();
            }
            else if (type == "burstall")
            {
                didWork = BreakAllPlayersTires();
            }
            else if (type == "drunk")
            {
                if (!isPlayerDrunk)
                {
                    setPlayerDrunk();
                    didWork = true;
                }
            }
            else if (type == "stop")
            {
                didWork = stopPlayerOnTheSpot();
            }
            else if (type == "speed")
            {
                didWork = givePlayerLotsOfForwardMomentum();
            }
            else if (type == "launch")
            {
                didWork = launchPlayerUp();
            }
            else if (type == "gta1cam")
            {
                if (!isGta1CamOn && !isShakeCamOn)
                {
                    Gta1camEndTime = DateTime.Now.AddSeconds(rand.Next(20, 50));
                    isGta1CamOn = true;
                    didWork = true;
                }
            }
            // else if (type == "reversecam")
            // {
            //     SetReverseCam();
            // }
            else if (type == "bounce")
            {
                didWork = BounceVehicle();
            }
            else if (type == "kickflip")
            {
                didWork = PerformKickflip();
            }
            else if (type == "gunjam")
            {
                if (!canGunJam)
                {
                    canGunJam = true;
                    maxShots = rand.Next(2, 15);
                    didWork = true;
                }
            }
            // else if (type == "fame")
            // {
            //     fameStartTime = DateTime.Now;
            //     StartNPCsRunningTowardsPlayer();
            // }
            // else if (type == "carfame")
            // {
            //     carFameStartTime = DateTime.Now;
            //     StartCarsDrivingIntoPlayer();
            // }
            // else if (type == "imponent")
            // {
            //     SpawnImponentFollower();
            // }
            else if (type == "paint")
            {
                didWork = ChangeCarColor();
            }
            else if (type == "paintall") //works best only runner does it. Otherwise you get desync issues.
            {
                PaintAllNearbyVehicles();
                didWork = true;
            }
            // else if (type == "crack")
            // {
            //     BreakAllVehicleWindows();
            // }
            else if (type == "electricalglitch")
            {
                if (!isElectricalGlitchRunning && Game.PlayerPed.IsInVehicle())
                {
                    StartElectricalGlitch();
                    didWork = true;
                }
            }
            else if (type == "starmode")
            {
                if (!isStarmodeOn)
                {
                    isStarmodeOn = true;
                    StartRainbowCarEffect();
                    didWork = true;
                }
            }
            // else if (type == "carborrow")
            // {
            //     TeleportIntoRandomNPCVehicle();
            // }
            else if (type == "compacted")
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    ChangeToCompactCar();
                    didWork = true;
                }
            }
            else if (type == "supered")
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    ChangeToSuperCar();
                    didWork = true;
                }
            }
            else if (type == "couped")
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    ChangeToCoupeCar();
                    didWork = true;
                }
            }
            else if (type == "shitboxed")
            {
                if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle.Model.Hash != (uint)API.GetHashKey("voodoo2"))
                {
                    ChangeToShitBoxCar();
                    didWork = true;
                }
            }
            else if (type == "boated")
            {
                if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.CurrentVehicle.Model.Hash != (uint)API.GetHashKey("jetmax"))
                {
                    ChangeToBoat();
                    didWork = true;
                }
            }
            // else if (type == "speedlimiter")
            // {
            //     if (Game.PlayerPed.IsInVehicle())
            //     {
            //     ApplySpeedLimiter();
            // }
            // else if (type == "carswap")
            // {
            //     SwapCarsWithAnotherPlayer();
            // }
            else if (type == "shake")
            {
                if (!isGta1CamOn && !isShakeCamOn)
                {
                    shakeCam();
                    didWork = true;
                }
            }
            else if (type == "locationchat")
            {
                rightLocationChat();
                didWork = true;
            }
            else if (type == "fix")
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    FixVehicle();
                    didWork = true;
                }
            }
            else
            {
                Debug.WriteLine("StreamLootsEffect doesn't exist. Not cued.");
            }
            return didWork;
        }

        void ClearAllTires() //works best when targeting everyone
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

        public List<int> vehiclesWith2Tires = new List<int> { 8, 13 };
        public List<int> vehiclesWith4Tires = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 17, 18, 19 };

        //eturns an int Vehicle Classes: 0: Compacts 1: Sedans 2: SUVs 3: Coupes 4: Muscle 5: Sports Classics 6: Sports 7: Super 8: Motorcycles 
        //9: Off-road 10: Industrial 11: Utility 12: Vans 13: Cycles 14: Boats 15: Helicopters 16: Planes 17: Service 18: Emergency 19: Military 
        //20: Commercial 21: Trains 22: Open Wheel char buffer[128]

        bool BreakSomePlayerTires() //works only when player is in a vehicle and that vehicle has some tires to pop. Is compatible with everything else. If it doesn't run it should try again later.
        {
            bool didIPopOne = false;
            if (!Game.PlayerPed.IsInVehicle())
            {
                return didIPopOne;
            }
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            var VehClass = API.GetVehicleClass(veh.Handle);
            if (!vehiclesWith2Tires.Contains(VehClass) && !vehiclesWith4Tires.Contains(VehClass) && VehClass != 20)
            {
                return didIPopOne;
            }

            //0 = wheel_lf / bike, plane or jet front 
            //1 = wheel_rf 
            //2 = wheel_lm / in 6 wheels trailer, plane or jet is first one on left 
            //3 = wheel_rm / in 6 wheels trailer, plane or jet is first one on right 
            //4 = wheel_lr / bike rear / in 6 wheels trailer, plane or jet is last one on left 
            //5 = wheel_rr / in 6 wheels trailer, plane or jet is last one on right 
            //45 = 6 wheels trailer mid wheel left 
            //47 = 6 wheels trailer mid wheel right

            int again = 0;
            List<int> alreadyPopped = new List<int>();

            if (vehiclesWith2Tires.Contains(VehClass)) //2 tired vehicles
            {
                if (API.IsVehicleTyreBurst(veh.Handle, 0, false) && API.IsVehicleTyreBurst(veh.Handle, 4, false))
                {
                    Debug.WriteLine($"Skipping burstsome, both tires are already popped.");
                    return didIPopOne;
                }
                else
                {
                    int index = rand.Next(0, 2);
                    Debug.WriteLine($"index = {index}");
                    if ((!API.IsVehicleTyreBurst(veh.Handle, 0, false) && index == 0) || (API.IsVehicleTyreBurst(veh.Handle, 4, false) && index == 1)) //check for index 1 as that should burst back tire. Ingame that is tire 45
                    {
                        API.SetVehicleTyreBurst(veh.Handle, 0, true, 1000);
                    }
                    else //if rand.Next produces index 1. Or if index 0 is already popped beforehand.
                    {
                        API.SetVehicleTyreBurst(veh.Handle, 4, true, 1000);
                        //index 4 targets the back wheel of a bike. Tire 0 is front wheel so doesn't neeed changing.
                    }
                    didIPopOne = true;
                }
            }

            else if (vehiclesWith4Tires.Contains(VehClass)) //4 tired vehicles (mostly)
            {
                Debug.WriteLine($"4 tires");

                if (API.IsVehicleTyreBurst(veh.Handle, 0, false)) alreadyPopped.Add(0);
                if (API.IsVehicleTyreBurst(veh.Handle, 1, false)) alreadyPopped.Add(1);
                if (API.IsVehicleTyreBurst(veh.Handle, 4, false)) alreadyPopped.Add(4);
                if (API.IsVehicleTyreBurst(veh.Handle, 5, false)) alreadyPopped.Add(5);
                if (alreadyPopped.Count == 4)
                {
                    Debug.WriteLine($"Skipping burstsome. All 4 tires are already popped.");
                    return didIPopOne;
                }


                while (again == 0 && alreadyPopped.Count < 4)
                {
                    int index = rand.Next(0, 4);
                    if (index == 2 || index == 3)
                    {
                        index = index + 2; //tire 2 and 3 are for vehicles with more tires. tire 4 and 5 are rear left and rear right of a vehicle with 4 tires. Thats why i add 2 to it.
                    }
                    if (alreadyPopped.Contains(index))
                    {
                        Debug.WriteLine($"skipping {index}, is already popped.");
                        continue;
                    }
                    Debug.WriteLine($"popping {index}");
                    API.SetVehicleTyreBurst(veh.Handle, index, true, 1000);
                    again = rand.Next(0, 4); //25% chance to pop another tire.
                    alreadyPopped.Add(index);
                    Debug.WriteLine($"count {alreadyPopped.Count}, again = {again} ");
                }
            }


            else if (VehClass == 20) //more tired vehicles
            {

                if (API.IsVehicleTyreBurst(veh.Handle, 0, false)) alreadyPopped.Add(0);
                if (API.IsVehicleTyreBurst(veh.Handle, 1, false)) alreadyPopped.Add(1);
                if (API.IsVehicleTyreBurst(veh.Handle, 2, false)) alreadyPopped.Add(2);
                if (API.IsVehicleTyreBurst(veh.Handle, 3, false)) alreadyPopped.Add(3);
                if (API.IsVehicleTyreBurst(veh.Handle, 4, false)) alreadyPopped.Add(4);
                if (API.IsVehicleTyreBurst(veh.Handle, 5, false)) alreadyPopped.Add(5);

                if (alreadyPopped.Count == 6)
                {
                    Debug.WriteLine($"Skipping burstsome. All 6 tires are already popped.");
                    return didIPopOne;
                }


                while (again == 0 && alreadyPopped.Count < 6)
                {
                    int index = rand.Next(0, 6);
                    if (alreadyPopped.Contains(index))
                    {
                        Debug.WriteLine($"skipping {index}, is already popped.");
                        continue;
                    }
                    Debug.WriteLine($"popping {index}");
                    API.SetVehicleTyreBurst(veh.Handle, index, true, 1000);
                    again = rand.Next(0, 2); //50% chance to pop another tire.
                    alreadyPopped.Add(index);
                    Debug.WriteLine($"count {alreadyPopped.Count}, again = {again}");
                }
            }
            return didIPopOne;

            // test for what tire has what id. Set the function to async void.
            // int x = 0;
            // Vehicle veh = Game.PlayerPed.CurrentVehicle;
            // while (x < 100)
            // {
            //     API.SetVehicleTyreBurst(veh.Handle, x, true, 1000);
            //     Debug.WriteLine($"burst {x}");
            //     x ++;
            //     await Delay(2000);
            // }
        }

        bool BreakAllPlayersTires() //only runs if player is in vehicle and has tires left to pop. Otherwise should try again later.
        {
            bool didIBurstAll = false;
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                var VehClass = API.GetVehicleClass(veh.Handle);

                if (vehiclesWith2Tires.Contains(VehClass)) //2 tired vehicles (mostly)
                {

                    if (API.IsVehicleTyreBurst(veh.Handle, 0, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 4, false))
                    {
                        Debug.WriteLine($"All tires are already popped. Skipping BurstAll.");
                        return didIBurstAll;
                    }
                    API.SetVehicleTyreBurst(veh.Handle, 0, true, 1000);
                    API.SetVehicleTyreBurst(veh.Handle, 4, true, 1000);
                    didIBurstAll = true;
                }
                else if (vehiclesWith4Tires.Contains(VehClass)) //4 tired vehicles (mostly)
                {

                    if (API.IsVehicleTyreBurst(veh.Handle, 0, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 1, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 4, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 5, false))
                    {
                        Debug.WriteLine($"All tires are already popped. Skipping BurstAll.");
                        return didIBurstAll;
                    }
                    {
                        API.SetVehicleTyreBurst(veh.Handle, 0, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 1, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 4, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 5, true, 1000);
                        didIBurstAll = true;
                    }
                }
                else if (VehClass == 20)
                {

                    if (API.IsVehicleTyreBurst(veh.Handle, 0, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 1, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 2, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 3, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 4, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 5, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 6, false) &&
                        API.IsVehicleTyreBurst(veh.Handle, 7, false))
                    {
                        Debug.WriteLine($"All tires are already popped. Skipping BurstAll.");
                        return didIBurstAll;
                    }
                    {
                        API.SetVehicleTyreBurst(veh.Handle, 0, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 1, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 2, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 3, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 4, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 5, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 6, true, 1000);
                        API.SetVehicleTyreBurst(veh.Handle, 7, true, 1000);
                        didIBurstAll = true;
                    }
                }
            }
            return didIBurstAll;
        }

        async void setPlayerDrunk()
        // {
        // int handle = Game.PlayerPed.Handle;
        // API.SetAudioSpecialEffectMode(7);
        // API.SetPedIsDrunk(handle ,isPlayerDrunk);
        // API.
        {
            //Debug.WriteLine($"PlayerDrunk started.");
            isPlayerDrunk = true;
            DateTime drunkEndTime;
            drunkEndTime = DateTime.Now.AddSeconds(rand.Next(15, 40));

            // Apply the timecycle modifier for a drunk effect
            API.SetTimecycleModifier("spectator5");

            // Increase the intensity of the effect (lower values = stronger effect)
            API.SetTimecycleModifierStrength(1.0f);

            // Apply camera shake for added realism
            API.ShakeGameplayCam("DRUNK_SHAKE", 1.0f);

            // Optionally, apply screen effects
            //API.StartScreenEffect("DrugsTrevorClownsFight", 0, true);

            while (DateTime.Now < drunkEndTime)
            {
                //Debug.WriteLine($"While drunk.");
                await Delay(100);
            }
            // Clear timecycle and camera effects
            API.ClearTimecycleModifier();
            API.StopGameplayCamShaking(true);
            //API.StopScreenEffect("DrugsTrevorClownsFight");
            isPlayerDrunk = false;
            //Debug.WriteLine($"Ending drunk");
        }

        private bool stopPlayerOnTheSpot()
        {
            bool didStopOnTheSpot = false;
            if (Game.PlayerPed.IsInVehicle() && (Game.PlayerPed.Velocity.X > 1 || Game.PlayerPed.Velocity.Y > 1 || Game.PlayerPed.Velocity.Z > 1))
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                API.SetEntityVelocity(veh.Handle, 0, 0, 0);
                didStopOnTheSpot = true;
            }
            return didStopOnTheSpot;
        }

        private bool givePlayerLotsOfForwardMomentum()
        {
            bool didGiveLotsOfForwardMomentum = false;
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 120.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, true);
                didGiveLotsOfForwardMomentum = true;
            }
            return didGiveLotsOfForwardMomentum;
        }

        private bool launchPlayerUp()
        {
            bool didLaunchPlayer = false;
            if (Game.PlayerPed.IsAlive)
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    Vehicle veh = Game.PlayerPed.CurrentVehicle;
                    API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 0.0f, 120.0f, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, true);
                    didLaunchPlayer = true;
                }
                else
                {
                    API.ApplyForceToEntity(Game.PlayerPed.Handle, 1, 0.0f, 0.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, true);
                    Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);
                    didLaunchPlayer = true;
                }
            }
            return didLaunchPlayer;
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
            if (DateTime.Now > Gta1camEndTime) //turns off the gta1cam.
            {
                isGta1CamOn = false;
                didGta1CamStartOnce = false;
            }

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
                    didGta1CamStartOnce = true;
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
            if (!didGta1CamStartOnce && isGta1CamOn)
            {
                Gta1camEndTime = DateTime.Now.AddSeconds(rand.Next(20, 50));
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

        private bool BounceVehicle()
        {
            bool didBounce = false;
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
                    didBounce = true;
                }
            }
            return didBounce;
        }

        private bool PerformKickflip()
        {
            bool didKickFlip = false;
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null && veh.Exists())
                {
                    Vector3 velocity = veh.Velocity;
                    velocity.Z = velocity.Z + 4;
                    veh.Velocity = velocity;
                    API.ApplyForceToEntity(veh.Handle, 1, 0.0f, 0.0f, 3.6f, 1.8f, 0.0f, 0.0f, 0, true, true, true, false, true);
                    didKickFlip = true;
                }
            }
            return didKickFlip;
        }

        private int shotsFired = 0;
        private int maxShots = 10;
        public bool canGunJam = false;
        private bool isGunJammed = false;
        private dynamic jammedWeapon = null;

        private async Task MonitorWeaponAndShooting()
        {
            var playerPed = Game.PlayerPed;

            // If the weapon is jammed, disable firing
            if (isGunJammed)
            {
                //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"isjammed"}});
                API.DisablePlayerFiring(playerPed.Handle, true);
                // if (API.IsControlJustPressed(0, (int)Control.Attack)) // Check for attack key press
                // {
                //     API.PlaySoundFrontend(-1, "WEAPON_EMPTY", "HUD_LIQUOR_STORE_SOUNDSET", false);
                // }
                // Allow unjamming by reloading, switching weapon, or getting in vehicle
                if (API.IsPedReloading(playerPed.Handle) || API.IsControlJustPressed(0, 24) || !Game.PlayerPed.IsOnFoot || Game.PlayerPed.Weapons.Current.Hash != jammedWeapon) // Check for reload key press
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
                int ammoCount = 0;
                int weaponHash = (int)Game.PlayerPed.Weapons.Current.Hash;
                API.GetAmmoInClip(Game.PlayerPed.Handle, (uint)weaponHash, ref ammoCount);
                if (API.IsPedShooting(Game.PlayerPed.Handle) && ammoCount > 1)
                {
                    //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"shot"}});
                    shotsFired++;
                }

                // Jam the weapon if shots exceed the allowed limit
                if (shotsFired >= maxShots)
                {
                    isGunJammed = true;
                    jammedWeapon = Game.PlayerPed.Weapons.Current.Hash;
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

        bool ChangeCarColor()
        {
            bool changeCarColorWorked = false;
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                int[] colorIndices = { 88, 89, 91, 92, 135, 136, 137, 138, 145, 41, 42, 55, 83, 81, 140 };
                int randomColor = colorIndices[rand.Next(colorIndices.Length)];
                veh.Mods.PrimaryColor = (VehicleColor)randomColor;
                veh.Mods.SecondaryColor = (VehicleColor)randomColor;
                changeCarColorWorked = true;
            }
            return changeCarColorWorked;
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
            isElectricalGlitchRunning = true;
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                for (int i = 0; i < 150; i++)
                {
                    var horn = rand.Next(0, 5);
                    var dooropen = rand.Next(0, 15);
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
            isElectricalGlitchRunning = false;
        }

        async void StartRainbowCarEffect()
        {
            bool startedStarMode = false;
            while (isStarmodeOn && !startedStarMode)
            {
                if (Game.PlayerPed.IsInVehicle())
                {
                    startedStarMode = true;
                }
                await Delay(100);
            }
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null && isStarmodeOn)
            {
                int primary = -1;
                int secondary = -1;
                API.GetVehicleColours(veh.Handle, ref primary, ref secondary);

                int r = 0;
                int g = 0;
                int b = 0;
                int state = 0; //state: 0 r increases. 1 b decreases. 2 g increases. 3 r decreases. 4 b increases. 5 g decreases. repeat
                DateTime starmodeEndTime;
                starmodeEndTime = DateTime.Now.AddSeconds(rand.Next(15, 40));
                Debug.WriteLine($"starmodeEndTime: {starmodeEndTime}, {DateTime.Now}");

                int rgb = rand.Next(0, 5); //sets a random start colour
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
                API.ClearVehicleCustomSecondaryColour(veh.Handle);
                API.SetVehicleColours(veh.Handle, primary, secondary);
                isStarmodeOn = false;
                VehiclePersistenceClient.lastVehicle = null;
            }
            else
            {//in case something doesn't start starmode
                isStarmodeOn = false;
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
                await Delay(5);
                API.SetVehicleCurrentRpm(compactCar.Handle, 1.0f);
                API.SetVehicleForwardSpeed(compactCar.Handle, playerSpeed.Length());
                compactCar.Velocity = playerSpeed;
                //NotificationScript.ShowNotification($"spawned a " + compactNames[value]);
            }
        }

        async void ChangeToCoupeCar()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                int value = rand.Next(0, 7);
                string coupes = "zion,sentinel,exemplar,felon,oracle2,oracle,windsor";
                string[] compactNames = coupes.Split(',');
                Vector3 playerSpeed = Game.PlayerPed.Velocity;
                Vector3 currentPosition = veh.Position;
                float heading = Game.PlayerPed.Heading;
                veh.Delete();
                var model = new Model(max_Vehicle.VehicleNameToHash[compactNames[value]]);
                Vehicle coupeCar = await World.CreateVehicle(model, currentPosition, heading);
                API.SetVehicleEngineOn(coupeCar.Handle, true, true, false);
                Game.PlayerPed.Task.WarpIntoVehicle(coupeCar, VehicleSeat.Driver);
                await Delay(5);
                API.SetVehicleCurrentRpm(coupeCar.Handle, 1.0f);
                API.SetVehicleForwardSpeed(coupeCar.Handle, playerSpeed.Length());
                coupeCar.Velocity = playerSpeed;
                //NotificationScript.ShowNotification($"spawned a " + compactNames[value]);
            }
        }

        async void ChangeToSuperCar()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                int value = rand.Next(0, 6);
                string supers = "bullet,adder,entityxf,infernus,voltic,t20";
                string[] compactNames = supers.Split(',');
                Vector3 playerSpeed = Game.PlayerPed.Velocity;
                Vector3 currentPosition = veh.Position;
                float heading = Game.PlayerPed.Heading;
                veh.Delete();
                var model = new Model(max_Vehicle.VehicleNameToHash[compactNames[value]]);
                Vehicle superCar = await World.CreateVehicle(model, currentPosition, heading);
                API.SetVehicleEngineOn(superCar.Handle, true, true, false);
                Game.PlayerPed.Task.WarpIntoVehicle(superCar, VehicleSeat.Driver);
                await Delay(5);
                API.SetVehicleCurrentRpm(superCar.Handle, 1.0f);
                API.SetVehicleForwardSpeed(superCar.Handle, playerSpeed.Length());
                superCar.Velocity = playerSpeed;
                //NotificationScript.ShowNotification($"spawned a " + compactNames[value]);
            }
        }
        async void ChangeToShitBoxCar()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                Vector3 playerSpeed = Game.PlayerPed.Velocity;
                Vector3 currentPosition = veh.Position;
                float heading = Game.PlayerPed.Heading;
                veh.Delete();
                var model = new Model(max_Vehicle.VehicleNameToHash["voodoo2"]);
                Vehicle shitbox = await World.CreateVehicle(model, currentPosition, heading);
                API.SetVehicleEngineOn(shitbox.Handle, true, true, false);
                Game.PlayerPed.Task.WarpIntoVehicle(shitbox, VehicleSeat.Driver);
                await Delay(5);
                API.SetVehicleCurrentRpm(shitbox.Handle, 1.0f);
                API.SetVehicleForwardSpeed(shitbox.Handle, playerSpeed.Length());
                shitbox.Velocity = playerSpeed;
                //NotificationScript.ShowNotification($"spawned a " + compactNames[value]);
            }
        }

        async void ChangeToBoat()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            if (veh != null)
            {
                Vector3 playerSpeed = Game.PlayerPed.Velocity;
                Vector3 currentPosition = veh.Position;
                float heading = Game.PlayerPed.Heading;
                veh.Delete();
                var model = new Model(max_Vehicle.VehicleNameToHash["jetmax"]);
                Vehicle jetmax = await World.CreateVehicle(model, currentPosition, heading);
                API.SetVehicleEngineOn(jetmax.Handle, true, true, false);
                Game.PlayerPed.Task.WarpIntoVehicle(jetmax, VehicleSeat.Driver);
                await Delay(5);
                API.SetVehicleCurrentRpm(jetmax.Handle, 1.0f);
                API.SetVehicleForwardSpeed(jetmax.Handle, playerSpeed.Length());
                jetmax.Velocity = playerSpeed;
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
            isShakeCamOn = true;
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
            for (int i = 0; i < 7; i++)
            {
                API.ShakeGameplayCam("LARGE_EXPLOSION_SHAKE", 1);
                await Delay(900);
            }
            API.StopGameplayCamShaking(false);
            isShakeCamOn = false;
        }

        public static void rightLocationChat()
        {
            string[] trimmedClosestCalloutName = Callouts.closestCalloutName.Split('*');
            TriggerServerEvent("SendSLChat", Game.Player.ServerId, $"Let me give you a hint. I'm at {trimmedClosestCalloutName[0]}.");
        }

        private List<string> fixLines = new List<string>{
              $"{Game.PlayerPed.Handle}'s car: fresh off the assembly line!",
                "What damage? I see perfection.",
                "From crumpled to crisp in 10 seconds flat.",
                "Who needs insurance when you have me?",
                "Not a scratch… probably.",
                "Brand new! If you squint.",
                "This car’s got that ‘just-fixed’ smell.",
                "Bent, broken, battered—never heard of them.",
                "Certified dent-free... until next time.",
                "You break it, I fake it.",
                "It’s like it never happened. Trust me.",
                "Wreckage? More like wreck-LESS now.",
                "Built tougher, shinier, and with 90% less duct tape.",
                "Damage control complete. Time for damage 2.0?",
                "As good as new... or at least looks that way.",
                "This car went from disaster to dazzling.",
                "From scrapheap to street beast!",
                "Some call it a miracle; I call it talent.",
                "This car’s glow-up is Oscar-worthy.",
                "Your car, now officially un-wrecked.",
                "Once a jalopy, now a trophy.",
                "Fixed faster than your insurance can say ‘premium.’",
                "Wrecked? Nah, it’s just resting beautifully.",
                "Just like magic, but with wrenches.",
                "Rebuilt stronger... mostly for show.",
                "This car is so fixed, it’s practically cheating.",
                "A little spit, a lot of polish, and voila!",
                "You’re welcome, [Name]. Try not to ruin it again.",
                "This car has risen from the ashes. Again.",
                "Rolling out of here like it’s prom night."
        };
        void FixVehicle()
        {
            Game.PlayerPed.CurrentVehicle.Repair();
            string message = fixLines[rand.Next(0, fixLines.Count)];
            NotificationScript.ShowNotification($"Fixed: {message}");
        }

    }
}