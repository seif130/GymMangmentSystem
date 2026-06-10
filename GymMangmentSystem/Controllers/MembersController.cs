using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymMangment.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly ImemberService _memberService;
        public MembersController(ImemberService memberService)
        {
            _memberService = memberService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);

            if (result) TempData["SuccessMessage"] = "Member created successfully.";
            else TempData["ErrorMessage"] = "Failed to create member.";
            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        //{
        //    return View();
        //}

        //public IActionResult HealthRecordDetails(int id, CancellationToken ct)
        //{
        //}
    }
}
