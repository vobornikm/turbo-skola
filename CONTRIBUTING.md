# ?? Pøispívání do projektu Turbo Škola

Dìkujeme za zájem pøispìt! ??

---

## ?? Jak pøispìt

### 1?? Fork & Clone
```bash
# Fork repozitáø na GitHubu
# Pak:
git clone https://github.com/TVOJE_JMENO/turbo-skola.git
cd turbo-skola
git remote add upstream https://github.com/vobornikm/turbo-skola.git
```

### 2?? Vytvoø branch
```bash
# Pro novou funkci
git checkout -b feature/nazev-funkce

# Pro opravu chyby
git checkout -b bugfix/popis-problemu

# Pro dokumentaci
git checkout -b docs/co-dokumentujes
```

### 3?? Proveï zmìny
- Piš èitelný kód
- Dodržuj existující code style
- Pøidej komentáøe pokud je to potøeba
- Otestuj zmìny lokálnì

### 4?? Commit zmìny
```bash
git add .
git commit -m "? feat: Popis zmìny"
```

### 5?? Push & Pull Request
```bash
git push origin feature/nazev-funkce
```
Pak vytvoø Pull Request na GitHubu.

---

## ?? Commit konvence

Používáme **Conventional Commits** s emoji:

| Emoji | Typ | Použití |
|-------|-----|---------|
| ? | `feat` | Nová funkce |
| ?? | `fix` | Oprava chyby |
| ?? | `docs` | Dokumentace |
| ?? | `style` | CSS/UI zmìny |
| ?? | `refactor` | Refaktoring kódu |
| ? | `perf` | Performance |
| ? | `test` | Testy |
| ?? | `chore` | Build/config |
| ?? | `security` | Bezpeènost |

**Pøíklady:**
```bash
? feat: Pøidána velká násobilka (11-20)
?? fix: Oprava zobrazení statistik na mobilu
?? docs: Aktualizace README s instalaèními kroky
?? style: Responzivní design pro tablety
?? refactor: Refaktoring TrainingService
? perf: Optimalizace naèítání profilù
```

---

## ?? Code Style

### C# / Blazor
```csharp
// ? DOBØE
private async Task SaveSessionAsync()
{
    // Jasný název metody
    // Async suffix
    // Task return type
}

// ? ŠPATNÌ
private void save()
{
    // Nejasný název
    // Není async
}
```

### CSS
```css
/* ? DOBØE - BEM-like konvence */
.profile-card {
    /* Block */
}

.profile-card__avatar {
    /* Element */
}

.profile-card--active {
    /* Modifier */
}
```

### Razor komponenty
```razor
@* ? DOBØE *@
<div class="training-container">
    @if (isLoading)
    {
        <LoadingSpinner />
    }
</div>

@* ? ŠPATNÌ - špatné odsazení *@
<div class="training-container">
@if (isLoading) {
<LoadingSpinner />
}
</div>
```

---

## ?? Testování

Pøed commitem zkontroluj:

```bash
# Build projektu
dotnet build

# Spuštìní aplikace
dotnet run

# Kontrola
# - ? Aplikace se spustí bez chyb
# - ? Nová funkce funguje
# - ? Neporušil jsi existující funkce
# - ? Responzivita na mobilu/tabletu/desktopu
```

---

## ?? Struktura projektu

Kam co patøí:

```
Ucivo/
??? Components/Pages/         # Nové stránky
??? Components/Shared/        # Reusable komponenty
??? Services/                 # Business logika
??? Data/Models/              # DB entity
??? TrainingGenerators/       # Generátory cvièení
??? wwwroot/                  # Statické soubory
```

---

## ?? Pravidla

### ? DÌLEJ
- Piš èitelný kód
- Pøidávej komentáøe pro složité èásti
- Testuj na mobilu i desktopu
- Aktualizuj README pokud je potøeba
- Respektuj existující code style

### ? NEDÌLEJ
- Necommituj `bin/`, `obj/`, `.vs/`
- Necommituj citlivá data (hesla, connection stringy)
- Nemìò `appsettings.json` bez dùvodu
- Neporušuj existující funkce
- Nezapomeò na responzivitu

---

## ?? Komunikace

- **Issues:** Pro bugy a feature requesty
- **Discussions:** Pro otázky a nápady
- **Pull Requests:** Pro code review

---

## ?? Priority

Co prioritnì potøebujeme:

1. ?? **Bugfixy** - vždy vítány
2. ?? **Dokumentace** - README, komentáøe v kódu
3. ? **Nové generátory** - velká násobilka, sèítání...
4. ?? **Mobile UX** - vylepšení pro mobily
5. ? **Performance** - optimalizace

---

## ?? Licence

Pøispíváním souhlasíš s MIT licencí projektu.

---

**Dìkujeme za pøíspìvek! ??**
