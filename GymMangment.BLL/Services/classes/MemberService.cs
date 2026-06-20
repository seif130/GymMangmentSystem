using AutoMapper;
using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.Models;
using GymMangment.BLL.Common;
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
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork , IMapper mapper, IAttachmentService attachmentService)
        {
           _unitofwork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }



        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitofwork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];

             var memberViewModel = _mapper.Map<IEnumerable<Member>,IEnumerable<MemberViewModel>>(members);
    
            return memberViewModel;
        }


        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            var emailExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email , ct: ct);
            var phoneExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct: ct);

            if (emailExists)
                return Result.Validation("A member with this email already exists.");

            if (phoneExists)
                return Result.Validation("A member with this phone number already exists.");


            //upload photo
            var storedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(),model.PhotoFile.FileName, "MembersPhoto");
            if (string.IsNullOrEmpty(storedPhotoName))
                return Result.Fail("Failed To upload photo");

        var member = _mapper.Map<Member>(model);
            member.Photo = storedPhotoName;
            
            _unitofwork.GetRepository<Member>().Add(member);
            var result = await _unitofwork.SaveChangesAsync();
            if (result == 0)
            {
                if (!string.IsNullOrEmpty(member.Photo))
                    _attachmentService.Delete(member.Photo, "members");

                return Result.Fail("Failed To Create Member");
            }
            else

                return Result.Ok();

        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(MemberId, ct);
            if (member == null) return null;

            var model = _mapper.Map<Member,MemberViewModel>(member);
        
            
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
               return _mapper.Map<HealthRecord, HealthRecordViewModel>(record);
          

        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(memberId, ct);

            if (member == null) return null;
            else 
                return _mapper.Map<Member,MemberToUpdateViewModel>(member);
  
        }

        public async Task<Result> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
          var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(id, ct);

            if (member == null) return Result.NotFound();

            var emailExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct: ct);
            var phoneExists = await _unitofwork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct: ct);

            if (emailExists)
                return Result.Validation("A member with this email already exists.");

            if (phoneExists)
                return Result.Validation("A member with this phone number already exists.");

            _mapper.Map(member, model);
      
            member.UpdatedAt = DateTime.Now;
    
            _unitofwork.GetRepository<Member>().Update(member);
            var result = await _unitofwork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Member");

        }

        public async Task<Result> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
         var member = await _unitofwork.GetRepository<Member>().GetByIDAsync(id, ct);
            if(member == null) return Result.NotFound();

            var hasactivebokking = await _unitofwork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct: ct);
            if (hasactivebokking) return Result.Fail("Cannot delete a member with upcoming sessions.");

            _unitofwork.GetRepository<Member>().Delete(member);
            var result = await _unitofwork.SaveChangesAsync();
           return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Member");
        }
    }
}