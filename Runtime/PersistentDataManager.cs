using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServices
{
	public class PersistentDataService : GameService
	{
		private static Dictionary<string, string> Data;
		private static HashSet<string> DirtySet;

		public void DeleteKey(string key)
		{
			Data.Remove(key);
			DirtySet.Add(key);
		}
	
		public void Set(string key, string value)
		{
			Data[key] = value;
			DirtySet.Add(key);
		}

		public async void Save()
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			List<string> keysToDelete = new List<string>();

			foreach (string key in DirtySet)
			{
				if (data.ContainsKey(key))
				{
					data[key] = Data[key];
				}
				else
				{
					keysToDelete.Add(key);
				}
			}

			DirtySet.Clear();

			foreach (string key in keysToDelete)
			{
				await SaveData.ForceDeleteAsync(key);
			}

			await SaveData.ForceSaveAsync(data);
		}

		protected override async Task<bool> TryInitialize()
		{
			try
			{
				Data = await SaveData.LoadAllAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected override Task<bool> TryShutdown()
		{
			Data.Clear();
			return Task.FromResult(true);
		}
	}
}
