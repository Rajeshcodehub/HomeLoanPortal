using System;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.Models
{
    public class LoanAccount
    {
        [Key]
        public int Id { get; set; }

        public string AccountNumber { get; set; }        // generated account number
        public int LoanApplicationId { get; set; }
        public LoanApplication LoanApplication { get; set; }

        public decimal DisbursedAmount { get; set; }
        public DateTime CreatedOn { get; set; }

        // Balance or remaining principal (optional to track)
        public decimal CurrentBalance { get; set; }
    }
}
