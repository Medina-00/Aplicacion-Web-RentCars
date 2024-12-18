

namespace Core.Application.ViewModel.Reserva
{
    public class ReservaMapa
    {
        public double Latitud { get; set; }  // Ubicación del vehículo durante la reserva
        public double Longitud { get; set; } // Ubicación del vehículo durante la reserva

        public string Carro { get; set; }
        public string Cliente { get; set; }
    }
}
