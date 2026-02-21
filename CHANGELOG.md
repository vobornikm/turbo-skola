# ?? Changelog

Všechny významné zmìny v projektu budou dokumentovány v tomto souboru.

Formát vychází z [Keep a Changelog](https://keepachangelog.com/cs/1.0.0/),
a tento projekt dodržuje [Semantic Versioning](https://semver.org/lang/cs/).

---

## [Unreleased]

### Plánováno
- Velká násobilka (11-20)
- Sèítání a odèítání
- Achievementy a odmìny
- Export statistik do PDF
- Dark mode

---

## [0.1.0] - 2025-01-XX

### ? Pøidáno
- **Malá násobilka** - násobení a dìlení 1-10
- **Systém profilù** - vícero dìtí na jeden úèet
- **Emoji avatary** - personalizace profilù s barvami
- **PIN ochrana** - zabezpeèení rodièovského profilu
- **Tréninky**:
  - Èasový režim (1-15 minut)
  - Režim poètu pøíkladù (10-100)
  - Výbìr násobièek (1-10)
  - Násobení + dìlení
- **Statistiky**:
  - Historie sessions
  - Úspìšnost podle násobièek
  - Celkový pøehled pokroku
- **Responzivní design**:
  - Desktop layout
  - Mobilní hamburger menu
  - Touch-friendly komponenty
- **Moderní UI**:
  - Gradient design (#667eea ? #764ba2)
  - Custom checkbox styling
  - Smooth animations
  - Loading spinner
- **Authentication**:
  - Registrace s emailem
  - Login system
  - Session management
  - Password change

### ?? Opravy
- Fix: Unicode checkmark zobrazení ? CSS border trick
- Fix: Responzivní grid pro profily (4?3?2 sloupce)
- Fix: Hamburger menu vpravo (iOS standard)
- Fix: Text pod checkboxem (inline layout)
- Fix: NavMenu dropdown na mobilu (full width)
- Fix: "?? Vyber profil" emoji rendering

### ?? Styling
- Pøidán moderní custom checkbox
- Responsive breakpointy (768px, 480px, 350px)
- Mobilní menu slide-in zprava
- Profile avatar hover efekty
- Gradient pozadí u tréninkù

### ?? Technické
- .NET 10 + Blazor Server
- SQLite + EF Core
- Custom authentication (bez Identity)
- Performance monitoring service
- Session service pro doèasná data

---

## Typy zmìn
- `? Pøidáno` - nové funkce
- `?? Zmìnìno` - zmìny existující funkcionality
- `?? Zastaralé` - funkce k odstranìní
- `??? Odstranìno` - odstranìné funkce
- `?? Opravy` - bugfixy
- `?? Zabezpeèení` - bezpeènostní záplaty
