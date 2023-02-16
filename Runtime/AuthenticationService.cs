using Unity.Services.Authentication;
using System;
using System.Threading.Tasks;

namespace GameServices
{
	public class AuthenticationService : GameService
	{
		public string AccountId { get; private set; } = null;

		protected override async Task<bool> TryInitialize()
		{
			try
			{
				await Unity.Services.Authentication.AuthenticationService.Instance.SignInAnonymouslyAsync();
				AccountId = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected override Task<bool> TryShutdown()
		{
			AccountId = null;
			return Task.FromResult(true);
		}
	}
}
