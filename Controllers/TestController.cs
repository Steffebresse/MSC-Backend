

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
public class TestController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly MovieApiService _MApiService;
    public TestController(MyDbContext context, MovieApiService MApiService)
    {
        _context = context;
        _MApiService = MApiService;
    }


    [Authorize]
    [HttpGet("tESTthIS")]
    public IActionResult Test123()
    {
        var returnthis = "Work";
        return Ok(returnthis);
    }
    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout(SignInManager<ApplicationUser> signInManager)
    {
        string empty = "fattar inte";
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
        return Unauthorized();
    }

    [Authorize]
    [HttpPost("AddTestMovie")]
    public async Task<IActionResult> TestAddMovie(UserManager<ApplicationUser> userManager)
    {
        var movieToAdd = await _context.Movies.FirstOrDefaultAsync();

        var userId = userManager.GetUserId(User);

        var user = await userManager.Users
            .Include(u => u.Movies)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Movies != null)
            return Unauthorized("Doesnt work test");
        user.Movies.Add(movieToAdd);
        await userManager.UpdateAsync(user);

        return Ok();

    }

    [Authorize]
    [HttpPost("AddMovieFromIMDB")]
    public async Task<IActionResult> TestAddMovieFromIMDB(UserManager<ApplicationUser> userManager, [FromBody] string movieTitle)
    {
        var movieToAdd = await _MApiService.TryToAddMovieToDb(movieTitle);

        var userId = userManager.GetUserId(User);

        var user = await userManager.Users
            .Include(u => u.Movies)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Movies != null)
            return Unauthorized("Doesnt work test");
        user.Movies.Add(movieToAdd);
        await userManager.UpdateAsync(user);

        return Ok();

    }


    [HttpGet("TestGetMovie")]
    public async Task<IActionResult> TestGetMovie(Guid Id)
    {
        var gotMovie = await _MApiService.GetMovie(Id);

        if (gotMovie == null)
            return BadRequest("SOMETHING WENT WRONG");

        return Ok(gotMovie);
    }
    [Authorize]
    [HttpPost("profile")]
    public async Task<IActionResult> AddToProfile([FromBody] string title, UserManager<ApplicationUser> userManager)
    {
        

        var userId = userManager.GetUserId(User);

        var movie = await _MApiService.AddMovieToProfileAsync(title, userId);

        if (movie is null)
            return NotFound("Movie not found or could not be added.");

        return Ok(movie);
    }
}