using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;
using Microsoft.Xna.Framework;

namespace TerraShop
{
    public static class P2PShop
    {
        public static Dictionary<string, ShopItem> DirectOffers = new Dictionary<string, ShopItem>();
        public static Dictionary<string, List<string>> PendingMessages = new Dictionary<string, List<string>>();
        public static Dictionary<string, long> PendingPayments = new Dictionary<string, long>();
        public static Dictionary<string, List<ShopItem>> PendingItems = new Dictionary<string, List<ShopItem>>();

        public static void ShowShop(CommandArgs args)
        {
            var player = args.Player;
            int page = 1;
            if (args.Parameters.Count > 0) int.TryParse(args.Parameters[0], out page);

            var regList = TShock.Regions.InAreaRegion(player.TileX, player.TileY);
            var topReg = TShock.Regions.GetTopRegion(regList);

            List<ShopItem> displayList;
            string header;

            // Filtro por región activa
            if (topReg != null && ShopCore.ShopRegions.Contains(topReg.Name))
            {
                displayList = ShopCore.GlobalShop.Where(item => {
                    var acc = TShock.UserAccounts.GetUserAccountByName(item.Seller);
                    return acc != null && (topReg.AllowedIDs.Contains(acc.ID) || item.Seller == topReg.Owner);
                }).ToList();
                header = ShopLang.GetText(player, "SHOP_HEADER_FILTER", topReg.Name, "{0}", "{1}");
            }
            else
            {
                displayList = ShopCore.GlobalShop;
                header = ShopLang.GetText(player, "SHOP_HEADER", "{0}", "{1}");
            }

            if (displayList.Count == 0) { player.SendInfoMessage(ShopLang.GetText(player, "NO_ITEMS")); return; }

            int totalPages = (int)Math.Ceiling(displayList.Count / 5.0);
            if (page < 1) page = 1; if (page > totalPages) page = totalPages;

            player.SendMessage(string.Format(header, page, totalPages), Color.Yellow);

            int start = (page - 1) * 5;
            for (int i = start; i < Math.Min(start + 5, displayList.Count); i++)
            {
                var it = displayList[i];

                // Es tradeo y monedas?
                string priceTag = it.IsTrade ? $"[i/s{it.TradeQuantity}:{it.TradeItemID}]" : ShopCore.FormatCoins(it.PriceCopper);
                string line = $"{i + 1}. [i/s{it.Quantity}:{it.ItemID}] {ShopLang.GetText(player, "FOR_TEXT")} {priceTag} ({it.Seller})";

                player.SendMessage(line, Color.White);
            }

            if (page < totalPages) player.SendInfoMessage(ShopLang.GetText(player, "SHOP_FOOTER_MORE", page + 1));
            player.SendInfoMessage(ShopLang.GetText(player, "BUY_NUMBER"));
        }

        public static void SellItem(CommandArgs args)
        {
            var player = args.Player;
            var item = player.TPlayer.inventory[player.TPlayer.selectedItem];

            if (item == null || item.stack <= 0 || item.type == 0) { player.SendErrorMessage(ShopLang.GetText(player, "HELD_EMPTY")); return; }
            if (item.type >= 71 && item.type <= 74) { player.SendErrorMessage(ShopLang.GetText(player, "INVALID_COIN")); return; }
            if (args.Parameters.Count < 1) { player.SendErrorMessage(ShopLang.GetText(player, "USAGE_SELL")); return; }

            string priceStr = args.Parameters[0].ToLower();
            long mult = 0;
            if (priceStr.EndsWith("p")) mult = 1000000;
            else if (priceStr.EndsWith("g")) mult = 10000;
            else if (priceStr.EndsWith("s")) mult = 100;
            else if (priceStr.EndsWith("c")) mult = 1;

            if (mult == 0 || !long.TryParse(priceStr.Substring(0, priceStr.Length - 1), out long val))
            {
                player.SendErrorMessage(ShopLang.GetText(player, "INVALID_PRICE"));
                return;
            }
            long total = val * mult;

            string itemTag = $"[i/s{item.stack}:{item.type}]";
            var newItem = new ShopItem(item.type, item.stack, total, player.Account.Name);

            if (args.Parameters.Count > 1)
            {
                var target = TShock.Players.FirstOrDefault(p => p != null && p.Name.ToLower().Contains(args.Parameters[1].ToLower()));
                if (target == null || target.Account == null) { player.SendErrorMessage(ShopLang.GetText(player, "PLAYER_NOT_FOUND")); return; }

                DirectOffers[target.Account.Name] = newItem;
                InventoryUtils.ClearHeldItem(player);

                player.SendSuccessMessage(ShopLang.GetText(player, "OFFER_SENT", target.Name, ShopCore.FormatCoins(total)));
                target.SendInfoMessage(ShopLang.GetText(target, "OFFER_RECEIVED", player.Name, itemTag, ShopCore.FormatCoins(total)));
            }
            else
            {
                ShopCore.GlobalShop.Add(newItem);
                InventoryUtils.ClearHeldItem(player);
                player.SendSuccessMessage(ShopLang.GetText(player, "ITEM_ADDED", itemTag, ShopCore.FormatCoins(total)));
                ShopStorage.Save();
            }
        }

        public static void BuyLogic(CommandArgs args)
        {
            var player = args.Player;

            // 1. Lógica para compra directa (oferta P2P)
            if (args.Parameters.Count == 0)
            {
                string acc = player.Account.Name;
                if (!DirectOffers.ContainsKey(acc)) { player.SendErrorMessage(ShopLang.GetText(player, "OFFER_EXPIRED")); return; }
                var offer = DirectOffers[acc];

                // condicional para el tradeo directo
                if (offer.IsTrade)
                {
                    if (TradeMisc.RemoveItems(player, offer.TradeItemID, offer.TradeQuantity))
                    {
                        player.GiveItem(offer.ItemID, offer.Quantity);
                        player.SaveServerCharacter();
                        TradeMisc.NotifyTradePayment(offer.Seller, offer.ItemID, offer.Quantity, offer.TradeItemID, offer.TradeQuantity);
                        DirectOffers.Remove(acc);
                    }
                    else player.SendErrorMessage("NO_ITEMS_TRADE");
                }
                else 
                {
                    if (ShopMisc.RemoveCoins(player, offer.PriceCopper))
                    {
                        player.GiveItem(offer.ItemID, offer.Quantity);
                        player.SaveServerCharacter();
                        Notify(offer.Seller, offer.ItemID, offer.Quantity, offer.PriceCopper);
                        DirectOffers.Remove(acc);
                    }
                    else player.SendErrorMessage(ShopLang.GetText(player, "NO_COINS"));
                }
            }
            else if (int.TryParse(args.Parameters[0], out int idx))
            {
                idx--;
                var regList = TShock.Regions.InAreaRegion(player.TileX, player.TileY);
                var topReg = TShock.Regions.GetTopRegion(regList);

                List<ShopItem> list;
                if (topReg != null && ShopCore.ShopRegions.Contains(topReg.Name))
                {
                    list = ShopCore.GlobalShop.Where(i => {
                        var account = TShock.UserAccounts.GetUserAccountByName(i.Seller);
                        return account != null && (topReg.AllowedIDs.Contains(account.ID) || i.Seller == topReg.Owner);
                    }).ToList();
                }
                else list = ShopCore.GlobalShop;

                if (idx < 0 || idx >= list.Count) { player.SendErrorMessage(ShopLang.GetText(player, "ITEM_NOT_FOUND")); return; }
                var item = list[idx];
                if (item.Seller == player.Account.Name) { player.SendErrorMessage(ShopLang.GetText(player, "OWN_ITEM")); return; }

                if (item.IsTrade)
                {
                    if (TradeMisc.RemoveItems(player, item.TradeItemID, item.TradeQuantity))
                    {
                        player.GiveItem(item.ItemID, item.Quantity);
                        player.SaveServerCharacter();
                        TradeMisc.NotifyTradePayment(item.Seller, item.ItemID, item.Quantity, item.TradeItemID, item.TradeQuantity);
                        ShopCore.GlobalShop.Remove(item);
                        ShopStorage.Save();
                    }
                    else player.SendErrorMessage("NO_ITEMS_TRADE");
                }
                else
                {
                    if (ShopMisc.RemoveCoins(player, item.PriceCopper))
                    {
                        player.GiveItem(item.ItemID, item.Quantity);
                        player.SaveServerCharacter();
                        Notify(item.Seller, item.ItemID, item.Quantity, item.PriceCopper);
                        ShopCore.GlobalShop.Remove(item);
                        ShopStorage.Save();
                    }
                    else player.SendErrorMessage(ShopLang.GetText(player, "NO_COINS"));
                }
            }
        }

        public static void ReturnItem(string sellerName, ShopItem item)
        {
            string msg = ShopLang.GetTextByAccount(sellerName, "EXPIRED_NOTICE", $"[i/s{item.Quantity}:{item.ItemID}]");
            var seller = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == sellerName && p.IsLoggedIn);

            if (seller != null)
            {
                seller.GiveItem(item.ItemID, item.Quantity);
                seller.SaveServerCharacter();
                seller.SendErrorMessage(msg);
            }
            else
            {
                if (!PendingItems.ContainsKey(sellerName)) PendingItems[sellerName] = new List<ShopItem>();
                PendingItems[sellerName].Add(item);

                if (!PendingMessages.ContainsKey(sellerName)) PendingMessages[sellerName] = new List<string>();
                PendingMessages[sellerName].Add(msg);
                ShopStorage.Save();
            }
        }

        private static void Notify(string acc, int id, int stack, long price)
        {
            string m = ShopLang.GetTextByAccount(acc, "SELL_SUCCESS", $"[i/s{stack}:{id}]", ShopCore.FormatCoins(price));
            var sel = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == acc);

            if (sel != null)
            {
                ShopMisc.GiveCoins(sel, price);
                sel.SaveServerCharacter();
                sel.SendSuccessMessage(m);
            }
            else
            {
                if (PendingPayments.ContainsKey(acc)) PendingPayments[acc] += price; else PendingPayments[acc] = price;
                if (!PendingMessages.ContainsKey(acc)) PendingMessages[acc] = new List<string>();
                PendingMessages[acc].Add(m);
                ShopStorage.Save();
            }
        }
    }
}
