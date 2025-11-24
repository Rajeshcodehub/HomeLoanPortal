using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class LoanDetailsViewModel
    {
        public int LoanApplicationId { get; set; }

        [Required]
        [Range(10000, 100000000)]
        [Display(Name = "Loan Amount Required (₹)")]
        public decimal LoanAmountRequired { get; set; }

        [Required]
        [Range(1, 480, ErrorMessage = "Tenure must be between 1 and 480 months")]
        [Display(Name = "Loan Tenure (months)")]
        public int LoanTenureMonths { get; set; }

        [Required]
        [Display(Name = "Interest Rate (%)")]
        public decimal InterestRate { get; set; } = 8.5m;

        // Optional: show auto-calculated eligibility
        public decimal? MaxEligibleAmount { get; set; }
    }
}
