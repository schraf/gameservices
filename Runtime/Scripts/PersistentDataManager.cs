using Unity.Services.CloudSave;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServices
{
	public interface IPersistentData
	{
		void Serialize(BinaryWriter writer);
		void Deserialize(BinaryReader reader);
	}

	public class PersistentDataService : GameService
	{
		private const string Key = "PersistentData";

		public async Task Load(IPersistentData data)
		{
			Dictionary<string, string> savedData = await SaveData.LoadAllAsync();

			if (savedData.ContainsKey(Key))
			{
				char[] quotes = {'"'};
				string encodedData = savedData[Key].Trim(quotes); // Bug in Cloud Save that adds quotes. Fix should be in next release.
				byte[] decodedBytes = Convert.FromBase64String(encodedData);

				using (MemoryStream memoryStream = new MemoryStream(decodedBytes))
				{
					using (BinaryReader reader = new BinaryReader(memoryStream))
					{
						data.Deserialize(reader);
					}
				}
			}
		}

		public async Task Save(IPersistentData data)
		{
			if (!Config.SavePersistentData)
				return;

			using (MemoryStream memoryStream = new MemoryStream(Config.MaxPersistentDataSize))
			{
				BinaryWriter writer = new BinaryWriter(memoryStream);
				data.Serialize(writer);
				byte[] bytes = memoryStream.GetBuffer();
				Dictionary<string, object> dataToSave = new Dictionary<string, object>(1);
				dataToSave[Key] = Convert.ToBase64String(bytes, 0, (int)memoryStream.Position);
				await SaveData.ForceSaveAsync(dataToSave);
			}
		}
	}
}
