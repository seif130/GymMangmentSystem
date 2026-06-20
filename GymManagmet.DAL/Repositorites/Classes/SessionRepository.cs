using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<Session> query = _dbContext.Sessions
                .AsNoTracking()
                .Include(s => s.Trainer)
                .Include(s => s.Category);

            if (predicate is not null) query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
           return await _dbContext.Bookings.AsNoTracking().CountAsync(s => s.SessionId == sessionId);
        }

        public async Task<Session?> GetSessionByIdWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
        {
           return await _dbContext.Sessions.AsNoTracking().Include(x => x.Trainer).Include(x =>x.Category).FirstOrDefaultAsync(x=>x.Id == sessionId);
        }
    }
}
