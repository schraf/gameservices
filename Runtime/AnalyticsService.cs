using Unity.Services.Analytics;
using System.Collections.Generic;

namespace GameServices
{
	public class AnalyticsService : GameService
	{
		public void Flush()
		{
			if (State == State.Initialized)
			{
				Events.Flush();
			}
		}

		public void SendEvent(string name, IDictionary<string, object> data)
		{
			if (State == State.Initialized)
			{
				Events.CustomData(name, data);
			}
		}
	}

}
