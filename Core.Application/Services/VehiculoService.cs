

using AutoMapper;
using Core.Application.Enums;
using Core.Application.Helpers;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.ViewModel.Vehiculo;
using Core.Domain.Entities;
using InternetBanking.Core.Application.Services;

namespace Core.Application.Services
{
    public class VehiculoService : GenericService< SaveVehiculoViewModel,VehiculoViewModel, Vehiculo>, IVehiculoService
    {
        private readonly IVehiculoRepository vehiculoRepository;
        private readonly IMapper mapper;
        private readonly IReservaRepository reservaRepository;

        public VehiculoService(IVehiculoRepository vehiculoRepository ,IMapper mapper , IReservaRepository reservaRepository):base(vehiculoRepository,mapper)
        {
            this.vehiculoRepository = vehiculoRepository;
            this.mapper = mapper;
            this.reservaRepository = reservaRepository;
        }
        public override async Task<List<VehiculoViewModel>> GetAllViewModel()
        {
            var reservas = await reservaRepository.GetAllAsync(); // Obtener todas las reservas

            var reservasExpiradas = reservas.Where(r => r.FechaFin < DateTime.Now).ToList();

            foreach (var reserva in reservasExpiradas)
            {
                // Obtener el vehículo asociado a la reserva
                var vehiculo = await vehiculoRepository.GetByIdAsync(reserva.VehiculoId);

                // Actualizar la disponibilidad del vehículo a 'true' (disponible)
                if (vehiculo != null)
                {
                    await Cambiarestado(vehiculo.VehiculoId, EnumEstados.Disponible.ToString());
                }
            }
            var entityList = await vehiculoRepository.GetAllAsync();

            return mapper.Map<List<VehiculoViewModel>>(entityList);
        }
        public async Task Cambiarestado(int id, string estado)
        {
            var vehiculo = await vehiculoRepository.GetByIdAsync(id);
            vehiculo.Estado = estado;
            await vehiculoRepository.UpdateAsync(vehiculo,id);
        }

        

    }
}
