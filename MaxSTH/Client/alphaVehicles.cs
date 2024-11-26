using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using STHMaxzzzie.Server;


namespace STHMaxzzzie.Client
{
    public class max_Vehicle : BaseScript
    {
        string allowedToFixStatus = "wait"; //can be on/off/wait/lsc.
        int timeStationairBeforeFix = 10;
        bool isVehSpawningRestricted = true;
        static Dictionary<string, string> vehicleinfoDict = new Dictionary<string, string>();
        public static Dictionary<string, VehicleHash> VehicleNameToHash = null;
        public max_Vehicle()
        {
            //make a dictionary mapping vehicle name => hash
            //https://stackoverflow.com/a/5583817
            VehicleNameToHash = new Dictionary<string, VehicleHash>();
            foreach (var veh_hash in Enum.GetValues(typeof(VehicleHash)))
            {
                VehicleNameToHash.Add(veh_hash.ToString().ToLower(), (VehicleHash)veh_hash);
                //Debug.WriteLine($"{veh_hash.ToString().ToLower()} , {(VehicleHash)veh_hash}");
            }
            TriggerServerEvent("sendVehicleinfoDict");
        }

        [EventHandler("getVehicleinfoDict")]
        void getVehicleinfoDict(string vehicleName, string vehicleInfo)
        {
            vehicleinfoDict[vehicleName] = vehicleInfo;

            //vehicle dict formatting --> Key= Tug || value= -2100640717,Boats,true || Value is vehiclehash, vehicle class, allowed to spawn or not bool.
            //use these formats to access the vehicleInfo components.
            // long vehicleHash = long.Parse(vehicleInfo.Split(',')[0]);           
            // string vehicleClass = vehicleInfo.Split(',')[1];
            // bool allowedVehicle = bool.Parse(vehicleInfo.Split(',')[2]);
            // Debug.WriteLine($"clientVehicleInfoDict = {vehicleName} {vehicleInfo} -- {vehicleHash} {vehicleClass} allowed ?{allowedVehicle}");
        }

        [EventHandler("whatIsVehAllowed")]
        void whatIsVehicleAllowed(bool vehicleSpawningRestricted)
        {
            isVehSpawningRestricted = vehicleSpawningRestricted;
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
            if (RoundHandling.gameMode != "none" && RoundHandling.thisClientIsTeam == 1)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"A game is running, vehicle spawns are disabled for runners." } });
                return;
            }
            if (args.Count == 0)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"What vehicle do you want to spawn?" } });
            }

            else if (args.Count == 1 && VehicleNameToHash.ContainsKey(args[0].ToString()) && vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                string vehicleInfo = vehicleinfoDict[args[0].ToString()];
                if (bool.Parse(vehicleInfo.Split(',')[2]) || !isVehSpawningRestricted)
                {
                    var model = new Model(VehicleNameToHash[args[0].ToString()]);
                    Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, multiline = true, args = new[] { $"You spawned an {args[0]}." } });
                }
                else { TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"This vehicle isn't allowed to be spawned." } }); }
            }
            else if (args.Count == 1 && VehicleNameToHash.ContainsKey(args[0].ToString()) && !vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Ask Max. FiveM recognises this vehicle but it doesn't have a definition for a restriction." } });
            }
            else if (args.Count == 1)
            {
                string playerName = args[0].ToString();
                if (playerName == "boss") vehBoss();
                else if (playerName == "drift") vehDrifting();
                else if (playerName == "finger") vehFinger();
                else if (playerName == "fw2") vehFirewolf2();
                else if (playerName == "fw") vehFirewolf1();
                else if (playerName == "ed") vehEd();
                else if (playerName == "gil") vehGilly();
                else if (playerName == "max") vehMax();
                //else if (playerName == "") veh();
                else TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Something went wrong!\nYou should do /veh \"vehiclename\"." } });


            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Something went wrong!\nYou should do /veh \"vehiclename\"." } });
            }
        }

        //places player in vehicle when spawning one.
        [Command("inveh")]
        async void inVehicle(int source, List<object> args, string raw)
        {
            if (RoundHandling.gameMode != "none" && RoundHandling.thisClientIsTeam == 1)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"A game is running, vehicle spawns are disabled for runners." } });
                return;
            }
            if (args.Count == 0)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"What vehicle do you want to spawn in?" } });
            }
            else if (args.Count == 1 && VehicleNameToHash.ContainsKey(args[0].ToString()) && vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                string vehicleInfo = vehicleinfoDict[args[0].ToString()];
                if (bool.Parse(vehicleInfo.Split(',')[2]) || !isVehSpawningRestricted) //checks if vehicle is true OR if vehicles aren't restricted.
                {
                    var model = new Model(VehicleNameToHash[args[0].ToString()]);
                    Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a {args[0]}." } });
                    Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                    //Game.PlayerPed.CurrentVehicle.CreateRandomPedOnSeat(VehicleSeat.Passenger);
                    API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
                }

                else { TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"This vehicle isn't allowed to be spawned." } }); }
            }
            else if (args.Count == 1 && VehicleNameToHash.ContainsKey(args[0].ToString()) && !vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Ask Max to add this to the list of secrets, he might do it." } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Something went wrong!\nYou should do /inveh \"vehiclename\"." } });
            }
        }

        [EventHandler("clientVeh")]
        void clientVeh(string vehicleName, bool isPrivate, string sourceName)
        {
            bool exists = VehicleNameToHash.ContainsKey(vehicleName);
            // Debug.WriteLine($"client veh is triggered ?{exists}");
            if (exists)
            {
                var model = new Model(VehicleNameToHash[vehicleName]);
                World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
                if (isPrivate)
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You got a vehicle called \"{vehicleName}\" from {sourceName}." } });
                if (!isPrivate)
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Everyone got a \"{vehicleName}\" from {sourceName}." } });
            }
            else { TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"A host messed up LOL." } }); }
        }


        //For vehicle colors and customisations look here. The original pastebins aren't functional anymore in the natives list.

        //https://wiki.rage.mp/index.php?title=Vehicle_Mods
        //https://wiki.rage.mp/index.php?title=Vehicle_Colors





        //spawns a custom fq2 for cornelius, 
        async void vehBoss()
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

        //spawns a custom vehicle for Drifting
        async void vehDrifting()
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a vehicle. Enjoy Drifting!" } });
            int color1 = 73; //12 is matte black
            int color2 = 73;
            var model = new Model(VehicleNameToHash["sultan"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleExtraColours(vehicle.Handle, 131, 73);
            API.SetVehicleNumberPlateText(vehicle.Handle, "drifting");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 10, 70, 255);
            API.SetVehicleNumberPlateTextIndex(vehicle.Handle, 2);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            //XENON --> Default = 255,White = 0, Blue = 1, ElectricBlue = 2, MintGreen = 3, LimeGreen = 4,Yellow = 5,GoldenShower = 6,Orange = 7,Red = 8,PonyPink = 9,HotPink = 10,Purple = 11,Blacklight = 12
            API.SetVehicleXenonLightsColor(vehicle.Handle, 2);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
        }


        //spawns a custom f620 for finger 
        async void vehFinger()
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a specialized f620. Enjoy Finger!" } });
            int color1 = 52;
            int color2 = 52; //52 = Metalic olive green
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
        async void vehFirewolf2()
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
        async void vehFirewolf1()
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
            API.SetVehicleExtraColours(vehicle.Handle, 34, 34);
        }

        //spawns a custom police cruiser for ed
        async void vehEd()
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

        //spawns a custom police cruiser for ed
        async void vehGilly()
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a custom Alpha. Enjoy Gilly!" } });
            int color1 = 127;
            int color2 = 140;
            var model = new Model(VehicleNameToHash["alpha"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleNumberPlateText(vehicle.Handle, "Gilly");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 8, 233, 250);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            SetVehicleXenonLightsCustomColor(vehicle.Handle, 8, 233, 250);
            API.SetVehicleExtraColours(vehicle.Handle, 131, 131);
        }

        //spawns a custom comet for max
        async void vehMax()
        {
            TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You spawned a custom comet. Enjoy Max!" } });
            int color1 = 41;
            int color2 = 42;
            var model = new Model(VehicleNameToHash["comet2"]);
            Vehicle vehicle = await World.CreateVehicle(model, Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0)), Game.PlayerPed.Heading);
            API.SetVehicleColours(vehicle.Handle, color1, color2);
            API.SetVehicleNumberPlateText(vehicle.Handle, "Maxzzzie");
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 0, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 1, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 2, true);
            API.SetVehicleNeonLightEnabled(vehicle.Handle, 3, true);
            API.SetVehicleNeonLightsColour(vehicle.Handle, 251, 226, 18);
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
            API.ToggleVehicleMod(vehicle.Handle, 22, true);
            API.SetVehicleHeadlightsColour(vehicle.Handle, 5);
            API.SetVehicleExtraColours(vehicle.Handle, 89, 89);

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
                else if (allowedToFixStatus == "on" && RoundHandling.thisClientIsTeam != 1)
                {
                    Game.PlayerPed.CurrentVehicle.Repair();
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You fixed your vehicle." } });
                }

                //fix where vehicle repair is set to wait.
                else if (allowedToFixStatus == "wait" && RoundHandling.thisClientIsTeam != 1)
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
                //7 | LSIA auto repair | -415.7665, -2179.21, 10.31806 | 15
                //8 | Mirror park mechanic | 1120, -779, 57 | 8
                //9 | Simeon dock garage | 1204.147, -3115.262, 5.540327 || 8 
                //10 | Benny's | -213.6762, -1327.373, 30.24028 || 8

                {
                    Vector3 playerPosition = Game.PlayerPed.Position;
                    float d1 = GetDistanceBetweenCoords(-337, -136, 39, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d2 = GetDistanceBetweenCoords(732, -1085, 22, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d3 = GetDistanceBetweenCoords(-1152, -2008, 13, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d4 = GetDistanceBetweenCoords(1178, 2638, 37, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d5 = GetDistanceBetweenCoords(107, 6624, 31, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d6 = GetDistanceBetweenCoords(-1538, -577, 25, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d7 = GetDistanceBetweenCoords(-415, -2179, 10, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d8 = GetDistanceBetweenCoords(1120, -779, 57, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d9 = GetDistanceBetweenCoords(1204, -3115, 5, playerPosition.X, playerPosition.Y, playerPosition.Z, true);
                    float d10 = GetDistanceBetweenCoords(-213, -1327, 30, playerPosition.X, playerPosition.Y, playerPosition.Z, true);

                    //Debug.WriteLine($"1: rockford {d1} | 2: la mesa {d2} | 3: lsia {d3} | 4: blaine county {d4} | 5: paleto {d5} \n lombank {d6} | LSIA auto repair {d7} | Simeon dock garage {d9}");
                    if (d1 < 19 || d2 < 11 || d3 < 15 || d4 < 11 || d5 < 8 || d6 < 8 || d7 < 15 || d8 < 8 || d9 < 8 || d10 < 8)
                    {
                        Game.PlayerPed.CurrentVehicle.Repair();
                        //API.PlaySoundFrontend(-1, "MECHANIC_TOOL_RATTLE", "DEFAULT", false);
                        TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"The mechanic repaired your vehicle." } });
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Go to a Los Santos Customs mechanic to fix your vehicle. \nThey are marked on the map with a blip." } });
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


    public class VehiclePersistenceClient : BaseScript
    {
        bool vehicleShouldChangePlayerColour = true;
        bool vehicleShouldNotDespawn = true;
        private int? primaryColour = null;
        private int? secondaryColour = null;
        private int pearlescentColour = -1;
        private Vector3 rgbLightsColour; 
        public static Vehicle lastVehicle = null;

        [EventHandler("updateClientColourAndDespawn")]
        private void updateClientColourAndDespawn(bool setVehicleShouldChangePlayerColour, bool setVehicleShouldNotDespawn)
        {
            vehicleShouldChangePlayerColour = setVehicleShouldChangePlayerColour;
            vehicleShouldNotDespawn = setVehicleShouldNotDespawn;
            lastVehicle = null;
        }

        private async Task WaitFor100Milliseconds(int seconds)
        {
            int targetTime = Environment.TickCount + (seconds * 100);
            while (Environment.TickCount < targetTime)
            {
                await Delay(1);
            }
        }

        // Usage in the constructor:
        public VehiclePersistenceClient()
        {
            Tick += async () =>
            {
                await WaitFor100Milliseconds(1);
                await CheckVehicleEntry();
            };
        }

        // Main loop to check if the player has entered a new vehicle
        private async Task CheckVehicleEntry()
        {
            // Check if the player is in a vehicle
            if (Game.PlayerPed.IsInVehicle())
            {
                Vehicle currentVehicle = Game.PlayerPed.CurrentVehicle;

                // Check if this is a new vehicle (not the last one the player entered) and player is in the driver seat
                if (currentVehicle != lastVehicle && Game.PlayerPed.SeatIndex == VehicleSeat.Driver)
                {
                    // Set the new vehicle as the current vehicle
                    lastVehicle = currentVehicle;
                    // Debug.WriteLine("Player entered a new vehicle.");

                    // Handle mission entity and color only if conditions are met
                    await HandleMissionEntity(currentVehicle);
                    await HandleVehicleColor(currentVehicle);
                }
            }
            else
            {
                // Reset lastVehicle if the player is not in any vehicle
                lastVehicle = null;
            }
        }

        // Ensure vehicle does not despawn by making it a mission entity if required
        private async Task HandleMissionEntity(Vehicle vehicle)
        {
            // Only set as mission entity if vehicleShouldNotDespawn is true
            if (vehicleShouldNotDespawn && !API.IsEntityAMissionEntity(vehicle.Handle))
            {
                // Debug.WriteLine("Setting vehicle as mission entity to prevent despawn.");
                API.SetEntityAsMissionEntity(vehicle.Handle, true, false);
            }

            await Task.FromResult(0);
        }

        // Apply or request vehicle color based on conditions
        private async Task HandleVehicleColor(Vehicle vehicle)
        {
            // Apply color if needed, regardless of mission entity status and checks if player isn't a runner.
            if (vehicleShouldChangePlayerColour && RoundHandling.thisClientIsTeam != 1)
            {
                if (!primaryColour.HasValue || !secondaryColour.HasValue || pearlescentColour == -1 || rgbLightsColour == null)
                {
                    // Debug.WriteLine("Requesting vehicle color from the server.");
                    TriggerServerEvent("requestVehicleColor");
                }
                else
                {
                   ApplyVehicleColor(vehicle.Handle);
                }
            }

            await Task.FromResult(0);
        }

        // Called when the client receives color data from the server
        [EventHandler("receiveVehicleColor")]
        private void receiveVehicleColor(int primary, int secondary, int pearlescent, int lightR, int lightG, int lightB)
        {
            // Debug.WriteLine("Received vehicle color from server."); // Debug message for receiving data

            // Store color values locally (only needs to happen once)
            primaryColour = primary;
            secondaryColour = secondary;
            pearlescentColour = pearlescent;
            rgbLightsColour = new Vector3(lightR, lightG, lightB);

            // Apply color to the vehicle if the player is currently in one and in the driver seat
            if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.SeatIndex == VehicleSeat.Driver && vehicleShouldChangePlayerColour)
            {
                ApplyVehicleColor(Game.PlayerPed.CurrentVehicle.Handle);
            }
        }

        // Applies the stored colors to a vehicle
        private void ApplyVehicleColor(int vehicleHandle)
        {
            if (primaryColour.HasValue && secondaryColour.HasValue && RoundHandling.thisClientIsTeam != 1 && !StreamLootsEffects.isStarmodeOn)
            {
                // Debug.WriteLine($"Setting vehicle colors to Primary: {primaryColor.Value}, Secondary: {secondaryColor.Value}");
                    API.SetVehicleColours(vehicleHandle, primaryColour.Value, secondaryColour.Value);
                    API.SetVehicleExtraColours(vehicleHandle, pearlescentColour, pearlescentColour);
                    API.SetVehicleNeonLightEnabled(vehicleHandle, 0, true);
                    API.SetVehicleNeonLightEnabled(vehicleHandle, 1, true);
                    API.SetVehicleNeonLightEnabled(vehicleHandle, 2, true);
                    API.SetVehicleNeonLightEnabled(vehicleHandle, 3, true);
                    API.SetVehicleNeonLightsColour(vehicleHandle, (int)rgbLightsColour.X, (int)rgbLightsColour.Y, (int)rgbLightsColour.Z);
                    API.ToggleVehicleMod(vehicleHandle, 22, true); //turns on neons
                    API.SetVehicleXenonLightsCustomColor(vehicleHandle, (int)rgbLightsColour.X, (int)rgbLightsColour.Y, (int)rgbLightsColour.Z);
            }
        }
    }
}
