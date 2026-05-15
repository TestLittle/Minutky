# Semestrální projekt - Systém objednávání minutek
## 👥 Členové týmu
| Jméno a příjmení | Role v týmu | Poměr práce
|:---|:---|:---|
| **Tomáš Rusnok** - vedoucí | Architektura, datový model, DTOs, WebAPI, Keycloak, Dokumentace | 1.2 |
| **Martin Bartoš** | Propojení API a databáze, integrační testy, Blazor klienti, SSE notifikace | 0.8 |

## Požadavky pro spuštění
1. Visual Studio 2026
2. .NET 10
3. Docker Desktop

## 🚀 Spuštění projektu
1. Ujistěte se, že běží **Docker Desktop** 
2. Otevřete solution ve Visual Studiu 2026
3. Počkejte, než se načtou všechny potřebné závislosti
4. Nastavte `UTB.Minute.AppHost` jako Start-up projekt a spusťte jej (F5)
5. V prohlížeči se otevře **.NET Aspire Dashboard**
6. U služby `utb-minute-dbmanager` klikněte na tlačítko pro spuštění příkazu **Reset Database**. Tím se databáze vyčistí a naplní testovacími daty 
7. WebAPI je následně dostupné pod službou `utb-minute-webapi`
8. Jako databázový server je vybrán Microsoft SQL Server, pro zobrazení dat klikněte na **View** -> **SQL Server Object Explorer**.
U služby `utb-minute-db` v **Aspire Dashboard** klikněte na tlačítko více informací a zkopírujte connection string.
Tento connection string vložte do karty **Connection String** po kliknutí na **Add new SQL Server**.
9. Pro přístup k objednávkovému systému, klikněte v **Aspire Dashboard** na `utb-minute-canteenclient`, pro úpravu jídel klikněte na `utb-minute-adminclient` (mohou využívat pouze uživatelé s rolí `meal-admin`), kde budete vyzváni k přihlášení (využijte níže uvedené uživatele)

|Uživatelé|
|:---|
|**Kuchař (role cook)** => Pavel Test - username: **pavel**, heslo: **pavel**|
|**Správce jídel (role meal-admin)** => Jan Meal - username: **jmeal**, heslo: **jmeal**|

## 🗂️ Struktura, datový model a DTO
Struktura projektu vychází ze specifikací a vypadá následovně:

- `UTB.Minute.AppHost`: Aspire orchestrace, která propojuje a spouští všechny služby.
- `UTB.Minute.Db`: Jednotlivé datové entity, které jsou uvedeny níže, a `DbContext`.
- `UTB.Minute.DbManager`: Obsahuje endpoint pro **Http Command** (reset databáze).
- `UTB.Minute.Contracts`: Sdílená DTO, aby byla zajištěna typová bezpečnost mezi API a klienty.
- `UTB.Minute.WebApi`: Hlavní byznys logika, správa objednávek a SSE hub.
- `UTB.Minute.WebApi.Tests`: Hlavní automatizované integrační testy, které pokrývají funkcionalitu jednotlivých WebApi requestů.
- `UTB.Minute.AdminClient`: Blazor klient umožňující správu jídel autorizovaným uživatelům s rolí `meal-admin` na stránce **Meals** a možnosti plánování jídel na stránce **MenuManager**
- `UTB.Minute.CanteenClient`: Blazor klient umožňující objednání jídla z menu na stránce **TodayMenu**, přehled objednávek na stránce **Orders** a možnost změny stavu jídel přihlášeným uživatelům s rolí `cook`, jež jsou povinni se přihlásit pro přístup k dané stránce, na stránce **Kitchen**

Architektura striktně odděluje databázové entity od objektů, které se posílají ven přes API. Entity nejsou nikdy vraceny přímo.

**Entity (projekt `UTB.Minute.Db`):**
* `MinuteMeal` - Obsahuje vlastnost `IsActive` pro zajištění Soft-delete (jídla se nemažou, pouze deaktivují).
* `MenuItem` - Konkrétní položka v menu pro daný den s počtem porcí obsahující `[Timestamp]` pro vyřešení problému souběžnosti.
* `Order` - Samotná objednávka vázaná na menu.
* `OrderStatus` - Enum reprezentující stavy (Preparing, Ready, Cancelled, Completed).

**DTO objekty (projekt `UTB.Minute.Contracts`):**
* Pro jídla: `MinuteMealDto`, `MinuteMealRequestDto`, `MinuteMealPatchDescDto`, `MinuteMealPatchPriceDto`, `MinuteMealPatchIsActiveDto`
* Pro menu: `MenuItemDto`, `MenuItemRequestDto`, `MenuItemPatchDateDto`, `MenuItemPatchPortionsDto`, `MenuItemPatchMeal`
* Pro objednávky: `OrderDto`, `OrderRequestDto`, `OrderPatchStatusDto`, `OrderPatchMenuItemDto`

## 🛠️ Klíčová implementační rozhodnutí

### 1. Autorizace a Keycloak
**CanteenClient** - K objednávání má přístup každý uživatel (přihlášený i nepříhlášený), ovšem po kliknutí na možnost **Kuchařka** dojde k automatickému přesměrování na přihlašovací formulář, takže zde je to vyřešené pomocí `[Authorize(Roles = "cook")]` na samotné stránce společně s `<AuthorizeView Roles="cook">`, kde je přidána funcionalita pro odhlášení přihlášeného uživatele.
**AdminClient** - Zde je zabezpečená veškerá aplikace pomocí `app.MapRazorComponents<App>().RequireAuthorization(pb => pb.RequireRole("meal-admin")).AddInteractiveServerRenderMode();`

### 2. SSE Notifikace
-

### 3. Business pravidla
Počet objednávek je ošetřeno ve `WebAPI` díky `[Timestamp]` přidané k položce `MenuItem`.

---

## 📝 Poznámky k odevzdání (Stav projektu)
* **Stav:** Projekt splňuje všechny body potřebné k splnění semestrálního odevzdání, kromě implementovaných SSE notifikací.
* **Souběžnost:** Snížení počtu porcí při objednávce je ošetřeno proti souběžnému přístupu (Concurrency).
* **Testování:** Automatizované integrační testy (`UTB.Minute.WebApi.Tests`) využívají testovací kontejnerizovanou databázi spravovanou přes .NET Aspire. Testy pokrývají kompletní scénář od vytvoření jídla, přes úpravu, přidání do menu, vytvoření objednávky, změnu stavu až po konečnou deaktivaci a smazání. Zároveň dochází k otestování několika nevalidních vstupů. Testy prochází bez chyb a bez warningů.

## 🧪 Seznam API endpointů
* `GET /minuteMeals` - Seznam všech jídel.
* `GET /minuteMeals/active` - Seznam všech aktivních jídel.
* `POST /minuteMeals` - Vytvoření nového jídla.
* `PUT /minuteMeals/{id}` - Plná aktualizace (popis, cena a zda je aktivní) jídla.
* `PATCH /minuteMeals/{id}/active` - Aktualizace stavu aktivity jídla.
* `PATCH /minuteMeals/{id}/desc` - Aktualizace popisu jídla.
* `PATCH /minuteMeals/{id}/price` - Aktualizace ceny jídla.

* `GET /menuItems` - Seznam všech položek menu.
* `POST /menuItems` - Vytvoření nové položky menu.
* `DELETE /menuItems/{id}` - Smazání položky menu.
* `PUT /menuItems/{id}` - Plná aktualizace (datum, počet porcí a ID jídla) položky menu.
* `PATCH /menuItems/{id}/date` - Aktualizace data vydávání položky menu.
* `PATCH /menuItems/{id}/meal` - Aktualizace jídla u položky menu.
* `PATCH /menuItems/{id}/portion` - Snížení počtu porcí o jednu u položky menu.
* `PATCH /menuItems/{id}/portions` - Aktualizace počtu porcí na danou hodnotu u položky menu.

* `GET /orders` - Seznam všech objednávek.
* `POST /orders` - Vytvoření nové objednávky.
* `PUT /orders/{id}` - Plná aktualizace (stav, ID položky menu) objednávky. 
* `PATCH /orders/{id}/status` - Aktualizace stavu objednávky.
* `PATCH /orders/{id}/menuItem` - Aktualizace položky menu u objednávky.

