

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class Discussion // Dependent since, this need to exist for a post to exist
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string DiscussionContent { get; set; }
    public Guid MovieId { get; set; }
    [Required]
    public Movie Movie { get; set; }
    [Required]
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }
    public List<Post>? Posts { get; set; }
    public DateTime PostedAt { get; set; } = DateTime.Now;



}

public class DiscussionPostDTO // Post Discussion
{
    public string Title { get; set; }
    public string DiscussionContent { get; set; }
    public Guid MovieId { get; set; }


}

public class DiscussionGetListDTO : DiscussionPostDTO // Hämta lista på alla Diskussioner använd denna
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public int? PostCount { get; set; } = null;
    public DateTime PostedAt { get; set; } = DateTime.Now;

    public DiscussionGetListDTO Map(Discussion map)
    {
        try
        {
            DiscussionGetListDTO mapped = new();

            mapped.Title = map.Title;
            mapped.DiscussionContent = map.DiscussionContent;
            mapped.MovieId = map.MovieId;
            mapped.Id = map.Id;
            mapped.UserId = map.UserId;
            mapped.PostCount = map.Posts.Count();
            mapped.PostedAt = map.PostedAt;

            return mapped;

        }
        catch (ArgumentException ex)
        {
            throw new Exception("Something went wrong Excpetion: " + ex);
        }
        

        
    }
}

public class DiscussionGetOpenedDiscussion : DiscussionGetListDTO // Använd denna om du skall öppna en specifik diskussion
{
    public List<PostDto?> Posts { get; set; } = new();

    public DiscussionGetOpenedDiscussion Map(Discussion map)
    {
        
            try
            {
                DiscussionGetOpenedDiscussion mapped = new();

                mapped.Title = map.Title;
                mapped.DiscussionContent = map.DiscussionContent;
                mapped.MovieId = map.MovieId;
                mapped.Id = map.Id;
                mapped.UserId = map.UserId;
                mapped.PostCount = map.Posts.Count();
                mapped.PostedAt = map.PostedAt;
                mapped.Posts = [.. map.Posts.Select(p => p.Map(p)).ToList()];

                return mapped;

            }
            catch (ArgumentException ex)
            {
                throw new Exception("Something went wrong Excpetion: " + ex);
            }
        
    }
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
    public string Content { get; set; }

    public PostDto Map(Post map)
    {
        try
        {
            PostDto mapped = new();

            mapped.DiscussionId = map.DiscussionId;
            mapped.Content = map.Content;
            //mapped.UserName = map.User.UserName ?? string.Empty; // Denna funkar inte av ngn anledning kolla upp det
            mapped.userId = map.UserId;
            return mapped;
        }
        catch (ArgumentException ex)
        {
            throw new Exception("Something went wrong Exception: " + ex);
        }
        

    }
}

public class PostDto
{

    public Guid DiscussionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;


}