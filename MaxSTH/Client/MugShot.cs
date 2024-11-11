using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Client
{
    public class MugShot : BaseScript
    {
        //fix ped component variation
        //         //location mugshotroom player =     position: 402.848, -996.826, -99.000 rotation: 0.000, -0.000, 178.805 heading: 178.805
        //         //location camera =                 Position: 402.629, -1002.263, -99.004 rotation: 0.000, -0.000, 0.230 heading: 0.230

        private Vector3 mugshotModelPosition = new Vector3(-2148.798f, 223.015f, 183.702f);
        private Vector3 cameraRotation = new Vector3(180f, 180f, 65f); //tilt, roll, pan (tilt 180 is level, 170 is up slightly) 180f, 180f, 123f
        int pedHeadBoneIndex = 0;
        private float cameraFov = 10;
        private int customCamera;
        private int pedHandle;
        private bool isRunning = false;

        // Method that starts the mugshot sequence
        [EventHandler("MugShotEvent")]
        private async void MugShotEvent(string modelName)
        {
            //Debug.WriteLine($"Mugshot process with {modelName}");
            if (isRunning)
            {
                // Prevent command spamming
                TriggerEvent("chat:addMessage", new{color=new[]{255,0,0},args=new[]{$"Wait"}});
                return;
            }

            isRunning = true;

            await SpawnNPC(modelName); // Pass the model name dynamically
            // await ForceLoadMugshotArea();
            // await SetCamera();
            await Delay(4000);
            DespawnNPC();
        }

        public async Task SpawnNPC(string modelName)
        {
            Vector3 playerPosition = Game.PlayerPed.GetOffsetPosition(new Vector3(0, 5, 0));
            uint modelHash = (uint)API.GetHashKey(modelName);

            // Request the model
            API.RequestModel(modelHash);

            // Wait until the model is loaded
            while (!API.HasModelLoaded(modelHash))
            {
                await Delay(50);
            }

            // Create the NPC (ped)
            pedHandle = API.CreatePed(4, modelHash, playerPosition.X, playerPosition.Y, playerPosition.Z, 70, true, false);
            //pedHandle = API.CreatePed(4, modelHash, mugshotModelPosition.X, mugshotModelPosition.Y, mugshotModelPosition.Z, 70, true, false);

            //sets all components default.
            for (int componentId = 0; componentId < 12; componentId++) // 0-11 covers all drawable components
            {
                API.SetPedComponentVariation(pedHandle, componentId, 0, 0, 0);
            }
            // Release the model from memory
            API.SetModelAsNoLongerNeeded(modelHash);
        }


        // Despawn the NPC after the sequence is done
        private void DespawnNPC()
        {
            if (API.DoesEntityExist(pedHandle))
            {
                API.DeletePed(ref pedHandle);
                isRunning= false;
            }
        }
    }
}




        // Set the custom camera to view the NPC
        // public async Task SetCamera()
        // {
        //     // Ensure the NPC is present before setting the camera
        //     if (pedHandle == 0)
        //     {
        //         Debug.WriteLine("NPC not found, aborting camera setup.");
        //         return;
        //     }

        //     if (API.HasCollisionLoadedAroundEntity(pedHandle))
        //     { Debug.WriteLine("Mugshot 6"); }


        //     // Create the custom camera
        //     customCamera = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
        //     API.AttachCamToPedBone(customCamera, pedHandle, 31086, 0.3f, 2.6f, 0.0f, true);
        //     API.SetCamRot(customCamera, cameraRotation.X, cameraRotation.Y, cameraRotation.Z, 2);
        //     API.SetCamFov(customCamera, cameraFov);
        //     API.RenderScriptCams(true, true, 1, true, false);
        //     TriggerEvent("chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"camera's pedHeadBoneIndex{pedHeadBoneIndex}" } });
        //     // Wait for a short duration before resetting
        //     await Delay(4000);

        //     // Reset the camera after the delay
        //     ResetCamera();
        // }

        // Reset the camera back to normal
        // public void ResetCamera()
        // {
        //     API.RenderScriptCams(false, false, 0, true, false);

        //     if (customCamera != 0)
        //     {
        //         API.DestroyCam(customCamera, false);
        //         customCamera = 0;
        //     }
        // }














        // Force loading the mugshot area
        // private async Task ForceLoadMugshotArea()
        // {
        //     Debug.WriteLine("running ForceLoadMugshotArea");
        //     while (pedHandle == 0)
        //     {
        //         Debug.WriteLine("No ped to load collision for. Retrying in 10ms.");
        //         await Delay(10);
        //     }
        //     API.RequestCollisionForModel((uint)pedHandle);
        //     while (!API.HasCollisionLoadedAroundEntity(pedHandle))
        //     {
        //         Debug.WriteLine("Collision hasn't loaded around ped. Retrying in 10ms.");
        //         await Delay(10);
        //     }
        // }

        // Spawn an NPC with the given model name

















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