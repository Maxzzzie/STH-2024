using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace STH_Maxzzzie.Client
{
    public class ToggleSFV : BaseScript
    {
        private bool isShootingFromVehicleAllowed = false;

        public ToggleSFV()
        {
            Tick += OnTick;
        }

        [EventHandler("disableCanPlayerShootFromVehicles")]
        private void OnSFVToggled(bool shootingFromVehicleAllowed)
        {
            isShootingFromVehicleAllowed = shootingFromVehicleAllowed;
        }

        private Task OnTick()
        {
            if (isShootingFromVehicleAllowed)
            {
                API.SetPlayerCanDoDriveBy(LocalPlayer.Handle, true);
                return Task.FromResult(0);
            }
            WeaponHash currentWeapon = Game.PlayerPed.Weapons.Current?.Hash ?? WeaponHash.Unarmed;
            bool canDriveBy = currentWeapon == WeaponHash.StickyBomb || currentWeapon == WeaponHash.Unarmed || currentWeapon == WeaponHash.FlareGun || currentWeapon == WeaponHash.Flare;
            API.SetPlayerCanDoDriveBy(LocalPlayer.Handle, canDriveBy);
            return Task.FromResult(0);
        }
    }
}
