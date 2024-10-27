using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace STHMaxzzzie.Client
{
    public class DelayMode : BaseScript
    {
        public bool delayModeOn = false;
        int distanceToBlip = 400;
        static List<Vector4> delayLocationList = new List<Vector4>();
        int delayBlipHandle = 0;


        [EventHandler("getBlipLocationForDelayMode")]
        public async void getBlipLocationForDelayMode(bool isDelayModeOn, int distance)
        {
            delayModeOn = isDelayModeOn;
            distanceToBlip = distance;
            Vector3 lastPos = Game.PlayerPed.Position;
            Vector3 blipPosition = Game.PlayerPed.Position;
            delayLocationList.Add(new Vector4(lastPos.X, lastPos.Y, lastPos.Z, 0));

            while (delayModeOn && !Game.PlayerPed.IsDead)
            {
                Vector3 pos = Game.PlayerPed.Position;
                float positionDifference = GetDistanceBetweenCoords(pos.X, pos.Y, pos.Z, lastPos.X, lastPos.Y, lastPos.Z, true);
                //Debug.WriteLine($"Count: {delayLocationList.Count} || Difference: {positionDifference} || Position: {pos.X}, {pos.Y}, {pos.Z}");

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

                        if (count + distanceBetweenEntry < distanceToBlip)
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
                    //RemoveBlip(ref delayBlipHandle);
                    //delayBlipHandle = AddBlipForCoord(blipPosition.X, blipPosition.Y, blipPosition.Z);
                    itemsToRemove.Clear();
                    lastPos = pos;
                    TriggerServerEvent("updateDelayBlip", blipPosition, true);
                    //Debug.WriteLine($"Setting a blip at {blipPosition.X}, {blipPosition.Y}, {blipPosition.Z}");
                }
                await WaitFor50Miliseconds(6);
            }
            TriggerServerEvent("updateDelayBlip", blipPosition, false);
            delayModeOn = false;
            //Debug.WriteLine($"Ending getBlipLocationForDelayMode");
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
        public void updateBlipLocationOnMapForDelayMode(Vector3 coordsForDelayBlip, bool isDelayModeOn)
        {
            //Debug.WriteLine($"updateBlipLocationOnMapForDelayMode: updated!");
            RemoveBlip(ref delayBlipHandle);
            if (isDelayModeOn)
            {
                delayModeOn = isDelayModeOn;
                delayBlipHandle = AddBlipForCoord(coordsForDelayBlip.X, coordsForDelayBlip.Y, coordsForDelayBlip.Z);
            }
            else delayModeOn = false;
        }
    }
}