

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public List<Movie> Movies { get; set; } = [];
    [PersonalData]
    public List<Discussion>? Discussions { get; set; }
    public List<Post>? Posts { get; set; }
    public string firstName { get; set; } = string.Empty;


}