using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymMangment.PL.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _trainerService.GetAllTrainersAsync(ct));



        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _trainerService.CreateTrainerAsync(model, ct);
            if (result)
            { 
                TempData["SuccessMessage"] = "Trainer created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Trainer Failed to create";
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Trainer updated successfully.";
            else
                TempData["ErrorMessage"] = "Trainer Failed to update";

            return RedirectToAction(nameof(Index));
        }

        #endregion


        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _trainerService.RemoveTrainerAsync(id, ct);
            if (result)
                TempData["SucessMessage"] = "Trainer deleted Successfully";
            else
                TempData["ErrorMessaage"] = "failed to delete Trainer";

            return RedirectToAction(nameof(Index));

        }

        #endregion
      
      

    }

}
