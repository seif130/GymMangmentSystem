using GymManagmet.DAL.Repositorites.Classes;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.Models;
using Microsoft.AspNetCore.Mvc;


namespace GymMangmentSystem.Controllers
{
    public class PlansController : Controller
    {

        private readonly IgenericRepository<Plan> planRepository;

        public PlansController(IgenericRepository<Plan> planRepository)
        {
            this.planRepository = planRepository;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIDAsync(id, ct: ct);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }
    }
}
