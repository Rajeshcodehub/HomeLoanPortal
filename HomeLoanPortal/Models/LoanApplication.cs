using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.Models
{
    public class LoanApplication
    {
        [Key]
        public int Id { get; set; }

        // Generated application identifier shown to user (e.g. "APP20250001")
        public string ApplicationNumber { get; set; }

        // Link to the applicant (user)
        public string ApplicantId { get; set; }
        public ApplicationUser Applicant { get; set; }

        // Personal details (duplicate snapshot at time of application)
        public string ApplicantFullName { get; set; }
        public DateTime? ApplicantDOB { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }

        // Income & Employment / Property details (from Outline)
        public string EmploymentType { get; set; }     // Salaried / Self-employed
        public int? RetirementAge { get; set; }
        public string EmployerName { get; set; }
        public decimal? MonthlyIncome { get; set; }

        public string PropertyLocation { get; set; }
        public string PropertyName { get; set; }
        public decimal? EstimatedPropertyValue { get; set; }

        // Loan details
        public decimal? RequestedLoanAmount { get; set; }
        public int? TenureMonths { get; set; }
        public decimal InterestRate { get; set; } = 8.5m; // fixed as per doc

        // Status / workflow
        public LoanApplicationStatus Status { get; set; }
        public DateTime SubmittedOn { get; set; }
        public DateTime? VerificationAppointmentDate { get; set; }

        // Uploaded documents & relations
        public ICollection<LoanDocument> Documents { get; set; }

        // After approval, account created
        public LoanAccount LoanAccount { get; set; }

        // Optional: track status history
        public ICollection<LoanStatusHistory> StatusHistory { get; set; }
    }

    public enum LoanApplicationStatus
    {
        Draft,
        Submitted,
        SentForVerification,
        Verified,
        SentForFinalApproval,
        Approved,
        Rejected
    }
}
