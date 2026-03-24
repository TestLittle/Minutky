# Semestrální projekt - Systém objednávání minutek
## 👥 Členové týmu
* **Tomáš Rusnok** - Architektura, datový model, DTOs
* **Jméno Příjmení 2** - WebAPI, propojení API a databáze, integrační testy

## 🚀 Spuštění projektu
1. Ujistěte se, že běží **Docker Desktop** 
2. Otevřete solution ve Visual Studiu 
3. Nastavte `UTB.Minute.AppHost` jako Start-up projekt a spusťte jej (F5)
4. V prohlížeči se otevře **.NET Aspire Dashboard**
5. U služby `utb-minute-dbmanager` klikněte na tlačítko pro spuštění příkazu **Reset Database**. Tím se databáze vyčistí a naplní testovacími daty 
6. WebAPI je následně dostupné pod službou `utb-minute-webapi`

## 🗂️ Datový model a DTO
Architektura striktně odděluje databázové entity od objektů, které se posílají ven přes API. Entity nejsou nikdy vraceny přímo.

**Entity (projekt `UTB.Minute.Db`):**
* `MinuteMeal` - Obsahuje vlastnost `IsActive` pro zajištění Soft-delete (jídla se nemažou, pouze deaktivují).
* `MenuItem` - Konkrétní položka v menu pro daný den s počtem porcí.
* `Order` - Samotná objednávka vázaná na menu.
* `OrderStatus` - Enum reprezentující stavy (Preparing, Finished, Canceled, Completed).

**DTO objekty (projekt `UTB.Minute.Contracts`):**
* Pro jídla: `MinuteMealDto`, `MinuteMealRequestDto`, `MinuteMealPatchDescDto`, `MinuteMealPatchPriceDto`
* Pro menu: `MenuItemDto`
* Pro objednávky: `OrderDto`

## 📝 Poznámky k odevzdání (Stav projektu)
* **Stav:** Projekt je plně funkční a splňuje všechny požadavky pro půlsemestrální odevzdání.
* **Souběžnost:** Snížení počtu porcí při objednávce je ošetřeno proti souběžnému přístupu (Concurrency).
* **Testování:** Automatizované integrační testy (`UTB.Minute.WebApi.Tests`) využívají reálnou kontejnerizovanou databázi spravovanou přes .NET Aspire. Testy pokrývají kompletní scénář od vytvoření jídla, přes úpravu, přidání do menu, vytvoření objednávky, změnu stavu až po konečnou deaktivaci a smazání. Testy prochází bez chyb a bez warningů.
