using DataTypeMapping.CustMiddleware;
using DataTypeMapping.Model;
using DataTypeMapping.Model.Context;
using DataTypeMapping.Services;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities.AppSettingsDO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
builder.Services.AddScoped<IAddressService, AddressService>();

//Centralized app settings binding.
builder.Services.AddAppSettings(builder.Configuration);

//Custom Middleware registration for Token Validation
builder.AuthSchemeExt();
builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
