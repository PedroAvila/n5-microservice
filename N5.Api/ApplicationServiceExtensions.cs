using Serilog;

namespace N5.Api
{
    public static class ApplicationServiceExtensions
    {
        public static void AddRegisterService(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        public static void SerilogConfiguration(this IHostBuilder host)
        {
            host.UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console();
            });
        }
    }
}
