using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.Db;
using SocialNetwork.Models.ViewModels.Account;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SocialNetwork.Web.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private UserManager<User> _manager;
        private SignInManager<User> _signInManager;

        public AccountManagerController(IMapper mapper, UserManager<User> manager, SignInManager<User> signInManager) {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {

                var user = _mapper.Map<User>(model);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    }
                    else {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View("Views/Home/Index.cshtml");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
