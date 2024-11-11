using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class RoundHandling : BaseScript
    {
        public static Dictionary<int, int> teamAssignment = new Dictionary<int, int>(); //0= not assigned a team, 1= runner, 2= hunter, 3= spectator
        public static string gameMode = "none";
        Dictionary<string, int> teamNumberDict = new Dictionary<string, int> { { "none", 0 }, { "runner", 1 }, { "hunter", 2 }, { "spectator", 3 } };
        public static List<string> runnerList = new List<string>();
        public static int runnerThisGame = -1;

        [Command("startgame", Restricted = true)] //normal restriction true 
        [EventHandler("startGameMode")]
        public void startGameMode(int source, List<object> args, string raw)
        {
            if (args[0].ToString() == "delay")
            {
                Player sourceHost = Players[source];
                Player runner = Players[int.Parse(args[1].ToString())];
                DelayMode.delayMode(sourceHost, runner, args);
            }
            else if (args[0].ToString() == "hunt")
            {
                startGame("hunt", int.Parse(args[1].ToString()));
            }
        }

        [Command("endgame", Restricted = true)] //normal restriction true 
        [EventHandler("endGameMode")]
        public void endGameMode(int source, List<object> args, string raw)
        {
            if (gameMode != "none")
            {
            endGame("end");
                TriggerClientEvent("chat:addMessage", new{color=new[]{255,0,0},args=new[]{$"{Players[source].Name} ended the game early."}});
            }
            else 
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"No game is running."}});
            }
        }

        [EventHandler("startGame")]
        public void startGame(string mode, int runner)
        {
            runnerThisGame = runner;
            gameMode = mode;
            teamAssignment.Clear();
            runnerList.Clear();
            bool runnerFound = false;

            foreach (Player player in Players)
            {
                int playerId = int.Parse(player.Handle);

                if (playerId == runner)
                {
                    teamAssignment.Add(playerId, 1);
                    runnerList.Add(player.Name);
                    runnerFound = true;
                    //CitizenFX.Core.Debug.WriteLine($"Player {playerId} added to Runner team with name {player.Name}.");
                }
                else
                {
                    teamAssignment.Add(playerId, 2);
                    //CitizenFX.Core.Debug.WriteLine($"Player {playerId} added to Hunter team.");
                }
            }

            // Check if the runner was found; if not, log an error message
            if (!runnerFound)
            {
                CitizenFX.Core.Debug.WriteLine($"Error: Player with ID {runner} is not currently online and cannot be added as the Runner.");
                return;
            }

            // Prepare the team assignments for client synchronization
            List<Vector2> teamAssignmentForClient = new List<Vector2>();
            foreach (var kvp in teamAssignment)
            {
                teamAssignmentForClient.Add(new Vector2(kvp.Key, kvp.Value));
            }

            // Trigger client events to start the game and send 
            API.StopResource("playernames");
            TriggerClientEvent("startGame", teamAssignmentForClient, gameMode);
            TriggerClientEvent("gameStartNotification", runnerList);
        }

        [EventHandler("thisClientDiedForGameStateCheck")]
        public void thisClientDiedForGameStateCheck(int source)
        {
            if ((gameMode == "delay" || gameMode == "hunt") && teamAssignment.ContainsKey(1))
            {
                int teamNumber = teamAssignment[source];
                //CitizenFX.Core.Debug.WriteLine($"thisClientDiedForGameStateCheck {source},{teamNumber}");
                if (teamNumber == 1)
                {
                    endGame("hunter");
                }
            }
        }

        //[EventHandler("endGame")]
        public async void endGame(string winningTeam)
        {
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"EndGame {winningTeam} {gameMode}" } });

            endGameMessages(winningTeam);
            await Delay(100);
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"EndGame waited" } });

            teamAssignment.Clear();
            foreach (Player player in Players)
            {
                teamAssignment.Add(int.Parse(player.Handle), 0);
            }
            List<Vector2> teamAssignmentForClient = new List<Vector2>();
            foreach (var kvp in teamAssignment)
            {
                teamAssignmentForClient.Add(new Vector2(kvp.Key, kvp.Value));
            }
            TriggerClientEvent("clientEndGame", teamAssignmentForClient);
            if (gameMode == "delay")
            {
                DelayMode.delayModeOn = false;
                TriggerClientEvent("updateBlipLocationOnMapForDelayMode", new Vector3(0, 0, 0));
                //reset server setting permissions
            }
            else if (gameMode == "hunt")
            {
                //reset permissions
            }
            //update spawning/ teleports/ /pod /weapons /colouring /clear vehicles maybe/ fix/ 
            gameMode = "none";
            runnerThisGame = -1;
            API.StartResource("playernames");
        }

        [EventHandler("playerJoinedWhileGameIsActive")]
        public void playerJoinedWhileGameIsActive(int JoinedPlayerId)
        {
            Player joinedPlayer = Players[JoinedPlayerId];
            //TriggerClientEvent(joinedPlayer, "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"playerJoinedWhileGameIsActive server" } });
            if (gameMode == "none")
            {
                return;
            }
            int id = int.Parse(joinedPlayer.Handle);
            teamAssignment[id] = 2;
            foreach (Player player in Players) //updates all other clients with the new player.
            {
                if (player != joinedPlayer)
                    TriggerClientEvent(player, "updateTeamAssignment", id);
            }

            List<Vector2> teamAssignmentForClient = new List<Vector2>();
            foreach (var kvp in teamAssignment)
            {
                teamAssignmentForClient.Add(new Vector2(kvp.Key, kvp.Value));
            }

            TriggerClientEvent(joinedPlayer, "startGame", teamAssignmentForClient, gameMode);
            TriggerClientEvent(joinedPlayer, "gameJoinNotification", runnerList);
        }

        public void endGameMessages(string winningTeam)
        {
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"EndGameMessages {winningTeam}" } });
            List<int> winners = new List<int>();
            List<int> losers = new List<int>();
            List<int> neutral = new List<int>();
           foreach (var kvp in teamAssignment)
    {
        if (winningTeam == "end")
        {
            neutral.Add(kvp.Key);
            continue;
        }
        if (teamNumberDict.ContainsKey(winningTeam))
        {
            if (kvp.Value == teamNumberDict[winningTeam])
            {
                winners.Add(kvp.Key);
            }
            else if (kvp.Value != 0)
            {
                losers.Add(kvp.Key);
            }
            else
            {
                neutral.Add(kvp.Key);
            }
        }
        else
        {
            // Log an error or handle the case where winningTeam is not valid
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Invalid winning team: {winningTeam}" } });
        }
    }
            if (winners.Count == 0)
            {
                TriggerClientEvent("gameDrawNotification");
                return;
            }
            else
            {
                foreach (int winner in winners)
                {
                    TriggerClientEvent(Players[winner], "gameWonNotification", winners, losers);
                }
                if (losers.Count != 0)
                {
                    foreach (int loser in losers)
                    {
                        TriggerClientEvent(Players[loser], "gameLostNotification", winners, losers);
                    }
                }
                else if (neutral.Count != 0)
                {
                    foreach (int neutra in neutral)
                    {
                        TriggerClientEvent(Players[neutra], "gameNeutralNotification", winningTeam);
                    }
                }
            }
        }
    }
}
