using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Writers;
using Skipperu.Data;
using Skipperu.Data.Repositories;
using Skipperu.MappingProfile;
using Skipperu.Models.Accounts;
using Skipperu.Repos.Users;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using DotNetEnv.Configuration;
using DotNetEnv;

namespace Skipperu
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            DotNetEnv.Env.Load();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper((action => action.AddProfile(typeof(AutomapperProfile))));
            builder.Services.AddMemoryCache();
            builder.Services.AddDbContext<UserAuthenticationDBcontext>( options =>
            {
                //options.UseLazyLoadingProxies();
                options.EnableSensitiveDataLogging();
                options.UseSqlServer("Server=localhost,1436;Database=LuxDB;User Id=sa;Password=TestServerDefault1@1#$&;TrustServerCertificate=True;");
                //options.UseInMemoryDatabase("LuxDB");
            }
            );

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
             .AddEntityFrameworkStores<UserAuthenticationDBcontext>().AddDefaultTokenProviders();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("Google-auth-ClientID");
                options.ClientSecret = Environment.GetEnvironmentVariable("Google-auth-ClientSecret");
                options.SaveTokens = true;
                options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
                options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                options.Scope.Add("openid");
                options.ReturnUrlParameter = "/root";
                options.Validate();
                options.ForwardSignIn = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ForwardAuthenticate = CookieAuthenticationDefaults.AuthenticationScheme;
                
            }).AddCookie();


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", claimOptions => claimOptions.RequireClaim(Claims.Admin.Type, Claims.Admin.Value));
                options.AddPolicy("IsUser", claimOptions => claimOptions.RequireClaim(Claims.User.Type, Claims.User.Value));
            });



            builder.Services.AddScoped<ICollectionsRepo, CollectionsRepo>();
            builder.Services.AddScoped<IRequestsRepo, RequestsRepo>();
            builder.Services.AddScoped<IGlobalUserRepo, GlobalUserRepo>();
            builder.Services.AddScoped<IGoogleUserRepo, GoogleUserRepo>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
                options.SlidingExpiration = true;              
                options.Cookie.HttpOnly = true;                
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(30); 
            });

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

   

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
