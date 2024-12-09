

using Core.Application.ViewModel.Vehiculo;
using Core.Domain.Entities;

namespace Core.Application.Interfaces.Services
{
    public interface IVehiculoService : IGenericService<SaveVehiculoViewModel,VehiculoViewModel, Vehiculo>
    {
        Task Cambiarestado(int id, string estado);
    }
}
