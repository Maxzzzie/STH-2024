using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using STHMaxzzzie.Client;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class GameBounce : BaseScript
    {
        static Random rand = new Random();
        int bounceAreaBlip;//blip marking the area the player can be in.
        int bounceCenterBlip;//blip marking the center of the area in case it's off your map.
        bool runnerSeesCircleBlip = false; //does the runner see the circle too?
        static int defaultRadius = 450;

        public GameBounce()
        {
            TriggerServerEvent("updateClientBounceSettings");
        }

        //Don't make radius smaller than 50. It might give issues.
        [EventHandler("gameBounce")]
        public static async void gameBounceBlipCalculation()
        {
            bool firstBlip = true;
            //Debug.WriteLine("starting gameBounce");
            Vector4 blipPosAndRadius = new Vector4(Game.PlayerPed.Position, defaultRadius);

            //while (Game.PlayerPed.IsAlive)
            while (Game.PlayerPed.IsAlive && RoundHandling.gameMode == "bounce")
            {
                Vector3 newPos = Game.PlayerPed.Position;

                float distance = GetDistanceBetweenCoords(newPos.X, newPos.Y, newPos.Z, blipPosAndRadius.X, blipPosAndRadius.Y, blipPosAndRadius.Z, false);

                if (distance > blipPosAndRadius.W || firstBlip)
                {
                    firstBlip = false;
                    //Debug.WriteLine("distance>bounceBlipRadius gameBounce");
                    bool foundSolution = false;

                    while (!foundSolution)
                    {
                        // Generate a random angle in radians (0 to 2 * PI)
                        double angle = rand.NextDouble() * 2 * Math.PI;

                        // Generate a random distance (0 to radius), ensuring uniform distribution
                        double randomRadius = Math.Sqrt(rand.NextDouble()) * blipPosAndRadius.W;

                        // Calculate the offsets
                        float XOffset = (float)(randomRadius * Math.Cos(angle));
                        float YOffset = (float)(randomRadius * Math.Sin(angle));

                        // Determine the new potential position
                        Vector3 potentialNewBounceBlipLocation = new Vector3(newPos.X + XOffset, newPos.Y + YOffset, newPos.Z);

                        // Check if the new position is within the allowed distance
                        float dist = GetDistanceBetweenCoords(newPos.X, newPos.Y, newPos.Z, potentialNewBounceBlipLocation.X, potentialNewBounceBlipLocation.Y, potentialNewBounceBlipLocation.Z, false);
                        if (dist < blipPosAndRadius.W - 30)
                        {
                            // Update the blip position and exit the loop
                            blipPosAndRadius.X = potentialNewBounceBlipLocation.X;
                            blipPosAndRadius.Y = potentialNewBounceBlipLocation.Y;
                            blipPosAndRadius.Z = potentialNewBounceBlipLocation.Z;
                            foundSolution = true;
                        }
                    }

                    //Debug.WriteLine($"found solution gameBounce {blipPosAndRadius.X}, {blipPosAndRadius.Y}, {blipPosAndRadius.Z}, {blipPosAndRadius.W}");
                    TriggerServerEvent("sendGameBounceBlip", blipPosAndRadius, false);

                }
                //Debug.WriteLine("gameBounce dist " + distance);
                await Delay(500);
            }
            TriggerServerEvent("sendGameBounceBlip", blipPosAndRadius, true); //deletes the Bounce blip
            //Debug.WriteLine("end of gameBounce");
        }

        [EventHandler("setGameBounceBlip")]
        private void setGameBounceBlip(Vector4 blipPosAndRadius, bool removeBlipOnly)
        {
            //Debug.WriteLine("setGameBounceBlip");
            RemoveBlip(ref bounceAreaBlip);
            RemoveBlip(ref bounceCenterBlip);
            if (removeBlipOnly) return; //Stops placing of new blips.

            if (runnerSeesCircleBlip || RoundHandling.thisClientIsTeam != 1)
            {
                //Debug.WriteLine("setGameBounceBlip");
                bounceAreaBlip = AddBlipForRadius(blipPosAndRadius.X, blipPosAndRadius.Y, blipPosAndRadius.Z, blipPosAndRadius.W);
                SetBlipAlpha(bounceAreaBlip, 40);
                SetBlipColour(bounceAreaBlip, 49);
                bounceCenterBlip = AddBlipForCoord(blipPosAndRadius.X, blipPosAndRadius.Y, blipPosAndRadius.Z);
                SetBlipColour(bounceCenterBlip, 49);
                SetBlipSprite(bounceCenterBlip, 161);
            }
        }

        [EventHandler("updateBounceGameSettings")]
        void updateBounceGameSettings(int newRadius, bool newRunnerSeesCircleBlip)
        {
            
            defaultRadius = newRadius;
            runnerSeesCircleBlip = newRunnerSeesCircleBlip;
            //Debug.WriteLine($"updateBounceGameSettings {defaultRadius}, {runnerSeesCircleBlip}");

        }
    }
}