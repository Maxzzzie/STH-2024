using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace STHMaxzzzie.Client
{
     public class Teleport : BaseScript
    {
        //recieving tp request from server for multiple players.
        //This is offset +/- 4 to prevent stacking players.
        // It's just visual xD
        [EventHandler("tpPlayerRand")]
        void tpAllRand(int tpX, int tpY, int tpZ)
        {
            int tpXRnd = 0;
            int tpYRnd = 0;
            Random rnd = new Random();
            var deltaX = rnd.Next(-3, 3);
            var deltaY = rnd.Next(-3, 3);
            tpXRnd = tpX + deltaX;
            tpYRnd = tpY + deltaY;

            Game.PlayerPed.Position = new Vector3(tpXRnd, tpYRnd, tpZ);
            Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);
        }

        //recieving tp request from server.
        [EventHandler("tpPlayer")]
        void tpAll(int tpX, int tpY, int tpZ)
        {
            Game.PlayerPed.Position = new Vector3(tpX, tpY, tpZ);
            Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);
        }

        //[EventHandler("tpPlayerHeading")] in use at race
        // void tpPlayerHeading(int tpX, int tpY, int tpZ, int tpW)
        // {
        //     Game.PlayerPed.Position = new Vector3(tpX, tpY, tpZ);
        //     Game.PlayerPed.Heading = tpW;
        //     Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);
        // }//???
    }
}