using DataTypeMapping.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var conn = builder.Configuration.GetSection("ConnectionStrings");

//Db context for Api
builder.Services.AddDbContext<MapApiDbContext>(options => options.UseSqlServer(conn["MapperApiDb"]));

//Db Context for Identity
builder.Services.AddDbContext<MapApiIdentityContext>(options =>
    options.UseSqlServer(conn["MapperApiIdentityDb"]));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<MapApiIdentityContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
