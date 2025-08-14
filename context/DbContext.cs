


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : IdentityDbContext<ApplicationUser >
{

    public DbSet<Movie> movies { get; set; }
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Always call this for Identity first

        var seedId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"); 
        modelBuilder.Entity<Movie>().HasData(new Movie
        {
            Id = seedId,
            Title = "TestTitle"
        });
    }
}