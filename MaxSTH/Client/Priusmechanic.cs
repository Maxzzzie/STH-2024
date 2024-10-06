using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class pinkPrius : BaseScript
    {
        [Command("pri")]
        void pri(int source, List<object> args, string raw)
        {
            TriggerServerEvent("pri-spawn-requested", VehicleHash.Dilettante, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading - 180);
        }


        //[Tick]
        //public async Task OnTick()
        //{
        //    if (priIsAlive)
        //    {
        //        await Delay(1000);
        //        float priusEngineHealth = API.GetVehicleEngineHealth(prius.Handle);
        //        if (priusEngineHealth <= 0)
        //        {
        //            priIsAlive = false;
        //            int cause = API.GetVehicleCauseOfDestruction(prius.Handle);
        //            TriggerEvent("chat:addMessage", new { color = new[] { 180, 100, 180 }, args = new[] { $"Your pri is destroyed." } });
        //        }
        //    }
        //}
    }
}
