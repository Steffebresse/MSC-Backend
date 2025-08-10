using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["DataBase:ConnectionString"];
builder.Services.AddDbContext<DbContext>(
    options => options.UseSqlServer(connectionString)
);
builder.Services.AddIdentity();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();






app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

public static class ServiceCollections {
    public static IServiceCollection AddIdentity(this IServiceCollection collection)
    {
        collection.AddAuthorization();
        collection.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<DbContext>();

        return collection;   
    }
}
