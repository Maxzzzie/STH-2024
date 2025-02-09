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
        public static Dictionary<Player, int> playerPris = new Dictionary<Player, int>();
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
            float distanceToClosestCalloutToPri = float.PositiveInfinity;
            foreach (var kvp in ServerMain.maxzzzieCalloutsDict)
            {
                float distance = Vector3.Distance(position, kvp.Value);
                if (distance < distanceToClosestCalloutToPri)
                {
                    closestCalloutToPri = kvp.Key;
                    distanceToClosestCalloutToPri = distance;
                }
            }

            int vehicle = API.CreateVehicle(vehicleHash, position.X, position.Y, position.Z, heading, true, true);
            int attempts = 0;

            while (!API.DoesEntityExist(vehicle) && attempts < 500)
            {
                attempts++;
                await Delay(10);
            }

            if (!API.DoesEntityExist(vehicle))
            {
                Debug.WriteLine("Error: Vehicle entity still does not exist after attempts!");
                return; // Prevent further execution
            }
            int networkId = API.NetworkGetNetworkIdFromEntity(vehicle);
            //API.SetEntityDistanceCullingRadius(vehicle, 5000f);
            string[] trimmedClosestCalloutName = closestCalloutToPri.Split('*');
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 204, 0, 204 }, multiline = true, args = new[] { "Server", $"{player.Name} is spawning a Prius near {trimmedClosestCalloutName[0]}!" } });
            TriggerClientEvent("chat:addMessage", new { color = new[] { 204, 0, 204 }, args = new[] { player.Name, $"I'm spawning a Prius near {trimmedClosestCalloutName[0]}!" } });
            //TriggerClientEvent("ShowNotification", $"~h~~q~<C>{player.Name}<C>~s~~h~ spawned a ~h~~q~Prius~s~~h~ near~n~<C>{trimmedClosestCalloutName[0]}<C>!");
            await Delay(10);
            API.SetVehicleColours(vehicle, 135, 135);
            API.SetVehicleNumberPlateText(vehicle, $"{player.Name}");
            playerPris.Add(player, vehicle);
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            BlipHandler.BlipData pri = new BlipHandler.BlipData($"pri{player.Name}")
            {
                Coords = new Vector3(position.X, position.Y, position.Z),
                Type = "coord",
                EntityId = networkId,
                Sprite = 119,
                Colour = 48,
                MapName = $"Pri of {player.Name}"

            };
            request.BlipsToAdd.Add(pri);
            BlipHandler.AddBlips(request);
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
                        TriggerClientEvent(playerPri.Key, "ShowSpecialNotification", "Your ~q~~h~pri~h~~s~ got destroyed!", "Goal", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
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
                    TriggerClientEvent(playerPri.Key, "ShowErrorNotification", "Your ~q~~h~pri~h~~s~ disappeared!");
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
