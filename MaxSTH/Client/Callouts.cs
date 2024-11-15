using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class Callouts : BaseScript
    {
        private Dictionary<string, Vector3> maxzzzieCalloutsDict = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> nearbyCallouts = new Dictionary<string, Vector3>();
        private Vector3 lastPlayerPosition = Vector3.Zero;
        private string closestCalloutName = string.Empty;
        private bool isProcessing = false; // Flag to check if OnTick is already running
        private int calloutRange = 750; // Distance to store callouts in a temp dict.
        private int reloadRange = 500;  // Distance to move before the temp callouts dict gets remade.
        private bool triedOnce = false;
        private bool alreadyRefreshedServerResourceOnce = false;

        public Callouts()
        {
            Tick += OnTick;
            Tick += DisplayCalloutOnTick;
        }

        [EventHandler("getMaxzzzieCalloutsDict")]
        void getMaxzzzieCalloutsDict(string calloutsName, Vector3 calloutsLocation)
        {
            maxzzzieCalloutsDict[calloutsName] = calloutsLocation;
            // Debug.WriteLine($"Added callout: {calloutsName} at location: {calloutsLocation}");
        }

        private async Task OnTick()
        {
            if (isProcessing) return;
            
            isProcessing = true;

            Vector3 playerPosition = Game.PlayerPed.Position;

            // Check if the player has moved more than the reloadRange or if nearbyCallouts is empty
            if (Vector3.Distance(lastPlayerPosition, playerPosition) > reloadRange || nearbyCallouts.Count == 0)
            {
                if(nearbyCallouts.Count == 0 && triedOnce && !alreadyRefreshedServerResourceOnce)
                {
                    TriggerServerEvent("reloadResources");
                    triedOnce = false;
                    alreadyRefreshedServerResourceOnce = true;
                }
                triedOnce = true;
                // Update nearby callouts within calloutRange
                nearbyCallouts = maxzzzieCalloutsDict
                    .Where(callout => Vector3.Distance(playerPosition, callout.Value) <= calloutRange)
                    .ToDictionary(callout => callout.Key, callout => callout.Value);

                lastPlayerPosition = playerPosition; // Update last known position
                // Debug.WriteLine($"Updated nearby callouts. Found {nearbyCallouts.Count} callouts within {calloutRange} meters.");
            }

            // Find the closest callout from the nearby ones
            if (nearbyCallouts.Count > 0)
            {
                var closestCallout = nearbyCallouts
                    .OrderBy(callout => Vector3.Distance(playerPosition, callout.Value))
                    .FirstOrDefault();

                closestCalloutName = closestCallout.Key;
            }

            await Delay(500);
            isProcessing = false;

        }

        private async Task DisplayCalloutOnTick()
        {   string[] trimmedClosestCalloutName = closestCalloutName.Split('*');
            if (!string.IsNullOrEmpty(closestCalloutName))
            {
                SetTextFont(4); // Set font type
                SetTextProportional(false);
                SetTextScale(0.55f, 0.55f); // Adjust text size
                SetTextColour(255, 255, 255, 255); // Set text color to white
                SetTextJustification(0); // Center justify text
                SetTextWrap(0.0f, 1.0f); // Ensure text wraps within the screen boundaries
                SetTextOutline(); // Add outline for better readability
                SetTextEntry("STRING");
                AddTextComponentString($"{trimmedClosestCalloutName[0]}");
                DrawText(0.5f, 0.05f); // Centered near the top

               
                await Task.FromResult(0);
            }
        }
    }
}