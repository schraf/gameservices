using Unity.Services.RemoteConfig;
using System.Threading.Tasks;

namespace GameServices
{
	public class RemoteConfigService : GameService
	{
		private RuntimeConfig config;

		private struct UserAttributes { };
		private struct AppAttributes { };

		protected override async Task<bool> TryInitialize()
		{
			Unity.Services.RemoteConfig.RemoteConfigService service = Unity.Services.RemoteConfig.RemoteConfigService.Instance;
			service.SetCustomUserID(GameServicesManager.Instance.Auth.AccountId);
			service.SetEnvironmentID(GameServicesManager.Instance.Config.Environment.ToString().ToLower());
			config = await service.FetchConfigsAsync<UserAttributes, AppAttributes>(new UserAttributes(), new AppAttributes());
			return true;
		}

		public bool GetBool(string name, bool defaultValue)
		{
			return config.GetBool(name, defaultValue);
		}

		public int GetInt(string name, int defaultValue)
		{
			return config.GetInt(name, defaultValue);
		}

		public string GetString(string name, string defaultValue)
		{
			return config.GetString(name, defaultValue);
		}

		public float GetFloat(string name, float defaultValue)
		{
			return config.GetFloat(name, defaultValue);
		}
	}
}
