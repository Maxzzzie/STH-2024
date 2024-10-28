using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Globalization;

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
        if (parts.Length == 2)
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
            if (parts.Length < 4)
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
            if (parts.Length != 5)
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
            if (parts.Length < 4)
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
            if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

            string[] parts = line.Split(' ');
            if (parts.Length < 2)
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
}


// using System;
// using System.Collections.Generic;
// using System.IO; //we need this for linking the files.
// using CitizenFX.Core;
// using CitizenFX.Core.Native;

// //string[] words = lines[0].Split(' '); Splits a string into a seperate array using spaces as seperators. Defined by (' ')

// public static class LoadResources
// {

//     public static List<string> loadWhitelist()
//     {
//         var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//         var path_to_whitelist_file = $"{path_to_resource}/Resources/Whitelist.txt";

//         bool exists = File.Exists(path_to_whitelist_file); // true if file is there
//         if (exists == false)
//         {
//             CitizenFX.Core.Debug.WriteLine($"Whitelist.txt does not exist in :{path_to_whitelist_file}");
//             return null;
//         }

//         string[] lines = File.ReadAllLines(path_to_whitelist_file);
//         List<string> result = new List<string>();

//         foreach (string line in lines)
//         {
//             char first_char = line[0];
//             //first word is int, probably a discord id
//             int temp;
//             bool is_int = int.TryParse(first_char.ToString(), out temp);

//             if (is_int == false)
//             {
//                 continue; //means we skip to next line
//             }

//             result.Add(line.Split(' ')[0].Trim());
//         }
//         return result;
//     }


//     public static Dictionary<string, Vector3> tpLocations()
//     {
//         var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//         var path_to_teleportLocations_file = $"{path_to_resource}/Resources/TeleportLocations.txt";

//         bool exists = File.Exists(path_to_teleportLocations_file);
//         if (exists == false)
//         {
//             CitizenFX.Core.Debug.WriteLine($"TeleportLocations.txt does not exist in :{path_to_teleportLocations_file}");
//             return null;
//         }

//         string[] tpLines = File.ReadAllLines(path_to_teleportLocations_file);
//         var teleportLocationDict = new Dictionary<string, Vector3>();

//         foreach (string line in tpLines)
//         {
//             string tpName = (line.Split(' ')[0].Trim());
//             float tpX = int.Parse(line.Split(' ')[1].Trim());
//             float tpY = int.Parse(line.Split(' ')[2].Trim());
//             float tpZ = int.Parse(line.Split(' ')[3].Trim());
//             Vector3 tpXYZ = new Vector3(tpX, tpY, tpZ);
//             teleportLocationDict.Add(tpName, tpXYZ);
//         }
//         return teleportLocationDict;
//     }

//     public static Dictionary<string, Vector4> respawnLocations()
// {
//     var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//     var path_to_respawnLocations_file = $"{path_to_resource}/Resources/RespawnLocations.txt";

//     if (!File.Exists(path_to_respawnLocations_file))
//     {
//         CitizenFX.Core.Debug.WriteLine($"RespawnLocations.txt does not exist in :{path_to_respawnLocations_file}");
//         return new Dictionary<string, Vector4>(); // Return an empty dictionary
//     }

//     string[] spawnLines = File.ReadAllLines(path_to_respawnLocations_file);
//     var respawnLocationsDict = new Dictionary<string, Vector4>();

//     foreach (string line in spawnLines)
//     {
//         try
//         {
//             string[] parts = line.Split(' ');
//             string spawnName = parts[0].Trim();
//             if (parts.Length < 5) throw new FormatException("Insufficient data");

//             float spawnW = float.Parse(parts[1].Trim());
//             float spawnX = float.Parse(parts[2].Trim());
//             float spawnY = float.Parse(parts[3].Trim());
//             float spawnZ = float.Parse(parts[4].Trim());
//             Vector4 spawnXYZH = new Vector4(spawnW, spawnX, spawnY, spawnZ);
//             respawnLocationsDict.Add(spawnName, spawnXYZH);
//         }
//         catch (Exception ex)
//         {
//             CitizenFX.Core.Debug.WriteLine($"Error processing line '{line}': {ex.Message}");
//         }
//     }
//     CitizenFX.Core.Debug.WriteLine($"server.Respawnlocations successfully loaded the respawnDict.");
//     return respawnLocationsDict;
// }
    
//     // public static Dictionary<string, Vector4> respawnLocations()
//     // {
//     //     var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//     //     var path_to_respawnLocations_file = $"{path_to_resource}/Resources/RespawnLocations.txt";

//     //     bool exists = File.Exists(path_to_respawnLocations_file);
//     //     if (exists == false)
//     //     {
//     //         CitizenFX.Core.Debug.WriteLine($"RespawnLocations.txt does not exist in :{path_to_respawnLocations_file}");
//     //         return null;
//     //     }

//     //     string[] spawnLines = File.ReadAllLines(path_to_respawnLocations_file);
//     //     var respawnLocationsDict = new Dictionary<string, Vector4>();

//     //     foreach (string line in spawnLines)
//     //     {
//     //         string spawnName = line.Split(' ')[0].Trim();
//     //         float spawnW = float.Parse(line.Split(' ')[1].Trim());
//     //         float spawnX = float.Parse(line.Split(' ')[2].Trim());
//     //         float spawnY = float.Parse(line.Split(' ')[3].Trim());
//     //         float spawnZ = float.Parse(line.Split(' ')[4].Trim());
//     //         Vector4 spawnXYZH = new Vector4(spawnW, spawnX, spawnY, spawnZ);
//     //         respawnLocationsDict.Add(spawnName, spawnXYZH);
//     //     }
//     //     CitizenFX.Core.Debug.WriteLine($"server.Respawnlocations succesfully loaded the respawnDict.");
//     //     return respawnLocationsDict;
//     // }

//     public static Dictionary<string, Vector3> calloutsList()
//     {
//         var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//         var path_to_maxzzzieCallouts_file = $"{path_to_resource}/Resources/MaxzzzieCallouts.txt";

//         bool exists = File.Exists(path_to_maxzzzieCallouts_file);
//         if (exists == false)
//         {
//             CitizenFX.Core.Debug.WriteLine($"MaxzzzieCallouts.txt does not exist in :{path_to_maxzzzieCallouts_file}");
//             return null;
//         }

//         string[] calloutsLines = File.ReadAllLines(path_to_maxzzzieCallouts_file);
//         var maxzzzieCalloutsDict = new Dictionary<string, Vector3>();

//         foreach (string line in calloutsLines)
//         {
//             string spawnName = line.Split(',')[0].Trim();
//             float spawnX = float.Parse(line.Split(',')[1].Trim());
//             float spawnY = float.Parse(line.Split(',')[2].Trim());
//             float spawnZ = float.Parse(line.Split(',')[3].Trim());
//             Vector3 spawnXYZH = new Vector3(spawnX, spawnY, spawnZ);
//             maxzzzieCalloutsDict.Add(spawnName, spawnXYZH);
//         }
//         CitizenFX.Core.Debug.WriteLine($"server.CalloutList succesfully loaded the Callouts.");
//         return maxzzzieCalloutsDict;
//     }


//     public static List<string> loadNonAnimalModels()
//     {
//         var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//         var path_to_nonAnimalModel_file = $"{path_to_resource}/Resources/PedModelsWithoutAnimals.txt";
//         CitizenFX.Core.Debug.WriteLine($"Making a NonAnimalModels playlist.");
//         bool exists = File.Exists(path_to_nonAnimalModel_file); // true if file is there
//         if (exists == false)
//         {
//             CitizenFX.Core.Debug.WriteLine($"PedModelsWithoutAnimals.txt does not exist in :{path_to_nonAnimalModel_file}");
//             return null;
//         }

//         string[] nonAnimalModelFile = File.ReadAllLines(path_to_nonAnimalModel_file);
//         List<string> result = new List<string>();

//         foreach (string model in nonAnimalModelFile)
//         {
//             result.Add(model.Trim());
//         }
//         CitizenFX.Core.Debug.WriteLine($"{result.Count}");
//         return result;
//     }

//     public static Dictionary<string, List<Vector3>> mapBounds()
//     {
//         Debug.WriteLine($"Loading Mapbounds");
//         var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//         var path_to_MapBounds_file = $"{path_to_resource}/Resources/MapBounds.txt";

//         bool exists = File.Exists(path_to_MapBounds_file);
//         if (exists == false)
//         {
//             CitizenFX.Core.Debug.WriteLine($"MapBounds.txt does not exist in :{path_to_MapBounds_file}");
//             return null;
//         }

//         string[] MapBoundsLines = File.ReadAllLines(path_to_MapBounds_file);
//         var MapBoundsDict = new Dictionary<string, List<Vector3>>();

//         foreach (string line in MapBoundsLines) //for each line in the file
//         {
//             List<Vector3> LocationsVectorList = new List<Vector3>();
//             string boundsLocationName = line.Split(' ')[0].Trim(); //get the name
//             string boundsLocationCords = line.Split(' ')[1].Trim(); //get whatever's behind the space
//             string[] locationsStringArray = boundsLocationCords.Split(';'); //split whatever's behind the space into an array called locationsStringArray
//             Debug.WriteLine($" foreach mapboundsdict name:{boundsLocationName}");

//             foreach (string xyz in locationsStringArray)
//             {
//                 float xcord = float.Parse(xyz.Split(',')[0]);
//                 float ycord = float.Parse(xyz.Split(',')[1]);
//                 float rad = float.Parse(xyz.Split(',')[2]);
//                 Vector3 vector = new Vector3(xcord, ycord, rad);
//                 LocationsVectorList.Add(vector);
//             }
//             MapBoundsDict.Add(boundsLocationName, LocationsVectorList);
//         }
//         return MapBoundsDict;
//     }


//     public static Dictionary<string, string> allowedVehicles()
//     {
//         Debug.WriteLine($"Loading allowedVehicles");
//         var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
//         var path_to_AllowedVehicles_file = $"{path_to_resource}/Resources/AllowedVehicles.txt";

//         bool exists = File.Exists(path_to_AllowedVehicles_file);
//         if (exists == false)
//         {
//             CitizenFX.Core.Debug.WriteLine($"AllowedVehicles.txt does not exist in :{path_to_AllowedVehicles_file}");
//             return null;
//         }

//         string[] AllowedVehiclesLines = File.ReadAllLines(path_to_AllowedVehicles_file);
//         var AllowedVehiclesDict = new Dictionary<string, string>();

//         foreach (string line in AllowedVehiclesLines)
//         {
//             string allowedVehicleName = line.Split(' ')[0].Trim().ToLower();
//             string vehicleDataString = line.Split(' ')[1].Trim().ToLower();
//             AllowedVehiclesDict.Add(allowedVehicleName, vehicleDataString);
//             //Debug.WriteLine($"{allowedVehicleName}, {vehicleDataString}");
//         }
//         return AllowedVehiclesDict;
//     }
// }
