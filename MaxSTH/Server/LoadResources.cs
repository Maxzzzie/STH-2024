using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Globalization;
using STHMaxzzzie.Server;

public static class LoadResources
{
    public static List<string> loadWhitelist()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_whitelist_file = $"{path_to_resource}/Resources/Whitelist.txt";

        // Check if the file exists
        if (!File.Exists(path_to_whitelist_file))
        {
            CitizenFX.Core.Debug.WriteLine($"Whitelist.txt does not exist at: {path_to_whitelist_file}");
            return null; // File does not exist, return null
        }

        // Read all lines from the whitelist file
        string[] lines = File.ReadAllLines(path_to_whitelist_file);
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
                    CitizenFX.Core.Debug.WriteLine($"Invalid Discord ID in Whitelist.txt: {discordIdString} from line: {line}");
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine($"Invalid line format in Whitelist.txt: {line}");
            }
        }

        // Check if any valid IDs were found
        if (result.Count == 0)
        {
            CitizenFX.Core.Debug.WriteLine("No valid entries found in Whitelist.txt.");
        }
        else
        {
            CitizenFX.Core.Debug.WriteLine($"{result.Count} valid entries loaded from Whitelist.txt.");
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

            string[] parts = line.Split(' ');
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
            {
                continue; // Skip empty lines and any lines starting with //
            }
            else if (parts.Length < 2)
            {
                CitizenFX.Core.Debug.WriteLine($"Insufficient data in AllowedVehicles.txt line: {line}");
                continue;
            }

            string allowedVehicleName = parts[0].Trim().ToLower();
            string vehicleDataString = parts[1].Trim().ToLower();
            AllowedVehiclesDict.Add(allowedVehicleName, vehicleDataString);
        }
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
                playerVehicleColourDict.Add(playerName,line);
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
            else if (parts.Length < 4)
            {
                CitizenFX.Core.Debug.WriteLine($"Insufficient data in StreamLootsCardInfo.txt line: {line}");
                continue;
            }
            else if (!Int32.TryParse(parts[3].ToString(), out int temp))
            {
                CitizenFX.Core.Debug.WriteLine($"Target isn't an int in StreamLootsCardInfo.txt line: {line}");
                continue;
            }

            string ChatText = parts[2].Trim();
            StreamLootsCardInfoDict.Add(ChatText, line);
            Debug.WriteLine($"read \"{line}\" and added {parts[2]}");
            
        }
        return StreamLootsCardInfoDict;
    }

}
