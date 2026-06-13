using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.DbContexts;
using Microsoft.EntityFrameworkCore;
using GymManagmet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace GymManagmet.DAL.Repositorites.Classes
{
    public class GenericRepository<TEntity> : IgenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext dbContext;
        private readonly DbSet<TEntity> dbSet;

        public GenericRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TEntity>();
        }


        public async void Add(TEntity entity)
        {
            dbSet.Add(entity);

        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,bool tracking = false, CancellationToken ct = default)
        {
           return dbSet.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
          
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
           IQueryable<TEntity> query = tracking ? dbSet : dbSet.AsNoTracking();
           return query.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? dbSet : dbSet.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIDAsync(int id, CancellationToken ct = default)
        {
            return await dbSet.FindAsync(id, ct);
        }

        public async void Update(TEntity entity)
        {
            dbSet.Update(entity);
          
        }

    }
}
