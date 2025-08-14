

using System.ComponentModel.DataAnnotations;

public class Movie : IDisposable
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string? Title { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int? Runtime { get; set; }
    public string? Plot { get; set; } = string.Empty;
    public float? ImdbRating { get; set; }
    public float? MscRating { get; set; }
    public List<string>? Actors { get; set; } = new();
    public List<string>? Directors { get; set; } = new();
    public Uri? Poster { get; set; } // antar det är bättre att spara med URI då slipper jag konvertera skiten?

    public List<ApplicationUser > mSCUsers { get; set; } = [];

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}