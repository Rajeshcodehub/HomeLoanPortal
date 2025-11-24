using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class EMIViewModel
    {
        [Required(ErrorMessage = "Enter loan amount")]
        [Range(1, 1000000000, ErrorMessage = "Enter a valid loan amount")]
        [Display(Name = "Loan Amount (₹)")]
        public decimal LoanAmount { get; set; }

        [Required(ErrorMessage = "Enter tenure in months")]
        [Range(1, 480, ErrorMessage = "Enter a tenure between 1 and 480 months")]
        [Display(Name = "Tenure (months)")]
        public int TenureMonths { get; set; }

        [Required(ErrorMessage = "Interest rate is required")]
        [Range(0.01, 100.0, ErrorMessage = "Enter a valid interest rate")]
        [Display(Name = "Interest Rate (annual %)")]
        public decimal InterestRateAnnual { get; set; } = 8.5m; // default 8.5%

        // Result
        public decimal? MonthlyEMI { get; set; }

        // Helper to show monthly rate decimal (optional)
        public decimal MonthlyRate => InterestRateAnnual / 100m / 12m;
    }
}
