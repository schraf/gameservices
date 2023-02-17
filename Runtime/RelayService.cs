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
			public NetworkDriver Driver;
			public string JoinCode;
		}

		public async Task<TaskResult> CreateServer(int maxConnections)
		{
			TaskResult result = new TaskResult();

			IRelayService service = Unity.Services.Relay.RelayService.Instance;
			Allocation allocation = await service.CreateAllocationAsync(maxConnections, null);

			Log.Debug($"RelayService: allocation guid {allocation.AllocationId}");
			Log.Debug($"RelayService: region {allocation.Region}");

			foreach (RelayServerEndpoint endpoint in allocation.ServerEndpoints)
			{
				Log.Debug($"RelayService: endpoint {endpoint.Host}:{endpoint.Port}::{endpoint.ConnectionType}");
			}

			RelayServerData serverData = new RelayServerData(allocation, "udp");
			NetworkSettings settings = new NetworkSettings();
			settings.WithRelayParameters(ref serverData);

			result.Driver = NetworkDriver.Create(settings);
			result.JoinCode = await service.GetJoinCodeAsync(allocation.AllocationId);

			Log.Debug($"RelayService: join code {result.JoinCode}");

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

			result.Driver = NetworkDriver.Create(settings);
			result.JoinCode = joinCode;

			return result;
		}
	}
}
