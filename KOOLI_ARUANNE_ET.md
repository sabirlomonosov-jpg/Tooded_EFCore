# Toodete pood – projekti kokkuvõte

## Mis on tehtud

Lõin uue Windows Forms (.NET 8) projekti. Kasutasin Entity Framework Core 8 ja SQL Server LocalDB andmebaasi. Rakenduses on toodete haldus, pood, ostukorv, arved ja sisselogimine.

## Andmebaas

Andmebaasi nimi on Tooded_DB. Tabelid on Kategooriatabel, Toodetabel ja Kasutajad. Tabelid luuakse migratsioonidega.

## Vormid

Form2 lubab tooteid lisada, muuta ja kustutada. Form3 näitab tooteid kategooriate kaupa. ArveForm loob PDF-arve ja saab selle e-postiga saata. FormMain on peamenüü. LoginForm kontrollib kasutaja parooli.

## Kasutajad

Admin saab kõiki vorme kasutada. Tavakasutaja saab avada poe ja arvevormi, aga ei saa muuta tooteid. Paroolid on andmebaasis räsidena.

## Lisafunktsioonid

Ostmisel kontrollitakse kauba kogust ja vähendatakse laoseisu. Pilte hoitakse eraldi kaustas. PDF-arved salvestatakse kausta Arved.

## Testimine

Lähtekoodi ja failide struktuuri on staatiliselt kontrollitud. Windowsi rakenduse käivitamine, LocalDB, e-post ja MSI vajavad veel praktilist kontrolli Visual Studios. Töö lõplikuks esitamiseks tuleb need testid teha.
