using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

    //TriggerEvent("addBlip", false, $"blipName", "coord", new Vector3(X, Y, Z), 0, 66, 0, true, false, true);
    //TriggerEvent("addBlip", false, $"blipName", "entity", new Vector3(-2000, 0, 0), 0, 66, 0, true, false, true);
    //https://docs.fivem.net/docs/game-references/blips/
public class BlipManager : BaseScript
{


    private Dictionary<string, int> blips = new Dictionary<string, int>();

[EventHandler("ClearBlips")]
    private async void ClearBlips()
    {
        foreach (var kvp in blips)
        {
            string tempBlipName = kvp.Key;
            int blipHandle = kvp.Value;
            API.RemoveBlip(ref blipHandle);      
        }
        await Delay(50);
        blips.Clear();

    }
[EventHandler("HandleBlip")]
    private async void HandleBlip(string blipName, string type, Vector3 coords, int blipEntityHandle, int blipSprite, int blipColour, bool blipFriendly, bool isFlashing, bool blipAllClients)
    {
        Debug.WriteLine($"HandleBlip 1");
        foreach (var key in blips)
        {
            Debug.WriteLine($"foreach HandleBlip key:{key}");
        }
        bool exists = blips.ContainsKey(blipName);
        if (exists)
        {
            Debug.WriteLine($"HandleBLip 4 blipname exists: {blipName}");
            int existingBlip = blips[blipName];
            API.RemoveBlip(ref existingBlip);
            blips.Remove(blipName);
        }
        await Delay(100);
        int blipHandle = 0;

        if (type == "coord")
        {
            Debug.WriteLine($"HandleBlip 2 coord{blipName}");
            blipHandle = API.AddBlipForCoord(coords.X, coords.Y, coords.Z);
        }
        if (type == "entity")
        {
            Debug.WriteLine($"HandleBlip 3 entity");
            blipHandle = API.AddBlipForEntity(blipEntityHandle);
        }
        // else //I don't know why this runs if the type is correct. That's odd to me.
        if (type != "coord" && type != "entity")//this is the fix
        {
            Debug.WriteLine($"Error: type wasn't coord or entity.");
            return;
        }

        API.SetBlipSprite(blipHandle, blipSprite);
        API.SetBlipColour(blipHandle, blipColour);
        API.SetBlipAsFriendly(blipHandle, blipFriendly);
        API.SetBlipFlashes(blipHandle, isFlashing);
        blips.Add(blipName, blipHandle);
        //Debug.WriteLine($"HandleBlip add blip to dict blipName:{blipName} blipHandle:{blipHandle} Contains key:{blips.ContainsKey(blipName)}");
    }

    [EventHandler("removeBlipWithName")]
        public async void removeBlipWithName(string blipName)
    {
        //Debug.WriteLine($"removeBlipWithName {blipName}");
        if (blips.ContainsKey(blipName))
        {
            int existingBlip = blips[blipName];
            API.RemoveBlip(ref existingBlip);
            await Delay(100);
            blips.Remove(blipName);
        }
    }
}