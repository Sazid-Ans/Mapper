using DataTypeMapping.Model;
using DataTypeMapping.Services;
using DataTypeMapping.Services.Interface;
using DataTypeMapping.Utilities.AppSettingsDO;
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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();


//Centralized app settings binding.
builder.Services.AddAppSettings(builder.Configuration);


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

app.MapControllers();

app.Run();
