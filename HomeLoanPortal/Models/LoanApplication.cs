using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.Models
{
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

    public class LoanApplication
    {
        public int Id { get; set; }

        // Application number generated at final submit (e.g. APP20250001)
        public string? ApplicationNumber { get; set; }

        // Link to user
        public string ApplicantId { get; set; }
        public ApplicationUser Applicant { get; set; }

        // PERSONAL DETAILS (Step 7A)
        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }

        public string AadharNumber { get; set; }

        public string PanNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        // INCOME / PROPERTY / LOAN etc. fields are added later (Step 7B/7C)
        // For relationships
        public ICollection<LoanDocument> Documents { get; set; }
        public ICollection<LoanStatusHistory> StatusHistory { get; set; }
        public LoanAccount LoanAccount { get; set; }

        public LoanApplicationStatus Status { get; set; } = LoanApplicationStatus.Draft;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedOn { get; set; }
        public DateTime? VerificationAppointmentDate { get; set; }

        //Income & Property Details

        // Step 7B fields
        public string? EmploymentType { get; set; }
        public int? RetirementAge { get; set; }
        public string? EmployerName { get; set; }
        public decimal? MonthlyIncome { get; set; }

        public string? PropertyLocation { get; set; }
        public string? PropertyName { get; set; }
        public decimal? EstimatedPropertyValue { get; set; }

        //7C — Loan Details Page

        public decimal? LoanAmountRequired { get; set; }
        public int? LoanTenureMonths { get; set; }
        public decimal? InterestRate { get; set; } = 8.5m;  // default



    }
}
