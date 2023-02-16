using UnityEngine;

namespace GameServices
{
	public enum ServicesEnvironment
	{
		Development,
		Production,
		Staging,
		Live,
	}

	[CreateAssetMenu(fileName = "GameServicesConfig", menuName = "GameServices/GameServicesConfig")]
	public class GameServicesConfig : ScriptableObject
	{
		public ServicesEnvironment Environment;
		public bool CanCreateLobbies;
		public bool SendAnalyticEvents;
		public bool SavePersistentData;
		public int MaxPersistentDataSize = 1 * 1024 * 1024;
	}
}
