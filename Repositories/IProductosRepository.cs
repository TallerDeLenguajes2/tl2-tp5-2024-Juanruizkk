using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaAPI.Models;

namespace TiendaAPI.Repositories
{
    public interface IProductosRepository
    {
        Task AddAsync(Productos producto);
        Task UpdateAsync(int idProd, Productos producto);
        Task<List<Productos>> GetAllAsync();
        Task<Productos> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}