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

        // // Function 1: Hurt player for 10% of their current health
        // [EventHandler("HurtPlayer")]
        // private void HurtPlayer()
        // {
        //     int playerPed = API.PlayerPedId();
        //     int currentHealth = API.GetEntityHealth(playerPed);
        //     API.SetEntityHealth(playerPed, currentHealth - 10);
        //     Debug.WriteLine($"Player hurt by 10. Current health: {API.GetEntityHealth(playerPed)}");
        // }

        // [EventHandler("CheckHealthStats")]
        // private void CheckHealthStats()
        // {
        //     int playerPed = API.PlayerPedId();

        //     // Retrieve health, armor, and stamina details
        //     float maxHealth = API.GetEntityMaxHealth(playerPed);
        //     float currentHealth = API.GetEntityHealth(playerPed);
        //     int maxArmor = 100; // Standard max armor in FiveM
        //     int currentArmor = API.GetPedArmour(playerPed); // Get current armor value
        //     float maxStamina = 100f; // Stamina in FiveM ranges between 0-100
        //     float currentStamina = API.GetPlayerSprintStaminaRemaining(API.PlayerId()); // Current stamina value

        //     // Log or display the retrieved stats
        //     Debug.WriteLine($"Player Stats:");
        //     Debug.WriteLine($"- Max Health: {maxHealth}. Current Health: {currentHealth}");
        //     Debug.WriteLine($"- Max Armor: {maxArmor}. Current Armor: {currentArmor}");
        //     Debug.WriteLine($"- Max Stamina: {maxStamina}");
        //     Debug.WriteLine($"- Current Stamina: {currentStamina}");
        // }


        // Function 3: Set health, armor, and stamina to specified values
        [EventHandler("SetPlayerStats")]
        public static void SetPlayerStats()
        {
            int playerPed = API.PlayerPedId();

            API.SetEntityMaxHealth(playerPed, 225); // Set maximum health
            API.SetEntityHealth(playerPed, 200);    // Set current health
            API.SetPedArmour(playerPed, 100);        // Set armor
            API.SetPlayerMaxStamina(playerPed, 100);

            // Stamina is handled automatically by the game, but you can manage it here for control
            // There's no direct "set stamina" function in FiveM, but you can simulate full stamina management
            Debug.WriteLine($"Player health and armor added.");
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

                if (currentStamina > 60f) // Threshold to refill stamina
                {
                    API.ResetPlayerStamina(playerId);
                    //Debug.WriteLine("Stamina refilled!");
                }
            }
        }
    }
}