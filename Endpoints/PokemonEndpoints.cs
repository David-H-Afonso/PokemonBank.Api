
using Microsoft.EntityFrameworkCore;
using PokemonBank.Api.Contracts;
using PokemonBank.Api.Infrastructure;
using PokemonBank.Api.Infrastructure.Services;


namespace PokemonBank.Api.Endpoints
{
    public static class PokemonEndpoints
    {
        public static IEndpointRouteBuilder MapPokemonEndpoints(this IEndpointRouteBuilder app)
        {
            // Admin endpoint to wipe the entire database (dangerous!)
            app.MapPost("/admin/wipe-database", async (AppDbContext db) =>
            {
                db.Pokemon.RemoveRange(db.Pokemon);
                db.PokemonTags.RemoveRange(db.PokemonTags);
                db.Stats.RemoveRange(db.Stats);
                db.Moves.RemoveRange(db.Moves);
                db.RelearnMoves.RemoveRange(db.RelearnMoves);
                db.Files.RemoveRange(db.Files);
                db.Tags.RemoveRange(db.Tags);
                await db.SaveChangesAsync();
                return Results.Ok("Database wiped.");
            });
            // Eliminar solo de la base de datos (no toca archivos en disco)
            app.MapDelete("/pokemon/{pokemonId:int}/database", async (int pokemonId, AppDbContext db) =>
            {
                var poke = await db.Pokemon.FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();

                var stats = await db.Stats.Where(s => s.PokemonId == pokemonId).ToListAsync();
                db.Stats.RemoveRange(stats);
                var moves = await db.Moves.Where(m => m.PokemonId == pokemonId).ToListAsync();
                db.Moves.RemoveRange(moves);
                var relearnMoves = await db.RelearnMoves.Where(rm => rm.PokemonId == pokemonId).ToListAsync();
                db.RelearnMoves.RemoveRange(relearnMoves);
                var file = await db.Files.FirstOrDefaultAsync(f => f.Id == poke.FileId);
                if (file != null)
                    db.Files.Remove(file);
                db.Pokemon.Remove(poke);
                await db.SaveChangesAsync();
                return Results.Ok(new { Deleted = true, BackupDeleted = false });
            });

            // Eliminar de base de datos y backup/disco
            app.MapDelete("/pokemon/{pokemonId:int}/backup", async (int pokemonId, AppDbContext db, FileStorageService storage) =>
            {
                var poke = await db.Pokemon.FirstOrDefaultAsync(x => x.Id == pokemonId);
                if (poke == null) return Results.NotFound();

                var stats = await db.Stats.Where(s => s.PokemonId == pokemonId).ToListAsync();
                db.Stats.RemoveRange(stats);
                var moves = await db.Moves.Where(m => m.PokemonId == pokemonId).ToListAsync();
                db.Moves.RemoveRange(moves);
                var relearnMoves = await db.RelearnMoves.Where(rm => rm.PokemonId == pokemonId).ToListAsync();
                db.RelearnMoves.RemoveRange(relearnMoves);
                var file = await db.Files.FirstOrDefaultAsync(f => f.Id == poke.FileId);
                if (file != null)
                {
                    try { storage.Delete(file.StoredPath); } catch { }
                    db.Files.Remove(file);
                }
                db.Pokemon.Remove(poke);
                await db.SaveChangesAsync();
                return Results.Ok(new { Deleted = true, BackupDeleted = true });
            });

            // ...resto de endpoints (get, patch, etc.)...

            app.MapGet("/pokemon", async (AppDbContext db, [AsParameters] PokemonQuery q) =>
            {
                var query = db.Pokemon.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(q.Search))
                    query = query.Where(p => (p.Nickname ?? "").Contains(q.Search) || p.OtName.Contains(q.Search));
                if (q.SpeciesId.HasValue)
                    query = query.Where(p => p.SpeciesId == q.SpeciesId);
                if (q.IsShiny.HasValue)
                    query = query.Where(p => p.IsShiny == q.IsShiny);
                if (q.BallId.HasValue)
                    query = query.Where(p => p.BallId == q.BallId);
                if (q.OriginGame.HasValue)
                    query = query.Where(p => p.OriginGame == q.OriginGame);
                if (q.TeraType.HasValue)
                    query = query.Where(p => p.TeraType == q.TeraType);

                var total = await query.CountAsync();
                var items = await query
                    .OrderByDescending(p => p.Id)
                    .Skip(q.Skip)
                    .Take(q.Take)
                    .Select(p => new PokemonListItemDto
                    {
                        Id = p.Id,
                        SpeciesId = p.SpeciesId,
                        Nickname = p.Nickname,
                        Level = p.Level,
                        IsShiny = p.IsShiny,
                        BallId = p.BallId,
                        TeraType = p.TeraType,
                        SpriteKey = p.SpriteKey
                    })
                    .ToListAsync();

                return Results.Ok(new PagedResult<PokemonListItemDto>(items, total));
            });

            app.MapGet("/pokemon/{id:int}", async (int id, AppDbContext db) =>
            {
                var p = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (p == null) return Results.NotFound();
                var stats = await db.Stats.AsNoTracking().FirstOrDefaultAsync(x => x.PokemonId == p.Id);
                var moves = await db.Moves.AsNoTracking().Where(x => x.PokemonId == p.Id).OrderBy(x => x.Slot).ToListAsync();
                var relearnMoves = await db.RelearnMoves.AsNoTracking().Where(x => x.PokemonId == p.Id).OrderBy(x => x.Slot).ToListAsync();
                return Results.Ok(new PokemonDetailDto(p, stats, moves, relearnMoves));
            });

            app.MapGet("/pokemon/{id:int}/showdown", async (int id, AppDbContext db) =>
            {
                var p = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (p == null) return Results.NotFound();
                var stats = await db.Stats.AsNoTracking().FirstOrDefaultAsync(x => x.PokemonId == p.Id);
                var moves = await db.Moves.AsNoTracking().Where(x => x.PokemonId == p.Id).OrderBy(x => x.Slot).ToListAsync();
                var text = PokemonBank.Api.Domain.ValueObjects.ShowdownExport.From(p, stats, moves);
                return Results.Text(text);
            });

            app.MapPatch("/pokemon/{id:int}", async (int id, UpdatePokemonDto dto, AppDbContext db) =>
            {
                var p = await db.Pokemon.FirstOrDefaultAsync(x => x.Id == id);
                if (p == null) return Results.NotFound();
                if (dto.Favorite.HasValue) p.Favorite = dto.Favorite.Value;
                if (dto.Notes is not null) p.Notes = dto.Notes;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // Compare two Pokemon to see differences (useful for debugging trades)
            app.MapGet("/pokemon/compare/{id1:int}/{id2:int}", async (int id1, int id2, AppDbContext db) =>
            {
                var p1 = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id1);
                var p2 = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id2);
                
                if (p1 == null || p2 == null) 
                    return Results.NotFound("One or both Pokemon not found");
                
                var comparison = PokemonComparisonService.Compare(p1, p2);
                
                return Results.Ok(new
                {
                    Pokemon1 = new { Id = p1.Id, Species = PkHexStringService.GetSpeciesName(p1.SpeciesId), Nickname = p1.Nickname },
                    Pokemon2 = new { Id = p2.Id, Species = PkHexStringService.GetSpeciesName(p2.SpeciesId), Nickname = p2.Nickname },
                    AreIdentical = comparison.AreIdentical,
                    Differences = comparison.Differences,
                    Summary = comparison.AreIdentical ? "Pokemon are identical" : $"Found {comparison.Differences.Count} differences"
                });
            });

            return app;
        }
    }
}
