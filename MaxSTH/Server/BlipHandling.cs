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
        static Dictionary<string, BlipData> blips = new Dictionary<string, BlipData>();


        public static void AddBlips(UpdateBlipsRequest request)
        {
            foreach (BlipData blip in request.BlipsToAdd)
            {
                if (!blips.ContainsKey(blip.Name))
                {
                    blips.Add(blip.Name, blip);
                }
                else
                {
                    blips[blip.Name] = blip;
                }
            }
            foreach (string blip in request.BlipsToRemove)
            {
                if (blips.ContainsKey(blip))
                {
                    blips.Remove(blip);
                }

            }
            List<BlipData> updatedBlips = blips.Values.ToList();
            UnpackBlipDataForClient(updatedBlips);
        }

        public static void UpdateClientBlips()
        {
            List<BlipData> updatedBlips = blips.Values.ToList();
            UnpackBlipDataForClient(updatedBlips);
        }

        public static void UnpackBlipDataForClient(List<BlipData> unpack)
        {
            List<string> unpacked = new List<string>();
            foreach (var blip in unpack)
            {
                string blipDetails = $"{blip.Name}," +
                                     $"{blip.Type}," +
                                     $"{blip.Coords.X},{blip.Coords.Y},{blip.Coords.Z}," +
                                     $"{blip.EntityId}," +
                                     $"{blip.Sprite}," +
                                     $"{blip.Colour}," +
                                     $"{blip.Alpha}," +
                                     $"{blip.IsFlashing}," +
                                     $"{blip.IsFriendly}," +
                                     $"{blip.IsShortRange}," +
                                     $"{blip.IsOnRadar}," +
                                     $"{blip.FlashIntervalInMs}," +
                                     $"{blip.Priority}," +
                                     $"{blip.Shrink}," +
                                     $"{blip.HasFriendIndicator}," +
                                     $"{blip.HasCrewIndicator}," +
                                     $"{blip.MapName}," +
                                     $"{blip.Category}," +
                                     $"{blip.Visibility.VisibilityType}," +
                                     $"[{string.Join(",", blip.Visibility.Players)}]";
                unpacked.Add(blipDetails);
                //TriggerClientEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"blipDetails: {blipDetails}"}});
            }
            if (unpacked.Count == 0)
            {
                TriggerClientEvent("DeleteAllBlips");
                Debug.WriteLine("unpacked.Count was 0");
            }
            TriggerClientEvent("RepackBlipDataForClient", unpacked);
            //Debug.WriteLine("sending unpacked list to client");
        }

        public class BlipData
        {
            public BlipData(string name)
            {
                Name = name;
            }
            public string Name { get; }
            //public bool Remove { get; set; } = false;
            public BlipVisibility Visibility { get; set; } = new BlipVisibility();
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

        public class BlipVisibility
        {
            public List<int> Players { get; set; } = new List<int>();
            public VisibilityType VisibilityType { get; set; } = VisibilityType.All;
        }

        public enum VisibilityType
        {
            All,
            Except,
            Only,
        }

        public class UpdateBlipsRequest
        {
            public List<BlipData> BlipsToAdd { get; set; } = new List<BlipData>();
            public List<string> BlipsToRemove { get; set; } = new List<string>();
        }
    }
}