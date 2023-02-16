using Unity.Services.Analytics;
using System.Collections.Generic;

namespace GameServices
{
	public class AnalyticsService : GameService
	{
		public void Flush()
		{
			IAnalyticsService service = Unity.Services.Analytics.AnalyticsService.Instance;
			service.Flush();
		}

		public void SendEvent(string name, IDictionary<string, object> data)
		{
			IAnalyticsService service = Unity.Services.Analytics.AnalyticsService.Instance;
			service.CustomData(name, data);
		}
	}
}
