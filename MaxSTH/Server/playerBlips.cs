using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Reflection;
using STHMaxzzzie.Server;

namespace STHMaxzzzie.Server
{
    public class playerBlips : BaseScript
    {
        bool showPlayerBlips = true;

        public playerBlips()
        {

        }


        [Command("pb", Restricted = true)] //normal restriction true 
        public void playerBlipHandling(int source, List<object> args, string raw)
        {
            showPlayerBlips = !showPlayerBlips;
            updatePlayerBlipsForClient();
        }

        public void updatePlayerBlipsForClient()
        {
            BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
            
            foreach (Player player in Players)
            {
                int serverId = int.Parse(player.Handle);
                string name = player.Name;
                bool friendly = true;
                if (RoundHandling.teamAssignment[serverId] == 1) //checks if player is runner
                {
                    friendly = false;
                }
                if(showPlayerBlips)
                {
                BlipHandler.BlipData playerblip = new BlipHandler.BlipData($"{name}-{serverId}")
                {
                    Type = "player",
                    Sprite = 57,
                    Colour = 3,
                    IsShortRange = false,
                    MapName = name,
                    IsFriendly = friendly
                };
                request.BlipsToAdd.Add(playerblip);
                }
                else
                {
                    request.BlipsToRemove.Add($"{name}-{serverId}");
                }
            }
            BlipHandler.AddBlips(request);
        }
    }
}