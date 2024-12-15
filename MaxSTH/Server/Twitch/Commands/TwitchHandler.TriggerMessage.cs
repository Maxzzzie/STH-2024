using CitizenFX.Core;
using System.Collections.Generic;

namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler
    {
        public void TriggerMessage(string message, Player player = null)
        {
            List<object> messagesToSend = new List<object>
            {
                "Twitch",
                message
            };
            if (player == null)
            {
                TriggerClientEvent("chat:addMessage", new
                {
                    color = GetTextColor(),
                    multiline = true,
                    args = messagesToSend.ToArray()
                });
            }
            else
            {
                TriggerClientEvent(player, "chat:addMessage", new
                {
                    color = GetTextColor(),
                    multiline = true,
                    args = messagesToSend.ToArray()
                });
            }
        }

        public void TriggerErrorMessage(Player player)
        {
            TriggerMessage("Please enter at least one parameter. Try running 'twitch help' for more information.", player);
        }
    }
}
