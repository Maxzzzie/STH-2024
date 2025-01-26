using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class GameInfected : BaseScript
    {
        [EventHandler("sendClientTeamAssignment")]
        public static void sendClientTeamAssignment()
        {

            Debug.WriteLine($"updateClientTeamAssignment");
            List<Vector2> teamAssignmentForClient = new List<Vector2>();
            foreach (var kvp in RoundHandling.teamAssignment)
            {
                teamAssignmentForClient.Add(new Vector2(kvp.Key, kvp.Value));
            }

            TriggerClientEvent("updateClientTeamAssignmentForInfected", teamAssignmentForClient);
        }

        public static void shouldGameEndAfterPlayerDisconnect()
        {
            Debug.WriteLine($"shouldGameEndAfterPlayerDisconnect");

            int hunterCount = 0;
            int runnerCount = 0;
            foreach (var kvp in RoundHandling.teamAssignment)
            {
                if (kvp.Value == 1) runnerCount++;
                else if (kvp.Value == 2) hunterCount++;
            }
            if (hunterCount == 0 || runnerCount == 0)  //if the only player in a team disconnects it ends the game.
            {
                TriggerEvent("endGame", "end");
                return;
            }
        }
    }
}