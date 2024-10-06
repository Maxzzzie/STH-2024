using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace STHMaxzzzie.Server
{
    public class ServerCommands : BaseScript
    {
        public ServerCommands()
        {
            // Register the server-side command "mycommand"
            API.RegisterCommand("mycommand", new Action<int, List<object>, string>(HandleMyCommand), false);
        }

        // Function to handle the command
        private void HandleMyCommand(int source, List<object> args, string rawCommand)
        {
            // Get the player who executed the command
            Player player = Players[source];
            string playerName = player.Name;

            // Example action: Log to server console
            CitizenFX.Core.Debug.WriteLine($"{playerName} executed /mycommand with arguments: {string.Join(" ", args)}");

            // Example action: Send a confirmation message to the player
            TriggerClientEvent(player, "chat:addMessage", new
            {
                color = new[] { 0, 255, 0 },
                args = new[] { "Server", $"You executed /mycommand with arguments: {string.Join(" ", args)}" }
            });
        }
    }
}