using System;
using System.ComponentModel.DataAnnotations;

namespace HomeLoanPortal.ViewModels
{
    public class PersonalDetailsViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        [Display(Name = "Aadhar Number")]
        public string AadharNumber { get; set; }

        [Required]
        [Display(Name = "PAN Number")]
        public string PanNumber { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        // Optional: loan application id when editing an existing draft
        public int? LoanApplicationId { get; set; }
    }
}
