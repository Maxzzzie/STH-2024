using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;

namespace STHMaxzzzie.Client
{
public class StuckScript : BaseScript
{
    private int usageCount = 0;
    private DateTime lastUsed = DateTime.MinValue;
    private Vector3 lastPosition;
    public List<int> allowedClassIdForStuckVeh = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 17, 18, 19, 20 };
    public StuckScript()
    {
        RegisterCommand("stuck", new Action(StuckCommand), false);
        RegisterKeyMapping("+Stuck", "Vehicle Stuck Recovery", "keyboard", "n"); // Change "n" to your desired key.
    }

    private async void StuckCommand()
    {
        Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

        if (vehicle == null || !vehicle.Exists())
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be in a car to use /stuck." } });
            return;
        }
        var VehClass = GetVehicleClass(vehicle.Handle);

        if (!allowedClassIdForStuckVeh.Contains(VehClass))
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be in a car to use /stuck." } });
            return;
        }
        Vector3 speed = GetEntityVelocity(vehicle.Handle);
        if (speed.X > 1 || speed.Y > 1 || speed.Z > 1)
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "You need to be standing still to use /stuck." } });
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
                if (isUpsideDown)
                {
                    force = new Vector3(3, 0, 5f);
                    vehicle.Rotation = new Vector3(rotation.X, 0, rotation.Z);
                }
                else
                {
                    force = new Vector3(-2, 0, 3f);
                }
                    NotificationScript.ShowNotification($"~h~~g~Stuck huh?~s~~n~Let me give you a little nudge.");
                lastUsed = DateTime.Now;
                break;
            case 2:
                if (isUpsideDown)
                {
                    force = new Vector3(-3, 0, 5f);
                    vehicle.Rotation = new Vector3(rotation.X, 0, rotation.Z);
                }
                else
                {
                    force = new Vector3(-3, 0, 5f);
                }
                    NotificationScript.ShowNotification($"~h~~g~Still stuck?~s~~n~Here's a push.");

                lastUsed = DateTime.Now;
                break;
            case 3:
                force = new Vector3(6, 0, 10f);
                lastUsed = DateTime.Now;
                                    NotificationScript.ShowNotification($"~h~~r~Come on man!~s~~n~Be free already!");

                break;
            case 4:
                force = new Vector3(-6, 5, 10f);
                lastUsed = DateTime.Now;
                                    NotificationScript.ShowNotification($"~h~~g~Stepbro?~s~~n~What are you doing?");

                break;
            default:
                    NotificationScript.ShowNotification($"~h~~w~I think i helped enough.~s~~n~~h~~r~Wait.");
                usageCount = 4;
                break;
        }

        SetEntityVelocity(vehicle.Handle, force.X, force.Y, force.Z);
        //TriggerEvent("chat:addMessage", new { color = new[] { 0, 255, 0 }, args = new[] { $"/stuck used {usageCount} time(s)." } });

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
        StuckCommand();
    }

    [Command("-Stuck")]
    private void StuckKeyRelease()
    {
        // No action needed for release in this context
    }
}
}