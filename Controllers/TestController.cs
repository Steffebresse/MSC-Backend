

using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

[ApiController]
public class TestController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly MovieApiService _MApiService;
    private readonly EmailSender _emailSender;
    private readonly UserManager<ApplicationUser> _userManager;
    public TestController(MyDbContext context, MovieApiService MApiService, EmailSender emailsender, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _MApiService = MApiService;
        _emailSender = emailsender;
        _userManager = userManager;
    }


    [Authorize]
    [HttpGet("tESTthIS")]
    public IActionResult Test123()
    {
        var returnthis = "Work";
        return Ok(returnthis);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        // Create the user object
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email
        };

        // Create user with hashed password
        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                Message = "Registration failed",
                Errors = result.Errors
            });
        }

        // Generate confirmation token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Build link (to API or frontend)
        var confirmUrl = Url.Action(
            nameof(ConfirmEmail),
            "Test", // controller name without "Controller"
            new { userId = user.Id, code = tokenEncoded },
            protocol: Request.Scheme);

        // Send email
        await _emailSender.SendEmailAsync(
            req.Email,
            "Confirm your email",
            $"Please confirm your account by clicking this link: {confirmUrl}");

        return Ok(new { Message = "Registration successful. Check your email to confirm your account." });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("Invalid user");

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Email confirmation failed", Errors = result.Errors });
        }

        return Ok(new { Message = "Email confirmed successfully!" });
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

    [Authorize]
    [HttpPost("StartDiscussion")]
    public async Task<IActionResult> AddDiscussion([FromBody] DiscussionPostDTO postDiscussion, UserManager<ApplicationUser> userManager)
    {
        var userId = userManager.GetUserId(User);

        if (userId == null)
            return BadRequest("UserId not valid");

        var success = await _MApiService.StartDiscussionAsync(postDiscussion, userId);

        if (success != null)
            return Ok(success);

        return BadRequest("Something went wrong creating the discussion");
    }

    [Authorize]
    [HttpPost("PostToDiscussion")]
    public async Task<IActionResult> AddPostToDiscussion([FromBody] PostDto postPost, UserManager<ApplicationUser> userManager)
    {
        var userId = userManager.GetUserId(User);

        if (userId == null)
            return BadRequest("UserId not valid");

        var success = await _MApiService.PostToDiscussion(postPost, userId);

        if (success != null)
            return Ok(success);

        return BadRequest("Something went wrong creating the discussion");
    }

    [Authorize]
    [HttpGet("GetDiscussion")]
    public async Task<IActionResult> AddPostToDiscussion([FromBody] Guid DiscussionId)
    {


        if (DiscussionId == Guid.Empty)
            return BadRequest("DiscussionId not valid");

        var success = await _MApiService.GetDiscussionsAsync(DiscussionId);

        if (success != null)
            return Ok(success);

        return BadRequest("Something went wrong creating the discussion");
    }

    [Authorize]
    [HttpGet("GetDiscussionFromMovieId")]
    public async Task<IActionResult> GetDiscussionFromMovieId(Guid? movieId, string? UserId = null)
    {
        var success = await _MApiService.GetDiscussionsListed(movieId, UserId);

        if (success == null)
            return NotFound("Discussion Not Found");

        return Ok(success);
    }

    // ----- Delete Endpoints

    [Authorize]
    [HttpDelete("DeleteDiscussion")]
    public async Task<IActionResult> DeleteDiscussion(Guid? discussionId)
    {
        if (discussionId == null || discussionId == Guid.Empty)
            return NotFound("DiscussionId is null");

        return await _MApiService.DeleteDiscussion(discussionId.Value)
            ? Ok("Deleted successfully")
            : NotFound("Discussion not found");
    }

    [Authorize]
    [HttpDelete("DeletePost")]
    public async Task<IActionResult> DeletePost(Guid? postId)
    {
        if (postId == null || postId == Guid.Empty)
            return NotFound("postId is null");

        return await _MApiService.DeletePost(postId.Value)
            ? Ok("Deleted successfully")
            : NotFound("Discussion not found");
    }

    // update Endpoints

    [Authorize]
    [HttpPut("UpdateDiscussion")]
    public async Task<IActionResult> UpdateDiscussion(Guid? discussionId, string content)
    {
        if (discussionId == null || discussionId == Guid.Empty)
            return NotFound("postId is null");

        return Ok(await _MApiService.UpdateDiscussion(discussionId, content));

    }

    [Authorize]
    [HttpPut("UpdatePost")]
    public async Task<IActionResult> UpdatePost(Guid? postId, string content)
    {
        if (postId == null || postId == Guid.Empty)
            return NotFound("postId is null");

        return Ok(await _MApiService.UpdatePost(postId, content));

    }
    

    // Admin test

    [Authorize(
    AuthenticationSchemes = "Identity.Application", // "Identity.Application"
    Roles = "Admin")]
    [HttpGet("AdminTest")]
    public IActionResult TestAdmin()
    {
        return Ok("Worked");
    }
}