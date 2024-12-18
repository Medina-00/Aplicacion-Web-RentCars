using Core.Application.Enums;
using Core.Application.Interfaces.Services;
using Core.Application.Services;
using Core.Application.ViewModel.DashBoard;
using Core.Application.ViewModel.Reserva;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        // Exportar los datos a Excel
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Obtener los datos de vehículos y reservas
            var vehiculos = await vehiculoService.GetAllViewModel();
            var reservas = await reservaService.GetAllViewModel();
            var usuarios = await userService.GetAllUser();
            // Crear el archivo Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Resumen");

                // Establecer un estilo de fuente general
                var headerFont = worksheet.Cells[1, 1, 1, 6].Style.Font;
                headerFont.Bold = true;
                headerFont.Size = 12;
                headerFont.Color.SetColor(System.Drawing.Color.White);

                // Encabezados de las métricas generales
                worksheet.Cells[1, 1].Value = "Cantidad de Vehículos";
                worksheet.Cells[1, 2].Value = "Vehículos Disponibles";
                worksheet.Cells[1, 3].Value = "Vehículos Rentados";
                worksheet.Cells[1, 4].Value = "Cantidad de Usuarios";
                worksheet.Cells[1, 5].Value = "Cantidad de Reservas";
                worksheet.Cells[1, 6].Value = "Monto Total Hecho";

                // Establecer color de fondo para los encabezados
                worksheet.Cells[1, 1, 1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);

                worksheet.Cells[2, 1].Value = vehiculos.Count;
                worksheet.Cells[2, 2].Value = vehiculos.Count(v => v.Estado == "Disponible");
                worksheet.Cells[2, 3].Value = vehiculos.Count(v => v.Estado == "Rentado");
                worksheet.Cells[2, 4].Value = usuarios.Count();
                worksheet.Cells[2, 5].Value = reservas.Count;
                worksheet.Cells[2, 6].Value = reservas.Sum(r => r.PrecioTotal);

                // Estilo para los datos de las métricas
                worksheet.Cells[2, 1, 2, 6].Style.Numberformat.Format = "#,##0.00"; // Formato de números

                // *** Sección de Vehículos ***

                // Título para la sección de vehículos
                worksheet.Cells[4, 1].Value = "Datos de Vehículos";
                worksheet.Cells[4, 1, 4, 9].Merge = true; // Unir celdas para el título
                worksheet.Cells[4, 1].Style.Font.Bold = true;
                worksheet.Cells[4, 1].Style.Font.Size = 14;
                worksheet.Cells[4, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Encabezados de los vehículos
                worksheet.Cells[5, 1].Value = "Vehículo";
                worksheet.Cells[5, 2].Value = "Estado";
                worksheet.Cells[5, 3].Value = "Marca";
                worksheet.Cells[5, 4].Value = "Modelo";
                worksheet.Cells[5, 5].Value = "Año";
                worksheet.Cells[5, 6].Value = "Precio por Día";
                worksheet.Cells[5, 7].Value = "Transmisión";
                worksheet.Cells[5, 8].Value = "Combustible";
                worksheet.Cells[5, 9].Value = "Personas";

                // Estilo para los encabezados de vehículos
                worksheet.Cells[5, 1, 5, 9].Style.Font.Bold = true;
                worksheet.Cells[5, 1, 5, 9].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[5, 1, 5, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Estilo de bordes
                var borderStyle = worksheet.Cells[5, 1, 5, 9].Style.Border;
                borderStyle.Top.Style = borderStyle.Bottom.Style = borderStyle.Left.Style = borderStyle.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                for (int i = 0; i < vehiculos.Count; i++)
                {
                    worksheet.Cells[i + 6, 1].Value = vehiculos[i].Marca + " " + vehiculos[i].Modelo;
                    worksheet.Cells[i + 6, 2].Value = vehiculos[i].Estado;
                    worksheet.Cells[i + 6, 3].Value = vehiculos[i].Marca;
                    worksheet.Cells[i + 6, 4].Value = vehiculos[i].Modelo;
                    worksheet.Cells[i + 6, 5].Value = vehiculos[i].Año;
                    worksheet.Cells[i + 6, 6].Value = vehiculos[i].PrecioPorDia;
                    worksheet.Cells[i + 6, 7].Value = vehiculos[i].Transmision;
                    worksheet.Cells[i + 6, 8].Value = vehiculos[i].Combustible;
                    worksheet.Cells[i + 6, 9].Value = vehiculos[i].NumeroPersonas;

                    // Aplicar bordes a cada fila de datos
                    var rowBorder = worksheet.Cells[i + 6, 1, i + 6, 9].Style.Border;
                    rowBorder.Top.Style = rowBorder.Bottom.Style = rowBorder.Left.Style = rowBorder.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // *** Sección de Reservas ***

                // Título para la sección de reservas
                worksheet.Cells[vehiculos.Count + 7, 1].Value = "Datos de Reservas";
                worksheet.Cells[vehiculos.Count + 7, 1, vehiculos.Count + 7, 7].Merge = true; // Unir celdas para el título
                worksheet.Cells[vehiculos.Count + 7, 1].Style.Font.Bold = true;
                worksheet.Cells[vehiculos.Count + 7, 1].Style.Font.Size = 14;
                worksheet.Cells[vehiculos.Count + 7, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[vehiculos.Count + 7, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                worksheet.Cells[vehiculos.Count + 7, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Encabezados de las reservas
                worksheet.Cells[vehiculos.Count + 8, 1].Value = "Reserva";
                worksheet.Cells[vehiculos.Count + 8, 2].Value = "Usuario";
                worksheet.Cells[vehiculos.Count + 8, 3].Value = "Fecha Inicio";
                worksheet.Cells[vehiculos.Count + 8, 4].Value = "Fecha Fin";
                worksheet.Cells[vehiculos.Count + 8, 5].Value = "Precio Total";
                worksheet.Cells[vehiculos.Count + 8, 6].Value = "Cédula";
                worksheet.Cells[vehiculos.Count + 8, 7].Value = "Número Licencia";

                // Estilo para los encabezados de reservas
                worksheet.Cells[vehiculos.Count + 8, 1, vehiculos.Count + 8, 7].Style.Font.Bold = true;
                worksheet.Cells[vehiculos.Count + 8, 1, vehiculos.Count + 8, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[vehiculos.Count + 8, 1, vehiculos.Count + 8, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                for (int i = 0; i < reservas.Count; i++)
                {
                    worksheet.Cells[vehiculos.Count + 9 + i, 1].Value = reservas[i].ReservaId;
                    worksheet.Cells[vehiculos.Count + 9 + i, 2].Value = reservas[i].UsuarioId;
                    worksheet.Cells[vehiculos.Count + 9 + i, 3].Value = reservas[i].FechaInicio.ToString("yyyy-MM-dd");
                    worksheet.Cells[vehiculos.Count + 9 + i, 4].Value = reservas[i].FechaFin.ToString("yyyy-MM-dd");
                    worksheet.Cells[vehiculos.Count + 9 + i, 5].Value = reservas[i].PrecioTotal;
                    worksheet.Cells[vehiculos.Count + 9 + i, 6].Value = reservas[i].Cedula;
                    worksheet.Cells[vehiculos.Count + 9 + i, 7].Value = reservas[i].NumeroLincencia;

                    // Aplicar bordes a cada fila de reservas
                    var rowBorder = worksheet.Cells[vehiculos.Count + 9 + i, 1, vehiculos.Count + 9 + i, 7].Style.Border;
                    rowBorder.Top.Style = rowBorder.Bottom.Style = rowBorder.Left.Style = rowBorder.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // Ajustar el tamaño de las columnas
                worksheet.Cells.AutoFitColumns();

                // Convertir el paquete a un arreglo de bytes
                var fileBytes = package.GetAsByteArray();

                // Retornar el archivo Excel como descarga
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Resumen_RentCars.xlsx");
            }

        }
        public IActionResult Ubicacion()
        {
            return View();
        }

        public async Task<IActionResult> UbicacionVehiculo()
        {
            var vehiculos = await vehiculoService.GetAllViewModel();
            var reservas = await reservaService.GetAllViewModel();
            var usuarios = await userService.GetAllUser();

            List<ReservaMapa> reservaMapas = new List<ReservaMapa>();

            foreach (var item in reservas)
            {
                foreach (var item1 in vehiculos)
                {
                    foreach (var item2 in usuarios)
                    {
                        if (item.VehiculoId == item1.VehiculoId && item.UsuarioId == item2.UserId)
                        {
                            reservaMapas.Add(new ReservaMapa
                            {
                                Latitud = item.Latitud,
                                Longitud = item.Longitud,
                                Carro = item1.Marca + " " + item1.Modelo + " " + item1.Año,
                                Cliente = item2.Nombre +" "+item2.Apellido
                            });
                        }
                    }
                   
                }


            }
            return View(reservaMapas);
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
