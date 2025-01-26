using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;


namespace STHMaxzzzie.Server
{
    public class UpdateConfig : BaseScript
    {

        public UpdateConfig()
        {
            LoadResources.LoadConfig();
        }

        [Command("updateconfig", Restricted = true)]
        public void updateConfig(int source, List<object> args, string raw)
        {
            LoadResources.LoadConfig();
        }

        [EventHandler("SendConfigToClient")]
        static public void SendConfigToClient()
        {
            TriggerClientEvent("updateDefaultSpawnLocation", LoadResources.DefaultSpawnLocation);
            TriggerClientEvent("updateMugshotArea", LoadResources.MugshotPosition);
            Debug.WriteLine($"Sending client {LoadResources.MugshotPosition.X}, {LoadResources.MugshotPosition.Y}, {LoadResources.MugshotPosition.Z}, {LoadResources.MugshotPosition.W}");
            TriggerClientEvent("updateMessageOfTheDay", LoadResources.MOTD);
            Misc.updateFireStatus();
        }
    }
}