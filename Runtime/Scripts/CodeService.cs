using Unity.Services.CloudCode;
using System.Threading.Tasks;

namespace GameServices
{
	public class CodeService : GameService
	{
		public static async Task<TResult> Call<TResult>(string function, object args)
		{
			return await CloudCode.CallEndpointAsync<TResult>(function, args);
		}
	}
}
