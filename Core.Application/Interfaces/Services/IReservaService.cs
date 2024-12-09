

using Core.Application.ViewModel.Reserva;
using Core.Domain.Entities;
using System.Security.Claims;

namespace Core.Application.Interfaces.Services
{
    public interface IReservaService : IGenericService<SaveReservaViewModel,ReservaViewModel, Reserva>
    {
        Task<SaveReservaViewModel> Add(SaveReservaViewModel vm , string UserId);
    }
}
