using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class EligibilityViewModel
    {
        [Required]
        [Range(1, 10000000, ErrorMessage = "Enter a valid monthly income.")]
        public decimal MonthlyIncome { get; set; }

        public decimal? EligibleLoanAmount { get; set; }
    }
}
