using GymManagmet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Interfaces
{
    public interface ISessionRepository :IgenericRepository<Session> 
    {
        Task<IEnumerable<Session>> GetAllSessionWithTrainerAndCategory(CancellationToken ct = default);
        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
    }
}
