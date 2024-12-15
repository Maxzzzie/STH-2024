using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Numerics;
using STHMaxzzzie;
using System.Drawing;


namespace STHMaxzzzie.Server
{
    public class ServerMain : BaseScript
    {
        static List<string> allowed_discord_ids = new List<string>();
        public static Dictionary<string, Vector4> respawnLocationsDict;
        public static Dictionary<string, Vector3> maxzzzieCalloutsDict;
        public static Dictionary<string, string> vehicleinfoDict;
        public static bool isVehRestricted = true;




        public ServerMain()
        {
            //load whitelist to list allowed_discord_ids
            allowed_discord_ids = LoadResources.loadWhitelist();

            foreach (string id in allowed_discord_ids)
            {
                API.ExecuteCommand($"add_ace identifier.discord:{id} \"command\" allow");
                //CitizenFX.Core.Debug.WriteLine($"load whitelist id : {id}");
            }


            respawnLocationsDict = LoadResources.respawnLocations();
            maxzzzieCalloutsDict = LoadResources.calloutsList();
            vehicleinfoDict = LoadResources.allowedVehicles();

        }

        [EventHandler("reloadResources")]
        void reloadResources(int source)
        {
            //CitizenFX.Core.Debug.WriteLine($"reloadResources triggered");
            respawnLocationsDict = LoadResources.respawnLocations();
            maxzzzieCalloutsDict = LoadResources.calloutsList();
            vehicleinfoDict = LoadResources.allowedVehicles();
            Vehicles.vehicleColourForPlayer = LoadResources.playerVehicleColour();
            foreach (Player player in Players)
            {
                ServerMain.sendRespawnLocationsDict(player);
                ServerMain.sendVehicleinfoDict(player);
                ServerMain.sendMaxzzzieCalloutsDict(player);
                Appearance.sendNonAnimalModel(player);
                string name = player.Name;
                if (Vehicles.vehicleColourForPlayer.ContainsKey(name))
                {
                    string line = Vehicles.vehicleColourForPlayer[name];
                    string[] parts = line.Split(',');
                    int primairyColour = int.Parse(parts[1].Trim());
                    int secundaryColour = int.Parse(parts[2].Trim());
                    int pearlescentColour = int.Parse(parts[3].Trim());
                    int lightsR = int.Parse(parts[4].Trim());
                    int lightsG = int.Parse(parts[5].Trim());
                    int lightsB = int.Parse(parts[6].Trim());
                    TriggerClientEvent(player, "receiveVehicleColor", primairyColour, secundaryColour, pearlescentColour, lightsR, lightsG, lightsB);
                }
            }
            MapBounds.updateCircle(true);
            TriggerClientEvent("isWeaponAllowed", Armoury.isWeaponsAllowed);
            TriggerClientEvent("updatePvp", Armoury.isPvpAllowed);
            TriggerClientEvent("stamina");
            TriggerClientEvent("whatIsVehAllowed", isVehRestricted);
            TriggerClientEvent("VehicleFixStatus", Misc.AllowedToFixStatus, Misc.fixWaitTime);
            TriggerClientEvent("disableCanPlayerShootFromVehicles", Armoury.isShootingFromVehicleAllowed);
            DelayMode.updateClientsDelayModeSettings();
        }


        [EventHandler("playerJoining")]
        void playerJoiningHandler([FromSource] Player source, string old_id)
        {
            ServerMain.sendRespawnLocationsDict(source);
            ServerMain.sendVehicleinfoDict(source);
            ServerMain.sendMaxzzzieCalloutsDict(source);
            MapBounds.updateCircle(true);
            Appearance.sendNonAnimalModel(source);
            source.TriggerEvent("isWeaponAllowed", Armoury.isWeaponsAllowed);
            source.TriggerEvent("updatePvp", Armoury.isPvpAllowed);
            source.TriggerEvent("disableCanPlayerShootFromVehicles", Armoury.isShootingFromVehicleAllowed);
            source.TriggerEvent("respawnPlayer");
            source.TriggerEvent("VehicleFixStatus", Misc.AllowedToFixStatus, Misc.fixWaitTime);
            source.TriggerEvent("Stamina");
            source.TriggerEvent("whatIsVehAllowed", isVehRestricted);
            BlipHandler.UpdateClientBlips();
            TriggerEvent("playerJoinedWhileGameIsActive", source.Handle);
            TriggerEvent("updatePlayerBlips");
        }

        [EventHandler("playerDropped")]
        void playerDroppedHandler([FromSource] Player source, string reason)
        {
            if (RoundHandling.teamAssignment[int.Parse(source.Handle)] == 1 && RoundHandling.gameMode != "none")
            {
                TriggerEvent("endGame", "end");
            }
            if (PriusMechanics.playerPris.ContainsKey(source))
            {
                PriusMechanics.playerPris.Remove(source);
                BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
                request.BlipsToRemove.Add($"pri{source.Name}");
                BlipHandler.AddBlips(request);
            }
            TriggerEvent("updatePlayerBlips");
        }

        [Command("togglevehres", Restricted = true)] //restriction default = true
        void toggleveh(int source, List<object> args, string raw)
        {
            {
                if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
                {
                    isVehRestricted = bool.Parse(args[0].ToString());
                    TriggerClientEvent("whatIsVehAllowed", isVehRestricted);
                }
                else if (args.Count == 0)
                {
                    isVehRestricted = !isVehRestricted;
                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglevehres (true/false)");
                }
            }
            TriggerClientEvent("whatIsVehAllowed", isVehRestricted);
            if (isVehRestricted) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Vehiclespawns are restricted." } });
            else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Vehiclespawns are unrestricted." } });
        }

        public static void sendRespawnLocationsDict(Player source)
        {
            foreach (KeyValuePair<string, Vector4> entry in respawnLocationsDict)
            {
                source.TriggerEvent("getRespawnLocationsDict", entry.Key, entry.Value);
            }
        }

        public static void sendMaxzzzieCalloutsDict(Player source)
        {
            foreach (KeyValuePair<string, Vector3> entry in maxzzzieCalloutsDict)
            {
                source.TriggerEvent("getMaxzzzieCalloutsDict", entry.Key, entry.Value);
            }
        }

        public static void sendVehicleinfoDict(Player source)
        {
            foreach (KeyValuePair<string, string> entry in vehicleinfoDict)
            {
                source.TriggerEvent("getVehicleinfoDict", entry.Key, entry.Value);
            }
        }



        [Command("getdiscordid", Restricted = true)]
        void get_id_handler()
        {
            foreach (Player p in Players)
            {
                CitizenFX.Core.Debug.WriteLine($"Name: {p.Name} | id: {p.Identifiers["discord"]}");
            }
        }
        bool isPlayerHost(Player p)
        {
            //https://docs.fivem.net/docs/scripting-reference/runtimes/lua/functions/GetPlayerIdentifiers/
            string players_discord_id = p.Identifiers["discord"];
            //p.identifier is requesting info of a player. In this case their discord.
            return allowed_discord_ids.Contains(players_discord_id);
        }


    }


    public class Misc : BaseScript

    {
        public static string AllowedToFixStatus = "on";
        public static int fixWaitTime = 10;
        public static bool isPodOn = true;

        public Misc()
        {
            UpdateLSCustomsBlips();
        }


        [Command("togglepod", Restricted = true)]
        void togglepod(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                if (!isPodOn)
                {
                    isPodOn = true;
                    API.StartResource("playernames");
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PlayerOverheadDisplay is now on." } });

                }
                else
                {
                    isPodOn = false;
                    API.StopResource("playernames");
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PlayerOverheadDisplay is now off." } });
                }
            }
            else if (args.Count == 1 && args[0].ToString() == "true")
            {
                isPodOn = true;
                API.StartResource("playernames");
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PlayerOverheadDisplay is now on." } });
            }
            else if (args.Count == 1 && args[0].ToString() == "false")
            {
                isPodOn = false;
                API.StopResource("playernames");
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PlayerOverheadDisplay is now off." } });
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglepod (true/false)");
            }
        }

        string lastFixStatus = "none";
        [Command("togglefix", Restricted = true)]
        void toggleFix(int source, List<object> args, string raw)
        {
            lastFixStatus = AllowedToFixStatus;
            if (args.Count == 1)
            {
                if (args[0].ToString() == "on" || args[0].ToString() == "off" || args[0].ToString() == "lsc" || args[0].ToString() == "wait")
                {
                    AllowedToFixStatus = args[0].ToString();
                    TriggerClientEvent("VehicleFixStatus", AllowedToFixStatus, fixWaitTime);
                    if (args[0].ToString() != "wait")
                    {
                        TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/fix status is now set to {args[0].ToString()}." } });
                    }
                    else
                    {
                        TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/fix status is now set to \"wait\" with a time of {fixWaitTime} seconds." } });
                    }
                }
                else if (args[0].ToString() == "help")
                {
                    CitizenFX.Core.Debug.WriteLine($"/togglefix sets the state of the /fix command. There are 4 options. \n/togglefix on | allows players to instantly fix their vehicle.\n/togglefix off | It turns the fix feature off.\n/togglefix lsc | Requires a player to drive to an LS customs for a fix.\n/togglefix wait (time in seconds) | Makes a player wait stationairy for a fix.");

                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglefix on/off/lsc/wait(and a value).");
                }
            }
            else if (args.Count == 2)
            {
                int temp = 0;
                string arg1 = args[1].ToString();
                bool a2 = int.TryParse(arg1, out temp);
                if (a2 && args[0].ToString() == "wait")
                {
                    AllowedToFixStatus = args[0].ToString();
                    fixWaitTime = int.Parse(args[1].ToString());
                    TriggerClientEvent("VehicleFixStatus", AllowedToFixStatus, fixWaitTime);
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/fix status is now set to \"wait\" with a time of {fixWaitTime} seconds." } });
                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglefix (on/off/lsc/wait(and a value))");
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglefix (on/off/lsc/wait(and a value)/)");
            }
            UpdateLSCustomsBlips();
        }

        public void UpdateLSCustomsBlips()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            if (lastFixStatus != "lsc" && AllowedToFixStatus == "lsc")
            {

                //TriggerClientEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"lsc requested a blip."}});
                BlipHandler.BlipData lsc1 = new BlipHandler.BlipData("lsc1")
                {
                    Coords = new Vector3(-337, -136, 39),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc2 = new BlipHandler.BlipData("lsc2")
                {
                    Coords = new Vector3(732, -1085, 22),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc3 = new BlipHandler.BlipData("lsc3")
                {
                    Coords = new Vector3(-1152, -2008, 13),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc4 = new BlipHandler.BlipData("lsc4")
                {
                    Coords = new Vector3(1178, 2638, 37),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc5 = new BlipHandler.BlipData("lsc5")
                {
                    Coords = new Vector3(107, 6624, 31),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc6 = new BlipHandler.BlipData("lsc6")
                {
                    Coords = new Vector3(-1538, -577, 25),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc7 = new BlipHandler.BlipData("lsc7")
                {
                    Coords = new Vector3(-415, -2179, 10),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc8 = new BlipHandler.BlipData("lsc8")
                {
                    Coords = new Vector3(1120, -779, 57),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc9 = new BlipHandler.BlipData("lsc9")
                {
                    Coords = new Vector3(1204, -3115, 5),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                BlipHandler.BlipData lsc10 = new BlipHandler.BlipData("lsc10")
                {
                    Coords = new Vector3(-213, -1327, 30),
                    Sprite = 402,
                    IsShortRange = true,
                    MapName = "LS customs"
                };
                request.BlipsToAdd.Add(lsc1);
                request.BlipsToAdd.Add(lsc2);
                request.BlipsToAdd.Add(lsc3);
                request.BlipsToAdd.Add(lsc4);
                request.BlipsToAdd.Add(lsc5);
                request.BlipsToAdd.Add(lsc6);
                request.BlipsToAdd.Add(lsc7);
                request.BlipsToAdd.Add(lsc8);
                request.BlipsToAdd.Add(lsc9);
                request.BlipsToAdd.Add(lsc10);
                BlipHandler.AddBlips(request);
            }
            else if (AllowedToFixStatus != "lsc" && lastFixStatus == "lsc")
            {
                request.BlipsToRemove.Add("lsc1");
                request.BlipsToRemove.Add("lsc2");
                request.BlipsToRemove.Add("lsc3");
                request.BlipsToRemove.Add("lsc4");
                request.BlipsToRemove.Add("lsc5");
                request.BlipsToRemove.Add("lsc6");
                request.BlipsToRemove.Add("lsc7");
                request.BlipsToRemove.Add("lsc8");
                request.BlipsToRemove.Add("lsc9");
                request.BlipsToRemove.Add("lsc10");
                BlipHandler.AddBlips(request);
            }
        }

        [Command("clear", Restricted = true)] //restriction (default true)
        async void clear(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                TriggerClientEvent("clear_vehicles", false);
            }
            else if (args.Count == 1 && (args[0].ToString() == "all"))
            {
                TriggerClientEvent("clear_vehicles", true);
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine($"This option clears vehicles and entities.\nType \"/clear\" to clear vehicles and \"/clear all\" to clear all entities.");
            }
            didClearJustHappen();
        }
        [EventHandler("didClearJustHappen")] //upon removing pri's this event get's called by the client. This to prevent the msg's not popping up due to ping. As the pri check happens too quick (OnTick).
        async void didClearJustHappen()
        {
            await Delay(1000);
            PriusMechanics.didClearJustHappen = true;
        }
    }
    public class Armoury : BaseScript

    {
        public static bool isWeaponsAllowed = true;
        public static bool isPvpAllowed = true;
        public static bool isShootingFromVehicleAllowed = false;


        [Command("togglesfv", Restricted = true)] //restriction (default true)
        void toggleShootingFromVehicle(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                isShootingFromVehicleAllowed = !isShootingFromVehicleAllowed;
                TriggerClientEvent("disableCanPlayerShootFromVehicles", isShootingFromVehicleAllowed);
                if (isShootingFromVehicleAllowed)
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Shooting from vehicles is now enabled." } });
                }
                else
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Shooting from vehicles is now disabled." } });
                }
            }
            else if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
            {
                isShootingFromVehicleAllowed = bool.Parse(args[0].ToString());
                TriggerClientEvent("disableCanPlayerShootFromVehicles", isShootingFromVehicleAllowed);
                if (isShootingFromVehicleAllowed)
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Shooting from vehicles is now enabled." } });
                }
                else
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Shooting from vehicles is now disabled." } });
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine($"Shooting from vehicle is set to: {isShootingFromVehicleAllowed}. The right command is: /togglesfv (true/false)");
            }
        }

        [Command("toggleweapon", Restricted = true)]
        void toggleweapon(int source, List<object> args, string raw)
        {
            if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
            {
                isWeaponsAllowed = bool.Parse(args[0].ToString());
                TriggerClientEvent("isWeaponAllowed", isWeaponsAllowed);
                if (isWeaponsAllowed)
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Weapons are enabled." } });
                }
                else
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Weapons are disabled." } });
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /toggleweapon (true/false)");
            }
        }


        [Command("togglepvp", Restricted = true)]
        void togglepvp(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                isPvpAllowed = !isPvpAllowed;
                if (isPvpAllowed)
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PVP is now enabled." } });
                }
                else
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PVP is now disabled." } });
                }
            }
            else if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
            {
                isPvpAllowed = bool.Parse(args[0].ToString());
                if (isPvpAllowed)
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PVP is now enabled." } });
                }
                else
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PVP is now disabled." } });
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglepvp (true/false)");
            }
            TriggerClientEvent("updatePvp", isPvpAllowed);
        }
    }



    public class MapBounds : BaseScript
    // a few blip color id's :1=red 2=green 3=lightblue 4=white 5=yellow 6=lighterRed 7=lila 8=purple/pink 64=orange 69=lime Find the rest here https://docs.fivem.net/docs/game-references/blips/
    {
        static List<float[]> argArrayList = new List<float[]>();
        //argArrayList stores the input values for the map blips so players that join late can sync the map bounds.
        public static Dictionary<string, List<Vector3>> mapBoundsDict;
        int argColor = 4; //initial default circle colour

        public MapBounds()
        {
            mapBoundsDict = LoadResources.mapBounds();
            CitizenFX.Core.Debug.WriteLine($"I'm making a MapBounds dictionary.");
        }

        //updates the player's circles.
        [EventHandler("updateCircle")]
        public static void updateCircle(bool isPlayerJoining)
        {
            TriggerClientEvent("delCircle");

            foreach (float[] argArray in argArrayList)
            {
                TriggerClientEvent("updateCircle", argArray);
            }
            if (!isPlayerJoining)
            {
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Mapbounds have been updated." } });
            }
        }


        //adds circle to host and then updates the client.
        [Command("circle", Restricted = true)] //restriction (default true)
        void circle(int source, List<object> args, string raw)
        {
            //CitizenFX.Core.Debug.WriteLine($"server circle 1");
            if (args.Count <= 1 && args.Count >= 4)
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do \"/circle help\" for more info.");
                return;
            }

            else if (args.Count == 2 && args[0].ToString() == "color")
            {
                int temp;
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
                if (isArgs1Int)
                {
                    argColor = Int32.Parse(args[1].ToString());
                    return;
                }
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do \"/circle help\" for more info.");
            }
            if (args.Count == 4)
            {
                //temp because function needs a return. Is not used other then to return something
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
                bool isArgs2Int = Int32.TryParse(args[2].ToString(), out temp);
                bool isArgs3Int = Int32.TryParse(args[3].ToString(), out temp);

                if (isArgs0Int == false || isArgs1Int == false || isArgs2Int == false || isArgs3Int == false)
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do \"/circle help\" for assistance.");
                    return;
                }
                float argX = float.Parse(args[0].ToString());
                float argY = float.Parse(args[1].ToString());
                float argRadius = float.Parse(args[2].ToString());
                argColor = int.Parse(args[3].ToString());
                CitizenFX.Core.Debug.WriteLine("The server recieved cords for a circle.");
                float[] argArray = { argX, argY, argRadius, argColor };
                argArrayList.Add(argArray);
                updateCircle(false);
            }

            else if (args.Count == 3)
            {
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
                bool isArgs2Int = Int32.TryParse(args[2].ToString(), out temp);

                if (isArgs0Int == false || isArgs1Int == false || isArgs2Int == false)
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do \"/circle help\" for assistance.");
                    return;
                }
                float argX = float.Parse(args[0].ToString());
                float argY = float.Parse(args[1].ToString());
                float argRadius = float.Parse(args[2].ToString());
                int argColor = int.Parse(args[3].ToString());
                float[] argArray = { argX, argY, argRadius, argColor };
                argArrayList.Add(argArray);
                updateCircle(false);
            }

            else if (args.Count == 1)
            {
                string arg0 = args[0].ToString();
                //CitizenFX.Core.Debug.WriteLine($"locationname :{arg0}");
                //CitizenFX.Core.Debug.WriteLine($"dict count {mapBoundsDict.Count}\nThat's how many maps are loaded in dictionairy.");


                if (mapBoundsDict.ContainsKey(arg0))
                {
                    //CitizenFX.Core.Debug.WriteLine("The server recieved the location for a circle.");
                    //CitizenFX.Core.Debug.WriteLine($"Map is made out of {mapBoundsDict[arg0].Count} circles.");
                    List<Vector3> mapBoundsLocationList = mapBoundsDict[arg0];
                    foreach (Vector3 location in mapBoundsLocationList)
                    {
                        float mapX = location.X;
                        float mapY = location.Y;
                        float mapRadius = location.Z;
                        float[] argArray = { mapX, mapY, mapRadius, argColor };
                        argArrayList.Add(argArray);
                    }
                    updateCircle(false);
                    CitizenFX.Core.Debug.WriteLine($"Added {arg0} to your circles.");
                }
                else if (arg0 == "list")
                {
                    var combined = string.Join("\n", mapBoundsDict.Keys);
                    CitizenFX.Core.Debug.WriteLine("All avalible preset circle locations are:\n" + combined);
                }
                else if (arg0 == "help")
                {
                    CitizenFX.Core.Debug.WriteLine($"\"/circle\" gives you all options to set mapbounds using (a set of) circles. \nThere is a bunch of preset mapbounds embedded in the server. Type \"/circle list\" to see a list of all avalible presets. \nNote, some names are made shorter for ease of use (vespucci --> ves). \nManual placement of circles uses \"/circle x y radius colorId(0-100)\". \nIf no colorId is defined it will pick the last used one. Change the color manualy using \"/circle color colorIdValue\".");
                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine($"Oh no. Something went wrong!\n{arg0} isn't in the list of avalible mapbounds. Type \"/circle list\" for all mapbounds.");
                }

            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do \"/circle help\" for assistance.");
            }
        }



        //remove circle options
        [Command("delcircle", Restricted = true)] //restriction (default true)
        void delCircle(int source, List<object> args, string raw)
        {

            if (argArrayList.Count == 0)
            {
                CitizenFX.Core.Debug.WriteLine("There is no circle to delete.");
                return;
            }
            if (args.Count == 1 && (args[0].ToString() == "all" || args[0].ToString() == "first" || args[0].ToString() == "last"))
            {
                CitizenFX.Core.Debug.WriteLine($"Delcircle {args[0].ToString()} command got recieved by the server.");
                if (args[0].ToString() == "last")
                {
                    argArrayList.RemoveAt(argArrayList.Count - 1);
                    updateCircle(false);
                }
                else if (args[0].ToString() == "first")
                {
                    argArrayList.RemoveAt(0);
                    updateCircle(false);
                }
                else if (args[0].ToString() == "all")
                {
                    argArrayList.Clear();
                    CitizenFX.Core.Debug.WriteLine("All circles are deleted.");
                    updateCircle(false);
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /delcircle (first/ last/ all)");
            }
        }
    }


    public class Teleports : BaseScript
    {

        public static bool isPlayerAllowedToTp = true;
        public static Dictionary<string, Vector3> tpLocationsDict;


        public Teleports()
        {
            tpLocationsDict = LoadResources.tpLocations();
            CitizenFX.Core.Debug.WriteLine($"I'm making a tp location dictionary.");
        }

        [Command("tpall", Restricted = true)] //restriction (default true)
        void tpAll(int sourceId, List<object> args, string raw)
        {
            Player source = Players[sourceId];
            if (args.Count == 1)
            {
                string tpAllName = args[0].ToString();
                if (tpLocationsDict.ContainsKey(tpAllName) == false)
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYour location is invalid. Type /tplocations for all avalible locations.");
                }

                else
                {
                    var tpVector = tpLocationsDict[tpAllName];
                    TriggerClientEvent("tpPlayerRand", tpVector.X, tpVector.Y, tpVector.Z);
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 153, 153 },
                        args = new[] { $"All players have been teleported to {tpAllName} by {source.Name}."
                        }
                    });
                }
            }

            else if (args.Count == 3)
            {
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
                bool isArgs2Int = Int32.TryParse(args[2].ToString(), out temp);
                //the code above checks if args[0-2] are ints. They need to output something so we do that to a temp variable.
                if (args.Count == 3 && isArgs0Int == true && isArgs1Int == true && isArgs2Int == true)
                {
                    TriggerClientEvent("tpPlayerRand", int.Parse(args[0].ToString()), int.Parse(args[1].ToString()), int.Parse(args[2].ToString()));
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"All players have been teleported to some very specific coords by {source.Name}." } });
                    return;
                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /tpall \"location\"/ (x y z). \nType /tplocations for all avalible locations.");
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /tpall \"location\"/ (x y z). \nType /tplocations for all avalible locations.");
            }
        }

        [Command("tp", Restricted = false)]
        void tpHandler(int sourceid, List<object> args, string raw)
        {
            if (!isPlayerAllowedToTp) return;

            Player source = Players[sourceid];

            if (args.Count != 1 && args.Count != 3)
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /tp \"location\"/ (x y z). \nType /tplocations for all avalible locations.");
                return;
            }
            if (args.Count == 1)
            {
                string tpLocationName = args[0].ToString();
                if (tpLocationsDict.ContainsKey(tpLocationName) == false)
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYour location is invalid. Type /tplocations for all avalible locations.");
                    return;
                }
                var tpVector = tpLocationsDict[tpLocationName];
                source.TriggerEvent("tpPlayer", tpVector.X, tpVector.Y, tpVector.Z);
                CitizenFX.Core.Debug.WriteLine($"You have been teleported to {tpLocationName}.");
                return;
            }
            int temp;
            bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
            bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
            bool isArgs2Int = Int32.TryParse(args[2].ToString(), out temp);
            //the code above checks if args[0-2] are ints. They need to output something so we do that to a temp variable.
            if (args.Count == 3 && isArgs0Int == true && isArgs1Int == true && isArgs2Int == true)
            {
                source.TriggerEvent("tpPlayer", int.Parse(args[0].ToString()), int.Parse(args[1].ToString()), int.Parse(args[2].ToString()));
                CitizenFX.Core.Debug.WriteLine($"You teleported yourself to specific coords.");
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /tp \"location\"/ (x y z). \nType /tplocations for all avalible locations.");
            }
        }

        //toggle personal teleport
        [Command("toggletp", Restricted = true)] //restriction (default true)
        void toggletp(int source, List<object> args, string raw)
        {
            // Player source = Players[sourceId];
            if (args.Count == 1 && (args[0].ToString() == "true" || args[0].ToString() == "false"))
            {
                isPlayerAllowedToTp = bool.Parse(args[0].ToString());
                if (isPlayerAllowedToTp)
                {
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Personal teleports are now enabled." } });
                }
                else
                {
                    // TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{source.Name} just disabled personal teleports." } });
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Personal teleports are now disabled." } });

                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /toggletp (true/false)");
            }
        }

        [Command("tplocations")]
        void tplocations(int source, List<object> args, string raw)
        {
            Player player = Players[source];
            var combined = string.Join(", ", tpLocationsDict.Keys);
            TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Locations: {combined}" } });
        }
    }

    public class Appearance : BaseScript
    {
        static List<string> nonAnimalModel = new List<string>();

        public Appearance()
        {
            //load nonAnimalModels to list NonAnimalModel
            nonAnimalModel = LoadResources.loadNonAnimalModels();
        }

        [EventHandler("sendNonAnimalModel")]
        public static void sendNonAnimalModel(Player source)
        {
            foreach (string model in nonAnimalModel)
            {
                source.TriggerEvent("getNonAnimalModelList", model);

            }
        }
    }

    public class Vehicles : BaseScript
    {
        bool vehicleShouldChangePlayerColour = true;
        bool vehicleShouldNotDespawn = true;
        public static Dictionary<string, string> vehicleColourForPlayer = new Dictionary<string, string>();


        public Vehicles()
        {
            vehicleColourForPlayer = LoadResources.playerVehicleColour();
        }

        [Command("clientveh", Restricted = true)] //restriction default = true
        void clientveh(int source, List<object> args, string raw)
        {
            Player sourceHost = Players[source];
            if (args.Count == 1) //this sends a vehicle to all clients
            {
                TriggerClientEvent("clientVeh", args[0].ToString(), false, sourceHost.Name);
                TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"You tried spawning a {args[0].ToString()} for everyone." } });
            }
            else if (args.Count == 2) //this sends a vehicle to a specific client
            {
                int temp;
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
                if (isArgs1Int)
                {
                    int clientID = int.Parse(args[1].ToString());
                    Player player = Players[clientID];
                    TriggerClientEvent(player, "clientVeh", args[0].ToString(), true, sourceHost.Name);
                    TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"You tried spawning a {args[0].ToString()} for {player.Name}" } });
                }
                else
                {
                    TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong!\nYou should do /clientveh \"VehicleName\" \"clientID/ Empty for all.\"" } });

                }
            }
            else
            {
                TriggerClientEvent(sourceHost, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { $"Something went wrong!\nYou should do /clientveh \"VehicleName\" \"clientID/ Empty for all.\"" } });
            }
        }

        [EventHandler("requestVehicleColor")]
        private void requestVehicleColor([FromSource] Player player)
        {
            string Name = player.Name;
            if (Vehicles.vehicleColourForPlayer.ContainsKey(Name))
            {
                string line = Vehicles.vehicleColourForPlayer[Name];
                string[] parts = line.Split(',');
                int primairyColour = int.Parse(parts[1].Trim());
                int secundaryColour = int.Parse(parts[2].Trim());
                int pearlescentColour = int.Parse(parts[3].Trim());
                int lightsR = int.Parse(parts[4].Trim());
                int lightsG = int.Parse(parts[5].Trim());
                int lightsB = int.Parse(parts[6].Trim());
                TriggerClientEvent(player, "receiveVehicleColor", primairyColour, secundaryColour, pearlescentColour, lightsR, lightsG, lightsB);
            }
            else
            {
                TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Your name isn't linked to a vehicle colour yet. Ask the host to add it." } });
            }
        }

        [Command("togglepvd", Restricted = true)]
        void setVehicleShouldNotDespawn(int source, List<object> args, string raw)
        {
            Player player = Players[source];

            if (args.Count == 0)
            {
                vehicleShouldNotDespawn = !vehicleShouldNotDespawn;
                if (vehicleShouldNotDespawn) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Player vehicles will no longer despawn." } });
                else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Player vehicles will now despawn." } });
            }
            else if (args.Count == 1)
            {
                string toggleArg = args[0].ToString().ToLower();

                if (toggleArg == "true" || toggleArg == "on")
                {
                    vehicleShouldNotDespawn = true;
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Player vehicles will no longer despawn." } });
                }
                else if (toggleArg == "false" || toggleArg == "off")
                {
                    vehicleShouldNotDespawn = false;
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Player vehicles will now despawn." } });
                }
                else
                {
                    TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "Something went wrong. Do /togglepvd \"true/false\" to toggle vehicle player vehicle despawning." } });
                }

            }
            else TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "Something went wrong. Do /togglepvd \"true/false\" to toggle vehicle player vehicle despawning." } });

            TriggerClientEvent("updateClientColourAndDespawn", vehicleShouldChangePlayerColour, vehicleShouldNotDespawn);

        }

        [Command("togglepvc", Restricted = true)]
        void setVehicleShouldChangePlayerColour(int source, List<object> args, string raw)
        {
            Player player = Players[source];

            if (args.Count == 0)
            {
                vehicleShouldChangePlayerColour = !vehicleShouldChangePlayerColour;
                if (vehicleShouldChangePlayerColour) TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Vehicles will now change to the drivers colour." } });
                else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Vehicles will no longer change to the drivers colour." } });
                TriggerClientEvent("updateClientColourAndDespawn", vehicleShouldChangePlayerColour, vehicleShouldNotDespawn);
            }
            else if (args.Count == 1)
            {
                string toggleArg = args[0].ToString().ToLower();

                if (toggleArg == "true" || toggleArg == "on")
                {
                    vehicleShouldChangePlayerColour = true;
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Vehicles will now change to the drivers colour." } });
                }
                else if (toggleArg == "false" || toggleArg == "off")
                {
                    vehicleShouldChangePlayerColour = false;
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { "Vehicles will no longer change to the drivers colour." } });
                }

                else TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "Something went wrong. Do /togglepvc \"true/false\" or \"playerID, primary colour id, secondary colour id\"" } });


                TriggerClientEvent("updateClientColourAndDespawn", vehicleShouldChangePlayerColour, vehicleShouldNotDespawn);

            }
            else if (args.Count == 7)
            {
                int playerId = (args[0].ToString() == "x") ? -1 : int.TryParse(args[0].ToString(), out int parsedPlayerId) ? parsedPlayerId : -1;
                int primairyColour = (args[1].ToString() == "x") ? -1 : int.TryParse(args[1].ToString(), out int parsedPrimairyColour) ? parsedPrimairyColour : -1;
                int secundaryColour = (args[2].ToString() == "x") ? -1 : int.TryParse(args[2].ToString(), out int parsedSecundaryColour) ? parsedSecundaryColour : -1;
                int pearlescentColour = (args[3].ToString() == "x") ? -1 : int.TryParse(args[3].ToString(), out int parsedPearlescentColour) ? parsedPearlescentColour : -1;
                int lightsR = (args[4].ToString() == "x") ? -1 : int.TryParse(args[4].ToString(), out int parsedLightsR) ? parsedLightsR : -1;
                int lightsG = (args[5].ToString() == "x") ? -1 : int.TryParse(args[5].ToString(), out int parsedLightsG) ? parsedLightsG : -1;
                int lightsB = (args[6].ToString() == "x") ? -1 : int.TryParse(args[6].ToString(), out int parsedLightsB) ? parsedLightsB : -1;
                string playerName = Players[playerId].Name;

                if (playerId != -1 && Vehicles.vehicleColourForPlayer.ContainsKey(Players[playerId].Name)) //player exists. And values might be -1 to be unchanged. Read from the original colour save.
                {
                    string[] DataLine = Vehicles.vehicleColourForPlayer[playerName].Split(',');
                    if (primairyColour == -1)
                    {
                        primairyColour = int.Parse(DataLine[1]);
                    }
                    if (secundaryColour == -1)
                    {
                        secundaryColour = int.Parse(DataLine[2]);
                    }
                    if (pearlescentColour == -1)
                    {
                        pearlescentColour = int.Parse(DataLine[3]);
                    }
                    if (lightsR == -1)
                    {
                        lightsR = int.Parse(DataLine[4]);
                    }
                    if (lightsG == -1)
                    {
                        lightsG = int.Parse(DataLine[5]);
                    }
                    if (lightsB == -1)
                    {
                        lightsB = int.Parse(DataLine[6]);
                    }
                    AddOrUpdatePlayerVehicleColour(playerName, primairyColour, secundaryColour, pearlescentColour, lightsR, lightsG, lightsB);
                    TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 0, 255, 0 }, args = new[] { "Updating a players colour." } });
                }
                else if (playerId != -1) //player doesn't exist yet.
                {
                    AddOrUpdatePlayerVehicleColour(playerName, primairyColour, secundaryColour, pearlescentColour, lightsR, lightsG, lightsB);
                    TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 0, 255, 0 }, args = new[] { "Setting a new player colour." } });
                }
                else
                {
                    TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "Something went wrong. Do /togglepvc \"true/false\" or \"playerID, primary colour id, secondary, pearlescent, lights r, g, b\"\nValue can be \"x\" as well to keep the existing colour." } });
                }
            }
            else
            {
                TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "Something went wrong. Do /togglepvc \"true/false\" or \"playerID, primary colour id, secondary, pearlescent, lights r, g, b\"" } });
            }
        }

        private void AddOrUpdatePlayerVehicleColour(string playerName, int primairyColour, int secundaryColour, int pearlescentColour, int lightsR, int lightsG, int lightsB)
        {
            vehicleColourForPlayer[playerName] = $"{playerName},{primairyColour},{secundaryColour},{pearlescentColour},{lightsR},{lightsG},{lightsB}";

            LoadResources.SavePlayerVehicleColours(vehicleColourForPlayer);
            updateClientsVehicleColours();
        }

        void updateClientsVehicleColours()
        {
            foreach (Player player in Players)
            {
                string name = player.Name;
                if (Vehicles.vehicleColourForPlayer.ContainsKey(name))
                {
                    string line = Vehicles.vehicleColourForPlayer[name];
                    string[] parts = line.Split(',');
                    int primairyColour = int.Parse(parts[1].Trim());
                    int secundaryColour = int.Parse(parts[2].Trim());
                    int pearlescentColour = int.Parse(parts[3].Trim());
                    int lightsR = int.Parse(parts[4].Trim());
                    int lightsG = int.Parse(parts[5].Trim());
                    int lightsB = int.Parse(parts[6].Trim());
                    TriggerClientEvent(player, "receiveVehicleColor", primairyColour, secundaryColour, pearlescentColour, lightsR, lightsG, lightsB);
                }
            }
        }

        [Command("stuck", Restricted = false)]
        void stuckCommand([FromSource] Player source, List<object> args, string raw)
        {
            TriggerClientEvent(source, "StuckCommand");
        }
    }
}




// private string[] deathMsgs = new string[] { "{0} has died.", "{0} isn't.", "Rest in peace {0}?" };
// private string[] killedMsgs = new string[] { "{1} killed {0}.", "{0} ended {1}.", "A conflict happened between {0} and {1}. {0} came out worse." };
// private string[] killSelfMessages = new string[] { "{0} ended their own life.", "Suicide: {0}", "{0} couldn't handle it anymore." };
// private string[] killedByPlayerMessages = new string[] { "{1} killed {0}.", "{0} was taken out by {1}.", "{0} met their demise at the hands of {1}." };


// private void OnPlayerDied([FromSource] Player source)
// {
//     string message = string.Format(deathMsgs[new Random().Next(0, deathMsgs.Length)], source.Name);
//     TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { message } });
// }

// private void OnPlayerKilled([FromSource] Player killer, string reason)
// {
//     Player victim = Players[killer.Handle];

//     string message;

//     if (killer.Handle == victim.Handle)
//     {
//         message = string.Format(killSelfMessages[new Random().Next(0, killSelfMessages.Length)], victim.Name);
//     }
//     else
//     {
//         message = string.Format(killedByPlayerMessages[new Random().Next(0, killedByPlayerMessages.Length)], victim.Name, killer.Name);
//     }

//     TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { message } });
// }




