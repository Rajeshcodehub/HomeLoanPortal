using Microsoft.AspNetCore.Identity;
using System;

namespace HomeLoanPortal.Models
{
    // Identity user extended with fields from registration/personal details in your docs
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }     // DOB used for Loan Tracker auth
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string AadharNumber { get; set; }       // personal identification
        public string PanNumber { get; set; }
    }
}
