using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["DataBase:ConnectionString"];
builder.Services.AddDbContext<MyDbContext>(
    options => options.UseSqlServer(connectionString)
);



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

app.Run();

public static class ServiceCollections
{
    public static IServiceCollection AddIdentity(this IServiceCollection collection)
    {
        collection.AddAuthorization();
        collection.AddIdentityApiEndpoints<ApplicationUser>()
            .AddEntityFrameworkStores<MyDbContext>();


        return collection;
    }
}

// TODO REMINDERS
/*
 - Testa att ändra Apllicationuser till något annat namn, bekräfta om det är ett krav

 
*/
