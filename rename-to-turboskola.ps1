# Skript pro p¯ejmenov·nÌ projektu Ucivo na TurboSkola
# PÿED SPUäTÃNÕM: Zav¯i Visual Studio a vöechny editory!

Write-Host "=== P¯ejmenov·nÌ projektu Ucivo -> TurboSkola ===" -ForegroundColor Green

# 1. P¯ejmenovat namespace ve vöech .cs souborech
Write-Host "`n1. Aktualizuji namespace v .cs souborech..." -ForegroundColor Yellow
Get-ChildItem -Path "Ucivo" -Filter "*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'namespace Ucivo', 'namespace TurboSkola'
    $content = $content -replace 'using Ucivo\.', 'using TurboSkola.'
    Set-Content $_.FullName -Value $content -NoNewline
    Write-Host "  ? $($_.Name)" -ForegroundColor Gray
}

# 2. P¯ejmenovat using statements v .razor souborech  
Write-Host "`n2. Aktualizuji using v .razor souborech..." -ForegroundColor Yellow
Get-ChildItem -Path "Ucivo" -Filter "*.razor" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match '@using Ucivo\.' -or $content -match '@inject.*Ucivo') {
        $content = $content -replace '@using Ucivo\.', '@using TurboSkola.'
        $content = $content -replace 'Ucivo\.Data', 'TurboSkola.Data'
        $content = $content -replace 'Ucivo\.Services', 'TurboSkola.Services'
        $content = $content -replace 'Ucivo\.Utilities', 'TurboSkola.Utilities'
        Set-Content $_.FullName -Value $content -NoNewline
        Write-Host "  ? $($_.Name)" -ForegroundColor Gray
    }
}

# 3. Aktualizovat connection string (Database=Ucivo -> Database=TurboSkola)
Write-Host "`n3. Aktualizuji connection string..." -ForegroundColor Yellow
$programCs = Get-Content "Ucivo\Program.cs" -Raw
$programCs = $programCs -replace 'Database=Ucivo', 'Database=TurboSkola'
$programCs = $programCs -replace 'UcivoDbContext', 'TurboSkolaDbContext'
Set-Content "Ucivo\Program.cs" -Value $programCs -NoNewline
Write-Host "  ? Program.cs" -ForegroundColor Gray

# 4. P¯ejmenovat UcivoDbContext -> TurboSkolaDbContext
Write-Host "`n4. P¯ejmenov·v·m DbContext..." -ForegroundColor Yellow
if (Test-Path "Ucivo\Data\UcivoDbContext.cs") {
    $content = Get-Content "Ucivo\Data\UcivoDbContext.cs" -Raw
    $content = $content -replace 'class UcivoDbContext', 'class TurboSkolaDbContext'
    $content = $content -replace 'public UcivoDbContext', 'public TurboSkolaDbContext'
    Set-Content "Ucivo\Data\UcivoDbContext.cs" -Value $content -NoNewline
    Rename-Item -Path "Ucivo\Data\UcivoDbContext.cs" -NewName "TurboSkolaDbContext.cs"
    Write-Host "  ? UcivoDbContext.cs -> TurboSkolaDbContext.cs" -ForegroundColor Gray
}

# 5. P¯ejmenovat sloûku projektu
Write-Host "`n5. P¯ejmenov·v·m sloûku projektu..." -ForegroundColor Yellow
if (Test-Path "Ucivo") {
    Rename-Item -Path "Ucivo" -NewName "TurboSkola"
    Write-Host "  ? Ucivo -> TurboSkola" -ForegroundColor Gray
}

# 6. Aktualizovat .slnx
Write-Host "`n6. Aktualizuji solution..." -ForegroundColor Yellow
if (Test-Path "TurboSkola.slnx") {
    $slnContent = Get-Content "TurboSkola.slnx" -Raw
    $slnContent = $slnContent -replace 'Ucivo\\', 'TurboSkola\'
    $slnContent = $slnContent -replace 'Ucivo\.csproj', 'TurboSkola.csproj'
    Set-Content "TurboSkola.slnx" -Value $slnContent -NoNewline
    Write-Host "  ? TurboSkola.slnx" -ForegroundColor Gray
}

Write-Host "`n=== HOTOVO! ===" -ForegroundColor Green
Write-Host "`nDalöÌ kroky:" -ForegroundColor Cyan
Write-Host "1. Otev¯i TurboSkola.slnx ve Visual Studiu"
Write-Host "2. Vytvo¯ novou datab·zi: dotnet ef database update"
Write-Host "3. Spusù projekt a otestuj!"
