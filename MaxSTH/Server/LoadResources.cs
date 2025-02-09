using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Globalization;

namespace STHMaxzzzie.Server
{
    public class LoadResources : BaseScript
    {
        public static List<string> loadAdmins()
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_admins_file = $"{path_to_resource}/Resources/Admins.txt";

            // Check if the file exists
            if (!File.Exists(path_to_admins_file))
            {
                CitizenFX.Core.Debug.WriteLine($"Admins.txt does not exist at: {path_to_admins_file}");
                return null; // File does not exist, return null
            }

            // Read all lines from the Admins.txt file
            string[] lines = File.ReadAllLines(path_to_admins_file);
            List<string> result = new List<string>();

            // Iterate through each line in the file
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                // Split the line at the colon
                string[] parts = line.Trim().Split(':');

                // Check if the split resulted in the expected format
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                else if (parts.Length == 2)
                {
                    // Trim the Discord ID and check if it's an integer
                    string discordIdString = parts[1].Trim();
                    if (long.TryParse(discordIdString, out long discordId))
                    {
                        result.Add(discordIdString); // Add the Discord ID to the result list
                    }
                    else
                    {
                        CitizenFX.Core.Debug.WriteLine($"Invalid Discord ID in Admins.txt: {discordIdString} from line: {line}");
                    }
                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine($"Invalid line format in Admins.txt: {line}");
                }
            }

            // Check if any valid IDs were found
            if (result.Count == 0)
            {
                CitizenFX.Core.Debug.WriteLine("No valid entries found in Admins.txt.");
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine($"{result.Count} valid entries loaded from Admins.txt.");
            }

            return result; // Return the list of valid Discord IDs
        }

        public static Dictionary<string, Vector3> tpLocations()
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_teleportLocations_file = $"{path_to_resource}/Resources/TeleportLocations.txt";

            if (!File.Exists(path_to_teleportLocations_file))
            {
                CitizenFX.Core.Debug.WriteLine($"TeleportLocations.txt does not exist at: {path_to_teleportLocations_file}");
                return null;
            }

            string[] tpLines = File.ReadAllLines(path_to_teleportLocations_file);
            var teleportLocationDict = new Dictionary<string, Vector3>();

            foreach (string line in tpLines)
            {
                string[] parts = line.Split(' ');
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                else if (parts.Length < 4)
                {
                    CitizenFX.Core.Debug.WriteLine($"Insufficient data in TeleportLocations.txt line: {line}");
                    continue;
                }

                try
                {
                    string tpName = parts[0].Trim();
                    float tpX = float.Parse(parts[1].Trim());
                    float tpY = float.Parse(parts[2].Trim());
                    float tpZ = float.Parse(parts[3].Trim());
                    Vector3 tpXYZ = new Vector3(tpX, tpY, tpZ);
                    teleportLocationDict.Add(tpName, tpXYZ);
                }
                catch (FormatException ex)
                {
                    CitizenFX.Core.Debug.WriteLine($"Error processing line '{line}': {ex.Message}");
                }
            }
            return teleportLocationDict;
        }

        public static Dictionary<string, Vector4> respawnLocations()
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_respawnLocations_file = $"{path_to_resource}/Resources/RespawnLocations.txt";

            if (!File.Exists(path_to_respawnLocations_file))
            {
                CitizenFX.Core.Debug.WriteLine($"RespawnLocations.txt does not exist at: {path_to_respawnLocations_file}");
                return new Dictionary<string, Vector4>(); // Return an empty dictionary
            }

            string[] spawnLines = File.ReadAllLines(path_to_respawnLocations_file);
            var respawnLocationsDict = new Dictionary<string, Vector4>();

            foreach (string line in spawnLines)
            {
                string[] parts = line.Split(',');
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                else if (parts.Length != 5)
                {
                    CitizenFX.Core.Debug.WriteLine($"Insufficient data in RespawnLocations.txt line: {line}");
                    continue;
                }

                try
                {
                    string spawnName = parts[0].Trim();
                    float spawnW = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
                    float spawnX = float.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
                    float spawnY = float.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
                    float spawnZ = float.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                    Vector4 spawnXYZH = new Vector4(spawnW, spawnX, spawnY, spawnZ);
                    respawnLocationsDict.Add(spawnName, spawnXYZH);
                    //CitizenFX.Core.Debug.WriteLine($"Respawn locations successfully loaded {spawnName}.");
                }
                catch (FormatException ex)
                {
                    CitizenFX.Core.Debug.WriteLine($"Error processing line '{line}': {ex.Message}");
                }
            }
            return respawnLocationsDict;
        }

        public static Dictionary<string, Vector3> calloutsList()
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_maxzzzieCallouts_file = $"{path_to_resource}/Resources/MaxzzzieCallouts.txt";
            //CitizenFX.Core.Debug.WriteLine($"load calloutsList");
            if (!File.Exists(path_to_maxzzzieCallouts_file))
            {
                CitizenFX.Core.Debug.WriteLine($"MaxzzzieCallouts.txt does not exist at: {path_to_maxzzzieCallouts_file}");
                return new Dictionary<string, Vector3>();
            }

            string[] calloutsLines = File.ReadAllLines(path_to_maxzzzieCallouts_file);
            var maxzzzieCalloutsDict = new Dictionary<string, Vector3>();

            foreach (string line in calloutsLines)
            {
                string[] parts = line.Split(',');
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                else if (parts.Length < 4)
                {
                    CitizenFX.Core.Debug.WriteLine($"Insufficient data in MaxzzzieCallouts.txt line: {line}");
                    continue;
                }

                try
                {
                    string calloutName = parts[0].Trim();
                    float calloutX = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
                    float calloutY = float.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
                    float calloutZ = float.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
                    Vector3 calloutXYZ = new Vector3(calloutX, calloutY, calloutZ);
                    maxzzzieCalloutsDict.Add(calloutName, calloutXYZ);
                    //CitizenFX.Core.Debug.WriteLine($"MaxzzzieCalloutsDict : {calloutName} vector3: {calloutXYZ.X} {calloutXYZ.Y} {calloutXYZ.Z}");
                }
                catch (FormatException ex)
                {
                    CitizenFX.Core.Debug.WriteLine($"Error processing line '{line}': {ex.Message}");
                }
            }
            //CitizenFX.Core.Debug.WriteLine("Callout list successfully loaded.");
            return maxzzzieCalloutsDict;
        }

        public static List<string> loadNonAnimalModels()
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_nonAnimalModel_file = $"{path_to_resource}/Resources/PedModelsWithoutAnimals.txt";

            CitizenFX.Core.Debug.WriteLine("Making a NonAnimalModels playlist.");
            if (!File.Exists(path_to_nonAnimalModel_file))
            {
                CitizenFX.Core.Debug.WriteLine($"PedModelsWithoutAnimals.txt does not exist at: {path_to_nonAnimalModel_file}");
                return null;
            }

            string[] nonAnimalModelFile = File.ReadAllLines(path_to_nonAnimalModel_file);
            List<string> result = new List<string>();

            foreach (string model in nonAnimalModelFile)
            {
                if (!string.IsNullOrWhiteSpace(model))
                {
                    result.Add(model.Trim());
                }
            }
            CitizenFX.Core.Debug.WriteLine($"{result.Count} non-animal models loaded.");
            return result;
        }

        public static Dictionary<string, List<Vector3>> mapBounds()
        {
            Debug.WriteLine("Loading Map Bounds");
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_MapBounds_file = $"{path_to_resource}/Resources/MapBounds.txt";

            if (!File.Exists(path_to_MapBounds_file))
            {
                CitizenFX.Core.Debug.WriteLine($"MapBounds.txt does not exist at: {path_to_MapBounds_file}");
                return null;
            }

            string[] MapBoundsLines = File.ReadAllLines(path_to_MapBounds_file);
            var MapBoundsDict = new Dictionary<string, List<Vector3>>();

            foreach (string line in MapBoundsLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                string[] parts = line.Split(' ');
                if (parts.Length < 2)
                {
                    CitizenFX.Core.Debug.WriteLine($"Insufficient data in MapBounds.txt line: {line}");
                    continue;
                }

                List<Vector3> LocationsVectorList = new List<Vector3>();
                string boundsLocationName = parts[0].Trim();
                string boundsLocationCords = parts[1].Trim();
                string[] locationsStringArray = boundsLocationCords.Split(';');

                foreach (string xyz in locationsStringArray)
                {
                    try
                    {
                        string[] coords = xyz.Split(',');
                        if (coords.Length != 3)
                        {
                            CitizenFX.Core.Debug.WriteLine($"Invalid coordinates in MapBounds.txt line: {xyz}");
                            continue;
                        }

                        float xcord = float.Parse(coords[0].Trim());
                        float ycord = float.Parse(coords[1].Trim());
                        float rad = float.Parse(coords[2].Trim());
                        Vector3 vector = new Vector3(xcord, ycord, rad);
                        LocationsVectorList.Add(vector);
                    }
                    catch (FormatException ex)
                    {
                        CitizenFX.Core.Debug.WriteLine($"Error processing coordinates '{xyz}': {ex.Message}");
                    }
                }
                MapBoundsDict.Add(boundsLocationName, LocationsVectorList);
            }
            return MapBoundsDict;
        }

        public static Dictionary<string, string> allowedVehicles()
        {
            List<string> vehiclesWithoutBool = new List<string>{};
            List<string> vehicleClasses = new List<string>{
              "boats","commercial","compacts","coupes","cycles","emergency","helicopters","industrial","military",
                "motorcycles","muscle","off-road","open-wheel","planes","sedans","service","sports","sportsclassics",
                "super","suvs","trains","utility","vans","trailer"};
            //Debug.WriteLine("Loading Allowed Vehicles");
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_AllowedVehicles_file = $"{path_to_resource}/Resources/AllowedVehicles.txt";

            if (!File.Exists(path_to_AllowedVehicles_file))
            {
                CitizenFX.Core.Debug.WriteLine($"AllowedVehicles.txt does not exist at: {path_to_AllowedVehicles_file}");
                return null;
            }

            string[] AllowedVehiclesLines = File.ReadAllLines(path_to_AllowedVehicles_file);
            var AllowedVehiclesDict = new Dictionary<string, string>();

            foreach (string line in AllowedVehiclesLines)
            {
                string lowerLine = line.ToLower();
                string[] parts = lowerLine.Trim().ToLower().Split(',');
                string allowedVehicleName = parts[0].Trim().ToLower();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                else if (parts.Length != 4)
                { 
                    CitizenFX.Core.Debug.WriteLine($"Incorrect data in AllowedVehicles.txt line: \"{line}\"");
                    continue;
                }
                else if (AllowedVehiclesDict.ContainsKey(parts[0]))
                {
                    CitizenFX.Core.Debug.WriteLine($"Double entry for \"{allowedVehicleName}\" in AllowedVehicles.txt line: \"{line}\".");
                    continue;
                }
                else if (!vehicleClasses.Contains(parts[2]))
                {
                    CitizenFX.Core.Debug.WriteLine($"Incorrect vehicle class in AllowedVehicles.txt line: \"{line}\"");
                    continue;
                }
                else if (parts.Length == 4 & !bool.TryParse(parts[3],out bool temp))
                {
                    vehiclesWithoutBool.Add(allowedVehicleName);
                    //Debug.WriteLine($"No bool for restriction set yet in \"{line}\".");
                }

                AllowedVehiclesDict.Add(allowedVehicleName, lowerLine);
                //Debug.WriteLine(lowerLine);

            }
            if(vehiclesWithoutBool.Count > 0)
            {
                Debug.WriteLine($"{vehiclesWithoutBool.Count} entries don't have a restriction bool set in AllowedVehicles.txt");
            }
            Debug.WriteLine($"{AllowedVehiclesDict.Count} vehicles finished loading in AllowedVehicles.txt");
            return AllowedVehiclesDict;
        }

        public static Dictionary<string, string> playerVehicleColour()
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_playerVehicleColour_file = $"{path_to_resource}/Resources/playerVehicleColour.txt";
            //CitizenFX.Core.Debug.WriteLine($"load calloutsList");
            if (!File.Exists(path_to_playerVehicleColour_file))
            {
                CitizenFX.Core.Debug.WriteLine($"playerVehicleColour.txt does not exist at: {path_to_playerVehicleColour_file}");
                return new Dictionary<string, string>();
            }

            string[] playerVehicleColourLine = File.ReadAllLines(path_to_playerVehicleColour_file);
            var playerVehicleColourDict = new Dictionary<string, string>();

            foreach (string line in playerVehicleColourLine)
            {
                string[] parts = line.Split(',');
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                else if (parts.Length != 7)
                {
                    CitizenFX.Core.Debug.WriteLine($"Incorrect data in playerVehicleColour.txt line: {line}");
                    continue;
                }

                try
                {
                    string playerName = parts[0].Trim();
                    int primairyColour = int.Parse(parts[1].Trim());
                    int secundaryColour = int.Parse(parts[2].Trim());
                    int pearlescentColour = int.Parse(parts[3].Trim());
                    int R = int.Parse(parts[4].Trim());
                    int G = int.Parse(parts[5].Trim());
                    int B = int.Parse(parts[6].Trim());
                    playerVehicleColourDict.Add(playerName, line);
                    //CitizenFX.Core.Debug.WriteLine($"playerVehicleColourDict : {playerName}, {primairyColour}, {secundaryColour}");
                }
                catch (FormatException ex)
                {
                    CitizenFX.Core.Debug.WriteLine($"Error processing line '{line}': {ex.Message}");
                }
            }
            //Debug.WriteLine("playerVehicleColour resource successfully loaded.");
            return playerVehicleColourDict;
        }

        // Function to save player vehicle colors to the file
        public static void SavePlayerVehicleColours(Dictionary<string, string> vehicleColourDict)
        {
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_playerVehicleColour_file = $"{path_to_resource}/Resources/playerVehicleColour.txt";

            // Prepare data to write to the file
            List<string> lines = new List<string>();
            lines.Add("//Store player vehicle colours here. ");
            lines.Add("//Use the following method of storing it. \"PlayerName, primairy_colour__id, secundary_colour_id, pearlescent_colour_id, Lights_R_value, Lights_G_value, Lights_B_value\". ");
            lines.Add("//Capitals are important. ");
            lines.Add("//Find the colour id's here. http://wiki.rage.mp/index.php?title=Vehicle_Colors");
            foreach (var entry in vehicleColourDict)
            {
                string line = entry.Value;
                lines.Add(line);
            }

            // Write data to the file, overwriting existing content
            File.WriteAllLines(path_to_playerVehicleColour_file, lines);

            //CitizenFX.Core.Debug.WriteLine("playerVehicleColour resource successfully saved.");
        }

        public static Dictionary<string, string> streamLootsCardInfo()
        {
            Debug.WriteLine("Loading StreamLootsCardInfo");
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_StreamLootsCardInfo_file = $"{path_to_resource}/Resources/StreamLootsCardInfo.txt";

            if (!File.Exists(path_to_StreamLootsCardInfo_file))
            {
                CitizenFX.Core.Debug.WriteLine($"StreamLootsCardInfo.txt does not exist at: {path_to_StreamLootsCardInfo_file}");
                return null;
            }

            string[] StreamLootsCardInfoLines = File.ReadAllLines(path_to_StreamLootsCardInfo_file);
            var StreamLootsCardInfoDict = new Dictionary<string, string>();

            foreach (string line in StreamLootsCardInfoLines)
            {

                string[] parts = line.Split('*');
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and any lines starting with //
                }
                if (parts.Length == 2 && line.StartsWith("Cue time interval in seconds*") && Int32.TryParse(parts[1].ToString(), out int time))
                {
                    StreamLootsEffect.SLItterateTime = time;
                    StreamLootsEffect.UpdateSLItterateTime();
                    continue;
                }
                else if (parts.Length != 4)
                {
                    CitizenFX.Core.Debug.WriteLine($"Insufficient or incorrect data in StreamLootsCardInfo.txt line: {line}");
                    continue;
                }
                else if (!Int32.TryParse(parts[3].ToString(), out int temp))
                {
                    CitizenFX.Core.Debug.WriteLine($"Target isn't an int in StreamLootsCardInfo.txt line: {line}");
                    continue;
                }

                string ChatText = parts[2].Trim();
                StreamLootsCardInfoDict.Add(ChatText, line);
                //Debug.WriteLine($"streamlootssuccesfully added \"{line}\"");

            }
            Debug.WriteLine($"Finished loading StreamLootsCardInfo");
            return StreamLootsCardInfoDict;
        }

        
        
        static public Vector4 DefaultSpawnLocation = new Vector4(0,0,71.5f,0);
        static public Vector4 MugshotPosition = new Vector4(0,0,71.5f,0);
        static public String MOTD = "No MotD is loaded.";
        static public bool defaultShouldFireBeControlled;
        static public int FireControlrange;


        public static void LoadConfig()
        {
            Debug.WriteLine("Loading MaxSTH_Config");
            var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
            var path_to_MaxSTH_Config_file = $"{path_to_resource}/Resources/MaxSTH_Config.txt";

            if (!File.Exists(path_to_MaxSTH_Config_file))
            {
                Debug.WriteLine($"MaxSTH_Config.txt does not exist at: {path_to_MaxSTH_Config_file}");
                return;
            }

            string[] MaxSTH_ConfigLines = File.ReadAllLines(path_to_MaxSTH_Config_file);

            // Repeated input checks
            bool isDefaultSpawnLocationSet = false;
            bool isMugshotAreaPositionSet = false;
            bool isMessageOfTheDaySet = false;
            bool isDefaultFireControlRangeSet = false;
            bool isDefaultShouldFireBeControlledSet = false;
            //bool isRestrictCommandsToHostsSet = false;    just not a way to set restrictions with bools atm.

            foreach (string line in MaxSTH_ConfigLines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue; // Skip empty lines and comments
                }

                string[] parts = line.Split('=');
                if (parts.Length != 2)
                {
                    Debug.WriteLine($"Invalid configuration line: {line}");
                    continue;
                }

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // Handle each configuration key
                switch (key)
                {
                    case "Default spawn location":
                        if (isDefaultSpawnLocationSet)
                        {
                            Debug.WriteLine($"Duplicate \"Default spawn location\" in MaxSTH_Config.txt");
                            break;
                        }
                        if (TryParseVector4(value, out Vector4 spawnLocation))
                        {
                            DefaultSpawnLocation = spawnLocation;
                            isDefaultSpawnLocationSet = true;
                            
                            Debug.WriteLine($"Set Default spawn location to: {spawnLocation}");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to parse vector4 for Default spawn location: {value}");
                        }
                        break;

                    case "Mugshot area position":
                        if (isMugshotAreaPositionSet)
                        {
                            Debug.WriteLine($"Duplicate \"Mugshot area position\" in MaxSTH_Config.txt");
                            break;
                        }
                        if (TryParseVector4(value, out Vector4 newMugshotPosition))
                        {
                            isMugshotAreaPositionSet = true;
                            MugshotPosition = newMugshotPosition;
                            Debug.WriteLine($"Set Mugshot area position to: {MugshotPosition}");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to parse vector4 for Mugshot area position: {value}");
                        }
                        break;

                    case "Message of the Day":
                        if (isMessageOfTheDaySet)
                        {
                            Debug.WriteLine($"Duplicate \"Message of the Day\" in MaxSTH_Config.txt");
                            break;
                        }
                        isMessageOfTheDaySet = true;
                        MOTD = value;
                        Debug.WriteLine($"Set Message of the Day to: {value}");
                        break;

                    case "Default fire control range":
                        if (isDefaultFireControlRangeSet)
                        {
                            Debug.WriteLine($"Duplicate \"Default fire control range\" in MaxSTH_Config.txt");
                            break;
                        }
                        if (int.TryParse(value, out int newFireControlRange))
                        {
                            isDefaultFireControlRangeSet = true;
                            FireControlrange = newFireControlRange;
                            Debug.WriteLine($"Set \"Default fire control range\" to: {FireControlrange}");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to parse integer for Default fire control range: {value}");
                        }
                        break;

                    case "Is fire control active":
                        if (isDefaultShouldFireBeControlledSet)
                        {
                            Debug.WriteLine($"Duplicate \"Is fire control active\" in MaxSTH_Config.txt");
                            break;
                        }
                        if (bool.TryParse(value, out bool newShouldFireBeControlled))
                        {
                            isDefaultShouldFireBeControlledSet = true;
                            defaultShouldFireBeControlled = newShouldFireBeControlled;
                            Debug.WriteLine($"Set \"Is fire control active\" to: {defaultShouldFireBeControlled}");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to parse boolean for \"Is fire control active\": {value}");
                        }
                        break;

                    // case "Restrict commands to hosts": //currently not functional
                    //     if (isRestrictCommandsToHostsSet)
                    //     {
                    //         Debug.WriteLine($"Duplicate \"Restrict commands to hosts\" in MaxSTH_Config.txt");
                    //         break;
                    //     }
                    //     if (bool.TryParse(value, out bool restrictToHosts))
                    //     {
                    //         isRestrictCommandsToHostsSet = true;
                    //        // TriggerClientEvent("updateRestrictCommands", restrictToHosts);
                    //         Debug.WriteLine($"Set Restrict commands to hosts to: {restrictToHosts}\nThis currently doesn't function.");
                    //     }
                    //     else
                    //     {
                    //         Debug.WriteLine($"Failed to parse boolean for Restrict commands to hosts: {value}");
                    //     }
                    //     break;

                    default:
                        Debug.WriteLine($"Unknown configuration key: {key}");
                        break;
                }
            }
             // Final checks for missing configurations
        if (!isDefaultSpawnLocationSet) Debug.WriteLine("\"Default spawn location\" not set. Data is missing.");
        if (!isMugshotAreaPositionSet) Debug.WriteLine("\"Mugshot area position\" not set. Data is missing.");
        if (!isMessageOfTheDaySet) Debug.WriteLine("\"Message of the Day\" not set. Data is missing.");
        if (!isDefaultFireControlRangeSet) Debug.WriteLine("\"Default fire control range\" not set. Data is missing.");
        if (!isDefaultShouldFireBeControlledSet) Debug.WriteLine("\"Is fire control active\" not set. Data is missing.");
        // if (!isRestrictCommandsToHostsSet) Debug.WriteLine("Restrict commands to hosts not set. Data is missing.");
            UpdateConfig.SendConfigToClient();
            Debug.WriteLine("Finished loading MaxSTH_Config");
        }

        

        private static bool TryParseVector4(string input, out Vector4 result)
        {
            result = default;
            string[] components = input.Split(',');
            if (components.Length != 4)
            {
                return false;
            }

            if (float.TryParse(components[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(components[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(components[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
                float.TryParse(components[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float heading))
            {
                result = new Vector4(x, y, z, heading);
                return true;
            }

            return false;
        }
    }
}

