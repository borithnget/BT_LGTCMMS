using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System.Globalization;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext context;
        private kim_mexEntities db = new kim_mexEntities();
        public UserController()
        {
            context = new ApplicationDbContext();
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult RegisterProjectManager(string user_first_name, string user_last_name, string user_telephone, string email)
        {
            try
            {
                tb_user_detail user_detail = new tb_user_detail();
                user_detail.user_detail_id = Guid.NewGuid().ToString();
                user_detail.user_first_name = user_first_name;
                user_detail.user_last_name = user_last_name;
                user_detail.user_position_id = "6";
                user_detail.user_telephone = user_telephone;
                user_detail.user_email = email;
                user_detail.status = true;
                user_detail.created_by = User.Identity.Name;
                user_detail.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_user_detail.Add(user_detail);
                db.SaveChanges();
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CreateNewSiteManager(string firstName,string lastName,string contactPhone,string email)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_user_detail userDetail = new tb_user_detail();
                userDetail.user_detail_id = Guid.NewGuid().ToString();
                userDetail.user_first_name = firstName;
                userDetail.user_last_name = lastName;
                userDetail.user_position_id = "5";
                userDetail.user_telephone = contactPhone;
                userDetail.user_email = email;
                userDetail.status = true;
                userDetail.created_by = User.Identity.Name;
                userDetail.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_user_detail.Add(userDetail);
                db.SaveChanges();
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "UserController.cs", "CreateNewSiteManager", ex.StackTrace, ex.Message);
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Details(String id)
        {
            UserRolesViewModel user_role = new UserRolesViewModel();
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users
                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,
                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 select role.Name).ToList()
                                 }).ToList()
                                 .Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,
                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName)
                                 }
                                 ).Where(m => m.UserId == id).FirstOrDefault();
                if (userRoles!=null)
                {
                    UserViewModel user = new UserViewModel();
                    user = GlobalMethod.GetUserInfomationDetail(userRoles.UserId);
                    user_role.UserId = userRoles.UserId;
                    user_role.Username = userRoles.Username;
                    user_role.Email = userRoles.Email;
                    user_role.Role = userRoles.Role;
                    user_role.user_first_name = user.user_first_name;
                    user_role.user_last_name = user.user_last_name;
                    user_role.position_name = user.position_name;
                    user_role.user_telephone = user.user_telephone;
                    user_role.user_signature = user.user_signature;


                    string date = Convert.ToDateTime(user.created_date).ToString("dd/MM/yyyy");
                    user_role.created_date = date;
                }
            }
            catch (Exception ex) { }
            return View(user_role);
        }
        public ActionResult Edit(string UserId)
        {
            return View();
        }
        public ActionResult UserDataTable()
        {
            List<UserRolesViewModel> users = new List<UserRolesViewModel>();
            var user_list = (from tbl in db.tb_user_detail
                             join po in db.tb_position on tbl.user_position_id equals po.position_id into pos from po in pos.DefaultIfEmpty()
                             where tbl.user_id != null && tbl.status == true
                             select new { user_id=tbl.user_id, user_first_name=tbl.user_first_name, user_last_name=tbl.user_last_name, user_position_id=tbl.user_position_id, position_name=po.position_name, user_telephone=tbl.user_telephone }).ToList();
            if (user_list.Any())
            {
                foreach (var user in user_list)
                {
                    if (user.user_id != null)
                    {
                        UserRolesViewModel user_role = new UserRolesViewModel();
                        user_role = GlobalMethod.GetUserNameDetail(user.user_id);
                        users.Add(new UserRolesViewModel() {
                            UserId = user_role.UserId,
                            Username = user_role.Username,
                            Email = user_role.Email,
                            Role = user_role.Role,
                            user_first_name = user.user_first_name,
                            user_last_name = user.user_last_name,
                            user_position_id = user.user_position_id,
                            position_name = user.position_name,
                            user_telephone = user.user_telephone,
                        });
                    }
                }
            }
            return Json(new { data = users }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPositionDropdownList()
        {
            List<PositionViewModel> positions = new List<PositionViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var position_lists = (from tbl in db.tb_position orderby tbl.position_name where tbl.status == true select tbl).ToList();
                if (position_lists.Any())
                {
                    foreach (var position in position_lists)
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
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var user_group_lists = context.Roles.ToList();
                if (user_group_lists.Any())
                {
                    foreach (var role in user_group_lists)
                    {
                        user_groups.Add(new UserRolesViewModel() { Role = role.Name });
                    }
                }
            }
            return Json(new { data = user_groups }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetSignature(string id)
        {
            UserViewModel user=UserUpdateViewModel.GetUserDetailById(id);
            
            return View(user);
        }
        [HttpPost]
        public ActionResult ResetSignature(string id,UserViewModel model)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_user_detail userdetail = db.tb_user_detail.Find(model.user_detail_id);
                if(userdetail != null)
                {
                    userdetail.user_signature= model.user_signature;
                    db.SaveChanges();
                }
            }
            UserViewModel user= UserUpdateViewModel.GetUserDetailById(id);
            ViewBag.Message = "Your data has been saved successfully.";
            return View(user);
        }
    }
}