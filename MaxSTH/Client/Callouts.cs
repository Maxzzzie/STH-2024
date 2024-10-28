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

        public Callouts()
        {
            Tick += OnTick; // Register the OnTick method to run every frame
        }

        [EventHandler("getMaxzzzieCalloutsDict")]
        void getMaxzzzieCalloutsDict(string calloutsName, Vector3 calloutsLocation)
        {
            maxzzzieCalloutsDict[calloutsName] = calloutsLocation;
            Debug.WriteLine($"getMaxzzzieCalloutsDict in client -> callout name: {calloutsName} location: {calloutsLocation}");
        }

        private async Task OnTick()
        {
            if (maxzzzieCalloutsDict.Count == 0)
                return;

            Vector3 playerPosition = Game.PlayerPed.Position;
            var closestCallout = maxzzzieCalloutsDict
                .OrderBy(callout => Vector3.Distance(playerPosition, callout.Value))
                .FirstOrDefault();

            string closestCalloutName = closestCallout.Key;
            Vector3 closestCalloutLocation = closestCallout.Value;

            // Display the closest callout on the screen
            DisplayCallout(closestCalloutName);

            await Task.FromResult(0); // Required to yield control in async method
        }

        private void DisplayCallout(string calloutText)
        {
            if (string.IsNullOrEmpty(calloutText))
                return;

            SetTextFont(4); // Set font type
            SetTextProportional(false);
            SetTextScale(0.5f, 0.5f); // Adjust text size
            SetTextColour(255, 255, 255, 255); // Set text color to white
            SetTextJustification(0); // Right justify text
            SetTextWrap(0.0f, 1.0f); // Ensure text wraps within the screen boundaries
            SetTextOutline(); // Add outline for better readability
            SetTextEntry("STRING");
            AddTextComponentString($"{calloutText}");
            DrawText(0.5f, 0.05f);
        }
    }
}

// using System;
// using System.Threading.Tasks;
// using CitizenFX.Core;
// using CitizenFX.Core.Native;
// using static CitizenFX.Core.Native.API;
// using System.Collections.Generic;
// using System.Dynamic;
// using System.Linq;

// namespace STHMaxzzzie.Client
// {
//     public class Callouts : BaseScript
//     {
//          private Dictionary<string, Vector3> maxzzzieCalloutsDict = new Dictionary<string, Vector3>();

//         [EventHandler("getMaxzzzieCalloutsDict")]
//         void getMaxzzzieCalloutsDict(string calloutsName, Vector3 calloutsLocation)
//         {
//             maxzzzieCalloutsDict.Add(calloutsName, calloutsLocation);
//             Debug.WriteLine($"getMaxzzzieCalloutsDict in client -> callout name: {calloutsName} location: {calloutsLocation.ToString()}");
//         }
//     }
// }

