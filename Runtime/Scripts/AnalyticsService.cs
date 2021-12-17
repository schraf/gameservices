using Unity.Services.Analytics;
using System.Collections.Generic;

namespace GameServices
{
	public class AnalyticsService : GameService
	{
		public void Flush()
		{
			Events.Flush();
		}

		public void SendEvent(string name, IDictionary<string, object> data)
		{
			Events.CustomData(name, data);
		}
	}
}
