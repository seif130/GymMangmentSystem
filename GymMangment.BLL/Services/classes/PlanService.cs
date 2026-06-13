using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.Models;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.classes
{
    public class PlanService : IPlanServices
    {

        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
           var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);

            return plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DurationDays = p.DurationDays,
                Price = p.Price,
                IsActive = p.IsActive
            });

        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().FirstOrDefaultAsync(p => p.Id == planId, ct: ct);

            if(plan == null)
                return null;

            return new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, ct: ct);

            if (plan == null || !plan.IsActive)
                return null;
            if(await HasActiveMembershipsAsync(planId, ct))
                return null;
            else
            return new UpdatePlanViewModel()
            {
             
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
        
            };
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, ct: ct);
            if(plan == null)
                return false;

          if(plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
                return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
           var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(id, ct: ct);
            if (plan == null || !plan.IsActive)
                return false;
            if (await HasActiveMembershipsAsync(id, ct))
                return false;

            plan.Name = model.PlanName;
            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
           var result = await  _unitOfWork.SaveChangesAsync();
            return result > 0;
        }


        //helper method to check if the plan has active memberships
        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.IsActive && m.EndDate > DateTime.Now, ct: ct);
        }
    }
}
