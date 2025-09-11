


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : IdentityDbContext<ApplicationUser>
{

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Post> Posts { get; set; }
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

        base.OnModelCreating(modelBuilder); // Identity configs

        var seedId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        modelBuilder.Entity<Movie>().HasData(new Movie
        {
            Id = seedId,
            Title = "TestTitle"
        });


        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Ratings)
            .WithOne(r => r.Movie)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Discussion)
            .WithOne(d => d.Movie)
            .HasForeignKey(d => d.MovieId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Post>()
            .HasOne(p => p.Discussion)
            .WithMany(d => d.Posts)
            .HasForeignKey(p => p.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Discussion>()
            .HasOne(d => d.User)
            .WithMany(u => u.Discussions)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<RatingSources>()
            .HasIndex(r => new { r.MovieId, r.Source })
            .IsUnique();
            
            // Databasindexering betyder, att, man, ehhh.. gör en indexering av specifika värden, som man söker ofta på, så hätmingnen går snabbare, annars måste systemet söka hela databasen när något hämtas, vilket tar tid 
            //Ids är alltid indexerade, så man behöver inte göra det i de fallen, då man oftast hämtar data via IDs så går det snabbt. Men om jag hade hämtat något via namn ofta, så är det bra att indexera denna propertie.
    }
}