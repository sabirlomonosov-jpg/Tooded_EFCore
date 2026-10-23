# Часть 7 — MSI-установщик

В исходном PDF (страницы 27–29) рекомендуется Visual Studio Installer Projects 2022. В архив добавлены **два пути**:

## Путь A: строго по PDF — Visual Studio Installer Projects 2022

1. Visual Studio 2022 → Extensions → Manage Extensions → Microsoft Visual Studio Installer Projects 2022 → Install, перезапустить VS.
2. Собрать приложение `Release` и проверить, что запускается.
3. Правая кнопка `Tooded_EFCore` → Publish → Folder. Готовый профиль `Properties/PublishProfiles/FolderProfile.pubxml` уже включён.
4. Конфигурация Release, win-x64, Self-contained (подключение .NET Runtime отдельно не нужно).
5. Solution → Add → New Project → Setup Project → имя `ToodedInstaller`.
6. Свойства проекта установщика: ProductName = `Tooded App`, Manufacturer = `Tooded Kooliprojekt`.
7. File System → Application Folder → Add → Project Output → `Publish Items` проекта `Tooded_EFCore`.
8. В свойствах Publish Items выбрать PublishProfilePath: `FolderProfile.pubxml`.
9. Создать ярлыки для `Publish Items` в User's Programs Menu и User's Desktop.
10. Build в конфигурации Release, проверить `ToodedInstaller.msi` на другом Windows ПК.

## Путь B: готовый код установщика WiX 3.11

Если требуется автоматическое создание `.msi` без ручной настройки Setup Project:

1. На Windows установить **WiX Toolset 3.11** и .NET 8 SDK.
2. Запустить PowerShell из каталога `Installer`:

```powershell
powershell -ExecutionPolicy Bypass -File .\Build-MSI.ps1
```

Скрипт публикует self-contained `win-x64` и на его основе собирает `Installer/Release/ToodedInstaller.msi` с ярлыками.

> Скрипт и XML предоставлены как **исходники установщика**, но в этой среде `.msi` не собран и не проверен запуском.

## Критически важный компонент: LocalDB

Приложение следует основному варианту **A** из PDF и использует сервер `(localdb)\MSSQLLocalDB`, базу `Tooded_DB`. На компьютере получателя **сначала установите SQL Server Express LocalDB**. MSI самостоятельно не включает LocalDB. Если она не установлена, стартовая миграция выдаст понятную ошибку.

Вариант SQLite в исходном PDF — **альтернативный проект со своими миграциями**. Нельзя просто заменить `UseSqlServer` на `UseSqlite` при уже созданных SQL Server миграциях. Не используйте текущие миграции для SQLite.

## Проверка установщика

Проверить установку на Windows x64 без Visual Studio, запуск ярлыка, первое создание базы, вход, работу картинок, PDF, отправку e-mail, удаление через Settings → Apps. Если на целевом ПК нет LocalDB, сначала поставить LocalDB.
