using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
        public class Test : BaseScript
        {
                [Command("test", Restricted = false)]
                void TestCommand(int source, List<object> args, string raw)
                {
                        // BlipHandler.UpdateBlipsRequest request = new BlipHandler.UpdateBlipsRequest();
                        // if (args[0].ToString() == "add")
                        //         foreach (Player player in Players)
                        //         {

                        //                 BlipHandler.BlipData playerblip = new BlipHandler.BlipData($"{player.Name}-{player.Handle}")
                        //                 {
                        //                         Type = "player",
                        //                         Colour = 6,
                        //                         Shrink = false,
                        //                         MapName = player.Name,
                        //                         Sprite = 64
                        //                 };
                        //                 request.BlipsToAdd.Add(playerblip);

                        //         }
                        // else if (args[0].ToString() == "del")
                        // {
                        //         foreach (Player player in Players)
                        //         {
                        //                 request.BlipsToRemove.Add($"{player.Name}-{player.Handle}");
                        //         }
                        // }
                        //         BlipHandler.AddBlips(request);
                }
                [Command("rpi", Restricted = true)]
                void rpi(int source, List<object> args, string raw)
                {
                        TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Did you mean to do /pri?" } });
                        foreach (Player notGil in Players)
                                if (notGil != Players[source])
                                {
                                        TriggerClientEvent(notGil, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Hahaha, Gilly wrote /pri wrong! He typed /rpi." } });
                                }
                }


        }
}
