using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;

namespace GameServices
{
	public class GameServicesManager : MonoBehaviour
	{
		public static GameServicesManager Instance { get; private set; } = null;

		public GameServicesConfig Config;

		public AuthenticationService Auth { get; private set; } = new AuthenticationService();
		public AnalyticsService Analytics { get; private set; } = new AnalyticsService();
		public LobbyService Lobby { get; private set; } = new LobbyService();
		public PersistentDataService PersistentData { get; private set; } = new PersistentDataService();

		private async void Start()
		{
			if (Instance != null)
				throw new Exception("Multiple GameServiceManager Instances");

			if (UnityServices.State == ServicesInitializationState.Uninitialized)
			{
				InitializationOptions options = new InitializationOptions();
				options.SetEnvironmentName(Config.Environment.ToString().ToLower());

				await UnityServices.InitializeAsync(options);
			}

			await Auth.Initialize(Config);
			await Analytics.Initialize(Config);
			await PersistentData.Initialize(Config);
			await Lobby.Initialize(Config);
		}

		private async void OnDestroy()
		{
			await Lobby.Shutdown();
			await PersistentData.Shutdown();	
			await Analytics.Shutdown();
			await Auth.Shutdown();
			Instance = null;
		}
	}
}

