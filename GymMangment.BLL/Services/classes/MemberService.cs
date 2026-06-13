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
        private readonly IUnitOfWork _unitofwork;

        public MemberService(IUnitOfWork unitOfWork)
        {
           _unitofwork = unitOfWork;
        }



        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitofwork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            List<MemberViewModel> memberViewModels = new List<MemberViewModel>();

            foreach (var member in members)
            {
                var memberViewModel = new MemberViewModel
                {
                    Id = member.Id,
                    Name = member.Name,
                    Photo = member.Photo,
                    Email = member.Email,
                    Phone = member.Phone,
                    Gender = member.Gender.ToString()
                };
                memberViewModels.Add(memberViewModel);
            }
            return memberViewModels;
        }


        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            var emailExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email , ct: ct);
            var phoneExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct: ct);

            if (emailExists || phoneExists) return false; // Email or phone already exists

            var member = new Member ()
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
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,

                }

            };
            _unitofwork.GetRepository<Member>().Add(member);
            var result = await _unitofwork.SaveChangesAsync();

            return result > 0;

        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(MemberId, ct);
            if (member == null) return null;

            var model = new MemberViewModel ()
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
            
            var activeMembership = await _unitofwork.GetRepository<Membership>().FirstOrDefaultAsync(m => m.MemberId == MemberId && m.EndDate > DateTime.Now);

            if(activeMembership != null)
            {
              var ActivePlan = await _unitofwork.GetRepository<Plan>().GetByIDAsync(activeMembership.PlanId, ct);
                model.PlanName = ActivePlan?.Name;
                model.MemberShipStartDate = activeMembership.CreatedAt.ToShortDateString();
                model.MembershipEndDate = activeMembership.EndDate.ToShortDateString();

            }
            return model;
        }

        public async Task<HealthRecordViewModel?> GetHealthRecordDetailsByIdAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitofwork.GetRepository<HealthRecord>().FirstOrDefaultAsync(hr => hr.MemberId == memberId, ct: ct);
            if(record == null) return null;
            else
               return new HealthRecordViewModel()
            {
                Height = record.Height,
                Weight = record.Weight,
                BloodType = record.BloodType,
                Note = record.Note
            };

        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(memberId, ct);

            if (member == null) return null;
            else 
                return new MemberToUpdateViewModel()
                {
                    Name = member.Name,
                    Email = member.Email,
                    Phone = member.Phone,
                    BuildingNumber = member.Address?.BuildingNumber ?? 0,
                    City = member.Address?.City ?? string.Empty,
                    Street = member.Address?.Street ?? string.Empty,
                    Photo = member.Photo
                };
        }

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
          var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(id, ct);

            if (member == null) return false;

            var emailExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct: ct);
            var phoneExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct: ct);
            if (emailExists || phoneExists) return false; 

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;
            
            
            _unitofwork.GetRepository<Member>().Update(member);
            var result = await _unitofwork.SaveChangesAsync();
            return result > 0;

        }

        public async Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
         var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(id, ct);
            if(member == null) return false;
            
            var hasactivebokking = await _unitofwork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct: ct);
            if (hasactivebokking) return false;

            _unitofwork.GetRepository<Member>().Delete(member);
            var result = await _unitofwork.SaveChangesAsync();
           return result > 0;
        }
    }
}