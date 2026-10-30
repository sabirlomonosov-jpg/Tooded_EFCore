# Tooded_EFCore – projekti kirjeldus (osad 1–7)

## Uuendus: McDonald'si menüü

Pood on nüüd McDonald'si stiilis. Menüüs on 17 toodet koos piltidega. Toodete hinnad on eurodes ja on näidishinnad.

Programmi esimesel käivitamisel asendatakse vanad näidistooted uue menüüga.

Lisainfo: `MCDONALDS_RU.md`

## Projekti info

See on **uus projekt**, mis on tehtud Windows Forms ja EF Core abil.

Projekt põhineb 30-leheküljelisel PDF-juhendil.

**Oluline:** uut projekti ei tohi panna vana `Tooted` projekti sisse. Vana `tooted.mdf` andmebaasi ei kasutata.

## Vajalikud programmid

- Windows 10 või Windows 11 (64-bit)
- Visual Studio 2022 (17.8 või uuem)
- .NET 8 SDK
- SQL Server Express LocalDB
- NuGet paketid

## Kuidas programmi käivitada?

1. Paki ZIP-fail lahti.
2. Ava Visual Studios `Tooded_EFCore.sln`.
3. Kontrolli, et projekt kasutab .NET 8.
4. Laadi alla NuGet paketid ja kontrolli, kas LocalDB töötab.
5. Vajuta F5.

Programm loob automaatselt andmebaasi `Tooded_DB`.

Projektis on juba kolm migratsiooni:

- `InitialCreate`
- `LisaKasutajad`
- `McDonaldsMenu`

**Oluline:** uusi migratsioone ei ole vaja teha, kui andmebaasi mudelid ei muutu.

## Kasutajakontod

| E-post | Parool | Roll |
|---|---|---|
| admin@admin.ee | admin123 | admin |
| kasutaja@pood.ee | kasutaja123 | kasutaja |

Need on testkontod. Programm loob need ainult siis, kui kasutajate tabel on tühi.

Paroolid salvestatakse andmebaasi BCrypt-räsidena.

### Kasutajate õigused

**Admin** saab:
- Lisada, muuta ja kustutada tooteid.
- Vaadata poodi.
- Kasutada ostukorvi.
- Koostada PDF-arveid.

**Tavaline kasutaja** saab:
- Vaadata tooteid.
- Lisada tooteid ostukorvi.
- Osta tooteid.
- Koostada PDF-arveid.

Tavaline kasutaja ei saa tooteid muuta ega kustutada.

## 1. Andmebaas ja mudelid

Projekt kasutab SQL Server LocalDB andmebaasi.

Peamised failid:

- `Toode.cs`
- `Kategooria.cs`
- `ToodedContext.cs`
- `Kaustad.cs`
- `Program.cs`
- `Migrations`

Andmebaasis on kolm põhitabelit.

**Kategooriatabel**

`Id`, `Kategooria_nimetus`, `Kirjeldus`

**Toodetabel**

`Id`, `Toodenimetus`, `Kogus`, `Hind`, `Pilt`, `KategooriaId`

**Kasutajad**

`Id`, `Nimi`, `Email`, `ParoolHash`, `Roll`

Tabelis `__EFMigrationsHistory` on info tehtud migratsioonide kohta.

Kolm algset kategooriat lisatakse `HasData` abil.

## 2. Tootehaldus – Form2

See vorm on ainult administraatorile.

Administraator saab:
- Lisada uusi tooteid.
- Muuta toodete andmeid.
- Kustutada tooteid.
- Lisada toodetele pilte.
- Valida toote kategooria.

**Kontrollid:**

- Kogus ei saa olla negatiivne.
- Hind ei saa olla negatiivne.
- Hinda saab kirjutada näiteks `2,50` või `2.50`.
- Pilt ei ole kohustuslik.

Kui kasutaja lisab pildi, kopeerib programm selle eraldi kausta. Andmebaasi salvestatakse ainult pildi failinimi.

Kui admin muudab ainult hinda, jäävad vana pilt ja kategooria alles.

## 3. Pood – Form3

Poes on tooted kategooriate järgi.

Igal kategoorial on oma vaheleht.

Tootekaardil on:
- Toote pilt.
- Toote nimi.
- Hind.
- Kogus.
- Nupp „Lisa korvi“.

Kasutaja saab lisada tooteid ostukorvi.

Ostukorv on kogu programmis ühine. See jääb alles ka siis, kui poe aken suletakse.

**Oluline:** ostukorvi ei saa lisada rohkem tooteid, kui laos on.

Ostmisel kontrollib programm kogust veel kord andmebaasist.

## 4. Ostukorv ja PDF-arve – ArveForm

Kasutaja valib tooted ja avab ostukorvi.

Nupp **„Genereeri PDF / Osta“** teeb ostu ja loob PDF-arve.

Programmi töö:

1. Kontrollib toodete koguseid.
2. Vähendab ostetud toodete kogust andmebaasis.
3. Loob PDF-arve.
4. Salvestab arve kausta `Arved`.
5. Tühjendab ostukorvi.

Ost toimub SQL-transaktsioonis.

Kui PDF-arve loomine ebaõnnestub, tühistatakse andmebaasi muudatused.

Pärast ostu saab PDF-arve saata e-postiga.

E-kirja saatmine ei tee uut ostu.

### E-posti seadistamine

Programm kasutab Gmaili.

E-posti aadressi ja rakenduse parooli ei salvestata lähtekoodi.

Windowsis tuleb määrata kaks keskkonnamuutujat:

```powershell
[Environment]::SetEnvironmentVariable('TOODED_SMTP_USER','your-address@gmail.com','User')

[Environment]::SetEnvironmentVariable('TOODED_SMTP_APP_PASSWORD','YOUR_APP_PASSWORD','User')
```

Pärast seda tuleb Visual Studio uuesti avada.

Gmaili jaoks kasutatakse rakenduse parooli, mitte tavalist konto parooli.

**Parooli ei tohi lisada GitHubi.**

## 5. Peamenüü – FormMain

Peamenüü kaudu saab avada teisi vorme.

Vormid avatakse käsuga `ShowDialog()`.

Administraator näeb kõiki vajalikke nuppe.

Tavaline kasutaja ei saa avada tootehalduse vormi.

## 6. Sisselogimine ja rollid

Sisselogimiseks kasutatakse `LoginForm` vormi.

Peamised failid:

- `Kasutaja.cs`
- `PasswordHelper.cs`
- `AktiivneKasutaja.cs`
- `LoginForm.cs`

Programm kontrollib kasutaja e-posti ja parooli.

Pärast sisselogimist avaneb peamenüü.

Kasutaja õigused sõltuvad tema rollist.

Kui peamenüü suletakse, lõpetab programm töö.

## 7. Installeri loomine

Programmi jaoks on ette valmistatud MSI-installeri failid.

Juhend asub failis `Installer/README_INSTALLER_RU.md`.

Projektis on olemas:
- Publish Profile.
- WiX MSI lähtekood.
- PowerShelli skript.

MSI-faili tuleb ehitada ja kontrollida Windowsis.

**Oluline:** MSI-faili lõplikku tööd ei ole veel kontrollitud.

Teises arvutis peab SQL Server Express LocalDB olema eraldi paigaldatud.

## Failide asukohad

Programmi failid salvestatakse kausta:

```text
%LOCALAPPDATA%\ToodedApp\
    Images\
    Arved\
``
