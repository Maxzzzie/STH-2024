using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;


namespace STHMaxzzzie.Server
{
    public class PriusMechanics : BaseScript
    {
        public static bool didClearJustHappen = false; //for the pri check. If clear happened i wanna remove blips.
        private Dictionary<Player, int> playerPris = new Dictionary<Player, int>();
        public PriusMechanics()
        {
            Tick += OnTick;
        }

        [EventHandler("pri-spawn-requested")]
        async void OnPriRequested([FromSource] Player player, uint vehicleHash, Vector3 position, float heading)
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
            string closestCalloutToPri = "the Void";
            float distanceToClosestCalloutToPri = 10000;
            foreach (var kvp in ServerMain.maxzzzieCalloutsDict)
            {
                float distance = Vector3.Distance(position, kvp.Value);
                if (distance < distanceToClosestCalloutToPri)
                {
                    closestCalloutToPri = kvp.Key;
                    distanceToClosestCalloutToPri = distance;
                }
            }

            int vehicle = API.CreateVehicle(vehicleHash, position.X, position.Y, position.Z, heading, true, true); // Vehicle Hash gotten from VehicleHash on client, for some reason not available on server?

            string[] trimmedClosestCalloutName = closestCalloutToPri.Split('*');
            TriggerClientEvent("chat:addMessage", new { color = new[] { 204, 0, 204 }, multiline = true, args = new[] { "Server", $"{player.Name} is spawning a Prius near {trimmedClosestCalloutName[0]}!" } });
            //TriggerClientEvent("ShowNotification", $"~h~~q~<C>{player.Name}<C>~s~~h~ spawned a ~h~~q~Prius~s~~h~ near~n~<C>{trimmedClosestCalloutName[0]}<C>!");
            await Delay(10);
            API.SetVehicleColours(vehicle, 135, 135);
            API.SetVehicleNumberPlateText(vehicle, $"{player.Name}");
            playerPris.Add(player, vehicle);
            //TriggerEvent("addBlip", false, $"pri{player.Name}", "coord", new Vector3(position.X, position.Y, position.Z), vehicle, 119, 48, true, false, true);
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            BlipHandler.BlipData pri = new BlipHandler.BlipData($"pri{player.Name}")
            {
                Coords = new Vector3(position.X, position.Y, position.Z),
                Sprite = 119,
                Colour = 48,
                MapName = $"Pri of {player.Name}"
            };
            request.BlipsToAdd.Add(pri);
            BlipHandler.AddBlips(request);
            //TriggerClientEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Trying to spawn a pri blip. with {pri.MapName}"}});
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
            foreach (var playerPri in playerPris)
            {
                if (API.DoesEntityExist(playerPri.Value))
                {
                    float health = API.GetVehicleEngineHealth(playerPri.Value);
                    if (health <= 0)
                    {
                        //Debug.WriteLine($"{playerPri.Key.Name}'s Pri got destroyed!");
                        keysToRemove.Add(playerPri.Key);
                        // TriggerClientEvent(playerPri.Key, "chat:addMessage", new
                        // {
                        //     color = new[] { 204, 0, 204 }, //pink color for msg
                        //     multiline = false,
                        //     args = new[] { "Server", "Your pri got destroyed!" }
                        // });
                        TriggerClientEvent("ShowNotification", "~q~Your ~h~pri~h~ got destroyed!");
                        BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
                        request.BlipsToRemove.Add($"pri{playerPri.Key.Name}");
                        BlipHandler.AddBlips(request);
                    }
                }
                if (didClearJustHappen && !API.DoesEntityExist(playerPri.Value))
                {
                    keysToRemove.Add(playerPri.Key);
                    // TriggerClientEvent(playerPri.Key, "chat:addMessage", new
                    // {
                    //     color = new[] { 204, 0, 204 }, //pink color for msg
                    //     multiline = false,
                    //     args = new[] { "Server", "Your pri disappeared!" }
                    // });
                    TriggerClientEvent("ShowNotification", "~q~Your ~h~pri~h~ disappeared!");
                    BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
                    request.BlipsToRemove.Add($"pri{playerPri.Key.Name}");
                    BlipHandler.AddBlips(request);
                }
            }
            foreach (var key in keysToRemove)
                playerPris.Remove(key);
            didClearJustHappen = false;
        }
    }
}
