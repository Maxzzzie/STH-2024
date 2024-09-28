using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API; //chat gpt thing? does it do anything?
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

// namespace STHMaxzzzie.Client
// {

//     public class MapIcons : BaseScript
//     {
//         //all icons should get deleted when it updates on the server.
//         // Event handler for blip creation
//         EventHandlers["blip:addBlip"] += new Action<Vector3, int, int, string>(OnAddBlip);
//     }

//     // This method is triggered when the server sends the 'blip:addBlip' event
//     private void OnAddBlip(Vector3 position, int spriteId, int colorId, string blipName)
//     {
//         // Create the blip on the map
//         Blip blip = AddBlipForCoord(position.X, position.Y, position.Z);
//         SetBlipSprite(blip, spriteId); // Set the sprite (icon)
//         SetBlipColour(blip, colorId); // Set the color
//         SetBlipScale(blip, 1.0f); // Optionally set the size

//         // Set the blip name (hover text)
//         BeginTextCommandSetBlipName("STRING");
//         AddTextComponentString(blipName);
//         EndTextCommandSetBlipName(blip);

//         // Optionally, make the blip short-range, so it only shows when the player is close
//         SetBlipAsShortRange(blip, true);
//     }
// }

