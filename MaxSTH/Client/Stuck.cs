using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using System.Collections.Generic;
using STHMaxzzzie.Client;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class StuckScript : BaseScript
    {
        private int usageCount = 0;
        private DateTime lastUsed = DateTime.MinValue;
        private Vector3 lastPosition;
        bool isRunning;
        public List<int> allowedClassIdForStuckVeh = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 17, 18, 19, 20 };
        public StuckScript()
        {
            API.RegisterKeyMapping("+Stuck", "Vehicle stuck nudge", "keyboard", "u"); // Change "n" to your desired key.
        }

        [EventHandler("StuckCommand")]
        private async void StuckCommand()
        {
            if (isRunning)
            {
                return;
            }
            isRunning = true;
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle == null || !vehicle.Exists())
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be in a car to use /stuck." } });
                isRunning = false;
                return;
            }

            if (RoundHandling.thisClientIsTeam == 1)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You cannot use /stuck as a runner." } });
                isRunning = false;
                return;
            }

            if (Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be in a driver seat to use /stuck." } });
                isRunning = false;
                return;
            }

            var VehClass = API.GetVehicleClass(vehicle.Handle);
            if (!allowedClassIdForStuckVeh.Contains(VehClass))
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be in a car to use /stuck." } });
                isRunning = false;
                return;
            }
            Vector3 speed = API.GetEntityVelocity(vehicle.Handle);
            if (speed.X > 0.2f || speed.Y > 0.2f || speed.Z > 0.2f)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be standing still to use /stuck." } });
                isRunning = false;
                return;
            }
            Vector3 speed2 = Game.PlayerPed.Velocity;
            if (speed2.X >= 0.2f || speed2.Y >= 0.2f || speed2.Z >= 0.2f)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be standing still to use /stuck." } });
                isRunning = false;
                return;
            }

            Vector3 currentPosition = vehicle.Position;
            if ((DateTime.Now - lastUsed).TotalSeconds > 60 || Vector3.Distance(currentPosition, lastPosition) > 1000f)
            {
                usageCount = 0;
            }

            usageCount++;
            lastPosition = currentPosition;
            Vector3 rotation = vehicle.Rotation;
            bool isUpsideDown = rotation.Y > 110 || rotation.Y < -110;

            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"upside down? {isUpsideDown}, {rotation.Y}" } }); //debug
            Vector3 force = new Vector3();
            switch (usageCount)
            {
                case 1:
                    {
                        force = new Vector3(-2, 0, 3f);
                    }
                    //PlaySoundFrontend(-1, "0x0469298F", "vehicles", false);
                    NotificationScript.ShowNotification($"~h~~g~Stuck huh?~s~~n~Let me give you a little nudge soon.");
                    lastUsed = DateTime.Now;
                    break;
                case 2:
                    {
                        force = new Vector3(-3, 0, 5f);
                    }
                    NotificationScript.ShowNotification($"~h~~g~Still stuck?~s~~n~Get ready for a small push.");
                    //PlaySoundFrontend(-1, "CRIME_1_48_RESIST_ARREST_03", "01_crime_1_48_resist_arrest", false);
                    lastUsed = DateTime.Now;
                    break;
                case 3:
                    force = new Vector3(6, 0, 10f);
                    //PlaySoundFrontend(-1, "0x0469298F", "vehicles", false);
                    lastUsed = DateTime.Now;
                    NotificationScript.ShowNotification($"~h~~r~Come on man!~s~~n~Be free already!");

                    break;
                case 4:

                    force = new Vector3(-6, 5, 10f);
                    lastUsed = DateTime.Now;
                    //PlaySoundFrontend(-1, "0x0469298F", "vehicles", false);
                    NotificationScript.ShowNotification($"~h~~g~Stepbro?~s~~n~What are you doing?");

                    break;
                default:
                    NotificationScript.ShowNotification($"~h~~w~I think i helped enough.~s~~n~~h~~r~Wait.");
                    usageCount = 4;
                    isRunning = false;
                    return;
            }
            await Delay(5000);
            if (Vector3.Distance(vehicle.Position, lastPosition) > 1f)
            {
                NotificationScript.ShowNotification($"~h~~w~You moved.~s~~n~~h~~r~Canceling push.");
                isRunning = false;
                return;
            }
            if (Game.PlayerPed.SeatIndex != VehicleSeat.Driver || vehicle != Game.PlayerPed.CurrentVehicle)
            {
                NotificationScript.ShowNotification($"~h~~w~You got out of your vehicle?~s~~n~~h~~r~Kids these days, no patience.");
                isRunning = false;
                return;
            }
            if (isUpsideDown) vehicle.Rotation = new Vector3(rotation.X, 0, rotation.Z);
            API.SetEntityVelocity(vehicle.Handle, force.X, force.Y, force.Z);
            //TriggerEvent("chat:addMessage", new { color = new[] { 0, 255, 0 }, args = new[] { $"/stuck used {usageCount} time(s)." } });
            isRunning = false;
            // Reset after 60 seconds of inactivity
            await Delay(60000);
            if ((DateTime.Now - lastUsed).TotalSeconds > 60)
            {
                usageCount = 0;
            }
        }

        [Command("+Stuck")]
        private void StuckKeyPress()
        {
            if (API.IsPauseMenuActive() || !Game.PlayerPed.IsAlive)
            {
                return;
            }       
                StuckCommand();
        }

        [Command("-Stuck")]
        private void StuckKeyRelease()
        {
            // No action needed for release in this context
        }
    }
}