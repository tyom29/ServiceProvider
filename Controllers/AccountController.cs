using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ServiceProvider.Domain;
using ServiceProvider.Models;
using ServiceProvider.Services;
using Unity;

namespace ServiceProvider.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ICustomerService _customerService;

        [InjectionConstructor]
        public AccountController(ICustomerService customerService)
        {
            _customerService = customerService;
        }


        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            var userid = UserManager.FindByEmail(model.Email).Id;
            if (!UserManager.IsEmailConfirmed(userid))
            {
                ModelState.AddModelError("", "Please confirm your email to continue. Do you want to <a onclick='a()'> resend '</a>'  it?");
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl,model.Email);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Customer customer=new Customer { Email = model.Email, RegDate = new DateTime(), ApiKey = Guid.NewGuid()};
                    _customerService.SaveCustomer(customer);
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    string Body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Content/beefree-q325opqvdt.htm"));
                    Body = Body.Replace("#url#", callbackUrl);
                    Body = Body.Replace("#UserName#", model.Email);

                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", Body);

                    return View("ThankYou");
                }
                AddErrors(result);
            }
            return View(model);
        }


        //
        // POST: /Account/ReSend
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReSend(string email)
        {

            var userid = UserManager.FindByEmail(email).Id;

            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userid);

            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userid, code = code }, protocol: Request.Url.Scheme);

            string Body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Content/beefree-q325opqvdt.htm"));
            Body = Body.Replace("#url#", callbackUrl);
            Body = Body.Replace("#UserName#", email);
            await UserManager.SendEmailAsync(userid, "Confirm your account", Body);

            return RedirectToAction("Login", "Account");

        }


        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            var tokenExpired = false;
            var unprotectedData = UserManager.Protector.Unprotect(Convert.FromBase64String(code));
            var ms = new MemoryStream(unprotectedData);
            using (BinaryReader reader = new BinaryReader(ms))
            {
                var creationTime = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
                var expirationTime = creationTime + UserManager.TokenLifespan;
                if (expirationTime < DateTimeOffset.UtcNow)
                {
                    tokenExpired = true;
                }
            }

            if (tokenExpired)
            {

                return View("ExpiredLink");
            }
            else
            {
                if (userId == null || code == null)
                {
                    return View("Error");
                }
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
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

        #region Helpers
        // Used for XSRF protection when adding external logins
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

        private ActionResult RedirectToLocal(string returnUrl,string email)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { mail = email });
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
    }
}