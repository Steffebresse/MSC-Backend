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
await app.UserAdminGeneration(builder.Configuration);

app.Run();

public static class ServiceCollections
{
    public static IServiceCollection AddIdentity(this IServiceCollection collection)
    {
        collection.AddAuthorization();
        collection.AddIdentityApiEndpoints<ApplicationUser>() // Lägger till services
        .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MyDbContext>();




        return collection;
    }

    public static async Task RoleManager(this WebApplication app) // Hämtar services
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Mod", "User" };

            foreach (var role in roles) // REMINDER:::: Anledningen till att man måste hämta scopes utanför kontroller och andra sammanhang när man 
                                        // hade kört DI för servicen, är för att DI kan inte dela ut services innan appen körts, vilket betyder om du behöver en service innan app.run()
                                        // Så behöver du hämta den med ett scope manuellt. Annars sköts det Automatiskt. Innan app.run() så förbereds endast servicarna, men du använder kontroller
                                        // efter app.run() exuterats, så darför kan du hämta services via DI då!
            {

                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));



            }
        }
    }

    public static async Task UserAdminGeneration(this WebApplication app, ConfigurationManager config)
    {
        using (var scope = app.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string? email = config["AdminCreds:email"];
            string? passWord = config["AdminCreds:passWord"];

            var adminUser = new ApplicationUser()
            {
                UserName = email,
                Email = email
            };



            await userManager.CreateAsync(adminUser, passWord);

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// TODO REMINDERS
/*

 - Gör en CI/CD

*/
