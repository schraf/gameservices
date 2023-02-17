using System.Diagnostics;
using UnityEngine;

namespace GameServices
{
	public class Log
	{
		[Conditional("ENABLE_GAME_SERVICES_LOGGING")]
		public static void Debug(string message)
		{
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			UnityEngine.Debug.Log($"{Time.realtimeSinceStartup}: {message}");
			#endif
		}

		[Conditional("ENABLE_GAME_SERVICES_LOGGING")]
		public static void Warning(string message)
		{
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			UnityEngine.Debug.LogWarning($"{Time.realtimeSinceStartup}: {message}");
			#endif
		}

		[Conditional("ENABLE_GAME_SERVICES_LOGGING")]
		public static void Error(string message)
		{
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			UnityEngine.Debug.LogError($"{Time.realtimeSinceStartup}: {message}");
			#endif
		}
	}
}
