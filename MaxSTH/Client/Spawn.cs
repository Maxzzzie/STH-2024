using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace STHMaxzzzie.Client
{
	public class Spawn : BaseScript
	{

		private static bool _spawnLock = false;
		public static bool SpawnLock {get {return _spawnLock;} }

		public static void FreezePlayer(int playerId, bool freeze)
		{
			var ped = GetPlayerPed(playerId);

			SetPlayerControl(playerId, !freeze, 0);

			if (!freeze)
			{
				if (!IsEntityVisible(ped))
					SetEntityVisible(ped, true, false);

				if (!IsPedInAnyVehicle(ped, true))
					SetEntityCollision(ped, true, true);

				FreezeEntityPosition(ped, false);
				//SetCharNeverTargetted(ped, false)
				SetPlayerInvincible(playerId, false);
			}
			else
			{
				if (IsEntityVisible(ped))
					SetEntityVisible(ped, false, false);

				SetEntityCollision(ped, false, true);
				FreezeEntityPosition(ped, true);
				//SetCharNeverTargetted(ped, true)
				SetPlayerInvincible(playerId, true);

				if (IsPedFatallyInjured(ped))
					ClearPedTasksImmediately(ped);
			}
		}

		///<exception cref="Exception">Throws when <paramref name="model"/> is invalid.</exception>
		///param name="action" An action to do while player is "spawning"
		public static async Task SpawnPlayer(float x, float y, float z, float heading, Func<Task> action = null)
		{
			if (_spawnLock)
				return;
			_spawnLock = true;

			DoScreenFadeOut(500);

			RequestCollisionAtCoord(x, y, z);

			while (IsScreenFadingOut())
			{
				await Delay(1);
			}

			FreezePlayer(PlayerId(), true);

			SetPedDefaultComponentVariation(GetPlayerPed(-1));
			RequestCollisionAtCoord(x, y, z);

			var ped = GetPlayerPed(-1);

			SetEntityCoordsNoOffset(ped, x, y, z, false, false, false);
			NetworkResurrectLocalPlayer(x, y, z, heading, true, true);
			ClearPedTasksImmediately(ped);
			// RemoveAllPedWeapons(ped, false);
			ClearPlayerWantedLevel(PlayerId());
			NetworkSetInSpectatorMode(false, 0);
			ResetPedVisibleDamage(ped);

			Debug.WriteLine("running respawn action");
			if(!(action is null)){
				await action();
			}
			Debug.WriteLine("respawn action complete :)");
			while (!HasCollisionLoadedAroundEntity(ped))
			{
				await Delay(1);
			}
			ShutdownLoadingScreenNui();
			ShutdownLoadingScreen();
			DoScreenFadeIn(500);

			while (IsScreenFadingIn())
			{
				await Delay(1);
			}

			FreezePlayer(PlayerId(), false);

			//TriggerEvent("playerSpawned", PlayerId());

			_spawnLock = false;
			
		}
	}
}