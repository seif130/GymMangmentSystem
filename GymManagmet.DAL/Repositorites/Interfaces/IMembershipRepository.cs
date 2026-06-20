using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Classes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagmet.DAL.Repositorites.Interfaces
{
    public interface IMembershipRepository : IgenericRepository<Membership>
    {
        Task<List<Membership>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate = null, CancellationToken ct = default);

    }
}
