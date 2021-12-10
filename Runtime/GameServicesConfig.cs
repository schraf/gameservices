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

	[CreateAssetMenu(fileName = "GameServicesConfig", menuName = "ScriptableObjects/GameServicesConfig", order = 1)]
	public class GameServicesConfig : ScriptableObject
	{
		public ServicesEnvironment Environment;
		public bool CanCreateLobbies;
		public bool SendAnalyticEvents;
		public bool SavePersistentData;
	}
}
