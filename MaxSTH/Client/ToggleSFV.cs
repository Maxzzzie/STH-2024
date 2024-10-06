using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace STH_Maxzzzie.Client
{
    public class ToggleSFV : BaseScript
    {
        private bool _enabled = false;

        public ToggleSFV()
        {
            Tick += OnTick;
        }

        [EventHandler("disableCanPlayerShootFromVehicles")]
        private void OnSFVToggled(bool enabled)
        {
            _enabled = enabled;
        }

        private Task OnTick()
        {
            if (!_enabled)
                return Task.FromResult(0);
            WeaponHash currentWeapon = Game.PlayerPed.Weapons.Current?.Hash ?? WeaponHash.Unarmed;
            bool canDriveBy = currentWeapon == WeaponHash.StickyBomb || currentWeapon == WeaponHash.Unarmed;
            API.SetPlayerCanDoDriveBy(LocalPlayer.Handle, canDriveBy);
            return Task.FromResult(0);
        }
    }
}
