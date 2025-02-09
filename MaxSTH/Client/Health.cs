using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Mono.CSharp;

namespace STHMaxzzzie.Client
{
    public class Health : BaseScript
    {

        public Health()
        {
            RefillStamina();
        }

        // Function 1: Hurt player for 10% of their current health
        [EventHandler("HurtPlayer")]
        private void HurtPlayer()
        {
            int playerPed = API.PlayerPedId();
            int currentHealth = API.GetEntityHealth(playerPed);
            API.SetEntityHealth(playerPed, Math.Max(5, currentHealth - 10));
            //Debug.WriteLine($"Player hurt by 10. Current health: {API.GetEntityHealth(playerPed)}");
        }

        [EventHandler("CheckHealthStats")]
        private void CheckHealthStats()
        {
            int playerPed = API.PlayerPedId();

            // Retrieve health, armor, and stamina details
            float maxHealth = API.GetEntityMaxHealth(playerPed);
            float currentHealth = API.GetEntityHealth(playerPed);
            int maxArmor = 100; // Standard max armor in FiveM
            int currentArmor = API.GetPedArmour(playerPed); // Get current armor value
            float maxStamina = 100f; // Stamina in FiveM ranges between 0-100
            float currentStamina = API.GetPlayerSprintStaminaRemaining(API.PlayerId()); // Current stamina value

            // Log or display the retrieved stats
            Debug.WriteLine($"Player Stats:");
            Debug.WriteLine($"- Max Health: {maxHealth}. Current Health: {currentHealth}");
            Debug.WriteLine($"- Max Armor: {maxArmor}. Current Armor: {currentArmor}");
            Debug.WriteLine($"- Max Stamina: {maxStamina}");
            Debug.WriteLine($"- Current Stamina: {currentStamina}. Heal%: {API.GetPlayerHealthRechargeLimit(Game.Player.Handle) * 100}");
        }

        [EventHandler("HealCompletely")]
        private void HealCompletely()
        {
            API.SetPlayerHealthRechargeLimit(Game.Player.Handle, 1);
        }

        [EventHandler("HealHalf")]
        private void HealHalf()
        {
            API.SetPlayerHealthRechargeLimit(Game.Player.Handle, 0.33f);
        }

        //client usage is Health.SetPlayerStats(300, 100);
        [EventHandler("SetPlayerStats")]
        public static void SetPlayerStats(int setHealthTo, int setArmourTo)
        {
            int playerPed = API.PlayerPedId();

            API.SetPedMaxHealth(playerPed, 300); // Set maximum health
            API.SetPlayerMaxStamina(playerPed, 100);
            API.SetEntityHealth(playerPed, setHealthTo);    // Set current health
            API.SetPedArmour(playerPed, setArmourTo);        // Set armor
            Debug.WriteLine($"Player health and armour updated. {setHealthTo}, {setArmourTo}");
        }

        [EventHandler("RefillStamina")]
        // Function 4: Monitor and refill stamina before it depletes fully
        private async void RefillStamina()
        {
            int playerId = API.PlayerId();
            while (true)
            {
                await Delay(1000); // Check every 1000ms
                float currentStamina = API.GetPlayerSprintStaminaRemaining(playerId);

                if (currentStamina > 70f) // Threshold to refill stamina
                {
                    API.SetPlayerStamina(playerId, 30);
                    //Debug.WriteLine("Stamina refilled!");
                }
            }
        }
    }
}