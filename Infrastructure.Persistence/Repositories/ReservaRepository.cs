

using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Reposiroty;

namespace Infrastructure.Persistence.Repositories
{
    public class ReservaRepository : GenericRepository<Reserva>,IReservaRepository
    {
        private readonly ApplicactionContext applicactionContext;

        public ReservaRepository(ApplicactionContext applicactionContext) : base(applicactionContext)
        {
            this.applicactionContext = applicactionContext;
        }
    }
}
