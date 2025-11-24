using HomeLoanPortal.Data;
using HomeLoanPortal.Models;
using HomeLoanPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeLoanPortal.Controllers
{
    public class LoanTrackerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoanTrackerController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Controllers/LoanTrackerController.cs
        [HttpGet]
        public IActionResult Index(string? applicationNumber = null)
        {
            var vm = new LoanTrackerViewModel();

            if (!string.IsNullOrWhiteSpace(applicationNumber))
            {
                vm.ApplicationNumber = applicationNumber;
                // Do not attempt to look up yet; user still must enter DOB.
            }

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Index(LoanTrackerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loan = await _db.LoanApplications
                .Include(x => x.Applicant)
                .FirstOrDefaultAsync(x => x.ApplicationNumber == model.ApplicationNumber);

            if (loan == null)
            {
                ModelState.AddModelError("", "Invalid Application ID.");
                return View(model);
            }

            // Validate DOB
            if (loan.DateOfBirth == null || loan.DateOfBirth.Value.Date != model.DateOfBirth.Date)
            {
                ModelState.AddModelError("", "Date of Birth does not match our records.");
                return View(model);
            }

            // Fill output fields
            model.Status = loan.Status.ToString();
            model.VerificationAppointmentDate = loan.VerificationAppointmentDate;
            model.ApplicantName = $"{loan.FirstName} {loan.LastName}";

            return View(model);
        }
    }
}
