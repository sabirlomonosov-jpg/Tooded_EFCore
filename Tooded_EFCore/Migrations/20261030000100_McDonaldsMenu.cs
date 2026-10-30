using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tooded_EFCore.Migrations;

[DbContext(typeof(ToodedContext))]
[Migration("20261030000100_McDonaldsMenu")]
public class McDonaldsMenu : Migration
{
    // Data-only migration. Existing demo rows retain their IDs and stock.
    // Other products and all user accounts are left intact. Runs once via Migrate().
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'Burgerid');

IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Kana')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'Kana');

IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'Lisandid');

IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'Joogid');

IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'McCafé')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'McCafé');

IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'Magustoidud');

IF NOT EXISTS (SELECT 1 FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Happy Meal')
    INSERT INTO [Kategooriatabel] ([Kategooria_nimetus]) VALUES (N'Happy Meal');

UPDATE [Toodetabel] SET [Toodenimetus] = N'Big Mac', [Hind] = 5.50,
    [Pilt] = N'mcd-big-mac.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid')
WHERE [Toodenimetus] = N'Tume viinamari, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Big Mac');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Tume viinamari, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Big Mac')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Big Mac', 5.50, 25, N'mcd-big-mac.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Juustuburger', [Hind] = 2.20,
    [Pilt] = N'mcd-cheeseburger.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid')
WHERE [Toodenimetus] = N'Mandariin, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Juustuburger');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Mandariin, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Juustuburger')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Juustuburger', 2.20, 25, N'mcd-cheeseburger.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Double Cheeseburger', [Hind] = 3.80,
    [Pilt] = N'mcd-double-cheeseburger.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid')
WHERE [Toodenimetus] = N'Kirss-ploomtomat, 250 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Double Cheeseburger');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Kirss-ploomtomat, 250 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Double Cheeseburger')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Double Cheeseburger', 3.80, 25, N'mcd-double-cheeseburger.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'McChicken', [Hind] = 5.20,
    [Pilt] = N'mcd-mcchicken.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid')
WHERE [Toodenimetus] = N'Pikk kurk, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'McChicken');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Pikk kurk, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'McChicken')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'McChicken', 5.20, 25, N'mcd-mcchicken.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Filet-O-Fish', [Hind] = 4.90,
    [Pilt] = N'mcd-filet-o-fish.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid')
WHERE [Toodenimetus] = N'Kartul lahtine, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Filet-O-Fish');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Kartul lahtine, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Filet-O-Fish')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Filet-O-Fish', 4.90, 25, N'mcd-filet-o-fish.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Burgerid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Chicken McNuggets 9 tk', [Hind] = 5.90,
    [Pilt] = N'mcd-nuggets.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Kana')
WHERE [Toodenimetus] = N'Hurmaa, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Chicken McNuggets 9 tk');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Hurmaa, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Chicken McNuggets 9 tk')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Chicken McNuggets 9 tk', 5.90, 25, N'mcd-nuggets.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Kana';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Suured friikartulid', [Hind] = 3.20,
    [Pilt] = N'mcd-fries-large.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid')
WHERE [Toodenimetus] = N'Või Tere, 82%, 200 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Suured friikartulid');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Või Tere, 82%, 200 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Suured friikartulid')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Suured friikartulid', 3.20, 25, N'mcd-fries-large.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Väikesed friikartulid', [Hind] = 2.20,
    [Pilt] = N'mcd-fries-small.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid')
WHERE [Toodenimetus] = N'Grillbroiler Tallegg, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Väikesed friikartulid');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Grillbroiler Tallegg, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Väikesed friikartulid')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Väikesed friikartulid', 2.20, 25, N'mcd-fries-small.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Coca-Cola 0,4 l', [Hind] = 2.60,
    [Pilt] = N'mcd-cola.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid')
WHERE [Toodenimetus] = N'Seaabaliha kamarata, kondita, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Coca-Cola 0,4 l');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Seaabaliha kamarata, kondita, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Coca-Cola 0,4 l')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Coca-Cola 0,4 l', 2.60, 25, N'mcd-cola.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Fanta 0,4 l', [Hind] = 2.60,
    [Pilt] = N'mcd-fanta.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid')
WHERE [Toodenimetus] = N'Kodune ahjupraad Rakvere, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Fanta 0,4 l');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Kodune ahjupraad Rakvere, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Fanta 0,4 l')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Fanta 0,4 l', 2.60, 25, N'mcd-fanta.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'McCafé must kohv', [Hind] = 2.00,
    [Pilt] = N'mcd-coffee.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'McCafé')
WHERE [Toodenimetus] = N'Jahutatud Norra lõhefilee, 1 kg'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'McCafé must kohv');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Jahutatud Norra lõhefilee, 1 kg';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'McCafé must kohv')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'McCafé must kohv', 2.00, 25, N'mcd-coffee.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'McCafé';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Vesi 0,5 l', [Hind] = 2.00,
    [Pilt] = N'mcd-water.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid')
WHERE [Toodenimetus] = N'Kreeka jogurti kook Halloween, Eesti Pagar, 350 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Vesi 0,5 l');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Kreeka jogurti kook Halloween, Eesti Pagar, 350 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Vesi 0,5 l')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Vesi 0,5 l', 2.00, 25, N'mcd-water.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Joogid';

UPDATE [Toodetabel] SET [Toodenimetus] = N'McFlurry Oreo', [Hind] = 3.50,
    [Pilt] = N'mcd-mcflurry.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud')
WHERE [Toodenimetus] = N'Jäätis Snickers, 48 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'McFlurry Oreo');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Jäätis Snickers, 48 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'McFlurry Oreo')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'McFlurry Oreo', 3.50, 25, N'mcd-mcflurry.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Šokolaadi Sundae', [Hind] = 2.50,
    [Pilt] = N'mcd-sundae.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud')
WHERE [Toodenimetus] = N'Hommikusöögibatoon Nestle, 6 x 25 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Šokolaadi Sundae');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Hommikusöögibatoon Nestle, 6 x 25 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Šokolaadi Sundae')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Šokolaadi Sundae', 2.50, 25, N'mcd-sundae.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Õunapirukas', [Hind] = 1.80,
    [Pilt] = N'mcd-apple-pie.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud')
WHERE [Toodenimetus] = N'Šokolaad Kalev, 270 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Õunapirukas');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Šokolaad Kalev, 270 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Õunapirukas')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Õunapirukas', 1.80, 25, N'mcd-apple-pie.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Magustoidud';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Happy Meal hamburgeriga', [Hind] = 5.50,
    [Pilt] = N'mcd-happy-meal.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Happy Meal')
WHERE [Toodenimetus] = N'Karastusjook Coca-Cola, 2 x 1,5 l'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Happy Meal hamburgeriga');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Karastusjook Coca-Cola, 2 x 1,5 l';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Happy Meal hamburgeriga')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Happy Meal hamburgeriga', 5.50, 25, N'mcd-happy-meal.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Happy Meal';

UPDATE [Toodetabel] SET [Toodenimetus] = N'Keskmised friikartulid', [Hind] = 2.80,
    [Pilt] = N'mcd-fries-medium.png',
    [KategooriaId] = (SELECT MIN([Id]) FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid')
WHERE [Toodenimetus] = N'Jahvatatud kohv Löfbergs, 500 g'
AND NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Keskmised friikartulid');
DELETE FROM [Toodetabel] WHERE [Toodenimetus] = N'Jahvatatud kohv Löfbergs, 500 g';
IF NOT EXISTS (SELECT 1 FROM [Toodetabel] WHERE [Toodenimetus] = N'Keskmised friikartulid')
    INSERT INTO [Toodetabel] ([Toodenimetus], [Hind], [Kogus], [Pilt], [KategooriaId])
    SELECT N'Keskmised friikartulid', 2.80, 25, N'mcd-fries-medium.png', MIN([Id])
    FROM [Kategooriatabel] WHERE [Kategooria_nimetus] = N'Lisandid';
""");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder) =>
        MigrationModel.Build(modelBuilder, users: true);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("Menu replacement cannot restore previous prices and images. Restore a database backup to roll back.");
}
