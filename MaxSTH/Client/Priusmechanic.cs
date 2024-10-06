using CitizenFX.Core;
using System.Collections.Generic;

namespace STHMaxzzzie.Client
{
    public class pinkPrius : BaseScript
    {
        [Command("pri")]
        private void Pri(int source, List<object> args, string raw)
        {
            TriggerServerEvent("pri-spawn-requested", VehicleHash.Dilettante, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading - 180);
        }
    }
}
