
namespace Core.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T t);
        Task DeleteAsync(T t);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task UpdateAsync(T t, int id);
    }
}