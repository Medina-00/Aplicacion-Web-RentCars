using Core.Application.Enums;
using Core.Application.Interfaces.Services;
using Core.Application.ViewModel.Reserva;
using Core.Application.ViewModel.Vehiculo;
using Core.Domain.Entities;
using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RentCars.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class ReservaController : Controller
    {
        private readonly IReservaService reservaService;
        private readonly UserManager<ApplitationUser> userManager;
        private readonly IVehiculoService vehiculoService;

        public ReservaController(IReservaService reservaService, UserManager<ApplitationUser> userManager , IVehiculoService vehiculoService)
        {
            this.reservaService = reservaService;
            this.userManager = userManager;
            this.vehiculoService = vehiculoService;
        }
        // GET: ReservaController
        public async Task<ActionResult> Index()
        {
            var result = await reservaService.GetAllViewModel();

            var currentUser = await userManager.GetUserAsync(User);
           var vehiculos = await vehiculoService.GetAllViewModel();
          
            ViewBag.Vehiculos = vehiculos;
            return View(result.Where(r => r.UsuarioId == currentUser!.Id));
        }

        // GET: ReservaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            
            return View(await vehiculoService.GetByIdSaveViewModel(id));
        }

        // GET: ReservaController/Create
        public async Task<ActionResult> Create(int id)
        {

            var currentUser = await userManager.GetUserAsync(User);
            var vehiculos = await vehiculoService.GetAllViewModel();
            ViewBag.Vehiculos = vehiculos.Where(v => v.VehiculoId == id);
           var result = await vehiculoService.GetByIdSaveViewModel(id);
            ViewBag.precioTotal = result;
           var vehiculoSelectList = vehiculos.Select(v => new VehiculoSelectListItem
            {
                Id = v.VehiculoId,
                Descripcion = $"{v.Marca} {v.Modelo} {v.Año}",
                precio = v.PrecioPorDia// Concatenar las propiedades
            }).ToList();

           

            if(id == 0)
            {

                return View(new SaveReservaViewModel { UsuarioId = currentUser!.Id , Vehiculos = vehiculoSelectList, PrecioTotal = 0});
            }
            else
            {
                return View(new SaveReservaViewModel {VehiculoId = id, UsuarioId = currentUser!.Id, Vehiculos = vehiculoSelectList , PrecioTotal = 0 });

            }
        }

        // POST: ReservaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveReservaViewModel collection, int id)
        {
            try
            {
               
                if (!ModelState.IsValid)
                {
                    return View(nameof(Create));
                }
                var currentUser = await userManager.GetUserAsync(User);
                await reservaService.Add(collection,currentUser!.Id);
                 await vehiculoService.Cambiarestado(id, EnumEstados.Rentado.ToString());
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        

      
        
    }
}
