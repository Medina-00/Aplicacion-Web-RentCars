using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Interfaces.Services;
using Infrastructure.Identity.Entities;
using Core.Application.ViewModel.User;
using Core.Application.Dtos.Account.Response;
using Core.Application.Dtos.Account;
using Core.Application.Helpers;
using Core.Application.Services;
using Core.Application.ViewModel.Vehiculo;


namespace RentCars.Controllers
{
    
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<ApplitationUser> userManager;

        public UserController(IUserService userService, UserManager<ApplitationUser> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }
        // GET: UserController
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginView)
        {
            if (!ModelState.IsValid)
            {
                return View(loginView);
            }
            AuthenticationResponse authentication = await userService.LoginAsync(loginView);
            if (authentication != null && authentication.HasError != true)
            {
                if(authentication.Roles.Contains("Admin"))
                {
                    HttpContext.Session.Set<AuthenticationResponse>("user", authentication);
                    return RedirectToRoute(new { controller = "Home", action = "Dashboard" });
                }
                HttpContext.Session.Set<AuthenticationResponse>("user", authentication);
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            else
            {
                loginView.HasError = authentication!.HasError;
                loginView.Error = authentication.Error;
                return View(loginView);
            }

            
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            // Obtener el usuario actualmente autenticado
            var currentUser = await userManager.GetUserAsync(User);

            var data = await userService.GetAllUser();
            return View(data.Where(u => u.UserId != currentUser!.Id));
        }

        
        // GET: UserController/Create
        public ActionResult Create()
        {
            return View(new SaveUserViewModel ());
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveUserViewModel saveUser)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    return View(saveUser);
                }
                var origin = Request.Headers["origin"];
                RegisterResponse request = await userService.RegisterAsync(saveUser , origin!);

                if (request.HasError)
                {
                    saveUser.HasError = request!.HasError;
                    saveUser.Error = request.Error;
                    return View(saveUser);
                }
                return RedirectToRoute(new { controller = "User", action = "Login" });
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            string response = await userService.ConfirmEmailAsync(userId, token);
            return View("ConfirmEmail", response);
        }

        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(forgotPasswordViewModel);
                }
                var origin = Request.Headers["origin"];
                ForgotPasswordReponse request = await userService.ForgotPasswordAsync(forgotPasswordViewModel, origin!);

                if (request.HasError)
                {
                    forgotPasswordViewModel.HasError = request!.HasError;
                    forgotPasswordViewModel.Error = request.Error;
                    return View(forgotPasswordViewModel);
                }
                return RedirectToRoute(new { controller = "User", action = "Login" });
            }
            catch
            {
                return View();
            }

        }
        public ActionResult ResetPassword(string token)
        {
            return View(new ResetPasswordViewModel { Token = token});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(resetPasswordViewModel);
                }
                ResetPasswordReponse request = await userService.ResetPasswordAsync(resetPasswordViewModel);

                if (request.HasError)
                {
                    resetPasswordViewModel.HasError = request!.HasError;
                    resetPasswordViewModel.Error = request.Error;
                    return View(resetPasswordViewModel);
                }
                return RedirectToRoute(new { controller = "User", action = "Login" });
            }
            catch
            {
                return View();
            }

        }
        // GET: UserController/Edit/5
        [Authorize(Roles = "Admin,Usuario")]
        public async Task<ActionResult> Edit( string id)
        {

            var data = await userService.GetById(id , true);
            
            return View(data);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UpdateUserViewModel updateUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(updateUser);
                }
                UpdateUserViewModel updateUserView = await userService.GetById(updateUser.UserId, true);
                var result = await userService.UpdateUserAsync(id, updateUser);
                return RedirectToRoute(new { controller = "User", action = "Index" });
            }
            catch
            {
                return View();
            }
        }
        // GET: VehiculoController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            return View(await userService.GetById(id));
        }

        // POST: VehiculoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, UpdateUserViewModel collection)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                await userManager.DeleteAsync(user!);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<IActionResult> LogOut()
        {
            await userService.SignOutAsync();
            HttpContext.Session.Remove("user");
            return RedirectToRoute(new { controller = "User", action = "Login" });
        }

    }
}
