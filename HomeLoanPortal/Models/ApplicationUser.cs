using Microsoft.AspNetCore.Identity;
using System;

namespace HomeLoanPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }
    }
}
