using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class Callouts : BaseScript
    {
         private Dictionary<string, Vector3> maxzzzieCalloutsDict = new Dictionary<string, Vector3>();

        [EventHandler("getMaxzzzieCalloutsDict")]
        void getMaxzzzieCalloutsDict(string calloutsName, Vector3 calloutsLocation)
        {
            maxzzzieCalloutsDict.Add(calloutsName, calloutsLocation);
            Debug.WriteLine($"name: {calloutsName} location: {calloutsLocation.ToString()}");
            int calloutblip = AddBlipForCoord(calloutsLocation.X,calloutsLocation.Y,calloutsLocation.Z);
            SetBlipSprite(calloutblip,128);
            SetBlipColour(calloutblip, 1);
            SetBlipAsShortRange(calloutblip, true);
        }
    }

}

