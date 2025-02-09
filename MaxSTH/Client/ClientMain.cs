using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Mono.CSharp;

namespace STHMaxzzzie.Client
{
    public class Misc : BaseScript
    {
        bool isShootingFromVehicleAllowed = false;

     

        //[EventHandler("disableCanPlayerShootFromVehicles")]
        void DisableCanPlayerShootFromVehicles(bool sfv)
        {
            isShootingFromVehicleAllowed = sfv;
            SetPlayerCanDoDriveBy(PlayerId(), isShootingFromVehicleAllowed);
        }
    }

    public class MapBounds : BaseScript
    //all circles get cleared everytime the circlelist updates on the server.
    //circles get a different (blip id) for each client that makes a circle. So they are stored localy in blipList
    {
        int blip = 0;
        List<Int32> blipList = new List<Int32>();

        //deleting circle
        [EventHandler("delCircle")]
        void delCircle()
        {
            foreach (int blip in blipList)
            {
                int allBlip = blip;
                RemoveBlip(ref allBlip);
            }
            blipList.Clear();
        }

        [EventHandler("updateCircle")]
        void updateCircle(List<object> argArray)
        {
            blip = AddBlipForRadius(float.Parse(argArray[0].ToString()), float.Parse(argArray[1].ToString()), 0, float.Parse(argArray[2].ToString()));
            SetBlipAlpha(blip, 40);//sets opacity of the mapbound circles
            SetBlipColour(blip, int.Parse(argArray[3].ToString()));
            blipList.Add(blip);
        }

        [EventHandler("PrintCoords")]
        void PrintCoords()
        {
            Vector4 pos = new Vector4(Game.PlayerPed.Position, Game.PlayerPed.Heading);
            NotificationScript.ShowNotification($"Current player position = {pos.X}, {pos.Y}, {pos.Z}, {pos.W}");
            NotificationScript.displayClientDebugLine($"{(int)pos.X}, {(int)pos.Y}, {(int)pos.Z}, {(int)pos.W}");
        }

        [EventHandler("SendCoordsToServerForMapbounds")]
        void SendCoordsToServer(int CircleRadius, int source)
        {
            //Debug.WriteLine("SendCoordsToServer");
            TriggerServerEvent("SetMapboundsWithPlayerCoords", Game.PlayerPed.Position, CircleRadius, source);
        }
    }
    public class DisableGangAggro : BaseScript
    {
        public DisableGangAggro()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
           int playerGroup = GetPedRelationshipGroupHash(PlayerPedId());

// Gang hashes
int[] gangHashes = new int[]
{
    GetHashKey("GANG_BALLAS"),
    GetHashKey("GANG_FAMILY"),
    GetHashKey("GANG_VAGOS"),
    GetHashKey("AMBIENT_GANG_MEXICAN"),
    GetHashKey("AMBIENT_GANG_BIKER"),
    GetHashKey("AMBIENT_GANG_ARMENIAN"),
    GetHashKey("AMBIENT_GANG_AZTECA"),
    GetHashKey("AMBIENT_GANG_CHINESE")
};

// Set all gangs to be neutral to the player
foreach (var gang in gangHashes)
{
    SetRelationshipBetweenGroups(1, (uint)playerGroup, (uint)gang);
    SetRelationshipBetweenGroups(1, (uint)gang, (uint)playerGroup);
}

            await Delay(5000); // Adjust delay as needed
        }
    }

    // public class NoWeaponsNPCs : BaseScript
    // {
    //     public NoWeaponsNPCs()
    //     {
    //         Tick += OnTick;
    //     }

    //     private async Task OnTick()
    //     {
    //         foreach (var ped in World.GetAllPeds())
    //         {
    //             if (!ped.IsPlayer && GetDistanceBetweenCoords(ped.Position.X, ped.Position.Y, ped.Position.Z, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, false) < 50)
    //             {
    //                 // Remove weapons from NPCs
    //                ped.Weapons.RemoveAll();
    //                 // Prevent NPCs from dropping weapons when dead
    //                 //SetPedDropsWeaponsWhenDead(ped.Handle, false);
    //             }
    //         }
    //         await Delay(5000); // Run every second
    //     }
    // }


}


