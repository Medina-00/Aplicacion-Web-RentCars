
using Core.Application.ViewModel.Vehiculo;

namespace Core.Application.ViewModel.Reserva
{
    public class ReservaViewModel
    {
        public int ReservaId { get; set; }
        public string UsuarioId { get; set; }
        public int VehiculoId { get; set; } // Clave foránea

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioTotal { get; set; }

        // Relación  con Vehiculos
        public VehiculoViewModel ?Vehiculos { get; set; }
    }
}
