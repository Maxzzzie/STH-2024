using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder; // chat gpt addition to prevent compile error


namespace STHMaxzzzie.Server
{
    public class DeathMessages : BaseScript
    {
        // Dictionary<string, VehicleHash> VehicleNameToHash = null;
        // public deathVehicleType()
        // {
        //     //make a dictionary mapping vehicle name => hash
        //     //https://stackoverflow.com/a/5583817 / https://gtahash.ru/car/
        //     VehicleNameToHash = new Dictionary<string, VehicleHash>();
        //     foreach (var veh_hash in Enum.GetValues(typeof(VehicleHash)))
        //     {
        //         VehicleNameToHash.Add(veh_hash.ToString().ToLower(), (VehicleHash)veh_hash);
        //     }
        // }
        public DeathMessages()
        {
            Debug.WriteLine($"DeathMessages has started.");
            EventHandlers["baseevents:onPlayerDied"] += new Action<Player, int, List<object>>(OnPlayerDied);
            EventHandlers["baseevents:onPlayerKilled"] += new Action<Player, int, dynamic>(OnPlayerKilled);
        }

        //onplayerkilled will trigger when a player is killed by another player.
        public void OnPlayerKilled([FromSource] Player victim, int deathCause, dynamic killerData)
        {
            var killerDataDictionary = (IDictionary<string, object>)killerData;

            foreach (KeyValuePair<string, object> property in killerDataDictionary)
            {
                Debug.WriteLine($"foreach --> {property.Key} - {property.Value}");
            }

            if (killerDataDictionary.ContainsKey("killerpos"))
            {
                // Get the 'killerpos' value and cast it to List<object>
                var killerPosList = killerDataDictionary["killerpos"] as List<object>;

                if (killerPosList != null && killerPosList.Count >= 3)
                {
                    // Assuming it represents a position (X, Y, Z)
                    float x = Convert.ToSingle(killerPosList[0]);
                    float y = Convert.ToSingle(killerPosList[1]);
                    float z = Convert.ToSingle(killerPosList[2]);

                    Debug.WriteLine($"Killer position: X={x}, Y={y}, Z={z}");
                }
                else
                {
                    Debug.WriteLine("Killer position data is missing or incomplete from onPlayerKilled.");
                }
            }

            // if (dictionary.ContainsKey("vehicletype"))
            // {
            //     if (vehicletype.value != null && VehicleNameToHash.ContainsValue(vehicletype.value))
            //     {
            //         foreach (var kvp in VehicleNameToHash)
            //         {
            //             if (kvp.Value.Equals(vehicletype.value))
            //             {
            //                 string killervehicle = kvp.Key;
            //             }
            //         }
            //     }

            //     else
            //     {
            //         Debug.WriteLine("Vehicle data is missing or incomplete from onPlayerKilled.");
            //     }
            // }
            //the above, shows: killerpos, weaponhash, killertype, killervehseat, killerinveh, killervehname
            //vehicle type (empty if killer wasn't in a vehicle), vehicleseat, weaponhash "https://gtahash.ru/weapons/?page=3", killertype, and some more. killer maybe?

            Debug.WriteLine($"died {victim.Name}, pedType?: {deathCause}.");
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"{victim.Name} died." } });
        }

        //onplayerdied will trigger when a player isn't killed by another player
        private void OnPlayerDied([FromSource] Player player, int killerType, List<object> deathCoords)
        {
            // Extract the death coordinates from the deathCoords array
            float deathX = Convert.ToSingle(deathCoords[0]);
            float deathY = Convert.ToSingle(deathCoords[1]);
            float deathZ = Convert.ToSingle(deathCoords[2]);
            Vector3 deathPosition = new Vector3(deathX, deathY, deathZ);

            Debug.WriteLine($"SERVER DeathMessages: Player: {player.Name}, killerType = {killerType} Coords = {deathPosition}.");
            //killerType is an and in this case linked to the model.
            //find info here https://docs.fivem.net/docs/resources/baseevents/events/onPlayerDied/
            //the following code makes no sence then.
            string cause = "null";

            if (killerType == -1)
            {
                cause = "law enforcement"; //gets triggered a lot.
            }
            else if (killerType == 0)
            {
                cause = "suicide";
            }
            else if (killerType == 1)
            {
                cause = "a player";//not right
            }
            else if (killerType == 2)
            {
                cause = "falldamage";
            }
            else if (killerType == 3)
            {
                cause = "3. a colision with an object";//have never been able to get this.
            }
            else if (killerType == 4)
            {
                cause = "4. a big boom";//never managed to get this.
            }
            else if (killerType == 5)
            {
                cause = "5. something hot";//never managed to get this.
            }
            else if (killerType == 6)
            {
                cause = "6, unknown reason";//never managed to get this.
            }
            else if (killerType == 7)
            {
                cause = "7. a cougar perhaps";//never managed to get this. Also never tried to get killed by wildlife.
            }
            else if (killerType == 8)
            {
                cause = "8. NPC's, what a loser";//never managed to get this. Usually it will be 1 if killed by an npc like angry ped or gangster.
            }
            else
            {
                cause = "reasons unknown";
            }
            Debug.WriteLine($"Player {player.Name} died because of {cause}. Debug info: {killerType}");
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{player.Name} died maybe because of {cause}." } });
        }
    }
}