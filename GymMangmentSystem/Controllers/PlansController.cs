
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GymMangmentSystem.Controllers
{
    public class PlansController : Controller
    {

   
            private readonly IPlanServices _planService;

            public PlansController(IPlanServices planService)
            {
                _planService = planService;
            }

            public async Task<IActionResult> Index(CancellationToken ct)
                => View(await _planService.GetAllPlansAsync(ct));

            [HttpGet]
            public async Task<IActionResult> Details(int id, CancellationToken ct)
            {
                var plan = await _planService.GetPlanByIdAsync(id, ct);
                if (plan is null)
                {
                    TempData["ErrorMessage"] = "Plan not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(plan);
            }


        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id, ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan cannot be edited (not found, inactive, or has active memberships).";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _planService.UpdatePlanAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Plan updated successfully.";
            else 
                TempData["ErrorMessage"] = "Failed to update plan. It may be inactive or have active memberships.";
            return RedirectToAction(nameof(Index));
            
          
        }

        #endregion



        [HttpPost]
            public async Task<IActionResult> Activate(int id, CancellationToken ct)
            {
                var result = await _planService.ToggleActivationAsync(id, ct);
               if(result)
                TempData["SuccessMessage"] = "Plan activation status toggled successfully.";
            else
                TempData["ErrorMessage"] = "Failed to toggle plan activation. It may have active memberships.";
            return RedirectToAction(nameof(Index));
        }
        }
    }

