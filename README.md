# Semestrální projekt - Systém objednávání minutek
## 👥 Členové týmu
| Jméno a příjmení | Role v týmu
|:---|:---|
| **Tomáš Rusnok** - vedoucí | Architektura, datový model, DTOs, WebAPI |
| **Martin Bartoš** | Propojení API a databáze, integrační testy, stručná dokumentace |

## Požadavky pro spuštění
1. Visual Studio 2026
2. .NET 10
3. Docker Desktop

## 🚀 Spuštění projektu
1. Ujistěte se, že běží **Docker Desktop** 
2. Otevřete solution ve Visual Studiu 
3. Nastavte `UTB.Minute.AppHost` jako Start-up projekt a spusťte jej (F5)
4. V prohlížeči se otevře **.NET Aspire Dashboard**
5. U služby `utb-minute-dbmanager` klikněte na tlačítko pro spuštění příkazu **Reset Database**. Tím se databáze vyčistí a naplní testovacími daty 
6. WebAPI je následně dostupné pod službou `utb-minute-webapi`
7. Jako databázový server je vybrán Microsoft SQL Server, pro zobrazení dat klikněte na **View** -> **SQL Server Object Explorer**.
U služby `utb-minute-db` v **Aspire Dashboard** klikněte na tlačítko více informací a zkopírujte connection string.
Tento connection string vložte do karty **Connection String** po kliknutí na **Add new SQL Server**.

## 🗂️ Struktura, datový model a DTO
Struktura projektu vychází ze specifikací a vypadá následovně:

- `UTB.Minute.AppHost`: Aspire orchestrace, která propojuje a spouští všechny služby.
- `UTB.Minute.Db`: Jednotlivé datové entity, které jsou uvedeny níže, a `DbContext`.
- `UTB.Minute.DbManager`: Obsahuje endpoint pro **Http Command** (reset databáze).
- `UTB.Minute.Contracts`: Sdílená DTO, aby byla zajištěna typová bezpečnost mezi API a klienty.
- `UTB.Minute.WebApi`: Hlavní byznys logika, správa objednávek a SSE hub.
- `UTB.Minute.WebApi.Tests`: Hlavní automatizované integrační testy, které pokrývají funkcionalitu jednotlivých WebApi requestů.

Architektura striktně odděluje databázové entity od objektů, které se posílají ven přes API. Entity nejsou nikdy vraceny přímo.

**Entity (projekt `UTB.Minute.Db`):**
* `MinuteMeal` - Obsahuje vlastnost `IsActive` pro zajištění Soft-delete (jídla se nemažou, pouze deaktivují).
* `MenuItem` - Konkrétní položka v menu pro daný den s počtem porcí.
* `Order` - Samotná objednávka vázaná na menu.
* `OrderStatus` - Enum reprezentující stavy (Preparing, Ready, Cancelled, Completed).

**DTO objekty (projekt `UTB.Minute.Contracts`):**
* Pro jídla: `MinuteMealDto`, `MinuteMealRequestDto`, `MinuteMealPatchDescDto`, `MinuteMealPatchPriceDto`, `MinuteMealPatchIsActiveDto`
* Pro menu: `MenuItemDto`, `MenuItemRequestDto`, `MenuItemPatchDateDto`, `MenuItemPatchPortionsDto`, `MenuItemPatchMeal`
* Pro objednávky: `OrderDto`, `OrderRequestDto`, `OrderPatchStatusDto`, `OrderPatchMenuItemDto`

## 📝 Poznámky k odevzdání (Stav projektu)
* **Stav:** Projekt je plně funkční a splňuje všechny požadavky pro půlsemestrální odevzdání.
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
