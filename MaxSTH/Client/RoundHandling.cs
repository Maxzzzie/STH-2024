using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using STHMaxzzzie.Client;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class RoundHandling : BaseScript
    {
        public static int thisClientIsTeam = 0; //0= not assigned a team, 1= runner, 2= hunter 3= spectator
        public static Dictionary<int, int> teamAssignment = new Dictionary<int, int>();
        public static string gameMode = "none";
        int serverId = Game.Player.ServerId;

        [EventHandler("startGame")]
        public void startGame(List<object> playerIdAndTeamAssignment, string newGameMode)
        {
            //Debug.WriteLine($"client startGame");

            gameMode = newGameMode;

            teamAssignment.Clear();
            foreach (var obj in playerIdAndTeamAssignment)
            {
                if (obj is Vector2 vector)
                {
                    int playerId = (int)vector.X;
                    int team = (int)vector.Y;
                    teamAssignment.Add(playerId, team);
                    //TriggerEvent("pma-voice:SetPlayerRadioChannel", playerId, team);
                    if (playerId == serverId)
                    {
                        thisClientIsTeam = team;
                    }
                }
            }
            if (gameMode == "hunt")
            {
                //Debug.WriteLine($"client startGame was Hunt");
                startHuntMode();
                
            }
            else if (gameMode == "delay")
            {
                startDelayMode();
            }

            if (thisClientIsTeam == 1)
                {
                    Health.SetPlayerStats(300, 100);
                    TriggerEvent("HealCompletely");
                }
            else
            {
            Health.SetPlayerStats(300, 0);
            }

        }

        [EventHandler("clientEndGame")]
        public void clientEndGame(List<object> playerIdAndTeamAssignment)
        {
            Debug.WriteLine($"client clientEndGame");
            gameMode = "none";
            TriggerEvent("HealHalf");

            teamAssignment.Clear();
            foreach (var obj in playerIdAndTeamAssignment)
            {
                if (obj is Vector2 vector)
                {
                    int playerId = (int)vector.X;
                    int team = (int)vector.Y;
                    teamAssignment.Add(playerId, team);
                    if (playerId == serverId)
                    {
                        thisClientIsTeam = team;
                    }
                }
            }
        }

        public void startHuntMode()
        {
            //Debug.WriteLine($"client startHuntMode");
            if (thisClientIsTeam == 1) { TriggerEvent("runWeapon", false); }
            else if (thisClientIsTeam == 2) { TriggerEvent("huntWeapon", false); }
        }
        public void startDelayMode()
        {
            //Debug.WriteLine($"client startHuntMode");
            if (thisClientIsTeam == 1) { TriggerEvent("runWeapon", false); }
            else if (thisClientIsTeam == 2) { TriggerEvent("huntWeapon", false); }
        }

        [EventHandler("gameStartNotification")]
        public void GameStartNotification(List<object> runnerList)
        {
            //Debug.WriteLine($"client gameStartNotification");

            if (runnerList.Count == 0)
            {
                startNotification("unknown");
            }
            else
            {
                // Convert and handle potential conversion errors
                List<string> runnerNamesList = new List<string>();
                foreach (var obj in runnerList)
                {
                    if (obj is string name)
                    {
                        runnerNamesList.Add(name);
                    }
                }

                string runnerNames = string.Join(", ", runnerNamesList);
                startNotification(runnerNames);
            }
        }

        [EventHandler("updateTeamAssignment")]
        public void updateTeamAssignment(int joinedPlayerId)
        {
            //Debug.WriteLine($"client updateTeamAssignment");
            teamAssignment[joinedPlayerId] = 2;
        }

        [EventHandler("gameWonNotification")]
        public void gameWonNotification(List<object> winners, List<object> losers)
        {
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"gameWonNotification" } });
            Debug.WriteLine($"client gameWonNotification");
            wonNotification(winners, losers);
        }

        [EventHandler("gameLostNotification")]
        public void gameLostNotification(List<object> winners, List<object> losers)
        {
            //Debug.WriteLine($"client gameLostNotification");
            lostNotification(winners, losers);
        }

        [EventHandler("gameNeutralNotification")]
        public void gameNeutralNotification(string winningTeam)
        {
           //Debug.WriteLine($"client gameNeutralNotification");
            neutralNotification(winningTeam);
        }

        [EventHandler("gameDrawNotification")]
        public void gameDrawNotification()
        {
            //Debug.WriteLine($"client gameDrawNotification");
            drawNotification();
        }

        [EventHandler("gameJoinNotification")]
        public void gameJoinNotification(List<object> runnerList)
        {
            //Debug.WriteLine($"client gameJoinNotification");
             //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"gameJoinNotification" } });

            if (runnerList.Count == 0)
            {
                joinNotification("unknown");
            }
            else
            {
                // Convert and handle potential conversion errors
                List<string> runnerNamesList = new List<string>();
                foreach (var obj in runnerList)
                {
                    if (obj is string name)
                    {
                        runnerNamesList.Add(name);
                    }
                }

                string runnerNames = string.Join(", ", runnerNamesList);
                joinNotification(runnerNames);
            }
        }

        public async Task startNotification(string runnerNames)
        {
           // Debug.WriteLine($"client startNotification");
            string notificationText = "null";

             notificationText = $"A new round of \"{gameMode}\" is starting. \n{runnerNames} is a runner.";

            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"startNotification" } });

            await DisplayCenteredNotification(notificationText, 0, 255, 0, 255);
        }

        public async Task wonNotification(List<object> winners, List<object> losers)
        {
            //Debug.WriteLine($"client wonNotification");
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"wonNotification" } });

            // Convert winners and losers to List<int>
            List<int> winnerIds = winners.ConvertAll(obj => Convert.ToInt32(obj));
            List<int> loserIds = losers.ConvertAll(obj => Convert.ToInt32(obj));

            string winnerNames = string.Join(", ", winnerIds.ConvertAll(id => API.GetPlayerName(id)));

            string notificationText;
            if (winnerIds.Count == 1)
            {
                Debug.WriteLine($"client wonNotification winners = 1");
                notificationText = $"Game over! You win this round of {gameMode}.";
            }
            else if (winnerIds.Count > 1)
            {
                Debug.WriteLine($"client wonNotification winners > 1");
                if (winnerNames.Length > 10)
                {
                    notificationText = $"Game over!\nYour team wins this round of {gameMode}";
                }
                else
                {
                    notificationText = $"Game over!\nYour team wins this round of {gameMode}.";
                }
            }
            else
            {
                notificationText = $"Unknown win in {gameMode}";
            }

            await DisplayCenteredNotification(notificationText, 0, 255, 0, 255); // Green for win
        }

        public async Task lostNotification(List<object> winners, List<object> losers)
        {
            //Debug.WriteLine($"client lostNotification");
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"lostNotification" } });

            // Convert winners and losers to List<int>
            List<int> winnerIds = winners.ConvertAll(obj => Convert.ToInt32(obj));
            List<int> loserIds = losers.ConvertAll(obj => Convert.ToInt32(obj));

            string winnerNames = string.Join(", ", winnerIds.ConvertAll(id => API.GetPlayerName(id)));

            string notificationText;
            if (winnerIds.Count == 1)
            {
                //Debug.WriteLine($"client lostNotification winners = 1");
                notificationText = $"Game over!\nYou lost this round of {gameMode}.";
            }
            else if (winnerIds.Count > 1)
            {
                //Debug.WriteLine($"client lostNotification winners > 1");
                if (winnerNames.Length > 10)
                {
                    notificationText = $"Game over!\nYour team lost this round of {gameMode}";
                }
                else
                {
                    notificationText = $"Game over!\nYour team lost this round of {gameMode}";
                }
            }
            else
            {
                
                notificationText = $"Game over. \nYou lost this round.";
            }

            await DisplayCenteredNotification(notificationText, 255, 0, 0, 255); // Red for loss
        }

        public async Task drawNotification()
        {
            //Debug.WriteLine($"client drawNotification");
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"gameDrawNotification" } });
            string notificationText = "The game ended in a draw.";

            await DisplayCenteredNotification(notificationText, 255, 255, 255, 255);
        }

        public async Task neutralNotification(string winningTeam)
        { string notificationText;
        //Debug.WriteLine($"client neutralNotification");
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"gameNeutralNotification" } });
            if (winningTeam == "draw")
            {
                notificationText = $"This round of {gameMode} has ended before a winner was determined.";
            }
            else
            {
                notificationText = $"This round of \"{gameMode}\" has ended. The {winningTeam} team has won!";
            }

            await DisplayCenteredNotification(notificationText, 255, 255, 255, 255);
        }

        public async Task joinNotification(string runnerNames)
        {
            //Debug.WriteLine($"client joinNotification");
            string notificationText = "null";
            notificationText = $"You joined a round of \"{gameMode}\".\n{runnerNames} is the runner.";
            //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"startNotification" } });
            await DisplayCenteredNotification(notificationText, 0, 255, 0, 255);
        }

        private async Task DisplayCenteredNotification(string text, int r, int g, int b, int a)
        { 
            //Debug.WriteLine($"client DisplayCenteredNotification");
            float baseX = 0.5f; // Center X
            float baseY = 0.5f; // Center Y
            float scale = 0.6f; // Text scale

            // Split text into lines
            string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.None);
            int lineCount = lines.Length;

            // Measure the width and height based on the longest line
            float textWidth = MeasureTextWidth(text, scale);
            float textHeight = 0.04f * scale; // Height of one line
            float lineSpacing = 0.015f; // Additional spacing between lines
            float rectWidth = textWidth + 0.05f; // Add padding for background
            float rectHeight = (textHeight * lineCount) + (lineSpacing * (lineCount - 1)) + 0.03f; // Adjust height based on line count and spacing

            // Center the text block by adjusting baseY
            float textY = baseY - (rectHeight / 2) + (textHeight / 2); // Center the entire text block within rectangle

            // Draw the notification for a few seconds
            DateTime endTime = DateTime.Now.AddSeconds(5);
            while (DateTime.Now < endTime)
            {
                // Draw background rectangle
                Function.Call(Hash.DRAW_RECT, baseX, baseY + 0.01f, rectWidth, rectHeight, 0, 0, 0, 150);

                // Draw each line of text with added spacing
                for (int i = 0; i < lineCount; i++)
                {
                    float lineY = textY + (i * (textHeight + lineSpacing)); // Adjust Y position per line, including spacing
                    DrawTextOnScreen(lines[i], baseX, lineY, scale, r, g, b, a);
                }

                await BaseScript.Delay(0); // Run in a loop to keep it displayed
            }
        }





        private void DrawTextOnScreen(string text, float x, float y, float scale, int r, int g, int b, int a)
        {
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_PROPORTIONAL, true);
            Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
            Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
            Function.Call(Hash.SET_TEXT_CENTRE, true);
            Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
            Function.Call(Hash._DRAW_TEXT, x, y);
        }

        private float MeasureTextWidth(string text, float scale)
        {
            Function.Call(Hash._BEGIN_TEXT_COMMAND_GET_WIDTH, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
            return Function.Call<float>(Hash._END_TEXT_COMMAND_GET_WIDTH, true);
        }
    }
}