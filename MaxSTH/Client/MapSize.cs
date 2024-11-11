using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
using System.Linq;

namespace STHMaxzzzie.Server
{
    public class MapSize : BaseScript
    {
        static int currentMode = 2;

        [EventHandler("setClientMapSize")]
        public static void setClientMapSize()
        {
            if (currentMode == 1)
            {
                API.SetBigmapActive(false, false);
                currentMode = 2;
            }
            else if (currentMode == 2)
            {
                API.SetBigmapActive(true, false);
                currentMode = 3;
            }
            else if (currentMode == 3)
            {
                API.SetBigmapActive(true, true);
                currentMode = 1;
            }
        }
    }
}