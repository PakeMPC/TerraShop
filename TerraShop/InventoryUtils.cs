using TShockAPI;
using Terraria;

namespace TerraShop
{
    public static class InventoryUtils
    {
        public static void ClearHeldItem(TSPlayer player)
        {
            if (player?.TPlayer == null) return;

            int slot = player.TPlayer.selectedItem;
            player.TPlayer.inventory[slot].SetDefaults(0);

            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, player.Index, slot);

            player.SaveServerCharacter();
        }
    }
}
