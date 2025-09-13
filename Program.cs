using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebPWrecover.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["DataBase:ConnectionString"];
builder.Services.AddDbContext<MyDbContext>(
    options => options.UseSqlServer(connectionString)
);

builder.Services.Configure<AuthMessageSenderOptions>(
    builder.Configuration.GetSection(AuthMessageSenderOptions.Position));

builder.Services.AddTransient<MovieApiService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);



builder.Services.AddIdentity();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});
 
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        // Define Bearer authentication (works with Identity's opaque tokens)
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",         // <- important
            BearerFormat = "Token",    // label text; could be "JWT" as well
            In = ParameterLocation.Header,
            Description = "Enter: {access_token}"
        };

        // Make Bearer required by default for all operations
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});


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
        collection.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.BearerScheme;          //Lägger detta som default scheme, så att appen endast kör med denna authentication, sålänge jag inte säger något annat.
            options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
            options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
        });
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

            if (await userManager.FindByEmailAsync(email) == null)
            {
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
}

// TODO REMINDERS
/*

 - Gör en CI/CD

*/
