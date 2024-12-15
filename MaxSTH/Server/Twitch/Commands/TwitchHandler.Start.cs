using CitizenFX.Core;

namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler
    {
        public void Start(Player player, string[] args)
        {
            TriggerMessage("Starting Twitch listener", player);
            if (args.Length > 0)
            {
                EnableFeatures(player, args);
            }
            GetTwitchListener().Start();
            TriggerMessage("Started Twitch listener", player);
        }
    }
}
