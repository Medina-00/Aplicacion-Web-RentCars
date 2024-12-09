

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.ViewModel.Vehiculo
{
    public class SaveVehiculoViewModel
    {
        public int VehiculoId { get; set; }
        public string? Categoria { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Año { get; set; }
        public int PrecioPorDia { get; set; }
        public string? ImagenVehiculo { get; set; }
        public string Estado { get; set; }
        public string? Transmision { get; set; }
        public string? Combustible { get; set; }        // Tipo de combustible (Gasolina, Diesel, Eléctrico, Híbrido)
        public int NumeroPersonas { get; set; }          // Número de plazas disponibles (personas)

        public string? Descripcion { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? File { get; set; }
    }
}
