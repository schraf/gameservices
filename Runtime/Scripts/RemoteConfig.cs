using Unity.RemoteConfig;
using System.Threading.Tasks;

namespace GameServices
{
	public class RemoteConfigService : GameService
	{
		private struct UserAttributes { };
		private struct AppAttributes { };

		protected override async Task<bool> TryInitialize()
		{
			ConfigManager.SetCustomUserID(GameServicesManager.Instance.Auth.AccountId);
			RuntimeConfig config = await ConfigManager.FetchConfigsAsync<UserAttributes, AppAttributes>(new UserAttributes(), new AppAttributes());
			return true;
		}

		public bool GetBool(string name, bool defaultValue)
		{
			return ConfigManager.appConfig.GetBool(name, defaultValue);
		}

		public int GetInt(string name, int defaultValue)
		{
			return ConfigManager.appConfig.GetInt(name, defaultValue);
		}

		public string GetString(string name, string defaultValue)
		{
			return ConfigManager.appConfig.GetString(name, defaultValue);
		}

		public float GetFloat(string name, float defaultValue)
		{
			return ConfigManager.appConfig.GetFloat(name, defaultValue);
		}
	}
}
