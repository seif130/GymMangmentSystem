using GymMangment.BLL.Common;
using GymMangment.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface ImemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);

        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<MemberViewModel?> GetMemberDetailsByIdAsync (int MemberId, CancellationToken ct = default);

        Task<HealthRecordViewModel?> GetHealthRecordDetailsByIdAsync(int memberId, CancellationToken ct = default);

        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync (int memberId, CancellationToken ct = default);

        Task<Result> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);

        public Task<Result> DeleteMemberAsync(int id, CancellationToken ct = default);
    }
}
