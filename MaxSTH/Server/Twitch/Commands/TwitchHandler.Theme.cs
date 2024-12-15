namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler
    {
        private readonly int[] TWITCH_TEXT_COLOR = new int[3] { 145, 70, 255 };

        public int[] GetTextColor() => TWITCH_TEXT_COLOR;
    }
}
