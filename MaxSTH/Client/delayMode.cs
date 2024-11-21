using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Client
{
    public class DelayMode : BaseScript
    {
        int distanceToBlip = 400;
        static List<Vector4> delayLocationList = new List<Vector4>();
        int delayBlipHandle = 0;

        
        static int highSpeedSpeed = 10;
        static int highSpeedBlipDistanceSubtraction = 10;
        static int highSpeedBlipDistanceAddition = 10;
        static int highSpeedBlipTimeSubtractTrigger = 60;
        static int highSpeedBlipTimeAddTrigger = 45;
        static int highSpeedBlipTimeAdd = 2;
        static int highSpeedBlipTimeSubtract = 1;
        static int highSpeedBlipMinimumDistance = 80;


        [EventHandler("updateDelayModeSettings")]
        private void updateDelayModeSettings(int temp1, int temp2, int temp3, int temp4, int temp5, int temp6, int temp7, int temp8)
        {
           highSpeedSpeed = temp1;
           highSpeedBlipDistanceSubtraction = temp2;
           highSpeedBlipDistanceAddition = temp3;
           highSpeedBlipTimeAddTrigger = temp4;
           highSpeedBlipTimeSubtractTrigger = temp5;
           highSpeedBlipTimeAdd = temp6;
           highSpeedBlipTimeSubtract = temp7;
           highSpeedBlipMinimumDistance = temp8; 
        }


        [EventHandler("getBlipLocationForDelayMode")]
        public async void getBlipLocationForDelayMode(bool isDelayModeOn, int distance)
        {

            distanceToBlip = distance;
            Vector3 lastPos = Game.PlayerPed.Position;
            Vector3 blipPosition = Game.PlayerPed.Position;
            delayLocationList.Add(new Vector4(lastPos.X, lastPos.Y, lastPos.Z, 0));
            int distanceToBlipTakingSpeedIntoAccount = distanceToBlip;
            int isRunnerAtHighSpeedForALongTime = 0;

            while (RoundHandling.gameMode == "delay" && !Game.PlayerPed.IsDead)
            {
                Vector3 pos = Game.PlayerPed.Position;
                float positionDifference = GetDistanceBetweenCoords(pos.X, pos.Y, pos.Z, lastPos.X, lastPos.Y, lastPos.Z, true);
                //Debug.WriteLine($"starting getblipLocationForDelayMode \nCount: {delayLocationList.Count} || Difference: {positionDifference} || Position: {pos.X}, {pos.Y}, {pos.Z}");
                //string difference = "none";
                //string isRunnerAtHighSpeedForALongTimeDifference = "none";
                if (positionDifference > highSpeedSpeed) //highspeedspeed 10 ||checks if the player is going above the set speedlimit.
                {
                    if (isRunnerAtHighSpeedForALongTime < highSpeedBlipTimeAddTrigger) //60 || checks if the player is driving at high speed for an x amount of time, if not it adds an x amount of value to the time.
                    {
                        isRunnerAtHighSpeedForALongTime = isRunnerAtHighSpeedForALongTime + highSpeedBlipTimeAdd; //2
                        //isRunnerAtHighSpeedForALongTimeDifference = "yes";
                    }
                    if (distanceToBlipTakingSpeedIntoAccount >= highSpeedBlipMinimumDistance && isRunnerAtHighSpeedForALongTime >= highSpeedBlipTimeAddTrigger) //80 and 60
                    {
                        if (distanceToBlipTakingSpeedIntoAccount - highSpeedBlipDistanceSubtraction < highSpeedBlipMinimumDistance)
                        {
                            distanceToBlipTakingSpeedIntoAccount = highSpeedBlipMinimumDistance;
                        }
                        else
                        {
                        distanceToBlipTakingSpeedIntoAccount = distanceToBlipTakingSpeedIntoAccount - highSpeedBlipDistanceSubtraction; //-10
                        }
                        //difference = "-10";
                    }
                }
                else //player is driving slower than the set speed limit.
                {
                    if (isRunnerAtHighSpeedForALongTime > 1) //lowers the time spend at high speed
                    { 
                        if (isRunnerAtHighSpeedForALongTime < highSpeedBlipTimeSubtract) 
                        {
                            isRunnerAtHighSpeedForALongTime = 0;
                        }
                        else
                        {
                        isRunnerAtHighSpeedForALongTime = isRunnerAtHighSpeedForALongTime - highSpeedBlipTimeSubtract; //1
                        }
                       // isRunnerAtHighSpeedForALongTimeDifference = "no";
                    }
                    if (distanceToBlipTakingSpeedIntoAccount <= distanceToBlip - highSpeedBlipDistanceAddition && isRunnerAtHighSpeedForALongTime < highSpeedBlipTimeSubtractTrigger) //adds distance back to the blip up to it's set game length. Only after driving slower for a sertain amount of time.
                    {
                        distanceToBlipTakingSpeedIntoAccount = distanceToBlipTakingSpeedIntoAccount + highSpeedBlipDistanceAddition;
                        //difference = "+10";
                    }
                    else if (isRunnerAtHighSpeedForALongTime < highSpeedBlipTimeSubtractTrigger)
                    {
                        distanceToBlipTakingSpeedIntoAccount = distanceToBlip;
                    }
                }
               // NotificationScript.ShowNotification($"distance: {distanceToBlipTakingSpeedIntoAccount} | difference: {difference} | longTime: {isRunnerAtHighSpeedForALongTimeDifference} {isRunnerAtHighSpeedForALongTime}");
                
                if (positionDifference > 1)
                {
                    float count = 0;
                    delayLocationList.Add(new Vector4(pos.X, pos.Y, pos.Z, positionDifference));
                    bool didUpdateBlip = false;
                    List<Vector4> itemsToRemove = new List<Vector4>();

                    for (int i = delayLocationList.Count - 1; i >= 0; i--)
                    {
                        Vector4 line = delayLocationList[i];
                        Vector3 position = new Vector3(line.X, line.Y, line.Z);
                        float distanceBetweenEntry = line.W;

                        if (count + distanceBetweenEntry < distanceToBlipTakingSpeedIntoAccount)
                        {
                            count += distanceBetweenEntry;
                        }
                        else if (!didUpdateBlip)
                        {
                            didUpdateBlip = true;
                            blipPosition = position;
                        }
                        else
                        {
                            itemsToRemove.Add(line);
                        }
                    }

                    foreach (var item in itemsToRemove)
                    {
                        delayLocationList.Remove(item);
                    }

                    itemsToRemove.Clear();
                    lastPos = pos;
                    TriggerServerEvent("updateDelayBlip", blipPosition, true);
                }
                await WaitFor50Miliseconds(6); //usually 6 for a 300ms update time
                //Debug.WriteLine($"client GetBlipPositionForDelaymode updated blip position");
            }
            //TriggerServerEvent("updateDelayBlip", blipPosition, false);
            // Debug.WriteLine($"Ending getBlipLocationForDelayMode");
            delayLocationList.Clear();
        }

        private async Task WaitFor50Miliseconds(int seconds)
        {
            int targetTime = Environment.TickCount + (seconds * 50);
            while (Environment.TickCount < targetTime)
            {
                await Delay(1);
            }
        }

        [EventHandler("updateBlipLocationOnMapForDelayMode")]
        public void updateBlipLocationOnMapForDelayMode(Vector3 coordsForDelayBlip)
        {
            //Debug.WriteLine($"updateBlipLocationOnMapForDelayMode: updated!");
            RemoveBlip(ref delayBlipHandle);
            if (RoundHandling.gameMode == "delay")
            {
                delayBlipHandle = AddBlipForCoord(coordsForDelayBlip.X, coordsForDelayBlip.Y, coordsForDelayBlip.Z);
                BeginTextCommandSetBlipName("STRING");
                AddTextComponentSubstringPlayerName("Runner");
                EndTextCommandSetBlipName(delayBlipHandle);
            }
        }
    }
}