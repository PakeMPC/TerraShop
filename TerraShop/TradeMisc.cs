using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;

namespace TerraShop
{
    public static class TradeMisc
    {
        public static int CountItems(TSPlayer player, int itemType)
        {
            if (player?.TPlayer == null) return 0;
            int count = 0;
            for (int i = 0; i < 58; i++) // Inventario principal
            {
                if (player.TPlayer.inventory[i].type == itemType)
                    count += player.TPlayer.inventory[i].stack;
            }
            return count;
        }

        public static bool RemoveItems(TSPlayer player, int itemType, int amount)
        {
            if (CountItems(player, itemType) < amount) return false;

            int remaining = amount;
            for (int i = 0; i < 58; i++)
            {
                if (player.TPlayer.inventory[i].type == itemType)
                {
                    if (player.TPlayer.inventory[i].stack > remaining)
                    {
                        player.TPlayer.inventory[i].stack -= remaining;
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, player.Index, i);
                        break;
                    }
                    else
                    {
                        remaining -= player.TPlayer.inventory[i].stack;
                        player.TPlayer.inventory[i].SetDefaults(0);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, player.Index, i);
                        if (remaining == 0) break;
                    }
                }
            }
            player.SaveServerCharacter();
            return true;
        }

        public static void NotifyTradePayment(string sellerName, int soldId, int soldQty, int paymentId, int paymentQty)
        {
            string msg = ShopLang.GetTextByAccount(sellerName, "SELL_SUCCESS", $"[i/s{soldQty}:{soldId}]", $"[i/s{paymentQty}:{paymentId}]");
            var sel = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == sellerName && p.IsLoggedIn);

            if (sel != null)
            {
                sel.GiveItem(paymentId, paymentQty);
                sel.SaveServerCharacter();
                sel.SendSuccessMessage(msg);
            }
            else
            {
                // Pago offline
                if (!P2PShop.PendingItems.ContainsKey(sellerName)) P2PShop.PendingItems[sellerName] = new List<ShopItem>();
                P2PShop.PendingItems[sellerName].Add(new ShopItem(paymentId, paymentQty, 0, "TradePayment"));

                if (!P2PShop.PendingMessages.ContainsKey(sellerName)) P2PShop.PendingMessages[sellerName] = new List<string>();
                P2PShop.PendingMessages[sellerName].Add(msg);
                ShopStorage.Save();
            }
        }
    }
}