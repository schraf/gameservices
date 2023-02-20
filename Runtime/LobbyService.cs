using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServices
{
	[CreateAssetMenu(fileName = "LobbyConfig", menuName = "GameServices/LobbyConfig")]
	public class LobbyConfig : ScriptableObject
	{
		public string Type;
		public bool Private;
		public int MaxPlayers;
	}

	public class LobbyService : GameService
	{
		private Dictionary<string, Lobby> LobbyMap = new Dictionary<string, Lobby>();
	
		public async Task<string> CreateLobby(LobbyConfig config, string connectionInfo)
		{
			string lobbyCode = null;

			if (Config.CanCreateLobbies)
			{
				CreateLobbyOptions options = new CreateLobbyOptions();
				options.IsPrivate = config.Private;
				options.Data = new Dictionary<string, DataObject>()
				{
					{ "type", new DataObject(
						visibility: DataObject.VisibilityOptions.Public, 
						value: config.Type,
						index: DataObject.IndexOptions.S1)
					},
					{ "connectionInfo", new DataObject(
						visibility: DataObject.VisibilityOptions.Member, 
						value: connectionInfo)
					}
				};

				options.Player = new Player(
					id: GameServicesManager.Instance.Auth.AccountId
				);

				string lobbyName = Guid.NewGuid().ToString();
				Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, config.MaxPlayers, options);
				lobbyCode = lobby.LobbyCode;
				LobbyMap[lobbyCode] = lobby;
			}

			return lobbyCode;
		}

		public async Task<bool> JoinLobby(string code)
		{
			try
			{
				Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code);
				LobbyMap[code] = lobby;
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

		public async Task<string> JoinLobby(LobbyConfig config)
		{
			string lobbyCode = null;

			try
			{
				QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

				options.Player = new Player(
					id: GameServicesManager.Instance.Auth.AccountId
				);

				options.Filter = new List<QueryFilter>()
				{
					new QueryFilter(
						field: QueryFilter.FieldOptions.AvailableSlots,
						op: QueryFilter.OpOptions.GE,
						value: "1"),
					new QueryFilter(
						field: QueryFilter.FieldOptions.S1,
						op: QueryFilter.OpOptions.EQ,
						value: config.Type
					)
				};

				Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);
				lobbyCode = lobby.LobbyCode;
				LobbyMap[lobbyCode] = lobby;
			}
			catch (LobbyServiceException ex)
			{
				Debug.LogException(ex);
			}

			return lobbyCode;
		}

		public async Task<bool> LeaveLobby(string code)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				try
				{
					string playerId = GameServicesManager.Instance.Auth.AccountId;
					await Lobbies.Instance.RemovePlayerAsync(lobby.Id, playerId);
					return true;
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}

			return false;
		}

		public bool GetLobbyData(string code, string key, out string value)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				DataObject data;
				
				if (lobby.Data.TryGetValue(key, out data))
				{
					value = data.Value;
					return true;
				}
			}

			value = null;
			return false;
		}

		public bool GetLobbyConnectionInfo(string code, out string connectionInfo)
		{
			return GetLobbyData(code, "connectionInfo", out connectionInfo);
		}

		protected async override Task<bool> TryShutdown()
		{
			foreach (KeyValuePair<string, Lobby> kvp in LobbyMap)
			{
				Lobby lobby = kvp.Value;
				string playerId = GameServicesManager.Instance.Auth.AccountId;
				await Lobbies.Instance.RemovePlayerAsync(lobby.Id, playerId);
			}

			LobbyMap.Clear();
			return true;
		}
	}
}
