using AutoMapper;
using GymManagmet.DAL.Models;
using GymManagmet.DAL.Models.Enums;
using GymManagmet.DAL.Repositorites.Classes;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymMangment.BLL.Common;
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
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _UnitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be after start date.");
            if(model.StartDate <= DateTime.Now) return Result.Validation("Start date must be in the future.");
            if(model.Capacity < 1 || model.Capacity > 25 ) return Result.Validation("Capacity must be between 1 and 25");

            var trainer = await _UnitOfWork.GetRepository<Trainer>().GetByIDAsync(model.TrainerId);
            if (trainer == null) return Result.NotFound("Trainer not found."); ;

            var category = await _UnitOfWork.GetRepository<Category>().GetByIDAsync(model.CategoryId);
            if (category == null) return Result.NotFound("Category not found.");


            var isValidSpecialty = Enum.TryParse<Speciality>(category.CategoryName, true, out var categorySpecialty);

            if (!isValidSpecialty || trainer.Speciality != categorySpecialty) return Result.Validation("Cannot create this session for this trainer.");


            var session = _mapper.Map<CreateSessionViewModel,Session>(model);

             _UnitOfWork.GetRepository<Session>().Add(session);


            var result = await _UnitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed To create Session");

        }


        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _UnitOfWork.sessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct: ct);


            if (sessions?.Any() != true) return null;

            sessions = sessions.OrderByDescending(X => X.StartDate);
            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in MappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _UnitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return MappedSessions;

        }


        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _UnitOfWork.GetRepository<Category>().GetAllAsync(ct : ct);

            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<Result<SessionViewModel>?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _UnitOfWork.sessionRepository.GetSessionByIdWithTrainerAndCategoryAsync(sessionId , ct);
            if (session == null) return Result<SessionViewModel>.NotFound("Session not found");
            else
            {
                var mappsession = _mapper.Map<SessionViewModel>(session);
                mappsession.AvailableSlots = mappsession.Capacity - await _UnitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(sessionId , ct);
                return Result<SessionViewModel>.Ok(mappsession);
            }
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _UnitOfWork.sessionRepository.GetByIDAsync(sessionId , ct);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound("session not found");

            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Cant update session that has alerady started");

            var bokkingcount = await _UnitOfWork.sessionRepository.GetCountOfBookedSlotsAsync (sessionId , ct);
            if(bokkingcount > 0)
                return Result<UpdateSessionViewModel>.Fail("Cant update session that has alerady bokkings");

            var mappedsession = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedsession);

        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _UnitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);

            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _UnitOfWork.sessionRepository.GetByIDAsync (sessionId , ct);
            if (session == null) return Result.NotFound("session not found");
            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Cant Delete session that has not ended yet");

            var bokkingcount = await _UnitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bokkingcount > 0)
                return Result.Fail("Cant Delete session that has bokkings");

            _UnitOfWork.sessionRepository.Delete(session);

            var result = await _UnitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete session.");

        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _UnitOfWork.sessionRepository.GetByIDAsync(id, ct);
            if (session == null) return Result.NotFound("session not found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cant update session that has alerady started");
            if (model.EndDate <= model.StartDate)
                return Result.Fail("End date must be after start date");

            var bokkingcount = await _UnitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bokkingcount > 0)
                return Result.Fail("Cant update session that has alerady bokkings");

            if (model.StartDate <= DateTime.Now)
                return Result.Fail("start date must be in future");

            var trainer = await _UnitOfWork.GetRepository<Trainer>().GetByIDAsync(model.TrainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer not found");

            var category = await _UnitOfWork.GetRepository<Category>().GetByIDAsync(session.CategoryId, ct);
 
            if (category is null)
                return Result.NotFound("Category not found.");


            var isValidSpecialty = Enum.TryParse<Speciality>(category.CategoryName, true, out var categorySpecialty);

            if (!isValidSpecialty || trainer.Speciality != categorySpecialty)
            {
                return Result.Validation("This trainer does not match the session category.");
            }

            _mapper.Map(model, session);

            session.UpdatedAt = DateTime.Now;

            _UnitOfWork.sessionRepository.Update(session);

            var result = await _UnitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to update session.");

        }
    }
}
