using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace STHMaxzzzie.Server
{
    public class BlipHandler : BaseScript
    {
        public static Dictionary<string, object> sharedBlipDict = new Dictionary<string, object>();

        [EventHandler("addBlip")]
        void addBlip(bool blipRemove, string blipName, string blipType, Vector3 blipCoords, int blipEntityHandle, int blipSprite, int blipColour, bool blipFriendly, bool blipFlashing, bool blipAllClients)
        {
            Debug.WriteLine($"addblip is triggered - remove:{blipRemove} name:{blipName} Type:{blipType} XYZ:{blipCoords.X},{blipCoords.Y},{blipCoords.Z} entityHandle:{blipEntityHandle} sprite:{blipSprite} colour:{blipColour} friendly:{blipFriendly} flashing:{blipFlashing} shared:{blipAllClients}");
            if (blipAllClients)
            {
                setSharedBlipHandler(blipRemove, blipName, blipType, blipCoords, blipEntityHandle, blipSprite, blipColour, blipFriendly, blipFlashing, blipAllClients);
            }
            else
            {
                Debug.WriteLine($"blip per player isn't implemented yet.");
            }
        }


        [EventHandler("sharedBlipHandler")]
        void setSharedBlipHandler(bool blipRemove, string blipName, string blipType, Vector3 blipCoords, int blipEntityHandle, int blipSprite, int blipColour, bool blipFriendly, bool blipFlashing, bool blipAllClients)
        {
            Debug.WriteLine($"sharedBlipHandler 0");
            if (!blipRemove)
            {
                Debug.WriteLine($"sharedBlipHandler 1 {blipName}");
                if (sharedBlipDict.ContainsKey(blipName))
                {
                    Debug.WriteLine($"sharedBlipHandler 3");
                    sharedBlipDict.Remove(blipName);
                }
                object[] blipInfoArray = new object[8];
                blipInfoArray[0] = blipType;
                blipInfoArray[1] = blipCoords;
                blipInfoArray[2] = blipEntityHandle;
                blipInfoArray[3] = blipSprite;
                blipInfoArray[4] = blipColour;
                blipInfoArray[5] = blipFriendly;
                blipInfoArray[6] = blipFlashing;
                blipInfoArray[7] = blipAllClients;
                sharedBlipDict[blipName] = blipInfoArray;
                updateSharedClientBlips();
            }

            else if (blipRemove && sharedBlipDict.ContainsKey(blipName))
            {
                Debug.WriteLine($"sharedBlipHandler 2");
                sharedBlipDict.Remove(blipName);
                TriggerClientEvent("removeBlipWithName", blipName);
            }
            else
            {
                Debug.WriteLine($"something went wrong and stopped at sharedbliphandler");
            }
        }

        [Command("redoblips", Restricted = true)]
        [EventHandler("updateSharedClientBlips")]
        async void updateSharedClientBlips()
        {
            //Debug.WriteLine($"updateSharedClientBlips 1");
            TriggerClientEvent("ClearBlips");
            await Delay(1000);
            foreach (var kvp in sharedBlipDict)
            {

                string blipName = kvp.Key;
                object[] blipInfoArray = kvp.Value as object[];
                Debug.WriteLine($"updateSharedClientBlips {blipName}");
                string blipType = (string)blipInfoArray[0];
                Vector3 blipCoords = (Vector3)blipInfoArray[1];
                int blipEntityHandle = (int)blipInfoArray[2];
                int blipSprite = (int)blipInfoArray[3];
                int blipColour = (int)blipInfoArray[4];
                bool blipFriendly = (bool)blipInfoArray[5];
                bool blipFlashing = (bool)blipInfoArray[6];
                bool blipAllClients = (bool)blipInfoArray[7];
                TriggerClientEvent("HandleBlip", //leave as is
            blipName, //blipname
            blipType, //coord or entity
            blipCoords, //coords if coord
            blipEntityHandle,  //int entity handle
            blipSprite, //blip sprite
            blipColour, //blip colour
            blipFriendly, //is blip classed as friendly. True = friendly
            blipFlashing, //is blip flashing
            blipAllClients); //is blip shared with all clients or just the source.
            }
        }
    }
}