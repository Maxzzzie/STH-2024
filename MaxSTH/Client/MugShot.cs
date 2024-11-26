// using System;
// using System.Threading.Tasks;
// using CitizenFX.Core;
// using CitizenFX.Core.Native;

// namespace STHMaxzzzie.Client
// {
//     public class MugShot : BaseScript
//     {
//         private Vector3 mugshotModelPosition = new Vector3(402.848f, -996.826f, -99.900f);
//         private float cameraFov = 5.0f; // Zoom in for a close-up
//         private int customCamera = 0; // Camera handle
//         private int pedHandle = 0; // NPC handle
//         private bool isRunning = false;

//         public MugShot()
//         {
//             API.RegisterKeyMapping("+MugShotKey", "Show Mugshot", "keyboard", "m");
//         }

//         [Command("+MugShotKey")]
//         private void MugShotKeyIsPressed()
//         {
//             if (!Game.PlayerPed.IsAlive || API.IsPauseMenuActive() || isRunning)
//                 return;

//             TriggerServerEvent("mugShot");
//         }

//         [Command("-MugShotKey")]
//         private void MugShotKeyIsUnpressed()
//         { }//empty to prevent a msg in chat.

//         [EventHandler("MugShotEvent")]
//         private async void MugShotEvent(string modelName)
//         {
//             if (isRunning)
//                 return;

//             isRunning = true;

//             try
//             {
//                 FocusMugshotArea(); // Ensure the area is loaded
//                 await SpawnNPC(modelName); // Spawn the NPC model
//                 SetupCamera(); // Set up the camera
//                 await Delay(4000); // Wait for 4 seconds
//             }
//             finally
//             {
//                 ClearFocus(); // Clear the focus to reload the player's area
//                 ResetCamera(); // Reset the camera to default
//                 DespawnNPC(); // Remove the NPC
//                 isRunning = false;
//             }
//         }

//         private void FocusMugshotArea()
//         {
//             // Use SetFocusPosAndVel to ensure the area is loaded
//             API.SetFocusPosAndVel(mugshotModelPosition.X, mugshotModelPosition.Y-1, mugshotModelPosition.Z, 0.0f, 0.0f, 0.0f);
//         }

//         private void ClearFocus()
//         {
//             API.ClearFocus(); // Reset focus back to the player's area
//         }

//         private async Task SpawnNPC(string modelName)
//         {
//             uint modelHash = (uint)API.GetHashKey(modelName);

//             // Request and load the NPC model
//             API.RequestModel(modelHash);
//             while (!API.HasModelLoaded(modelHash))
//             {
//                 await Delay(50);
//             }

//             // Spawn the NPC
//             pedHandle = API.CreatePed(4, modelHash, mugshotModelPosition.X, mugshotModelPosition.Y, mugshotModelPosition.Z, 178.805f, true, false);

//             // Set all components to default
//             for (int componentId = 0; componentId < 12; componentId++)
//             {
//                 API.SetPedComponentVariation(pedHandle, componentId, 0, 0, 0);
//             }

//             // Release model from memory
//             API.SetModelAsNoLongerNeeded(modelHash);
//         }

//         private void SetupCamera()
//         {
//             if (pedHandle == 0)
//             {
//                 Debug.WriteLine("NPC not found, camera setup aborted.");
//                 return;
//             }

//             // Create or reuse the custom camera
//             if (customCamera == 0)
//             {
//                 customCamera = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
//             }

//             // Attach the camera to the ped's head/neck (bone index 31086 = head)
//             API.AttachCamToPedBone(customCamera, pedHandle, 31086, 0.0f, 4.8f, 0.0f, true); // Adjust offset as needed
//             API.SetCamFov(customCamera, cameraFov);
//             API.PointCamAtPedBone(customCamera, pedHandle, 31086, 0.0f, 0.0f, 0.09f, true);

//             // Enable the custom camera
//             API.RenderScriptCams(true, false, 0, true, false);
//         }

//         private void ResetCamera()
//         {
//             // Return to the player's default camera view
//             API.RenderScriptCams(false, false, 0, true, false);

//             if (customCamera != 0)
//             {
//                 API.DestroyCam(customCamera, false);
//                 customCamera = 0;
//             }
//         }

//         private void DespawnNPC()
//         {
//             if (API.DoesEntityExist(pedHandle))
//             {
//                 API.DeletePed(ref pedHandle);
//                 pedHandle = 0;
//             }
//         }
//     }
// }

using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class MugShot : BaseScript
    {
        private Vector3 mugshotModelPosition = new Vector3(402.848f, -996.826f, -99.900f);
        private float cameraFov = 5.0f; // Zoom in for a close-up
        private int customCamera = 0; // Camera handle
        private int pedHandle = 0; // NPC handle
        private bool isRunning = false;

        public MugShot()
        {
            API.RegisterKeyMapping("+MugShotKey", "Show Mugshot", "keyboard", "m");
        }

        [Command("+MugShotKey")]
        private void MugShotKeyIsPressed()
        {
            if (!Game.PlayerPed.IsAlive || API.IsPauseMenuActive() || isRunning)
                return;

            TriggerServerEvent("mugShot");
        }

        [Command("-MugShotKey")]
        private void MugShotKeyIsUnpressed()
        { } // Empty to prevent chat message spam

        [EventHandler("MugShotEvent")]
        private async void MugShotEvent(string modelName)
        {
            if (isRunning)
                return;

            isRunning = true;

            try
            {
                await SpawnNPC(modelName); // Spawn the NPC model
                FocusMugshotArea(); // Ensure the area is visually loaded
                SetupCamera(); // Set up the camera
                await Delay(4000); // Wait for 4 seconds
            }
            finally
            {
                ResetCamera(); // Reset the camera to default
                DespawnNPC(); // Remove the NPC
                ClearFocus(); // Clear the focus to reload the player's area
                isRunning = false;
            }
        }

        private void FocusMugshotArea()
{
    if (pedHandle != 0 && API.DoesEntityExist(pedHandle))
    {
        // Focus on the NPC ped to prioritize loading its area
        API.SetFocusEntity(pedHandle);
    }
    else
    {
        Debug.WriteLine("Ped handle is invalid. Focus not set.");
    }
}

        private void ClearFocus()
        {
            // Reset focus back to the player's current position
            API.ClearFocus();
        }

        private async Task SpawnNPC(string modelName)
        {
            uint modelHash = (uint)API.GetHashKey(modelName);

            // Request and load the NPC model
            API.RequestModel(modelHash);
            while (!API.HasModelLoaded(modelHash))
            {
                await Delay(50);
            }

            // Spawn the NPC
            pedHandle = API.CreatePed(4, modelHash, mugshotModelPosition.X, mugshotModelPosition.Y, mugshotModelPosition.Z, 178.805f, true, false);

            // Set all components to default
            for (int componentId = 0; componentId < 12; componentId++)
            {
                API.SetPedComponentVariation(pedHandle, componentId, 0, 0, 0);
            }

            // Release model from memory
            API.SetModelAsNoLongerNeeded(modelHash);
        }



        private void SetupCamera()
        {
            if (pedHandle == 0)
            {
                Debug.WriteLine("NPC not found, camera setup aborted.");
                return;
            }

            // Create or reuse the custom camera
            if (customCamera == 0)
            {
                customCamera = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            }

            // Attach the camera to the ped's head/neck (bone index 31086 = head)
            API.AttachCamToPedBone(customCamera, pedHandle, 31086, 0.0f, 4.8f, 0.0f, true); // Adjust offset as needed
            API.SetCamFov(customCamera, cameraFov);
            API.PointCamAtPedBone(customCamera, pedHandle, 31086, 0.0f, 0.0f, 0.09f, true);

            // Enable the custom camera
            API.RenderScriptCams(true, false, 0, true, false);
        }

        private void ResetCamera()
        {
            // Return to the player's default camera view
            API.RenderScriptCams(false, false, 0, true, false);

            if (customCamera != 0)
            {
                API.DestroyCam(customCamera, false);
                customCamera = 0;
            }
        }

        private void DespawnNPC()
        {
            if (API.DoesEntityExist(pedHandle))
            {
                API.DeletePed(ref pedHandle);
                pedHandle = 0;
            }
        }
    }
}

