


using Core.Application.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Reposiroty
{
    public class GenericRepository<T> :  IGenericRepository<T> where T : class
    {
        private readonly ApplicactionContext appContext;

        public GenericRepository(ApplicactionContext appContext)
        {
            this.appContext = appContext;
            this.appContext = appContext;
        }
        public virtual async Task<T> AddAsync(T t)
        {
            await appContext.Set<T>().AddAsync(t);
            await appContext.SaveChangesAsync();
            return t;
        }

        public virtual async Task UpdateAsync(T t, int id)
        {
            var entity = await appContext.Set<T>().FindAsync(id);
            appContext.Entry(entity).CurrentValues.SetValues(t!);
            await appContext.SaveChangesAsync();

        }

        public virtual async Task DeleteAsync(T t)
        {
            appContext.Set<T>().Remove(t);
            await appContext.SaveChangesAsync();
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await appContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await appContext.Set<T>().FindAsync(id);
        }

    }
}
