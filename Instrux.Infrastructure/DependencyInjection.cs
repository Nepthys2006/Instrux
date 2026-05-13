using Instrux.Domain.Interfaces;
using Instrux.Infrastructure.Data;
using Instrux.Infrastructure.Data.Interceptors;
using Instrux.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instrux.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                opt => opt.MigrationsAssembly(
                    typeof(AppDbContext).Assembly.FullName));

            options.AddInterceptors(interceptor);
        });

        services.AddScoped<DbContext, AppDbContext>(
            sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
