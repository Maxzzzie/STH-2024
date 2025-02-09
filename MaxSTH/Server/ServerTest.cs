using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Linq;

namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        bool isGfredActive = false;

        [Command("test", Restricted = false)]
        public void TestCommand(int source, List<object> args, string raw)
        {
            if (args.Count == 1 && args[0].ToString() == "audio") TriggerClientEvent(Players[source], "clientTest1");
            else if (args.Count == 2 && args[0].ToString() == "veh") TriggerClientEvent(Players[source], "randVeh", args[1].ToString());
            else if (args.Count == 1 && args[0].ToString() == "gfred")
            {
                if (!isGfredActive)
                {
                    startGfred();

                    isGfredActive = true;
                }
                else isGfredActive = false;
            }
            else if (args.Count == 2 && args[0].ToString() == "carlist" && int.TryParse(args[1].ToString(), out int value)) TriggerClientEvent(Players[source], "testAllVehiclesFrom", value);
            else TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Error test." } });
        }

        List<DateTime> startTimes = new List<DateTime>();
        private Dictionary<int, Vector2> scoreBoard = new Dictionary<int, Vector2>();
        private Dictionary<int, string[]> endScoreBoard = new Dictionary<int, string[]>();
        private List<int> finishedPlayers = new List<int>();
        private int playerInFirst = 0;
        private DateTime lastUpdateTime;
        private static bool firstPlayerFinished = false;
        int totalCpCount = 16;

        async void startGfred()
        {
            scoreBoard.Clear();
            finishedPlayers.Clear();
            endScoreBoard.Clear();
            firstPlayerFinished = false;
            foreach (Player player in Players)
            {
                Vector3 pos1 = new Vector3(3667.31f, 4924.25f, 15.48f);//gfred
                Vector3 pos2 = new Vector3(3691.54f, 4947.95f, 23.01f);//gfred
                // Vector3 pos1 = new Vector3(570.51f, -754.26f, 10.73f);
                // Vector3 pos2 = new Vector3(590.51f, -754.26f, 30.73f);
                Vector3 randomPos = GetRandomPosition(pos1, pos2);
                Debug.WriteLine("triggerevent for player tpPlayerOnFloor");
                TriggerClientEvent(player, "tpPlayerOnFloor", new Vector4(randomPos.X, randomPos.Y, randomPos.Z, 46));//gfred

                await Delay(1);
            }
            TriggerClientEvent("Gfred");
            StartRace();
        }

        [EventHandler("receiveStartTime")]
        void ReceiveStartTime(string time)
        {
            if (DateTime.TryParse(time, out DateTime parsedTime))
            {
                startTimes.Add(parsedTime);
            }
        }


        [EventHandler("updateFirstPlaceAndScoreboard")]
        private void UpdateScoreBoard(int serverId, int currentIndex, int distance)
        {
            if (!isGfredActive) return;
            scoreBoard[serverId] = new Vector2(currentIndex, distance);
            lastUpdateTime = DateTime.UtcNow;
        }

        public async void StartRace()
        {
            endScoreBoard.Clear();
            foreach (Player player in Players)
            {
                string[] temp = new string[2] { player.Name, "dnf" };
                endScoreBoard[int.Parse(player.Handle)] = temp;
                Debug.WriteLine($"Adding {player.Handle} to the dict. with {temp[0]} and {temp[1]}.");
            }

            // Wait until all players have sent their start time
            while (startTimes.Count < endScoreBoard.Count) await Delay(1);

            // Calculate the median start time as the middle between the lowest and highest times
            DateTime minTime = startTimes.First();
            DateTime maxTime = startTimes.Last();
            DateTime medianStartTime = new DateTime((minTime.Ticks + maxTime.Ticks) / 2);
            lastUpdateTime = DateTime.UtcNow;

            ShowLeaderboard(medianStartTime);

            CheckRaceEnd();

            while (isGfredActive && finishedPlayers.Count == 0)
            {
                CheckLeader();
                await Delay(3000);
            }
        }

        private void CheckLeader()
        {
            if (scoreBoard.Count == 0 || firstPlayerFinished) return;

            var newLeader = scoreBoard.OrderByDescending(p => p.Value.X).ThenBy(p => p.Value.Y).FirstOrDefault().Key;

            if (newLeader != playerInFirst)
            {
                playerInFirst = newLeader;
                TriggerClientEvent("ShowSpecialNotification", $"{GetPlayerName(playerInFirst)} entered first!", "FIRST_PLACE", "HUD_MINI_GAME_SOUNDSET");
            }
        }

        // private async void ShowLeaderboard(DateTime medianStartTime)
        // {
        //     while (isGfredActive)
        //     {
        //         var nextUpdate = medianStartTime.AddSeconds(60 * ((DateTime.UtcNow - medianStartTime).TotalMinutes + 1));
        //         var delay = (int)(nextUpdate - DateTime.UtcNow).TotalMilliseconds;
        //         await Delay(Math.Max(delay, 1000));

        //         if (scoreBoard.Count == 0) continue;
        //         var sortedPlayers = scoreBoard.Where(p => !finishedPlayers.Contains(p.Key)).OrderByDescending(p => p.Value.X).ThenBy(p => p.Value.Y).ToList();

        //         foreach (var (index, kvp) in sortedPlayers.Select((value, i) => (i, value)))
        //         {
        //             string place = index switch { 0 => "1st", 1 => "2nd", 2 => "3rd", _ => $"{index + 1}th" };
        //             TriggerClientEvent("ShowSpecialNotification", $"{place} {GetPlayerName(kvp.Key)} - Checkpoint {kvp.Value.X}/{totalCpCount}", "", "");
        //         }
        //     }
        // }

private int currentPage = 0;

    public async void ShowLeaderboard(DateTime medianStartTime)
    {
        while (isGfredActive)
        {
            var nextUpdate = medianStartTime.AddSeconds(60 * ((DateTime.UtcNow - medianStartTime).TotalMinutes + 1));
            var delay = (int)(nextUpdate - DateTime.UtcNow).TotalMilliseconds;
            await Delay(Math.Max(delay, 1000));

            if (scoreBoard.Count == 0 || !isGfredActive) continue;

            // Separate finished and unfinished players
            var finishedList = finishedPlayers
                .Select((playerId, index) => new { PlayerId = playerId, Position = index + 1 })
                .ToList();

            var unfinishedList = scoreBoard
                .Where(p => !finishedPlayers.Contains(p.Key))
                .OrderByDescending(p => p.Value.X)
                .ThenBy(p => p.Value.Y)
                .ToList();

            // Merge finished first, then unfinished
            var sortedPlayers = finishedList
                .Select(f => new { Place = $"{f.Position}{GetOrdinal(f.Position)}", Name = GetPlayerName(f.PlayerId), Status = "finished" })
                .Concat(unfinishedList.Select((kvp, i) => new { Place = $"{i + 1 + finishedList.Count}{GetOrdinal(i + 1 + finishedList.Count)}", Name = GetPlayerName(kvp.Key), Status = $"Checkpoint {kvp.Value.X}/{totalCpCount}" }))
                .ToList();

            if (sortedPlayers.Count == 0) continue;

            // **Pagination**: Display in groups of 4
            int itemsPerPage = 4;
            var pagedPlayers = sortedPlayers.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();
            currentPage = (currentPage + 1) % ((sortedPlayers.Count + itemsPerPage - 1) / itemsPerPage); // Cycle through pages

            // Construct the notification message
            string message = string.Join("\n", pagedPlayers.Select(p => $"{p.Place} {p.Name} {p.Status}"));
            TriggerClientEvent("ShowSpecialNotification", message, "", "");
        }
    }

    // Fixed GetOrdinal for C# 8.0 compatibility
    private static string GetOrdinal(int number)
    {
        int lastTwoDigits = number % 100;
        if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
            return "th";

        int lastDigit = number % 10;
        switch (lastDigit)
        {
            case 1: return "st";
            case 2: return "nd";
            case 3: return "rd";
            default: return "th";
        }
    }


        private async void CheckRaceEnd()
        {
            await Delay(15000);
            while (isGfredActive)
            {
                await Delay(500);
                if ((DateTime.UtcNow - lastUpdateTime).TotalSeconds >= 10)
                {
                    isGfredActive = false;
                    ShowFinalResults();
                }
            }
        }

        private void ShowFinalResults()
        {
            var sortedResults = endScoreBoard
                .OrderBy(kvp => kvp.Value[1] == "dnf" ? 1 : 0)
                .ThenBy(kvp => kvp.Value[1])
                .ToList();

            // Full results for debug line.
            string fullResultMessage = "Race Results:\n";
            for (int i = 0; i < sortedResults.Count; i++)
            {
                fullResultMessage += $"{i + 1}. {sortedResults[i].Value[0]} Time: {sortedResults[i].Value[1]}\n";
            }

            // Only top 3 results for notification
            string topThreeMessage = $"Top {Math.Min(3, sortedResults.Count)} Race Results:\n";
            for (int i = 0; i < Math.Min(3, sortedResults.Count); i++)
            {
                topThreeMessage += $"{i + 1}. {sortedResults[i].Value[0]} | {sortedResults[i].Value[1]}\n";
            }

            TriggerClientEvent("ShowNotification", topThreeMessage); // Only top 3 results
            TriggerClientEvent("DisplayClientDebugLine", fullResultMessage); // Full results for debugging
        }

        private string GetPlayerName(int playerId)
        {
            Player player = Players.FirstOrDefault(p => p.Handle == playerId.ToString());
            return player != null ? player.Name : $"Player {playerId}";
        }

        [EventHandler("sendRaceResults")]
        void getRaceResults(int playerId, string endTime)
        {
            if (!finishedPlayers.Contains(playerId))
            {
                finishedPlayers.Add(playerId);
                scoreBoard[playerId] = new Vector2(totalCpCount, 0);
            }
            if (endScoreBoard.ContainsKey(playerId))
            {
                endScoreBoard[playerId][1] = endTime;
                firstPlayerFinished = true;

                if (endScoreBoard.Count == finishedPlayers.Count) { ShowFinalResults(); isGfredActive = false; isGfredActive = false; return; }
                int place = finishedPlayers.Count;
                string suffix = place switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" };
                TriggerClientEvent("ShowNotification", $"{GetPlayerName(playerId)} finished {place}{suffix}\nTime: {endTime}");
            }
        }


        public static Vector3 GetRandomPosition(Vector3 pointA, Vector3 pointB)
        {
            float t = (float)new Random().NextDouble(); // Get a random value between 0 and 1
            return Vector3.Lerp(pointA, pointB, t); // Interpolates between pointA and pointB
        }

        [Command("rpi", Restricted = true)]
        void rpi(int source, List<object> args, string raw)
        {
            TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Did you mean to do /pri?" } });
            foreach (Player notGil in Players)
                if (notGil != Players[source])
                {
                    TriggerClientEvent(notGil, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Hahaha, Gilly wrote /pri wrong! He typed /rpi." } });
                }
        }


    }
}
