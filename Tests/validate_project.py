"""Короткие проверки целостности исходников (без компиляции .NET)."""
from pathlib import Path
import re
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[1]
project = root / 'Tooded_EFCore'
checks = []

def verify(name, condition):
    checks.append((name, bool(condition)))

expected = [
    'Program.cs', 'Kaustad.cs', 'Toode.cs', 'Kategooria.cs',
    'ToodedContext.cs', 'Kasutaja.cs', 'PasswordHelper.cs',
    'AktiivneKasutaja.cs', 'Ostukorv.cs',
    'Form2.cs', 'Form3.cs', 'ArveForm.cs', 'FormMain.cs', 'LoginForm.cs',
    'Migrations/20261023000100_InitialCreate.cs',
    'Migrations/20261023000200_LisaKasutajad.cs',
    'Migrations/ToodedContextModelSnapshot.cs',
]
for name in expected:
    verify(f'файл {name}', (project/name).is_file())
for name in ['Form2', 'Form3', 'ArveForm', 'FormMain', 'LoginForm']:
    designer = project/f'{name}.Designer.cs'
    form = project/f'{name}.cs'
    verify(f'{name} программный интерфейс', 'Controls.Add' in form.read_text('utf-8'))
    verify(f'{name} конструктор', f'public {name}()' in form.read_text('utf-8'))
    verify(f'{name} namespace', 'namespace Tooded_EFCore;' in form.read_text('utf-8'))

csproj = ET.parse(project/'Tooded_EFCore.csproj')
xml = (project/'Tooded_EFCore.csproj').read_text('utf-8')
verify('SDK-style csproj', csproj.getroot().attrib.get('Sdk') == 'Microsoft.NET.Sdk')
verify('правильный TargetFramework', '<TargetFramework>net8.0-windows</TargetFramework>' in xml)
verify('правильная версия языка', '<LangVersion>12.0</LangVersion>' in xml)
for package in ['Microsoft.EntityFrameworkCore.SqlServer', 'Microsoft.EntityFrameworkCore.Tools',
                'PdfSharpCore', 'BCrypt.Net-Next']:
    verify(f'NuGet {package}', f'Include="{package}"' in xml)

context = (project/'ToodedContext.cs').read_text('utf-8')
for table in ['Toodetabel','Kategooriatabel','Kasutajad']:
    verify(f'таблица {table}', f'"{table}"' in context)

program = (project/'Program.cs').read_text('utf-8')
verify('Migrate()', 'Database.Migrate()' in program)
verify('LoginForm entry', 'Application.Run(new LoginForm())' in program)
verify('демо-аккаунты по методичке', 'admin@admin.ee' in program and 'kasutaja@pood.ee' in program)
verify('BCrypt hashing', 'PasswordHelper.Hash' in program)

store = (project/'Form2.cs').read_text('utf-8')
for handler in ['btnLisa_Click','btnUuenda_Click','btnKustuta_Click','btnValiPilt_Click']:
    verify('Form2 '+handler, len(re.findall(r'\b'+handler+r'\s*\(',store))>=1)
verify('Фото копируются в LocalAppData', 'Kaustad.KopeeriPilt' in store)
verify('Картинка необязательна при редактировании', 'if (valitudPilt != null)' in store)

shop=(project/'Form3.cs').read_text('utf-8')
verify('категории на табах', 'LisaLeht(grupp.Key, grupp.ToList())' in shop and 'GroupBy(t => t.Kategooria.Kategooria_nimetus)' in shop)
verify('визуальные карточки', 'LooTootekaart' in shop and 'PictureBox' in shop)
cart=(project/'Ostukorv.cs').read_text('utf-8')
verify('общая корзина', 'static class Ostukorv' in cart)
verify('проверка остатков в корзине','korvis + kogus > toode.Kogus' in cart)

invoice=(project/'ArveForm.cs').read_text('utf-8')
verify('PDFsharpCore', 'using PdfSharpCore.Pdf;' in invoice)
verify('PDF сохраняется в Arved', 'Kaustad.Arved' in invoice and 'dokument.Save(fail);' in invoice)
verify('SMTP send', 'SendMailAsync' in invoice)
verify('нехардкоженный SMTP пароль', 'Environment.GetEnvironmentVariable("TOODED_SMTP_APP_PASSWORD")' in invoice)
verify('остатки списываются в SQL транзакции', 'Database.BeginTransaction()' in invoice and 'ExecuteSqlInterpolated' in invoice)
verify('закрытие корзины при покупке', 'Ostukorv.Tyhjenda();' in invoice)

login=(project/'LoginForm.cs').read_text('utf-8')
verify('форма логина', 'PasswordHelper.Verify' in login)
verify('закрытие LoginForm после меню', 'menyy.FormClosed' in login)
menu=(project/'FormMain.cs').read_text('utf-8')
verify('управление ролями','btnTooted.Enabled = AktiivneKasutaja.OnAdmin;' in menu)

migration = (project/'Migrations/20261023000100_InitialCreate.cs').read_text('utf-8')
verify('InitialCreate — создание таблиц', 'CreateTable' in migration and 'InsertData' in migration)
verify('LisaKasutajad — создание таблицы', 'CreateTable' in (project/'Migrations/20261023000200_LisaKasutajad.cs').read_text('utf-8'))
verify('опубликованный профиль', (project/'Properties/PublishProfiles/FolderProfile.pubxml').exists())
for f in [project/'Tooded_EFCore.csproj',project/'Properties/PublishProfiles/FolderProfile.pubxml',root/'Installer/ToodedInstaller.wxs']:
    try:
        ET.parse(f)
        verify('XML ' + f.name, True)
    except ET.ParseError:
        verify('XML ' + f.name, False)

failed = [name for name, status in checks if not status]
for name, status in checks:
    print(('PASS' if status else 'FAIL') + ' - ' + name)
print(f'\nИтог: {len(checks)-len(failed)}/{len(checks)} статических проверок пройдено')
if failed:
    raise SystemExit(1)
