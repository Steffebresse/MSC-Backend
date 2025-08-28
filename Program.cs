using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["DataBase:ConnectionString"];
builder.Services.AddDbContext<MyDbContext>(
    options => options.UseSqlServer(connectionString)
);

builder.Services.AddTransient<MovieApiService>();

builder.Services.AddIdentity();

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });

}



app.MapIdentityApi<ApplicationUser>();

// controller

app.MapControllers();

app.UseHttpsRedirection();

await app.RoleManager();

app.Run();

public static class ServiceCollections
{
    public static IServiceCollection AddIdentity(this IServiceCollection collection)
    {
        collection.AddAuthorization();
        collection.AddIdentityApiEndpoints<ApplicationUser>()
        .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MyDbContext>();




        return collection;
    }

    public static async Task RoleManager(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Mod", "User" };

            foreach (var role in roles)
            {

                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            }
        }
    }
}

// TODO REMINDERS
/*

 - Gör en CI/CD

*/
