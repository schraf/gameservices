using UnityEngine;
using UnityEngine.Events;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;
using System.Threading.Tasks;

namespace GameServices
{
	public class GameServicesManager : MonoBehaviour
	{
		public static GameServicesManager Instance { get; private set; } = null;

		public GameServicesConfig Config;
		
		public UnityEvent InitializedEvent;
		public UnityEvent ShutdownEvent;

		public AuthenticationService Auth { get; private set; } = new AuthenticationService();
		public AnalyticsService Analytics { get; private set; } = new AnalyticsService();
		public CodeService Code { get; private set; } = new CodeService();
		public EconomyService Economy { get; private set; } = new EconomyService();
		public LobbyService Lobby { get; private set; } = new LobbyService();
		public PersistentDataService PersistentData { get; private set; } = new PersistentDataService();
		public RelayService Relay { get; private set; } = new RelayService();
		public RemoteConfigService RemoteConfig { get; private set; } = new RemoteConfigService();

		private async void Start()
		{
			if (Instance != null)
				throw new Exception("Multiple GameServiceManager Instances");

			Instance = this;

			Log.Debug("Initializing Game Services");

			if (UnityServices.State == ServicesInitializationState.Uninitialized)
			{
				InitializationOptions options = new InitializationOptions();
				options.SetEnvironmentName(Config.Environment.ToString().ToLower());

				await UnityServices.InitializeAsync(options);
			}

			await Auth.Initialize(Config);

			Log.Debug($"Authenticated Game Services PlayerId({Auth.AccountId})");

			await Task.WhenAll(new[] {
				Analytics.Initialize(Config),
				PersistentData.Initialize(Config),
				Lobby.Initialize(Config),
				Relay.Initialize(Config),
				Code.Initialize(Config),
				RemoteConfig.Initialize(Config),
				Economy.Initialize(Config)
			});

			Log.Debug("Game Services Initialized");
			OnInitialized();
		}

		private async void OnDestroy()
		{
			Log.Debug("Shutting down Game Services");

			await Task.WhenAll(new[] {
				Analytics.Shutdown(),
				PersistentData.Shutdown(),
				Lobby.Shutdown(),
				Relay.Shutdown(),
				Code.Shutdown(),
				RemoteConfig.Shutdown(),
				Economy.Shutdown()
			});

			await Auth.Shutdown();
			Instance = null;

			Log.Debug("Shutdown Game Services");
			OnShutdown();
		}

		private void OnInitialized()
		{
			if (InitializedEvent != null)
			{
				InitializedEvent.Invoke();
			}
		}

		private void OnShutdown()
		{
			if (ShutdownEvent != null)
			{
				ShutdownEvent.Invoke();
			}
		}
	}
}

