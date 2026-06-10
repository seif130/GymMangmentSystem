using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.Models;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.classes
{
    public class MemberService : ImemberService
    {
        private readonly IgenericRepository<Member> _memberRepository;
        private readonly IgenericRepository<Membership> _memberShipRepository;
        private readonly IgenericRepository<Plan> _planRepository;

        public MemberService(IgenericRepository<Member> memberRepository , IgenericRepository<Membership> memberShipRepository , IgenericRepository<Plan> planRepository)
        {
            _memberRepository = memberRepository;
            _memberShipRepository = memberShipRepository;
            _planRepository = planRepository;
        }



        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            List<MemberViewModel> memberViewModels = new List<MemberViewModel>();

            var memberViewModel = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Photo = m.Photo,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender.ToString()
            });
            memberViewModels.AddRange(memberViewModel);
            return memberViewModels;
        }


        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            var emailExists = await _memberRepository.AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await _memberRepository.AnyAsync(m => m.Phone == model.Phone, ct);

            if (emailExists || phoneExists) return false; // Email or phone already exists

            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,

                }

            };

         var result = await _memberRepository.AddAsync(member);
            return result > 0;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIDAsync(MemberId, ct);
            if (member == null) return null;

            var model = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address?.BuildingNumber}, {member.Address?.Street}, {member.Address?.City}"
           
            };

            var activeMembership = await _memberShipRepository.FirstOrDefaultAsync(m => m.MemberId == MemberId && m.EndDate > DateTime.Now);

            if(activeMembership != null)
            {
              var ActivePlan = await _planRepository.GetByIDAsync(activeMembership.PlanId, ct);
                model.PlanName = ActivePlan?.Name;
                model.MemberShipStartDate = activeMembership.CreatedAt.ToShortDateString();
                model.MembershipEndDate = activeMembership.EndDate.ToShortDateString();

            }
            return model;
        }
    }
}