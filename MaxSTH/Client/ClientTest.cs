using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class MugShot : BaseScript
    {
        //         //location mugshotroom player =     position: 402.848, -996.826, -99.000 rotation: 0.000, -0.000, 178.805 heading: 178.805
        //         //location camera =                 Position: 402.629, -1002.263, -99.004 rotation: 0.000, -0.000, 0.230 heading: 0.230

        private Vector3 mugshotRoomPosition = new Vector3(-2493.5f, -1500, 50.1f); //-2493, -1500, 51.1
        //private Vector3 mugshotRoomPosition = new Vector3(402.848f, -996.826f, -99.9f);
        //private Vector3 cameraPosition = new Vector3(402.85f, -999f, -98.4f);
        private Vector3 cameraPosition = new Vector3(-2495.6f, -1501.3f, 51.8f); //-2495.6f, -1501.3f, 51.8f
        private Vector3 cameraRotation = new Vector3(180f, 180f, 123f); //tilt, roll, pan (tilt 180 is level, 170 is up slightly) 180f, 180f, 123f
        private float cameraFov = 10f;
        private int customCamera;
        private int pedHandle;
        private bool isRunning = false;

        // Method that starts the mugshot sequence
        [EventHandler("MugShotEvent")]
        private async void MugShotEvent(string modelName)
        {
            Debug.WriteLine($"Mugshot process with {modelName}");
            if (isRunning)
            {
                // Prevent command spamming
                Debug.WriteLine("Mugshot process is already running.");
                return;
            }

            isRunning = true;

            await SpawnPresetNPC(modelName); // Pass the model name dynamically
            await ForceLoadMugshotArea();
            await SetCamera();

            DespawnNPC();

            isRunning = false;
        }

        // Force loading the mugshot area
        private async Task ForceLoadMugshotArea()
        {
            Debug.WriteLine("Mugshot 4");
            while (pedHandle == 0)
            {
                Debug.WriteLine("Mugshot 3");
                await Delay(10);
            }
            API.RequestCollisionForModel((uint)pedHandle);
            while (!API.HasCollisionLoadedAroundEntity(pedHandle))
            {
                Debug.WriteLine("Mugshot 5");
                await Delay(10); 
            }
        }

        // Spawn an NPC with the given model name
        public async Task SpawnPresetNPC(string modelName)
        {
            uint modelHash = (uint)API.GetHashKey(modelName);

            // Request the model
            API.RequestModel(modelHash);

            // Wait until the model is loaded
            while (!API.HasModelLoaded(modelHash))
            {
                await Delay(50);
            }

            // Create the NPC (ped)
            pedHandle = API.CreatePed(4, modelHash, mugshotRoomPosition.X, mugshotRoomPosition.Y, mugshotRoomPosition.Z, 120f, true, false);

            // Release the model from memory
            API.SetModelAsNoLongerNeeded(modelHash);
        }

        // Set the custom camera to view the NPC
        public async Task SetCamera()
        {
            // Ensure the NPC is present before setting the camera
            if (pedHandle == 0)
            {
                Debug.WriteLine("NPC not found, aborting camera setup.");
                return;
            }

            if (API.HasCollisionLoadedAroundEntity(pedHandle))
            { Debug.WriteLine("Mugshot 6"); }


            // Create the custom camera
            customCamera = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            API.SetCamCoord(customCamera, cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            API.SetCamRot(customCamera, cameraRotation.X, cameraRotation.Y, cameraRotation.Z, 2);
            API.SetCamFov(customCamera, cameraFov);
            API.RenderScriptCams(true, true, 1, true, false);

            // Wait for a short duration before resetting
            await Delay(4000);

            // Reset the camera after the delay
            ResetCamera();
        }

        // Reset the camera back to normal
        public void ResetCamera()
        {
            API.RenderScriptCams(false, false, 0, true, false);

            if (customCamera != 0)
            {
                API.DestroyCam(customCamera, false);
                customCamera = 0;
            }
        }

        // Despawn the NPC after the sequence is done
        private void DespawnNPC()
        {
            if (API.DoesEntityExist(pedHandle))
            {
                API.DeletePed(ref pedHandle);
            }
        }
    }
}









// using System;
// using System.Threading.Tasks;
// using CitizenFX.Core;
// using CitizenFX.Core.Native;

// namespace STHMaxzzzie.Client
// {
//     public class mugShot : BaseScript
//     {
//         //location mugshotroom player =     position: 402.848, -996.826, -99.000 rotation: 0.000, -0.000, 178.805 heading: 178.805
//         //location camera =                 Position: 402.629, -1002.263, -99.004 rotation: 0.000, -0.000, 0.230 heading: 0.230
//         private int customCamera;
//         private int pedHandle;

//         [EventHandler("mugShot")]
//         public async void MugShot()
//         {
//             await SpawnPresetNPC();
//             await ForceLoadMugshotArea();
//             SetCamera();
//         }
//         private async Task ForceLoadMugshotArea()
//     {
//         API.RequestCollisionAtCoord(402, -996, -99);
//         while (!API.HasCollisionLoadedAroundEntity(pedHandle))
//         {
//             await Delay(50);
//         }
//     }

//      public async Task SpawnPresetNPC()
//     {
//         string modelName = "s_m_m_strperf_01"; // Preset model name
//         Vector3 spawnLocation = new Vector3(402.848f, -996.826f, -99.9f); // Preset location
//         float spawnHeading = 180.0f; // Preset heading (orientation)

//         uint modelHash = (uint)API.GetHashKey(modelName);

//         // Request the model to load it into memory
//         API.RequestModel(modelHash);

//         // Wait until the model is loaded
//         while (!API.HasModelLoaded(modelHash))
//         {
//             await Delay(50); // Wait a bit and try again
//         }

//         // Create the NPC (ped) at the preset position and heading
//         pedHandle = API.CreatePed(4, modelHash, spawnLocation.X, spawnLocation.Y, spawnLocation.Z, spawnHeading, true, false);

//         // Release the model from memory once it's no longer needed
//         API.SetModelAsNoLongerNeeded(modelHash);
//     }

//         // Method to set the camera to a specific location
//         //public async void SetCamera(Vector3 position, Vector3 rotation, float fov, int duration)
//         public async void SetCamera()
//         {
//             //API.RequestIpl("mp_m_freemode_01_mugshot");
//             Vector3 position = new Vector3(402.85f, -999, -98.4f);
//             Vector3 rotation = new Vector3(179, 180, 180); //tilt, roll, pan (tilt 180 is level, 170 is up slightly)
//             float fov = 11;
//             int duration = 4000;
//             // Disable the player's control to prevent movement
//             //API.SetPlayerControl(API.PlayerId(), false, 0);

//             // Create a new camera at the specified position and rotation
//             customCamera = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
//             API.SetCamCoord(customCamera, position.X, position.Y, position.Z);
//             API.SetCamRot(customCamera, rotation.X, rotation.Y, rotation.Z, 2);
//             API.SetCamFov(customCamera, fov);
//             API.RenderScriptCams(true, false, 0, true, false);

//             // Wait for the specified duration
//             await Delay(duration);

//             // Restore the player's original camera and controls
//             ResetCamera();
//         }

//         // Method to restore the original camera
//         public void ResetCamera()
//         {
//             // Restore the default game camera
//             API.RenderScriptCams(false, false, 0, true, false);

//             // Destroy the custom camera if it exists
//             if (customCamera != 0)
//             {
//                 API.DestroyCam(customCamera, false);
//                 customCamera = 0;
//             }

//             // Enable player control again
//             //API.SetPlayerControl(API.PlayerId(), true, 0);
//         }



//         [EventHandler("clientTest")]
//         void ClientTest()
//         {

//         }
//     }
// }




































// //      public class NotificationClient : BaseScript
// // {
// //     public NotificationClient()
// //     {
// //         EventHandlers["showNotification"] += new Action<string>(ShowNotification);
// //     }

// //     private void ShowNotification(string message)
// //     {
// //         API.SetNotificationTextEntry("STRING");
// //         API.AddTextComponentString(message);
// //         API.DrawNotification(false, true);
// //     }
// // }