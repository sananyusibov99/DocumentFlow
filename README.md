# DocumentFlow

A WPF document management app for a company: documents and their lifecycle, contacts, calendar, news and Gmail, all in one window. It was a team project built in 2019 at STEP IT Academy. The original repository is [programmistik/DocumentFlow](https://github.com/programmistik/DocumentFlow).

## What the app does

- Documents with types, states, currencies and change history
- Users, departments and positions; login and an admin panel (only active users can sign in)
- Contacts, organisations and companies
- Calendar and schedule synced with Google Calendar
- Reading and sending mail through the Gmail API
- Company news feed
- Reference data and settings, with the interface in Russian and English

## My part

I worked mostly on the interface and the MVVM layer:

- Designs for the login page, main window, schedule page, settings and admin panel
- Page structure and navigation, plus a responsive calendar
- Contacts page and contact list
- Data binding fixes in the calendar and event pages; input validation for e-mail, phone and numeric fields
- Interface localisation: all text moved into Russian and English resource dictionaries
- Custom message boxes, a new logo and icons, and cleaner ViewModels and XAML

## Tech stack

C#, WPF, MVVM (MVVM Light), Entity Framework 6 (Code First, migrations), SQL Server LocalDB, Autofac, MaterialDesignThemes, MahApps.Metro, Google Calendar API, Gmail API, .NET Framework 4.7.2

## How to run

1. Open `DocumentFlow.sln` in Visual Studio and restore NuGet packages.
2. Create the database from the migrations (`Update-Database` in the Package Manager Console).
3. Calendar and mail need your own Google Cloud OAuth client: put its `credentials.json` in `DocumentFlow/Resources/`.

## Status

This is a learning team project from 2019 and is no longer maintained.
