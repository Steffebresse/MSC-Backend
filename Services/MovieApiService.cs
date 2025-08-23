

using System.Drawing;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;

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



        if (await _context.Movies.AnyAsync(m => m.ImdbId == movieToAdd.ImdbId))
        {
            return null;
        }


        await _context.Movies.AddAsync(movieToAdd);
        await _context.SaveChangesAsync();

        return movieToAdd;


    }

    public async Task<MovieDTO?> AddMovieToProfileAsync(string? movieTitle, string userId)
    {
        if (movieTitle is null) return null;


        Movie? movie = new();
        /*
        if (id is not null)
        {
            movie = await _context.Movies.FindAsync(id.Value);
        }
        */


        movie = await TryToAddMovieToDb(movieTitle!);

        if (movie is null) return null;

        var user = _context.Users.Find(userId);

        user.Movies ??= new List<Movie>();
        if (!user.Movies.Any(m => m.ImdbId == movie.ImdbId))
            user.Movies.Add(movie);

        MovieDTO dto = new();

        dto = movie.Map(movie);

        await _context.SaveChangesAsync();

        return dto;
    }

    public async Task<MovieDTO?> GetMovie(Guid Id) // Remove this later
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

    public async Task<DiscussionPostDTO?> StartDiscussionAsync(DiscussionPostDTO postDto, string userId)
    {
        Discussion postDiscussion = new()
        {
            Id = Guid.NewGuid(),
            Title = postDto.Title,
            DiscussionContent = postDto.DiscussionContent,
            MovieId = postDto.MovieId,
            UserId = userId,
        };

        var addEntity = await _context.AddAsync(postDiscussion);

        var success = await _context.SaveChangesAsync();

        if (success > 0)
        {
            return postDto;
        }
        else
        {
            return null;
        }

    }

    public async Task<PostDto?> PostToDiscussion(PostDto post, string userId)
    {
        Post postToDB = new()
        {
            UserId = userId,
            DiscussionId = post.DiscussionId,
            Content = post.Content
        };

        await _context.AddAsync(postToDB);

        var success = await _context.SaveChangesAsync();

        if (success > 0)
        {
            return post;
        }
        else
        {
            return null;
        }
    }

    public async Task<DiscussionGetOpenedDiscussion?> GetDiscussionsAsync(Guid Id)
    {
        if (Id == Guid.Empty)
            return null;


        var fetched = await _context.Discussions.Where(d => d.Id == Id).Include(p => p.Posts).ThenInclude(u => u.User).FirstOrDefaultAsync();

        DiscussionGetOpenedDiscussion dto = new();

        var mapped = dto.Map(fetched);

        return mapped;
    }

    public async Task<List<DiscussionGetListDTO>?> GetDiscussionsListed(Guid? movieId = null, string? userId = null)
    {
        if (movieId == Guid.Empty)
        {
            return null;
        }
       var fetched = await _context.Discussions
            .Where(d => d.MovieId == movieId)
            .OrderByDescending(d => d.Posts.Max(p => p.PostedAt))
            .Include(d => d.Posts)                              
            .ToListAsync();

       

         return fetched
        .Select(src => new DiscussionGetListDTO().Map(src)) 
        .ToList();


    }

    



    


    

}