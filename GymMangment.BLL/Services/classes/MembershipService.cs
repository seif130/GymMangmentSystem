using AutoMapper;
using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Classes;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.Models;
using GymMangment.BLL.Common;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberShipViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitofwork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateMembershipAsync(CreateMemberShipViewModel model, CancellationToken ct = default)
        {

            var memberExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, ct:ct);
            if (!memberExists) return Result.NotFound("Member not found.");

            var plan = await _unitofwork.GetRepository<Plan>().GetByIDAsync(model.PlanId, ct);
            if (plan is null) return Result.NotFound("Plan not found.");
            if (!plan.IsActive) return Result.Fail("Plan is not active.");


            var hasActive = await _unitofwork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct:ct);
            if (hasActive) return Result.Fail("Member already has an active membership.");

            var entity = _mapper.Map<Membership>(model);

            entity.PlanId = plan.Id;
            entity.CreatedAt = DateTime.Now;
            entity.EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays);

            _unitofwork.MembershipRepository.Add(entity);

            var result = await _unitofwork.SaveChangesAsync(ct);

            return result > 0? Result.Ok(): Result.Fail("Failed To Create New Membership");
        }

        public async Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct = default)
        {
            var active = await _unitofwork.MembershipRepository.FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.Now, tracking: true, ct: ct);

            if (active is null) return Result.NotFound("No active membership for this member.");

            _unitofwork.MembershipRepository.Delete(active);
            var result = await _unitofwork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Membership");
        }

        public async Task<IEnumerable<MemberShipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var memberships = await _unitofwork.MembershipRepository.GetAllMembershipsWithMemberAndPlanAsync(m => m.EndDate > DateTime.Now, ct);
            return _mapper.Map<IEnumerable<MemberShipViewModel>>(memberships);
        }
            
        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitofwork.GetRepository<Member>().GetAllAsync(ct: ct);

            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);

        }

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await _unitofwork.GetRepository<Plan>().GetAllAsync(ct: ct);

            var activePlans = plans.Where(p => p.IsActive);

            return _mapper.Map<IEnumerable<PlanSelectListViewModel>>(activePlans);
          

        }
    }
}
