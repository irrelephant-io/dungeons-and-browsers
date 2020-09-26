using System.Text;
using System.Threading.Tasks;
using Irrelephant.DnB.Server.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Irrelephant.DnB.Server.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static byte[] GetIssuerKey(IConfiguration configuration)
        {
            var stringKey = configuration.GetValue<string>("Auth:JwtTokenKey");
            return Encoding.ASCII.GetBytes(stringKey);
        }

        private static string GetIssuer(IConfiguration configuration)
        {
            return configuration.GetValue<string>("Auth:JwtTokenIssuer");
        }

        public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var issuerKey = GetIssuerKey(configuration);
            var issuer = GetIssuer(configuration);
            services
                .AddAuthentication(x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opts => {
                    opts.RequireHttpsMetadata = false;
                    opts.SaveToken = true;
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(issuerKey),
                        ValidIssuer = issuer,
                        ValidateIssuer = !string.IsNullOrEmpty(issuer),
                        ValidateAudience = false
                    };
                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => {
                            if (context.Request.Path.ToString().StartsWith("/combat"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }

                            return Task.CompletedTask;
                        },
                    };
                });
            return services
                .AddSingleton<ITokenValidator, GoogleIdTokenValidator>()
                .AddSingleton<IAuthenticationService>(provider => {
                    var tokenValidator = provider.GetService<ITokenValidator>();
                    return new JwtViaGoogleAuthenticationService(tokenValidator, issuerKey, issuer);
                });
        }
    }
}
