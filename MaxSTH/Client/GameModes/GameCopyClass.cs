using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using STHMaxzzzie.Client;
using System.Linq;

namespace STHMaxzzzie.Client
{
    public class GameCopyClass : BaseScript
    {
        //Detect vehicle change
        //Check for class of vehicle
        //Send server class.
        //Receive randomized vehicle of that class.
        //Check and keep checking if player is in the same vehicle.
        public static string currentClass = "emergency";
        public static bool forceChosenVehicleOnly = false;
        static string chosenVehName = "none";
        static bool classDidChange = false;

        public GameCopyClass()
        {
            TriggerServerEvent("sendClientCopyClassSettings");
            startCopyClass(); //both functions don't do anything if copy class isn't active.
        }

        public static void startCopyClass()
        {
            DetectVehicleClassChange();
            ForceCurrentVehicleClass();
        }

        [EventHandler("UpdateCopyClassSettings")]
        static void UpdateCopyClassSettings(string newCurrentClass, bool newForceChosenVehicleOnly)
        {
            if (currentClass != newCurrentClass)
            {
            currentClass = newCurrentClass;
                chosenVehName = VehicleHandler.GetRandomVehicleFromClass(currentClass);
                Debug.WriteLine($"New vehicle for client = {chosenVehName} from {currentClass}");
            }
            forceChosenVehicleOnly = newForceChosenVehicleOnly;
        }

        [EventHandler("UpdateCopyClassClass")]
        static void UpdateCopyClassClass(string newCurrentClass)
        {
            currentClass = newCurrentClass;
            chosenVehName = VehicleHandler.GetRandomVehicleFromClass(currentClass);
            Debug.WriteLine($"New vehicle for client = {chosenVehName}, {currentClass}");
            classDidChange = true;
        }

        async static void DetectVehicleClassChange()
        {
            Vehicle veh = Game.PlayerPed.CurrentVehicle;

            while (RoundHandling.gameMode == "copyclass" && RoundHandling.thisClientIsTeam == 1)
            {
                Vehicle newVeh = Game.PlayerPed.CurrentVehicle;
                if (newVeh == null || Game.PlayerPed.IsOnFoot)
                {
                    await Delay(200);
                    continue;
                }
                string newVehClass = newVeh.ClassLocalizedName.ToLower().Replace(" ", "").Trim();
                if (newVeh != veh) //detects vehicle change.
                {
                    veh = newVeh;
                    Debug.WriteLine($"New vehicle detected from the same class. Class {newVehClass}, {newVehClass}");
                }
                if (newVehClass != currentClass)
                {
                    NotificationScript.ShowNotification($"New vehicle class detected.\n~r~{currentClass}\n~g~{newVehClass}");
                    currentClass = newVehClass;
                    TriggerServerEvent("sendClientCopyClassClass", currentClass);
                }
                await Delay(500);
            }
        }


        async static void ForceCurrentVehicleClass()
        {
            Vehicle spawnedVeh = Game.PlayerPed.CurrentVehicle;

            while (RoundHandling.gameMode == "copyclass" && RoundHandling.thisClientIsTeam == 2)
            {
                Vehicle currentVeh = Game.PlayerPed.CurrentVehicle;
                if (currentVeh == null || Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
                {
                    await Delay(200);
                    continue;
                }
                string currentVehClass = currentVeh.ClassLocalizedName.ToLower().Replace(" ", "").Trim();
                string currentVehName = currentVeh.LocalizedName.ToLower().Trim(); // Trim spaces to avoid mismatches

                //Debug.WriteLine($"{forceChosenVehicleOnly} force. {chosenVehName} chosenVehName. {currentVehName} newVehName. {currentClass} currentclass. {currentVehClass} newVehClass.");

                bool case1 = classDidChange;                                        //change when class has changed.
                bool case2 = spawnedVeh != currentVeh && forceChosenVehicleOnly;    //Change when player is in not the exact vehicle he got given and force exact vehicle is on.
                bool case3 = currentClass != currentVehClass;                       //Change when player is in the wrong class.
                //if (case2 || case3)
                Debug.WriteLine($"1 {case1} + 2 {case2} + 3 {case3}");
                if (case1 || case2 || case3)
                {
                    VehicleHandler.SetPlayerIntoNewVehicle(chosenVehName);

                    await Delay(1000); // Wait for vehicle switch to complete

                    // Now get the actual assigned vehicle name
                    Vehicle assignedVeh = Game.PlayerPed.CurrentVehicle;
                    if (assignedVeh != null)
                    {
                        spawnedVeh = assignedVeh;
                    }
                    classDidChange = false;
                }

                await Delay(1000);
            }
        }

    }
}