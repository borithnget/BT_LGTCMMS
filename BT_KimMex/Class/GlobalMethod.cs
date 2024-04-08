using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using Microsoft.AspNet.Identity;
using System.Security.Principal;

namespace BT_KimMex.Class
{
    public static class GlobalMethod
    {
        private static kim_mexEntities db = new kim_mexEntities();
        private static ApplicationDbContext context = new ApplicationDbContext();
        public static bool IsBOQProjectExist(string projectID)
        {
            bool isExist = false;
            try
            {
                var boq = db.tb_build_of_quantity.Where(m => m.project_id == projectID&&m.status==true).FirstOrDefault();
                if (boq != null)
                {
                    isExist = true;
                }else
                {
                    isExist = false;
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "GlobalMethod.cs", "IsBOQProjectExist", ex.StackTrace, ex.Message);
            }
            return isExist;
        }
        public static bool IsProjectExist(string projectID)
        {
            bool isExist = false;
            try
            {
                var project = db.tb_project.Where(m => m.project_id == projectID).FirstOrDefault();
                if (project != null)
                {
                    isExist = true;
                }else
                {
                    isExist = false;
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "GlobalMethod.cs", "IsProjectExist", ex.StackTrace, ex.Message);
            }
            return isExist;
        }

        public static UserRolesViewModel GetUserNameDetail(string userID)
        {
            UserRolesViewModel user_detail = new UserRolesViewModel();
            try
            {
                var userRoles = (from user in context.Users
                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,
                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 select role.Name).ToList(),
                                     
                                 }).ToList()
                                 .Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,
                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName),
                                 }
                                 ).OrderBy(p => p.Username);
                if (userRoles.Any())
                {
                    foreach (var user_role in userRoles)
                    {
                        if (string.Compare(user_role.UserId, userID)==0)
                        {
                            user_detail = new UserRolesViewModel();
                            user_detail.UserId = user_role.UserId;
                            user_detail.Username = user_role.Username;
                            user_detail.Email = user_role.Email;
                            user_detail.Role = user_role.Role;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "GlobalMethod.cs", "GetUserNameDetail", ex.StackTrace, ex.Message);
            }
            return user_detail;
        }
        public static UserViewModel GetUserInfomationDetail(string userID)
        {
            UserViewModel user = new UserViewModel();
            try
            {
                user = (from us in db.tb_user_detail
                         join po in db.tb_position on us.user_position_id equals po.position_id into pos
                        from po in pos.DefaultIfEmpty()
                        where us.user_id == userID
                         select new UserViewModel {user_detail_id=us.user_detail_id,user_id=us.user_id,user_first_name=us.user_first_name,user_last_name=us.user_last_name,user_position_id=us.user_position_id,position_name=po.position_name,user_telephone=us.user_telephone,created_date=us.created_date ,user_signature=us.user_signature}).FirstOrDefault();
            }catch(Exception ex) {
               
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "GlobalMethod.cs", "GetUserInfomationDetail", ex.StackTrace, ex.Message); }
            return user;
        }
        
        public static List<AttachmentViewModel> GetUserSignatures(string userId)
        {
            List<AttachmentViewModel> signatures = new List<AttachmentViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                signatures = db.tb_attachment.Where(s=>string.Compare(s.attachment_ref_id,userId)==0).Select(s => new AttachmentViewModel()
                {
                    attachment_id=s.attachment_id,
                    attachment_name=s.attachment_name,
                    attachment_path=s.attachment_path,
                    attachment_extension=s.attachment_extension,
                    attachment_ref_id=s.attachment_ref_id,
                    attachment_ref_type=s.attachment_ref_type,
                }).ToList() ;

            }catch(Exception ex)
            {

            }
            return signatures;
        }

        public static List<tb_project> GetProjectDropdownlist()
        {
            List<tb_project> projects = new List<tb_project>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                projects = db.tb_project.OrderBy(m => m.project_short_name).Where(m => m.project_status == true && m.p_status == "Active").ToList();
            }
            return projects;
        }
        public static List<tb_product_category> GetItemTypeDropdownlist()
        {
            List<tb_product_category> types = new List<tb_product_category>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                types = db.tb_product_category.OrderBy(m => m.p_category_code).Where(m => m.status == true).ToList();
            }
            return types;
        }
        public static bool IsUserInRole(string UserName,string UserRole)
        {
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
                                 ).Where(m => m.Username == UserName).FirstOrDefault();
            if (userRoles.Role.Contains(UserRole))
                return true;
            else
                return false;
        }
        public static List<UserRolesViewModel> GetUserListItemsbyRole(string roleName)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<UserRolesViewModel> users = new List<UserRolesViewModel>();
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users

                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,

                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 where string.Compare(role.Name,roleName)==0

                                                 select role.Name).ToList()
                                 }).ToList().Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,

                                     Username = p.Username,
                                     Email = p.Email,
                                     
                                     Role = string.Join(",", p.RoleName)
                                 }).Where(p =>string.Compare(p.Role,roleName)==0).OrderBy(p => p.Username);
                foreach(UserRolesViewModel item in userRoles)
                {
                    var ud = db.tb_user_detail.Where(w=>string.Compare(w.user_id,item.UserId)==0).FirstOrDefault();
                    if (ud != null)
                    {
                        item.user_first_name = ud.user_first_name + " " + ud.user_last_name;
                        users.Add(item);
                    }
                    
                }
                return users;
            }
        }
    }
}