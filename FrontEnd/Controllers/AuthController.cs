using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        public async Task<IActionResult> Login()
        {
            return await Task.Run(()=>View("Login"));
        }
        public async Task<IActionResult> LoginRequest(LoginRequestViewModel model)
        {
            if(await _authServices.Login(model))
            {
                TempData["Success"] = "Logged in Successfully!";
                return RedirectToAction("Index","Home");
            }
            TempData["Failure"] = "Invalid Email or Password please Retry!";
            return RedirectToAction("Login", "Auth");
        }
        public async Task<IActionResult> Details()
        {
            var model = await _authServices.Details();
            if (model != null)
            {
                return View(model);
            }
            return View("~/Views/Shared/Error.cshtml");
        }
        public async Task<IActionResult> Logout()
        {
            _authServices.Logout();
            TempData["Success"] = "Logged Out Successfully!";
            return RedirectToAction("Index", "Home");
        } 
        public async Task<IActionResult> Form()
        {
            var model = await _authServices.Details();
            return View(model);
        }
        public async Task<IActionResult> Update(UserViewModel model)
        {
            if (await _authServices.Update(model))
            {
                return RedirectToAction("Details", "Auth");
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AllCustomers()
        {
            var model = await _authServices.GetCustomers();
            if (model == null) {
                return RedirectToAction("Index","Home");
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();   
        }
        public async Task<IActionResult> Create(RegistrationViewModel model)
        {
            if(await _authServices.create(model))
            {
                TempData["Success"] = "User Registered Successfully!";
                return RedirectToAction("Index", "Home");
            }
            TempData["Failure"] = "Please Register again Registration Failed!";
            return RedirectToAction("Index", "Home");
        }
    }
}
