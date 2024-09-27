using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder; // chat gpt addition to prevent compile error


namespace STHMaxzzzie.Server
{
    public class DeathMessages : BaseScript
    {
        public DeathMessages()
        {
            Debug.WriteLine($"DeathMessages has started.");
            EventHandlers["baseevents:onPlayerDied"] += new Action<Player, int, List<object>>(OnPlayerDied);
            EventHandlers["baseevents:onPlayerKilled"] += new Action<Player, int, dynamic>(OnPlayerKilled);
        }







        public void OnPlayerKilled([FromSource] Player victim, int two, dynamic three)
        {
            var dictionary = (IDictionary<string, object>)three;

            foreach (KeyValuePair<string, object> property in dictionary)
            {
                Debug.WriteLine($"Expando --> key: {property.Key} || Value {property.Value}");
            }
            Debug.WriteLine($"Triggered OnPlayerKilled");

            Debug.WriteLine($"died {victim.Name}, pedType?: {two}.");
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"died {victim.Name}, 2{two} 3{three}." } });
        }









        private void OnPlayerDied([FromSource] Player player, int killerType, List<object> deathCoords)
        {
            // Extract the death coordinates from the deathCoords array
            float deathX = Convert.ToSingle(deathCoords[0]);
            float deathY = Convert.ToSingle(deathCoords[1]);
            float deathZ = Convert.ToSingle(deathCoords[2]);
            Vector3 deathPosition = new Vector3(deathX, deathY, deathZ);

            Debug.WriteLine($"SERVER DeathMessages: Player: {player.Name}, killerType = {killerType} Coords = {deathPosition}.");
            //killerType is an and in this case linked to the model.
            //find info here https://docs.fivem.net/docs/resources/baseevents/events/onPlayerDied/
            //the following code makes no sence then.
            string cause = "null";

            if (killerType == -1)
            {
                cause = "law enforcement";
            }
            else if (killerType == 0)
            {
                cause = "suicide";
            }
            else if (killerType == 1)
            {
                cause = "a player";
            }
            else if (killerType == 2)
            {
                cause = "falldamage";
            }
            else if (killerType == 3)
            {
                cause = "3. a colision with an object";
            }
            else if (killerType == 4)
            {
                cause = "4. a big boom";
            }
            else if (killerType == 5)
            {
                cause = "5. something hot";
            }
            else if (killerType == 6)
            {
                cause = "6, unknown reason";
            }
            else if (killerType == 7)
            {
                cause = "7. a cougar perhaps";
            }
            else if (killerType == 8)
            {
                cause = "8. NPC's, what a loser";
            }
            else
            {
                cause = "reasons unknown";
            }
            Debug.WriteLine($"Player {player.Name} died because of {cause}. Debug info: {killerType}");
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{player.Name} died because of {cause}." } });
        }
    }
}

// public class Test : BaseScript
// {
//     [Command("test", Restricted = false)]
//     void TestCommand(int source, List<object> args, string raw)
//     {
//         Debug.WriteLine($"This is the test command.");

//         // Display notification to the player
//         API.SetNotificationTextEntry("STRING");
//         API.AddTextComponentString("This is a test notification.");
//         API.SetNotificationMessage("CHAR_CARSITE", "CHAR_CARSITE", true, 1, "Test Notification", "");
//         API.DrawNotification(false, true);
//     }
//     void test(int source, List<object> args, string raw)
//     {
//         string message = "This is the test command.";
//         string type = "CHAR_ABIGAIL";
//         TriggerClientEvent("STH:ShowNotification", source, message, type);
//     }
// }