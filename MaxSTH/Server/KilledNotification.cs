using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;

namespace STHMaxzzzie.Server
{
    public class DeathMessages : BaseScript
    {
        string[] OnPlayerKilledRandMsg = new string[] {
        "{Killer.Name} killed {Victim.Name}.",
        "{Victim.Name} ended up at the hands of {Killer.Name}.",
        "A conflict happened between {Victim.Name} and {Killer.Name}. {Victim.Name} came out worse.",
        "{Killer.Name} dealt the final blow to {Victim.Name}.",
        "{Killer.Name} has taken out {Victim.Name}.",
        "{Killer.Name} has killed {Victim.Name}.",
"{Killer.Name} has ended {Victim.Name}.",
"{Victim.Name} was taken down by {Killer.Name}.",
"{Victim.Name} met their end at the hands of {Killer.Name}.",
"A conflict happened between {Victim.Name} and {Killer.Name}. {Victim.Name} came out worse.",
"{Killer.Name} emerged victorious over {Victim.Name}.",
"{Victim.Name} was defeated by {Killer.Name}.",
"{Killer.Name} put an end to {Victim.Name}.",
"{Killer.Name} proved to be too much for {Victim.Name}.",
"{Killer.Name} took out {Victim.Name}.",
"{Killer.Name} got the best of {Victim.Name}.",
"{Killer.Name} claimed the life of {Victim.Name}.",
"{Victim.Name} was slain by {Killer.Name}.",
"{Killer.Name} dealt the fatal blow to {Victim.Name}.",
"{Killer.Name} overpowered {Victim.Name}.",
"{Killer.Name} executed {Victim.Name}.",
"{Killer.Name} ended {Victim.Name}'s life.",
"{Killer.Name} triumphed over {Victim.Name}.",
"{Killer.Name} was the last one standing after the fight with {Victim.Name}.",
"{Killer.Name} emerged as the victor in the battle with {Victim.Name}.",
"{Victim.Name} fell to {Killer.Name}'s might.",
"{Killer.Name} delivered the killing blow to {Victim.Name}.",
"{Killer.Name} defeated {Victim.Name} in combat.",
"{Killer.Name} put an end to {Victim.Name}'s reign of terror.",
"{Killer.Name} took down {Victim.Name} in a fierce struggle.",
"{Killer.Name} vanquished {Victim.Name} in battle.",
"{Killer.Name} dispatched {Victim.Name} with ease.",
"{Killer.Name} eliminated {Victim.Name} with lethal force.",
"{Killer.Name} ended {Victim.Name}'s time in this world.",
"{Killer.Name} sent {Victim.Name} to meet their maker.",
"{Killer.Name} took {Victim.Name} out of the picture.",
"{Victim.Name} was no match for {Killer.Name}'s strength.",
"{Killer.Name} made quick work of {Victim.Name}.",
"{Killer.Name} outmatched {Victim.Name} in the fight.",
"{Killer.Name} emerged victorious against {Victim.Name}'s onslaught.",
"{Killer.Name} put an end to {Victim.Name}'s plans for domination.",
"{Victim.Name} fell before {Killer.Name}'s relentless assault.",
"{Killer.Name} came out on top in the battle with {Victim.Name}.",
"{Killer.Name} proved to be too much for {Victim.Name} to handle.",
"{Killer.Name} emerged as the conqueror over {Victim.Name}.",
"{Killer.Name} delivered the final blow to {Victim.Name}.",
"{Killer.Name} proved their superiority by defeating {Victim.Name}.",
"{Killer.Name} put a stop to {Victim.Name}'s rampage.",
"{Killer.Name} sent {Victim.Name} to the afterlife.",
"{Killer.Name} triumphed over {Victim.Name} in battle.",
"{Killer.Name} prevailed over {Victim.Name}'s resistance.",
"{Killer.Name} emerged victorious in the fight with {Victim.Name}.",
"{Killer.Name} stood tall after defeating {Victim.Name}.",
"{Killer.Name} struck down {Victim.Name} with deadly precision.",
"{Killer.Name} dominated {Victim.Name} in combat.",
"{Killer.Name} has taken down {Victim.Name}.",
    "{Victim.Name} has fallen at the hands of {Killer.Name}.",
    "{Killer.Name} has claimed {Victim.Name}.",
    "{Killer.Name} has emerged victorious over {Victim.Name}.",
    "{Killer.Name} has dispatched {Victim.Name}.",
    "{Victim.Name} has been slain by {Killer.Name}.",
    "{Killer.Name} has brought down {Victim.Name}.",
    "{Victim.Name} has met their end at the hands of {Killer.Name}.",
    "{Killer.Name} has proven too much for {Victim.Name}.",
    "{Killer.Name} has bested {Victim.Name}.",
    "{Victim.Name} has been eliminated by {Killer.Name}.",
    "{Killer.Name} has vanquished {Victim.Name}.",
    "{Victim.Name} has been taken out by {Killer.Name}.",
    "{Killer.Name} has outplayed {Victim.Name}.",
    "{Victim.Name} has fallen to {Killer.Name}.",
    "{Killer.Name} has triumphed over {Victim.Name}."};

        string[] OnPlayerDiedRandMsg = new string[] {
        "{Victim.Name} has died.",
        "Rest in peace {Victim.Name}.",
        "{Victim.Name} passed away.",
        "{Victim.Name} has left us.",
        "{Victim.Name} has gone to a better place.",
        "{Victim.Name} has passed away.",
    "{Victim.Name} has kicked the bucket.",
    "{Victim.Name} has gone to meet their maker.",
    "{Victim.Name} has shuffled off this mortal coil.",
    "{Victim.Name} has departed.",
    "{Victim.Name} has left us.",
    "{Victim.Name} has gone on to the next life.",
    "{Victim.Name} has met their end.",
    "{Victim.Name} has ceased to be.",
    "{Victim.Name} has left this world.",
    "{Victim.Name} has gone to a better place.",
    "{Victim.Name} has gone to the great beyond.",
    "{Victim.Name} has been taken from us.",
    "{Victim.Name} has gone to the other side.",
    "{Victim.Name} has gone to be with the angels.",
    "{Victim.Name} has gone to the light.",
    "{Victim.Name} has gone to the afterlife.",
    "{Victim.Name} has joined the choir invisible.",
    "{Victim.Name} has gone to their eternal rest.",
    "{Victim.Name} has left this mortal realm.",
    "{Victim.Name} has been called home.",
    "{Victim.Name} has transcended.",
    "{Victim.Name} has crossed over.",
    "{Victim.Name} has passed into the beyond.",
    "{Victim.Name} has gone to a higher plane.",
    "{Victim.Name} has gone to meet their ancestors.",
    "{Victim.Name} has returned to the dust from whence they came.",
    "{Victim.Name} has gone to their final resting place.",
    "{Victim.Name} has gone to the land of the dead.",
    "{Victim.Name} has gone to the great mystery.",
    "{Victim.Name} has left their mortal coil behind.",
    "{Victim.Name} has journeyed to the next life.",
    "{Victim.Name} has taken their final breath.",
    "{Victim.Name} has gone to the big sleep.",
    "{Victim.Name} has gone to the silent land.",
    "{Victim.Name} has gone to the undiscovered country.",
    "{Victim.Name} has gone to their ultimate fate.",
    "{Victim.Name} has gone to the next world.",
    "{Victim.Name} has gone to the great unknown.",
    "{Victim.Name} has gone to the eternal hunting grounds.",
    "{Victim.Name} has gone to the happy hunting ground.",
    "{Victim.Name} has gone to Valhalla.",
    "{Victim.Name} has gone to Fólkvangr.",
    "{Victim.Name} has gone to the realm of the dead.",
    "{Victim.Name} has gone to the underworld.",
    "{Victim.Name} has gone to the afterworld.",
    "{Victim.Name} has gone to the spirit world.",
    "{Victim.Name} has gone to the netherworld.",
    "{Victim.Name} has gone to the shadow realm."};
        Random rand = new Random();
        public DeathMessages()
        {
            // Debug.WriteLine($"DeathMessages has started.");
        }


        [EventHandler("OnPlayerKilled")]
        //onplayerkilled will trigger when a player is killed by another player.
        public void OnPlayerKilled(int VictimId, int KillerId, Vector3 VictimLocation, Vector3 KillerLocation, int weaponHash, bool KillerInVehicle, int VehicleHash)
        {
            Debug.WriteLine($"onPlayerKilled server {VictimId}, {KillerId}");
            Player Victim = Players[VictimId];
            Player Killer = Players[KillerId];
            string message = FormatMessage(OnPlayerKilledRandMsg[rand.Next(0, OnPlayerKilledRandMsg.Length)], Victim, Killer);
            Debug.WriteLine(message);
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"{Victim.Name} was killed by {Killer.Name}." } });
            TriggerClientEvent("ShowNotification", $"~h~~o~{Victim.Name}~s~~h~ was killed by ~h~~r~{Killer.Name}~h~~w~");
        }

        //onplayersuicide will trigger when a player is killed by himself.
        [EventHandler("OnPlayerSuicide")]
        private void OnPlayerSuicide(int VictimId, Vector3 VictimLocation)
        {
            Player Victim = Players[VictimId];
            Debug.WriteLine($"OnPlayerSuicide server: Victim: {Victim.Name}, Coords = {VictimLocation.X}, {VictimLocation.Y}, {VictimLocation.Z}.");
            Debug.WriteLine($"{Victim.Name} chose the easy way out.");
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{Victim.Name} died." } });
            TriggerClientEvent("ShowNotification", $"~o~~h~{Victim.Name}~h~~s~ commited suicide.");
        }

        //onplayerdied will trigger when a player isn't killed by another player
        [EventHandler("OnPlayerDied")]
        private void OnPlayerDied(int VictimId, Vector3 VictimLocation)
        {
            Player Victim = Players[VictimId];
            Debug.WriteLine($"onPlayerDied server: Victim: {Victim.Name}, Coords = {VictimLocation.X}, {VictimLocation.Y}, {VictimLocation.Z}.");
            string message = FormatMessage(OnPlayerDiedRandMsg[rand.Next(0, OnPlayerDiedRandMsg.Length)], Victim, Victim);
            Debug.WriteLine(message);
            //TriggerClientEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{Victim.Name} died." } });
            TriggerClientEvent("ShowNotification", $"~o~~h~{Victim.Name}~h~~s~ died");
        }

        private string FormatMessage(string template, Player Victim, Player Killer)
        {
            string message = template.Replace("{Victim.Name}", Victim.Name);
            if (Killer != Victim)
            {
                message = message.Replace("{Killer.Name}", Killer.Name);
            }
            return message;
        }
    }
}