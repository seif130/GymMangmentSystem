using GymManagmet.DAL.Models;
using GymManagmet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Interfaces
{
    public interface IgenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        Task<TEntity?> GetByIDAsync(int id, CancellationToken ct = default);

        Task<int> AddAsync(TEntity entity, CancellationToken ct = default);

        Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default);

        Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default);
    }
}
