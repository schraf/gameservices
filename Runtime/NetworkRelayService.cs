using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;

namespace GameServices
{
	public class NetworkRelayService : GameService
	{
		public async Task<Allocation> CreateServer(int maxConnections)
		{
			IRelayService service = Unity.Services.Relay.RelayService.Instance;
			Allocation allocation = await service.CreateAllocationAsync(maxConnections, null);

			Log.Debug($"RelayService: allocation guid {allocation.AllocationId}");
			Log.Debug($"RelayService: region {allocation.Region}");

			foreach (RelayServerEndpoint endpoint in allocation.ServerEndpoints)
			{
				Log.Debug($"RelayService: endpoint {endpoint.Host}:{endpoint.Port}::{endpoint.ConnectionType}");
			}

			return allocation;
		}

		public async Task<string> GetJoinCode(Allocation allocation)
		{
			IRelayService service = Unity.Services.Relay.RelayService.Instance;
			return await service.GetJoinCodeAsync(allocation.AllocationId);
		}

		public async Task<JoinAllocation> JoinServer(string joinCode)
		{
			IRelayService service = Unity.Services.Relay.RelayService.Instance;
			return await service.JoinAllocationAsync(joinCode);
		}
	}
}
