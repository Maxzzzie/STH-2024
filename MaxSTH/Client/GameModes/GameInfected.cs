using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using STHMaxzzzie.Client;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class GameInfected : BaseScript
    {
        static bool updateWeapons = false;
        public static int hunterCount = 0;
        public static int runnerCount = 0;
        [EventHandler("updateClientTeamAssignmentForInfected")]
        static void updateClientTeamAssignmentForInfected(List<object> playerIdAndTeamAssignment)
        {   int previousHunterCount = hunterCount;
            hunterCount = 0;
            runnerCount = 0;
            RoundHandling.teamAssignment.Clear();

            foreach (var obj in playerIdAndTeamAssignment)
            {
                if (obj is Vector2 vector)
                {
                    int playerId = (int)vector.X;
                    int team = (int)vector.Y;
                    if (team == 1) runnerCount ++;
                    if (team == 2) hunterCount ++;
                    RoundHandling.teamAssignment.Add(playerId, team);
                    if (playerId == RoundHandling.serverId && RoundHandling.thisClientIsTeam != team) //if team is switched or there are more than 2 hunters. Update weapons!
                    {
                        RoundHandling.thisClientIsTeam = team;
                        updateWeapons = true;
                        if (Radio.setsAutomatically) TriggerEvent("AddPlayerToRadio", team);
                    }
                }
            }
            if (previousHunterCount == 2 && hunterCount == 3 && RoundHandling.thisClientIsTeam == 2) updateWeapons = true;
            if (updateWeapons) infectedWeapons();
        }

        
        public static void infectedWeapons()
        {
            if (RoundHandling.thisClientIsTeam == 1) Armoury.giveNonLethalWeapon(false);
            else if (RoundHandling.thisClientIsTeam == 2 && hunterCount <= 2) Armoury.giveInfectedHuntWeapon(true); //gives initial stronger weapons.
            else if (RoundHandling.thisClientIsTeam == 2 && hunterCount  >= 3) Armoury.giveHuntWeapon(true);
        }
    }
    
}