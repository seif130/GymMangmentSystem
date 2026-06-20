using GymManagmet.DAL.Models;
using GymMangment.BLL.Common;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymMangment.PL.Controllers
{
    public class SessionController : Controller
    {

        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);

        }


        [HttpGet]
        public async Task<IActionResult> Details (int id , CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id , ct);

            if(result.Success)
                return View(result.Value);
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }



        #region Create
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdownsAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(model);
            }
      

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Created.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.error;
            await PopulateDropdownsAsync(ct);
            return View(model);

        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);


            if (result.Success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(result.Value);
            }
               
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }

        }
            
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model , CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }

            var result = await _sessionService.UpdateSessionAsync(id,model,ct);

            if (result.success)
            {
                TempData["SucessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));
            }

            else
            {
                TempData["ErrorMessage"] = result.error;
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }

        }
        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionByIdAsync(id, ct);
            if (session.Success)
            {
                
                return View(session.Value);
            }
            else
            {
                TempData["ErrorMessage"] = session.Error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Deleted";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion



        private async Task PopulateDropdownsAsync(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropDownAsync(ct), "Id", "CategoryName");
        }

    }
}

