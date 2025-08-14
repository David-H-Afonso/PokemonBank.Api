# Pokémon Bank (Personal)

A local-first **Pokémon bank** to import, store, compare, and browse Pokémon from `.pk*` files across **all generations** (Gen 1 → Gen 9, plus supported spin-offs). Uses **PKHeX.Core** for parsing and name resolution, **SQLite** for storage, and exposes a clean **REST API**. Designed to be extended with a React/TypeScript UI.

---

## TL;DR

- **Import** one or many `.pk*` files (e.g., `.pk9`) and keep the **original file** (unmodified, for full fidelity).
- **Store** full metadata locally (species, forms, OT/TID/SID, IV/EV, moves, ball, language, origin, Tera Type, ribbons, marks, etc.).
- **Compare** any two Pokémon and see all differences (e.g., after trading or editing).
- **Export** a **Pokémon Showdown** set (with names from PKHeX.Core).
- **Stack:** .NET 9 + ASP.NET Core + EF Core + SQLite + PKHeX.Core + Swagger.
- **Status:** Full PK9 support, PKHeX.Core GameStrings migration, original file preservation, Pokémon comparison endpoint, and clean codebase (no custom enums).

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
PokemonBank.Api/
├── Contracts/           # DTOs
├── Domain/              # Entities (PokemonEntity, StatsEntity, etc.)
├── Endpoints/           # Minimal API endpoints (Import, Health, Pokemon, Compare)
├── Extensions/          # WebApplication extensions
├── Infrastructure/      # DbContext, services, PKHeX integration, helpers
│   ├── AppDbContext.cs
│   ├── Services/
│   │   ├── FileStorageService.cs
│   │   ├── PkhexCoreParser.cs
│   │   ├── PkHexStringService.cs
│   │   └── PokemonComparisonService.cs
│   └── Mappings/
├── Migrations/          # EF Core migrations
├── Properties/          # Launch settings
├── ReferenceData/       # (Removed) Old enums/data, now replaced by PKHeX.Core
├── appsettings.json
├── Program.cs
├── PokemonBank.Api.csproj
├── README.md
```

**Data Storage Location:**
The application uses two separate directories for optimal security and usability:

**Private Data (Hidden from user):**

- **Database:** `%LocalAppData%\Pokebank\storage\pokemonbank.db`

**Public Data (User accessible):**

- **Pokemon Files:** `%UserProfile%\Documents\Pokebank\storage\`

**📁 Automatic File Detection:**
The application automatically monitors the Documents folder and:

- Scans for new Pokemon files on startup
- Supports drag-and-drop workflow (just copy files to Documents/Pokebank/storage)
- Automatically imports new files and skips duplicates
- Works with subdirectories

**Supported file formats:**

- `.pk1`, `.pk2`, `.pk3`, `.pk4`, `.pk5`, `.pk6`, `.pk7`, `.pk8`, `.pk9`
- `.pb7`, `.pb8` (Pokemon Box files)
- `.ek1` - `.ek9` (Encrypted files)
- `.ekx` (Encrypted batch)

This ensures:

- Database is secure and hidden from casual user access
- Pokemon files are easily accessible for backup/sharing
- Clean separation between application data and user data
- Multiple users can run the application independently
- "Dump" folder functionality for easy file management

You can override these paths in `appsettings.json` if needed:

```json
{
  "Vault": {
    "BasePath": "C:\\CustomPath\\storage"
  },
  "ConnectionStrings": {
    "Default": "Data Source=C:\\CustomPath\\pokemonbank.db"
  }
}
```

- `Contracts/`: DTOs for API requests/responses.
- `Domain/`: Core entities for Pokémon, stats, moves, etc.
- `Endpoints/`: Minimal API endpoints (import, get, compare, health).
- `Infrastructure/Services/`: Business logic, file storage, PKHeX integration, helpers (including name resolution and comparison).
- `ReferenceData/`: **Removed**. All enums and static data now use PKHeX.Core GameStrings.

---

## Tech stack

- **Backend:**

  - .NET 9
  - ASP.NET Core Minimal API
  - Entity Framework Core
  - SQLite (local storage)
  - PKHeX.Core (Pokémon parsing, name resolution, GameStrings)
  - Swashbuckle / Swagger UI

- **Other:**
  - Dependency Injection
  - HealthChecks
  - Modular endpoint & service structure
  - Multi-generation compatibility
  - Original file preservation (vault)
  - Pokémon comparison tooling

---

## Goals

### General

Create a **personal Pokémon bank** that:

- Imports Pokémon from `.pk*` files obtained legally.
- Displays detailed information in a user-friendly format.
- Keeps a secure local record of the collection (including original files).

### Technical

- Full compatibility with `.pk*` from Gen 1 to Gen 9 + supported spin-offs.
- Local storage of Pokémon data, metadata & original file (unmodified).
- Advanced parsing and name resolution with PKHeX.Core (no custom enums).
- Showdown export (with accurate names).
- Pokémon comparison endpoint for difference analysis.
- Modular architecture for future UI integration.
- REST API with Swagger documentation.

---

## Current progress

- Full PK9 support: All fields parsed and stored, including new Gen 9 data.
- PKHeX.Core GameStrings: All names (species, moves, items, etc.) resolved via PKHeX.Core, no custom enums.
- Original file preservation: Every import stores the unmodified file in the vault.
- Pokémon comparison: `/pokemon/compare/{id1}/{id2}` endpoint to analyze all differences between two Pokémon (e.g., after trading or editing).
- Showdown export: `/pokemon/{id}/showdown` (GET) returns a Showdown set with accurate names.
- Clean codebase: ReferenceData and all custom enums removed.
- Minimal API endpoints: Import, get, compare, health.
- Swagger UI for testing & docs.
- Launch profiles for HTTP + HTTPS.

---

## Next steps

- [ ] Support ribbons, marks, contest stats (in progress).
- [ ] Return sprites/images in API responses.
- [ ] Implement advanced filtering/search.
- [ ] Build React/TypeScript UI.

---

## License

Personal use. PKHeX.Core & dependencies retain their respective licenses.
