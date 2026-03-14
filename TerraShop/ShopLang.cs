using System.Collections.Generic;
using Terraria.Localization;
using TShockAPI;

namespace TerraShop
{

    //Llaves de traducción para mensajes (pueden contrubuir con traducciones)
    //Translation keys for messages (community contributions welcome) -En inglés por si nadie entiende xd
    public static class ShopLang
    {
        public static string DefaultLanguage = "en";

        public static Dictionary<string, string> PlayerLanguages = new Dictionary<string, string>();

        private static readonly Dictionary<string, Dictionary<string, string>> Languages = new Dictionary<string, Dictionary<string, string>>
        {
            ["es"] = new Dictionary<string, string> {
                { "NO_ITEMS", "No hay ítems en la tienda." },
                { "HELD_EMPTY", "No tienes nada en la mano para vender." },
                { "INVALID_COIN", "No se puede vender dinero." },
                { "INVALID_PRICE", "Precio inválido." },
                { "USAGE_SELL", "Uso: /sell <precio><c/s/g/p> [jugador]" },
                { "PLAYER_NOT_FOUND", "Jugador no encontrado." },
                { "OFFER_SENT", "Oferta enviada a {0} por {1}." },
                { "OFFER_RECEIVED", "{0} vende {1} por {2}. Usa /buy para aceptar." },
                { "ITEM_ADDED", "Añadido {0} a la tienda por {1}." },
                { "BUY_SUCCESS", "Has comprado {0} por {1}." },
                { "SELL_SUCCESS", "Has vendido {0} por {1}." },
                { "OFFER_EXPIRED", "La oferta ha expirado." },
                { "ITEM_NOT_FOUND", "Ítem no encontrado." },
                { "OWN_ITEM", "No puedes comprar tu propio ítem." },
                { "TIME_PERMISSION", "Permisos insuficientes para cambiar el tiempo." },
                { "NO_COINS", "No tienes suficientes monedas." },
                { "NO_ITEMS_TRADE", "No tienes suficientes items." },
                { "ITEM_ID_USE", "Puedes usar el ID del item." },
                { "LANG_CHANGED", "Idioma cambiado a {0}." },
                { "REGION_EXISTS", "Ya existe una tienda aquí." },
                { "REGION_CREATED", "Tienda habilitada en {0}." },
                { "REGION_NOT_FOUND", "Región no encontrada." },
                { "REGION_DELETED", "Tienda eliminada de {0}." },
                { "REGION_INSTRUCTIONS", "Uso: /shop region <add|delete> [NombreDeRegion]" },
                { "BUY_NUMBER", "Usa /buy <número> para comprar." },
                { "NO_REGION_PERMS", "No tienes permisos en esta región." },
                { "USAGE_SHOPLANG", "Uso: /shoplang <es|en|pt>" },
                { "TIME_SET", "Expiración ajustada a {0}." },
                { "SHOP_CLEARED", "Tienda limpiada." },
                { "REGION_ENTER_NOTICE", "--- Tienda de: {0}. /shop para ver---" },
                { "EXPIRED_NOTICE", "Producto {0} no vendido, límite de tiempo excedido." },
                { "OFFLINE_PAYMENT_DESC", "ventas offline" },
                { "SHOP_HEADER", "--- Tienda Global ({0}/{1}) ---" },
                { "SHOP_HEADER_FILTER", "--- Tienda en {0} ({1}/{2}) ---" },
                { "SHOP_FOOTER_MORE", "/shop {0} para ver más" },
                { "FOR_TEXT", "por" },
                { "USAGE_TRADE", "Uso: /trade <\"Nombre Item\"/ID> [cantidad] [jugador]" },
                { "CAN_USE_SELL", "Si quieres dinero, puedes usar /sell" },
            },
            ["en"] = new Dictionary<string, string> {
                { "NO_ITEMS", "No items in shop." },
                { "HELD_EMPTY", "You are not holding anything to sell." },
                { "INVALID_COIN", "You cannot sell money." },
                { "INVALID_PRICE", "Invalid price." },
                { "USAGE_SELL", "Usage: /sell <price><c/s/g/p> [player]" },
                { "PLAYER_NOT_FOUND", "Player not found." },
                { "OFFER_SENT", "Offer sent to {0} for {1}." },
                { "OFFER_RECEIVED", "{0} sells {1} for {2}. Use /buy to accept." },
                { "ITEM_ADDED", "Added {0} to shop for {1}." },
                { "BUY_SUCCESS", "Bought {0} for {1}." },
                { "SELL_SUCCESS", "Sold {0} for {1}." },
                { "OFFER_EXPIRED", "Offer expired." },
                { "ITEM_NOT_FOUND", "Item not found." },
                { "OWN_ITEM", "Cannot buy own item." },
                { "TIME_PERMISSION", "Insuficient permissions to change the time." },
                { "NO_COINS", "Not enough coins." },
                { "NO_ITEMS_TRADE", "Not enough required item." },
                { "ITEM_ID_USE", "You can use the item ID." },
                { "LANG_CHANGED", "Language changed to {0}." },
                { "REGION_EXISTS", "Shop already exists here." },
                { "REGION_CREATED", "Shop created in {0}." },
                { "REGION_NOT_FOUND", "Region not found." },
                { "REGION_DELETED", "Shop deleted from {0}." },
                { "REGION_INSTRUCTIONS", "Use: /shop region <add|delete> [RegionName]" },
                { "BUY_NUMBER", "Use /buy <number> to purchase." },
                { "NO_REGION_PERMS", "No permission in this region." },
                { "USAGE_SHOPLANG", "Usage: /shoplang <es|en|pt>" },
                { "TIME_SET", "Expiration adjusted to {0}." },
                { "SHOP_CLEARED", "Shop cleared." },
                { "REGION_ENTER_NOTICE", "--- {0}'s Shop. /shop to see ---" },
                { "EXPIRED_NOTICE", "Product {0} not sold, time limit exceeded." },
                { "OFFLINE_PAYMENT_DESC", "offline sales" },
                { "SHOP_HEADER", "--- Global Shop ({0}/{1}) ---" },
                { "SHOP_HEADER_FILTER", "--- Shop in {0} ({1}/{2}) ---" },
                { "SHOP_FOOTER_MORE", "/shop {0} to see more" },
                { "FOR_TEXT", "for" },
                { "USAGE_TRADE", "Usage: /trade <\"Item Name\"/ID> [amount] [player]" },
                { "CAN_USE_SELL", "If you want money, can use /sell" },

            },
            ["pt"] = new Dictionary<string, string> {
                { "NO_ITEMS", "Não há itens na loja." },
                { "HELD_EMPTY", "Você não tem nada na mão para vender." },
                { "INVALID_COIN", "Você não pode vender dinheiro." },
                { "INVALID_PRICE", "Preço inválido." },
                { "USAGE_SELL", "Uso: /sell <preço><c/s/g/p> [jogador]" },
                { "PLAYER_NOT_FOUND", "Jogador não encontrado." },
                { "OFFER_SENT", "Oferta enviada para {0} por {1}." },
                { "OFFER_RECEIVED", "{0} vende {1} por {2}. Use /buy para aceitar." },
                { "ITEM_ADDED", "Adicionado {0} à loja por {1}." },
                { "BUY_SUCCESS", "Comprou {0} por {1}." },
                { "SELL_SUCCESS", "Vendeu {0} por {1}." },
                { "OFFER_EXPIRED", "Oferta expirada." },
                { "ITEM_NOT_FOUND", "Item não encontrado." },
                { "OWN_ITEM", "Não pode comprar seu próprio item." },
                { "TIME_PERMISSION", "Permissões insuficientes para mudar o tempo." },
                { "NO_COINS", "Moedas insuficientes." },
                { "NO_ITEMS_TRADE", "Não tem os suficientes items." },
                { "ITEM_ID_USE", "Pode usar o ID do item." },
                { "LANG_CHANGED", "Idioma alterado para {0}." },
                { "REGION_EXISTS", "Loja já existe aqui." },
                { "REGION_CREATED", "Loja criada em {0}." },
                { "REGION_NOT_FOUND", "Região não encontrada." },
                { "REGION_DELETED", "Loja removida de {0}." },
                { "REGION_INSTRUCTIONS", "Uso: /shop region <add|delete> [NomeDaRegião]" },
                { "BUY_NUMBER", "Use /buy <número> para comprar." },
                { "NO_REGION_PERMS", "Sem permissão nesta região." },
                { "USAGE_SHOPLANG", "Uso: /shoplang <es|en|pt>" },
                { "TIME_SET", "Expiração ajustada para {0}." },
                { "SHOP_CLEARED", "Loja limpa." },
                { "REGION_ENTER_NOTICE", "--- Loja de: {0}. /shop para ver ---" },
                { "EXPIRED_NOTICE", "Produto {0} não vendido, limite de tempo excedido." },
                { "OFFLINE_PAYMENT_DESC", "vendas offline" },
                { "SHOP_HEADER", "--- Loja Global ({0}/{1}) ---" },
                { "SHOP_HEADER_FILTER", "--- Loja em {0} ({1}/{2}) ---" },
                { "SHOP_FOOTER_MORE", "/shop {0} para ver mais" },
                { "FOR_TEXT", "por" },
                { "USAGE_TRADE", "Uso: /trade <\"Nome Item\"/ID> [quantidade] [jogador]" },
                { "CAN_USE_SELL", "Se quiser dinheiro, pode usar /sell" }
            }
        };

        public static string GetTextByAccount(string accountName, string key, params object[] args)
        {
            string lang = DefaultLanguage;

            if (!string.IsNullOrEmpty(accountName) && PlayerLanguages.TryGetValue(accountName, out string pLang))
            {
                lang = pLang;
            }

            if (!Languages.ContainsKey(lang))
            {
                lang = "en";
            }

            if (Languages.ContainsKey(lang) && Languages[lang].ContainsKey(key))
            {
                return string.Format(Languages[lang][key], args);
            }

            return key; 
        }

        // Esto es una función descartada xd, pero la dejo por si acaso y referencio a la de arriba para no romper nada
        public static string GetText(TSPlayer player, string key, params object[] args)
        {
            return GetTextByAccount(player?.Account?.Name, key, args); 
        }

        public static void ChangeLanguage(TSPlayer player, string lang)
        {
            if (Languages.ContainsKey(lang) && player?.Account != null)
            {
                PlayerLanguages[player.Account.Name] = lang;
                player.SendSuccessMessage(GetText(player, "LANG_CHANGED", lang)); 
                ShopStorage.Save(); 
            }
        }
    }
}
