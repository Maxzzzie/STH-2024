using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using STHMaxzzzie.Client;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class VehicleHandler : BaseScript
    {
        bool isSpawningRunning = false;
        static Random rand = new Random();
        string allowedToFixStatus = "wait"; //can be on/off/wait/lsc.
        int timeStationairBeforeFix = 10;
        static bool isVehSpawningRestricted = true;
        public static Dictionary<string, string> vehicleinfoDict = new Dictionary<string, string>();
        bool CarChangesColourInLscForRunner = true;

        // Mapping vehicle class names to their corresponding dictionaries
        //static Dictionary<string, Dictionary<string, bool>> vehicleClasses = new Dictionary<string, Dictionary<string, bool>>();

        Dictionary<string, string> alternateNamesForClasses = new Dictionary<string, string>
        { {"boat", "boats"}, {"comercial", "commercial"}, {"truck", "commercial"},{"trucks", "commercial"},
        {"compact", "compacts"}, {"coupe", "coupes"}, {"cycle", "cycles"},
        {"bicycle", "cycles"}, {"helicopter", "helicopters"}, {"heli", "helicopters"},
        {"militairy", "military"}, {"motorcycle", "motorcycles"}, {"motor", "motorcycles"}, {"bike", "motorcycles"},
        {"mc", "motorcycles"}, {"muscles", "muscle"}, {"off road", "off-road"}, {"offroad", "off-road"},
        {"open wheel", "open-wheel"}, {"openwheel", "open-wheel"}, {"plane", "planes"}, {"sedan", "sedans"},
        {"services", "service"}, {"sport", "sports"}, {"sportsclassic", "sportsclassics"},
        {"sportclassics", "sportsclassics"},{"sc", "sportsclassics"}, {"emergancy", "emergency"},
        {"sportclassic", "sportsclassics"}, {"supers", "super"}, {"suv", "suvs"}, {"suv's", "suvs"},
        {"utilitys", "utility"}, {"van", "vans"}, {"trailers", "trailer"}}; //{"train", "trains"}, 

        // Declare the dictionaries but do not assign them yet
        public static Dictionary<string, bool> boatsInfo;
        public static Dictionary<string, bool> commercialInfo;
        public static Dictionary<string, bool> compactsInfo;
        public static Dictionary<string, bool> coupesInfo;
        public static Dictionary<string, bool> cyclesInfo;
        public static Dictionary<string, bool> emergencyInfo;
        public static Dictionary<string, bool> helicoptersInfo;
        public static Dictionary<string, bool> industrialInfo;
        public static Dictionary<string, bool> militaryInfo;
        public static Dictionary<string, bool> motorcyclesInfo;
        public static Dictionary<string, bool> muscleInfo;
        public static Dictionary<string, bool> offroadInfo;
        public static Dictionary<string, bool> openwheelInfo;
        public static Dictionary<string, bool> planesInfo;
        public static Dictionary<string, bool> sedansInfo;
        public static Dictionary<string, bool> serviceInfo;
        public static Dictionary<string, bool> sportsInfo;
        public static Dictionary<string, bool> sportsclassicsInfo;
        public static Dictionary<string, bool> superInfo;
        public static Dictionary<string, bool> suvInfo;
        public static Dictionary<string, bool> utilityInfo;
        public static Dictionary<string, bool> vansInfo;
        public static Dictionary<string, bool> trialerInfo;
        public static Dictionary<string, bool> trainsInfo;

        // Declare the vehicleClasses dictionary
        static Dictionary<string, Dictionary<string, bool>> vehicleClasses;

        public VehicleHandler()
        {
            PopulateVehicleClasses();
        }

        static void PopulateVehicleClasses()
        {
            boatsInfo = new Dictionary<string, bool>();
            commercialInfo = new Dictionary<string, bool>();
            compactsInfo = new Dictionary<string, bool>();
            coupesInfo = new Dictionary<string, bool>();
            cyclesInfo = new Dictionary<string, bool>();
            emergencyInfo = new Dictionary<string, bool>();
            helicoptersInfo = new Dictionary<string, bool>();
            industrialInfo = new Dictionary<string, bool>();
            militaryInfo = new Dictionary<string, bool>();
            motorcyclesInfo = new Dictionary<string, bool>();
            muscleInfo = new Dictionary<string, bool>();
            offroadInfo = new Dictionary<string, bool>();
            openwheelInfo = new Dictionary<string, bool>();
            planesInfo = new Dictionary<string, bool>();
            sedansInfo = new Dictionary<string, bool>();
            serviceInfo = new Dictionary<string, bool>();
            sportsInfo = new Dictionary<string, bool>();
            sportsclassicsInfo = new Dictionary<string, bool>();
            superInfo = new Dictionary<string, bool>();
            suvInfo = new Dictionary<string, bool>();
            utilityInfo = new Dictionary<string, bool>();
            vansInfo = new Dictionary<string, bool>();
            trialerInfo = new Dictionary<string, bool>();
            trainsInfo = new Dictionary<string, bool>();

            vehicleClasses = new Dictionary<string, Dictionary<string, bool>>()
        {
            { "boats", boatsInfo },
            { "commercial", commercialInfo },
            { "compacts", compactsInfo },
            { "coupes", coupesInfo },
            { "cycles", cyclesInfo },
            { "emergency", emergencyInfo },
            { "helicopters", helicoptersInfo },
            { "industrial", industrialInfo },
            { "military", militaryInfo },
            { "motorcycles", motorcyclesInfo },
            { "muscle", muscleInfo },
            { "off-road", offroadInfo },
            { "open-wheel", openwheelInfo },
            { "planes", planesInfo },
            { "sedans", sedansInfo },
            { "service", serviceInfo },
            { "sports", sportsInfo },
            { "sportsclassics", sportsclassicsInfo },
            { "super", superInfo },
            { "suvs", suvInfo },
            { "utility", utilityInfo },
            { "vans", vansInfo },
            { "trailer", trialerInfo },
        { "trains" , trainsInfo }
        };
            TriggerServerEvent("sendVehicleinfoDict");

        }

        [EventHandler("PopulateInfoDicts")]
        static async void PopulateInfoDicts()
        {
            // Wait until vehicleinfoDict is populated
            while (vehicleinfoDict != null && vehicleinfoDict.Count == 0 && vehicleClasses.Count != 24)
            {
                await Delay(200);
            }

            // Iterate through the main dictionary and populate corresponding class dictionaries
            foreach (var kvp in vehicleinfoDict)
            {
                string[] value = kvp.Value.Split(',');
                if (value.Length != 4)
                {
                    Debug.WriteLine($"Issues in PopulateInfoDicts line: {kvp.Value}");
                    continue;
                }
                string vehClass = value[2];
                if (!bool.TryParse(value[3], out bool allowedBool)) { allowedBool = false; }

                // Check if the class exists in the map and add the vehicle to the appropriate dictionary
                if (vehicleClasses.TryGetValue(vehClass, out var specificDict))
                {
                    if (specificDict == null)  // This check prevents the error
                    {
                        Debug.WriteLine($"Error: specificDict for class '{vehClass}' is null.");
                    }
                    else
                    {
                        specificDict[kvp.Key] = allowedBool;
                        //Debug.WriteLine($"{kvp.Key} | {allowedBool} | {vehClass} | {specificDict} | {kvp.Value}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Error: Vehicle class '{vehClass}' not found in vehicleClasses dictionary.");
                }

            }
        }

        [EventHandler("getVehicleinfoDict")]
        void getVehicleinfoDict(List<object> vehicleInfo)
        {
            Debug.WriteLine("starting getVehicleInfoDict");
            foreach (string line in vehicleInfo)
            {
                string[] splitLine = line.Split(',');
                vehicleinfoDict[splitLine[0]] = line;
            }
            Debug.WriteLine("finished getVehicleInfoDict");
            PopulateInfoDicts();
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
        void vehicle(int source, List<object> args, string raw)
        {
            if (isSpawningRunning) return;
            isSpawningRunning = true;
            if (args.Count == 1 && !DoesVehicleNameExist(args[0].ToString()) && !vehicleClasses.ContainsKey(args[0].ToString()) && !alternateNamesForClasses.ContainsKey(args[0].ToString()))
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
                else NotificationScript.ShowErrorNotification($"The vehicle or vehicle class \"{args[0].ToString()}\" does not exist in FiveM.");
                isSpawningRunning = false;
                return;
            }
            if (RoundHandling.gameMode != "none" && RoundHandling.thisClientIsTeam == 1)
            {
                NotificationScript.ShowErrorNotification("Vehicle spawns are disabled for runners.");
                isSpawningRunning = false;
                return;
            }

            //gives randomized vehicle if a client adds nothing to the command.
            if (args.Count == 0)
            {
                string vehicleName = GetRandomVehicle(true);
                if (vehicleName != "null") GivePlayerNewVehicle(vehicleName);
                else NotificationScript.ShowErrorNotification($"Something went wrong.");
            }

            //gives specific vehicle the client called for
            else if (args.Count == 1 && vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                string[] vehicleInfo = vehicleinfoDict[args[0].ToString()].Split(',');

                // Check if the restriction is properly defined
                if (!bool.TryParse(vehicleInfo[3], out bool isSpawningThisAllowed))
                {
                    // Notify that the vehicle restriction is undefined and return
                    TriggerServerEvent("notifyEveryone", $"~r~{Game.Player.Name} ~s~- Max, {args[0].ToString()} doesn't have a valid definition for a vehicle restriction.");
                    isSpawningRunning = false;
                    return;
                }

                // Check if vehicle spawning is unrestricted or the vehicle is not restricted
                if (!isVehSpawningRestricted || isSpawningThisAllowed)
                {
                    GivePlayerNewVehicle(vehicleInfo[0]);
                }
                else
                {
                    NotificationScript.ShowErrorNotification($"Tried to spawn {vehicleInfo[0]}. But it is restricted right now.");
                }
            }


            //gives randomized vehicle of a class that is requested selected.
            else if ((args.Count == 1 && vehicleClasses.ContainsKey(args[0].ToString())) || (args.Count == 1 && alternateNamesForClasses.ContainsKey(args[0].ToString())))
            {
                string vehicleClass;
                vehicleClass = args[0].ToString();

                //if the client typed heli, this dict might have the actual class stored. heli becomes helicopters. supers becomes super etc.
                if (alternateNamesForClasses.ContainsKey(vehicleClass)) vehicleClass = alternateNamesForClasses[vehicleClass];

                if (vehicleClass == "super" && isVehSpawningRestricted)
                {
                    NotificationScript.ShowNotification($"You are not allowed to spawn a supercar right now.");
                    isSpawningRunning = false;
                    return;
                }

                string vehicleName = GetRandomVehicleFromClass(vehicleClass);
                if (vehicleName != "null") GivePlayerNewVehicle(vehicleName);

                else NotificationScript.ShowErrorNotification($"No vehicles unrestricted in the {vehicleClass} class.");
            }

            //vehicles that do exist but not in the AllowedVehicles.txt file should end up here.
            else if (args.Count == 1 && !vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                TriggerServerEvent("notifyEveryone", $"~r~{Game.Player.Name} ~s~- Max, {args[0].ToString()} exists in FiveM but not in the AllowedVehicles resource.");
            }

            else NotificationScript.ShowErrorNotification($"Something went wrong!\nYou should do /inveh (\"vehiclename\" or \"class\").");

            isSpawningRunning = false;
        }

        //spawning a vehicle and puts player in it. 
        [Command("inveh")]
        void inVehicle(int source, List<object> args, string raw)
        {
            if (isSpawningRunning) return;
            isSpawningRunning = true;
            if (args.Count == 1 && !DoesVehicleNameExist(args[0].ToString()) && !vehicleClasses.ContainsKey(args[0].ToString()) && !alternateNamesForClasses.ContainsKey(args[0].ToString()))
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
                else NotificationScript.ShowErrorNotification($"The vehicle or vehicle class \"{args[0].ToString()}\" does not exist in FiveM.");
                isSpawningRunning = false;
                return;
            }
            if (RoundHandling.gameMode != "none" && RoundHandling.thisClientIsTeam == 1)
            {
                NotificationScript.ShowErrorNotification("Vehicle spawns are disabled for runners.");
                isSpawningRunning = false;
                return;
            }

            //gives randomized vehicle if a client adds nothing to the command.
            if (args.Count == 0)
            {
                string vehicleName = GetRandomVehicle(true);
                if (vehicleName != "null") SetPlayerIntoNewVehicle(vehicleName);
                else NotificationScript.ShowErrorNotification($"Something went wrong.");
            }

            //gives specific vehicle the client called for
            else if (args.Count == 1 && vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                string[] vehicleInfo = vehicleinfoDict[args[0].ToString()].Split(',');

                // Check if the restriction is properly defined
                if (!bool.TryParse(vehicleInfo[3], out bool isSpawningThisAllowed))
                {
                    // Notify that the vehicle restriction is undefined and return
                    TriggerServerEvent("notifyEveryone", $"~r~{Game.Player.Name} ~s~- Max, {args[0].ToString()} doesn't have a valid definition for a vehicle restriction.");
                    isSpawningRunning = false;
                    return;
                }

                // Check if vehicle spawning is unrestricted or the vehicle is not restricted
                if (!isVehSpawningRestricted || isSpawningThisAllowed)
                {
                    uint vehicleHash = (uint)Game.GenerateHash(vehicleInfo[0]);
                    if (!API.IsModelInCdimage(vehicleHash) || !API.IsModelAVehicle(vehicleHash))
                    {
                        NotificationScript.ShowErrorNotification($"[ERROR] {vehicleInfo[0]} is not a valid vehicle model.");
                        return;
                    }

                    SetPlayerIntoNewVehicle(vehicleInfo[0]);
                }
                else
                {
                    NotificationScript.ShowErrorNotification($"Tried to spawn {vehicleInfo[0]}. But it is restricted right now.");
                }
            }

            //gives randomized vehicle of a class that is requested selected.
            else if ((args.Count == 1 && vehicleClasses.ContainsKey(args[0].ToString())) || (args.Count == 1 && alternateNamesForClasses.ContainsKey(args[0].ToString())))
            {

                string vehicleClass;
                vehicleClass = args[0].ToString();

                //if the client typed heli, this dict might have the actual class stored. heli becomes helicopters. supers becomes super etc.
                if (alternateNamesForClasses.ContainsKey(vehicleClass)) vehicleClass = alternateNamesForClasses[vehicleClass];

                if (vehicleClass == "super" && isVehSpawningRestricted && !GameRace.isGfredRunning)
                {
                    NotificationScript.ShowNotification($"You are not allowed to spawn a supercar right now.");
                    isSpawningRunning = false;
                    return;
                }

                string vehicleName = GetRandomVehicleFromClass(vehicleClass);
                if (vehicleName != "null") SetPlayerIntoNewVehicle(vehicleName);

                else NotificationScript.ShowErrorNotification($"No vehicles unrestricted in the {vehicleClass} class.");
            }

            //vehicles that do exist but not in the AllowedVehicles.txt file should end up here.
            else if (args.Count == 1 && !vehicleinfoDict.ContainsKey(args[0].ToString()))
            {
                TriggerServerEvent("notifyEveryone", $"~r~{Game.Player.Name} ~s~- Max, {args[0].ToString()} exists in FiveM but not in the AllowedVehicles resource.");
            }

            else NotificationScript.ShowErrorNotification($"Something went wrong!\nYou should do /inveh (\"vehiclename\" or \"class\").");

            isSpawningRunning = false;
        }

        public static string GetRandomVehicle(bool excludeAircraft)
        {
            int maxAttempts = 25; // Limit attempts to prevent infinite loops
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                attempts++;

                // Get a random vehicle from the dictionary
                var randomVehicle = vehicleinfoDict.ElementAt(rand.Next(0, vehicleinfoDict.Count));
                string vehicleName = randomVehicle.Key;
                string[] vehicleInfo = randomVehicle.Value.Split(',');

                // Exclude unwanted vehicle types
                if (vehicleInfo[2] == "trains" || vehicleInfo[2] == "trailer" || vehicleInfo[2] == "boats" || excludeAircraft && (vehicleInfo[2] == "helicopters" || vehicleInfo[2] == "planes"))
                {
                    Debug.WriteLine($"Random vehicle: {vehicleName} is a {vehicleInfo[2]} and we don't want those.");
                    continue;
                }
                if (vehicleInfo[2] == "super" && isVehSpawningRestricted && !GameRace.isGfredRunning)
                {
                    Debug.WriteLine($"Random vehicle: {vehicleName} is a {vehicleInfo[2]} and the restriction is on.");
                    continue;
                }
                if (!DoesVehicleNameExist(vehicleName)) {TriggerServerEvent("notifyEveryone", $"~r~Max, {vehicleName} doesn't exist in FiveM but it does in the restriction file."); continue;}
                

                // Check restrictions
                bool isNotRestricted = vehicleInfo.Length == 4 && bool.TryParse(vehicleInfo[3], out bool restrictionBool) && restrictionBool;
                if (!isVehSpawningRestricted || isNotRestricted)
                {
                    Debug.WriteLine($"Random vehicle: {vehicleName} is allowed.");
                    return vehicleName; // Found an allowed vehicle
                }

                Debug.WriteLine($"Random vehicle: {vehicleName} is not allowed.");
            }

            // If no valid vehicle is found within the attempts
            Debug.WriteLine($"[GetRandomVehicle] returning null.");
            return "null";
        }





        //public async static Task<string> GetRandomVehicleFromClass(string vehClass)
        public static string GetRandomVehicleFromClass(string vehClass)
        {
            string chosenVehicleName = "null";
            bool foundGoodVehicle = false;
            while (!foundGoodVehicle)
            {
                //Check if the vehicle class exists in the mapping
                if (vehicleClasses.TryGetValue(vehClass, out Dictionary<string, bool> randomClassVehicleInfo))
                {
                    // Filter the dictionary for keys with value `true` if restrictions apply
                    var validVehicles = isVehSpawningRestricted
                        ? randomClassVehicleInfo.Where(v => v.Value).Select(v => v.Key).ToList()
                        : randomClassVehicleInfo.Keys.ToList();

                    // Choose a random vehicle if any are available
                    if (validVehicles.Count > 0)
                    {
                        chosenVehicleName = validVehicles[rand.Next(0, validVehicles.Count)];
                    }
                }
                Debug.WriteLine($"Trying {chosenVehicleName}");
                //foundGoodVehicle = await DoesVehicleNameExist(chosenVehicleName);
                foundGoodVehicle = DoesVehicleNameExist(chosenVehicleName);
            }
            Debug.WriteLine($"[GetRandomVehicleFromClass] Class = {vehClass}, Name = {chosenVehicleName}.");
            return chosenVehicleName;
        }




        static async void GivePlayerNewVehicle(string VehicleName)
        {
            if (!DoesVehicleNameExist(VehicleName)) {Debug.WriteLine("VehicleName VehicleName doesn't exist"); return;}
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash(VehicleName)), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
            API.SetVehicleNumberPlateText(vehicle.Handle, Game.Player.Name);
            NotificationScript.ShowNotification($"A {VehicleName} spawned.");
        }




        // public static async void SetPlayerIntoNewVehicle(string VehicleName)
        // {
        //     Vehicle veh = Game.PlayerPed.CurrentVehicle;
        //     Vector3 playerSpeed = Game.PlayerPed.Velocity;
        //     float speed = playerSpeed.Length();
        //     Vector3 currentPosition = Game.PlayerPed.Position;
        //     if (!DoesVehicleNameExist(VehicleName)) {Debug.WriteLine("VehicleName VehicleName doesn't exist"); return;}

        //     // If the player is in a vehicle, update current position and delete the vehicle if they are the driver
        //     if (veh != null)
        //     {
        //         currentPosition = veh.Position;

        //         // Adjust position if the player isn't in the driver seat
        //         if (Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
        //         {
        //             currentPosition.Z += 7; // Add offset to X-coordinate
        //         }

        //         // Delete only if the player is the driver
        //         if (Game.PlayerPed.SeatIndex == VehicleSeat.Driver)
        //         {

        //             veh.Delete();
        //         }
        //     }


        //     // Generate vehicle model
        //     Debug.WriteLine($"[setPlayerIntoVehicle] generating hash for {VehicleName}");
        //     int vehicleHash = Game.GenerateHash(VehicleName);
        //     Debug.WriteLine($"[setPlayerIntoVehicle] generated hash: for {VehicleName} - {vehicleHash}");
        //     Model model = new Model(vehicleHash);
        //     //if (model == null){Debug.WriteLine($"[setPlayerIntoVehicle] model doesn't exist."); return;}

        //     string vehicleInfo = vehicleinfoDict[VehicleName];
        //     string[] vehicleInfoSplit = vehicleInfo.Split(',');

        //     // Adjust Z-coordinate for helicopters and planes
        //     if (vehicleInfoSplit[2] == "helicopters")//&& RoundHandling.gameMode != "copyclass")
        //     {
        //         currentPosition.Z += 50; // Spawn helicopters higher
        //     }
        //     else if (vehicleInfoSplit[2] == "planes")//&& RoundHandling.gameMode != "copyclass")
        //     {
        //         speed = 40; // Boost speed for planes
        //         currentPosition.Z += 150; // Spawn planes even higher
        //     }

        //     Vehicle newVeh = veh; //original veh is deleted. But that doesn't matter as i cannot have it empty here. And it gets changed anyway before using it.
        //     // Create the new vehicle
        //     bool canContinue = false;
        //     while (!canContinue)
        //     {
        //         Vehicle tryNewVeh = await World.CreateVehicle(model, currentPosition, Game.PlayerPed.Heading);
        //         if (tryNewVeh == null)
        //         {
        //             Debug.WriteLine($"[setPlayerIntoVehicle] vehicle doesn't exist.");
        //             await Delay(1);
        //         }
        //         else
        //         {
        //             Debug.WriteLine($"Vehicle spawned, continuing.");
        //             canContinue = true;
        //             newVeh = tryNewVeh;
        //         }
        //     }
        //     canContinue = false;
        //     int time = 1;
        //     while (!canContinue)
        //     {
        //         // Warp player into the new vehicle
        //         Game.PlayerPed.Task.ClearAllImmediately();
        //         Game.PlayerPed.Task.WarpIntoVehicle(newVeh, VehicleSeat.Driver);
        //         await Delay(time);
        //         if (Game.PlayerPed.IsInVehicle())
        //         {
        //             canContinue = true;
        //             Debug.WriteLine($"[setPlayerIntoVehicle] Warp into vehicle succeeded, continuing.{time}");
        //         }
        //         else
        //         {
        //             Debug.WriteLine($"[setPlayerIntoVehicle] Warp into vehicle failed, retrying. {time}");
        //             await Delay(1);
        //         }
        //         time ++;
        //     }

        //     // Set vehicle properties
        //     API.SetVehicleNumberPlateText(newVeh.Handle, Game.Player.Name);
        //     API.SetVehicleEngineOn(newVeh.Handle, true, true, false);
        //     bool hasKers = GetVehicleHasKers(newVeh.Handle);
        //     SetVehicleKersAllowed(newVeh.Handle, true);

        //     // Add a small delay for vehicle initialization
        //     await Delay(1);

        //     // Set vehicle speed and RPM
        //     if (!hasKers) SetVehicleKersAllowed(newVeh.Handle, false);
        //     API.SetVehicleCurrentRpm(newVeh.Handle, 1.0f);
        //     API.SetVehicleForwardSpeed(newVeh.Handle, speed);
        //     NotificationScript.ShowNotification($"You spawned in a \"{VehicleName}\"");
        // }

public static async void SetPlayerIntoNewVehicle(string VehicleName)
{
    Vehicle veh = Game.PlayerPed.CurrentVehicle;
    Vector3 playerSpeed = Game.PlayerPed.Velocity;
    float speed = playerSpeed.Length();
    Vector3 currentPosition = Game.PlayerPed.Position;
    Vector3 rotation = Game.PlayerPed.Rotation;
    bool wasPlayerFrozen = IsEntityPositionFrozen(Game.PlayerPed.Handle);

    if (!DoesVehicleNameExist(VehicleName))
    {
        Debug.WriteLine("VehicleName doesn't exist");
        return;
    }

    // Freeze player to prevent falling issues
    API.FreezeEntityPosition(Game.PlayerPed.Handle, true);
    API.SetEntityCollision(Game.PlayerPed.Handle, false, true);
    Game.PlayerPed.IsVisible = false; // Make player invisible

    // If in a vehicle, update position & delete if driver
    if (veh != null)
    {
        currentPosition = veh.Position;
        rotation = veh.Rotation;

        if (Game.PlayerPed.SeatIndex == VehicleSeat.Driver)
        {
            veh.Delete();
        }
        else
        {
            currentPosition.Z += 7;
        }
    }

    // Generate vehicle model
    Debug.WriteLine($"[setPlayerIntoVehicle] generating hash for {VehicleName}");
    int vehicleHash = Game.GenerateHash(VehicleName);
    Model model = new Model(vehicleHash);

    string vehicleInfo = vehicleinfoDict[VehicleName];
    string[] vehicleInfoSplit = vehicleInfo.Split(',');

    if (vehicleInfoSplit[2] == "helicopters")
    {
        currentPosition.Z += 50;
    }
    else if (vehicleInfoSplit[2] == "planes")
    {
        speed = 40;
        currentPosition.Z += 150;
    }

    // Create the new vehicle
    Vehicle newVeh = null;
    bool canContinue = false;

    while (!canContinue)
    {
        newVeh = await World.CreateVehicle(model, currentPosition, Game.PlayerPed.Heading);
        if (newVeh != null)
        {
            Debug.WriteLine($"Vehicle spawned, continuing.");
            canContinue = true;
        }
        else
        {
            Debug.WriteLine($"[setPlayerIntoVehicle] vehicle doesn't exist.");
            await Delay(1);
        }
    }

    // Apply saved velocity & rotation
    newVeh.Velocity = playerSpeed;
    newVeh.Rotation = rotation;

    // Attempt to warp player into vehicle
    canContinue = false;
    int time = 1;

    while (!canContinue)
    {
        Game.PlayerPed.Task.ClearAllImmediately();
        Game.PlayerPed.Task.WarpIntoVehicle(newVeh, VehicleSeat.Driver);
        await Delay(time);

        if (Game.PlayerPed.IsInVehicle())
        {
            canContinue = true;
            Debug.WriteLine($"[setPlayerIntoVehicle] Warp into vehicle succeeded, continuing. {time}");
        }
        else
        {
            Debug.WriteLine($"[setPlayerIntoVehicle] Warp into vehicle failed, retrying. {time}");
            await Delay(1);
        }
        time++;
    }
    // Restore player collision & visibility
    if (!wasPlayerFrozen) API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
    API.SetEntityCollision(Game.PlayerPed.Handle, true, true);
    Game.PlayerPed.IsVisible = true;

    // Set vehicle properties
    API.SetVehicleNumberPlateText(newVeh.Handle, Game.Player.Name);
    API.SetVehicleEngineOn(newVeh.Handle, true, true, false);
    bool hasKers = GetVehicleHasKers(newVeh.Handle);
    SetVehicleKersAllowed(newVeh.Handle, true);

    await Delay(1);

    if (!hasKers) SetVehicleKersAllowed(newVeh.Handle, false);
    API.SetVehicleCurrentRpm(newVeh.Handle, 1.0f);
    API.SetVehicleForwardSpeed(newVeh.Handle, speed);


    NotificationScript.ShowSpecialNotification($"You spawned in a \"{VehicleName}\"", "Frontend_Beast_Fade_Screen","FM_Events_Sasquatch_Sounds");
}


        [EventHandler("clientVeh")]
        void clientVeh(string vehicleName, bool isPrivate, string sourceName)
        {
            if (DoesVehicleNameExist(vehicleName))
            {
                GivePlayerNewVehicle(vehicleName);
                if (isPrivate)
                    NotificationScript.ShowNotification($"You got a vehicle called \"{vehicleName}\" from {sourceName}.");
                if (!isPrivate)
                    NotificationScript.ShowNotification($"Everyone got a \"{vehicleName}\" from {sourceName}.");
            }
            else { NotificationScript.ShowErrorNotification($"A host messed up LOL.\n{sourceName} tried spawning a {vehicleName}"); }
        }




        //async static Task<bool> DoesVehicleNameExist(string vehName)
        static bool DoesVehicleNameExist(string vehName)
        {
            bool exists;
            exists = IsModelInCdimage((uint)Game.GenerateHash(vehName));
            return exists;
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("fq2")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("sultan")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("f620")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("sanctus")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("carbonizzare")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("police3")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("alpha")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
            Vehicle vehicle = await World.CreateVehicle(new Model(Game.GenerateHash("comet2")), Game.PlayerPed.GetOffsetPosition(new Vector3(0, 7, 0)), Game.PlayerPed.Heading);
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
                return;
            }
            if (Game.PlayerPed.IsInVehicle() && RoundHandling.thisClientIsTeam == 1 && allowedToFixStatus != "lsc")
            {
                NotificationScript.ShowErrorNotification($"Cannot currently fix as a runner.");
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
                        if (RoundHandling.thisClientIsTeam == 1 && CarChangesColourInLscForRunner)
                        {
                            Vehicle veh = Game.PlayerPed.CurrentVehicle;
                            if (veh != null)
                            {
                                int[] colorIndices = { 0, 1, 2, 3, 4, 8, 9, 11, 27, 34, 49, 50, 62, 63, 74, 94, 95, 96, 97, 103, 147, 120 };
                                int randomColor = colorIndices[rand.Next(colorIndices.Length)];
                                //int randomColor = rand.Next(160);
                                veh.Mods.PrimaryColor = (VehicleColor)randomColor;
                                veh.Mods.SecondaryColor = (VehicleColor)randomColor;
                                //add particle effects at some point?
                            }
                        }
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

        [EventHandler("delveh")]
        static void deleteVehicle()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.CurrentVehicle.Delete();
                NotificationScript.ShowNotification($"You deleted your vehicle.");
            }
            else if (!Game.PlayerPed.IsInVehicle() && Game.PlayerPed.LastVehicle != null && Game.PlayerPed.LastVehicle.Exists())
            {
                Game.PlayerPed.LastVehicle.Delete();
                //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You deleted your last vehicle." } });
                NotificationScript.ShowNotification($"You deleted your last vehicle.");
            }
            else
            {
                //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"There is no (last) vehicle found or it's alredy deleted." } });
                NotificationScript.ShowNotification($"There is no (last) vehicle found or it's alredy deleted.");
            }
        }

        //has to be a little complicated as if we send it directly to the client requesting it it will only delete the vehicle he's loading in. 
        //now we get the clients position first. And trigger an event to the server that distributes that position to all clients.
        [EventHandler("sendClearNearVehiclesInfo")]
        void sendClearNearVehiclesInfo(int range)
        {
            TriggerServerEvent("clearNearVehicles", new Vector4(Game.PlayerPed.Position, range));
        }

        [EventHandler("clearNearVehicles")]
        void RemoveNearVehicles(Vector4 clearInfo)
        {
            Vehicle[] allVeh = World.GetAllVehicles();
            foreach (Vehicle veh in allVeh)
            {
                Vector3 vehpos = veh.Position;
                if ((IsVehicleSeatFree(veh.Handle, -1) || veh.IsDead || !veh.IsDriveable) && GetDistanceBetweenCoords(clearInfo.X, clearInfo.Y, clearInfo.Z, vehpos.X, vehpos.Y, vehpos.Z, true) < clearInfo.W)
                {
                    veh.Delete();
                }
            }
            TriggerServerEvent("didClearJustHappen");
        }

        [EventHandler("clear_vehicles")]
        void RemoveAllVehicles(bool shouldRemoveProps)
        {
            Vehicle[] allVeh = World.GetAllVehicles();
            foreach (Vehicle veh in allVeh)
            {
                veh.Delete();
            }
            NotificationScript.ShowNotification($"All vehicles are removed.");
            if (shouldRemoveProps)
            {
                Prop[] allProp = World.GetAllProps();
                foreach (Prop prop in allProp)
                {
                    prop.Delete();
                }
                NotificationScript.ShowNotification($"All entities are removed too.");
            }
            TriggerServerEvent("didClearJustHappen");
        }
    }

    public class VehicleMisc : BaseScript
    {
        [Command("nodoors")]
        private void BreakAllDoorsOfCar()
        {
            if (Game.PlayerPed.IsInVehicle() && Game.PlayerPed.SeatIndex == VehicleSeat.Driver)
            {
                Vehicle vehicle = Game.PlayerPed.CurrentVehicle;
                API.SetVehicleDoorBroken(vehicle.Handle, 0, true);
                API.SetVehicleDoorBroken(vehicle.Handle, 1, true);
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
