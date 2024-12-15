using CitizenFX.Core;

namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler
    {
        public void Help(Player player, string[] args)
        {
            TriggerMessage("The twitch command can be used with these parameters 'help', 'start' and 'stop'", player);
        }
    }
}
