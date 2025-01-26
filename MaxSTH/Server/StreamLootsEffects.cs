using System;
using System.Collections.Generic;
using CitizenFX.Core;
using STHMaxzzzie;


namespace STHMaxzzzie.Server
{
    public class StreamLootsEffect : BaseScript
    {
        public static bool isSLOn = true;
        public static int SLItterateTime = 10;
        private List<string> effectNames = new List<string>{
              "cleartires", //removes all tires from near the client
                "spotlight", //puts a spotlight on the client from above
                "burstsome", //bursts a tire of the client vehicle
                "burstall", //bursts all the clients tires
                "drunk", //shows a drunk effect on the client
                "stop", //stops player dead in his tracks
                "speed", //gives the player lots of forward momentum
                "launch", //launches the player up
                "gta1cam", //(broken does nothing) clients looks through a top down camera
                "reversecam", //(broken goes in 1st person) clients look through the reverse camera
                "bounce", //reverses the clients direction and heading
                "kickflip", //jumps the client vehicle up a little and gives it a rolling force
                "gunjam", //(broken forces instant reload) after a few shots the gun stops working until the client reloads
                "fame", //(broken npcs don't all get targeted) npc's run towards the client
                "carfame", //(broken npcs don't all get targeted) npc's drive their car into the client
                "imponent", //(broken spawns a few times but goes to old cords and doesn't get in car) imponent rage steals your car
                "paint", //paints the clients car a bright colour
                "paintall", //paints all cars in the area a bright colour
                "pebble", //(works at times) breaks all windows around the clienta6
                "electricalglitch", //flashes the lights and horn and doors of the clients car
                "starmode", //sets the clients car to rgb mode and resets it when its done
                "carborrow", // (broken does nothing) sets the client into an npc's car
                "compacted", // sets the client into a compact car
                "supered", // sets the client into a super car
                "couped", // sets the client into a coupe car
                "shitboxed", // sets the client into a shitbox (voodoo)
                "boated", // sets the client into a boat (jetmax)
                "speedlimiter", //limits the speed of the client
                "carswap", //(broken does nothing) swaps the clients car with a different players vehicle
                "shake", // (broken does nothing) shakes player cam for 5 secs
                "locationchat", // sends location of player in chat for everyone but player.
                "fix", // fixes a vehicle
                "gravity" //makes the client anti gravity when on foot
             };

        [Command("togglesl", Restricted = true)]
        void ToggleStreamLootsCommand(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                if (!isSLOn)
                {
                    isSLOn = true;

                    TriggerClientEvent(Players[source], "ShowNotification", "StreamLoots command is now on.");

                }
                else
                {
                    isSLOn = false;
                    TriggerClientEvent(Players[source], "ShowNotification", "StreamLoots command is now off.");
                }
            }
            else if (args.Count == 1 && args[0].ToString() == "true")
            {
                isSLOn = true;
                TriggerClientEvent(Players[source], "ShowNotification", "StreamLoots command is now on.");

            }
            else if (args.Count == 1 && args[0].ToString() == "false")
            {
                isSLOn = false;
                TriggerClientEvent(Players[source], "ShowNotification", "StreamLoots command is now off.");
            }
            else if (args.Count == 2 && args[0].ToString() == "time" && Int32.TryParse(args[1].ToString(), out int Time))
            {
                SLItterateTime = Time;
                UpdateSLItterateTime();
            }
            else
            {
                TriggerClientEvent(Players[source], "ShowNotification", "Oh no. Something went wrong!\nYou should do /togglesl (true/false)");
            }
        }

        public static void UpdateSLItterateTime()
        {
            TriggerClientEvent("UpdateSLItterateTime", SLItterateTime);
        }

        [Command("sl", Restricted = true)]
        void StreamLootsCommand(int source, List<object> args, string raw)
        {
            //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"StreamLoots command." } });
            if (!isSLOn)
            {
                TriggerClientEvent(Players[source], "ShowNotification", "StreamLoots command is off.");
                return;
            }
            if (args.Count == 2 && effectNames.Contains(args[0].ToString()))
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"StreamLoots command received, {args[0]} for player {args[1]}." } });
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out int playerid);
                if (isArgs1Int)
                {
                    if (playerid == 0)
                    {
                        TriggerClientEvent(Players[source], "StreamLootsEffect", args[0]);
                    }
                    else
                    {
                        TriggerClientEvent(Players[playerid], "StreamLootsEffect", args[0]);
                    }
                }
            }

            else if (args.Count == 1 && effectNames.Contains(args[0].ToString()))
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"StreamLoots command received, {args[0]} for all players." } });
                TriggerClientEvent("StreamLootsEffect", args[0].ToString());
            }

            else
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/sl effectname clientId(0 for self and empty for all)" } });
            }
        }

        [EventHandler("SendSLChat")]
        private void SendSLChat(int source, string text)
        {
            foreach (Player player in Players)
            {
                if (int.Parse(player.Handle) != source)
                    TriggerClientEvent(player, "chat:addMessage", new { color = new[] { 255, 255, 255 }, args = new[] { Players[source].Name, text } });
            }
        }
    }
}