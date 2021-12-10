using Unity.Services.Authentication;
using System;
using System.Threading.Tasks;

namespace GameServices
{
	public class AuthenticationService : GameService
	{
		public Int32 AccountId { get; private set; } = 0;

		protected override async Task<bool> TryInitialize()
		{
			try
			{
				await Unity.Services.Authentication.AuthenticationService.Instance.SignInAnonymouslyAsync();
				AccountId = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId.GetHashCode();
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected override Task<bool> TryShutdown()
		{
			AccountId = 0;
			return Task.FromResult(true);
		}
	}
}
