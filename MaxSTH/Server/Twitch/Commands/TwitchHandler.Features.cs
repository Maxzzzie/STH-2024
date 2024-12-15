using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TwitchTestClient.Server.Twitch.Features;

namespace TwitchTestClient.Server.Twitch.Commands
{
    public partial class TwitchHandler
    {
        public void EnableFeatures(Player player, string[] args)
        {
            ToggleFeatures(player, args, true);
        }
        public void DisableFeatures(Player player, string[] args)
        {
            ToggleFeatures(player, args, false);
        }

        private void ToggleFeatures(Player player, string[] args, bool enable)
        {
            if (args.Length == 0)
            {
                TriggerMessage("You need to provide at least 1 feature to turn enable", player);
                return;
            }

            TriggerMessage($"{(enable ? "Enabling" : "Disabling")} the following features: {string.Join(", ", args)}");

            List<string> availableFeatures = AvailableFeatures();
            List<string> failedFeatures = new List<string>();
            foreach (string arg in args)
            {
                if (!availableFeatures.Contains(arg))
                {
                    failedFeatures.Add(arg);
                    continue;
                }
                if (enabledFeatures.ContainsKey(arg))
                    enabledFeatures[arg] = enable;
                else 
                    enabledFeatures.Add(arg, enable);
            }

            if (failedFeatures.Count > 0)
            {
                TriggerMessage($"The following features could not be {(enable ? "enabled" : "disabled")}: {string.Join(", ", failedFeatures)}", player);
            }
        }

        public void ClearFeatures(Player player, string[] args)
        {
            enabledFeatures.Clear();
            TriggerMessage("Features cleared");
        }

        public void GetFeatures(Player player, string[] args)
        {
            TriggerMessage($"The following features are enabled: {string.Join(", ", enabledFeatures.Where(kv => kv.Value).Select(kv => kv.Key).ToArray())}");
        }

        public void GetAvailableFeatures(Player player, string[] args)
        {
            TriggerMessage($"The following features are available: {string.Join(", ", AvailableFeatures())}");
        }

        #region Helper functions
        private Dictionary<string, TwitchBaseFeature> features = new Dictionary<string, TwitchBaseFeature>();
        public static void RegisterFeature(TwitchBaseFeature feature)
        {
            Instance.features.Add(feature.GetFeature(), feature);
            Debug.WriteLine($"Feature registered: {feature.GetFeature()}");
        }
        private List<string> AvailableFeatures()
        {
            return features.Keys.ToList();
        }
        #endregion Helper functions
    }
}
