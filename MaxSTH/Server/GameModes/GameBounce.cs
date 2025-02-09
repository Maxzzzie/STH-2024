using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class GameBounce : BaseScript
    {
        public static int radius = 450;
        public static bool runnerSeesCircleBlip = false;
        public static bool shouldDecay = true;
        public static bool shouldDing = true;
        public static int defaultColour = 5;
        public static int defaultAlpha = 70;
        public static int decayRate = 30;


        [EventHandler("updateClientBounceSettings")]
        public static void updateClientBounceSettings()
        {
            //Debug.WriteLine($"updateClientBounceSettings {radius}, {runnerSeesCircleBlip}");
            TriggerClientEvent("updateBounceGameSettings", radius, runnerSeesCircleBlip, shouldDecay, shouldDing, defaultColour, defaultAlpha, decayRate);
        }

        [EventHandler("sendGameBounceBlip")]
        public void sendGameBounceBlip(Vector4 blipPosAndRadius, bool removeBlipOnly)
        {
            TriggerClientEvent("setGameBounceBlip", blipPosAndRadius, removeBlipOnly);
        }

        [EventHandler("changeGameBounceSetting")]
        public void changeGameBounceSetting(int source, List<object> args)
        {
            if (args.Count == 1){}
            else if (args.Count == 3) 
            {
                if (args[1].ToString() == "seesblip" && bool.TryParse(args[2].ToString(),out runnerSeesCircleBlip)){}
                else if (args[1].ToString() == "decay" && bool.TryParse(args[2].ToString(),out shouldDecay)){}
                else if (args[1].ToString() == "sound" && bool.TryParse(args[2].ToString(),out shouldDing)){}
                else if (args[1].ToString() == "radius" && int.TryParse(args[2].ToString(),out radius)){}
                else if (args[1].ToString() == "colour" && int.TryParse(args[2].ToString(),out defaultColour)){}
                else if (args[1].ToString() == "alpha" && int.TryParse(args[2].ToString(),out defaultAlpha)){}
                else if (args[1].ToString() == "decayrate" && int.TryParse(args[2].ToString(),out decayRate)){}
            }
            else TriggerClientEvent(Players[source], "ShowErrorNotification", $"/settings bounce (gives current state)\nAdd (seesblip/decay + bool) or\n(radius/colour/alpha/decayrate + int).");
            sendCurrentSettings(source);
            updateClientBounceSettings();
        }
        public void sendCurrentSettings(int source)
        {
            TriggerClientEvent(Players[source], "ShowNotification", $"~b~CopyClass settings~s~\nRadius: {radius}m. Runner sees blip: {runnerSeesCircleBlip}.\nCircle should decay: {shouldDecay}. Colour: {defaultColour}.\nAlpha: {defaultAlpha}. Decay rate(m/s): {decayRate}.");
        }
    }
}