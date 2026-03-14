using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace TerraShop
{
    public static class ShopStorage
    {
        private static string filePath = Path.Combine(TShock.SavePath, "shops.json");

        public static void Save()
        {
            try
            {
                var data = new ShopData
                {
                    GlobalShop = ShopCore.GlobalShop,
                    ShopRegions = ShopCore.ShopRegions,
                    PendingPayments = P2PShop.PendingPayments,
                    PendingMessages = P2PShop.PendingMessages,
                    PendingItems = P2PShop.PendingItems,
                    Languages = ShopLang.PlayerLanguages,
                    ExpirationMinutes = ShopCore.ShopExpirationMinutes,
                    DefaultLanguage = ShopLang.DefaultLanguage,
                    DirectOfferExpiration = ShopCore.DirectOfferExpiration
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex) { TShock.Log.Error("[TerraShop] Save Error: " + ex.Message); }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Save();
                    TShock.Log.ConsoleInfo("[TerraShop].json file created.");
                    return;
                }

                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<ShopData>(json);

                if (data != null)
                {
                    ShopCore.GlobalShop = data.GlobalShop ?? new List<ShopItem>();
                    ShopCore.ShopRegions = data.ShopRegions ?? new List<string>();
                    P2PShop.PendingPayments = data.PendingPayments ?? new Dictionary<string, long>();
                    P2PShop.PendingMessages = data.PendingMessages ?? new Dictionary<string, List<string>>();
                    P2PShop.PendingItems = data.PendingItems ?? new Dictionary<string, List<ShopItem>>();
                    ShopLang.PlayerLanguages = data.Languages ?? new Dictionary<string, string>();
                    ShopCore.ShopExpirationMinutes = data.ExpirationMinutes == 0 ? 720 : data.ExpirationMinutes;
                    ShopLang.DefaultLanguage = string.IsNullOrEmpty(data.DefaultLanguage) ? "en" : data.DefaultLanguage;
                    ShopCore.DirectOfferExpiration = string.IsNullOrEmpty(data.DirectOfferExpiration) ? "10s" : data.DirectOfferExpiration;
                }
            }
            catch (Exception ex) { TShock.Log.Error("[TerraShop] Load Error: " + ex.Message); }
        }
    }

    public class ShopData
    {
        public List<ShopItem> GlobalShop { get; set; }
        public List<string> ShopRegions { get; set; }
        public Dictionary<string, long> PendingPayments { get; set; }
        public Dictionary<string, List<string>> PendingMessages { get; set; }
        public Dictionary<string, List<ShopItem>> PendingItems { get; set; }
        public Dictionary<string, string> Languages { get; set; }
        public int ExpirationMinutes { get; set; }
        public string DefaultLanguage { get; set; }
        public string DirectOfferExpiration { get; set; }
    }
}
