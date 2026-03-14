using System;
using TShockAPI;
using Terraria;

namespace TerraShop
{
    public static class ShopMisc
    {
        public static bool RemoveCoins(TSPlayer player, long priceInCopper)
        {
            if (player?.TPlayer == null) return false;

            long total = 0;
            for (int i = 0; i < 59; i++)
                total += GetCoinValue(player.TPlayer.inventory[i].type, player.TPlayer.inventory[i].stack);

            total += GetCoinValue(Main.mouseItem.type, Main.mouseItem.stack);

            if (total < priceInCopper) return false;

            for (int i = 0; i < 59; i++)
            {
                int type = player.TPlayer.inventory[i].type;
                if (type >= 71 && type <= 74)
                {
                    player.TPlayer.inventory[i].SetDefaults(0);
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, player.Index, i);
                }
            }

            if (Main.mouseItem.type >= 71 && Main.mouseItem.type <= 74)
            {
                Main.mouseItem.SetDefaults(0);
                player.SendData(PacketTypes.PlayerSlot, "", player.Index, -1);
            }

            long change = total - priceInCopper;
            GiveCoins(player, change);

            // Guardado mediante comando (por si se duplica al salir xd)
            Commands.HandleCommand(TSPlayer.Server, $"/savessc \"{player.Name}\"");
            return true;
        }

        public static void GiveCoins(TSPlayer player, long copper)
        {
            int p = (int)(copper / 1000000); copper %= 1000000;
            int g = (int)(copper / 10000); copper %= 10000;
            int s = (int)(copper / 100);
            int c = (int)(copper % 100);

            if (p > 0) player.GiveItem(74, p);
            if (g > 0) player.GiveItem(73, g);
            if (s > 0) player.GiveItem(72, s);
            if (c > 0) player.GiveItem(71, c);
        }

        private static long GetCoinValue(int type, int stack)
        {
            if (type == 71) return (long)stack;
            if (type == 72) return (long)stack * 100;
            if (type == 73) return (long)stack * 10000;
            if (type == 74) return (long)stack * 1000000;
            return 0;
        }
    }
}

