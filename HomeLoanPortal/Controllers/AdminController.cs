using HomeLoanPortal.Data;
using HomeLoanPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeLoanPortal.ViewModels;


namespace HomeLoanPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var list = await _db.LoanApplications.ToListAsync();

            ViewBag.Total = list.Count;
            ViewBag.Approved = list.Count(a => a.Status == LoanApplicationStatus.Approved);
            ViewBag.Rejected = list.Count(a => a.Status == LoanApplicationStatus.Rejected);
            ViewBag.Pending = list.Count(a => a.Status == LoanApplicationStatus.SentForVerification);

            return View();
        }



        // All applications
        public async Task<IActionResult> Applications()
        {
            var list = await _db.LoanApplications
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            return View(list);
        }

        // Application Details
        public async Task<IActionResult> Details(int id)
        {
            var loan = await _db.LoanApplications
                .Include(l => l.Documents)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            return View(loan);
        }

        // Update Status POST
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, LoanApplicationStatus status)
        {
            var loan = await _db.LoanApplications.FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            loan.Status = status;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }


        /// <summary>
        /// Add Reports Action in AdminController
        /// </summary>
      

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reports()
        {
            var applications = await _db.LoanApplications.ToListAsync();

            // Build values for charts
            ViewBag.Total = applications.Count;
            ViewBag.Approved = applications.Count(a => a.Status == LoanApplicationStatus.Approved);
            ViewBag.Rejected = applications.Count(a => a.Status == LoanApplicationStatus.Rejected);
            ViewBag.Verified = applications.Count(a => a.Status == LoanApplicationStatus.Verified);
            ViewBag.Pending = applications.Count(a => a.Status == LoanApplicationStatus.SentForVerification);
            ViewBag.Draft = applications.Count(a => a.Status == LoanApplicationStatus.Draft);

            return View();
        }


    }
}
