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
        Dictionary<string, VehicleHash> VehicleNameToHash = null;
        public pinkPrius()
        {
            //make a dictionary mapping vehicle name => hash
            //https://stackoverflow.com/a/5583817
            VehicleNameToHash = new Dictionary<string, VehicleHash>();
            foreach (var veh_hash in Enum.GetValues(typeof(VehicleHash)))
            {
                VehicleNameToHash.Add(veh_hash.ToString().ToLower(), (VehicleHash)veh_hash);
            }
        }

        Vehicle prius;
        bool priIsAlive = false;
        [Command("pri")]
        async void pri(int source, List<object> args, string raw)
        {
            if (priIsAlive)
            {
                APISetEntityAsMissionEntity(prius.Handle, true, true);
                API.DeleteVehicle(prius.Handle);
                TriggerEvent("chat:addMessage", new { color = new[] { 180, 100, 180 }, args = new[] { $"You replaced a prius." } });
                var model = new Model(VehicleNameToHash["dilettante"]);
                prius = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading - 180);
                API.SetVehicleColours(prius.Handle, 135, 135);
                API.SetVehicleNumberPlateText(prius.Handle, $"{Players[source]}");
                API.SetVehicleNeonLightEnabled(prius.Handle, 0, true);
                API.SetVehicleNeonLightEnabled(prius.Handle, 1, true);
                API.SetVehicleNeonLightEnabled(prius.Handle, 2, true);
                API.SetVehicleNeonLightEnabled(prius.Handle, 3, true);
                API.SetVehicleNeonLightsColour(prius.Handle, 204, 0, 204);
                API.SetVehicleXenonLightsColor(prius.Handle, 10);
                API.SetVehicleLightsMode(prius.Handle, 2);
                API.SetNetworkIdCanMigrate(prius.Handle, false);
                //return;
            }
            
            TriggerEvent("chat:addMessage", new { color = new[] { 180, 100, 180 }, args = new[] { $"You spawned a prius." } });
            var model = new Model(VehicleNameToHash["dilettante"]);
            prius = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading - 180);
            API.SetVehicleColours(prius.Handle, 135, 135);
            API.SetVehicleNumberPlateText(prius.Handle, $"{Players[source]}");
            API.SetVehicleNeonLightEnabled(prius.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(prius.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(prius.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(prius.Handle, 3, true);
            API.SetVehicleNeonLightsColour(prius.Handle, 204, 0, 204);
            API.SetVehicleXenonLightsColor(prius.Handle, 10);
            API.SetVehicleLightsMode(prius.Handle, 2);
            API.SetNetworkIdCanMigrate(prius.Handle, false)
            priIsAlive = true;
           
            



        }


        [Tick]
        public async Task OnTick()
        {
            if (priIsAlive)
            {
                await Delay(1000);
                float priusEngineHealth = API.GetVehicleEngineHealth(prius.Handle);
                if (priusEngineHealth <= 0)
                {
                    priIsAlive = false;
                    int cause = API.GetVehicleCauseOfDestruction(prius.Handle);
                    TriggerEvent("chat:addMessage", new { color = new[] { 180, 100, 180 }, args = new[] { $"Your pri is destroyed." } });
                }
            }
        }
    }
}
