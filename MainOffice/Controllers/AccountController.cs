using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MainOffice.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using MainOffice.App_LocalResources;

namespace MainOffice.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Подтверждение учетной записи");

                    ViewBag.errorMessage = "Вы должны подтвердить адрес электронной почты, чтобы иметь возможность войти " +
                        "Ссылка для подтверждения была отправлена еще раз";
                    return View("Error");
                }
            }

            // Сбои при входе не приводят к блокированию учетной записи
            // Чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неудачная попытка входа.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
            // Если пользователь введет неправильные коды за указанное время, его учетная запись 
            // будет заблокирована на заданный период. 
            // Параметры блокирования учетных записей можно настроить в IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неправильный код.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = "admin")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Locality = "uk-UA" };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Отправка сообщения электронной почты с этой ссылкой
                    await UserManager.AddToRoleAsync(user.Id, "visitor");
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Подтверждение учетной записи");
                    
                    ViewBag.Message = "На указанный адрес было выслано письмо с ссылкой для подтверждения адреса.<br>Для завершения регистрации подтвердите адрес электронной почты.";

                    return View("Info");

                    //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }
        
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Подтвердите вашу учетную запись, щелкнув <a href=\"" + callbackUrl + "\">здесь</a><br> Если вы не знаете по какой причине получили это письмо, удалите его или игнорируйте.");

            return callbackUrl;
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Не показывать, что пользователь не существует или не подтвержден
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Отправка сообщения электронной почты с этой ссылкой
                 string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                 await UserManager.SendEmailAsync(user.Id, "Сброс пароля", "Сбросьте ваш пароль, щелкнув <a href=\"" + callbackUrl + "\">здесь</a>");
                 return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Не показывать, что пользователь не существует
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Запрос перенаправления к внешнему поставщику входа
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Создание и отправка маркера
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет учетной записи, то ему предлагается создать ее
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Получение сведений о пользователе от внешнего поставщика входа
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        #region Мой код

        [Authorize(Roles = "admin")]
        public ActionResult UserList()
        {
            //IList<ApplicationUser> users = new List<ApplicationUser> { };
            //using (ApplicationDbContext context = new ApplicationDbContext())
            //{
            //    users = context.Users.ToList();
            //}
            //return View(users);
            List<UserViewModel> list = new List<UserViewModel>();
            foreach (var user in UserManager.Users)
            {
                UserViewModel userView = new UserViewModel(user);
                using (AppDbContext db = new AppDbContext())
                {
                    Employee employee = db.Employees.SingleOrDefault(x=> x.Id == user.EmployeeId);
                    if (employee != null)
                    { 
                        userView.Employee = employee.FamilyName + " " + employee.Name + " " + employee.FathersName + " " + employee.BirthDay.ToShortDateString();
                    }
                    else
                    {
                        userView.Employee = GlobalRes.Empty;
                    }
                }
                list.Add(userView);
            }
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public ActionResult DetailsUser(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            ViewBag.Roles = UserManager.GetRoles(id);
            return View(user);
        }

        //
        // GET: /Account/DeleteUser
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(string id)
        {
            ApplicationUser user = new ApplicationUser();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                user = context.Users.Find(id);
            }
            return View(user);
        }

        //
        // POST: /Account/DeleteUser
        [HttpPost, ActionName("DeleteUser")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUserConfirm(string id)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(id);
                var result = await UserManager.DeleteAsync(user);
                AddErrors(result);
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return RedirectToAction("UserList");
        }

        //
        // GET: /Account/EditUser
        [Authorize(Roles = "admin")]
        public ActionResult EditUser(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            IList<IdentityRole> roles = new List<IdentityRole> { };
            List<Locality> LocalityList = new List<Locality> { }; 
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                roles = context.Roles.ToList();
                LocalityList = context.Localities.ToList();
            }
            string[] activeRoles = UserManager.GetRoles(id).ToArray();
            IList<UserRole> userRoles = new List<UserRole> { };
            foreach (IdentityRole role in roles)
            {
                UserRole userRole = new UserRole();
                userRole.Id = role.Id;
                userRole.Name = role.Name;
                userRole.IsInRole = false;
                if (activeRoles.Contains(role.Name))
                {
                    userRole.IsInRole = true;
                }
                userRoles.Add(userRole);

            }
            ViewBag.UserRoles = userRoles;
            ViewBag.Locality = new SelectList(LocalityList, "Name", "Language");
            List<Employee> EmployeeList = new List<Employee>();
            List<Salon> SalonList = new List<Salon>();
            using (AppDbContext db = new AppDbContext())
            {
                EmployeeList = db.Employees.OrderBy(x => x.FamilyName).ThenBy(x => x.Name).ThenBy(x => x.FathersName).ToList();
                SalonList = db.Salons.ToList();
            }
            EmployeeList = GetEmployeeSelectName(EmployeeList);
            EmployeeList.Insert(0, new Employee() { Name = GlobalRes.Empty });

            ViewBag.EmployeeId = new SelectList(EmployeeList, "Id", "Name");

            SalonList.Insert(0, new Salon() { Name = GlobalRes.Empty });

            ViewBag.SalonId = new SelectList(SalonList, "Id", "Name");

            return View(user);
        }

        //
        // POST: /Account/EditUser
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(ApplicationUser model, string[] newRoles)
        {
            if (ModelState.IsValid)
            {

                var user = UserManager.FindById(model.Id);
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.UserName = model.UserName;
                user.Locality = model.Locality;
                user.EmployeeId = SetValueToNull(model.EmployeeId);
                user.SalonId = SetValueToNull(model.SalonId);

                var result = await UserManager.UpdateAsync(user);

                AddErrors(result);
                if (result.Succeeded)
                {
                    await UserManager.RemoveFromRolesAsync(user.Id, UserManager.GetRoles(user.Id).ToArray());
                    if (!newRoles.Contains("visitor"))
                    {
                        newRoles = newRoles.Concat(new string[] { "visitor" }).ToArray();
                    }
                    newRoles = newRoles.Where(role => role != "false").ToArray();
                    await UserManager.AddToRolesAsync(model.Id, newRoles);
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return RedirectToAction("UserList");
        }

        private List<Employee> GetEmployeeSelectName(List<Employee> employees)
        {
            List<Employee> result = new List<Employee>();
            foreach (Employee employee in employees)
            {
                employee.Name = employee.StaffNumber + " " + employee.FamilyName + " " + employee.Name;
                result.Add(employee);
            }
            return result;
        }
        private int? SetValueToNull(int? value)
        {
            if (IsOfNullableType(value))
            {
                if (value == 0)
                {
                    return null;
                }
            }
            return value;
        }
        private bool IsOfNullableType<T>(T o)
        {
            var type = typeof(T);
            return Nullable.GetUnderlyingType(type) != null;
        }
        #endregion  
    }
}