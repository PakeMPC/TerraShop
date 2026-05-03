using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraShop
{
    public class ShopItem
    {
        public int ItemID { get; set; }
        public int Quantity { get; set; }

        public byte Prefix { get; set; } = 0;
        public long PriceCopper { get; set; }
        public string Seller { get; set; }
        public DateTime DateAdded { get; set; }

        public int TradeItemID { get; set; } = 0;
        public int TradeQuantity { get; set; } = 0;
        public bool IsTrade => TradeItemID > 0;

        public ShopItem() { }
        public ShopItem(int id, int qty, long price, string seller, byte prefix = 0)
        {
            ItemID = id;
            Quantity = qty;
            PriceCopper = price;
            Seller = seller;
            DateAdded = DateTime.UtcNow;
        }
    }

    public static class ShopCore
    {
            public static List<ShopItem> GlobalShop = new List<ShopItem>();
            public static List<string> ShopRegions = new List<string>();
            public static int ShopExpirationMinutes = 720;

            public static string DirectOfferExpiration = "10s";

            public static int GetDirectOfferSeconds()
            {
                if (string.IsNullOrEmpty(DirectOfferExpiration)) return 10;

                string input = DirectOfferExpiration.ToLower();
                string numPart = new string(input.Where(char.IsDigit).ToArray());

                if (!int.TryParse(numPart, out int value)) return 300;

                if (input.EndsWith("s")) return value;
                if (input.EndsWith("h")) return value * 3600;
                if (input.EndsWith("m")) return value * 60;

                return value * 60;
            }


            public static string FormatCoins(long copper)
            {
                if (copper <= 0) return "[i/s0:71]";
                StringBuilder sb = new StringBuilder();

                long p = copper / 1000000; copper %= 1000000;
                long g = copper / 10000; copper %= 10000;
                long s = copper / 100;
                long c = copper % 100;

                if (p > 0) sb.Append($"[i/s{p}:74] ");
                if (g > 0) sb.Append($"[i/s{g}:73] ");
                if (s > 0) sb.Append($"[i/s{s}:72] ");
                if (c > 0) sb.Append($"[i/s{c}:71] ");

                return sb.ToString().Trim();
            }
        }
    }