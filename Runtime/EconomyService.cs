using UnityEngine;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServices
{
	public class EconomyService : GameService
	{
		/*
		public class PlayerInventory
		{
			public Dictionary<string, PlayerBalance> Currencies = new Dictionary<string, PlayerBalance>();
			public Dictionary<string, PlayersInventoryItem> Items = new Dictionary<string, PlayersInventoryItem>();
		};

		public Dictionary<string, CurrencyDefinition> Currencies = new Dictionary<string, CurrencyDefinition>();
		public Dictionary<string, InventoryItemDefinition> Items = new Dictionary<string, InventoryItemDefinition>();
		public Dictionary<string, VirtualPurchaseDefinition> Store = new Dictionary<string, VirtualPurchaseDefinition>();

		public PlayerInventory Inventory = new PlayerInventory();

		public async Task<bool> SetCurrencyBalance(PlayerBalance balance, long amount)
		{
			PlayerBalances.SetBalanceOptions options = new PlayerBalances.SetBalanceOptions();
			options.WriteLock = balance.WriteLock;
			long oldBalance = balance.Balance;

			try
			{
				balance.Balance = amount;
				PlayerBalance newBalance = await Economy.PlayerBalances.SetBalanceAsync(balance.CurrencyId, balance.Balance, options);
				Inventory.Currencies[balance.CurrencyId] = newBalance;
				return true;
			}
			catch (EconomyException ex)
			{
				balance.Balance = oldBalance;
				Debug.LogException(ex);
				return false;
			}
		}

		public async Task<bool> AddItem(InventoryItemDefinition item)
		{
			try
			{
				PlayersInventoryItem newItem = await Economy.PlayerInventory.AddInventoryItemAsync(item.Id);
				Inventory.Items[newItem.PlayersInventoryItemId] = newItem;
				return true;
			}
			catch (EconomyException ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

		public async Task<bool> UpdateItem(PlayersInventoryItem item)
		{
			Unity.Services.Economy.PlayerInventory.UpdatePlayersInventoryItemOptions options = new Unity.Services.Economy.PlayerInventory.UpdatePlayersInventoryItemOptions();
			options.WriteLock = item.WriteLock;

			try
			{
				PlayersInventoryItem newItem = await Economy.PlayerInventory.UpdatePlayersInventoryItemAsync(item.PlayersInventoryItemId, item.InstanceData, options);

				if (item.PlayersInventoryItemId != newItem.PlayersInventoryItemId)
				{
					Inventory.Items.Remove(item.PlayersInventoryItemId);
				}

				Inventory.Items[newItem.PlayersInventoryItemId] = newItem;
				return true;
			}
			catch (EconomyException ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

		public async Task<bool> RemoveItem(PlayersInventoryItem item)
		{
			Unity.Services.Economy.PlayerInventory.DeletePlayersInventoryItemOptions options = new Unity.Services.Economy.PlayerInventory.DeletePlayersInventoryItemOptions();
			options.WriteLock = item.WriteLock;

			try
			{
				await Economy.PlayerInventory.DeletePlayersInventoryItemAsync(item.PlayersInventoryItemId, options);
				Inventory.Items.Remove(item.PlayersInventoryItemId);
				return true;
			}
			catch (EconomyException ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

		public async Task<bool> MakePurchase(VirtualPurchaseDefinition purchase, List<PlayersInventoryItem> items = null)
		{
			Purchases.MakeVirtualPurchaseOptions options = null;

			if (items != null && items.Count > 0)
			{
				options = new Purchases.MakeVirtualPurchaseOptions();
				options.PlayersInventoryItemIds = new List<string>();

				foreach (PlayersInventoryItem item in items)
				{
					options.PlayersInventoryItemIds.Add(item.PlayersInventoryItemId);
				}
			}

			try
			{
				MakeVirtualPurchaseResult result = await Economy.Purchases.MakeVirtualPurchaseAsync(purchase.Id, options);

				foreach (CurrencyExchangeItem exchange in result.Costs.Currency)
				{
					CurrencyDefinition currency = Currencies[exchange.Id];
					Inventory.Currencies[exchange.Id] = await currency.GetPlayerBalanceAsync();
				}

				foreach (InventoryExchangeItem exchange in result.Costs.Inventory)
				{
					foreach (string itemId in exchange.PlayersInventoryItemIds)
					{
						Inventory.Items.Remove(itemId);
					}
				}

				foreach (CurrencyExchangeItem exchange in result.Rewards.Currency)
				{
					CurrencyDefinition currency = Currencies[exchange.Id];
					Inventory.Currencies[exchange.Id] = await currency.GetPlayerBalanceAsync();
				}

				foreach (InventoryExchangeItem exchange in result.Rewards.Inventory)
				{
					Unity.Services.Economy.PlayerInventory.GetInventoryOptions itemOptions = new Unity.Services.Economy.PlayerInventory.GetInventoryOptions();
					itemOptions.PlayersInventoryItemIds = exchange.PlayersInventoryItemIds;

					GetInventoryResult rewardedItemsResult = await Economy.PlayerInventory.GetInventoryAsync(itemOptions);

					while (rewardedItemsResult != null)
					{
						foreach (PlayersInventoryItem rewardedItem in rewardedItemsResult.PlayersInventoryItems)
						{
							Inventory.Items[rewardedItem.PlayersInventoryItemId] = rewardedItem;
						}

						rewardedItemsResult = await rewardedItemsResult.GetNextAsync();
					}
				}

				return true;
			}
			catch (EconomyException ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

		protected override async Task<bool> TryInitialize()
		{
			foreach (CurrencyDefinition currency in await Economy.Configuration.GetCurrenciesAsync())
			{
				Currencies[currency.Id] = currency;
			}

			foreach (InventoryItemDefinition item in await Economy.Configuration.GetInventoryItemsAsync())
			{
				Items[item.Id] = item;
			}

			foreach (VirtualPurchaseDefinition purchase in await Economy.Configuration.GetVirtualPurchasesAsync())
			{
				Store[purchase.Id] = purchase;
			}

			foreach (CurrencyDefinition currency in Currencies.Values)
			{
				PlayerBalance balance = await currency.GetPlayerBalanceAsync();
				Inventory.Currencies[balance.CurrencyId] = balance;
			}

			foreach (InventoryItemDefinition item in Items.Values)
			{
				GetInventoryResult pagedResult = await item.GetAllPlayersInventoryItemsAsync();

				while (pagedResult != null)
				{
					foreach (PlayersInventoryItem inventoryItem in pagedResult.PlayersInventoryItems)
					{
						Inventory.Items[inventoryItem.PlayersInventoryItemId] = inventoryItem;
					}

					pagedResult = await pagedResult.GetNextAsync();
				}
			}
			
			return true;
		}
		*/
	}
}
