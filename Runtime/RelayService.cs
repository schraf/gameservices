using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;
using System.Threading.Tasks;

namespace GameServices
{
	public class RelayService : GameService
	{
		private NetworkDriver Driver;
		private NativeList<NetworkConnection> Connections;
		private JobHandle ServerJobHandle;

		protected virtual Task<bool> TryShutdown()
		{
			ShutdownDriver();
			return Task.FromResult(true);
		}

		private Task ShutdownDriver()
		{
			if (Driver.IsCreated)
			{
				ServerJobHandle.Complete();
				Connections.Dispose();
				Driver.Dispose();
			}
		}

		public async Task<string> CreateServer(int maxConnections)
		{
			string joinCode = null;

			ShutdownDriver();

			try
			{
				Connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
        		Driver = NetworkDriver.Create();

				IRelayService service = Unity.Services.Relay.RelayService.Instance;
				Allocation allocation = await service.CreateAllocationAsync(maxConnections, null);
				joinCode = await service.GetJoinCodeAsync(allocation.AllocationId);
/*
				Transport.SetRelayServerData(
					allocation.RelayServer.IpV4, 
					(ushort)allocation.RelayServer.Port, 
					allocation.AllocationIdBytes, 
					allocation.Key, 
					allocation.ConnectionData);
*/
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return joinCode;
		}

		public async Task<bool> JoinServer(string joinCode)
		{
			try
			{
				IRelayService service = Unity.Services.Relay.RelayService.Instance;
				JoinAllocation allocation = await service.JoinAllocationAsync(joinCode);
/*
				Transport.SetRelayServerData(
					allocation.RelayServer.IpV4, 
					(ushort)allocation.RelayServer.Port, 
					allocation.AllocationIdBytes, 
					allocation.Key, 
					allocation.ConnectionData, 
					allocation.HostConnectionData);
*/
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return false;
		}
	}
}
