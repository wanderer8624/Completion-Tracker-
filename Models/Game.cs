namespace GameVault.Models;
public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "Action";
    public string Platform { get; set; } = "PC";
    public string Status { get; set; } = "Backlog";
    public int? Rating { get; set; }
    public int HoursPlayed { get; set; }
    public string? Notes { get; set; }
    public string CoverUrl { get; set; } = "";
    public string? OwnerEmail { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
