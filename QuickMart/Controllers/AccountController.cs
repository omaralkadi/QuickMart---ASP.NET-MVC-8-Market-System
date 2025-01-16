using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using QuickMart.Helpers;
using QuickMart.ViewModels;

namespace QuickMart.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerVM.FName + registerVM.LName,
                    Email = registerVM.Email,
                    FirstName = registerVM.FName,
                    LastName = registerVM.LName,
                    FullName = registerVM.FName + " " + registerVM.LName,
                    PhoneNumber = registerVM.Phone,
                    Address = registerVM.Address
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "Customer");
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(registerVM);
        }

        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(loginVM.Email);
                if (User is not null)
                {
                    var result = await _signInManager.PasswordSignInAsync(User, loginVM.Password, loginVM.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(loginVM);
        }


        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var User = await _userManager.FindByEmailAsync(model.Email);
            if (User is not null)
            {
                var Token = await _userManager.GeneratePasswordResetTokenAsync(User);
                var url = Url.Action("ResetPassword", "Account", new { email = model.Email, token = Token }, Request.Scheme);

                var email = new DAL.Entities.Email
                {
                    Subject = "Reset Password",
                    Body = url,
                    Recipient = model.Email
                };

                await SendEmail.send(email);

                return RedirectToAction(nameof(CheckYourInBox));
            }

            ModelState.AddModelError("", "Email Does not Exist");
            return View();
        }

        public async Task<IActionResult> CheckYourInBox()
        {
            return View();
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            var email = TempData["email"] as string;
            var token = TempData["token"] as string;
            if (!ModelState.IsValid) { return View(model); }


            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(PasswordChangedSuccessfully));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ModelState.AddModelError("", "Email Does not Exist");

            return View(model);
        }
        public async Task<IActionResult> PasswordChangedSuccessfully()
        {
            return View();
        }

    }
}