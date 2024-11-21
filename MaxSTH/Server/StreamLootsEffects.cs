using System;
using System.Collections.Generic;
using CitizenFX.Core;
using STHMaxzzzie;


namespace STHMaxzzzie.Server
{
    public class StreamLootsEffect : BaseScript
    {   
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
                "fame", //(broken npcs go to 1 location) npc's run towards the client
                "carfame", //(broken npcs go to 1 location) npc's drive their car into the client
                "imponent", //(broken spawns a few times but goes to old cords and doesn't get in car) imponent rage steals your car
                "paint", //paints the clients car a bright colour
                "paintall", //paints all cars in the area a bright colour
                "pebble", //(works at times) breaks all windows around the client
                "electricalglitch", //flashes the lights and horn and doors of the clients car
                "starmode", //sets the clients car to rgb mode and resets it when its done
                "carborrow", // (broken does nothing) sets the client into an npc's car
                "compacted", //(broken does only panto) sets the client into a compact
                "speedlimiter", //limits the speed of the client
                "carswap" //(broken does nothing) swaps the clients car with a different players vehicle
             };

        [Command("sl", Restricted = false)]
        void StreamLootsCommand(int source, List<object> args, string raw)
        {
            //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"StreamLoots command." } });

            if (args.Count == 2 && effectNames.Contains(args[0].ToString()))
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"StreamLoots command received, {args[0]} for player {args[1]}." } });
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out int playerid);
                if (isArgs1Int)
                {
                    TriggerClientEvent(Players[playerid], "StreamLootsEffect", args[0]);
                }
            }

            else if (args.Count == 1 && effectNames.Contains(args[0].ToString()))
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"StreamLoots command received, {args[0]} for all players." } });
                TriggerClientEvent("StreamLootsEffect", args[0].ToString());
            }

            else
            {
                TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/sl burstsome/spotlight/burstall/" } });
            }
        }
    }
}