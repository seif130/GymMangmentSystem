using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
        public readonly Dictionary<string, object> _repositories = [];

    

        public UnitOfWork(GymDbContext dbContext , ISessionRepository SessionRepository)
        { 
            _dbContext = dbContext;
            sessionRepository = SessionRepository;

        }

        public ISessionRepository sessionRepository { get; }



        public IgenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (_repositories.TryGetValue(typeName, out object value))
            {
                return (IgenericRepository<TEntity>)value;
            }
            else
            {
                var repositoryInstance = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repositoryInstance;
                return repositoryInstance;
            }
          
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _dbContext.SaveChangesAsync(ct);

    }
}
