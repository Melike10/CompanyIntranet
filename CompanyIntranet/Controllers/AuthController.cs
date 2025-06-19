using CompanyIntranet.Models;
using CompanyIntranet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CompanyIntranet.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {

        
        //SignInManager, giriş yapma, çıkış yapma, cookie oluşturma ve doğrulama işlemlerini yapar.
        private readonly SignInManager<AppUser> _signInManager;
        //UserManager, kullanıcıları oluşturma, silme, güncelleme, parola değiştirme gibi işlemleri sağlar.
        private readonly UserManager<AppUser> _userManager;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("IK"))
                    return RedirectToAction("Index", "Announcement");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Giriş başarısız! Email ya da şifre yanlış.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
