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
    public class Armoury : BaseScript
    {
        bool isWeaponAllowed = true;
        bool isPvpAllowed = false;
        string lastWeaponClass = "hunt";

        [Command("weapon")]
        public void weaponCommand(int source, List<object> args, string raw)
        {
            if (args.Count == 1)
            {
                string input = args[0].ToString();
                if (input == "hunt")
                {
                    giveHuntWeapon(false);
                }
                else if (input == "run")
                {
                    giveRunWeapon(false);
                }
                else if (input == "all")
                {
                    giveAllWeapon(false);
                }

                else
                {
                    giveHuntWeapon(true); //i'm giving default weapons anyway. Might as well. Without a msg to it.
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Something went wrong. You should type /weapon hunt/run/all." } });
                }
            }
            else if (args.Count == 0)
            {
                handleLastWeaponClass(false);
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Something went wrong. You should type /weapon hunt/run/all." } });
            }
        }

        [EventHandler("lastWeaponClass")]
        void handleLastWeaponClass(bool model)
        //if we get this from the /model command, "model" is true.
        //This prevents the you now have weapons message.
        {
            if (lastWeaponClass == "hunt")
            {
                giveHuntWeapon(model);
            }
            else if (lastWeaponClass == "all")
            {
                giveAllWeapon(model);
            }
            else if (lastWeaponClass == "run")
            {
                giveRunWeapon(model);
            }
        }

        [EventHandler("updatePvp")]
        void updatePvp(bool updateIsPvpAllowed)
        {
            isPvpAllowed = updateIsPvpAllowed;
            NetworkSetFriendlyFireOption(isPvpAllowed);
            Debug.WriteLine($"Pvp is allowed is now set to {isPvpAllowed}");
        }


        [EventHandler("isWeaponAllowed")]
        void isWeaponAllowedHandler(bool isAllowed)
        {
            isWeaponAllowed = isAllowed;
            if (isWeaponAllowed == false)
            {
                Game.PlayerPed.Weapons.RemoveAll();
            }
            if (isWeaponAllowed == true)
            {
                TriggerEvent("lastWeaponClass", true);
            }

        }

        [EventHandler("giveWeapon")]
        void setGiveWeapon()
        {
            TriggerEvent("lastWeaponClass", false); //true to not show up a message when giving the class again.
        }


        //Giving a runweapon
        [EventHandler("runWeapon")]
        public void giveRunWeapon(bool model)
        //Void doesn't return anything. These are datatypes. (var/int/string/bool/double/char/my_own!)

        {
            lastWeaponClass = "run";
            //checks if weapons are enabled
            if (isWeaponAllowed)
            {
                //actualy does the giving of weapons

                Game.PlayerPed.Weapons.RemoveAll();
                Game.PlayerPed.Weapons.Give(WeaponHash.Firework, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.FlareGun, 25, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.RayPistol, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.PumpShotgun, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.SawnOffShotgun, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.SMG, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.StickyBomb, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Flare, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.StunGun, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Nightstick, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Knife, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Musket, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.CarbineRifle, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Pistol, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.SniperRifle, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.HomingLauncher, 5, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);

                if (!model)
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You have run weapons." } });
                }
            }
            //if (isWeaponAllowed) checks for true/false. And continues with the body if it's true.
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Weapon command is currently off." } });
            }
        }

        //Giving a weapon
        [EventHandler("huntWeapon")]
        void giveHuntWeapon(bool model)
        //Void doesn't return anything. These are datatypes. (var/int/string/bool/double/char/my_own!)
        {
            lastWeaponClass = "hunt";
            //checks if weapons are enabled
            if (isWeaponAllowed)
            {
                //actualy does the giving of weapons

                Game.PlayerPed.Weapons.RemoveAll();
                Game.PlayerPed.Weapons.Give(WeaponHash.Firework, 5, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.RayPistol, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.FlareGun, 25, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.PumpShotgun, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.SawnOffShotgun, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Flare, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Nightstick, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Knife, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Musket, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Pistol, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);

                if (!model)
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You now have hunter weapons." } });
                }
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Huntweapon command is currently off." } });
            }
        }
        //Giving all weapons
        [EventHandler("allWeapon")]
        void giveAllWeapon(bool model)
        {
            lastWeaponClass = "all";
            if (isWeaponAllowed)
            {
                Game.PlayerPed.Weapons.RemoveAll();
                foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 999, false, true);
                }
                if (!model)
                {
                    TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"You now have all weapons." } });
                }
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"Allweapon command is currently off." } });
            }
        }

        [Command("delweapon")]
        void delWeapon()
        {
            Game.PlayerPed.Weapons.RemoveAll();
        }
    }
}