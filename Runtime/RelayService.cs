using UnityEngine;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;
using System.Threading.Tasks;

namespace GameServices
{
	public class RelayService : GameService
	{
		public struct TaskResult
		{
			public NetworkDriver driver;
			public string joinCode;
		}

		public async Task<TaskResult> CreateServer(int maxConnections)
		{
			TaskResult result = new TaskResult();

			IRelayService service = Unity.Services.Relay.RelayService.Instance;
			Allocation allocation = await service.CreateAllocationAsync(maxConnections, null);

			RelayServerData serverData = new RelayServerData(allocation, "udp");
			NetworkSettings settings = new NetworkSettings();
			settings.WithRelayParameters(ref serverData);

			result.driver = NetworkDriver.Create(settings);
			result.joinCode = await service.GetJoinCodeAsync(allocation.AllocationId);

			return result;
		}

		public async Task<TaskResult> JoinServer(string joinCode)
		{
			TaskResult result = new TaskResult();

			IRelayService service = Unity.Services.Relay.RelayService.Instance;
			JoinAllocation allocation = await service.JoinAllocationAsync(joinCode);
			RelayServerData serverData = new RelayServerData(allocation, "udp");
			NetworkSettings settings = new NetworkSettings();
			settings.WithRelayParameters(ref serverData);

			result.driver = NetworkDriver.Create(settings);
			result.joinCode = joinCode;

			return result;
		}
	}
}
