using GymMangment.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);
        Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<bool> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
        Task<bool> RemoveSessionAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);
    }
}
