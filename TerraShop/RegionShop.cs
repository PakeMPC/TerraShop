using System.Collections.Generic;
using System.Linq; 
using TShockAPI;
using Terraria;

namespace TerraShop
{
    public static class RegionShop
    {
        public static void AddRegionShop(CommandArgs args)
        {
            var player = args.Player;

            var regionsInArea = TShock.Regions.InAreaRegion(player.TileX, player.TileY);
            var topRegion = TShock.Regions.GetTopRegion(regionsInArea);

            var regionParams = args.Parameters.Skip(2).ToList();

            string regionName = regionParams.Count > 0
                ? string.Join(" ", regionParams)
                : topRegion?.Name;

            if (string.IsNullOrEmpty(regionName) || TShock.Regions.GetRegionByName(regionName) == null)
            {
                player.SendErrorMessage(ShopLang.GetText(player, "REGION_NOT_FOUND"));
                return;
            }

            if (!TShock.Regions.CanBuild(player.TileX, player.TileY, player))
            {
                player.SendErrorMessage(ShopLang.GetText(player, "NO_REGION_PERMS"));
                return;
            }

            if (ShopCore.ShopRegions.Contains(regionName))
            {
                player.SendErrorMessage(ShopLang.GetText(player, "REGION_EXISTS"));
                return;
            }

            ShopCore.ShopRegions.Add(regionName);
            player.SendSuccessMessage(ShopLang.GetText(player, "REGION_CREATED", regionName));
            ShopStorage.Save();
        }

        public static void DeleteRegionShop(CommandArgs args)
        {
            var player = args.Player;

            var regionsInArea = TShock.Regions.InAreaRegion(player.TileX, player.TileY);
            var topRegion = TShock.Regions.GetTopRegion(regionsInArea);

            var regionParams = args.Parameters.Skip(2).ToList();

            string regionName = regionParams.Count > 0
                ? string.Join(" ", regionParams)
                : topRegion?.Name;

            if (string.IsNullOrEmpty(regionName) || !ShopCore.ShopRegions.Contains(regionName))
            {
                player.SendErrorMessage(ShopLang.GetText(player, "REGION_NOT_FOUND"));
                return;
            }

            if (!TShock.Regions.CanBuild(player.TileX, player.TileY, player))
            {
                player.SendErrorMessage(ShopLang.GetText(player, "NO_REGION_PERMS"));
                return;
            }

            ShopCore.ShopRegions.Remove(regionName);
            player.SendSuccessMessage(ShopLang.GetText(player, "REGION_DELETED", regionName));
            ShopStorage.Save();
        }
    }
}