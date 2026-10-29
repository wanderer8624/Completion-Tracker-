# GameVault — Game Completion Tracker

A cinematic, tactical-style ASP.NET Core MVC project for tracking a personal video-game backlog. The visual language is inspired by stealth-action mission interfaces: charcoal surfaces, olive tones, signal-lime accents, mono telemetry labels, and bold condensed headlines. It does not use Metal Gear Solid logos or official franchise artwork.

## Features
- Mission archive dashboard with live stats: titles, playing, completed, hours played, average rating.
- Search by title and filter by status and genre.
- Register, sign in, and sign out with salted PBKDF2 password hashes.
- Add, edit, and delete personal game records.
- Track genre, platform, status, hours, rating, notes, and cover image URL.
- Eight sample titles to demonstrate the UI before creating an account.
- Responsive layout and image fallback when remote cover art is unavailable.
- Persistent local JSON storage in `App_Data/vault.json`; no database server or third-party NuGet packages required.

## Requirements
- .NET 10 SDK (or change the target framework to a supported SDK installed on your machine).
- Internet connection for Google Fonts and the sample Steam cover images; the app can still run without them.

## Run locally
Open this folder in VS Code, then run in its terminal:

```bash
dotnet restore
dotnet build
dotnet run
```

Open the localhost URL printed by the terminal. Create an account to add, edit, and delete your own records. Sample records are shared read-only demo entries; each account's own records are private to that account within the local app.

## Data and security notes
- User/game data is stored in `App_Data/vault.json` on the machine running the app. Back up this file if you want to preserve your demo data.
- This is an educational project, not a production multi-user service. The JSON store is not designed for concurrent multi-process writes, and session-backed authentication is intentionally minimal.
- Do not deploy with real user data without a proper database, production-grade authentication, CSRF review, logging, backups, and deployment configuration.
- Sample cover art is loaded from Steam's public CDN and is subject to availability and the rights of their respective owners. The visual identity and logo in this project are original.
