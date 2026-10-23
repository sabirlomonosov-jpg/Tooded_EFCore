# Tooded_EFCore — полное задание (части 1–7)

Источник: `WinForms (.NET 8) + EF Core 8 – Toodete pood (parandatud juhend) 5.pdf`, 30 страниц.

Это **НОВЫЙ** проект. Не вставлять эти файлы в старый `Tooted` (.NET Framework 4.7.2)! Старую `tooted.mdf` приложение не использует.

## Требования

- Windows 10/11 x64.
- Visual Studio 2022 версии 17.8+ с рабочей нагрузкой **.NET desktop development**.
- .NET 8 SDK (для разработки); NuGet-доступ для восстановления пакетов.
- **SQL Server Express LocalDB** `(localdb)\MSSQLLocalDB` (как в основном варианте руководства).

## Как запустить за 5 шагов

1. Распакуйте весь архив, включая папку `Tooded_EFCore`, **не смешивая** со старым проектом.
2. Откройте **`Tooded_EFCore.sln`** в Visual Studio 2022. В Project → Properties проверьте `Target Framework: .NET 8.0`.
3. Solution → Restore NuGet Packages. Если пакеты скачаны, можно проверить Build → Rebuild Solution.
4. Убедитесь, что SQL Server Express LocalDB установлен и `(localdb)\MSSQLLocalDB` запускается.
5. Нажмите F5. `db.Database.Migrate()` автоматически создаёт базу `Tooded_DB` и применяет **две уже включённые миграции** (`InitialCreate`, `LisaKasutajad`).

**Не запускайте** `Add-Migration InitialCreate` поверх включённых миграций: они **уже находятся в проекте**. Для проверки доступности БД можно выполнить `Get-Migration`; новые миграции добавлять только при изменении моделей.

Если Visual Studio показывает `C# 7.3` или `file-scoped namespace`, значит открыт старый `.csproj` или установлен слишком старый SDK. В новом `.csproj` используется `net8.0-windows`, C# 12 и Windows Forms.

## Учётные записи по PDF (демонстрационные)

| E-post | Parool | Roll |
|---|---|---|
| admin@admin.ee | admin123 | admin |
| kasutaja@pood.ee | kasutaja123 | kasutaja |

Создаются **только при пустой таблице** `Kasutajad`, пароль в БД хранится в виде BCrypt-хеша. Не используйте демонстрационные пароли для настоящих аккаунтов.

## Роли

- `admin`: Tootehaldus (Form2), Pood (Form3), Ostukorv ja arve (ArveForm).
- `kasutaja`: Pood (Form3), Ostukorv ja arve (ArveForm). Кнопка Tootehaldus заблокирована; Form2 также проверяет роль самостоятельно.

## Части задания

### 1. Andmebaas, mudelid

`Toode.cs`, `Kategooria.cs`, `ToodedContext.cs`, `Kaustad.cs`, `Program.cs`, папка `Migrations`.

Таблицы: `Kategooriatabel` (Id, Kategooria_nimetus, Kirjeldus), `Toodetabel` (Id, Toodenimetus, Kogus, Hind, Pilt, KategooriaId), `Kasutajad` (Id, Nimi, Email, ParoolHash, Roll). В `__EFMigrationsHistory` хранятся применённые миграции. Три категории создаются через `HasData`.

### 2. Tootehaldus (Form2)

Admin добавляет, меняет, удаляет товары. `Kogus` — неотрицательное целое, `Hind` — десятичное, неотрицательное; разрешены `2,50` и `2.50`. Изображение необязательно. При выборе новой картинки она копируется в отдельную папку, в БД записывается только имя файла. При изменении только цены старое изображение и категория остаются прежними.

### 3. Pood (Form3) + Ostukorv

По одной вкладке на категорию; карточки с фото, ценой, количеством и кнопкой «Lisa korvi». Корзина общая для всего приложения и переживает закрытие Form3. Нельзя набрать в корзину товара больше, чем числится в карточке. На этапе проведения покупки остатки проверяются ещё раз в SQL.

### 4. PDF + e-post (ArveForm)

Выберите товары, откройте «Ostukorv ja arve», нажмите «Genereeri PDF / Osta». PDF сохраняется в `Arved`; продажа уменьшает складской остаток **в SQL-транзакции**. Если PDF создать не удалось, SQL откатывается. После успешной продажи корзина очищается, а последний PDF можно послать по e-mail. PDF не создаёт повторную продажу при отправке письма.

Адрес отправителя и Gmail app password **не хранятся в исходниках**. На Windows установите две переменные среды пользователя и перезапустите Visual Studio:

```powershell
[Environment]::SetEnvironmentVariable('TOODED_SMTP_USER','your-address@gmail.com','User')
[Environment]::SetEnvironmentVariable('TOODED_SMTP_APP_PASSWORD','YOUR_APP_PASSWORD','User')
```

Замените значения своими. **Не загружайте app password в GitHub и не публикуйте скриншоты с паролем.** Для Gmail нужен app password, обычный пароль аккаунта не используется. Отправка может зависеть от настроек Gmail.

### 5. Peamenüü

`FormMain.cs` открывает остальные формы через `ShowDialog()`.

### 6. Login ja rollid

`Kasutaja.cs`, `PasswordHelper.cs`, `AktiivneKasutaja.cs`, `LoginForm.cs`. Приложение начинается с LoginForm; при закрытии FormMain приложение завершается.

### 7. Installer (.msi)

См. `Installer/README_INSTALLER_RU.md`. Включён **готовый self-contained Publish Profile** и **исходник WiX MSI** с PowerShell-скриптом. Сборка/проверка конечного `.msi` возможна только на Windows с нужными инструментами и **в данной среде не выполнена**. Оригинальный PDF использует Visual Studio Installer Projects 2022; шаги по нему в README установщика. На ПК получателя необходимо установить SQL Server Express LocalDB отдельно.

## Где файлы приложения

```
%LOCALAPPDATA%\ToodedApp\
  Images\  (изображения товаров)
  Arved\   (PDF-счета)
```

Данные товаров и пользователей — не в этих папках, а в базе SQL Server LocalDB `Tooded_DB`.

## Проверки и ограничения

- Здесь выполнены проверки состава файлов, согласованности имён, XML и синтаксического баланса, наличие моделей, страниц задания и ключевых сценариев.
- **Не выполнялась реальная компиляция и не запускались WinForms, LocalDB, PDFsharp, Gmail или MSI в Windows**. Эти пункты требуют проверки в Visual Studio. См. `TEST_PLAN_RU.md`.
- При наличии ошибок Visual Studio используйте **Build → Rebuild Solution** и изучайте сообщения Error List; не меняйте целевую платформу обратно на .NET Framework.

## Примечание по изображениям

Изображения товаров хранятся с уникальными именами. Например, исходное `leib.png` копируется как `6c7e...png`, в поле `Pilt` хранится это новое имя. Такой подход предотвращает конфликты одинаковых имён.

## Улучшения относительно PDF

- Проверка совокупного количества товара в корзине (в PDF контролируется только один клик).
- Реальная SQL-транзакция при покупке с проверкой `Kogus >= N`.
- Поддержка нескольких страниц PDF.
- Не хранится Gmail app password в исходном коде.
- Понятные ошибки при отсутствии LocalDB/файла картинки.

## Известные ограничения

- Это учебное приложение, а не коммерческая касса (нет НДС/налоговой интеграции и реальной платёжной системы).
- Продажа учитывается уменьшением остатка, но не ведётся отдельный журнал продаж — в новом PDF такого требования нет.
- Имя и адрес администратора демонстрационные; смена пароля через интерфейс не реализована (в PDF её тоже нет).
