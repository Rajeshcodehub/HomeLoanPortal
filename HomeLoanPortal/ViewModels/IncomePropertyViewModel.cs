using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class IncomePropertyViewModel
    {
        public int LoanApplicationId { get; set; }

        // INCOME DETAILS
        [Required]
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }  // Salaried / Self-employed

        [Required]
        [Display(Name = "Retirement Age")]
        public int RetirementAge { get; set; }

        [Required]
        [Display(Name = "Employer Name / Business Name")]
        public string EmployerName { get; set; }

        [Required]
        [Range(1, 100000000)]
        [Display(Name = "Monthly Income (₹)")]
        public decimal MonthlyIncome { get; set; }

        // PROPERTY DETAILS
        [Required]
        [Display(Name = "Property Location")]
        public string PropertyLocation { get; set; }

        [Required]
        [Display(Name = "Property Name")]
        public string PropertyName { get; set; }

        [Required]
        [Range(1, 100000000)]
        [Display(Name = "Estimated Property Value (₹)")]
        public decimal EstimatedPropertyValue { get; set; }
    }
}
