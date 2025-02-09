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
    public class RoundHandling : BaseScript
    {
        public static Dictionary<int, int> teamAssignment = new Dictionary<int, int>(); //0= not assigned a team, 1= runner, 2= hunter, 3= spectator
        public static string gameMode = "none";
        Dictionary<string, int> teamNumberDict = new Dictionary<string, int> { { "none", 0 }, { "runner", 1 }, { "hunter", 2 }, { "spectator", 3 } };
        public static List<string> runnerList = new List<string>();
        public static int targetThisGame = -1;

        [Command("startgame", Restricted = true)] //normal restriction true 
        [EventHandler("startGameMode")]
        public void startGameMode(int source, List<object> args, string raw)
        {
            // Debug.WriteLine($"server startGameMode");
            if (gameMode != "none")
            {
                TriggerClientEvent(Players[source],"ShowErrorNotification", $"There is a game of {gameMode} running.");
                return;
            }
            if (args.Count == 1 || !int.TryParse(args[1].ToString(), out int targetId))
            {
                TriggerClientEvent(Players[source],"ShowErrorNotification", "Cannot start game.\ntargetId isn't specified.");
                return;
            }
            gameMode = args[0].ToString();

            targetThisGame = int.Parse(args[1].ToString());

            bool runnerIsOnline = false;
            List<int> onlinePlayers = new List<int>();
            foreach (Player player in Players)
            {
                int playerId = int.Parse(player.Handle);

                onlinePlayers.Add(playerId);

                if (playerId == targetThisGame) runnerIsOnline = true;
            }
            
            if (targetThisGame == 0)
            {
                Random rand = new Random();
                targetThisGame = onlinePlayers[rand.Next(0, onlinePlayers.Count)];
            }
            else if (!runnerIsOnline)
            {
                TriggerClientEvent(Players[source],"ShowErrorNotification", "Cannot start game.\nNo player with that ID is online.");
                targetThisGame = -1;
                gameMode = "none";
                return;
            }

            if (args.Count > 1 && gameMode == "delay")
            {
                Player sourceHost = Players[source];
                Player runner = Players[targetThisGame];
                DelayMode.runPlayer = runner;
                DelayMode.delayMode(sourceHost, runner, args);
            }
            else if (args.Count > 1 && gameMode == "hunt")
            {
                startGame();
            }
            else if (args.Count > 1 && gameMode == "bounce")
            {
                if (args.Count >= 3 && int.TryParse(args[2].ToString(), out int newRadius))
                {
                    GameBounce.radius = newRadius;
                }
                if (args.Count >= 4 && bool.TryParse(args[3].ToString(), out bool seesCircle))
                {
                    GameBounce.runnerSeesCircleBlip = seesCircle;
                }
                if (GameBounce.radius < 70)
                {
                    TriggerClientEvent(Players[source],"ShowErrorNotification", "Radius for bounce mode was too low, minimum is 70, default is 450. Radius is set to 70.");
                    GameBounce.radius = 70;
                }
                GameBounce.updateClientBounceSettings();
                startGame();
            }
            else if (args.Count == 2 && gameMode == "infected")
            {
                startGame();
            }
            else if ((args.Count == 2 || args.Count == 3) && gameMode == "copyclass")
            {
                if (args.Count == 3 && bool.TryParse(args[2].ToString(), out GameCopyClass.forceChosenVehicleOnly)) {}
                startGame();
            }
            else 
            {
                targetThisGame = -1;
                gameMode = "none";
                TriggerClientEvent(Players[source],"ShowErrorNotification", "No game was started. Incorrect mode set.");
            }
        }

        [Command("endgame", Restricted = true)] //normal restriction true 
        [EventHandler("endGameMode")]
        public void endGameMode(int source, List<object> args, string raw)
        {
            // Debug.WriteLine($"server endGameMode");
            if (gameMode != "none")
            {
                endGame("end");
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"{Players[source].Name} ended the game early." } });
            }
            else
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"No game is running." } });
            }
        }

        [EventHandler("startGame")]
        public void startGame()
        {
            // Debug.WriteLine($"server startGame");

            teamAssignment.Clear();
            runnerList.Clear();

            foreach (Player player in Players)
            {
                int playerId = int.Parse(player.Handle);

                if (gameMode != "infected")
                {
                    if (playerId == targetThisGame)
                    {
                        teamAssignment.Add(playerId, 1);
                        runnerList.Add(player.Name);
                    }
                    else
                    {
                        teamAssignment.Add(playerId, 2);
                    }
                }
                else //for infected target this game is hunter instead of runner. (potentially hide and seek too in the future)
                {
                    if (playerId == targetThisGame)
                    {
                        teamAssignment.Add(playerId, 2);
                    }
                    else
                    {
                        teamAssignment.Add(playerId, 1);
                        runnerList.Add(player.Name);
                    }
                }
            }

            // Prepare the team assignments for client synchronization
            List<Vector2> teamAssignmentForClient = new List<Vector2>();
            foreach (var kvp in teamAssignment)
            {
                teamAssignmentForClient.Add(new Vector2(kvp.Key, kvp.Value));
            }
            // Debug.WriteLine($"end server startGame");

            // Trigger client events to start the game.
            Armoury.turnOnPvpForGames();
            API.StopResource("playernames");
            TriggerClientEvent("startGame", teamAssignmentForClient, gameMode);
            TriggerClientEvent("gameStartNotification", runnerList);
            
            DelayMode.setOrRemoveDistanceBlipsForDelayMode(); //updates the blips on the map to indicate the distance between blip and player for delay mode. Removes it when there is not delay mode active.
        }

        [EventHandler("thisClientDiedForGameStateCheck")]
        public void thisClientDiedForGameStateCheck(int source)
        {
            // Debug.WriteLine($"server thisClientDiedForGameStateCheck");
            if ((gameMode == "delay" || gameMode == "hunt" || gameMode == "bounce" || gameMode == "copyclass") && teamAssignment.ContainsValue(1))
            {
                int teamNumber = teamAssignment[source];
                //CitizenFX.Core.Debug.WriteLine($"thisClientDiedForGameStateCheck {source},{teamNumber}");
                if (teamNumber == 1)
                {
                    endGame("hunter");
                }
            }
            else if ((gameMode == "infected") && teamAssignment.ContainsValue(1))
            {
                int teamNumber = teamAssignment[source];
                //CitizenFX.Core.Debug.WriteLine($"thisClientDiedForGameStateCheck {source},{teamNumber}");
                if (teamNumber == 1)
                {
                    teamAssignment[source] = 2; //makes client a hunter after death.
                    if (!teamAssignment.ContainsValue(1))
                    {
                        endGame("hunter");
                        TriggerClientEvent("ShowNotification", $"~o~{Players[source].Name} was the last infected survivor. Good job.");
                        return;
                    }
                    TriggerClientEvent(Players[source], "ShowSpecialNotification", "~r~~h~You died as a runner.\n~s~~w~You are now a hunter.", "SwitchRedWarning", ",SPECIAL_ABILITY_SOUNDSET");
                    GameInfected.sendClientTeamAssignmentForInfected();
                }
            }
        }

        [EventHandler("endGame")]
        public async void endGame(string winningTeam)
        {
            //  Debug.WriteLine($"server endGame");
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"EndGame {winningTeam} {gameMode}" } });

            endGameMessages(winningTeam);
            await Delay(100);
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"EndGame waited" } });

            teamAssignment.Clear();
            foreach (Player player in Players)
            {
                teamAssignment.Add(int.Parse(player.Handle), 0);
                //TriggerEvent("pma-voice:SetPlayerRadioChannel", player.Handle, 0);
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
                DelayMode.setOrRemoveDistanceBlipsForDelayMode();
                //reset server setting permissions
            }
            else if (gameMode == "hunt")
            {
                // Debug.WriteLine($"server endGame hunt");
            }
            else if (gameMode == "bounce")
            {

            }
            else if (gameMode == "infected")
            {
                TriggerClientEvent("setGameBounceBlip", new Vector4(0,0,0,0), true);
            }
            else if (gameMode == "copyclass")
            {
                
            }
            gameMode = "none";
            targetThisGame = -1;
            API.StartResource("playernames");
            
        }

        [EventHandler("playerJoinedWhileGameIsActive")]
        public void playerJoinedWhileGameIsActive(int JoinedPlayerId)
        {
            // Debug.WriteLine($"server playerJoinedWhileGameIsActive");
            Player joinedPlayer = Players[JoinedPlayerId];
            //TriggerClientEvent(joinedPlayer, "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"playerJoinedWhileGameIsActive server" } });
            if (gameMode == "none")
            {
                return;
            }
            int id = int.Parse(joinedPlayer.Handle);
            teamAssignment[id] = 2;
            //TriggerEvent("pma-voice:SetPlayerRadioChannel", id, teamAssignment);
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
            if (gameMode == "infected") GameInfected.sendClientTeamAssignmentForInfected();
        }

        [EventHandler("sendClientTeamAssignment")]
        public void sendClientTeamAssignment()
        {

            Debug.WriteLine($"updateClientTeamAssignment");
            List<Vector2> teamAssignmentForClient = new List<Vector2>();
            foreach (Player player in Players)
            {
                int playerId = int.Parse(player.Handle);
                if (!RoundHandling.teamAssignment.ContainsKey(playerId)) RoundHandling.teamAssignment[playerId] = 0;
            }
            
            foreach (var kvp in RoundHandling.teamAssignment)
            {
                teamAssignmentForClient.Add(new Vector2(kvp.Key, kvp.Value));
            }

            TriggerClientEvent("updateClientTeamAssignment", teamAssignmentForClient);
        }

        public void endGameMessages(string winningTeam)
        {
            // Debug.WriteLine($"server endGameMessages");
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"EndGameMessages {winningTeam}" } });
            List<int> winners = new List<int>();
            List<int> losers = new List<int>();
            List<int> neutral = new List<int>();
            foreach (var kvp in teamAssignment)
            {
                if (winningTeam == "end")
                {
                    // Debug.WriteLine($"server endGameMessages end");
                    neutral.Add(kvp.Key);
                    continue;
                }
                if (teamNumberDict.ContainsKey(winningTeam))
                {
                    // Debug.WriteLine($"server endGameMessages {winningTeam}");
                    if (kvp.Value == teamNumberDict[winningTeam])
                    {
                        winners.Add(kvp.Key);
                        // Debug.WriteLine($"server endGameMessages winner {kvp.Key}");
                    }
                    else if (kvp.Value != 0)
                    {
                        // Debug.WriteLine($"server endGameMessages loser {kvp.Key}");
                        losers.Add(kvp.Key);
                    }
                    else
                    {
                        // Debug.WriteLine($"server endGameMessages neutral {kvp.Key}");
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
                // Debug.WriteLine($"server endGameMessages winner count 0");
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
        [Command("gamestatus", Restricted = true)] //normal restriction true 
        public void gameStatus(int source, List<object> args, string raw)
        {
            // Debug.WriteLine($"server gameStatus");
            if (gameMode == "infected")
            {
                int hunters = 0;
                int runners = 0;
                foreach (var kvp in teamAssignment)
                {
                    if (kvp.Value == 1) runners ++;
                    else if (kvp.Value == 2) hunters ++;
                }
                TriggerClientEvent(Players[source], "ShowNotification", $"There is a game of {gameMode} running. There are {hunters} hunters and {runners} runners.");
            }
            else if (gameMode != "none")
            {
                TriggerClientEvent(Players[source], "ShowNotification", $"There is a game of {gameMode} running with {Players[targetThisGame].Name} as the runner.");
            }
            else
            {
                TriggerClientEvent(Players[source], "ShowNotification", $"There is no game running.");
            }
        }
    }
}
