using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;

namespace TerraShop
{
    public static class TradeShop
    {
        public static void CreateTrade(CommandArgs args)
        {
            var player = args.Player;
            var heldItem = player.TPlayer.inventory[player.TPlayer.selectedItem];

            if (heldItem == null || heldItem.stack <= 0 || heldItem.type == 0) { player.SendErrorMessage(ShopLang.GetText(player, "HELD_EMPTY")); return; }
            if (heldItem.type >= 71 && heldItem.type <= 74) { player.SendErrorMessage(ShopLang.GetText(player, "INVALID_COIN")); return; }

            if (args.Parameters.Count < 1)
            {
                player.SendErrorMessage(ShopLang.GetText(player, "USAGE_TRADE"));
                return;
            }

            var foundItems = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
            if (foundItems.Count == 0) { player.SendErrorMessage(ShopLang.GetText(player, "ITEM_NOT_FOUND")); return; }
            if (foundItems.Count > 1) { player.SendErrorMessage("ITEM_ID_USE"); return; }

            Item desiredItem = foundItems[0];
            if (desiredItem.type >= 71 && desiredItem.type <= 74) { player.SendErrorMessage("CAN_USE_SELL"); return; }

            int quantity = 1;
            if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out int parsedQty)) quantity = parsedQty;

            var newItem = new ShopItem(heldItem.type, heldItem.stack, 0, player.Account.Name)
            {
                TradeItemID = desiredItem.type,
                TradeQuantity = quantity
            };
            string itemTag = $"[i/s{heldItem.stack}:{heldItem.type}]";
            string priceTag = $"[i/s{quantity}:{desiredItem.type}]";

            if (args.Parameters.Count > 2)
            {
                var target = TShock.Players.FirstOrDefault(p => p != null && p.Name.ToLower().Contains(args.Parameters[2].ToLower()));
                if (target == null || target.Account == null) { player.SendErrorMessage(ShopLang.GetText(player, "PLAYER_NOT_FOUND")); return; }

                if (P2PShop.DirectOffers.ContainsKey(target.Account.Name))
                {
                    player.SendErrorMessage("PENDING_OFFER_EXISTS");
                    return;
                }

                P2PShop.DirectOffers[target.Account.Name] = newItem;
                InventoryUtils.ClearHeldItem(player);

                player.SendSuccessMessage(ShopLang.GetText(player, "OFFER_SENT", target.Name, priceTag));
                target.SendInfoMessage(ShopLang.GetText(target, "OFFER_RECEIVED", player.Name, itemTag, priceTag));
            }
            else
            {
                ShopCore.GlobalShop.Add(newItem);
                InventoryUtils.ClearHeldItem(player);
                player.SendSuccessMessage(ShopLang.GetText(player, "ITEM_ADDED", itemTag, priceTag));
                ShopStorage.Save();
            }
        }
    }
}