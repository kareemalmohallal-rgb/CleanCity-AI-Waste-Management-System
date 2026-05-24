//using CleanCity.MVC.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace CleanCity.MVC.Controllers
//{
//    public class LoginController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }


//            [HttpPost]
//            public IActionResult Index(LoginViewModel model)
//            {
//                if (!ModelState.IsValid)
//                    return View(model);
//                ModelState.AddModelError("", "اسم المستخدم أو كلمة المرور غير صحيحة");
//                return View(model);
//            }

//    }
//}
using CleanCity.Infrastracture.Identity;
using CleanCity.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanCity.MVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // GET: /Login/Index
        // إذا كان المستخدم مسجل دخول بالفعل (cookie موجودة وصالحة) → الصفحة الرئيسية مباشرة
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        // POST: /Login/Index
        // التحقق من بيانات الدخول وحفظ الـ Cookie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // isPersistent: true → يحفظ الـ Cookie حتى بعد إغلاق المتصفح
            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Input.Username.Trim(),
                password: model.Input.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "اسم المستخدم أو كلمة المرور غير صحيحة");
            return View(model);
        }

        // POST: /Login/Logout
        // حذف الـ Cookie وإعادة التوجيه لصفحة الدخول
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}