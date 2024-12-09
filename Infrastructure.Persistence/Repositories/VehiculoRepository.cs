

using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Reposiroty;

namespace Infrastructure.Persistence.Repositories
{
    public class VehiculoRepository : GenericRepository<Vehiculo> , IVehiculoRepository
    {
        private readonly ApplicactionContext applicactionContext;

        public VehiculoRepository(ApplicactionContext applicactionContext) : base(applicactionContext)
        {
            this.applicactionContext = applicactionContext;
        }

    }
}
