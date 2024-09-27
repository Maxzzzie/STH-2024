using System;
using System.Collections.Generic;
using System.IO; //we need this for linking the files.
using CitizenFX.Core;
using CitizenFX.Core.Native;

//string[] words = lines[0].Split(' '); Splits a string into a seperate array using spaces as seperators. Defined by (' ')

public static class LoadResources
{

    public static List<string> loadWhitelist()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_whitelist_file = $"{path_to_resource}/Resources/Whitelist.txt";

        bool exists = File.Exists(path_to_whitelist_file); // true if file is there
        if (exists == false)
        {
            Debug.WriteLine($"Whitelist.txt does not exist in :{path_to_whitelist_file}");
            return null;
        }

        string[] lines = File.ReadAllLines(path_to_whitelist_file);
        List<string> result = new List<string>();

        foreach (string line in lines)
        {
            char first_char = line[0];
            //first word is int, probably a discord id
            int temp;
            bool is_int = int.TryParse(first_char.ToString(), out temp);

            if (is_int == false)
            {
                continue; //means we skip to next line
            }

            result.Add(line.Split(' ')[0].Trim());
        }
        return result;
    }


    public static Dictionary<string, Vector3> tpLocations()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_teleportLocations_file = $"{path_to_resource}/Resources/TeleportLocations.txt";

        bool exists = File.Exists(path_to_teleportLocations_file);
        if (exists == false)
        {
            Debug.WriteLine($"TeleportLocations.txt does not exist in :{path_to_teleportLocations_file}");
            return null;
        }

        string[] tpLines = File.ReadAllLines(path_to_teleportLocations_file);
        var teleportLocationDict = new Dictionary<string, Vector3>();

        foreach (string line in tpLines)
        {
            string tpName = (line.Split(' ')[0].Trim());
            float tpX = int.Parse(line.Split(' ')[1].Trim());
            float tpY = int.Parse(line.Split(' ')[2].Trim());
            float tpZ = int.Parse(line.Split(' ')[3].Trim());
            Vector3 tpXYZ = new Vector3(tpX, tpY, tpZ);
            teleportLocationDict.Add(tpName, tpXYZ);
        }
        return teleportLocationDict;
    }

    public static Dictionary<string, Vector4> respawnLocations()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_respawnLocations_file = $"{path_to_resource}/Resources/RespawnLocations.txt";

        bool exists = File.Exists(path_to_respawnLocations_file);
        if (exists == false)
        {
            Debug.WriteLine($"RespawnLocations.txt does not exist in :{path_to_respawnLocations_file}");
            return null;
        }

        string[] spawnLines = File.ReadAllLines(path_to_respawnLocations_file);
        var respawnLocationsDict = new Dictionary<string, Vector4>();

        foreach (string line in spawnLines)
        {
            string spawnName = (line.Split(' ')[0].Trim());
            float spawnW = float.Parse(line.Split(' ')[1].Trim());
            float spawnX = float.Parse(line.Split(' ')[2].Trim());
            float spawnY = float.Parse(line.Split(' ')[3].Trim());
            float spawnZ = float.Parse(line.Split(' ')[4].Trim());
            Vector4 spawnXYZH = new Vector4(spawnW, spawnX, spawnY, spawnZ);
            respawnLocationsDict.Add(spawnName, spawnXYZH);
        }
        Debug.WriteLine($"server.Respawnlocations succesfully loaded the respawnDict.");
        return respawnLocationsDict;
    }


    public static List<string> loadNonAnimalModels()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_nonAnimalModel_file = $"{path_to_resource}/Resources/PedModelsWithoutAnimals.txt";
        Debug.WriteLine($"Making a NonAnimalModels playlist.");
        bool exists = File.Exists(path_to_nonAnimalModel_file); // true if file is there
        if (exists == false)
        {
            Debug.WriteLine($"PedModelsWithoutAnimals.txt does not exist in :{path_to_nonAnimalModel_file}");
            return null;
        }

        string[] nonAnimalModelFile = File.ReadAllLines(path_to_nonAnimalModel_file);
        List<string> result = new List<string>();

        foreach (string model in nonAnimalModelFile)
        {
            result.Add(model.Trim());
        }
        Debug.WriteLine($"{result.Count}");
        return result;
    }

    public static Dictionary<string, List<Vector3>> mapBounds()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_MapBounds_file = $"{path_to_resource}/Resources/MapBounds.txt";

        bool exists = File.Exists(path_to_MapBounds_file);
        if (exists == false)
        {
            Debug.WriteLine($"MapBounds.txt does not exist in :{path_to_MapBounds_file}");
            return null;
        }

        string[] MapBoundsLines = File.ReadAllLines(path_to_MapBounds_file);
        var MapBoundsDict = new Dictionary<string, List<Vector3>>();

        foreach (string line in MapBoundsLines) //for each line in the file
        {
            List<Vector3> LocationsVectorList = new List<Vector3>();
            string boundsLocationName = line.Split(' ')[0].Trim(); //get the name
            string boundsLocationCords = line.Split(' ')[1].Trim(); //get whatever's behind the space
            string[] locationsStringArray = boundsLocationCords.Split(';'); //split whatever's behind the space into an array called locationsStringArray

            foreach (string xyz in locationsStringArray)
            {
                float xcord = float.Parse(xyz.Split(',')[0]);
                float ycord = float.Parse(xyz.Split(',')[1]);
                float rad = float.Parse(xyz.Split(',')[2]);
                Vector3 vector = new Vector3(xcord, ycord, rad);
                LocationsVectorList.Add(vector);
            }
            MapBoundsDict.Add(boundsLocationName, LocationsVectorList);
        }
        return MapBoundsDict;
    }


    public static Dictionary<string, string> allowedVehicles()
    {
        var path_to_resource = API.GetResourcePath(API.GetCurrentResourceName());
        var path_to_AllowedVehicles_file = $"{path_to_resource}/Resources/AllowedVehicles.txt";

        bool exists = File.Exists(path_to_AllowedVehicles_file);
        if (exists == false)
        {
            Debug.WriteLine($"AllowedVehicles.txt does not exist in :{path_to_AllowedVehicles_file}");
            return null;
        }

        string[] AllowedVehiclesLines = File.ReadAllLines(path_to_AllowedVehicles_file);
        var AllowedVehiclesDict = new Dictionary<string, string>();

        foreach (string line in AllowedVehiclesLines)
        {
            string allowedVehicleName = line.Split(' ')[0].Trim().ToLower();
            string vehicleDataString = line.Split(' ')[1].Trim().ToLower();
            AllowedVehiclesDict.Add(allowedVehicleName, vehicleDataString);
        }
        return AllowedVehiclesDict;
    }
}
