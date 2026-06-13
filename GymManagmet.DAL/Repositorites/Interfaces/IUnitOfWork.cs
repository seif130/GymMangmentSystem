using GymManagmet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Interfaces
{
    public interface IUnitOfWork
    {
         IgenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity , new();

        Task<int> SaveChangesAsync(CancellationToken ct = default);

       public ISessionRepository sessionRepository { get; }
    }
}
