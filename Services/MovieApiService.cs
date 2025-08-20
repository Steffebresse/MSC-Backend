

using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class MovieApiService
{
    private readonly MyDbContext _context;
    private readonly IConfiguration _config;
   
    private HttpClient client = new();
   
    private string endPoint;

    private string ApiKey;

    public MovieApiService(MyDbContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _config = config;
        ApiKey = _config["MovieAPI:MovieApiKey"]!;
        endPoint = $"http://www.omdbapi.com/?apikey={ApiKey}&";
        
    }


    public async Task<Movie?> TryToAddMovieToDb(string movieTitle)
    {
        var response = await client.GetAsync(endPoint + $"t={movieTitle}");
        if (!response.IsSuccessStatusCode) return null;



        var json = await response.Content.ReadAsStringAsync();

        var DTOMovie = JsonSerializer.Deserialize<MovieDTO>(json);

        var movieToAdd = DTOMovie.Map(DTOMovie);

        await _context.Movies.AddAsync(movieToAdd);
        await _context.SaveChangesAsync();

        return movieToAdd;
    }

      public async Task<Movie?> AddMovieToProfileAsync(string? movieTitle, string userId)
    {
        if (movieTitle is null) return null;

    
        Movie? movie = new();
        /*
        if (id is not null)
        {
            movie = await _context.Movies.FindAsync(id.Value);
        }
        */
        
        
            movie = await TryToAddMovieToDb(movieTitle!); // make this async if it touches I/O
        
        if (movie is null) return null;

        var user = _context.Users.Find(userId);

        user.Movies ??= new List<Movie>();
        if (!user.Movies.Any(m => m.Id == movie.Id))
            user.Movies.Add(movie);

        await _context.SaveChangesAsync(); // Save via DbContext (preferred for relationship updates)

        return movie;
    }

    public async Task<MovieDTO?> GetMovie(Guid Id)
    {
        try
        {
            if (_context is not null)
            {
                var movie = await _context.Movies.Where(m => m.Id == Id).Include(m => m.Ratings).FirstOrDefaultAsync();

                if (movie is not null)
                {
                    var mapped = movie.Map(movie);
                    return mapped;
                }
            }
        }
        catch (ArgumentException ex)
        {
            System.Console.WriteLine($"Something went wrong: '{ex}'");
        }

        return null;



    }


    

}