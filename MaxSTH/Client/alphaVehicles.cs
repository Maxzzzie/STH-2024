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
    public class max_Vehicle : BaseScript
    {
        string allowedToFixStatus = "wait"; //can be on/off/wait/lsc.
        int timeStationairBeforeFix = 10;
        static Dictionary<string, string> vehicleinfoDict = new Dictionary<string, string>();
        Dictionary<string, VehicleHash> VehicleNameToHash = null;
        public max_Vehicle()
        {
            //make a dictionary mapping vehicle name => hash
            //https://stackoverflow.com/a/5583817
            VehicleNameToHash = new Dictionary<string, VehicleHash>();
            foreach (var veh_hash in Enum.GetValues(typeof(VehicleHash)))
            {
                VehicleNameToHash.Add(veh_hash.ToString().ToLower(), (VehicleHash)veh_hash);
            }
            TriggerServerEvent("sendVehicleinfoDict");
        }


        [EventHandler("getVehicleinfoDict")]
        void getVehicleinfoDict(string vehicleName, string vehicleInfo)
        {
            vehicleinfoDict.Add(vehicleName, vehicleInfo);
        }


        [EventHandler("VehicleFixStatus")]
        void whatIsVehicleFixStatus(string vehicleFixStatus, int fixWaitTime)
        {
            allowedToFixStatus = vehicleFixStatus;
            timeStationairBeforeFix = fixWaitTime;
        }

        //spawning a vehicle in front of the player.
        [Command("veh")]
        async void vehicle(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"What vehicle do you want to spawn?" } });
            }
            else if (args.Count == 1 && VehicleNameToHash.ContainsKey(args[0].ToString()))
            {
                var model = new Model(VehicleNameToHash[args[0].ToString()]);
                Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, multiline = true, args = new[] { $"You spawned an {args[0]}." } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Oh no. Something went wrong!\nYou should do /veh \"vehiclename\"." } });
            }
        }

        //places player in vehicle when spawning one.
        [Command("inveh")]
        async void inVehicle(int source, List<object> args, string raw)
        {
            if (args.Count == 1 && VehicleNameToHash.ContainsKey(args[0].ToString()))
            {
                var model = new Model(VehicleNameToHash[args[0].ToString()]);
                Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned an {args[0]}." } });
                Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                Game.PlayerPed.CurrentVehicle.CreateRandomPedOnSeat(VehicleSeat.Passenger);
                API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            }
        }

        //For vehicle colors and customisations look here. The original pastebins aren't functional anymore in the natives list.

        //https://wiki.rage.mp/index.php?title=Vehicle_Mods
        //https://wiki.rage.mp/index.php?title=Vehicle_Colors





        //spawns a custom fq2 for cornelius, 
        [Command("vehboss")]
        async void cor(int source, List<object> args, string raw)
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a specialized fq2. Enjoy Cornelius!" } });
            int color1 = 53; //53 is dark green
            int color2 = 0; //0 gives it black metalic look
            var model = new Model(VehicleNameToHash["fq2"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleDoorBroken(vehicle.Handle, 0, true);
            API.SetVehicleDoorBroken(vehicle.Handle, 1, true);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleNumberPlateText(vehicle.Handle, "Boss");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 0, 100, 0);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            API.SetVehicleXenonLightsColor(vehicle.Handle, 3);
        }

        //spawns a custom f620 for finger 
        [Command("vehfinger")]
        async void finger(int source, List<object> args, string raw)
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a specialized f620. Enjoy Finger!" } });
            int color1 = 57;
            int color2 = 57; //57 = Util Green
            var model = new Model(VehicleNameToHash["f620"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleNumberPlateText(vehicle.Handle, "Finger");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 0, 100, 0);
            API.ToggleVehicleMod(vehicle.Handle, 22, true); //turns xenon on
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            API.SetVehicleXenonLightsColor(vehicle.Handle, 3);
        }

        //spawns a custom sanctus for Firewolf
        [Command("vehfw2")]
        async void firewolf(int source, List<object> args, string raw)
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You morphed into GhostRider. Enjoy Firewolf!" } });
            int color1 = 12; //12 is matte black
            int color2 = 12;
            var model = new Model(VehicleNameToHash["sanctus"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleExtraColours(vehicle.Handle, 12, 30);
            API.SetVehicleNumberPlateText(vehicle.Handle, "ghost");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 0, 0, 0);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            API.SetVehicleXenonLightsColor(vehicle.Handle, 8);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            // API.SetEntityProofs(vehicle.Handle, false, true, false, false, false, false, false, false);
            // Game.PlayerPed.IsFireProof = true;
            //Debug.WriteLine($"wait");
            //await Delay(120);
            //API.StartEntityFire(vehicle.Handle);
            //Debug.WriteLine($"fire!");
        }

        //spawns a custom vehicle for Firewolf
        [Command("vehfw")]
        async void fireWolf(int source, List<object> args, string raw)
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a vehicle. Enjoy Firewolf!" } });
            int color1 = 12; //12 is matte black
            int color2 = 12;
            var model = new Model(VehicleNameToHash["carbonizzare"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleExtraColours(vehicle.Handle, 12, 30);
            API.SetVehicleNumberPlateText(vehicle.Handle, "firewolf");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 255, 0, 0);
            //API.SetVehicleWheelType(vehicle.Handle, 4); //Monster wheels are supposed to be here. But i can't figure it out now.
            //API.ToggleVehicleMod(vehicle.Handle, 23, true);
            API.SetVehicleNumberPlateTextIndex(vehicle.Handle, 1);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            API.SetVehicleXenonLightsColor(vehicle.Handle, 8);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
        }

        //spawns a custom police cruiser for ed
        [Command("vehed")]
        async void ed(int source, List<object> args, string raw)
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a custom police cruiser. Enjoy Ed!" } });
            int color1 = 42; //53 is dark green
            int color2 = 0; //0 gives it black metalic look
            var model = new Model(VehicleNameToHash["police3"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleNumberPlateText(vehicle.Handle, "ed");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 0, 100, 0);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            API.SetVehicleHeadlightsColour(vehicle.Handle, 5);
        }

        //fix help/on/off/wait/lsc
        [Command("fix")]
        //add blips on the map.
        async void fix(int source, List<object> args, string raw)
        {
            //player is not in a vehicle.
            if (!Game.PlayerPed.IsInVehicle())
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You need to be in a vehicle to make this command work." } });
            }

            else if (args.Count == 0 & Game.PlayerPed.IsInVehicle())
            {
                //fix where vehicle repair is set to off.
                if (allowedToFixStatus == "off")
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"The command /fix is currently off." } });
                }

                //fix where vehicle repair is set to on.
                else if (allowedToFixStatus == "on")
                {
                    Game.PlayerPed.CurrentVehicle.Repair();
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You fixed your vehicle." } });
                }

                //fix where vehicle repair is set to wait.
                else if (allowedToFixStatus == "wait")
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Stand still for {timeStationairBeforeFix} seconds to get your vehicle fixed." } });
                    Vector3 playerPositionAtStart = Game.PlayerPed.Position;

                    //Timer for timeStationairBeforeFix amount of time in secs.
                    for (int i = 0; i <= timeStationairBeforeFix; i++)
                    {
                        //await Delay(100 * timeStationairBeforeFix);
                        await WaitForSeconds(1); // Wait for 1 second
                        Vector3 playerPositionCurrent = Game.PlayerPed.Position;
                        float distanceTraveled = GetDistanceBetweenCoords(playerPositionAtStart.X, playerPositionAtStart.Y, playerPositionAtStart.Z, playerPositionCurrent.X, playerPositionCurrent.Y, playerPositionCurrent.Z, true);

                        var timeRemaining = timeStationairBeforeFix - i;
                        if (timeRemaining % 5 == 0 && timeRemaining != 0 && timeRemaining != timeStationairBeforeFix)
                        {
                            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/fix in {timeRemaining} seconds." } });
                        }
                        if (distanceTraveled > 20)
                        {
                            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/fix aborted because you moved. Stand still for {timeStationairBeforeFix} seconds." } });
                            break;
                        }
                        if (!Game.PlayerPed.IsInVehicle())
                        {
                            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/fix aborted. Stay in your vehicle." } });
                            break;
                        }
                        if (i == timeStationairBeforeFix)
                        {
                            Game.PlayerPed.CurrentVehicle.Repair();
                            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Your vehicle is fixed." } });
                        }
                    }

                }
                //fix where vehicle repair is set to Los santos customs. "lsc"
                else if (allowedToFixStatus == "lsc")
                //number | lsc | it's center's xyz cords | distance from center to count
                //1 | rockford | -337.516, -136.666, 39.010 | 19
                //2 | la mesa | 732.430, -1085.277, 22.169 | 11
                //3 | lsia | -1152.147, -2008.129, 13.180 | 15
                //4 | blaine county | 1178.634, 2638.898, 37.754 | 11
                //5 | paleto | 107.893, 6624.449, 31.787 | 8
                //6 | lombank | -1538.579, -577.189, 25.313 | 8
                {
                    Vector3 playerPosition = Game.PlayerPed.Position;
                    float d1 = GetDistanceBetweenCoords(-337, -136, 39, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d2 = GetDistanceBetweenCoords(732, -1085, 22, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d3 = GetDistanceBetweenCoords(-1152, -2008, 13, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d4 = GetDistanceBetweenCoords(1178, 2638, 37, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d5 = GetDistanceBetweenCoords(107, 6624, 31, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d6 = GetDistanceBetweenCoords(-1538, -577, 25, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    Debug.WriteLine($"1: rockford {d1} | 2: la mesa {d2} | 3: lsia {d3} | 4: blaine county {d4} | 5: paleto {d5} | lombank {d6}");
                    if (d1 < 19 || d2 < 11 || d3 < 15 || d4 < 11 || d5 < 8 || d6 < 8)
                    {
                        Game.PlayerPed.CurrentVehicle.Repair();
                        TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"The mechanic repaired your vehicle." } });
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Go to a Los Santos Customs mechanic to fix your vehicle. \nThey are marked on the map with a blip. (I haven't added that to the code yet. Sorry. -Max)" } });
                    }
                }
            }
        }

        private async Task WaitForSeconds(int seconds)
        {
            int targetTime = Environment.TickCount + (seconds * 1000); 
            while (Environment.TickCount < targetTime)
            {
                await Delay(1);
            }
        }

        [Command("delveh")]
        static void deleteVehicle(int source, List<object> args, string raw)
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.CurrentVehicle.Delete();
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You deleted your vehicle." } });
            }
            else if (!Game.PlayerPed.IsInVehicle() & Game.PlayerPed.LastVehicle.Exists()) //this somehow gives an error. Invocation. Because it can't find it. It doesn't exist so should give false?
            {
                Game.PlayerPed.LastVehicle.Delete();
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You deleted your last vehicle." } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"There is no (last) vehicle found or it's alredy deleted." } });
            }
        }
    }
}
