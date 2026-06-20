using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymMangment.PL.Controllers
{
    public class BookingController : Controller
    {
        private readonly IbokkingService _bookingService;

        public BookingController(IbokkingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
          => View(await _bookingService.GetAllSessionsAsync(ct));


        [HttpGet]
        public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken ct)
        => View(await _bookingService.GetMembersForUpcomingBySessionIdAsync(id, ct));

        [HttpGet]
        public async Task<IActionResult> GetMembersForOngoingSessions(int id, CancellationToken ct)
            => View(await _bookingService.GetMembersForOngoingBySessionIdAsync(id, ct));


        #region Create

        [HttpGet]
        public async Task<IActionResult> Create(int id, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForDropDownAsync(id, ct);
            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            var result = await _bookingService.CreateNewBookingAsync(model, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
                result.success ? "Booking created successfully." : result.error;
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = model.SessionId });
        }

        #endregion


        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
                result.success ? "Booking cancelled successfully." : result.error;
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendedAsync(memberId, sessionId, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
                result.success ? "Attendance recorded." : result.error;
            return RedirectToAction(nameof(GetMembersForOngoingSessions), new { id = sessionId });
        }
    }
}
