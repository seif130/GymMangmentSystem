using GymManagmet.DAL.Models;
using GymManagmet.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Interfaces
{
    public interface IgenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = false, CancellationToken ct = default);
        Task<TEntity?> GetByIDAsync(int id, CancellationToken ct = default);

        void Add(TEntity entity);

        void Update(TEntity entity);

         void Delete(TEntity entity);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate , bool tracking = false, CancellationToken ct = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default);
    }
}
