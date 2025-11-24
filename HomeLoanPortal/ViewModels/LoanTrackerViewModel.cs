using System;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class LoanTrackerViewModel
    {
        [Required]
        [Display(Name = "Application ID")]
        public string ApplicationNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        // OUTPUT FIELDS (make them nullable)
        public string? Status { get; set; }
        public DateTime? VerificationAppointmentDate { get; set; }
        public string? ApplicantName { get; set; }
    }
}
