

using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class MovieApiService
{
    private readonly MyDbContext _context;
    private readonly IConfiguration _config;
    private HttpClient client = new();
    private string endPoint;

    private string ApiKey;

    public MovieApiService(MyDbContext context, IConfiguration config)
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