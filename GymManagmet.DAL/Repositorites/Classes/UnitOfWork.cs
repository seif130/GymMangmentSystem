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

        public ISessionRepository sessionRepository { get; }

        public IMembershipRepository MembershipRepository { get; }

        public IBookingRepository BookingRepository { get; }

        public UnitOfWork(GymDbContext dbContext , ISessionRepository SessionRepository , IMembershipRepository membershipRepository , IBookingRepository bookingRepository)
        { 
            _dbContext = dbContext;
            sessionRepository = SessionRepository;
            MembershipRepository = membershipRepository;
            BookingRepository = bookingRepository;

        }

    

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
