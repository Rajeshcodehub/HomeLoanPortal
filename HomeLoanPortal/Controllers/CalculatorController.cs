using HomeLoanPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HomeLoanPortal.Controllers
{
    public class CalculatorController : Controller
    {
        // GET: /Calculator/EMI
        [HttpGet]
        public IActionResult EMI()
        {
            var model = new EMIViewModel
            {
                InterestRateAnnual = 8.5m // default per project
            };
            return View(model);
        }

        // POST: /Calculator/EMI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EMI(EMIViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Convert annual rate to monthly decimal rate
            decimal r = model.InterestRateAnnual / 100m / 12m;
            int n = model.TenureMonths;
            decimal P = model.LoanAmount;

            // EMI formula: EMI = P * r * (1+r)^n / ((1+r)^n - 1)
            if (r == 0m)
            {
                // If rate is 0 => EMI = P / n
                model.MonthlyEMI = Math.Round(P / n, 2);
            }
            else
            {
                // Use decimal math carefully
                decimal rPowN = (decimal)Math.Pow((double)(1 + r), n);
                decimal numerator = P * r * rPowN;
                decimal denominator = rPowN - 1;
                decimal emi = numerator / denominator;
                model.MonthlyEMI = Math.Round(emi, 2);
            }

            return View(model);
        }
    }
}
