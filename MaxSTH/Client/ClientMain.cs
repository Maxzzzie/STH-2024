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

        [EventHandler("clear_vehicles")]
        void RemoveAllVehicles(bool shouldRemoveProps)
        {
            Vehicle[] allVeh = World.GetAllVehicles();
            foreach (Vehicle veh in allVeh)
            {
                veh.Delete();
            }
            NotificationScript.ShowNotification($"All vehicles are removed.");
            if (shouldRemoveProps)
            {
                Prop[] allProp = World.GetAllProps();
                foreach (Prop prop in allProp)
                {
                    prop.Delete();
                }
                NotificationScript.ShowNotification($"All entities are removed too.");
            }
            TriggerServerEvent("didClearJustHappen");
        }

        [EventHandler("clearNearVehicles")]
        void RemoveNearVehicles(int range)
        {
            Vehicle[] allVeh = World.GetAllVehicles();
            Vector3 pos = Game.PlayerPed.Position;
            foreach (Vehicle veh in allVeh)
            {
                Vector3 vehpos = veh.Position;
                if (IsVehicleSeatFree(veh.Handle, -1) && GetDistanceBetweenCoords(pos.X, pos.Y, pos.Z, vehpos.X, vehpos.Y, vehpos.Z ,true) < range) veh.Delete();
                
            }
            NotificationScript.ShowNotification($"All empty vehicles are removed within {range}m.");
            TriggerServerEvent("didClearJustHappen");
        }

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
            NotificationScript.ShowNotification($"Current player position = {Game.PlayerPed.Position}");
        }

        [EventHandler("SendCoordsToServerForMapbounds")]
        void SendCoordsToServer(int CircleRadius, int source)
        {
            //Debug.WriteLine("SendCoordsToServer");
            TriggerServerEvent("SetMapboundsWithPlayerCoords", Game.PlayerPed.Position, CircleRadius, source);
        }
    }
}


