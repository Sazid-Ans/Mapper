using AuthServer.Dto.ResponseDto;
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

                  options.Events = new JwtBearerEvents
                  {
                      OnChallenge = async context =>
                      {
                          // Skip the default response
                          context.HandleResponse();

                          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                          context.Response.ContentType = "application/json";

                          var baseResponse = BaseResponse<string>.Failure(
                              "You are not authorized to access this resource." , StatusCodes.Status401Unauthorized.ToString());

                          await context.Response.WriteAsJsonAsync(baseResponse);
                      },
                      OnForbidden = async context =>
                      {
                          context.Response.StatusCode = StatusCodes.Status403Forbidden;
                          context.Response.ContentType = "application/json";

                          var baseResponse = BaseResponse<string>.Failure(
                              StatusCodes.Status403Forbidden.ToString(), "You do not have permission to access this resource.");

                          await context.Response.WriteAsJsonAsync(baseResponse);
                      }
                  };
              });
            return builder;
        }

    }
 }
