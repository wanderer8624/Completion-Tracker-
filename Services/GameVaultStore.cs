using System.Security.Cryptography;
using System.Text.Json;
using GameVault.Models;
namespace GameVault.Services;
public class GameVaultStore
{
    private readonly string _path;
    private readonly object _gate = new();
    private StoreData _data;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    public GameVaultStore(IWebHostEnvironment env) { var dir = Path.Combine(env.ContentRootPath, "App_Data"); Directory.CreateDirectory(dir); _path = Path.Combine(dir, "vault.json"); _data = Load(); if (_data.Games.Count == 0) { Seed(); Save(); } }
    private StoreData Load() { try { return File.Exists(_path) ? JsonSerializer.Deserialize<StoreData>(File.ReadAllText(_path)) ?? new() : new(); } catch { return new(); } }
    private void Save() { File.WriteAllText(_path, JsonSerializer.Serialize(_data, JsonOptions)); }
    private void Seed() {
        var rows = new[] {
            new Game{Title="Cyberpunk 2077",Genre="RPG",Platform="PC",Status="Backlog",CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/1091500/header.jpg"},
            new Game{Title="Elden Ring",Genre="Soulslike",Platform="PC",Status="Playing",HoursPlayed=18,CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/1245620/header.jpg"},
            new Game{Title="Red Dead Redemption 2",Genre="Action Adventure",Platform="PC",Status="Backlog",CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/1174180/header.jpg"},
            new Game{Title="Hades",Genre="Roguelike",Platform="PC",Status="Completed",Rating=9,HoursPlayed=34,CompletedAt=DateTime.UtcNow.AddDays(-12),CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/1145360/header.jpg"},
            new Game{Title="Baldur's Gate 3",Genre="RPG",Platform="PC",Status="Backlog",CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/1086940/header.jpg"},
            new Game{Title="Sekiro: Shadows Die Twice",Genre="Action",Platform="PC",Status="Playing",HoursPlayed=11,CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/814380/header.jpg"},
            new Game{Title="Portal 2",Genre="Puzzle",Platform="PC",Status="Completed",Rating=10,HoursPlayed=9,CompletedAt=DateTime.UtcNow.AddDays(-45),CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/620/header.jpg"},
            new Game{Title="The Witcher 3: Wild Hunt",Genre="RPG",Platform="PC",Status="Backlog",CoverUrl="https://cdn.akamai.steamstatic.com/steam/apps/292030/header.jpg"}
        };
        foreach(var g in rows) { g.Id = _data.NextGameId++; _data.Games.Add(g); }
    }
    public (int total, int playing, int completed, int backlog, int hours, double avgRating) Stats(string? email) { lock(_gate) { var g=ForUser(email).ToList(); return(g.Count,g.Count(x=>x.Status=="Playing"),g.Count(x=>x.Status=="Completed"),g.Count(x=>x.Status=="Backlog"),g.Sum(x=>x.HoursPlayed),g.Where(x=>x.Rating.HasValue).Select(x=>(double)x.Rating!.Value).DefaultIfEmpty(0).Average()); } }
    public List<Game> ForUser(string? email) { var baseGames=_data.Games.Where(g=>g.OwnerEmail==null || (email!=null && g.OwnerEmail==email)); return baseGames.OrderByDescending(g=>g.AddedAt).ToList(); }
    public Game? Find(int id,string? email) { lock(_gate) return _data.Games.FirstOrDefault(g=>g.Id==id && (g.OwnerEmail==null || g.OwnerEmail==email)); }
    public void Add(Game game,string email) { lock(_gate) { game.Id=_data.NextGameId++; game.OwnerEmail=email; game.AddedAt=DateTime.UtcNow; _data.Games.Add(game); Save(); } }
    public bool Update(Game game,string email) { lock(_gate) { var old=_data.Games.FirstOrDefault(g=>g.Id==game.Id && g.OwnerEmail==email); if(old==null)return false; old.Title=game.Title;old.Genre=game.Genre;old.Platform=game.Platform;old.Status=game.Status;old.Rating=game.Rating;old.HoursPlayed=Math.Max(0,game.HoursPlayed);old.Notes=game.Notes;old.CoverUrl=game.CoverUrl;if(old.Status=="Completed" && old.CompletedAt==null)old.CompletedAt=DateTime.UtcNow;if(old.Status!="Completed")old.CompletedAt=null;Save();return true; } }
    public bool Delete(int id,string email) { lock(_gate) { var g=_data.Games.FirstOrDefault(x=>x.Id==id && x.OwnerEmail==email);if(g==null)return false;_data.Games.Remove(g);Save();return true;} }
    public AppUser? User(string email) { lock(_gate) return _data.Users.FirstOrDefault(u=>u.Email.Equals(email,StringComparison.OrdinalIgnoreCase)); }
    public bool Register(AppUser user) { lock(_gate) { if(_data.Users.Any(u=>u.Email.Equals(user.Email,StringComparison.OrdinalIgnoreCase)))return false;_data.Users.Add(user);Save();return true;} }
    public static string Hash(string password) { var salt=RandomNumberGenerator.GetBytes(16);var hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,120000,HashAlgorithmName.SHA256,32);return Convert.ToBase64String(salt)+":"+Convert.ToBase64String(hash); }
    public static bool Verify(string password,string encoded) { try { var a=encoded.Split(':');var salt=Convert.FromBase64String(a[0]);var expected=Convert.FromBase64String(a[1]);var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,120000,HashAlgorithmName.SHA256,32);return CryptographicOperations.FixedTimeEquals(actual,expected); } catch { return false; } }
    private class StoreData { public int NextGameId {get;set;}=1; public List<Game> Games {get;set;}=new(); public List<AppUser> Users {get;set;}=new(); }
}
