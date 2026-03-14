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
        public override Version Version => new Version(1, 4, 0);

        private Dictionary<int, string> LastPlayerRegion = new Dictionary<int, string>();

        public TerraShop(Main game) : base(game) { }

        public override void Initialize()
        {
            ShopStorage.Load();

            Commands.ChatCommands.Add(new Command("terrashop.shop", ShopCommandRouter, "shop"));
            Commands.ChatCommands.Add(new Command("terrashop.shop", P2PShop.SellItem, "sell"));
            Commands.ChatCommands.Add(new Command("terrashop.shop", P2PShop.BuyLogic, "buy"));
            Commands.ChatCommands.Add(new Command("terrashop.trade", TradeShop.CreateTrade, "trade"));

            Commands.ChatCommands.Add(new Command("terrashop.shoplang", (args) => {
                if (args.Parameters.Count > 0)
                    ShopLang.ChangeLanguage(args.Player, args.Parameters[0].ToLower());
                else
                    args.Player.SendErrorMessage(ShopLang.GetText(args.Player, "USAGE_SHOPLANG"));
            }, "shoplang"));

            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void ShopCommandRouter(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string sub = args.Parameters[0].ToLower();

                if (int.TryParse(sub, out _))
                {
                    P2PShop.ShowShop(args);
                    return;
                }

                switch (sub)
                {
                    case "clear":
                        ClearCommand(args);
                        return;

                    case "time":
                        if (!args.Player.HasPermission("shop.time"))
                        {
                            args.Player.SendErrorMessage("TIME_PERMISSION");
                            return;
                        }

                        TimeCommand(args);
                        return;

                    case "region":
                        if (!args.Player.HasPermission("terrashop.region"))
                        {
                            args.Player.SendErrorMessage(ShopLang.GetText(args.Player, "NO_REGION_PERMS"));
                            return;
                        }

                        if (args.Parameters.Count > 1)
                        {
                            string action = args.Parameters[1].ToLower();
                            if (action == "add") { RegionShop.AddRegionShop(args); return; }
                            if (action == "delete" || action == "del" || action == "remove") { RegionShop.DeleteRegionShop(args); return; }
                        }

                        args.Player.SendErrorMessage("REGION_INSTRUCTIONS");
                        return;
                }
            }

            P2PShop.ShowShop(args);
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
                    foreach (var m in P2PShop.PendingMessages[accName]) player.SendErrorMessage(m);
                    P2PShop.PendingMessages.Remove(accName);
                    needsSave = true;
                }

                if (needsSave)
                {
                    player.SaveServerCharacter();
                    ShopStorage.Save();
                }
            }

            // 2. Expiración de Ofertas Directas (P2P) leyendo el .json
            int maxSeconds = ShopCore.GetDirectOfferSeconds();
            var expiredOffers = P2PShop.DirectOffers.Where(kvp => (now - kvp.Value.DateAdded).TotalSeconds >= maxSeconds).ToList();
            foreach (var kvp in expiredOffers)
            {
                P2PShop.ReturnItem(kvp.Value.Seller, kvp.Value);

                var target = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == kvp.Key);
                if (target != null)
                    target.SendErrorMessage(ShopLang.GetText(target, "EXPIRED_NOTICE", $"[i/s{kvp.Value.Quantity}:{kvp.Value.ItemID}]"));

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
                P2PShop.ReturnItem(item.Seller, item);
            }
            if (expiredGlobal.Count > 0) ShopStorage.Save();
        }

        private void ClearCommand(CommandArgs args)
        {
            ShopCore.GlobalShop.Clear();
            foreach (var p in TShock.Players.Where(p => p != null && p.IsLoggedIn))
                p.SaveServerCharacter();

            args.Player.SendSuccessMessage(ShopLang.GetText(args.Player, "SHOP_CLEARED"));
            ShopStorage.Save();
        }

        private void TimeCommand(CommandArgs args)
        {
            var timeParams = args.Parameters.Skip(1).ToList();
            if (timeParams.Count == 0) return;

            string input = string.Join("", timeParams).ToLower();
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
