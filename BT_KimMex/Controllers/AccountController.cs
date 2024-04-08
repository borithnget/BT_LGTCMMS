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
using BT_KimMex.Models;
using System.Collections.Generic;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using System.Web.Security;
using System.IO;
using System.Security.Cryptography.Xml;
using ImageResizer;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;
        public AccountController()
        {
            context = new ApplicationDbContext();
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
            /*
            HttpCookie cookie = Request.Cookies[DefaultAuthenticationTypes.ApplicationCookie];
            if (!string.IsNullOrEmpty(cookie.Value))
                return RedirectToLocal(returnUrl);
                */
            if(Request.IsAuthenticated)
                return RedirectToLocal(returnUrl);
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

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    Session["Auth"] = model.UserName;
                    HttpCookie cookie = new HttpCookie(DefaultAuthenticationTypes.ApplicationCookie, model.UserName);
                    cookie.Value = model.UserName;
                    Response.Cookies.Add(cookie);
                    return RedirectToLocal(returnUrl);
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

        public ActionResult Logon()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
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

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [Authorize(Roles= "Admin")]
        public ActionResult Register()
        {
            List<tb_position> positions = new List<tb_position>();
            List<UserRolesViewModel> groups = new List<UserRolesViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                positions = db.tb_position.OrderBy(p => p.position_name).Where(p=>p.status==true).ToList();
                //var user_group_lists = context.Roles.ToList();
                //if (user_group_lists.Any())
                //{
                //    foreach (var role in user_group_lists)
                //    {
                //        groups.Add(new UserRolesViewModel() { Role = role.Name });
                //    }
                //}

                groups = db.AspNetRoles.OrderBy(s=>s.Name).Where(s => s.IsActive == true).Select(s => new UserRolesViewModel() { Role = s.Name }).ToList();
            }
            ViewBag.PositionID = new SelectList(positions, "position_id", "position_name");
            //ViewBag.GroupID = new SelectList(groups, "Role", "Role");
            ViewBag.GroupID = groups;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (model.Email == null || string.IsNullOrEmpty(model.Email))
                model.Email = string.Format("{0}@email.com",model.LastName+model.FirstName);

            if (ModelState.IsValid)
            {
                var isExist = await UserManager.FindByNameAsync(model.UserName);
                if (isExist == null)
                {
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        foreach(var role in model.Role)
                        {
                            await this.UserManager.AddToRoleAsync(user.Id, role);
                        }
                        //await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);

                        kim_mexEntities db = new kim_mexEntities();
                        tb_user_detail user_detail = new tb_user_detail();
                        user_detail.user_detail_id = Guid.NewGuid().ToString();
                        user_detail.user_id = user.Id;
                        user_detail.user_first_name = model.FirstName;
                        user_detail.user_last_name = model.LastName;
                        user_detail.user_position_id = model.Position;
                        user_detail.user_telephone = model.Telephone;
                        user_detail.user_email = model.Email;
                        user_detail.status = true;
                        user_detail.created_by = User.Identity.Name;
                        user_detail.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        user_detail.user_signature = model.user_signature;
                        db.tb_user_detail.Add(user_detail);
                        db.SaveChanges();

                        for(int i = 0; i < model.user_signatures.Count(); i++)
                        {
                            string attachmentId = model.user_signatures[i];
                            tb_attachment signature = db.tb_attachment.Find(attachmentId);
                            if (signature != null)
                            {
                                signature.attachment_ref_id = user.Id;
                                db.SaveChanges();
                            }
                        }
                        
                        return RedirectToAction("Index", "User");
                    }
                    AddErrors(result);
                }
            }
            
            // If we got this far, something failed, redisplay form
            List<tb_position> positions = new List<tb_position>();
            List<UserRolesViewModel> groups = new List<UserRolesViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                positions = db.tb_position.OrderBy(p => p.position_name).Where(p => p.status == true).ToList();
            }
            groups = this.GetUserGroup();
            ViewBag.PositionID = new SelectList(positions, "position_id", "position_name");
            //ViewBag.GroupID = new SelectList(groups, "Role", "Role");
            ViewBag.GroupID = groups;
            return View(model);
        }
        public ActionResult EditUser(string id)
        {
            UserUpdateViewModel user_role = new UserUpdateViewModel();
            List<tb_position> positions = new List<tb_position>();
            List<UserRolesViewModel> groups = new List<UserRolesViewModel>();
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users
                                 select new { UserId = user.Id, Username = user.UserName, Email = user.Email, Role = (from userRole in user.Roles join role in context.Roles on userRole.RoleId equals role.Id select role.Name).ToList() }).ToList()
                                 .Select(p => new UserUpdateViewModel() { UserID = p.UserId, UserName = p.Username, Email = p.Email, Role = p.Role.ToArray()  }).Where(m => m.UserID == id).FirstOrDefault();
                if (userRoles != null)
                {
                    UserViewModel user = new UserViewModel();
                    user = GlobalMethod.GetUserInfomationDetail(userRoles.UserID);
                    user_role.UserID = userRoles.UserID;
                    user_role.UserName = userRoles.UserName;
                    user_role.Email = userRoles.Email;
                    user_role.Role = userRoles.Role;
                    user_role.FirstName = user.user_first_name;
                    user_role.LastName = user.user_last_name;
                    user_role.Position = user.user_position_id;
                    user_role.Telephone = user.user_telephone;
                    user_role.user_signature= user.user_signature;

                    string date = Convert.ToDateTime(user.created_date).ToString("dd/MM/yyyy");
                    user_role.created_date = date;

                    user_role.signatures = GlobalMethod.GetUserSignatures(user_role.UserID);
                }
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    positions = db.tb_position.OrderBy(p => p.position_name).Where(p => p.status == true).ToList();
                }
                groups = this.GetUserGroup();
                ViewBag.PositionID = new SelectList(positions, "position_id", "position_name");
                //ViewBag.GroupID = new SelectList(groups, "Role", "Role");
                ViewBag.GroupID = groups;
            }
            catch(Exception ex) { }
            return View(user_role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(string id,UserUpdateViewModel model)
        {
            if (model.Email == null || string.IsNullOrEmpty(model.Email))
                model.Email = string.Format("{0}@email.com", model.LastName + model.FirstName);
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(id);
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    //await UserManager.RemovePasswordAsync(id);
                    //await UserManager.AddPasswordAsync(id, model.Password);
                    var userRoles = (from us in context.Users
                                     select new { UserId = us.Id, Username = us.UserName, Email = us.Email, Role = (from userRole in us.Roles join role in context.Roles on userRole.RoleId equals role.Id select role.Name).ToList() }).ToList()
                                 .Select(p => new UserUpdateViewModel() { UserID = p.UserId, UserName = p.Username, Email = p.Email, Role = p.Role.ToArray() }).Where(m => m.UserID == id).FirstOrDefault();
                    foreach(var role in userRoles.Role.ToArray())

                    await this.UserManager.RemoveFromRolesAsync(user.Id,role);

                    foreach (var role in model.Role)
                    {
                        await this.UserManager.AddToRoleAsync(user.Id, role);
                    }
                    //await this.UserManager.AddToRoleAsync(user.Id, model.Role);

                    kim_mexEntities db = new kim_mexEntities();
                    tb_user_detail user_detail = db.tb_user_detail.Where(u => u.status == true && u.user_id == user.Id).FirstOrDefault();
                    user_detail.user_id = user.Id;
                    user_detail.user_first_name = model.FirstName;
                    user_detail.user_last_name = model.LastName;
                    user_detail.user_position_id = model.Position;
                    user_detail.user_telephone = model.Telephone;
                    user_detail.user_email = model.Email;
                    user_detail.status = true;
                    user_detail.updated_by = User.Identity.Name;
                    user_detail.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    user_detail.user_signature = model.user_signature;
                    db.SaveChanges();

                    for (int i = 0; i < model.user_signatures.Count(); i++)
                    {
                        string attachmentId = model.user_signatures[i];
                        tb_attachment signature = db.tb_attachment.Find(attachmentId);
                        if (signature != null)
                        {
                            if (string.IsNullOrEmpty(signature.attachment_ref_id))
                            {
                                signature.attachment_ref_id = user.Id;
                                db.SaveChanges();
                            }
                            
                        }
                    }

                    return RedirectToAction("Index", "User");
                }
                AddErrors(result);
            }
            List<tb_position> positions = new List<tb_position>();
            List<UserRolesViewModel> groups = new List<UserRolesViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                positions = db.tb_position.OrderBy(p => p.position_name).Where(p => p.status == true).ToList();
            }
            groups = this.GetUserGroup();
            ViewBag.PositionID = new SelectList(positions, "position_id", "position_name");
            ViewBag.GroupID = new SelectList(groups, "Role", "Role");
            return View(model);
        }
        
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var user =UserManager.FindById(id);

                tb_user_deleted d_user = new tb_user_deleted();
                d_user.user_deteled_id = Guid.NewGuid().ToString();
                d_user.user_id = user.Id;
                d_user.user_email = user.Email;
                d_user.user_name = user.UserName;
                d_user.created_by = User.Identity.Name;
                d_user.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_user_deleted.Add(d_user);
                db.SaveChanges();

                tb_user_detail de_user = db.tb_user_detail.Where(m => m.user_id == user.Id).FirstOrDefault();
                de_user.user_id = null;
                de_user.status = false;
                de_user.updated_by = User.Identity.Name;
                de_user.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                this.UserManager.RemoveFromRoles(user.Id);
                this.UserManager.Delete(user);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex) { return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet); }
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
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
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
        public ActionResult ResetPassword(string id,string status)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.UserID = id;
            model.UserName = UserManager.FindById(id).UserName;
            model.Code = !string.IsNullOrEmpty(status) ? status : "";
            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            await UserManager.RemovePasswordAsync(user.Id);
            var result = await UserManager.AddPasswordAsync(user.Id, model.Password);
            if (result.Succeeded)
            {
                if(string.Compare(model.Code, "ChangePassword") == 0) 
                    return RedirectToAction("Index", "Home");
                return RedirectToAction("Index", "User");
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
            // Request a redirect to the external login provider
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

            // Generate the token and send it
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

            // Sign in the user with this external login provider if the user already has a login
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
                    // If the user does not have an account, then prompt the user to create an account
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
                // Get the information about the user from the external login provider
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
            if (Request.Cookies[DefaultAuthenticationTypes.ApplicationCookie] != null)
            {
                var c = new HttpCookie(DefaultAuthenticationTypes.ApplicationCookie);
                c.Expires = Class.CommonClass.ToLocalTime(DateTime.Now).AddDays(-1);
                Response.Cookies.Add(c);
            }
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
        public async Task<ActionResult> RegisterUser(RegisterViewModel model,string user_first_name,string user_last_name,string position_id,string user_telephone)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        //await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);

                        kim_mexEntities db = new kim_mexEntities();
                        tb_user_detail user_detail = new tb_user_detail();
                        user_detail.user_detail_id = Guid.NewGuid().ToString();
                        user_detail.user_id = user.Id;
                        user_detail.user_first_name =user_first_name;
                        user_detail.user_last_name = user_last_name;
                        user_detail.user_position_id = position_id;
                        user_detail.user_telephone = user_telephone;
                        user_detail.user_email = model.Email;
                        user_detail.status = true;
                        user_detail.created_by = User.Identity.Name;
                        user_detail.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        db.tb_user_detail.Add(user_detail);
                        db.SaveChanges();

                        return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    AddErrors(result);
                }
                return Json(new { result = "fail" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPositionDropdownList()
        {
            List<PositionViewModel> positions = new List<PositionViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var position_lists = (from tbl in db.tb_position orderby tbl.position_name where tbl.status == true select tbl).ToList();
                if (position_lists.Any())
                {
                    foreach(var position in position_lists)
                    {
                        positions.Add(new PositionViewModel() { position_id = position.position_id, position_name = position.position_name });
                    }
                }
            }
            return Json(new { data = positions }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserGroupDropdownList()
        {
            List<UserRolesViewModel> user_groups = new List<UserRolesViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var user_group_lists = context.Roles.ToList();
                if (user_group_lists.Any())
                {
                    foreach(var role in user_group_lists)
                    {
                        user_groups.Add(new UserRolesViewModel() { Role = role.Name });
                    }
                }
            }
            return Json(new { data = user_groups }, JsonRequestBehavior.AllowGet);
        }
        private List<UserRolesViewModel> GetUserGroup()
        {
            List<UserRolesViewModel> groups = new List<UserRolesViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //var user_group_lists = context.Roles.ToList();
                //if (user_group_lists.Any())
                //{
                //    foreach (var role in user_group_lists)
                //    {
                //        groups.Add(new UserRolesViewModel() { Role = role.Name });
                //    }
                //}
                groups = db.AspNetRoles.OrderBy(s => s.Name).Where(s => s.IsActive == true).Select(s => new UserRolesViewModel() { Role = s.Name }).ToList();
            }
            return groups;
        }

        [HttpPost]
        public ActionResult UploadUserSignature(POSTUserSignatureModel model)
        {
            
            var file = model.ImageFile;
            // byte[] imagebyte = null;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (file != null)
                {
                    string path = Server.MapPath("/Documents/Signature/" + file.FileName);
                    file.SaveAs(path);

                    //ResizeSettings resizeSetting = new ResizeSettings
                    //{
                    //    Width = 150,
                    //    Height = 100,
                    //    Format = "png"
                    //};
                    //ImageBuilder.Current.Build(path, path, resizeSetting);

                    //BinaryReader reader = new BinaryReader(file.InputStream);

                    //imagebyte = reader.ReadBytes(file.ContentLength); 
                    string SignaturePath = "/Documents/Signature/" + file.FileName;

                    return Json(new { SignaturePath = SignaturePath }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { SignaturePath = string.Empty }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UploadUserMultipleSignature()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_attachment attachment = new tb_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/Signature/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    attachment.attachment_id = file_id;
                    attachment.attachment_name = file_name;
                    attachment.attachment_extension = file_extension;
                    attachment.attachment_path = file_path;
                    attachment.attachment_ref_type = "Quote";
                    db.tb_attachment.Add(attachment);
                    db.SaveChanges();
                    
                }

                return Json(new { attachment }, JsonRequestBehavior.AllowGet);
            }
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
    }
}