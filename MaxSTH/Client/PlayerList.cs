using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;

namespace STHMaxzzzie.Client
{
    public class PlayerList : BaseScript
    {
        bool isKeyPressed = false;
        public PlayerList()
        {
            RegisterKeyMapping("+playerList", "Show player list", "keyboard", "o"); // Change "o" to your desired key.
        }

        [Command("+playerList")]
        private void playerListPress()
        {
            if (IsPauseMenuActive() || isKeyPressed)
            {
                return;
            }
                isKeyPressed = true;
                playerListCommand();
            }
        

        [Command("-playerList")]
        private void playerListRelease()
        {
            if (isKeyPressed) isKeyPressed = false;
            // No action needed for release in this context. But prevents a msg
        }

        private void playerListCommand()
        {
            TriggerServerEvent("playerList", Game.Player.ServerId, new List<string>(), "null");
        }
    }
}