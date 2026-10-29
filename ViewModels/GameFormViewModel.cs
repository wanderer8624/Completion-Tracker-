using System.ComponentModel.DataAnnotations;
namespace GameVault.ViewModels;
public class GameFormViewModel { public int Id {get;set;} [Required, StringLength(80)] public string Title {get;set;}=""; [Required] public string Genre {get;set;}="Action"; [Required] public string Platform {get;set;}="PC"; [Required] public string Status {get;set;}="Backlog"; [Range(0,10)] public int? Rating {get;set;} [Range(0,10000)] public int HoursPlayed {get;set;} [StringLength(1000)] public string? Notes {get;set;} [Url] public string? CoverUrl {get;set;} }
