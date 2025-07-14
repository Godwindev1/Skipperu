using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skipperu.Data;
using Skipperu.MappingProfile;

namespace Skipperu
{
    public class Program
    {
        public static void Main(string[] args)
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
                options.UseSqlServer("Server=localhost,1436;Database=LuxDB;User Id=sa;Password=TestServerDefault1@1#$&;TrustServerCertificate=True;");
            }
            );
            builder.Services.AddAuthentication();
            builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<UserAuthenticationDBcontext>();
            var app = builder.Build();

            app.MapIdentityApi<IdentityUser>();


            // Configure the HTTP request pipeline.
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
