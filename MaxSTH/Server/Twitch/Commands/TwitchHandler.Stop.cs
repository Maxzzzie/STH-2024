using CitizenFX.Core;

namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler
    {
        public void Stop(Player player, string[] args)
        {
            TriggerMessage("Stopping Twitch listener", player);
            GetTwitchListener().Stop();
        }
    }
}
