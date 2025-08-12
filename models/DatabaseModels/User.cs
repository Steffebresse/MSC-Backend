

using Microsoft.AspNetCore.Identity;

public class MSCUser : IdentityUser
{
    [PersonalData]
    public List<Movie> movies { get; set; } = [];
    
}