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
using TwitchTestClient.Server.Features;
using System.Linq.Expressions;
using TwitchLib.Client.Models;
using System.Linq;

namespace STHMaxzzzie.Server
{
    public class Settings : BaseScript
    {

        [Command("settings", Restricted = true)]
        void SettingsCommand(int source, List<object> args, string raw)
        {
            if (args.Count == 0) SendCurrentSettings(source);
            else if (args.Count >= 2 && args[0].ToString() == "bounce") TriggerEvent("changeGameBounceSetting", source, args);
            else if (args.Count >= 2 && args[0].ToString() == "copyclass") TriggerEvent("changeGameCopyClassSetting", source, args);
        }

        void SendCurrentSettings(int source)
        {
            TriggerClientEvent(Players[source], "ShowNotification", $"~h~~o~Current server settings are printed in the client console(f8).");
            TriggerClientEvent(Players[source], "displayClientDebugLine",
            "---   ---   ---   ---   ---   settings   ---   ---   ---   ---   ---\n"
            + $"Fix status: {Misc.AllowedToFixStatus}\nFix wait time: {Misc.fixWaitTime}\nIs POD on: {Misc.isPodOn}\n"
            + $"Is PvP on: {Armoury.isPvpAllowed}\nAre weapons allowed: {Armoury.isWeaponsAllowed}\nIs SFV allowed: {Armoury.isShootingFromVehicleAllowed}\n"
            + $"Are vehicle spawns restricted: {ServerMain.isVehRestricted}\nIs shots fired marker on: {ShotBlipServer.areShotsFiredVisible}\n"
            + $"Is fire surpression on: {LoadResources.defaultShouldFireBeControlled}\nRange of fire surpression: {LoadResources.FireControlrange}\n"
            + $"Are players allowed to tp: {Teleports.isPlayerAllowedToTp}\nPlayer vehicles should persist: {Vehicles.vehicleShouldNotDespawn}\n"
            + $"Is streamloots on: {StreamLootsEffect.isSLOn}\nStreamLoots itterate time: {StreamLootsEffect.SLItterateTime}\n"
            + $"Is player vehicles colour on: {Vehicles.vehicleShouldChangePlayerColour}\nBounce mode set radius:{GameBounce.radius}\n"
            + $"Bounce mode does player see blip: {GameBounce.runnerSeesCircleBlip}\nDelay mode does player see blip: {DelayMode.runnerSeesDelayBlip}\n"
            + $"Delay mode distance to blip: {DelayMode.distanceToBlip}\nCurrent game mode: {RoundHandling.gameMode}\n"
            + $"CopyClass force specific vehicle: {GameCopyClass.forceChosenVehicleOnly}\n"
            + "---   ---   ---   ---   ---   settings   ---   ---   ---   ---   ---");
        }
    }
}