using Core.Application.Enums;
using Core.Application.Interfaces.Services;
using Core.Application.Services;
using Core.Application.ViewModel.DashBoard;
using Core.Application.ViewModel.Reserva;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCars.Models;
using System.Diagnostics;

namespace RentCars.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReservaService reservaService;
        private readonly IUserService userService;
        private readonly IVehiculoService vehiculoService;

        public HomeController(ILogger<HomeController> logger, IReservaService reservaService, IUserService userService,IVehiculoService vehiculoService)
        {
            _logger = logger;
            this.reservaService = reservaService;
            this.userService = userService;
            this.vehiculoService = vehiculoService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Dashboard()
        {
            var vehiculos = await vehiculoService.GetAllViewModel();
            var reservas = await reservaService.GetAllViewModel();
            var usuarios = await userService.GetAllUser();
            ViewBag.Reservas = await reservaService.GetAllViewModel();
            ViewBag.Usuarios = await userService.GetAllUser();
            ViewBag.Vehiculos = await vehiculoService.GetAllViewModel();
            DashboardViewModel dashboardView = new DashboardViewModel();
            dashboardView.CandidadVehiculos = vehiculos.Count();
            dashboardView.CantidadReserva= reservas.Count();
            dashboardView.cantidadUsuarios = usuarios.Count();
            dashboardView.VehiculosRentados = vehiculos.Where(v => v.Estado == EnumEstados.Rentado.ToString()).Count();
            dashboardView.VehiculosDisponibles = vehiculos.Where(v => v.Estado == EnumEstados.Disponible.ToString()).Count();
            foreach (var rentad in reservas)
            {
                dashboardView.MontoTotalhecho += rentad.PrecioTotal;
            }

            return View(dashboardView);
        }

        public IActionResult Ubicacion()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
