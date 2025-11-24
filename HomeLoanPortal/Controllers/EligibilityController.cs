using HomeLoanPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeLoanPortal.Controllers
{
    public class EligibilityController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new EligibilityViewModel());
        }

        [HttpPost]
        public IActionResult Index(EligibilityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Formula from your document:
            // Loan Amount = 60 × (0.6 × net monthly salary)
            model.EligibleLoanAmount = 60 * (0.6m * model.MonthlyIncome);

            return View(model);
        }
    }
}
