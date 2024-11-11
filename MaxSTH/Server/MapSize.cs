using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
using System.Linq;

namespace STHMaxzzzie.Server
{
    public class MapSize : BaseScript
    {
        [Command("map", Restricted = false)]
        void map(int source, List<object> args, string raw)
        {
            TriggerClientEvent(Players[source], "setClientMapSize");
        }
    }
}