using System.Diagnostics;
using UnityEngine;

namespace GameServices
{
	public class Log
	{
		[Conditional("ENABLE_GAME_SERVICES_LOGGING")]
		public static void Debug(string message)
		{
			#if ENABLE_GAME_SERVICES_LOGGING || UNITY_EDITOR
			UnityEngine.Debug.Log($"{Time.realtimeSinceStartup}: {message}");
			#endif
		}

		[Conditional("ENABLE_GAME_SERVICES_LOGGING")]
		public static void Warning(string message)
		{
			#if ENABLE_GAME_SERVICES_LOGGING || UNITY_EDITOR
			UnityEngine.Debug.LogWarning($"{Time.realtimeSinceStartup}: {message}");
			#endif
		}

		[Conditional("ENABLE_GAME_SERVICES_LOGGING")]
		public static void Error(string message)
		{
			#if ENABLE_GAME_SERVICES_LOGGING || UNITY_EDITOR
			UnityEngine.Debug.LogError($"{Time.realtimeSinceStartup}: {message}");
			#endif
		}
	}
}
