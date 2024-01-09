using Microsoft.Reporting.WebForms;
using BT_KimMex.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;
using System.IO;
using System.Net;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing.Printing;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateProject(ProjectViewModel model)
        {
            if (this.CreateProjectManager(model))
            {
                TempData["message"] = "Your data has been saved!";
                return Json(new { Message = "Success",project_id=model.project_id }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["message"] = "Your data is error while saving!";
                return Json(new { Message = "Fail", project_id = model.project_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public bool CreateProjectManager(ProjectViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_project project = new tb_project();
                project.project_id = Guid.NewGuid().ToString();
                project.project_no = model.project_no;
                project.project_short_name = model.project_short_name;
                project.project_full_name = model.project_full_name;
                project.project_start_date = model.project_start_date;
                project.project_actual_start_date = model.project_actual_start_date;
                project.project_end_date = model.project_end_date;
                project.project_actual_end_date = model.project_actual_end_date;
                project.project_address = model.project_address;
                project.cutomer_id = model.cutomer_id;
                project.cutomer_signatory = model.cutomer_signatory;
                project.cutomer_project_manager = model.cutomer_project_manager;
                project.site_id = model.site_id;
                project.project_manager = model.project_manager;
                project.project_telephone = model.project_telephone;
                project.customer_telephone = model.customer_telephone;
                project.p_status = "Active";
                project.project_status = true;
                project.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                project.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                project.created_by = User.Identity.GetUserId();
                project.updated_by = User.Identity.GetUserId();
                db.tb_project.Add(project);
                db.SaveChanges();
                tb_site_manager_project site_manager = new tb_site_manager_project();
                for (var i = 0; i < model.project_site_managers.Count(); i++)
                {
                    site_manager = new tb_site_manager_project();
                    site_manager.site_manager_project_id = Guid.NewGuid().ToString();
                    site_manager.project_id = project.project_id;
                    site_manager.site_manager = model.project_site_managers[i].site_manager.ToString();
                    db.tb_site_manager_project.Add(site_manager);
                    db.SaveChanges();
                }
                //if (!string.IsNullOrEmpty(model.project_pm_id))
                //{
                //    tb_project_pm pm = new tb_project_pm();
                //    pm.project_pm_id = Guid.NewGuid().ToString();
                //    pm.project_id = project.project_id;
                //    pm.project_manager_id = model.project_pm_id;
                //    db.tb_project_pm.Add(pm);
                //    db.SaveChanges();
                //}

                foreach(var pmID in model.projectManagers)
                {
                    if (!string.IsNullOrEmpty(pmID))
                    {
                        tb_project_pm pm = new tb_project_pm();
                        pm.project_pm_id = Guid.NewGuid().ToString();
                        pm.project_id = project.project_id;
                        pm.project_manager_id = pmID;
                        db.tb_project_pm.Add(pm);
                        db.SaveChanges();
                    }

                }

                foreach (var sitesup in model.siteSupervisors)
                {
                    if (!string.IsNullOrEmpty(sitesup.site_supervisor_id))
                    {
                        tbSiteSiteSupervisor sitesupervisor = new tbSiteSiteSupervisor();
                        sitesupervisor.site_site_supervisor_id = Guid.NewGuid().ToString();
                        sitesupervisor.site_id = project.project_id;
                        sitesupervisor.site_supervisor_id = sitesup.site_supervisor_id;
                        db.tbSiteSiteSupervisors.Add(sitesupervisor);
                        db.SaveChanges();
                    }
                }

                if (model.spacemen_att_id == null)
                {

                }
                else
                {
                    tb_spacemen_attachment spacemen_attachment = db.tb_spacemen_attachment.Where(m => m.spacemen_att_id == model.spacemen_att_id).FirstOrDefault();
                    spacemen_attachment.project_id = project.project_id;
                    db.SaveChanges();
                }

                //update warehouse project
                if (!string.IsNullOrEmpty(model.warehouse_project_id))
                {
                    tb_warehouse warehouse = db.tb_warehouse.Find(model.warehouse_project_id);
                    warehouse.warehouse_project_id = project.project_id;
                    db.SaveChanges();
                }

                /* New Enhancement 2022 Sep 12 */
                //tb_site_site_admin siteAd = new tb_site_site_admin();
                //siteAd.site_site_admin_id = Guid.NewGuid().ToString();
                //siteAd.site_id = project.project_id;
                //siteAd.site_admin_id = model.site_admin_id;
                //db.tb_site_site_admin.Add(siteAd);
                //db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                //throw ex;
            }
        }
        public JsonResult UploadAttachment()
        {
            kim_mexEntities db = new kim_mexEntities();
            tb_spacemen_attachment spacemen_att = new tb_spacemen_attachment();
            var file = Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {
                var file_name = Path.GetFileName(file.FileName);
                var file_extension = Path.GetExtension(file_name);
                var file_id = Guid.NewGuid().ToString();
                var file_path = Path.Combine(Server.MapPath("~/Documents/Spacemen Signature/"), file_id + file_extension);
                file.SaveAs(file_path);

                spacemen_att.spacemen_att_id = file_id;
                spacemen_att.spacemen_filename = file_name;
                spacemen_att.spacemen_extension = file_extension;
                spacemen_att.file_path = file_path;
                db.tb_spacemen_attachment.Add(spacemen_att);
                db.SaveChanges();
            }
            return Json(new { attachment_id = spacemen_att.spacemen_att_id },JsonRequestBehavior.AllowGet);
        }
        public FileResult Download(String p,String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/Spacemen Signature/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        [HttpPost]
        public JsonResult DeleteSpacemenAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_spacemen_attachment spacemen_attachment = db.tb_spacemen_attachment.Find(id);
                if (spacemen_attachment == null)
                {
                    Response.StatusCode= (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }
                db.tb_spacemen_attachment.Remove(spacemen_attachment);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/Spacemen Signature/"), spacemen_attachment.spacemen_att_id+spacemen_attachment.spacemen_extension);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json(new { Result = "OK" });
            }
            catch(Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public ActionResult Details(string id)
        {
            return View(CommonFunctions.GetProjectDetailbyId(id));
        }

        //public ActionResult Details(string id)
        //{
        //    ProjectViewModel project = new ProjectViewModel();
        //    using(kim_mexEntities db=new kim_mexEntities())
        //    {
        //        var projectDetail = (from tbl in db.tb_project
        //                             join st in db.tb_site on tbl.site_id equals st.site_id into s
        //                             from site in s.DefaultIfEmpty()
        //                             join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id into c from customer in c.DefaultIfEmpty()
        //                             where tbl.project_id == id
        //                             select new ProjectViewModel
        //                             {
        //                                 project_id = tbl.project_id,
        //                                 project_no = tbl.project_no,
        //                                 created_date = tbl.created_date,
        //                                 project_short_name = tbl.project_short_name,
        //                                 project_full_name = tbl.project_full_name,
        //                                 project_start_date = tbl.project_start_date,
        //                                 project_actual_start_date = tbl.project_actual_start_date,
        //                                 project_end_date = tbl.project_end_date,
        //                                 project_actual_end_date = tbl.project_actual_end_date,
        //                                 project_address = tbl.project_address,
        //                                 cutomer_id = tbl.cutomer_id,
        //                                 customer_name = customer.customer_name,
        //                                 cutomer_signatory = tbl.cutomer_signatory,
        //                                 cutomer_project_manager = tbl.cutomer_project_manager,
        //                                 site_id = tbl.site_id,
        //                                 site_name = site.site_name,
        //                                 site_code = site.site_code,
        //                                 project_manager=tbl.project_manager,
        //                                 project_telephone=tbl.project_telephone,
        //                                 customer_telephone=tbl.customer_telephone,
        //                                 p_status=tbl.p_status,
        //                             }).FirstOrDefault();
        //        if (projectDetail != null)
        //        {
        //            project.project_id = projectDetail.project_id;
        //            project.project_no = projectDetail.project_no;
        //            project.project_short_name = projectDetail.project_short_name;
        //            project.project_full_name = projectDetail.project_full_name;
        //            project.project_start_date = projectDetail.project_start_date;
        //            project.project_actual_start_date = projectDetail.project_actual_start_date;
        //            project.project_end_date = projectDetail.project_end_date;
        //            project.project_actual_end_date = projectDetail.project_actual_end_date;
        //            project.project_address = projectDetail.project_address;
        //            project.cutomer_id = projectDetail.cutomer_id;
        //            project.customer_name = projectDetail.customer_name;
        //            project.cutomer_signatory = projectDetail.cutomer_signatory;
        //            project.cutomer_project_manager = projectDetail.cutomer_project_manager;
        //            project.site_id = projectDetail.site_id;
        //            project.site_name = projectDetail.site_name;
        //            project.site_code = projectDetail.site_code;
        //            project.project_manager = projectDetail.project_manager;
        //            project.created_date = projectDetail.created_date;
        //            project.customer_telephone = projectDetail.customer_telephone;
        //            project.project_telephone = projectDetail.project_telephone;
        //            project.p_status = projectDetail.p_status;
        //            project.spacemen_att_id = projectDetail.spacemen_att_id;
        //        }

        //        List<SiteManagerViewModel> site_managers = new List<SiteManagerViewModel>();
        //        var project_site_managers = (from tbl in db.tb_site_manager_project where tbl.project_id == id select tbl);
        //        if (project_site_managers.Any())
        //        {
        //            foreach(var site_manager in project_site_managers)
        //            {
        //                SiteManagerViewModel siteManager = new SiteManagerViewModel();
        //                var sm = db.tb_user_detail.Where(x => x.user_id == site_manager.site_manager).FirstOrDefault();
        //                //siteManager.site_manager = site_manager.site_manager;
        //                if (sm!=null)
        //                {
        //                    siteManager.site_manager = sm.user_first_name + " " + sm.user_last_name;
        //                    site_managers.Add(siteManager);
        //                }
        //            }
        //        }
        //        project.project_site_managers = site_managers;
        //        var spacemen_attachment = (from tbl in db.tb_spacemen_attachment where tbl.project_id == id select tbl).FirstOrDefault();
        //        if (spacemen_attachment!=null)
        //        {
        //            project.spacemen_att_id = spacemen_attachment.spacemen_att_id;
        //            project.spacemen_filename = spacemen_attachment.spacemen_filename;
        //            project.spacemen_extension = spacemen_attachment.spacemen_extension;
        //            project.file_path = spacemen_attachment.file_path;
        //        }
        //        project.boq_id = db.tb_build_of_quantity.Where(m => m.status == true && m.boq_status == "Completed" && m.project_id == id).Select(x => x.boq_id).FirstOrDefault();

        //        project.projectProjectManagers = db.tb_project_pm.Where(w => string.Compare(w.project_id, project.project_id) == 0).Select(s => new ProjectPMViewModel()
        //        {
        //            project_pm_id=s.project_pm_id,
        //            project_id=s.project_id,
        //            project_manager_id=s.project_manager_id,

        //        }).ToList();
        //        //Site supervisor
        //        var sitesups = (from sitesup in db.tbSiteSiteSupervisors where string.Compare(sitesup.site_id, project.project_id) == 0 select new { sitesup }).ToList();
        //                       foreach(var sitesup in sitesups)
        //        {
        //            var userdetail = db.tb_user_detail.Where(x => x.user_id == sitesup.sitesup.site_supervisor_id).FirstOrDefault();
        //            if (userdetail != null)
        //            {
        //                project.siteSupervisors.Add(new SiteSiteSupervisorViewModel() { site_supervisor_id = userdetail.user_first_name + " " + userdetail.user_last_name });
        //            }
        //        }
        //                       //site Admin
        //        var siteAdmins = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, project.project_id) == 0).ToList();
        //        foreach (var siteadmin in siteAdmins)
        //        {
        //            var userdetail = db.tb_user_detail.Where(s => string.Compare(s.user_id, siteadmin.site_admin_id) == 0).FirstOrDefault();
        //            if (userdetail != null)
        //                project.siteAdmins.Add(new SiteSiteAdminViewModel() { site_admin_id = userdetail.user_first_name + " " + userdetail.user_last_name });
        //        }
        //        //Site Stock Keeper
        //        var siteStockKeepers = (from skeeper in db.tb_stock_keeper_warehouse
        //                                join wh in db.tb_warehouse on skeeper.warehouse_id equals wh.warehouse_id
        //                                //where string.Compare(wh.warehouse_site_id, project.site_id) == 0
        //                                where string.Compare(wh.warehouse_project_id,project.project_id)==0
        //                                select new { skeeper }).ToList();
        //        foreach(var stockstockkeeper in siteStockKeepers)
        //        {
        //            var userdetail = db.tb_user_detail.Where(s => string.Compare(s.user_id, stockstockkeeper.skeeper.stock_keeper) == 0).FirstOrDefault();
        //            if (userdetail != null)
        //                project.warehouseStockKeepers.Add(new tb_stock_keeper_warehouse() { stock_keeper = userdetail.user_first_name + " " + userdetail.user_last_name });
        //        }
        //        //qaqc 
        //        var qaqcs = (from qaqc in db.tb_warehouse_qaqc
        //                     join wh in db.tb_warehouse on qaqc.warehouse_id equals wh.warehouse_id
        //                     //where string.Compare(wh.warehouse_site_id, project.site_id) == 0
        //                     where string.Compare(wh.warehouse_project_id, project.project_id) == 0
        //                     select new { qaqc }).ToList();
        //        foreach(var qaqc in qaqcs)
        //        {
        //            var userdetail = db.tb_user_detail.Where(s => string.Compare(s.user_id, qaqc.qaqc.qaqc_id) == 0).FirstOrDefault();
        //            if (userdetail != null)
        //                project.warehouseQAQCs.Add(new WarehouseQAQCViewModel() { qaqc_id = userdetail.user_first_name + " " + userdetail.user_last_name });
        //        }

        //    }
        //    return View(project);
        //}
        //public ActionResult Edit(string id)
        //{
        //    ProjectViewModel project = new ProjectViewModel();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        var projectDetail = (from tbl in db.tb_project
        //                             join st in db.tb_site on tbl.site_id equals st.site_id into s
        //                             from site in s.DefaultIfEmpty()
        //                             join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id into c
        //                             from customer in c.DefaultIfEmpty()
        //                             join wh in db.tb_warehouse on tbl.project_id equals wh.warehouse_project_id into w from wh in w.DefaultIfEmpty()
        //                             where tbl.project_id == id
        //                             select new ProjectViewModel
        //                             {
        //                                 project_id = tbl.project_id,
        //                                 project_no = tbl.project_no,
        //                                 created_date = tbl.created_date,
        //                                 project_short_name = tbl.project_short_name,
        //                                 project_full_name = tbl.project_full_name,
        //                                 project_start_date = tbl.project_start_date,
        //                                 project_actual_start_date = tbl.project_actual_start_date,
        //                                 project_end_date = tbl.project_end_date,
        //                                 project_actual_end_date = tbl.project_actual_end_date,
        //                                 project_address = tbl.project_address,
        //                                 cutomer_id = tbl.cutomer_id,
        //                                 customer_name = customer.customer_name,
        //                                 cutomer_signatory = tbl.cutomer_signatory,
        //                                 cutomer_project_manager = tbl.cutomer_project_manager,
        //                                 site_id = tbl.site_id,
        //                                 site_name = site.site_name,
        //                                 site_code = site.site_code,
        //                                 project_manager = tbl.project_manager,
        //                                 project_telephone = tbl.project_telephone,
        //                                 customer_telephone = tbl.customer_telephone,
        //                                 p_status=tbl.p_status,
        //                                 warehouse_project_id=wh.warehouse_id,
        //                                 warehouse_project_name=wh.warehouse_name,
        //                             }).FirstOrDefault();
        //        if (projectDetail != null)
        //        {
        //            project.project_id = projectDetail.project_id;
        //            project.project_no = projectDetail.project_no;
        //            project.project_short_name = projectDetail.project_short_name;
        //            project.project_full_name = projectDetail.project_full_name;
        //            project.project_start_date = projectDetail.project_start_date;
        //            project.project_actual_start_date = projectDetail.project_actual_start_date;
        //            project.project_end_date = projectDetail.project_end_date;
        //            project.project_actual_end_date = projectDetail.project_actual_end_date;
        //            project.project_address = projectDetail.project_address;
        //            project.cutomer_id = projectDetail.cutomer_id;
        //            project.customer_name = projectDetail.customer_name;
        //            project.cutomer_signatory = projectDetail.cutomer_signatory;
        //            project.cutomer_project_manager = projectDetail.cutomer_project_manager;
        //            project.site_id = projectDetail.site_id;
        //            project.site_name = projectDetail.site_name;
        //            project.site_code = projectDetail.site_code;
        //            project.project_manager = projectDetail.project_manager;
        //            project.created_date = projectDetail.created_date;
        //            project.customer_telephone = projectDetail.customer_telephone;
        //            project.project_telephone = projectDetail.project_telephone;
        //            project.p_status = projectDetail.p_status;
        //            project.warehouse_project_id = projectDetail.warehouse_project_id;
        //            project.warehouse_project_name = projectDetail.warehouse_project_name;
        //        }
        //        List<SiteManagerViewModel> site_managers = new List<SiteManagerViewModel>();
        //        var project_site_managers = (from tbl in db.tb_site_manager_project where tbl.project_id == id select tbl);
        //        if (project_site_managers.Any())
        //        {
        //            foreach (var site_manager in project_site_managers)
        //            {
        //                SiteManagerViewModel siteManager = new SiteManagerViewModel();
        //                siteManager.site_manager = site_manager.site_manager;
        //                site_managers.Add(siteManager);
        //            }
        //        }
        //        project.project_site_managers = site_managers;
        //        var spacemen_attachment = (from tbl in db.tb_spacemen_attachment where tbl.project_id == id select tbl).FirstOrDefault();
        //        if (spacemen_attachment != null)
        //        {
        //            project.spacemen_att_id = spacemen_attachment.spacemen_att_id;
        //            project.spacemen_filename = spacemen_attachment.spacemen_filename;
        //            project.spacemen_extension = spacemen_attachment.spacemen_extension;
        //            project.file_path = spacemen_attachment.file_path;
        //        }
        //        project.projectProjectManagers = db.tb_project_pm.Where(w => string.Compare(w.project_id, project.project_id) == 0).Select(s => new ProjectPMViewModel()
        //        {
        //            project_pm_id = s.project_pm_id,
        //            project_id = s.project_id,
        //            project_manager_id = s.project_manager_id,

        //        }).ToList();


        //        var sitesups = db.tbSiteSiteSupervisors.Where(s => string.Compare(s.site_id, project.project_id) == 0).ToList();
        //        foreach (var sitesup in sitesups)
        //        {
        //            SiteSiteSupervisorViewModel ss = new SiteSiteSupervisorViewModel();
        //            ss.site_supervisor_id = sitesup.site_supervisor_id;
        //            var ud = db.tb_user_detail.Where(s => string.Compare(s.user_id, sitesup.site_supervisor_id) == 0).FirstOrDefault();
        //            if (ud == null)
        //            {
        //                ss.site_supervisor_name = string.Empty;       
        //            }
        //            else
        //            {
        //                ss.site_supervisor_name = ud.user_first_name + " " + ud.user_last_name;
        //                project.siteSupervisors.Add(ss);
        //            }
                        
                    
        //        }
        //        project.siteAdmin = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, project.project_id) == 0).Select(s => new SiteSiteAdminViewModel()
        //        {
        //            site_admin_id = s.site_admin_id,
        //        }).FirstOrDefault();
        //        if (project.siteAdmin != null)
        //            project.site_admin_id = project.siteAdmin.site_admin_id;

        //    }
        //    return View(project);
        //}

            public ActionResult Edit(string id)
        {
            return View(CommonFunctions.GetProjectDetailbyId(id));
        }

        [HttpPost]
        public ActionResult EditProject(ProjectViewModel projectVM,string id)
        {
            if (this.EditProjectTransaction(projectVM, id))
            {
                TempData["message"] = "Your data has been updated!";
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["message"] = "Your data is error while updating!";
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public bool EditProjectTransaction(ProjectViewModel model,string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_project project = db.tb_project.FirstOrDefault(p => p.project_id == id);
                project.project_no = model.project_no;
                project.project_short_name = model.project_short_name;
                project.project_full_name = model.project_full_name;
                project.project_start_date = model.project_start_date;
                project.project_actual_start_date = model.project_actual_start_date;
                project.project_end_date = model.project_end_date;
                project.project_actual_end_date = model.project_actual_end_date;
                project.project_address = model.project_address;
                project.cutomer_id = model.cutomer_id;
                project.cutomer_signatory = model.cutomer_signatory;
                project.cutomer_project_manager = model.cutomer_project_manager;
                project.site_id = model.site_id;
                project.project_manager = model.project_manager;
                project.project_telephone = model.project_telephone;
                project.customer_telephone = model.customer_telephone;
                project.p_status = model.p_status;
                project.project_status = true;
                project.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                project.updated_by = User.Identity.Name;
                db.SaveChanges();
                //remove old site manager 
                var project_site_managers = (from tbl in db.tb_site_manager_project where tbl.project_id == project.project_id select tbl).ToList();
                if (project_site_managers.Any())
                {
                    foreach(tb_site_manager_project site_manager in project_site_managers)
                    {
                        tb_site_manager_project remove_site_manager = db.tb_site_manager_project.Find(site_manager.site_manager_project_id);
                        db.tb_site_manager_project.Remove(remove_site_manager);
                        db.SaveChanges();

                    }
                }
                //add new site manager
                tb_site_manager_project site_manager_project = new tb_site_manager_project();
                for (var i = 0; i < model.project_site_managers.Count(); i++)
                {
                    site_manager_project = new tb_site_manager_project();
                    site_manager_project.site_manager_project_id = Guid.NewGuid().ToString();
                    site_manager_project.project_id = project.project_id;
                    site_manager_project.site_manager = model.project_site_managers[i].site_manager.ToString();
                    db.tb_site_manager_project.Add(site_manager_project);
                    db.SaveChanges();
                }
                if (model.spacemen_att_id == null)
                {
                    
                }else
                {
                    tb_spacemen_attachment spacemen_attachment = db.tb_spacemen_attachment.Where(m => m.spacemen_att_id == model.spacemen_att_id).FirstOrDefault();
                    spacemen_attachment.project_id = project.project_id;
                    db.SaveChanges();
                }
                //project manager
                var pms = db.tb_project_pm.Where(s => string.Compare(s.project_id, project.project_id) == 0).ToList();
                foreach(var pm in pms)
                {
                    tb_project_pm ppm = db.tb_project_pm.Find(pm.project_pm_id);
                    db.tb_project_pm.Remove(ppm);
                    db.SaveChanges();
                }

                foreach (var pmId in model.projectManagers)
                {
                    
                    if (!string.IsNullOrEmpty(pmId))
                    {
                        tb_project_pm pmm = new tb_project_pm();
                        pmm.project_pm_id = Guid.NewGuid().ToString();
                        pmm.project_id = project.project_id;
                        pmm.project_manager_id = pmId;
                        db.tb_project_pm.Add(pmm);
                        db.SaveChanges();
                    }
                }
                
                //var pm = db.tb_project_pm.Where(s => string.Compare(s.project_id, project.project_id) == 0).FirstOrDefault();
                //if (!string.IsNullOrEmpty(model.project_pm_id))
                //{
                //    if (pm == null)
                //    {
                //        tb_project_pm pmm = new tb_project_pm();
                //        pmm.project_pm_id = Guid.NewGuid().ToString();
                //        pmm.project_id = project.project_id;
                //        pmm.project_manager_id = model.project_pm_id;
                //        db.tb_project_pm.Add(pmm);
                //        db.SaveChanges();
                //    }
                //    else
                //    {
                //        tb_project_pm pmm = db.tb_project_pm.Find(pm.project_pm_id);
                //        pmm.project_manager_id = model.project_pm_id;
                //        db.SaveChanges();
                //    }
                //}

                //update warehouse project
                if (!string.IsNullOrEmpty(model.warehouse_project_id))
                {
                    tb_warehouse warehouse = db.tb_warehouse.Find(model.warehouse_project_id);
                    warehouse.warehouse_project_id = project.project_id;
                    db.SaveChanges();
                }

                if (model.siteSupervisors != null)
                {
                    var oldSiteSups = db.tbSiteSiteSupervisors.Where(s => s.site_id == project.project_id).ToList();
                    foreach (var oldsitesup in oldSiteSups)
                    {
                        var sid = oldsitesup.site_site_supervisor_id;
                        tbSiteSiteSupervisor dd = db.tbSiteSiteSupervisors.Find(sid);
                        db.tbSiteSiteSupervisors.Remove(dd);
                        db.SaveChanges();
                    }
                    for (int i = 0; i < model.siteSupervisors.Count; i++)
                    {
                        tbSiteSiteSupervisor sitesupervisor = new tbSiteSiteSupervisor();
                        sitesupervisor.site_site_supervisor_id = Guid.NewGuid().ToString();
                        sitesupervisor.site_id =project.project_id;
                        sitesupervisor.site_supervisor_id = model.siteSupervisors[i].site_supervisor_id;
                        db.tbSiteSiteSupervisors.Add(sitesupervisor);
                        db.SaveChanges();
                    }

                }

                //site admin
                /* New Enhancement 2022 Sep 12 */
                var siteAdmin = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, project.project_id) == 0).FirstOrDefault();
                if (siteAdmin == null)
                {
                    tb_site_site_admin siteAd = new tb_site_site_admin();
                    siteAd.site_site_admin_id = Guid.NewGuid().ToString();
                    siteAd.site_id = project.project_id;
                    siteAd.site_admin_id = model.site_admin_id;
                    db.tb_site_site_admin.Add(siteAd);
                    db.SaveChanges();
                }
                else
                {
                    tb_site_site_admin siteAd = db.tb_site_site_admin.Find(siteAdmin.site_site_admin_id);
                    siteAd.site_admin_id = model.site_admin_id;
                    db.SaveChanges();
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_project project = db.tb_project.FirstOrDefault(m => m.project_id == id);
                project.project_status = false;
                project.updated_by = User.Identity.Name;
                project.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                TempData["message"] = "Your data has been deleted!";
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                TempData["message"] = "Your data is error while deleting!";
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateProjectStatus(string id,string p_status)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_project project = db.tb_project.FirstOrDefault(m => m.project_id == id);
                project.p_status = p_status;
                project.updated_by = User.Identity.Name;
                project.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                TempData["message"] = "Your data has been updated!";
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                TempData["message"] = "Your data is error while updating!";
                return Json(new { result = "fail",message=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Message(EnumConstants.MessageParameter? message)
        {
            ViewBag.StatusMessage = message == EnumConstants.MessageParameter.SaveSuccessfull ? "Your data has been saved!" :
                message == EnumConstants.MessageParameter.SaveError ? "Your data is error while saving!" :
                message == EnumConstants.MessageParameter.UpdateSuccessfull ? "Your data has been updated!" :
                message == EnumConstants.MessageParameter.UpdateError ? "Your data is error while updating!" :
                message == EnumConstants.MessageParameter.DeleteSuccessfull ? "Your data has been deleted!" :
                message == EnumConstants.MessageParameter.DeleteError ? "Your data is error while deleting"
                : "";
            ViewBag.ReturnUrl = Url.Action("Message");
            return View();
        }
        public ActionResult ProjectDataTable(string p_status)
        {
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                IQueryable<ProjectViewModel> projectList;
                if (p_status == "All")
                {
                    projectList = (from tbl in db.tb_project
                                       //join site in db.tb_site on tbl.site_id equals site.site_id
                                   join st in db.tb_site on tbl.site_id equals st.site_id into s from site in s.DefaultIfEmpty()
                                   join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id into c from customer in c.DefaultIfEmpty()
                                   join wh in db.tb_warehouse on tbl.project_id equals wh.warehouse_project_id into w from wh in w.DefaultIfEmpty()
                                       orderby tbl.created_date
                                       where tbl.project_status==true
                                       select new ProjectViewModel
                                       {
                                           project_id = tbl.project_id,
                                           project_no = tbl.project_no,
                                           created_date = tbl.created_date,
                                           project_short_name = tbl.project_short_name,
                                           project_full_name = tbl.project_full_name,
                                           project_start_date = tbl.project_start_date,
                                           project_actual_start_date = tbl.project_actual_start_date,
                                           project_end_date = tbl.project_end_date,
                                           project_actual_end_date = tbl.project_actual_end_date,
                                           project_address = tbl.project_address,
                                           cutomer_id = tbl.cutomer_id,
                                           customer_name = customer.customer_name,
                                           cutomer_signatory = tbl.cutomer_signatory,
                                           cutomer_project_manager = tbl.project_manager,
                                           project_manager = tbl.project_manager,
                                           site_id = tbl.site_id,
                                           site_name = site.site_name,
                                           site_code = site.site_code,
                                           site_manager_1 = tbl.site_manager_1,
                                           site_manager_2 = tbl.site_manager_2,
                                           project_telephone = tbl.project_telephone,
                                           customer_telephone = tbl.customer_telephone,
                                           p_status = tbl.p_status,
                                           warehouse_project_id=wh.warehouse_id,
                                           warehouse_project_name=wh.warehouse_name,
                                           isBOQ = db.tb_build_of_quantity.Where(x => x.status == true && x.boq_status == "Completed" && x.project_id == tbl.project_id).FirstOrDefault() != null ? true : false,
                                       });
                }
                else
                {
                    projectList = (from tbl in db.tb_project
                                   join st in db.tb_site on tbl.site_id equals st.site_id into s
                                   from site in s.DefaultIfEmpty()
                                   join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id into c
                                   from customer in c.DefaultIfEmpty()
                                   join wh in db.tb_warehouse on tbl.project_id equals wh.warehouse_project_id into w
                                   from wh in w.DefaultIfEmpty()
                                   orderby tbl.created_date
                                   where tbl.p_status==p_status && tbl.project_status==true
                                   select new ProjectViewModel
                                   {
                                       project_id = tbl.project_id,
                                       project_no = tbl.project_no,
                                       created_date = tbl.created_date,
                                       project_short_name = tbl.project_short_name,
                                       project_full_name = tbl.project_full_name,
                                       project_start_date = tbl.project_start_date,
                                       project_actual_start_date = tbl.project_actual_start_date,
                                       project_end_date = tbl.project_end_date,
                                       project_actual_end_date = tbl.project_actual_end_date,
                                       project_address = tbl.project_address,
                                       cutomer_id = tbl.cutomer_id,
                                       customer_name = customer.customer_name,
                                       cutomer_signatory = tbl.cutomer_signatory,
                                       cutomer_project_manager = tbl.project_manager,
                                       project_manager = tbl.project_manager,
                                       site_id = tbl.site_id,
                                       site_name = site.site_name,
                                       site_code = site.site_code,
                                       site_manager_1 = tbl.site_manager_1,
                                       site_manager_2 = tbl.site_manager_2,
                                       project_telephone = tbl.project_telephone,
                                       customer_telephone = tbl.customer_telephone,
                                       p_status = tbl.p_status,
                                       warehouse_project_id = wh.warehouse_id,
                                       warehouse_project_name = wh.warehouse_name,
                                       isBOQ = db.tb_build_of_quantity.Where(x => x.status == true && x.boq_status == "Completed" && x.project_id == tbl.project_id).FirstOrDefault() != null ? true : false,
                                   });
                }
                if (projectList.Any())
                {
                    foreach (var project in projectList)
                    {
                        projects.Add(new ProjectViewModel() {
                            project_id = project.project_id,
                            project_no=project.project_no,
                            project_short_name = project.project_short_name,
                            project_full_name=project.project_full_name,
                            project_start_date=project.project_start_date,
                            project_actual_start_date=project.project_actual_start_date,
                            project_end_date=project.project_end_date,
                            project_actual_end_date=project.project_actual_end_date,
                            project_address = project.project_address,
                            cutomer_id = project.cutomer_id,
                            customer_name = project.customer_name,
                            cutomer_signatory = project.cutomer_signatory,
                            cutomer_project_manager = project.project_manager,
                            site_id = project.site_id,
                            site_name = project.site_name,
                            site_code = project.site_code,
                            site_manager_1 = project.site_manager_1,
                            site_manager_2 = project.site_manager_2,
                            project_telephone=project.project_telephone,
                            customer_telephone=project.customer_telephone,
                            project_manager=project.project_manager,
                            p_status=project.p_status,
                            spacemen_att_id=project.spacemen_att_id,
                            isBOQ = project.isBOQ,
                            warehouse_project_id = project.warehouse_project_id,
                            warehouse_project_name = project.warehouse_project_name,
                            boq_id = db.tb_build_of_quantity.Where(x => x.status == true && x.boq_status == "Completed" && x.project_id == project.project_id).Select(x => x.boq_id).FirstOrDefault()
                        });
                    }
                }

            }
            catch (Exception ex) { }
            return Json(new { data = projects }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProjectManagerDropDown()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<UserViewModel> projectManagerListItems = new List<UserViewModel>();
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users
                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,
                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 where role.Name == "Project Manager"
                                                 select role.Name).ToList()
                                 }).ToList().Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,
                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName)
                                 }).Where(p => p.Role == "Project Manager").OrderBy(p => p.Username);
                foreach (var item in userRoles)
                {
                    var user = db.tb_user_detail.Where(s => string.Compare(s.user_id, item.UserId) == 0).FirstOrDefault();
                    projectManagerListItems.Add(new UserViewModel()
                    {
                        user_detail_id = user.user_detail_id,
                        user_first_name = user.user_first_name,
                        user_last_name = user.user_last_name,
                        user_telephone = user.user_telephone,
                        user_email = user.user_email,
                        user_id = user.user_id,
                    });
                }
                return Json(new { data = projectManagerListItems }, JsonRequestBehavior.AllowGet);



                //ApplicationDbContext context = new ApplicationDbContext();
                //var userRoles = (from user in context.Users select new
                //                     {
                //                         UserId = user.Id,
                //                         Username = user.UserName,
                //                         Email = user.Email,
                //                         RoleName = (from userRole in user.Roles
                //                                     join role in context.Roles on userRole.RoleId equals role.Id
                //                                     select role.Name).ToList()
                //                     }).ToList().Select(p => new UserRolesViewModel()
                //                                       {
                //                                           UserId = p.UserId,
                //                                           Username = p.Username,
                //                                           Email = p.Email,
                //                                           Role = string.Join(",",p.RoleName)
                //                                       }).Where(p=>p.Role=="Project Manager").OrderBy(p=>p.Username);
                //return Json(new { data = userRoles }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return null; }
        }
        public ActionResult ProjectManagerDropdownList()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<UserViewModel> project_managers = new List<UserViewModel>();
                //var project_manager_list = (from tbl in db.tb_user_detail orderby tbl.user_first_name where tbl.status == true && tbl.user_position_id == "6" && tbl.user_id==null select tbl).ToList() ;
                var project_manager_list = (from tbl in db.tb_user_detail orderby tbl.user_first_name where tbl.status == true && tbl.user_position_id == "6" select tbl).ToList();
                if (project_manager_list.Any())
                {
                    foreach(var project_manager in project_manager_list)
                    {
                        project_managers.Add(new UserViewModel() { user_detail_id = project_manager.user_detail_id, user_first_name = project_manager.user_first_name, user_last_name = project_manager.user_last_name, user_telephone = project_manager.user_telephone, user_email = project_manager.user_email });
                    }
                }
                return Json(new { data = project_managers }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return null;
            }
        }
        public ActionResult SiteManagerDropDown()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<UserViewModel> siteManagerListItems = new List<UserViewModel>();
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users
                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,
                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 where role.Name == "Site Manager"
                                                 select role.Name).ToList()
                                 }).ToList().Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,
                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName)
                                 }).Where(p => p.Role == "Site Manager").OrderBy(p => p.Username);
                foreach(var item in userRoles)
                {
                    var user = db.tb_user_detail.Where(s => string.Compare(s.user_id, item.UserId) == 0).FirstOrDefault();
                    siteManagerListItems.Add(new UserViewModel()
                    {
                        user_detail_id=user.user_detail_id,
                        user_first_name=user.user_first_name,
                        user_last_name = user.user_last_name,
                        user_telephone = user.user_telephone,
                        user_email = user.user_email,
                        user_id=user.user_id,
                    });
                }
                return Json(new { data = siteManagerListItems }, JsonRequestBehavior.AllowGet);


                //kim_mexEntities db = new kim_mexEntities();
                //List<UserViewModel> project_managers = new List<UserViewModel>();
                //var project_manager_list = (from tbl in db.tb_user_detail orderby tbl.user_first_name where tbl.status == true && tbl.user_position_id == "5" && tbl.user_id==null select tbl).ToList();
                //if (project_manager_list.Any())
                //{
                //    foreach (var project_manager in project_manager_list)
                //    {
                //        project_managers.Add(new UserViewModel() { user_detail_id = project_manager.user_detail_id, user_first_name = project_manager.user_first_name, user_last_name = project_manager.user_last_name, user_telephone = project_manager.user_telephone, user_email = project_manager.user_email });
                //    }
                //}
                //return Json(new { data = project_managers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return null; }
        }
        public ActionResult ProjectNo()
        {
            try
            {
                string last_no="", project_no;
                kim_mexEntities db = new kim_mexEntities();
                var number = (from tbl in db.tb_project orderby tbl.created_date descending select tbl.project_no).FirstOrDefault();
                if (number == null)
                    last_no = "001";
                else
                {
                    number = number.Substring(number.Length - 3, 3);
                    int num = Convert.ToInt32(number)+1;
                    if (num.ToString().Length == 1)
                        last_no = "00" + num;
                    else if (num.ToString().Length == 2)
                        last_no = "0" + num;
                    else if (num.ToString().Length == 3)
                        last_no = num.ToString();
                }
                string dd = Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString().Length==1? "0"+ Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString(): Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString();
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length==1?"0"+ Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString(): Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                project_no = "Pjr-"+dd+"-"+mm+"-"+last_no;
                return Json(new { data = project_no }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex) { return null; }
        }
        public ActionResult ProjectDropdownList(string p_status)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<ProjectViewModel> projects = new List<ProjectViewModel>();
                
                var project_list = (from tbl in db.tb_project orderby tbl.project_short_name where tbl.p_status == "Active" && tbl.project_status==true select tbl).ToList();
                if (project_list.Any())
                {
                    foreach(var project in project_list)
                    {
                        projects.Add(new ProjectViewModel() { project_id = project.project_id, project_short_name = project.project_short_name,project_full_name=project.project_full_name });
                    }
                }
                return Json(new { data = projects }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return null;
            }
        }
        public ActionResult ProjectInfoDetail(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var project_detail = (from tbl in db.tb_project
                                      join site in db.tb_site on tbl.site_id equals site.site_id
                                      join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id into c
                                      from customer in c.DefaultIfEmpty()
                                      orderby tbl.created_date
                                   where tbl.project_id==id
                                   select new ProjectViewModel
                                   {
                                       project_id = tbl.project_id,
                                       project_no = tbl.project_no,
                                       created_date = tbl.created_date,
                                       project_short_name = tbl.project_short_name,
                                       project_full_name = tbl.project_full_name,
                                       project_start_date = tbl.project_start_date,
                                       project_actual_start_date = tbl.project_actual_start_date,
                                       project_end_date = tbl.project_end_date,
                                       project_actual_end_date = tbl.project_actual_end_date,
                                       project_address = tbl.project_address,
                                       cutomer_id = tbl.cutomer_id,
                                       customer_name = customer.customer_name,
                                       cutomer_signatory = tbl.cutomer_signatory,
                                       cutomer_project_manager = tbl.project_manager,
                                       project_manager = tbl.project_manager,
                                       site_id = tbl.site_id,
                                       site_name = site.site_name,
                                       site_code = site.site_code,
                                       site_manager_1 = tbl.site_manager_1,
                                       site_manager_2 = tbl.site_manager_2,
                                       project_telephone = tbl.project_telephone,
                                       customer_telephone = tbl.customer_telephone,
                                       p_status=tbl.p_status,
                                       
                                   }).FirstOrDefault();
                return Json(new { data = project_detail }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        Project ds = new Project();
        public ActionResult ReportProject()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.AsyncRendering = false;
            reportViewer.ShowPageNavigationControls = false;
            System.Web.UI.ScriptManager scriptManager = new System.Web.UI.ScriptManager();
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);

            System.Drawing.Printing.PageSettings AlmostA4 = new System.Drawing.Printing.PageSettings(); AlmostA4.PaperSize = new System.Drawing.Printing.PaperSize("CustomType", 17, 12);
            reportViewer.SetPageSettings(AlmostA4);

            using (kim_mexEntities db=new kim_mexEntities())
            {
                var obj = db.tb_project.Where(x => x.project_status == true).ToList();
                DataTable tb = new DataTable();
                if (obj.Any())
                {
                    DataRow dtr;
                    DataColumn col = new DataColumn();
                    col.ColumnName = "project_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "project_no";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "project_short_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "project_full_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "project_start_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "project_actual_start_date";
                    tb.Columns.Add(col);

                    foreach(var pro in obj)
                    {
                        dtr = tb.NewRow();
                        dtr["project_id"] = pro.project_id;
                        dtr["project_no"] = pro.project_no;
                        dtr["project_short_name"] = pro.project_short_name;
                        dtr["project_full_name"] = pro.project_full_name;
                        dtr["project_start_date"] = pro.project_start_date;
                        dtr["project_actual_start_date"] = pro.project_actual_start_date;
                        tb.Rows.Add(dtr);
                    }

                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Project.rdlc";

                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Project", tb));
                    
                    reportViewer.LocalReport.Refresh();
                    ViewBag.ReportViewer = reportViewer;
                }
            }

            return View();
        }
        public ActionResult ProjectList()
        {
            return View();
        }
        public ActionResult GetWarehousebyProjectJson(string id)
        {
            return Json(new { data = CommonFunctions.GetWarehousebyProject(id) }, JsonRequestBehavior.AllowGet);
        }

    }
}