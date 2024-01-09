using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using BT_KimMex.Class;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class SiteController : Controller
    {
        // GET: Site
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(SiteViewModel model,List<string> sitesupervisors)
        {
            //if (ModelState.IsValid)
            //{
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    tb_site site = new tb_site();
                    site.site_id = Guid.NewGuid().ToString();
                    site.site_name = model.site_name;
                    site.site_address = model.site_address;
                    site.site_code = model.site_code;
                    site.create_dated = Class.CommonClass.ToLocalTime(DateTime.Now);
                    site.created_by = User.Identity.Name;
                    site.status = true;
                    db.tb_site.Add(site);
                    db.SaveChanges();
                    //if (sitesupervisors != null)
                    //{
                    //    for(int i = 0; i < sitesupervisors.Count; i++)
                    //    {
                    //        tbSiteSiteSupervisor sitesupervisor = new tbSiteSiteSupervisor();
                    //        sitesupervisor.site_site_supervisor_id = Guid.NewGuid().ToString();
                    //        sitesupervisor.site_id = site.site_id;
                    //        sitesupervisor.site_supervisor_id = sitesupervisors[i];
                    //        db.tbSiteSiteSupervisors.Add(sitesupervisor);
                    //        db.SaveChanges();
                    //    }
                        
                    //}
                    
                    //tb_site_site_admin siteAd = new tb_site_site_admin();
                    //siteAd.site_site_admin_id = Guid.NewGuid().ToString();
                    //siteAd.site_id = site.site_id;
                    //siteAd.site_admin_id = model.site_admin_id;
                    //db.tb_site_site_admin.Add(siteAd);
                    //db.SaveChanges();
                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            //}
            TempData["message"] = "Your data is error while saving!";
            return View();
        }
        public ActionResult CreateJson(string site_name,string site_address)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_site site = new tb_site();
                site.site_id = Guid.NewGuid().ToString();
                site.site_name = site_name;
                site.site_address = site_address;
                site.create_dated = Class.CommonClass.ToLocalTime(DateTime.Now);
                site.created_by = User.Identity.Name;
                site.status = true;
                db.tb_site.Add(site);
                db.SaveChanges();
                return Json(new { Message = "Success",site_id=site.site_id }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Details(string id)
        {
            SiteViewModel site = new SiteViewModel();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var siteDetail = (from tbl in db.tb_site where tbl.site_id == id select tbl).FirstOrDefault();
                if (siteDetail != null)
                {
                    site.site_id = siteDetail.site_id;
                    site.site_name = siteDetail.site_name;
                    site.site_address = siteDetail.site_address;
                    site.site_code = siteDetail.site_code;
                    site.create_dated = siteDetail.create_dated;
                    //var sitesups = db.tbSiteSiteSupervisors.Where(s => string.Compare(s.site_id, site.site_id) == 0).ToList();
                    //foreach(var sitesup in sitesups)
                    //{
                    //    SiteSiteSupervisorViewModel ss = new SiteSiteSupervisorViewModel();
                    //    ss.site_supervisor_id = sitesup.site_supervisor_id;
                    //    var ud = db.tb_user_detail.Where(s => string.Compare(s.user_id, sitesup.site_supervisor_id) == 0).FirstOrDefault();
                    //    ss.site_supervisor_name = ud.user_first_name + " " + ud.user_last_name;
                    //    site.siteSupervisors.Add(ss);
                    //}

                    //site.siteAdmin = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, site.site_id) == 0).Select(s => new SiteSiteAdminViewModel()
                    //{
                    //    site_admin_id = s.site_admin_id,
                    //}).FirstOrDefault();
                    //if (site.siteAdmin != null)
                    //{
                    //    var ud = db.tb_user_detail.Where(s => string.Compare(s.user_id, site.siteAdmin.site_admin_id) == 0).FirstOrDefault();
                    //    site.site_admin_id = ud.user_first_name + " " + ud.user_last_name;
                    //}
                }
            }
            return View(site);
        }
        public ActionResult Edit(string id)
        {
            SiteViewModel site = new SiteViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var siteDetail = (from tbl in db.tb_site where tbl.site_id == id select tbl).FirstOrDefault();
                if (siteDetail != null)
                {
                    site.site_id = siteDetail.site_id;
                    site.site_name = siteDetail.site_name;
                    site.site_address = siteDetail.site_address;
                    site.site_code = siteDetail.site_code;
                    site.create_dated = siteDetail.create_dated;
                    //var sitesups = db.tbSiteSiteSupervisors.Where(s => string.Compare(s.site_id, site.site_id) == 0).ToList();
                    //foreach (var sitesup in sitesups)
                    //{
                    //    SiteSiteSupervisorViewModel ss = new SiteSiteSupervisorViewModel();
                    //    ss.site_supervisor_id = sitesup.site_supervisor_id;
                    //    var ud = db.tb_user_detail.Where(s => string.Compare(s.user_id, sitesup.site_supervisor_id) == 0).FirstOrDefault();
                    //    ss.site_supervisor_name = ud.user_first_name + " " + ud.user_last_name;
                    //    site.siteSupervisors.Add(ss);
                    //}
                    //site.siteAdmin = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, site.site_id) == 0).Select(s => new SiteSiteAdminViewModel()
                    //{
                    // site_admin_id=s.site_admin_id,   
                    //}).FirstOrDefault();
                    //if (site.siteAdmin != null)
                    //    site.site_admin_id = site.site_admin_id;
                }
            }
            return View(site);
        }
        [HttpPost]
        public ActionResult Edit(SiteViewModel siteVM,string id,List<string> sitesupervisors)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //if (ModelState.IsValid)
                //{
                    tb_site site = db.tb_site.FirstOrDefault(model => model.site_id == id);
                    site.site_name = siteVM.site_name;
                    site.site_address = siteVM.site_address;
                    site.site_code = siteVM.site_code;
                    site.updated_dated = Class.CommonClass.ToLocalTime(DateTime.Now);
                    site.updated_by = User.Identity.Name;
                    site.status = true;
                    db.SaveChanges();
                    //var sitesup = db.tbSiteSiteSupervisors.Where(w => string.Compare(w.site_id, site.site_id) == 0).FirstOrDefault();
                    //if (sitesup == null)
                    //{
                    //    tbSiteSiteSupervisor sitesupervisor = new tbSiteSiteSupervisor();
                    //    sitesupervisor.site_site_supervisor_id = Guid.NewGuid().ToString();
                    //    sitesupervisor.site_id = site.site_id;
                    //    sitesupervisor.site_supervisor_id = siteVM.site_supervisor_id;
                    //    db.tbSiteSiteSupervisors.Add(sitesupervisor);
                    //    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    tbSiteSiteSupervisor sitesupervisor = db.tbSiteSiteSupervisors.Find(sitesup.site_site_supervisor_id);
                    //    sitesupervisor.site_supervisor_id = siteVM.site_supervisor_id;
                    //    db.SaveChanges();
                    //}

                    //if (sitesupervisors != null)
                    //{
                    //    var oldSiteSups = db.tbSiteSiteSupervisors.Where(s => s.site_id == site.site_id).ToList();
                    //    foreach(var oldsitesup in oldSiteSups)
                    //    {
                    //        var sid = oldsitesup.site_site_supervisor_id;
                    //        tbSiteSiteSupervisor dd = db.tbSiteSiteSupervisors.Find(sid);
                    //        db.tbSiteSiteSupervisors.Remove(dd);
                    //        db.SaveChanges();
                    //    }
                    //    for (int i = 0; i < sitesupervisors.Count; i++)
                    //    {
                    //        tbSiteSiteSupervisor sitesupervisor = new tbSiteSiteSupervisor();
                    //        sitesupervisor.site_site_supervisor_id = Guid.NewGuid().ToString();
                    //        sitesupervisor.site_id = site.site_id;
                    //        sitesupervisor.site_supervisor_id = sitesupervisors[i];
                    //        db.tbSiteSiteSupervisors.Add(sitesupervisor);
                    //        db.SaveChanges();
                    //    }

                    //}

                    ////site admin
                    //var siteAdmin = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, site.site_id) == 0).FirstOrDefault();
                    //if (siteAdmin == null)
                    //{
                    //    tb_site_site_admin siteAd = new tb_site_site_admin();
                    //    siteAd.site_site_admin_id = Guid.NewGuid().ToString();
                    //    siteAd.site_id = site.site_id;
                    //    siteAd.site_admin_id = siteVM.site_admin_id;
                    //    db.tb_site_site_admin.Add(siteAd);
                    //    db.SaveChanges();
                    //}else
                    //{
                    //    tb_site_site_admin siteAd = db.tb_site_site_admin.Find(siteAdmin.site_site_admin_id);
                    //    siteAd.site_admin_id = siteVM.site_admin_id;
                    //    db.SaveChanges();
                    //}
                    TempData["message"] = "Your data has been updated!";
                    return RedirectToAction("Index");
                }
            //}
            TempData["message"] = "Your data is error been updating!";
            return View();
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_site site = db.tb_site.FirstOrDefault(m => m.site_id == id);
                site.status = false;
                site.updated_dated = Class.CommonClass.ToLocalTime(DateTime.Now);
                site.updated_by = User.Identity.Name;
                db.SaveChanges();
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Message(EnumConstants.MessageParameter? message)
        {
            ViewBag.StatusMessage = message == EnumConstants.MessageParameter.SaveSuccessfull ? "Your data has been saved!" :
                message==EnumConstants.MessageParameter.SaveError?"Your data is error while saving!":
                message==EnumConstants.MessageParameter.UpdateSuccessfull?"Your data has been updated!":
                message==EnumConstants.MessageParameter.UpdateError?"Your data is error while updating!":
                message==EnumConstants.MessageParameter.DeleteSuccessfull?"Your data has been deleted!":
                message==EnumConstants.MessageParameter.DeleteError?"Your data is error while deleting"
                :"";
                ViewBag.ReturnUrl = Url.Action("Message");
            return View();
        }
        public ActionResult SiteDataTable()
        {
            List<SiteViewModel> sites = new List<SiteViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var siteList = (from tbl in db.tb_site
                                orderby tbl.site_name
                                where tbl.status == true
                                select tbl).ToList();
                if (siteList.Any())
                {
                    foreach (var site in siteList)
                    {
                        sites.Add(new SiteViewModel() { site_id = site.site_id, site_name = site.site_name, site_address = site.site_address, site_code = site.site_code,create_dated=site.create_dated });
                    }
                }
            }
            return Json(new { data = sites }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult SiteDropDown()
        //{
        //    try
        //    {
        //        kim_mexEntities db = new kim_mexEntities();
        //        var sites = (from tbl in db.tb_site orderby tbl.site_name where tbl.status==true select tbl).ToList();
        //        return Json(new { data = sites }, JsonRequestBehavior.AllowGet);
        //    }catch(Exception ex) { return null; }
        //}


            //kosal 28Feb2020
        public ActionResult SiteDropDown()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<SiteViewModel> sites = new List<SiteViewModel>();
                //var project_manager_list = (from tbl in db.tb_user_detail orderby tbl.user_first_name where tbl.status == true && tbl.user_position_id == "6" && tbl.user_id==null select tbl).ToList() ;
                var sitelists = (from tbl in db.tb_site orderby tbl.site_name where tbl.status == true select tbl).ToList();
                if (sitelists.Any())
                {
                    foreach (var sitelist in sitelists)
                    {
                        sites.Add(new SiteViewModel()
                        {

                            site_id = sitelist.site_id,
                            site_name = sitelist.site_name,

                        });
                    }

                }
                return Json(new { data = sites }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult SiteSupervisorDropDownJSON()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users

                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,

                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 where role.Name == Role.SiteSupervisor
                                                 select role.Name).ToList()
                                 }).ToList().Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,

                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName)
                                 }).Where(p => p.Role == Role.SiteSupervisor).OrderBy(p => p.Username);
                return Json(new { data = userRoles }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return null;
            }

        }

    }
}