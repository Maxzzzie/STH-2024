using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;


namespace STHMaxzzzie.Server
{
    public class ServerMain : BaseScript
    {
        // this is a constructor because it has same name as class
        // its a function that runs when gamemode starts
        static List<string> allowed_discord_ids = new List<string>();
        public static Dictionary<string, Vector4> respawnLocationsDict;
        public static Dictionary<string, string> vehicleinfoDict;

        private Dictionary<Player, int> playerPris = new Dictionary<Player, int>();

        public ServerMain()
        {
            //load whitelist to list allowed_discord_ids
            allowed_discord_ids = LoadResources.loadWhitelist();

            //add ace permissions
            //remove if permissions still work... string[] commands = { "toggleweapon", "circle", "delcircle", "tpall", "toggletp" };
            foreach (string id in allowed_discord_ids)
            {
                API.ExecuteCommand($"add_ace identifier.discord:{id} \"command\" allow");
            }

            //get spawnlocations
            respawnLocationsDict = LoadResources.respawnLocations();
            vehicleinfoDict = LoadResources.allowedVehicles();

            Tick += OnTick;
        }

        [EventHandler("pri-spawn-requested")]
        void OnPriRequested([FromSource] Player player, uint vehicleHash, Vector3 position, float heading)
        {
            if (playerPris.ContainsKey(player))
            {
                int oldPriusHandle = playerPris[player];
                if (API.DoesEntityExist(oldPriusHandle))
                {

                    API.DeleteEntity(oldPriusHandle);
                }
                playerPris.Remove(player); // Remove the old vehicle from the dictionary
            }
            
            int vehicle = API.CreateVehicle(vehicleHash, position.X, position.Y, position.Z, heading, true, false); // Vehicle Hash gotten from VehicleHash on client, for some reason not available on server?

            TriggerClientEvent("chat:addMessage", new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "Server", $"{player.Name} is spawning a Prius!" }
            });

            API.SetVehicleColours(vehicle, 135, 135);
            API.SetVehicleNumberPlateText(vehicle, $"{player.Name}");            

            playerPris.Add(player, vehicle);
        }

        [Tick]
        private Task OnTick()
        {
            CheckPriStatus();
            return Task.CompletedTask;
        }

        private void CheckPriStatus()
        {
            List<Player> keysToRemove = new List<Player>();
            foreach(var playerPri in playerPris)
            {
                float health = API.GetVehicleEngineHealth(playerPri.Value);
                if (health <= 0)
                {
                    Debug.WriteLine($"{playerPri.Key.Name}'s Pri got destroyed!");
                    keysToRemove.Add(playerPri.Key);
                    TriggerClientEvent(playerPri.Key, "chat:addMessage", new
                    {
                        color = new[] { 0, 255, 0 }, // Green color for the message
                        multiline = false,
                        args = new[] { "Server", "Your pri got destroyed!" }
                    });
                }
            }
            foreach (var key in keysToRemove)
                playerPris.Remove(key);
        }


        //https://docs.fivem.net/docs/scripting-reference/events/server-events/#playerjoining
        [EventHandler("playerJoining")]
        void playerJoiningHandler([FromSource] Player source, string old_id)
        {
            ServerMain.sendRespawnLocationsDict(source);
            ServerMain.sendVehicleinfoDict(source);
            MapBounds.updateCircle();
            source.TriggerEvent("isWeaponAllowed", Armoury.isWeaponsAllowed);
            source.TriggerEvent("updatePvp", Armoury.isPvpAllowed);
            source.TriggerEvent("disableCanPlayerShootFromVehicles", Armoury.isShootingFromVehicleAllowed);
            Appearance.sendNonAnimalModel(source);
            source.TriggerEvent("respawnPlayer");
            source.TriggerEvent("VehicleFixStatus", Misc.AllowedToFixStatus, Misc.fixWaitTime);
        }


        public static void sendRespawnLocationsDict(Player source)
        {
            foreach (KeyValuePair<string, Vector4> entry in respawnLocationsDict)
            {
                source.TriggerEvent("getRespawnLocationsDict", entry.Key, entry.Value);
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

    // public class Test : BaseScript
    // {
    //     [Command("test", Restricted = false)]

    //     void test(int source, List<object> args, string raw)
    //     {
    //         CitizenFX.Core.Debug.WriteLine($"This is the test command.");
    //     }
    // }

    // public class Notifications : BaseScript
    // {
    //     public Notifications()
    //     {
    //         EventHandlers["notification"] += new Action<string, string>(ShowNotification);
    //     }

    //     private void ShowNotification(string message, string type)
    //     {
    //         API.SetNotificationTextEntry("STRING");
    //         API.AddTextComponentString(message);
    //         API.SetNotificationMessage(type, type, true, 0, false, "");
    //         API.DrawNotification(false, true);
    //     }

    //     public static void DisplayNotification(string message)
    //     {
    //         TriggerClientEvent("notification", message, "success");
    //     }
    // }



    public class Misc : BaseScript

    {
        public static string AllowedToFixStatus = "on";
        public static int fixWaitTime = 10;


        [Command("togglepod", Restricted = false)]
        void toggleweapon(int source, List<object> args, string raw)
        {
            if (args.Count == 1 && args[0].ToString() == "on")
            {
                API.StartResource("playernames");
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PlayerOverheadDisplay is now on." } });
            }
            else if (args.Count == 1 && args[0].ToString() == "off")
            {
                API.StopResource("playernames");
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"PlayerOverheadDisplay is now off." } });
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglepod (on/off)");
            }
        }

        [Command("togglefix", Restricted = false)]
        void toggleFix(int source, List<object> args, string raw)
        {
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
                    CitizenFX.Core.Debug.WriteLine($"/togglefix sets the state of the /fix command. There are 4 options. \n/fix on | allows players to instantly fix their vehicle.\n/fix off | means what it says. It turns the feature off.\n/fix lsc | requires a player to drive to an ls customs and type the command.\n/fix wait (time in seconds) | Makes a player wait stationairy for a fix.");

                }
                else
                {
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglefix on/off//lsc/wait(and a value).");
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
                    CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglefix (on/off/wait(and a value)/)");
                }
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /togglefix (on/off/wait(and a value)/)");
            }
        }

        [Command("clear", Restricted = false)] //restriction (default true)
        void clear(int source, List<object> args, string raw)
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
        }
    }

    public class Armoury : BaseScript

    {
        public static bool isWeaponsAllowed = true;
        public static bool isPvpAllowed = true;
        public static bool isShootingFromVehicleAllowed = false;

        [Command("togglesfv", Restricted = false)] //restriction (default true)
        void toggleShootingFromVehicle(int source, List<object> args, string raw)
        {

            if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
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
                CitizenFX.Core.Debug.WriteLine($"This command toggle's shooting from vehicle. It is currently set to: {isShootingFromVehicleAllowed}\nThe right command is: /togglesfv (true/false)");
            }
        }

        [Command("toggleweapon", Restricted = false)]
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

        [Command("togglepvp", Restricted = false)]
        void togglepvp(int source, List<object> args, string raw)
        {
            if (args.Count == 1 && (args[0].ToString() == "false" || args[0].ToString() == "true"))
            {
                isPvpAllowed = bool.Parse(args[0].ToString());
                TriggerClientEvent("updatePvp", isPvpAllowed);
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
        }
    }



    public class MapBounds : BaseScript
    // a few blip color id's :1=red 2=green 3=lightblue 4=white 5=yellow 6=lighterRed 7=lila 8=purple/pink 64=orange 69=lime
    {
        static List<float[]> argArrayList = new List<float[]>();
        //argArrayList stores the input values for the blips so players that join late can sync the map bounds.
        public static Dictionary<string, List<Vector3>> mapBoundsDict;
        int argColor = 5;

        public MapBounds()
        {
            mapBoundsDict = LoadResources.mapBounds();
            CitizenFX.Core.Debug.WriteLine($"I'm making a MapBounds dictionary.");
        }

        //updates the player's circles.
        [EventHandler("updateCircle")]
        public static void updateCircle()
        {
            TriggerClientEvent("delCircle");

            foreach (float[] argArray in argArrayList)
            {
                TriggerClientEvent("updateCircle", argArray);
            }
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Mapbounds have been updated." } });
        }


        //adds circle to host and then updates the client.
        [Command("circle", Restricted = false)] //restriction (default true)
        void circle(int source, List<object> args, string raw)
        {
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
                CitizenFX.Core.Debug.WriteLine("2nd Oh no. Something went wrong!\nYou should do \"/circle help\" for more info.");
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
                    CitizenFX.Core.Debug.WriteLine("3rd Oh no. Something went wrong!\nYou should do \"/circle help\" for assistance.");
                    return;
                }
                float argX = float.Parse(args[0].ToString());
                float argY = float.Parse(args[1].ToString());
                float argRadius = float.Parse(args[2].ToString());
                argColor = int.Parse(args[3].ToString());
                CitizenFX.Core.Debug.WriteLine("The server recieved cords for a circle.");
                float[] argArray = { argX, argY, argRadius, argColor };
                argArrayList.Add(argArray);
                updateCircle();
            }

            else if (args.Count == 3)
            {
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);
                bool isArgs2Int = Int32.TryParse(args[2].ToString(), out temp);

                if (isArgs0Int == false || isArgs1Int == false || isArgs2Int == false)
                {
                    CitizenFX.Core.Debug.WriteLine("4th Oh no. Something went wrong!\nYou should do \"/circle help\" for assistance.");
                    return;
                }
                float argX = float.Parse(args[0].ToString());
                float argY = float.Parse(args[1].ToString());
                float argRadius = float.Parse(args[2].ToString());
                int argColor = int.Parse(args[3].ToString());
                float[] argArray = { argX, argY, argRadius, argColor };
                argArrayList.Add(argArray);
                updateCircle();
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
                    updateCircle();
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
        [Command("delcircle", Restricted = false)] //restriction (default true)
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
                    updateCircle();
                }
                else if (args[0].ToString() == "first")
                {
                    argArrayList.RemoveAt(0);
                    updateCircle();
                }
                else if (args[0].ToString() == "all")
                {
                    argArrayList.Clear();
                    CitizenFX.Core.Debug.WriteLine("All circles are deleted.");
                    updateCircle();
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

        [Command("tpall", Restricted = false)] //restriction (default true)
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
                        args = new[] { $"All players have been teleported to {tpAllName}\nBlame {source.Name}" }
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
                    TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"All players have been teleported to some very specific coords \nBlame {source.Name}" } });
                    return;
                }
                CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /tpall \"location\"/ (x y z). \nType /tplocations for all avalible locations.");
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
            CitizenFX.Core.Debug.WriteLine("Oh no. Something went wrong!\nYou should do /tp \"location\"/ (x y z). \nType /tplocations for all avalible locations.");
        }

        //toggle personal teleport
        [Command("toggletp", Restricted = false)] //restriction (default true)
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
        void tplocations()
        {
            var combined = string.Join(", ", tpLocationsDict.Keys);
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Locations: {combined}" } });
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

    // public class DeathNotification : BaseScript
    // {
    //     public DeathNotification()
    //     {
    //         EventHandlers["baseevents:onPlayerDied"] += new Action<Player>(OnPlayerDied);
    //         EventHandlers["baseevents:onPlayerKilled"] += new Action<Player, string>(OnPlayerKilled);
    //     }

    // }
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




//         "{1} killed {0}.",
//         "{0} ended up at the hands of {1}.",
//         "A conflict happened between {0} and {1}. {0} came out worse.",
//         "{1} dealt the final blow to {0}.",
//         "{1} has taken out {0}.",
//         "{1} has killed {0}.",
// "{1} has ended {0}.",
// "{0} was taken down by {1}.",
// "{0} met their end at the hands of {1}.",
// "A conflict happened between {0} and {1}. {0} came out worse.",
// "{1} emerged victorious over {0}.",
// "{0} was defeated by {1}.",
// "{1} put an end to {0}.",
// "{1} proved to be too much for {0}.",
// "{1} took out {0}.",
// "{1} got the best of {0}.",
// "{1} claimed the life of {0}.",
// "{0} was slain by {1}.",
// "{1} dealt the fatal blow to {0}.",
// "{1} overpowered {0}.",
// "{1} executed {0}.",
// "{1} ended {0}'s life.",
// "{1} triumphed over {0}.",
// "{1} was the last one standing after the fight with {0}.",
// "{1} emerged as the victor in the battle with {0}.",
// "{0} fell to {1}'s might.",
// "{1} delivered the killing blow to {0}.",
// "{1} defeated {0} in combat.",
// "{1} put an end to {0}'s reign of terror.",
// "{1} took down {0} in a fierce struggle.",
// "{1} vanquished {0} in battle.",
// "{1} dispatched {0} with ease.",
// "{1} eliminated {0} with lethal force.",
// "{1} ended {0}'s time in this world.",
// "{1} sent {0} to meet their maker.",
// "{1} took {0} out of the picture.",
// "{0} was no match for {1}'s strength.",
// "{1} made quick work of {0}.",
// "{1} outmatched {0} in the fight.",
// "{1} emerged victorious against {0}'s onslaught.",
// "{1} put an end to {0}'s plans for domination.",
// "{0} fell before {1}'s relentless assault.",
// "{1} came out on top in the battle with {0}.",
// "{1} proved to be too much for {0} to handle.",
// "{1} emerged as the conqueror over {0}.",
// "{1} delivered the final blow to {0}.",
// "{1} proved their superiority by defeating {0}.",
// "{1} put a stop to {0}'s rampage.",
// "{1} sent {0} to the afterlife.",
// "{1} triumphed over {0} in battle.",
// "{1} prevailed over {0}'s resistance.",
// "{1} emerged victorious in the fight with {0}.",
// "{1} stood tall after defeating {0}.",
// "{1} struck down {0} with deadly precision.",
// "{1} dominated {0} in combat."

//         "{0} has died.",
//         "Rest in peace {0}.",
//         "{0} passed away.",
//         "{0} has left us.",
//         "{0} has gone to a better place.",
//         "{0} has passed away.",
//     "{0} has kicked the bucket.",
//     "{0} has gone to meet their maker.",
//     "{0} has shuffled off this mortal coil.",
//     "{0} has departed.",
//     "{0} has left us.",
//     "{0} has gone on to the next life.",
//     "{0} has met their end.",
//     "{0} has ceased to be.",
//     "{0} has left this world.",
//     "{0} has gone to a better place.",
//     "{0} has gone to the great beyond.",
//     "{0} has been taken from us.",
//     "{0} has gone to the other side.",
//     "{0} has gone to be with the angels.",
//     "{0} has gone to the light.",
//     "{0} has gone to the afterlife.",
//     "{0} has joined the choir invisible.",
//     "{0} has gone to their eternal rest.",
//     "{0} has left this mortal realm.",
//     "{0} has been called home.",
//     "{0} has transcended.",
//     "{0} has crossed over.",
//     "{0} has passed into the beyond.",
//     "{0} has gone to a higher plane.",
//     "{0} has gone to meet their ancestors.",
//     "{0} has returned to the dust from whence they came.",
//     "{0} has gone to their final resting place.",
//     "{0} has gone to the land of the dead.",
//     "{0} has gone to the great mystery.",
//     "{0} has left their mortal coil behind.",
//     "{0} has journeyed to the next life.",
//     "{0} has taken their final breath.",
//     "{0} has gone to the big sleep.",
//     "{0} has gone to the silent land.",
//     "{0} has gone to the undiscovered country.",
//     "{0} has gone to their ultimate fate.",
//     "{0} has gone to the next world.",
//     "{0} has gone to the great unknown.",
//     "{0} has gone to the eternal hunting grounds.",
//     "{0} has gone to the happy hunting ground.",
//     "{0} has gone to Valhalla.",
//     "{0} has gone to Fólkvangr.",
//     "{0} has gone to the realm of the dead.",
//     "{0} has gone to the underworld.",
//     "{0} has gone to the afterworld.",
//     "{0} has gone to the spirit world.",
//     "{0} has gone to the netherworld.",
//     "{0} has gone to the shadow realm.",
//         "{1} has taken down {0}.",
//     "{0} has fallen at the hands of {1}.",
//     "{1} has claimed {0}.",
//     "{1} has emerged victorious over {0}.",
//     "{1} has dispatched {0}.",
//     "{0} has been slain by {1}.",
//     "{1} has brought down {0}.",
//     "{0} has met their end at the hands of {1}.",
//     "{1} has proven too much for {0}.",
//     "{1} has bested {0}.",
//     "{0} has been eliminated by {1}.",
//     "{1} has vanquished {0}.",
//     "{0} has been taken out by {1}.",
//     "{1} has outplayed {0}.",
//     "{0} has fallen to {1}.",
//     "{1} has triumphed over {0}."