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
        //         VehicleNameToHash.Add((VehicleHash)veh_hash, veh_hash.ToString().ToLower());
        //     }
        // }
        public DeathMessages()
        {
            Debug.WriteLine($"DeathMessages has started.");
            EventHandlers["baseevents:onPlayerDied"] += new Action<Player, int, List<object>>(OnPlayerDied);
            EventHandlers["baseevents:onPlayerKilled"] += new Action<Player, int, dynamic>(OnPlayerKilled);
        }

        string latestKillerName = "null";
        [EventHandler("sendKillerIDToServer")]
        public async void sendKillerIDToServer([FromSource] Player source, int killerID, dynamic data)
        {
            int latestKillerID = -1;

            if (latestKillerID == -1)
            {
                Debug.WriteLine($"No killer known");
            }
            else foreach (Player i in Players)
                {
                    Debug.WriteLine($"playerhandles = {i.Handle} Players = {i} Name = {i.Name}");
                    if (int.Parse(i.Handle) == latestKillerID)
                    {
                        Debug.WriteLine($"killerhandle ={i.Handle}\nkillername = {i.Name}");
                        latestKillerName = i.Name;
                    }
                }
            await Delay(600);
            latestKillerName = "null";
        }

        //onplayerkilled will trigger when a player is killed by another player.
        public async void OnPlayerKilled([FromSource] Player victim, int deathCause, dynamic killerData)
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
            Debug.WriteLine($"killed {victim.Name}.");

            await BaseScript.Delay(500);
            if (latestKillerName == "null")
            {
                TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"{victim.Name} was killed." } });
            }
            else TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"{victim.Name} was killed by {latestKillerName}." } });
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
                cause = "a player";//not right because for player killed its onPlayerKilled event.
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
            TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{player.Name} died unfortunately." } });
        }
    }
}
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