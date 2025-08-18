

using Microsoft.Extensions.Diagnostics.HealthChecks;

public class Discussion
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid MovieId { get; set; }
    public Movie? Movie { get; set; } = null;
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public List<Posts>? Posts { get; set; }



}

public class Posts 
{
    
}