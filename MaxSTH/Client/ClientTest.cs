using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class killerPatch : BaseScript //sends the killer info to the server during onplayerkilled. Temp patch i hope
    {
        //void OnPlayerKilled([FromSource]Player victim, int killerID, ExpandoObject info)
        void OnPlayerKilled(int killerID, ExpandoObject info)
        {
            Debug.WriteLine($"killer from client: {killerID}");
            TriggerServerEvent("sendKillerIDToServer", killerID);
        }
    }
}
//      public class NotificationClient : BaseScript
// {
//     public NotificationClient()
//     {
//         EventHandlers["showNotification"] += new Action<string>(ShowNotification);
//     }

//     private void ShowNotification(string message)
//     {
//         API.SetNotificationTextEntry("STRING");
//         API.AddTextComponentString(message);
//         API.DrawNotification(false, true);
//     }
// }