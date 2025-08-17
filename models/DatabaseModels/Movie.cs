

using System.ComponentModel.DataAnnotations;

public class Movie : IDisposable
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string ImdbId { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Runtime { get; set; } = string.Empty;
    public string Plot { get; set; } = string.Empty;
    public List<RatingSources>? Ratings { get; set; }
    public float MscRating { get; set; }
    public List<string> Actors { get; set; } = new();
    public List<string> Directors { get; set; } = new();
    public Uri? Poster { get; set; } // antar det är bättre att spara med URI då slipper jag konvertera skiten?

    public List<ApplicationUser> mSCUsers { get; set; } = [];

    public void Dispose()
    {
        throw new NotImplementedException();
    }


}

public class MovieDTO
{
    public string imdbID { get; set; }
    public string Title { get; set; }
    public string Released { get; set; }
    public string Runtime { get; set; }
    public string Plot { get; set; }
    public List<RatingSources> Ratings { get; set; } // Fixa ratings senare
    public string MscRating { get; set; }
    public string Actors { get; set; }
    public string Director { get; set; }
    public string Poster { get; set; }

    public Movie Map(MovieDTO movie)
    {
        Movie mapped = new();
        mapped.Id = Guid.NewGuid();
        mapped.Title = movie.Title;
        mapped.Actors = movie.Actors.Split(",").ToList();
        mapped.Directors = movie.Director.Split(",").ToList();
        mapped.Runtime = movie.Runtime;
        mapped.ReleaseDate = DateTime.Parse(movie.Released);
        mapped.Plot = movie.Plot;
        mapped.Poster = new Uri(movie.Poster);
        mapped.ImdbId = movie.imdbID;
        foreach (var rating in movie.Ratings)
        {
            rating.MovieId = mapped.Id;
        }
        mapped.Ratings = movie.Ratings;


        return mapped;

    }



}

public class RatingSources
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } 
    public string Source { get; set; }
    public string Value { get; set; }
}

