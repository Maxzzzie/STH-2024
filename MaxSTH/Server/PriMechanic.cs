// using System;
// using System.Threading.Tasks;
// using CitizenFX.Core;
// using CitizenFX.Core.Native;
// using System.Collections.Generic;

//             //native info
// //             CreateVehicle
// // 0xDD75460A
// // // CREATE_VEHICLE
// // int /* Entity */ CreateVehicle(uint modelHash, float x, float y, float z, float heading, bool isNetwork, bool netMissionEntity);
// // Parameters:
// // modelHash: The model of vehicle to spawn.
// // x: Spawn coordinate X component.
// // y: Spawn coordinate Y component.
// // z: Spawn coordinate Z component.
// // heading: Heading to face towards, in degrees.
// // isNetwork: Whether to create a network object for the vehicle. If false, the vehicle exists only locally.
// // netMissionEntity: Whether to register the vehicle as pinned to the script host in the R* network model.
// // Returns:
// // A script handle (fwScriptGuid index) for the vehicle, or 0 if the vehicle failed to be created.

// // Creates a vehicle with the specified model at the specified position. This vehicle will initially be owned by the creating script as a mission entity, and the model should be loaded already (e.g. using REQUEST_MODEL).

// // NativeDB Added Parameter 8: BOOL p7
// // This is the server-side RPC native equivalent of the client native CREATE_VEHICLE.

// namespace STHMaxzzzie.Server
// {
//     public class PriMechanic : BaseScript
//     {
//         [Command("pri", Restricted = false)]
//         void priCommand(int source, List<object> args, string raw)
//         {
//             Debug.WriteLine($"Running pri command.");
//             API.CreateVehicle(-1130810103, 0,0,80,180,true, true);
//         }
//     }
// }