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
		public int Version;
	}

	public class LobbyService : GameService
	{
		private Dictionary<string, Lobby> LobbyMap = new Dictionary<string, Lobby>();
	
		public async Task<string> CreateLobby(LobbyConfig config)
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
					{ "version", new DataObject(
						visibility: DataObject.VisibilityOptions.Public, 
						value: config.Version.ToString(),
						index: DataObject.IndexOptions.N1)
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
				Log.Warning(ex.Message);
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
					),
					new QueryFilter(
						field: QueryFilter.FieldOptions.N1,
						op: QueryFilter.OpOptions.EQ,
						value: config.Version.ToString()
					)
				};

				Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);
				lobbyCode = lobby.LobbyCode;
				LobbyMap[lobbyCode] = lobby;
			}
			catch (LobbyServiceException ex)
			{
				Log.Warning(ex.Message);
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
					Log.Warning(ex.Message);
				}
			}

			return false;
		}

		public async Task<bool> SetLobbyData(string code, string key, string value)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				try
				{
					UpdateLobbyOptions options = new UpdateLobbyOptions();
					options.Data = new Dictionary<string, DataObject>();
					options.Data[key] = new DataObject(DataObject.VisibilityOptions.Member, value);
					LobbyMap[code] = await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, options);
					return true;
				}
				catch (Exception ex)
				{
					Log.Warning(ex.Message);
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

		public async Task<bool> SendLobbyHeartbeat(string code)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				try
				{
					await Lobbies.Instance.SendHeartbeatPingAsync(lobby.Id);
					return true;
				}
				catch (Exception ex)
				{
					Log.Warning(ex.Message);
				}
			}

			return false;
		}

		public async Task UpdateLobbies()
		{
			foreach (string code in LobbyMap.Keys)
			{
				Lobby lobby = LobbyMap[code];
				LobbyMap[code] = await Lobbies.Instance.GetLobbyAsync(lobby.Id);
			}
		}

		public bool GetPlayers(string code, out List<string> players)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				players = new List<string>(lobby.Players.Count);

				foreach (Player player in lobby.Players)
				{
					players.Add(player.Id);
				}

				return true;
			}

			players = null;
			return false;
		}

		public bool GetPlayerData(string code, string playerId, string key, out string value)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				foreach (Player player in lobby.Players)
				{
					if (player.Id == playerId)
					{
						PlayerDataObject data;
						
						if (player.Data.TryGetValue(key, out data))
						{
							value = data.Value;
							return true;
						}

						value = null;
						return false;
					}
				}
			}

			value = null;
			return false;
		}

		public async Task<bool> SetPlayerData(string code, string key, string value)
		{
			Lobby lobby = null;
			
			if (LobbyMap.TryGetValue(code, out lobby))
			{
				string playerId = GameServicesManager.Instance.Auth.AccountId;
				UpdatePlayerOptions options = new UpdatePlayerOptions();
				options.Data = new Dictionary<string, PlayerDataObject>();
				PlayerDataObject data = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, value);
				options.Data[key] = data;

				try
				{
					LobbyMap[code] = await Lobbies.Instance.UpdatePlayerAsync(lobby.Id, playerId, options);
					return true;
				}
				catch (Exception ex)
				{
					Log.Warning(ex.Message);
				}
			}

			return false;
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
