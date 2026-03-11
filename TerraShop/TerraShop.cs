using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;

namespace TerraShop
{
    [ApiVersion(2, 1)]
    public class TerraShop : TerrariaPlugin
    {
        public override string Name => "TerraShop";
        public override string Author => "PakeMPC";
        public override Version Version => new Version(1, 3, 0);

        private Dictionary<int, string> LastPlayerRegion = new Dictionary<int, string>();

        public TerraShop(Main game) : base(game) { }

        public override void Initialize()
        {
            ShopStorage.Load();
            Commands.ChatCommands.Add(new Command("terrashop.shop", P2PShop.ShowShop, "shop"));
            Commands.ChatCommands.Add(new Command("terrashop.shop", P2PShop.SellItem, "sell"));
            Commands.ChatCommands.Add(new Command("terrashop.shop", P2PShop.BuyLogic, "buy"));
            Commands.ChatCommands.Add(new Command("terrashop.region", RegionShop.AddRegionShop, "shopregionadd"));
            Commands.ChatCommands.Add(new Command("terrashop.region", RegionShop.DeleteRegionShop, "shopregiondelete"));
            Commands.ChatCommands.Add(new Command("terrashop.shoplang", (args) => { if (args.Parameters.Count > 0) ShopLang.ChangeLanguage(args.Player, args.Parameters[0].ToLower()); }, "shoplang"));
            Commands.ChatCommands.Add(new Command("terrashop.shop", ClearCommand, "shopclear"));
            Commands.ChatCommands.Add(new Command("terrashop.admin", TimeCommand, "shoptime"));
            Commands.ChatCommands.Add(new Command("terrashop.trade", TradeShop.CreateTrade, "trade"));

            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Main.GameUpdateCount % 60 != 0) return;

            var now = DateTime.UtcNow;

            foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn && p.State >= 10))
            {
                var regList = TShock.Regions.InAreaRegion(player.TileX, player.TileY);
                var shopReg = regList.FirstOrDefault(r => ShopCore.ShopRegions.Contains(r.Name));

                if (shopReg != null)
                {
                    if (!LastPlayerRegion.ContainsKey(player.Index) || LastPlayerRegion[player.Index] != shopReg.Name)
                    {
                        List<string> members = new List<string> { shopReg.Owner };
                        foreach (int id in shopReg.AllowedIDs)
                        {
                            var acc = TShock.UserAccounts.GetUserAccountByID(id);
                            if (acc != null) members.Add(acc.Name);
                        }
                        string allMembers = string.Join(", ", members.Distinct());
                        player.SendInfoMessage(ShopLang.GetText(player, "REGION_ENTER_NOTICE", allMembers));
                    }
                    LastPlayerRegion[player.Index] = shopReg.Name;
                }
                else LastPlayerRegion[player.Index] = null;

                string accName = player.Account.Name;
                bool needsSave = false;

                if (P2PShop.PendingPayments.ContainsKey(accName))
                {
                    long amt = P2PShop.PendingPayments[accName];
                    ShopMisc.GiveCoins(player, amt);
                    P2PShop.PendingPayments.Remove(accName);
                    string desc = ShopLang.GetText(player, "OFFLINE_PAYMENT_DESC");
                    player.SendSuccessMessage(ShopLang.GetText(player, "SELL_SUCCESS", desc, ShopCore.FormatCoins(amt)));
                    needsSave = true;
                }

                if (P2PShop.PendingItems.ContainsKey(accName))
                {
                    foreach (var item in P2PShop.PendingItems[accName]) player.GiveItem(item.ItemID, item.Quantity);
                    P2PShop.PendingItems.Remove(accName);
                    needsSave = true;
                }

                if (P2PShop.PendingMessages.ContainsKey(accName))
                {
                    foreach (var m in P2PShop.PendingMessages[accName]) player.SendInfoMessage(m);
                    P2PShop.PendingMessages.Remove(accName);
                    needsSave = true;
                }

                if (needsSave)
                {
                    player.SaveServerCharacter();
                    ShopStorage.Save();
                }
            }

            // 2. Expiración P2P (10 segundos)
            var expiredOffers = P2PShop.DirectOffers.Where(kvp => (now - kvp.Value.DateAdded).TotalSeconds >= 10).ToList();
            foreach (var kvp in expiredOffers)
            {
                P2PShop.ReturnItem(kvp.Value.Seller, kvp.Value);

                var target = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == kvp.Key);
                if (target != null) target.SendErrorMessage(ShopLang.GetText(target, "OFFER_EXPIRED"));

                P2PShop.DirectOffers.Remove(kvp.Key);
            }

            // 3. Expiración Global de ítems
            CheckExpirations();
        }

        private void CheckExpirations()
        {
            if (ShopCore.ShopExpirationMinutes < 0) return;
            var now = DateTime.UtcNow;

            var expiredGlobal = ShopCore.GlobalShop.Where(i => (now - i.DateAdded).TotalMinutes >= ShopCore.ShopExpirationMinutes).ToList();
            foreach (var item in expiredGlobal)
            {
                ShopCore.GlobalShop.Remove(item);

                string msg = ShopLang.GetTextByAccount(item.Seller, "EXPIRED_NOTICE", $"[i/s{item.Quantity}:{item.ItemID}]");

                var seller = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == item.Seller);
                if (seller != null && seller.IsLoggedIn)
                {
                    seller.GiveItem(item.ItemID, item.Quantity);
                    seller.SaveServerCharacter();
                    seller.SendErrorMessage(msg);
                }
                else
                {
                    if (!P2PShop.PendingItems.ContainsKey(item.Seller)) P2PShop.PendingItems[item.Seller] = new List<ShopItem>();
                    P2PShop.PendingItems[item.Seller].Add(item);

                    if (!P2PShop.PendingMessages.ContainsKey(item.Seller)) P2PShop.PendingMessages[item.Seller] = new List<string>();
                    P2PShop.PendingMessages[item.Seller].Add(msg);
                }
            }
            if (expiredGlobal.Count > 0) ShopStorage.Save();
        }

        private void ClearCommand(CommandArgs args)
        {
            var myItems = ShopCore.GlobalShop.Where(i => i.Seller == args.Player.Account.Name).ToList();
            foreach (var item in myItems)
            {
                args.Player.GiveItem(item.ItemID, item.Quantity);
                ShopCore.GlobalShop.Remove(item);
            }
            args.Player.SaveServerCharacter();
            args.Player.SendSuccessMessage(ShopLang.GetText(args.Player, "SHOP_CLEARED"));
            ShopStorage.Save();
        }

        private void TimeCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0) return;
            string input = string.Join("", args.Parameters).ToLower();
            int minutes = 0;

            if (input.Contains("i")) minutes = -1;
            else
            {
                string numPart = new string(input.Where(char.IsDigit).ToArray());
                if (!int.TryParse(numPart, out int value)) return;

                if (input.EndsWith("s")) minutes = Math.Max(1, value / 60);
                else if (input.EndsWith("m")) minutes = value;
                else if (input.EndsWith("h")) minutes = value * 60;
                else if (input.EndsWith("d")) minutes = value * 1440;
                else minutes = value;
            }

            ShopCore.ShopExpirationMinutes = minutes;
            string timeDesc = minutes == -1 ? "Infinito" : minutes + "m";
            args.Player.SendSuccessMessage(ShopLang.GetText(args.Player, "TIME_SET", timeDesc));
            ShopStorage.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ShopStorage.Save();
            }
            base.Dispose(disposing);
        }
    }
}