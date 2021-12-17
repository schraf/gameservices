using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using System;
using System.Threading.Tasks;

namespace GameServices
{
	public class RelayService : GameService
	{
		UnityTransport Transport;

		protected override Task<bool> TryInitialize()
		{
			Transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
			return Task.FromResult(Transport != null);
		}

		protected override Task<bool> TryShutdown()
		{
			Transport = null;
			return Task.FromResult(true);
		}

		public async Task<string> CreateServer(int maxConnections)
		{
			string joinCode = null;

			try
			{
				Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections, null);
				joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

				Transport.SetRelayServerData(
					allocation.RelayServer.IpV4, 
					(ushort)allocation.RelayServer.Port, 
					allocation.AllocationIdBytes, 
					allocation.Key, 
					allocation.ConnectionData);

				if (!NetworkManager.Singleton.StartHost())
				{
					Debug.LogWarning($"Failed to host server on relay {allocation.RelayServer.IpV4}:${allocation.RelayServer.Port}");
					joinCode = null;
				}
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
				JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

				Transport.SetRelayServerData(
					allocation.RelayServer.IpV4, 
					(ushort)allocation.RelayServer.Port, 
					allocation.AllocationIdBytes, 
					allocation.Key, 
					allocation.ConnectionData, 
					allocation.HostConnectionData);

				return NetworkManager.Singleton.StartClient();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			return false;
		}
	}
}
