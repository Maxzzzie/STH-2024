using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class PointingScript : BaseScript
    {
        private bool mpPointing = false;
        private bool keypressIsOn = false;

        public PointingScript()
        {
            API.RegisterKeyMapping("+Pointing", "Pointing finger", "keyboard", "b");
            Tick += OnTick;
        }

        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        [Command("+Pointing")]
        private void Pointing()
        {
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Keypress is on." } }); debug
            // Check if the player is typing in chat or if the game is paused
            if (API.IsPauseMenuActive() || keypressIsOn)
            {
                return;
            }
            keypressIsOn = true;
            if (!mpPointing && Game.PlayerPed.IsOnFoot && Game.PlayerPed.IsAlive)
            {
                //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"startpointing" } }); debug
                StartPointing();
                mpPointing = true;
            }
            else if (mpPointing)
            {
                //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"stoppointing" } }); debug
                StopPointing();
                mpPointing = false;
            }
        }

        [Command("-Pointing")]
        void setMapSizeIsUnpressed()  //prevents an error msg and prevents the starter from running twice.
        {
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Keypress is off" } });debug
            keypressIsOn = false;
        }

        private async Task OnTick()
        {
            if (!mpPointing)
            {
                return;
            }
            // Stop the animation if the player switches to a weapon or isn't on foot
            if (mpPointing && (Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed || !Game.PlayerPed.IsOnFoot))
            {
                StopPointing();
                mpPointing = false;
            }

            if (API.IsTaskMoveNetworkActive(Game.PlayerPed.Handle))
            {
                int ped = Game.PlayerPed.Handle;
                float camPitch = GameplayCamera.RelativePitch;
                camPitch = Clamp(camPitch, -70.0f, 42.0f);
                camPitch = (camPitch + 70.0f) / 112.0f;

                float camHeading = GameplayCamera.RelativeHeading;
                camHeading = Clamp(camHeading, -180.0f, 180.0f);
                camHeading = (camHeading + 180.0f) / 360.0f;

                var coords = API.GetOffsetFromEntityInWorldCoords(ped,
                    (float)(Math.Cos(camHeading) * -0.2 - Math.Sin(camHeading) * (0.4 * camHeading + 0.3)),
                    (float)(Math.Sin(camHeading) * -0.2 + Math.Cos(camHeading) * (0.4 * camHeading + 0.3)),
                    0.6f);

                int ray = API.StartShapeTestRay(coords.X, coords.Y, coords.Z - 0.2f, coords.X, coords.Y, coords.Z + 0.2f, 95, ped, 7);
                bool blocked = false;
                Vector3 hitPosition = Vector3.Zero;
                Vector3 surfaceNormal = Vector3.Zero;
                int hitEntity = 0;
                API.GetShapeTestResult(ray, ref blocked, ref hitPosition, ref surfaceNormal, ref hitEntity);

                API.SetTaskMoveNetworkSignalFloat(ped, "Pitch", camPitch);
                API.SetTaskMoveNetworkSignalFloat(ped, "Heading", camHeading * -1.0f + 1.0f);
                API.SetTaskMoveNetworkSignalBool(ped, "isBlocked", blocked);
                API.SetTaskMoveNetworkSignalBool(ped, "isFirstPerson", API.GetFollowPedCamViewMode() == 4);
            }
            await Task.FromResult(0);
        }

        private async void StartPointing()
        {
            int ped = Game.PlayerPed.Handle;

            // Check if the player is holding a weapon and set them to unarmed
            if (Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed)
            {
                Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
            }
            //NotificationScript.ShowNotification($"anim@mp dict triedloading"); //debug
            API.RequestAnimDict("anim@mp_point");
            while (!API.HasAnimDictLoaded("anim@mp_point"))
            {
                await Delay(5);
            }
            //NotificationScript.ShowNotification($"startpointing past while"); //debug
            API.SetPedCurrentWeaponVisible(ped, false, true, true, true);
            API.SetPedConfigFlag(ped, 36, true);
            API.TaskMoveNetworkByName(ped, "task_mp_pointing", 0.5f, true, "anim@mp_point", 24);
            API.RemoveAnimDict("anim@mp_point");
           //NotificationScript.ShowNotification($"should be pointing now"); //debug
        }

        private void StopPointing()
        {
            int ped = Game.PlayerPed.Handle;
            API.ClearPedSecondaryTask(ped);
            API.SetPedConfigFlag(ped, 36, false);

            // Ensure that weapon visibility is reset if the player is not injured and not in a vehicle
            if (!Game.PlayerPed.IsInjured && !API.IsPedInAnyVehicle(ped, false))
            {
                API.SetPedCurrentWeaponVisible(ped, true, true, true, true);
            }
        }
    }
}