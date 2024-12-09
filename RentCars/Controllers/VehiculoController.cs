using Core.Application.Enums;
using Core.Application.Helpers;
using Core.Application.Interfaces.Services;
using Core.Application.ViewModel.Vehiculo;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RentCars.Controllers
{
    
    public class VehiculoController : Controller
    {
        private readonly IVehiculoService vehiculoService;

        public VehiculoController(IVehiculoService vehiculoService)
        {
            this.vehiculoService = vehiculoService;
        }
      
        public async Task<ActionResult> Index(string categoria, string combustible, string transmision, decimal? precio, int? numPersonas)
        {
            // Obtener todos los vehículos
            var result = await vehiculoService.GetAllViewModel();

            // Filtrar por Categoría si se proporciona
            if (!string.IsNullOrEmpty(categoria))
            {
                result = result.Where(v => v.Categoria!.Equals(categoria, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filtrar por Combustible si se proporciona
            if (!string.IsNullOrEmpty(combustible))
            {
                result = result.Where(v => v.Combustible!.Equals(combustible, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filtrar por Transmisión si se proporciona
            if (!string.IsNullOrEmpty(transmision))
            {
                result = result.Where(v => v.Transmision!.Equals(transmision, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filtrar por Precio máximo si se proporciona
            if (precio.HasValue)
            {
                result = result.Where(v => v.PrecioPorDia <= precio.Value).ToList();
            }

            // Filtrar por Número de Personas si se proporciona
            if (numPersonas.HasValue)
            {
                result = result.Where(v => v.NumeroPersonas == numPersonas.Value).ToList();
            }

            // Retornar la vista con los resultados filtrados
            return View(result.Where(v => v.Estado == EnumEstados.Disponible.ToString()));
        }
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> IndexDashboard()
        {
            return View(await vehiculoService.GetAllViewModel());
        }

        // GET: VehiculoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(await vehiculoService.GetByIdSaveViewModel(id));
        }
        [Authorize(Roles = "Admin")]


        // GET: VehiculoController/Create
        public ActionResult Create()
        {


            return View(new SaveVehiculoViewModel { Estado = EnumEstados.Disponible.ToString()});
        }

        // POST: VehiculoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveVehiculoViewModel collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                
                SaveVehiculoViewModel saveVehiculo = await vehiculoService.Add(collection);


                if (saveVehiculo != null && saveVehiculo.VehiculoId != 0)
                {
                    saveVehiculo.ImagenVehiculo = FileHelped.UploadFile(collection.File!, saveVehiculo.VehiculoId, false, "Vehiculo");
                   
                    await vehiculoService.Update(saveVehiculo, saveVehiculo.VehiculoId);
                }

                return RedirectToAction(nameof(IndexDashboard));
            }
            catch
            {
                return View();
            }
        }

        // GET: VehiculoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View(await vehiculoService.GetByIdSaveViewModel(id));
        }

        // POST: VehiculoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, SaveVehiculoViewModel collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(collection);
                }

                var entidad = await vehiculoService.GetByIdSaveViewModel(id);
                if (entidad != null)
                {
                    collection.VehiculoId = entidad.VehiculoId;
                    collection.ImagenVehiculo = FileHelped.UploadFile(collection.File!, id, true, "Vehiculo", entidad!.ImagenVehiculo!);
                    collection.Estado = entidad.Estado;
                }
                await vehiculoService.Update(collection, id);
                return RedirectToAction(nameof(IndexDashboard));
            }
            catch
            {
                return View();
            }
        }

        // GET: VehiculoController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(await vehiculoService.GetByIdSaveViewModel(id));
        }

        // POST: VehiculoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, SaveVehiculoViewModel collection)
        {
            try
            {
                await vehiculoService.Delete(id);
                FileHelped.FileDelete("Vehiculo", id);
                return RedirectToAction(nameof(IndexDashboard));
            }
            catch
            {
                return View();
            }
        }
    }
}
