using AutoMapper;
using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.Models;
using GymMangment.BLL.Common;
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
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
           var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);

            return _mapper.Map<IEnumerable<PlanViewModel>>(plans);


        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().FirstOrDefaultAsync(p => p.Id == planId, ct: ct);

            if(plan == null)
                return null;

            return plan == null ? null : _mapper.Map<PlanViewModel>(plan);

        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, ct: ct);

            if (plan == null || !plan.IsActive)
                return null;
            if(await HasActiveMembershipsAsync(planId, ct))
                return null;
            else
            return _mapper.Map<UpdatePlanViewModel>(plan);

        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, ct: ct);
            if(plan == null)
                return Result.NotFound("Plan not found.");

            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
                return Result.Fail("Cannot deactivate a plan that has active memberships.");
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Toggle Plan Status");
        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
           var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(id, ct: ct);
            if (plan == null || !plan.IsActive)
                return Result.NotFound("Plan not found.");
            if (await HasActiveMembershipsAsync(id, ct))
                return Result.Fail("Cannot edit a plan that has active memberships.");

            _mapper.Map(model, plan);
             plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
           var result = await  _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Plan");
        }


        //helper method to check if the plan has active memberships
        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.IsActive && m.EndDate > DateTime.Now, ct: ct);
        }
    }
}
