using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YSharp.Domain;
using YSharp.Models;
using Microsoft.Extensions.Configuration;
using YSharp.Services;
using Microsoft.AspNetCore.Http;
using YSharp.SDK;
using Org.BouncyCastle.Ocsp;

namespace YSharp.Admin.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ILoginLogService _loginLogService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserService _userService;
        private const string FOLDER_NAME = "avatar";
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            ILoginLogService loginLogService,
            IHttpContextAccessor accessor,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _loginLogService = loginLogService;
            _accessor = accessor;
            _userService = userService;
        }

        [TempData]
        public string ErrorMessage
        {
            get; set;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var model = _userService.GetProfile(UserId);
            return View(model);
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Profile(string id)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var errMsg = "";
                _userService.UploadAvatar(UserId, FOLDER_NAME, file, out errMsg);
            }
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException("user does not exist.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }
            var model = new ChangePasswordViewModel { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException("user does not exist");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException("user does not exist");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException("user does not exist");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(SetPassword));
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            var isLoginSuccess = false;
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var loginResult = "";
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        isLoginSuccess = true;
                        loginResult = "Succeeded";
                        return RedirectToLocal(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        isLoginSuccess = false;
                        loginResult = "LockedOut";
                        return RedirectToAction(nameof(Lockout));
                    }
                    if (result.IsNotAllowed)
                    {
                        isLoginSuccess = false;
                        loginResult = "NotAllowed";
                        return RedirectToAction(nameof(AccessDenied));
                    }
                    else
                    {
                        isLoginSuccess = false;
                        loginResult = "Wrong user name or password";
                        ModelState.AddModelError(string.Empty, "Wrong user name or password");
                        return View(model);
                    }
                }
                isLoginSuccess = false;
                loginResult = "Miss username or password";
                return View(model);
            }
            catch (Exception ex)
            {
                loginResult = ex.Message;
                isLoginSuccess = false;
                ModelState.AddModelError(string.Empty, "Occurred error, please contact admin");
                return View(model);
            }
            finally
            {
                var loginUrl = Request.Host.Value;
                var errMsg = "";
                var isSuccess = _loginLogService.Create(new LoginLogViewModel
                {
                    Description = loginResult,
                    IsSuccess = isLoginSuccess,
                    IP = ip,
                    LoginUrl = loginUrl,
                    CreateDate = DateTime.Now,
                    Id = RandomId.CreateEnhance(),
                    UserName = model.UserName,
                }, out errMsg);
                if (isSuccess == false)
                {
                    _logger.LogWarning(string.Format("create login log falied: username={0}, ip={1}, loginUrl={2}, result={3}, description={4}.", model.UserName, ip, loginUrl, isSuccess, loginResult));
                }
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
