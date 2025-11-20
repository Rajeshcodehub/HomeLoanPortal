using System;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.Models
{
    public class LoanDocument
    {
        public int Id { get; set; }

        // e.g. "Aadhar", "PAN", "SalarySlip", "NOC", "LOA", "AgreementToSale"
        [Required]
        public string DocumentType { get; set; }

        // stored filename or path (for now keep as string per code-first; storage strategy later)
        public string FilePath { get; set; }

        public DateTime UploadedOn { get; set; }

        // FK
        public int LoanApplicationId { get; set; }
        public LoanApplication LoanApplication { get; set; }
    }
}
