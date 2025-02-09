using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;
using System.Linq;

//https://docs.fivem.net/docs/game-references/blips/
namespace STHMaxzzzie.Client
{
    public class PlayerBlips : BaseScript
    {
        static int clientId;

        private static Dictionary<string, int> blips = new Dictionary<string, int>();
        public PlayerBlips()
        {
            DisplayPlayerNameTagsOnBlips(true);
            TriggerServerEvent("Server:updatePlayerBlipSettings");
            TriggerServerEvent("sendClientTeamAssignment");
            clientId = Game.Player.ServerId;
        }

        // Store blips for players: key = playerName, value = string[] (0 = blip handle, 1 = the way it placed the blip. Either entity or coord, 2 = EntityId)
        Dictionary<int, string[]> currentBlips = new Dictionary<int, string[]>();

        // List of blips that should be shown (in 'custom' mode)
        public static List<int> showThisBlip = new List<int>();
        // Player blip setting (on/off/custom)
        static string playerBlipSetting = "on";

        [EventHandler("updatePlayerBlipSettings")]
        void updatePlayerBlipSettings(string newPlayerBlipSetting, string showThisBlipString)
        {
            //Debug.WriteLine($"upb start {showThisBlipString} + {newPlayerBlipSetting} + {playerBlipSetting}");
            playerBlipSetting = newPlayerBlipSetting;
                showThisBlip.Clear();
            if (showThisBlipString != "")
            {
                string[] showThisBlipStringArray = showThisBlipString.Split(',');
                foreach (string var in showThisBlipStringArray)
                {
                    if (int.TryParse(var, out int playerId)) showThisBlip.Add(playerId);
                    else Debug.WriteLine($"upb int wasn't an int");
                }
            }
            //Debug.WriteLine("upb end");
        }

        //Boss,8,-1535.987,-1055.367,5.336304,0
        //Rivstar,7,-1506.185,-1049.802,5.218262,36.8504

        [EventHandler("setPlayerBlips")]
        private void SetPlayerBlips(List<object> playerBlipList)
        {
            Vector3 clientPos = Game.PlayerPed.Position;
            //Debug.WriteLine("[Blips] Updating player blips...");
            List<int> blipsPlaced = new List<int>();

            if (!RoundHandling.teamAssignment.ContainsKey(clientId))
            {
                Debug.WriteLine("[Blips] Client is not assigned to a team yet.");
                return;
            }

            int clientTeam = RoundHandling.teamAssignment[clientId];

            foreach (string line in playerBlipList)
            {
                //Debug.WriteLine($"[Blips] Processing: {line}");
                string[] pb = line.Split(',');
                string playerName = pb[0];
                int playerId = int.Parse(pb[1]);
                float posX = float.Parse(pb[2]);
                float posY = float.Parse(pb[3]);
                float posZ = float.Parse(pb[4]);
                float heading = float.Parse(pb[5]);

                Vector4 playerPosition = new Vector4(posX, posY, posZ, heading);

                if (playerId == clientId)
                {
                    //Debug.WriteLine("[Blips] Skipping self blip.");
                    continue;
                }

                if (!RoundHandling.teamAssignment.ContainsKey(playerId) && RoundHandling.gameMode != "none")
                {
                    //Debug.WriteLine($"[Blips] Skipping {playerName}, not in a team or no game mode.");
                    continue;
                }

                bool shouldShowBlip;

                switch (playerBlipSetting)
                {
                    case "on":
                        shouldShowBlip = RoundHandling.teamAssignment[playerId] == clientTeam;
                        break;
                    case "off":
                        shouldShowBlip = false;
                        break;
                    case "custom":
                        shouldShowBlip = showThisBlip.Contains(playerId);
                        break;
                    default:
                        shouldShowBlip = false;
                        break;
                }


                if (!shouldShowBlip)
                {
                    //Debug.WriteLine($"[Blips] Not showing blip for {playerName}.");
                    continue;
                }

                blipsPlaced.Add(playerId);

                int entityId = GetPlayerPed(GetPlayerFromServerId(playerId));
                int blipHandle = 0;
                string blipState = "coord";

                // Get distance between client and target player
                float distanceBetweenClientAndEntity = GetDistanceBetweenCoords(clientPos.X, clientPos.Y, clientPos.Z, posX, posY, posZ, true);

                // Check if we already have a blip for this player
                if (currentBlips.TryGetValue(playerId, out string[] existingBlipInfo))
                {
                    int existingBlipHandle = int.Parse(existingBlipInfo[0]);
                    string existingBlipState = existingBlipInfo[1];
                    int existingEntityId = int.Parse(existingBlipInfo[2]);

                    // If entity is unchanged & within 400m, keep the blip
                    if (entityId != 0 && entityId == existingEntityId && distanceBetweenClientAndEntity < 400)
                    {
                        //Debug.WriteLine($"[Blips] {playerName}'s entity unchanged, keeping existing blip.");
                        continue;
                    }

                    // Remove old blip since entity changed or is out of range
                    RemovePlayerBlip(ref existingBlipHandle);
                }

                // Check if we should use an entity blip
                if (entityId != 0 && entityId != Game.PlayerPed.Handle && distanceBetweenClientAndEntity < 400)
                {
                    blipHandle = AddBlipForEntity(entityId);
                    //Debug.WriteLine($"[Blips] Creating blip for {playerName} on entity {entityId}, blip handle is {blipHandle}.");
                    blipState = "entity";
                }
                else
                {
                    entityId =0;
                    blipHandle = AddBlipForCoord(posX, posY, posZ);
                   // Debug.WriteLine($"[Blips] Creating blip for {playerName} at coordinates ({posX}, {posY}, {posZ}). Blip handle is {blipHandle}.");
                }

                currentBlips[playerId] = new string[] { blipHandle.ToString(), blipState, entityId.ToString() };

                SetBlipSprite(blipHandle, 484);
                //SetBlipRotation(blipHandle, (int)Math.Ceiling(heading));
                SetBlipColour(blipHandle, Convert.ToInt32(colors[playerId - 1], 16));
                SetBlipAsFriendly(blipHandle, true);
                SetBlipShrink(blipHandle, false);
                SetBlipScale(blipHandle, 1f);

                BeginTextCommandSetBlipName("STRING");
                AddTextComponentSubstringPlayerName(playerName);
                EndTextCommandSetBlipName(blipHandle);
                SetBlipCategory(blipHandle, 7);
            }

            List<int> toRemove = currentBlips.Keys.Except(blipsPlaced).ToList();
            foreach (int removedId in toRemove)
            {
                Debug.WriteLine($"[Blips] Removing blip for disconnected player ID: {removedId}.");
                int removedBlipHandle = int.Parse(currentBlips[removedId][0]);
                RemovePlayerBlip(ref removedBlipHandle);
                currentBlips.Remove(removedId);
            }

            //Debug.WriteLine($"[Blips] Update complete. Active blips: {currentBlips.Count}");
        }

        // Utility method to remove blip if necessary
        private void RemovePlayerBlip(ref int blipHandle)
        {
            if (blipHandle == 0 || !DoesBlipExist(blipHandle))
            {
                //Debug.WriteLine($"[Blips] Blip handle {blipHandle} is invalid or already removed, skipping.");
                return;
            }

            //Debug.WriteLine($"[Blips] Removing blip handle: {blipHandle}");
            RemoveBlip(ref blipHandle);
            blipHandle = 0; // Reset the handle to indicate it's removed
        }

        List<string> colors = new List<string>
        {
            "0xFF0000FF", // Red
            "0x00FF00FF", // Green
            "0x0000FFFF", // Blue
            "0xFFFF00FF", // Yellow
            "0xFF00FFFF", // Magenta
            "0x00FFFFFF", // Cyan
            "0x800000FF", // Dark Red
            "0x008000FF", // Dark Green
            "0x000080FF", // Dark Blue
            "0x808000FF", // Olive
            "0x800080FF", // Purple
            "0x008080FF", // Teal
            "0xFFA500FF", // Orange
            "0xFF4500FF", // Red-Orange
            "0x32CD32FF", // Lime Green
            "0x4682B4FF", // Steel Blue
            "0x9400D3FF", // Dark Violet
            "0xDC143CFF", // Crimson
            "0x00FA9AFF", // Medium Spring Green
            "0x1E90FFFF", // Dodger Blue
            "0xADFF2FFF", // Green-Yellow
            "0xFF1493FF", // Deep Pink
            "0xFFD700FF", // Gold
            "0xE9967AFF", // Dark Salmon
            "0x48D1CCFF", // Medium Turquoise
            "0x4B0082FF", // Indigo
            "0x2E8B57FF", // Sea Green
            "0x5F9EA0FF", // Cadet Blue
            "0xD2691EFF", // Chocolate
            "0xB0E0E6FF", // Powder Blue
            "0xCD5C5CFF", // Indian Red
            "0x556B2FFF", // Dark Olive Green
            "0x8A2BE2FF", // Blue Violet
            "0xF08080FF", // Light Coral
            "0x90EE90FF", // Light Green
            "0xFFDEADFF", // Navajo White
            "0x7B68EEFF", // Medium Slate Blue
            "0xDDA0DDFF", // Plum
            "0xFF69B4FF", // Hot Pink
            "0xB22222FF", // Firebrick
            "0xEEE8AAFF", // Pale Goldenrod
            "0xDAA520FF", // Goldenrod
            "0xD3D3D3FF", // Light Gray
            "0xA9A9A9FF", // Dark Gray
            "0x696969FF", // Dim Gray
            "0x808080FF", // Gray
            "0x000000FF", // Black
            "0xFFFFFFFF"  // White
        };
    }
}


//displayplayernametagsonblip https://docs.fivem.net/natives/?_0x82CEDC33687E1F50
//setblipcategory https://docs.fivem.net/natives/?_0x234CDD44D996FD9A (needed for above)
//is blip on radar https://docs.fivem.net/natives/?_0xE41CA53051197A27
//is blip short distance https://docs.fivem.net/natives/?_0xDA5F8727EB75B926
//pulse blip https://docs.fivem.net/natives/?_0x742D6FD43115AF73 (what does it do)
//alpha https://docs.fivem.net/natives/?_0x45FF974EEE1C8734
//setblipbright https://docs.fivem.net/natives/?_0xB203913733F27884 (what does it do)
//set blip fade https://docs.fivem.net/natives/?_0x2AEE8F8390D2298C
//setblipflashinterval 
//setblipflashtimer (for how long does it need to be flashing)
//setblipflashesalternate
//setblipnamefromtextfile
//setblipnametoplayername
//setblippriority
//setbliprotation
//setblipscale
//setblipsecondarycolour (i think the outline etc)
//setblipShrink (makes blip go small when off the minimap) https://docs.fivem.net/natives/?_0x2B6D467DAB714E8D
//showfriendindicatoronblip (like in gta online half a circle around a blip) https://docs.fivem.net/natives/?_0x23C3EB807312F01A
//showheadingindicatoronblip https://docs.fivem.net/natives/?_0x5FBCA48327B914DF (adds player heading)
//showheightonblip i assume indicates going up or down https://docs.fivem.net/natives/?_0x75A16C3DA34F1245
//showoutlineindicatoronblip toggles a cyan outline around the blip https://docs.fivem.net/natives/?_0xB81656BC81FE24D1