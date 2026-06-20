using GymMangment.BLL.Services.classes;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymMangment.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly ImemberService _memberService;
        private readonly IAttachmentService _AttachmentService;
        public MembersController(ImemberService memberService , IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _AttachmentService = attachmentService;

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

            if (result.success)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = result.error;
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

            if (result.success)
            {
                TempData["SuccessMessage"] = "Member updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.error;
            return View(model);
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
        [HttpGet]

        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id);
            if (member is null || string.IsNullOrEmpty(member.Photo))
                return NotFound();


            var result = _AttachmentService.GetFile(member.Photo, "MembersPictures");
            if (result is null) return NotFound();

            return File(result.Value.Stream, result.Value.ContentType);
        }
    }
}
