using DataTypeMapping.CustMiddleware;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Services;
using DataTypeMapping.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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

builder.Services.AddIdentity<Customer, IdentityRole>().AddEntityFrameworkStores<MapApiIdentityContext>()
    .AddDefaultTokenProviders();

//Dependency Injection for Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IJwtService, JwtService>();

//Centralized app settings binding.
builder.Services.AddAppSettings(builder.Configuration);

//Custom Middleware registration for Token Validation
builder.Services.AddScoped<TokenValidatorMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("My API")
            .WithTheme(ScalarTheme.Default); // optional (Dark mode);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Custom Middleware for Token Validation
app.UseMiddleware<TokenValidatorMiddleware>(); // Use custom middleware BEFORE MVC

app.MapControllers();

app.Run();
