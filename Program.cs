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

namespace Skipperu
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper((action => action.AddProfile(typeof(AutomapperProfile))));
            builder.Services.AddDbContext<UserAuthenticationDBcontext>( options =>
            {
                //options.UseLazyLoadingProxies();
                options.UseSqlServer("Server=localhost,1436;Database=LuxDB;User Id=sa;Password=TestServerDefault1@1#$&;TrustServerCertificate=True;");
            }
            );
            builder.Services.AddAuthentication();
           
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", claimOptions => claimOptions.RequireClaim(Claims.Admin.Type, Claims.Admin.Value));
                options.AddPolicy("IsUser", claimOptions => claimOptions.RequireClaim(Claims.User.Type, Claims.User.Value));
            });


            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<UserAuthenticationDBcontext>().AddDefaultTokenProviders();
          
            builder.Services.AddScoped<ICollectionsRepo, CollectionsRepo>();
            builder.Services.AddScoped<IRequestsRepo, RequestsRepo>();
            builder.Services.AddScoped<IGlobalUserRepo, GlobalUserRepo>();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
