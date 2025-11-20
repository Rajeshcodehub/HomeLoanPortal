using System;

namespace HomeLoanPortal.Models
{
    public class LoanStatusHistory
    {
        public int Id { get; set; }
        public int LoanApplicationId { get; set; }
        public LoanApplication LoanApplication { get; set; }

        public LoanApplicationStatus Status { get; set; }
        public string ChangedBy { get; set; } // admin username or system
        public DateTime ChangedOn { get; set; }
        public string Notes { get; set; }
    }
}
