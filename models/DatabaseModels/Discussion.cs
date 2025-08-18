

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class Discussion // Dependent since, this need to exist for a post to exist
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid MovieId { get; set; }
    [Required]
    public Movie Movie { get; set; } 
    [Required]
    public string UserId { get; set; }
    
    public ApplicationUser User { get; set; }
    public List<Post>? Posts { get; set; }
    public DateTime PostedAt { get; set; } = DateTime.Now;



}

public class Post // Principal since, this existing is based on the Dependent entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime PostedAt { get; set; } = DateTime.Now;
    [Required]
    public Guid DiscussionId { get; set; }
    public Discussion Discussion { get; set; }
    [Required]
    public ApplicationUser User { get; set; }
    [Required]
    public string UserId { get; set; }

}