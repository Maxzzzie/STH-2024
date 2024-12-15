using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Twitch;
using TwitchLib.Client.Models;

namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler : BaseScript
    {
        public static event Action<ChatMessage> OnChatMessageReceived;

        /* TODO: read this from file */
        private const string USERNAME = "";
        private const string CHANNEL = "";
        private const string ACCESS_TOKEN = "";

        private const string DEBUG_MESSAGE_FORMAT = "{0} sent the following message: {1}";

        private readonly TwitchListener listener;

        private List<CommandParameters> commands;

        public TwitchHandler()
        {
            instance = this;
            commands = new List<CommandParameters>
            {
                new CommandParameters("help", Help),
                new CommandParameters("start", Start),
                new CommandParameters("stop", Stop),
                new CommandParameters("features", GetFeatures), 
                new CommandParameters("enable", EnableFeatures),
                new CommandParameters("disable", DisableFeatures),
                new CommandParameters("available", GetAvailableFeatures),
                new CommandParameters("clear", ClearFeatures),
            };

            listener = new TwitchListener(USERNAME, CHANNEL, ACCESS_TOKEN);
            
            TwitchListener.OnMessageReceived += OnChatReceived;
        }

        ~TwitchHandler()
        {
            TwitchListener.OnMessageReceived -= OnChatReceived;
        }

        #region Features
        private Dictionary<string, bool> enabledFeatures = new Dictionary<string, bool>();
        public static bool IsFeatureEnabled(string feature)
        {
            var features = Instance.enabledFeatures;
            if (!features.TryGetValue(feature, out bool enabled))
                return false;
            return enabled;
        }
        #endregion Features
        #region Singleton
        private static TwitchHandler instance;
        public static TwitchHandler Instance => instance;
        #endregion Singleton

        private TwitchListener GetTwitchListener() => listener;

        private void OnChatReceived(ChatMessage msg)
        {
            OnChatMessageReceived?.Invoke(msg);
        }

        [Command("twitch")]
        public void TwitchCommand([FromSource] Player player, params string[] args)
        {
            if (args.Length == 0)
            {
                TriggerErrorMessage(player);
                return;
            }

            string subcommand = args[0];
            CommandParameters command = commands.FirstOrDefault(c => c.Subcommand == subcommand);
            if (command == null)
            {
                TriggerErrorMessage(player);
                return;
            }

            command.Function?.Invoke(player, args.Skip(1).ToArray());
        }
    }

    public class CommandParameters
    {
        public CommandParameters(string subCommand, Action<Player, string[]> function)
        {
            Subcommand = subCommand;
            Function = function;
        }

        public string Subcommand { get; set; } = string.Empty;
        public Action<Player, string[]> Function { get; set; }
    }
}
