using AutoMapper;
using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymMangment.BLL.Common;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Services.classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TrainerService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct: ct))
                return Result.Fail("A trainer with this email already exists.");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct: ct))
                return Result.Fail("A trainer with this phone number already exists.");

            var trainer = _mapper.Map<Trainer>(model);

            _unitOfWork.GetRepository<Trainer>().Add(trainer);
           var result = await _unitOfWork.SaveChangesAsync(ct: ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Trainer");
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct: ct);
            return trainer is null ? null : _mapper.Map<TrainerViewModel>(trainer);
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
           var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct: ct);
         
                return trainer is null ? null : _mapper.Map<TrainerToUpdateViewModel>(trainer);

        }

        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
          var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct: ct);
            if(trainer == null) return Result.NotFound("Trainer not found.");
            var hasfutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct: ct);
            if (hasfutureSessions) return Result.Fail("Cannot delete a trainer with upcoming sessions.");


            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
           var result= await _unitOfWork.SaveChangesAsync(ct: ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Trainer");
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
           var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct: ct);
            if (trainer == null) Result.NotFound("Trainer not found.");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct: ct))
                return Result.Fail("Another trainer is already using this email.");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct: ct))
                return Result.Fail("Another trainer is already using this phone number.");


            _mapper.Map(model, trainer);
            trainer.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct: ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Trainer");
        }
    }
}
