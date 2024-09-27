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

    public class Wanted : BaseScript
    {
        //cops false turns off cops
        int maxWtdLvl = 0;

        public Wanted()
        {
            API.SetMaxWantedLevel(maxWtdLvl);
        }


        [Command("wanted")]
        void wantedLvl(int source, List<object> args, string raw)
        {
            int wanted = Game.Player.WantedLevel;

            //sets wanted lvl to 0 if player has one.
            if (args.Count == 0)
            {
                if (wanted == 0)
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You already had no cops. Did you mean to do something else? Type \"/wanted help\"." } });
                    return;
                }
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You cleared your wantedlevel." } });
                Game.Player.WantedLevel = 0;
            }

            //checks if arg 0 is an int to prevent errors.
            else if (args.Count == 1)
            {
                int temp;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out temp);

                //wanted 0-5
                if (isArgs0Int && (int.Parse(args[0].ToString()) >= 0 || int.Parse(args[0].ToString()) >= 5))
                {
                    //if (maxWtdLvl == 0 && int.Parse(args[0].ToString()) != 0)
                    if (maxWtdLvl < int.Parse(args[0].ToString()))
                    {
                        maxWtdLvl = int.Parse(args[0].ToString());
                        //TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Cops are turned on because you requested a wanted lvl." } });
                    }
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Your wanted lvl was {wanted} and is now set to {int.Parse(args[0].ToString())}" } });
                    API.SetMaxWantedLevel(maxWtdLvl);
                    Game.Player.WantedLevel = int.Parse(args[0].ToString());
                    return;
                }

                //wanted on
                else if (args[0].ToString() == "on")
                {
                    if (maxWtdLvl == 5)
                    {
                        TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Cops were already on." } });
                        return;
                    }
                    maxWtdLvl = 5;
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Cops are turned on." } });
                }

                //wanted off
                else if (args[0].ToString() == "off")
                {
                    if (maxWtdLvl == 0)
                    {
                        TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Cops were already off." } });
                        return;
                    }
                    maxWtdLvl = 0;
                    Game.Player.WantedLevel = 0;
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Cops are turned off." } });
                }

                //wanted help
                else if (args[0].ToString() == "help")
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"/wanted controls the cops for you.\nYou should do \"/wanted off/on/value\".\nTyping just \"/wanted\" clears the cops for you." } });
                }
                else
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Oh no. Something went wrong!\nYou should do \"/wanted off/on/value\" or \"/wanted\"." } });
                }
            }
            //wanted max (value 0-5)
            else if (args.Count == 2 && args[0].ToString() == "max")
            {
                int temp;
                bool isArgs1Int = Int32.TryParse(args[1].ToString(), out temp);

                if (isArgs1Int == true && (int.Parse(args[1].ToString()) >= 0 && int.Parse(args[1].ToString()) <= 5))
                {

                    maxWtdLvl = int.Parse(args[1].ToString());
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You set the highest wanted lvl you can obtain to {int.Parse(args[1].ToString())}" } });
                    Game.Player.WantedLevel = 0;
                }
                else
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Oh no. Something went wrong!\nYou should do \"/wanted max value\". Where value can be 0-5." } });
                }
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Oh no. Something went wrong!\nYou should do \"/wanted off/on/value\"." } });
            }

            API.SetMaxWantedLevel(maxWtdLvl);
        }
    }
}