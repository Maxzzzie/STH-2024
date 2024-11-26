using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
using System.Linq;

//TriggerEvent("addBlip", false, $"blipName", "coord", new Vector3(X, Y, Z), 0, 66, 0, true, false, true);
//TriggerEvent("addBlip", false, $"blipName", "entity", new Vector3(-2000, 0, 0), 0, 66, 0, true, false, true);
//https://docs.fivem.net/docs/game-references/blips/
namespace STHMaxzzzie.Client
{
    public class BlipManager : BaseScript
    {

        private static Dictionary<string, int> blips = new Dictionary<string, int>();
        public BlipManager()
        {
            API.DisplayPlayerNameTagsOnBlips(true);
        }

        [EventHandler("RepackBlipDataForClient")]
        public static void RepackBlipDataForClient(List<object> packedData)
        {
            //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Client receiveblipdatafromserver"}});
            List<string> blipDataStrings = packedData.Select(obj => obj.ToString()).ToList();
            List<BlipData> unpackedBlips = new List<BlipData>();
            //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Conversion from object to string complete."}});

            foreach (string dataString in blipDataStrings)
            {
                string[] parts = dataString.Split(',');
                //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Client blipDetails: {dataString}"}});

                // Parse the properties
                BlipData blip = new BlipData(parts[0])
                {
                    Type = parts[1],
                    Coords = new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4])),
                    EntityId = int.Parse(parts[5]),
                    Sprite = int.Parse(parts[6]),
                    Colour = int.Parse(parts[7]),
                    Alpha = int.Parse(parts[8]),
                    IsFlashing = bool.Parse(parts[9]),
                    IsFriendly = bool.Parse(parts[10]),
                    IsShortRange = bool.Parse(parts[11]),
                    IsOnRadar = bool.Parse(parts[12]),
                    FlashIntervalInMs = int.Parse(parts[13]),
                    Priority = int.Parse(parts[14]),
                    Shrink = bool.Parse(parts[15]),
                    HasFriendIndicator = bool.Parse(parts[16]),
                    HasCrewIndicator = bool.Parse(parts[17]),
                    MapName = parts[18],
                    Category = int.Parse(parts[19]),
                    Visibility = new BlipVisibility
                    {
                        VisibilityType = (VisibilityType)Enum.Parse(typeof(VisibilityType), parts[20], true),
                        Players = parts[21].Trim('[', ']').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse).ToList()
                    }
                };

                unpackedBlips.Add(blip);
            }
            HandleBlips(unpackedBlips);
        }

        // Helper method to parse Vector3 from a string like "(x, y, z)"
        private static void HandleBlips(List<BlipData> updateBlipData)
        {
            DeleteAllBlips();
            foreach (BlipData blip in updateBlipData)
            {
                //TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"HandleBlips client creates {blip.Name}"}});
                CreateBlip(blip);
            }
        }

        [EventHandler("DeleteAllBlips")]
        public static void DeleteAllBlips()
        {
            if (blips.Count() != 0)
                foreach (var blip in blips)
                {
                    //Debug.WriteLine($"removeBlipWithName {blip.Key}");
                    int blipHandle = blip.Value;
                    API.RemoveBlip(ref blipHandle);
                }
            blips.Clear();
        }

        private static void CreateBlip(BlipData blip)
        {
            //Debug.WriteLine($"CreateBlip 1");

            int Me = Game.Player.ServerId;
            if (blip.Visibility.VisibilityType != VisibilityType.All)
            {
                if (blip.Visibility.VisibilityType == VisibilityType.Except && blip.Visibility.Players.Contains(Me))
                {
                    return;
                }
                if (blip.Visibility.VisibilityType == VisibilityType.Only && !blip.Visibility.Players.Contains(Me))
                {
                    return;
                }
            }

            int blipHandle = 0;
            if (blip.Type == "coord") 
            {
                //Debug.WriteLine($"HandleBlip 2 coord added {blip.Name}");
                blipHandle = API.AddBlipForCoord(blip.Coords.X, blip.Coords.Y, blip.Coords.Z);
            }
                else if (blip.Type == "player")
                {
                    string[] splitName = blip.Name.Split('-');
                    Debug.WriteLine($"{splitName[1]} and {splitName[0]}");
                    int entityId = API.GetPlayerPed(API.GetPlayerFromServerId(int.Parse(splitName[1])));
                    blipHandle = API.AddBlipForEntity(entityId);
                }

            else if (blip.Type == "entity")
            {
                blipHandle = API.AddBlipForEntity(blip.EntityId);
            }
            else
            {
                Debug.WriteLine($"Error: Cannot add blip, type wasn't coord, player or entity.");
                return;
            }

            blips.Add(blip.Name, blipHandle);

            API.SetBlipSprite(blipHandle, blip.Sprite);
            API.SetBlipColour(blipHandle, blip.Colour);
            API.SetBlipAlpha(blipHandle, blip.Alpha);
            API.SetBlipFlashes(blipHandle, blip.IsFlashing);
            API.SetBlipAsFriendly(blipHandle, blip.IsFriendly);
            API.SetBlipAsShortRange(blipHandle, blip.IsShortRange);
            if (!blip.IsOnRadar)
            {
                API.SetBlipDisplay(blipHandle, 3);
            }
            API.SetBlipFlashInterval(blipHandle, blip.FlashIntervalInMs);
            API.SetBlipPriority(blipHandle, blip.Priority);
            API.SetBlipShrink(blipHandle, blip.Shrink);
            API.ShowFriendIndicatorOnBlip(blipHandle, blip.HasFriendIndicator);
            API.ShowCrewIndicatorOnBlip(blipHandle, blip.HasCrewIndicator);
            if (blip.MapName != "unknown")
            {
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentSubstringPlayerName(blip.MapName);
                API.EndTextCommandSetBlipName(blipHandle);
            }
            API.SetBlipCategory(blipHandle, blip.Category);
        }


    }

    class BlipData
    {
        public BlipData()
        {

        }
        public BlipData(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public BlipVisibility Visibility { get; set; } = new BlipVisibility();
        public string Handle { get; set; }
        //public bool Remove {get; set;} = false;
        public string Type { get; set; } = "coord";
        public Vector3 Coords { get; set; } = Vector3.Zero;
        public int EntityId { get; set; }
        public int Sprite { get; set; } = 0;
        public int Colour { get; set; } = 0;
        public int Alpha { get; set; } = 255;
        public bool IsFlashing { get; set; } = false;
        public bool IsFriendly { get; set; } = true;
        public bool IsShortRange { get; set; } = false;
        public bool IsOnRadar { get; set; } = true;
        public int FlashIntervalInMs { get; set; } = 750;
        public int Priority { get; set; } = 3;
        public bool Shrink { get; set; } = true;
        public bool HasFriendIndicator { get; set; } = false;
        public bool HasCrewIndicator { get; set; } = false;
        public string MapName { get; set; } = "unknown";
        public int Category { get; set; } = 1; //1 = No distance shown in legend 2 = Distance shown in legend 7 = "Other Players" category, also shows distance in legend 10 = "Property" category 11 = "Owned Property" category

    }

    class BlipVisibility
    {
        public List<int> Players { get; set; } = new List<int>();
        public VisibilityType VisibilityType { get; set; } = VisibilityType.All;
    }

    enum VisibilityType
    {
        All,
        Except,
        Only,
    }

    class UpdateBlipsRequest
    {
        public List<BlipData> Blips { get; set; } = new List<BlipData>();
        public List<string> BlipsToRemove { get; set; } = new List<string>();
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