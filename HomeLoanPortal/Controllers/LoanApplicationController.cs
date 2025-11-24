using HomeLoanPortal.Data;
using HomeLoanPortal.Models;
using HomeLoanPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeLoanPortal.Controllers
{
    [Authorize] // only logged-in users
    public class LoanApplicationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoanApplicationController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ------------------------- NAVIGATION GUARD HELPERS -------------------------

        // Loads the loan only if it belongs to the logged-in user
        private async Task<LoanApplication?> GetUserLoanAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            return await _db.LoanApplications
                .Include(l => l.Documents)
                .FirstOrDefaultAsync(l => l.Id == id && l.ApplicantId == userId);
        }

        // Redirects to correct step
        private IActionResult RedirectToStep(int step, int loanId)
        {
            switch (step)
            {
                case 1: return RedirectToAction("PersonalDetails", new { id = loanId });
                case 2: return RedirectToAction("IncomeDetails", new { id = loanId });
                case 3: return RedirectToAction("LoanDetails", new { id = loanId });
                case 4: return RedirectToAction("UploadDocuments", new { id = loanId });
                case 5: return RedirectToAction("SubmitApplication", new { id = loanId });
                default: return RedirectToAction("PersonalDetails", new { id = loanId });
            }
        }

        // ----------------------- END NAVIGATION GUARD HELPERS -----------------------


        // GET: LoanApplication/PersonalDetails
        [HttpGet]
        public async Task<IActionResult> PersonalDetails(int? id) // id = existing loan application id (edit)
        {
            PersonalDetailsViewModel vm = new PersonalDetailsViewModel();

            if (id.HasValue)
            {
                var loan = await _db.LoanApplications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == id.Value && l.ApplicantId == _userManager.GetUserId(User));

                if (loan == null) return NotFound();

                vm = new PersonalDetailsViewModel
                {
                    LoanApplicationId = loan.Id,
                    FirstName = loan.FirstName,
                    MiddleName = loan.MiddleName,
                    LastName = loan.LastName,
                    DateOfBirth = loan.DateOfBirth,
                    Gender = loan.Gender,
                    Nationality = loan.Nationality,
                    AadharNumber = loan.AadharNumber,
                    PanNumber = loan.PanNumber,
                    PhoneNumber = loan.PhoneNumber,
                    Email = loan.Email
                };
            }
            else
            {
                // Pre-fill email/phone from ApplicationUser if available
                var userId = _userManager.GetUserId(User);
                var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    vm.Email = user.Email;
                    vm.PhoneNumber = user.PhoneNumber;
                }
            }

            return View(vm);
        }

        // POST: LoanApplication/PersonalDetails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonalDetails(PersonalDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            LoanApplication loan;
            if (model.LoanApplicationId.HasValue)
            {
                // Update existing draft
                loan = await _db.LoanApplications.FirstOrDefaultAsync(l => l.Id == model.LoanApplicationId.Value && l.ApplicantId == userId);
                if (loan == null) return NotFound();

                loan.FirstName = model.FirstName;
                loan.MiddleName = model.MiddleName;
                loan.LastName = model.LastName;
                loan.DateOfBirth = model.DateOfBirth;
                loan.Gender = model.Gender;
                loan.Nationality = model.Nationality;
                loan.AadharNumber = model.AadharNumber;
                loan.PanNumber = model.PanNumber;
                loan.PhoneNumber = model.PhoneNumber;
                loan.Email = model.Email;

                _db.LoanApplications.Update(loan);
            }
            else
            {
                // Create new draft record
                loan = new LoanApplication
                {
                    ApplicantId = userId,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Nationality = model.Nationality,
                    AadharNumber = model.AadharNumber,
                    PanNumber = model.PanNumber,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Status = LoanApplicationStatus.Draft,
                    CreatedOn = DateTime.UtcNow
                };

                _db.LoanApplications.Add(loan);
            }

            await _db.SaveChangesAsync();

            // Redirect to next step — Income & Property Details (Step 7B)
            return RedirectToAction("IncomeDetails", new { id = loan.Id });
        }


        // ---------------------- STEP 7B: Income & Property ----------------------

        [HttpGet]
        public async Task<IActionResult> IncomeDetails(int id)
        {
            var loan = await GetUserLoanAsync(id);
            if (loan == null)
                return NotFound();

            // STEP 1 GUARD — Must complete Personal Details first
            if (string.IsNullOrWhiteSpace(loan.FirstName) ||
                string.IsNullOrWhiteSpace(loan.LastName) ||
                loan.DateOfBirth == null ||
                string.IsNullOrWhiteSpace(loan.AadharNumber) ||
                string.IsNullOrWhiteSpace(loan.PanNumber))
            {
                return RedirectToStep(1, id);
            }

            var vm = new IncomePropertyViewModel
            {
                LoanApplicationId = id,
                EmploymentType = loan.EmploymentType,
                RetirementAge = loan.RetirementAge ?? 0,
                EmployerName = loan.EmployerName,
                MonthlyIncome = loan.MonthlyIncome ?? 0,
                PropertyLocation = loan.PropertyLocation,
                PropertyName = loan.PropertyName,
                EstimatedPropertyValue = loan.EstimatedPropertyValue ?? 0
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncomeDetails(IncomePropertyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loan = await _db.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == model.LoanApplicationId && l.ApplicantId == _userManager.GetUserId(User));

            if (loan == null)
                return NotFound();

            // Save fields
            loan.EmploymentType = model.EmploymentType;
            loan.RetirementAge = model.RetirementAge;
            loan.EmployerName = model.EmployerName;
            loan.MonthlyIncome = model.MonthlyIncome;
            loan.PropertyLocation = model.PropertyLocation;
            loan.PropertyName = model.PropertyName;
            loan.EstimatedPropertyValue = model.EstimatedPropertyValue;

            _db.LoanApplications.Update(loan);
            await _db.SaveChangesAsync();

            // Redirect to Step 7C (Loan Details Page)
            return RedirectToAction("LoanDetails", new { id = loan.Id });
        }


        // ---------------------- STEP 7C: Loan Details ----------------------
        [HttpGet]
        public async Task<IActionResult> LoanDetails(int id)
        {
            var loan = await GetUserLoanAsync(id);
            if (loan == null)
                return NotFound();

            // STEP 1 GUARD — Must complete Personal Details
            if (string.IsNullOrWhiteSpace(loan.FirstName) ||
                loan.DateOfBirth == null)
            {
                return RedirectToStep(1, id);
            }

            // STEP 2 GUARD — Must complete Income & Property
            if (string.IsNullOrWhiteSpace(loan.EmploymentType) ||
                loan.MonthlyIncome == null ||
                string.IsNullOrWhiteSpace(loan.PropertyName) ||
                loan.EstimatedPropertyValue == null)
            {
                return RedirectToStep(2, id);
            }

            var vm = new LoanDetailsViewModel
            {
                LoanApplicationId = id,
                LoanAmountRequired = loan.LoanAmountRequired ?? 0,
                LoanTenureMonths = loan.LoanTenureMonths ?? 0,
                InterestRate = loan.InterestRate ?? 8.5m,
                MaxEligibleAmount = loan.MonthlyIncome.HasValue
                    ? 60 * (0.6m * loan.MonthlyIncome.Value)
                    : null
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoanDetails(LoanDetailsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loan = await _db.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == model.LoanApplicationId && l.ApplicantId == _userManager.GetUserId(User));

            if (loan == null)
                return NotFound();

            // Save loan details
            loan.LoanAmountRequired = model.LoanAmountRequired;
            loan.LoanTenureMonths = model.LoanTenureMonths;
            loan.InterestRate = model.InterestRate;

            _db.LoanApplications.Update(loan);
            await _db.SaveChangesAsync();

            // Move to Step 7D (document upload)
            return RedirectToAction("UploadDocuments", new { id = loan.Id });
        }


        // ---------------------- STEP 7D: Document Upload ----------------------

        [HttpGet]
        public async Task<IActionResult> UploadDocuments(int id)
        {
            var loan = await GetUserLoanAsync(id);
            if (loan == null)
                return NotFound();

            // STEP 1 GUARD
            if (string.IsNullOrWhiteSpace(loan.FirstName) || loan.DateOfBirth == null)
                return RedirectToStep(1, id);

            // STEP 2 GUARD
            if (string.IsNullOrWhiteSpace(loan.EmploymentType) ||
                loan.MonthlyIncome == null ||
                string.IsNullOrWhiteSpace(loan.PropertyName))
                return RedirectToStep(2, id);

            // STEP 3 GUARD
            if (loan.LoanAmountRequired == null || loan.LoanTenureMonths == null)
                return RedirectToStep(3, id);

            var vm = new DocumentUploadViewModel { LoanApplicationId = id };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocuments(DocumentUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loan = await _db.LoanApplications
                .FirstOrDefaultAsync(x => x.Id == model.LoanApplicationId && x.ApplicantId == _userManager.GetUserId(User));

            if (loan == null)
                return NotFound();

            // Create folder if not exists
            var root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedDocuments");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            // Helper function
            string SaveFile(IFormFile file, string type)
            {
                var folderPath = Path.Combine(root, model.LoanApplicationId.ToString());
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string uniqueName = $"{type}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(folderPath, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return $"/UploadedDocuments/{model.LoanApplicationId}/{uniqueName}";
            }

            // Save all documents
            var docs = new List<LoanDocument>
    {
        new LoanDocument { LoanApplicationId = loan.Id, DocumentType="AadharCard", FilePath = SaveFile(model.AadharCard, "Aadhar") },
        new LoanDocument { LoanApplicationId = loan.Id, DocumentType="PanCard", FilePath = SaveFile(model.PanCard, "PAN") },
        new LoanDocument { LoanApplicationId = loan.Id, DocumentType="SalarySlip", FilePath = SaveFile(model.SalarySlip, "Salary") },
        new LoanDocument { LoanApplicationId = loan.Id, DocumentType="NOC", FilePath = SaveFile(model.NOC, "NOC") },
        new LoanDocument { LoanApplicationId = loan.Id, DocumentType="LOA", FilePath = SaveFile(model.LOA, "LOA") },
        new LoanDocument { LoanApplicationId = loan.Id, DocumentType="AgreementToSale", FilePath = SaveFile(model.AgreementToSale, "Agreement") }
    };

            await _db.LoanDocuments.AddRangeAsync(docs);
            await _db.SaveChangesAsync();

            // Move to final step
            return RedirectToAction("SubmitApplication", new { id = loan.Id });
        }


        //7E: Final Submission 
        // GET: /LoanApplication/SubmitApplication?id=123
        [HttpGet]
        public async Task<IActionResult> SubmitApplication(int id)
        {
            var loan = await GetUserLoanAsync(id);
            if (loan == null)
                return NotFound();

            // STEP 1 GUARD
            if (string.IsNullOrWhiteSpace(loan.FirstName) || loan.DateOfBirth == null)
                return RedirectToStep(1, id);

            // STEP 2 GUARD
            if (string.IsNullOrWhiteSpace(loan.EmploymentType) ||
                loan.MonthlyIncome == null ||
                string.IsNullOrWhiteSpace(loan.PropertyName))
                return RedirectToStep(2, id);

            // STEP 3 GUARD
            if (loan.LoanAmountRequired == null || loan.LoanTenureMonths == null)
                return RedirectToStep(3, id);

            // STEP 4 GUARD — Must have uploaded documents
            bool hasDocs = await _db.LoanDocuments.AnyAsync(x => x.LoanApplicationId == id);
            if (!hasDocs)
                return RedirectToStep(4, id);

            // All steps passed → show review page
            return View(loan);
        }


        // POST: /LoanApplication/SubmitApplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApplicationConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var loan = await _db.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == id && l.ApplicantId == userId);

            if (loan == null) return NotFound();

            // Basic validation: ensure required sections are filled (you can expand these checks)
            if (string.IsNullOrWhiteSpace(loan.FirstName) ||
                string.IsNullOrWhiteSpace(loan.AadharNumber) ||
                loan.LoanAmountRequired == null ||
                loan.LoanTenureMonths == null)
            {
                ModelState.AddModelError("", "Please ensure Personal, Income/Property and Loan Details are complete before submission.");
                return View("SubmitApplication", loan);
            }

            // Generate unique ApplicationNumber if not already set
            if (string.IsNullOrWhiteSpace(loan.ApplicationNumber))
            {
                loan.ApplicationNumber = await GenerateApplicationNumberAsync();
            }

            loan.Status = LoanApplicationStatus.SentForVerification;
            loan.SubmittedOn = DateTime.UtcNow;

            // Choose an appointment date policy. Here: add 3 business days.
            loan.VerificationAppointmentDate = GetNextBusinessDate(DateTime.UtcNow.AddDays(3));

            _db.LoanApplications.Update(loan);
            await _db.SaveChangesAsync();

            // Redirect to confirmation page
            return RedirectToAction("SubmissionConfirmation", new { id = loan.Id });
        }


        // Generate application number in format APP{YYYY}{00001} using last numeric id
        private async Task<string> GenerateApplicationNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            // Get the last application numeric suffix (safe fallback if none exists)
            var lastApp = await _db.LoanApplications
                .Where(l => l.ApplicationNumber != null && l.ApplicationNumber.StartsWith($"APP{year}"))
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync();

            int nextSequence = 1;
            if (lastApp != null && !string.IsNullOrWhiteSpace(lastApp.ApplicationNumber))
            {
                // ApplicationNumber format: APP{year}{sequence}, try to extract trailing numeric part
                var suffix = lastApp.ApplicationNumber.Substring($"APP{year}".Length);
                if (int.TryParse(suffix, out int lastSeq))
                {
                    nextSequence = lastSeq + 1;
                }
                else
                {
                    // fallback: use max Id + 1
                    var maxId = await _db.LoanApplications.MaxAsync(l => (int?)l.Id) ?? 0;
                    nextSequence = maxId + 1;
                }
            }
            else
            {
                // No previous for this year: find max id and start from 1
                var maxId = await _db.LoanApplications.MaxAsync(l => (int?)l.Id) ?? 0;
                // use 1 or (maxId+1). We'll use 1 for new year
                nextSequence = 1;
            }

            return $"APP{year}{nextSequence.ToString("D5")}"; // APP2025000001
        }

        // Choose next business date utility (skips weekends)
        private DateTime GetNextBusinessDate(DateTime date)
        {
            var next = date.Date;
            while (next.DayOfWeek == DayOfWeek.Saturday || next.DayOfWeek == DayOfWeek.Sunday)
                next = next.AddDays(1);
            return next;
        }

        [HttpGet]
        public async Task<IActionResult> SubmissionConfirmation(int id)
        {
            var userId = _userManager.GetUserId(User);
            var loan = await _db.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == id && l.ApplicantId == userId);

            if (loan == null) return NotFound();

            return View(loan); // Views/LoanApplication/SubmissionConfirmation.cshtml
        }




        // --------------------- MY APPLICATIONS (DASHBOARD) ----------------------

        [HttpGet]
        public async Task<IActionResult> MyApplications(int page = 1, int pageSize = 10)
        {
            var userId = _userManager.GetUserId(User);

            var query = _db.LoanApplications
                .Where(l => l.ApplicantId == userId)
                .OrderByDescending(l => l.Id);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new MyApplicationsViewModel
                {
                    Id = l.Id,
                    ApplicationNumber = l.ApplicationNumber,
                    Status = l.Status.ToString(),
                    LoanAmountRequired = l.LoanAmountRequired,
                    SubmittedOn = l.SubmittedOn,
                    VerificationAppointmentDate = l.VerificationAppointmentDate
                })
                .ToListAsync();

            var vm = new MyApplicationsListViewModel
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> MyApplicationDetails(int id)
        {
            var userId = _userManager.GetUserId(User);

            var loan = await _db.LoanApplications
                .Include(l => l.Documents)
                .FirstOrDefaultAsync(l => l.Id == id && l.ApplicantId == userId);

            if (loan == null) return NotFound();

            return View(loan);
        }



    }
}
