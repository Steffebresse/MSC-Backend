

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

    // Post endpoints

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

    // Get endpoints

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
        if ((movieId == null || movieId == Guid.Empty) && string.IsNullOrWhiteSpace(userId))
            return null;

        List<Discussion> fetched = new();

        if (movieId != null && movieId != Guid.Empty)
        {
            fetched = await _context.Discussions
                .Where(d => d.MovieId == movieId)
                .OrderByDescending(d => d.Posts.Max(p => p.PostedAt))
                .Include(d => d.Posts)
                .ToListAsync();
        }
        else
        {
            fetched = await _context.Discussions
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Posts.Max(p => p.PostedAt))
                .Include(d => d.Posts)
                .ToListAsync();
        }

        return fetched
         .Select(src => new DiscussionGetListDTO().Map(src))
         .ToList();


    }

    // Delete methods

    public async Task<bool> DeleteDiscussion(Guid? discussionId)
    {
        var deleted = await _context.Discussions.Where(D => D.Id == discussionId).ExecuteDeleteAsync();

        await _context.SaveChangesAsync();

        return deleted > 0;
    }


    public async Task<bool> DeletePost(Guid? postId)
    {
        var deleted = await _context.Posts.Where(D => D.Id == postId).ExecuteDeleteAsync();

        await _context.SaveChangesAsync();

        return deleted > 0;
    }

    // Update endpoints

    public async Task<DiscussionGetListDTO?> UpdateDiscussion(Guid? discussionId, string content)
    {
        DiscussionGetListDTO success = new();

        if (discussionId == null || discussionId == Guid.Empty)
            return null;

        var updated = await _context.Discussions.Where(d => d.Id == discussionId).Include(p => p.Posts).FirstOrDefaultAsync();
        if (updated != null && content != string.Empty)
            updated.DiscussionContent = content;
        else
            return null;

        _context.Update(updated);

        await _context.SaveChangesAsync();

        return success.Map(updated);


    }
    

    public async Task<PostDto?> UpdatePost(Guid? postId, string content)
    {
        PostDto success = new();

        if (postId == null || postId == Guid.Empty)
            return null;

        var updated = await _context.Posts.Where(d => d.Id == postId).Include(u => u.User).FirstOrDefaultAsync();
        if (updated != null && content != string.Empty)
            updated.Content = content;
        else
            return null;

        _context.Update(updated);

        await _context.SaveChangesAsync();

        return updated.Map(updated);


    }



    


    

}