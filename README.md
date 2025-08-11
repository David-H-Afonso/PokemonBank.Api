# Pokémon Bank (Personal)

A local-first **Pokémon bank** to import, store, and browse Pokémon from `.pk*` files across **all generations** (Gen 1 → Gen 9, plus supported spin-offs). Uses **PKHeX.Core** for parsing, **SQLite** for storage, and exposes a clean **REST API**. Designed to be extended with a React/TypeScript UI.

---

## TL;DR

- **Import** one or many `.pk*` files (e.g., `.pk9`) and keep the **original file**.
- **Store** full metadata locally (species, forms, OT/TID/SID, IV/EV, moves, ball, language, origin, Tera Type, etc.).
- **Export** a **Pokémon Showdown** set.
- **Stack:** .NET 9 + ASP.NET Core + EF Core + SQLite + PKHeX.Core + Swagger.
- **Status:** MVP API bootstrapped; parser wired to PKHeX.Core; import & Showdown export endpoints in place.

---

## Clone & run locally

### 1) Prerequisites

- [Git](https://git-scm.com/downloads)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- (Optional) [Visual Studio Code](https://code.visualstudio.com/) or any IDE

### 2) Clone

```bash
git clone https://github.com/username/pokemon-bank.git
cd pokemon-bank/PokemonBank.Api
```

### 3) Restore & build

```bash
dotnet restore
dotnet build
```

### 4) Create the database

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add Initial
dotnet ef database update
```

### 5) Run the API

**Option A — HTTPS (recommended)**

1. Trust the dev cert (one-time):

```bash
dotnet dev-certs https --trust
```

2. Use the `https` launch profile:

```bash
dotnet run --launch-profile https
```

3. Open Swagger:

```
https://localhost:7178/swagger
```

**Option B — HTTP only (quick start)**  
Comment out `app.UseHttpsRedirection();` in `Program.cs` and run:

```bash
dotnet run --launch-profile http
```

Swagger:

```
http://localhost:5111/swagger
```

---

## Project structure

```
pokemon-bank/
├── PokemonBank.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Parsers/
│   ├── Services/
│   ├── Properties/
│   ├── appsettings.json
│   ├── Program.cs
│   ├── PokemonBank.Api.csproj
├── PokemonBank.sln
└── README.md
```

- `PokemonBank.Api/`: Main ASP.NET Core Web API project.
  - `Controllers/`: API endpoints (e.g., Import, Showdown export).
  - `Data/`: EF Core DbContext, migrations, and database models.
  - `Models/`: DTOs and API models.
  - `Parsers/`: Pokémon file parsing logic (PKHeX.Core integration).
  - `Services/`: Business logic, file storage, helpers.
  - `Properties/`: Launch settings and configuration.
  - `appsettings.json`: App configuration.
  - `Program.cs`: Entry point.
- `PokemonBank.Core/`: (Optional) Shared domain logic and models.
- `PokemonBank.sln`: Solution file.
- `README.md`: This documentation.

---

## Tech stack

- **Backend:**

  - .NET 9
  - ASP.NET Core Web API
  - Entity Framework Core
  - SQLite (local storage)
  - PKHeX.Core (Pokémon parsing)
  - Swashbuckle / Swagger UI

- **Other:**
  - Dependency Injection
  - HealthChecks
  - Modular endpoint & service structure
  - Multi-generation compatibility

---

## Goals

### General

Create a **personal Pokémon bank** that:

- Imports Pokémon from `.pk*` files obtained legally.
- Displays detailed information in a user-friendly format.
- Keeps a secure local record of the collection.

### Technical

- Full compatibility with `.pk*` from Gen 1 to Gen 9 + supported spin-offs.
- Local storage of Pokémon data, metadata & original file.
- Advanced parsing with PKHeX.Core.
- Showdown export.
- Modular architecture for future UI integration.
- REST API with Swagger documentation.

---

## Current progress

- Project structure: .NET 9 + ASP.NET Core + EF Core.
- SQLite database with initial migrations.
- Services:
  - `IPkParser` / `PkhexCoreParser` for parsing.
  - `IFileStorage` for file saving.
- Endpoints:
  - `/health` → HealthCheck
  - `/import` (POST) → Import `.pk*`
  - `/pokemon/{id}/showdown` (GET) → Export Showdown set
- Swagger UI for testing & docs.
- Launch profiles for HTTP + HTTPS.

---

## Next steps

- [ ] Add name resolution (species, balls, locations) via catalogs.
- [ ] Support ribbons, marks, contest stats.
- [ ] Return sprites/images in API responses.
- [ ] Implement advanced filtering/search.
- [ ] Build React/TypeScript UI.

---

## License

Personal use. PKHeX.Core & dependencies retain their respective licenses.
