// // Client-side script (ClientNotification.cs)
// using System;
// using System.Threading.Tasks;
// using CitizenFX.Core;
// using CitizenFX.Core.Native;

// public class ClientNotification : BaseScript
// {
//     public ClientNotification()
//     {
//         // Register the event handler for showing notifications
//         EventHandlers["ShowNotification"] += new Action<string, int[], bool>(ShowNotification);
//     }

//     private async void ShowNotification(string text, int[] color, bool blink = false)
//     {
//         // Log the notification call
//         Debug.WriteLine($"ShowNotification called with text: {text}, color: [{color[0]}, {color[1]}, {color[2]}], blink: {blink}");

//         // Set up the notification
//         API.SetNotificationTextEntry("STRING");
//         API.AddTextComponentSubstringPlayerName(text);
//         API.SetNotificationBackgroundColor(0); // Default background color

//         // Set notification flash color
//         API.SetNotificationFlashColor(color[0], color[1], color[2], 255); // RGB Flash Color

//         // Draw the notification
//         API.DrawNotification(blink, false);
//         Debug.WriteLine("Notification should now be drawn.");

//         // Wait for 5 seconds to ensure visibility
//         await BaseScript.Delay(5000); // Delay for 5 seconds
//     }
// }
