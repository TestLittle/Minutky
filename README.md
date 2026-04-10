# Semestrální projekt - Systém objednávání minutek
## 👥 Členové týmu
* **Tomáš Rusnok** - Architektura, datový model, DTOs, WebAPI
* **Martin Bartoš** - Propojení API a databáze, integrační testy, stručná dokumentace

## 🚀 Spuštění projektu
1. Ujistěte se, že běží **Docker Desktop** 
2. Otevřete solution ve Visual Studiu 
3. Nastavte `UTB.Minute.AppHost` jako Start-up projekt a spusťte jej (F5)
4. V prohlížeči se otevře **.NET Aspire Dashboard**
5. U služby `utb-minute-dbmanager` klikněte na tlačítko pro spuštění příkazu **Reset Database**. Tím se databáze vyčistí a naplní testovacími daty 
6. WebAPI je následně dostupné pod službou `utb-minute-webapi`
7. Jako databázový server je vybrán Microsoft SQL Server, pro zobrazení dat klikněte na **View** -> **SQL Server Object Explorer**.
U služby `utb-minute-db` klikněte na tlačítko více informací a zkopírujte connection string.
Tento connection string vložte do karty **Connection String** po kliknutí na **Add new SQL Server**.

## 🗂️ Datový model a DTO
Architektura striktně odděluje databázové entity od objektů, které se posílají ven přes API. Entity nejsou nikdy vraceny přímo.

**Entity (projekt `UTB.Minute.Db`):**
* `MinuteMeal` - Obsahuje vlastnost `IsActive` pro zajištění Soft-delete (jídla se nemažou, pouze deaktivují).
* `MenuItem` - Konkrétní položka v menu pro daný den s počtem porcí.
* `Order` - Samotná objednávka vázaná na menu.
* `OrderStatus` - Enum reprezentující stavy (Preparing, Finished, Cancelled, Completed).

**DTO objekty (projekt `UTB.Minute.Contracts`):**
* Pro jídla: `MinuteMealDto`, `MinuteMealRequestDto`, `MinuteMealPatchDescDto`, `MinuteMealPatchPriceDto`, `MinuteMealPatchIsActiveDto`
* Pro menu: `MenuItemDto`, `MenuItemRequestDto`, `MenuItemPatchDateDto`, `MenuItemPatchPortionsDto`, `MenuItemPatchMeal`
* Pro objednávky: `OrderDto`, `OrderRequestDto`, `OrderPatchStatusDto`, `OrderPatchMenuItemDto`

## 📝 Poznámky k odevzdání (Stav projektu)
* **Stav:** Projekt je plně funkční a splňuje všechny požadavky pro půlsemestrální odevzdání.
* **Souběžnost:** Snížení počtu porcí při objednávce je ošetřeno proti souběžnému přístupu (Concurrency).
* **Testování:** Automatizované integrační testy (`UTB.Minute.WebApi.Tests`) využívají reálnou kontejnerizovanou databázi spravovanou přes .NET Aspire. Testy pokrývají kompletní scénář od vytvoření jídla, přes úpravu, přidání do menu, vytvoření objednávky, změnu stavu až po konečnou deaktivaci a smazání. Testy prochází bez chyb a bez warningů.
