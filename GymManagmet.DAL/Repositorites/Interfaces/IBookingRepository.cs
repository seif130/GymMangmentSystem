using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Interfaces
{
    public interface IBookingRepository : IgenericRepository<Booking>
    {

        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default);


    }
}
