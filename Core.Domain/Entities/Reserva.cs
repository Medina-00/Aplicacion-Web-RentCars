

namespace Core.Domain.Entities
{
    public class Reserva
    {
        public int ReservaId { get; set; }
        public string UsuarioId { get; set; }
        public int VehiculoId { get; set; } // Clave foránea

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioTotal { get; set; }

        // Relación  con Vehiculo
        public Vehiculo Vehiculo { get; set; }

    }
}
