using Application.Banks;
using Application.Core;
using Application.Core.MappingProfiles;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            services.AddDbContext<DataContext>(opt =>
            {
                var conString = "";

                conString = config.GetConnectionString("DefaultConnection");

                var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
                opt.UseMySql(conString, serverVersion);
            });
             string MyAllowSpecificOrigins = "CorsPolicy";
            //add cors service
            services.AddCors(opt =>
            {
                opt.AddPolicy(name:MyAllowSpecificOrigins, policy =>
                {
                    var client_url = config.GetSection("CORS:AllowedOrigins").Get<string[]>();

                    policy.WithOrigins(client_url)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            });
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddSignalR();

            return services;
        }
    }
}
