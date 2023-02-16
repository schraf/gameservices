using Unity.Services.CloudCode;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameServices
{
	public class CodeService : GameService
	{
		public static async Task<TResult> Call<TResult>(string function, Dictionary<string, object> args)
		{
			ICloudCodeService service = CloudCodeService.Instance;
			return await service.CallEndpointAsync<TResult>(function, args);
		}
	}
}
