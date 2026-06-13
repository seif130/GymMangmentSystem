using GymManagmet.DAL.Repositorites.Interfaces;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _UnitOfWork;
        public SessionService(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }


        public Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _UnitOfWork.sessionRepository.GetAllSessionWithTrainerAndCategory(ct);
            if(sessions.Any()) return null;

            if(sessions == null || !sessions.Any()) return null;

            var mapsessions = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                TrainerName = s.Trainer.Name,
                Description = s.Description,
                EndDate = s.EndDate,
                StartDate = s.StartDate,

            });

            foreach (var session in mapsessions)
            {
                session.AvailableSlots = session.Capacity = await _UnitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(session.Id , ct);
            }
            return mapsessions;
        }

        public Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
