using Bl.Model;
using Bl.services;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Extensibility;

namespace ExamApp.Controllers
{
    public class AccountController : Controller
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClsUser _clsUser;

        #endregion
        

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IClsUser clsUser,
         RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _clsUser = clsUser;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserModel userModel)
        {
            try
            {

                var result = _clsUser.GetData(userModel.Email);


                if (result != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult Log = await _signInManager
                                        .PasswordSignInAsync(userModel.Email.Trim() ?? "", userModel.Password.Trim() ?? "", true, true);
                    // Save Cart And Order Review Cookies for User Login
                    if (Log.Succeeded)
                    {
                        if (User.IsInRole("User"))
                            return Redirect("/home/index");
                        else
                            return Redirect("/Admin/home/index");

                    }
                    else
                    {
                        ViewBag.errors = "Please Enter Correct Password";
                        return View("Login", userModel);
                    }

                }
                else
                {
                    ViewBag.errors = "Please Enter Valid Email";
                    return View("Login", userModel);
                }
               

            }
            catch 
            {
                ViewBag.errors = "SomeThing Wrong Happen Please Try Again";
                return View("Login", userModel);
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return Redirect("~/Home/Index");
        }
    }
}
