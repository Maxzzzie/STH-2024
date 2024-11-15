using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class MapSize : BaseScript
    {
        static int currentMode = 2;

public MapSize()
        {
            API.RegisterKeyMapping("+SetMapSize", "Change radar size.", "keyboard", "z");
        }

        //sends f6 press to server
        [Command("+SetMapSize")]
        void setMapSizeIsPressed()
        {
            if (Game.PlayerPed.IsAlive == false || API.IsPauseMenuActive())
            {
                return;
            }
            setClientMapSize(true);
        }

        //does nothing but prevends an error msg upon release of the key.
        [Command("-SetMapSize")]
        void setMapSizeIsUnpressed() { } //add empty handler so it doesn't show up in chat. 

        [EventHandler("setClientMapSize")]
        public static void setClientMapSize(bool wasThisKeypress)
        {
            if (currentMode == 1 || (currentMode == 3 && wasThisKeypress)) //excludes going to big map when it was a keypress
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