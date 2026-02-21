# ?? Screenshoty Turbo Škola

Tato sloka obsahuje screenshoty aplikace pro dokumentaci.

---

## ?? Jak vytvoøit screenshoty

### 1?? Spus aplikaci
```bash
cd Ucivo
dotnet run
```

### 2?? Otevøi v prohlíeèi
```
https://localhost:5001
```

### 3?? Udìlej screenshot

**Windows:**
- `Win + Shift + S` - Snipping Tool
- Vyber oblast
- Ulo do schránky
- Vlo do Paint / Snip & Sketch
- Ulo jako PNG

**Mac:**
- `Cmd + Shift + 4` - Screenshot oblasti
- Automaticky se uloí na Desktop

**Linux:**
- `Print Screen` nebo Flameshot

---

## ?? Potøebné screenshoty

### ? Homepage (`homepage.png`)
- Úvodní obrazovka s logem Turbíka
- Tlaèítko "Zaèít trénovat"

### ? Vıbìr profilu (`profile-selection.png`)
- Grid s profily
- Emoji avatary
- Tlaèítko "Spravovat profily"

### ? Nastavení tréninku (`training-setup.png`)
- Vıbìr násobièek (1-10)
- Èasovı / Poètovı reim
- Násobení / Dìlení checkboxy

### ? Tréninkovı session (`training-session.png`)
- Pøíklad (napø. "3 × 7 = ?")
- Input pole
- Tlaèítka (Potvrdit, Ukonèit, Pøeskoèit)

### ? Vısledky (`results.png`)
- "? Vıbornì! ?"
- Statistiky (doba, poèet pøíkladù, úspìšnost)
- Tlaèítka (Zpìt na domù, Další trénink)

### ? Statistiky (`statistics.png`)
- Graf nebo tabulka vısledkù
- Historie tréninkù
- Prùmìrná úspìšnost

### ? Správa profilù (`profile-management.png`)
- Seznam profilù
- Editaèní formuláø
- Emoji a barevnı vıbìr

---

## ?? Tipy pro kvalitní screenshoty

- ? **Rozlišení:** Minimálnì 1920×1080 (Full HD)
- ? **Formát:** PNG (zachovává kvalitu)
- ? **Obsah:** Ukázková data (ne prázdná obrazovka)
- ? **Soukromí:** Schovat citlivé informace (emaily)
- ? **Konzistence:** Všechny ze stejné velikosti okna

---

## ?? Doporuèené rozmìry

| Screenshot | Minimální rozlišení |
|------------|---------------------|
| Desktop | 1920×1080 |
| Tablet | 1024×768 |
| Mobile | 375×667 (iPhone SE) |

---

## ?? Aktualizace screenshotù

Kdy udìláš zmìny v UI:
```bash
# 1. Udìlej nové screenshoty
# 2. Pøepiš staré soubory
# 3. Commit
git add docs/images/*.png
git commit -m "?? docs: Aktualizace screenshotù"
git push
```

---

## ?? Název souborù

Pouívej konzistentní pojmenování:
```
homepage.png
profile-selection.png
training-setup.png
training-session.png
results.png
statistics.png
profile-management.png
mobile-training.png
tablet-view.png
```
