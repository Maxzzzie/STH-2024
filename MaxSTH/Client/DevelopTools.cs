using System;
using CitizenFX.Core;

public class SavePositions : BaseScript
{
    // Save callout location
    [EventHandler("givePedLocationAndHeadingForDevMode")]
    private void givePedLocationAndHeadingForDevMode(int source, string locationName, string respawnOrCallout)
    {

        Ped playerPed = Game.PlayerPed;
        Vector3 position = playerPed.Position;
        float heading = playerPed.Heading;


        Vector4 playerLocation = new Vector4(position.X, position.Y, position.Z, heading);

        if (respawnOrCallout == "callout")
        {
            TriggerServerEvent("saveCallout", source, locationName, playerLocation);
        }
        else
        {
            TriggerServerEvent("saveRespawn", source, locationName, playerLocation);
        }
    }
}