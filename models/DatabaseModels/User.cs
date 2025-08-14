

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public List<Movie> movies { get; set; } = [];
    public string firstName { get; set; } = string.Empty;
    
}