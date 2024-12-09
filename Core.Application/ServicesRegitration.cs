


using Core.Application.Interfaces.Services;
using Core.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application
{
    public static class ServicesRegitration
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            #region Services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IReservaService, ReservaService>();
            services.AddTransient<IVehiculoService, VehiculoService>();


            #endregion
        }

    }
}
