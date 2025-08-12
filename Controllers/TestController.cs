

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
    public async Task<IActionResult> Logout( SignInManager<MSCUser> signInManager)
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
    public async Task<IActionResult> TestAddMovie(UserManager<MSCUser> userManager)
    {
        var movieToAdd = await _context.movies.FirstOrDefaultAsync();

        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();
        user.movies.Add(movieToAdd);
        await userManager.UpdateAsync(user);

        return Ok(user);
        
    }
}