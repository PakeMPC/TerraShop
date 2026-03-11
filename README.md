# ESPAÑOL
-----
## TerraShop 🛒
Un plugin avanzado de economía y comercio (P2P y Global) para servidores de Terraria (TShock). TerraShop permite a los jugadores vender e intercambiar ítems usando el chat, crear tiendas filtradas por regiones, realizar ofertas directas y recibir pagos de forma segura incluso estando desconectados.

## ✨ Características Principales
* **Tienda Global y por Región:** Vende a todos o crea una "tienda local" donde solo aparecen los items de los dueños de una región específica.
* **Trueques (Trades):** Intercambia ítems por otros ítems sin necesidad de usar monedas.
* **Economía Segura Offline:** Si alguien compra tu ítem mientras estás desconectado, recibirás el dinero en tu inventario automáticamente al volver a entrar.
* **Ofertas P2P:** Envía ofertas directas que expiran en 10 segundos a otros jugadores.
* **Multi-idioma:** Soporte integrado para Español, Inglés y Portugués (`/shoplang`).

---

## 🔐 Permisos y Comandos

| Comando | Permiso Requerido | Descripción |
| :--- | :--- | :--- |
| `/shop [página]` | `terrashop.shop` | Muestra la lista de ítems a la venta. |
| `/buy <número>` | `terrashop.shop` | Compra el ítem con el índice especificado en la tienda. |
| `/buy` | `terrashop.shop` | Acepta una oferta directa (P2P) de otro jugador. |
| `/sell <precio> <jugador>`| `terrashop.shop` | Vende el ítem que tienes en la mano. |
| `/trade <ítem> <cant> <jugador>`| `terrashop.trade` | Intercambia el ítem en tu mano por otro ítem. |
| `/shopclear` | `terrashop.shop` | Retira todos tus ítems de la tienda y te los devuelve. |
| `/shoplang <es/en/pt>` | `terrashop.shoplang` | Cambia tu idioma personal del plugin. |
| `/shopregionadd <nombre>` | `terrashop.region`* | Convierte una región de TShock en una tienda local (necesitas permisos en la region). |
| `/shopregiondelete <nombre>`| `terrashop.region`* | Elimina la tienda de una región (necesitas permisos en la region). |
| `/shoptime <tiempo>` | `terrashop.admin` | Cambia el tiempo de expiración en la tienda global (ej. 12h, 30m). |

*\* **Nota:** Para los comandos de región, el jugador también debe tener permisos de construcción (owner o shared) en la región de TShock especificada.*

---

## 📖 Ejemplos de Uso

### 1. Vender y Comprar con Monedas
Para vender un ítem, ponlo en tu mano y especifica el precio usando `c` (Cobre), `s` (Plata), `g` (Oro) o `p` (Platino):
* `/sell 50g` -> Pone el ítem en la tienda global por 50 de oro.
* `/buy 1` -> Compra el primer ítem de la lista usando `/shop`.

### 2. Hacer un Trueque (Trade)
Para pedir ítems a cambio del que tienes en la mano:
* `/trade 9 100` -> Pide 100 bloques de Madera (ID: 9).
* `/trade "Iron Broadsword" 2` -> Pide 2 espadas de hierro. (Usa comillas para nombres con espacios).

### 3. Ofertas Directas a Jugadores (P2P)
¿Estás negociando con alguien en específico? Añade el nombre del jugador al final del comando:
* `/sell 1p Jack7w7` -> Le ofrece el ítem a Jack7w7 por 1 platino.
* `/trade 9 5 PakeMPC` -> Le ofrece a PakeMPC tu ítem a cambio de 5 de madera.
* *El otro jugador tiene 10 segundos para escribir `/buy` o la oferta expirará.*

### 4. Tiendas por Región (tiendas locales)
1. Párate dentro de una región que hayas creado (`/region define MiTienda`).
2. Escribe `/shopregionadd`.
3. Ahora, cuando cualquiera entre a esa región y escriba `/shop`, **solo verá** los ítems puestos a la venta por los dueños y miembros de esa región.
A los jugadores que pasen cerca de la región les aparecerá un aviso de que está pasando por tu tienda.





# ENGLISH
-----
## TerraShop 🛒
An advanced economy and trading plugin (P2P and Global) for Terraria (TShock) servers. TerraShop allows players to sell and trade items using the chat, create region-filtered shops, make direct offers, and securely receive payments even while offline.

## ✨ Key Features
* **Global and Region Shop:** Sell to everyone or create a "local shop" where only items from the owners of a specific region appear.
* **Trades:** Exchange items for other items without the need to use coins.
* **Secure Offline Economy:** If someone buys your item while you are disconnected, you will automatically receive the money in your inventory upon rejoining.
* **P2P Offers:** Send direct offers to other players that expire in 10 seconds.
* **Multi-language:** Built-in support for Spanish, English, and Portuguese (`/shoplang`).

---

## 🔐 Permissions and Commands

| Command | Required Permission | Description |
| :--- | :--- | :--- |
| `/shop [page]` | `terrashop.shop` | Displays the list of items for sale. |
| `/buy <number>` | `terrashop.shop` | Buys the item at the specified index from the shop. |
| `/buy` | `terrashop.shop` | Accepts a direct (P2P) offer from another player. |
| `/sell <price> <player>`| `terrashop.shop` | Sells the item you are holding in your hand. |
| `/trade <item> <qty> <player>`| `terrashop.trade` | Trades the item in your hand for another item. |
| `/shopclear` | `terrashop.shop` | Removes all your items from the shop and returns them to you. |
| `/shoplang <es/en/pt>` | `terrashop.shoplang` | Changes your personal plugin language. |
| `/shopregionadd <name>` | `terrashop.region`* | Turns a TShock region into a local shop (requires permissions in the region). |
| `/shopregiondelete <name>`| `terrashop.region`* | Removes the shop from a region (requires permissions in the region). |
| `/shoptime <time>` | `terrashop.admin` | Changes the expiration time in the global shop (e.g., 12h, 30m). |

*\* **Note:** For region commands, the player must also have build permissions (owner or shared) in the specified TShock region.*

---

## 📖 Usage Examples

### 1. Selling and Buying with Coins
To sell an item, hold it in your hand and specify the price using `c` (Copper), `s` (Silver), `g` (Gold), or `p` (Platinum):
* `/sell 50g` -> Lists the item in the global shop for 50 gold.
* `/buy 1` -> Buys the first item on the list using `/shop`.

### 2. Making a Trade
To ask for items in exchange for the one in your hand:
* `/trade 9 100` -> Asks for 100 Wood blocks (ID: 9).
* `/trade "Iron Broadsword" 2` -> Asks for 2 Iron Broadswords. (Use quotes for names with spaces).

### 3. Direct Player Offers (P2P)
Are you negotiating with someone specific? Add the player's name at the end of the command:
* `/sell 1p Jack7w7` -> Offers the item to Jack7w7 for 1 platinum.
* `/trade 9 5 PakeMPC` -> Offers your item to PakeMPC in exchange for 5 wood.
* *The other player has 10 seconds to type `/buy` or the offer will expire.*

### 4. Region Shops (local shops)
1. Stand inside a region you have created (`/region define MyShop`).
2. Type `/shopregionadd`.
3. Now, whenever anyone enters that region and types `/shop`, they will **only see** items listed for sale by the owners and members of that region.
Players passing near the region will receive a notification that they are entering your shop.
