

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
public class TestController : ControllerBase
{
    private readonly MyDbContext _context;
    public TestController(MyDbContext context)
    {
        _context = context;
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
    public async Task<IActionResult> Logout( SignInManager<ApplicationUser > signInManager)
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
        var movieToAdd = await _context.movies.FirstOrDefaultAsync();

        var userId = userManager.GetUserId(User);

        var user = await userManager.Users
            .Include(u => u.movies)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null || user.movies != null)
            return Unauthorized("Doesnt work test");
        user.movies.Add(movieToAdd);
        await userManager.UpdateAsync(user);

        return Ok();
        
    }
}