# ?? Turbo Škola

> Interaktivní výuková aplikace pro dìti - Malá násobilka s gamifikací a sledováním pokroku

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-blue)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## ?? O projektu

**Turbo Škola** je moderní webová aplikace postavená na **Blazor Server (.NET 10)**, která pomáhá dìtem procvièovat malou násobilku zábavnou a interaktivní formou.

### ? Hlavní funkce

- ?? **Malá násobilka** - násobení a dìlení 1-10
- ?? **Vícero profilù** - každé dítì má vlastní profil s avatarem
- ?? **Sledování pokroku** - detailní statistiky a výkonnostní metriky
- ?? **PIN ochrana** - rodièovský profil chránìný PINem
- ?? **Flexibilní tréninky** - èasové nebo poètové limity
- ?? **Responzivní design** - funguje na desktopu i mobilu
- ?? **Moderní UI** - gradientový design s emoji avatary

---

## ?? Technologie

- **Backend:** .NET 10, Blazor Server
- **Frontend:** Blazor Components, CSS3
- **Databáze:** SQLite (Entity Framework Core)
- **Auth:** Custom authentication system
- **Styling:** Custom CSS (žádný framework)

---

## ?? Instalace

### Pøedpoklady

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQLite support (zabudovaný v EF Core)

### Kroky

1. **Klonování repozitáøe**
   ```bash
   git clone https://github.com/vobornikm/turbo-skola.git
   cd turbo-skola
   ```

2. **Obnovení balíèkù**
   ```bash
   dotnet restore
   ```

3. **Spuštìní migrace databáze**
   ```bash
   cd Ucivo
   dotnet ef database update
   ```

4. **Spuštìní aplikace**
   ```bash
   dotnet run
   ```

5. **Otevøít v prohlížeèi**
   ```
   https://localhost:5001
   ```

---

## ?? Použití

### 1?? Registrace
- Vytvoø si úèet s emailem a heslem
- Systém automaticky vytvoøí rodièovský profil

### 2?? Vytvoøení dìtských profilù
- V sekci **Spravovat profily** pøidej profily pro dìti
- Vyber emoji avatar a barvu
- (Volitelnì) nastav PIN pro ochranu

### 3?? Trénink
- Vyber profil dítìte
- Nastav parametry (násobilka, èas/poèet pøíkladù)
- Zaèni trénovat! ??

### 4?? Statistiky
- Sleduj pokrok v sekci **Statistiky**
- Zobraz historii tréninkových sessions
- Analyzuj úspìšnost podle násobièek

---

## ?? Struktura projektu

```
Ucivo/
??? Components/           # Blazor komponenty
?   ??? Layout/          # MainLayout, NavMenu
?   ??? Pages/           # Stránky (Home, Training, Auth...)
?   ??? Shared/          # Sdílené komponenty (LoadingSpinner, PinInput...)
??? Data/
?   ??? Models/          # EF Core entity modely
?   ??? UcivoDbContext.cs
??? Services/            # Business logika (AuthService, TrainingService...)
??? TrainingGenerators/  # Generátory cvièení (Násobilka, ...)
??? wwwroot/             # Statické soubory (CSS, JS, obrázky)
```

---

## ?? Funkce v detailu

### ?? Trénink násobilky
- **Režimy:** Èas (1-15 min) nebo Poèet pøíkladù (10-100)
- **Násobení:** 1×1 až 10×10
- **Dìlení:** Zpìtné pøíklady z násobení
- **Live feedback:** Okamžitá zpìtná vazba
- **Pøeskoèení:** Možnost pøeskoèit tìžké pøíklady

### ?? Profily
- **Avatary:** 10+ emoji variant (??, ??, ??, ??, ?...)
- **Barvy:** Personalizace barevného schématu
- **PIN ochrana:** 4-místný PIN pro citlivé profily
- **Rodièovský profil:** Správa nastavení a profilù

### ?? Statistiky
- Celkový poèet sessions
- Prùmìrná úspìšnost
- Nejrychlejší èas
- Historie tréninkù s detaily
- Filtrace podle násobièek

---

## ??? Vývoj

### Spuštìní v development módu
```bash
dotnet watch run
```

### Build pro produkci
```bash
dotnet publish -c Release
```

### Migrace databáze
```bash
# Vytvoøení nové migrace
dotnet ef migrations add NazevMigrace

# Aplikace migrace
dotnet ef database update
```

---

## ?? Pøispívání

1. **Fork** repozitáøe
2. Vytvoø **feature branch** (`git checkout -b feature/nova-funkce`)
3. **Commit** zmìny (`git commit -m '? Pøidána nová funkce'`)
4. **Push** do branch (`git push origin feature/nova-funkce`)
5. Otevøi **Pull Request**

### Commit konvence
```
? feat: Nová funkce
?? fix: Oprava chyby
?? docs: Dokumentace
?? style: Styling/CSS
?? refactor: Refaktoring
? perf: Performance
? test: Testy
?? chore: Build/konfigurace
```

---

## ?? Roadmap

- [ ] **Velká násobilka** (11-20)
- [ ] **Sèítání/Odèítání** generátory
- [ ] **Odmìny a achievementy** ??
- [ ] **Multi-tenant** podpora pro školy
- [ ] **Export výsledkù** do PDF
- [ ] **Dark mode** ??
- [ ] **PWA support** (offline funkce)
- [ ] **Hlasová výslovnost** pøíkladù

---

## ?? Licence

Tento projekt je licencován pod **MIT License** - viz [LICENSE](LICENSE) soubor.

---

## ????? Autor

**Miloš Voborník**
- GitHub: [@vobornikm](https://github.com/vobornikm)

---

## ?? Podìkování

- **Blazor tým** za skvìlý framework
- **Entity Framework Core** za jednoduchý pøístup k datùm
- **Turbík** ?? - maskot projektu

---

<p align="center">Vytvoøeno s ?? pro èeské dìti</p>
