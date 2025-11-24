using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class DocumentUploadViewModel
    {
        public int LoanApplicationId { get; set; }

        // Required documents
        [Required]
        public IFormFile AadharCard { get; set; }

        [Required]
        public IFormFile PanCard { get; set; }

        [Required]
        public IFormFile SalarySlip { get; set; }

        [Required]
        public IFormFile NOC { get; set; }

        [Required]
        public IFormFile LOA { get; set; }

        [Required]
        public IFormFile AgreementToSale { get; set; }
    }
}
