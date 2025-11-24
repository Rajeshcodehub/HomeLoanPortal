using HomeLoanPortal.Models;
using HomeLoanPortal.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeLoanPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // -------------------- REGISTER (GET) -------------------- //

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        // -------------------- REGISTER (POST) -------------------- //

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Nationality = model.Nationality
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }


            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }

            return View(model);
        }


        // -------------------- LOGIN (GET) -------------------- //

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // -------------------- LOGIN (POST) -------------------- //

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false
            );

            if (result.Succeeded)
                return Redirect(returnUrl ?? "/");

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }


        // -------------------- LOGOUT -------------------- //

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
