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

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member == null) 
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetHealthRecordDetailsByIdAsync(id, ct);
            if(result == null)
            {
                TempData["ErrorMessage"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }


        #region create

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

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, [FromForm] MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(EditMember), model);
            var result = await _memberService.UpdateMemberAsync(id, model, ct);

            if (result) TempData["SuccessMessage"] = "Member updated successfully.";
            else TempData["ErrorMessage"] = "Failed to update member.";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete

        public async Task<IActionResult> DeleteMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);
            if (result) TempData["SuccessMessage"] = "Member deleted successfully.";
            else TempData["ErrorMessage"] = "Failed to delete member.";
            return RedirectToAction(nameof(Index));
        }


        #endregion


    }
}
