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

        public Armoury()
        {

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
                //Debug.WriteLine($"Weapon's are turned off."); //this msg is now handled by the server.
            }
            if (isWeaponAllowed == true)
            {
                TriggerEvent("giveweapon");
                //Debug.WriteLine($"Weapon's are turned on."); //this msg is now handled by the server.
            }

        }


        //Giving a weapon
        [Command("weapon")]
        [EventHandler("giveweapon")]
        public void giveWeapon()
        //Void doesn't return anything. These are datatypes. (var/int/string/bool/double/char/my_own!)

        {
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

                TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"You have general weapons."}});
            }
            //if (isWeaponAllowed) checks for true/false. And continues with the body if it's true.
            else
            {
             TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Weapon command is currently off."}});
            }
        }

        //Giving a weapon
        [Command("huntweapon")]
        [EventHandler("huntweapon")]
        void giveHuntWeapon()
        //Void doesn't return anything. These are datatypes. (var/int/string/bool/double/char/my_own!)
        {
            //checks if weapons are enabled
            if (isWeaponAllowed)
            {
                //actualy does the giving of weapons

                Game.PlayerPed.Weapons.RemoveAll();
                Game.PlayerPed.Weapons.Give(WeaponHash.Firework, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.RayPistol, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.FlareGun, 25, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.PumpShotgun, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.SawnOffShotgun, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Flare, 25, false, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Nightstick, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Knife, 1, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Musket, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Pistol, 500, true, false);
                Game.PlayerPed.Weapons.Give(WeaponHash.Parachute, 1, true, false);

                TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"You now have hunter weapons."}});
            }
            else
            {
               TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Huntweapon command is currently off."}});
            }
        }
        //Giving all weapons
        [Command("allweapon")]
        void giveAllWeapon()
        {
            if (isWeaponAllowed)
            {
                Game.PlayerPed.Weapons.RemoveAll();
                foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 999, false, true);
                }
                TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"You now have all weapons."}});
            }
            else
            {
                TriggerEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Allweapon command is currently off."}});
            }
        }

        [Command("delweapon")]
        void delWeapon()
        {
            Game.PlayerPed.Weapons.RemoveAll();
        }
    }
}