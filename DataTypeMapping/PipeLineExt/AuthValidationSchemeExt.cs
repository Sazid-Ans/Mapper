using DataTypeMapping.Utilities.AppSettingsDO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DataTypeMapping.CustMiddleware
{
    public static class AuthValidationSchemeExt
    {

        public static WebApplicationBuilder AuthSchemeExt(this WebApplicationBuilder builder)
        {
            //Dont use the below technique as we have already configured the Ioptions pattern for JwtSettings.

            //var JwtSettings = builder.Configuration.GetSection("JWTSettings");
            //var Secret = JwtSettings["SecretKey"];
            //var Issuer = JwtSettings["Issuer"];
            //var audience = JwtSettings["Audience"];

            //Instead use the above initialize approach(only for static class).
            //Resolbe DI manually (be precautious as it creates new container)

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
              
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(key),
                      ValidateIssuer = true,
                      ValidIssuer = jwtSettings.Issuer,
                      ValidateAudience = true,
                      ValidAudience = jwtSettings.Audience,
                      ValidateLifetime = true
                  };
              });
            return builder;
        }

    }
 }
