using BT_KimMex.Class;
using BT_KimMex.Entities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ProjectViewModel
    {
        [Key]
        public string project_id { get; set; }
        [Display(Name ="Project No.")]
        public string project_no { get; set; }

        [Display(Name ="Project Short Name:")]
        [Required(ErrorMessage ="Project short name is required.")]
        public string project_short_name { get; set; }
        [Required(ErrorMessage ="Project full name is required.")]
        [Display(Name ="Project Full Name:")]
        public string project_full_name { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name ="Start Date:")]
        public Nullable<System.DateTime> project_start_date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name ="Actual Start Date:")]
        public Nullable<System.DateTime> project_actual_start_date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name ="End Date:")]
        public Nullable<System.DateTime> project_end_date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name ="Actual End Date:")]
        public Nullable<System.DateTime> project_actual_end_date { get; set; }
        [Display(Name ="Address:")]
        public string project_address { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name ="Date")]
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        [Display(Name ="Customer:")]
        //[Required(ErrorMessage ="Customer is required.")]
        public string cutomer_id { get; set; }
        [Display(Name ="Customer Signatory:")]
        public string cutomer_signatory { get; set; }
        [Display(Name ="Customer Project Manager:")]
        public string cutomer_project_manager { get; set; }
        [Display(Name ="Site Name:")]
        //[Required(ErrorMessage ="Site name is required.")]
        public string site_id { get; set; }
        [Display(Name ="Project Engineer:")]
        //[Required(ErrorMessage ="Project Manager is required.")]
        public string project_manager { get; set; }
        [Display(Name ="Site Manager 1:")]
        public string site_manager_1 { get; set; }
        [Display(Name ="Site Manager 2:")]
        public string site_manager_2 { get; set; }
        public Nullable<bool> project_status { get; set; }
        public string customer_name { get; set; }
        public string site_name { get; set; }
        public string site_code { get; set; }
        [Display(Name ="Telephone:")]
        public string project_telephone { get; set; }
        [Display(Name ="Telephone:")]
        public string customer_telephone { get; set; }
        [Display(Name ="Project Status:")]
        public string p_status { get; set; }
        public string spacemen_att_id { get; set; }
        public string spacemen_filename { get; set; }
        public string spacemen_extension { get; set; }
        public string file_path { get; set; }
        public bool isBOQ { get; set; }
        public string boq_id { get; set; }
        public string project_pm_id { get; set; }
        public string warehouse_project_id { get; set; }
        public string warehouse_project_name { get; set; }
        public string site_admin_id { get; set; }
        public string site_admin_name { get; set; }
        public SiteSiteAdminViewModel siteAdmin { get; set; }
        public List<SiteManagerViewModel> project_site_managers { get; set; }
        public List<ProjectPMViewModel> projectProjectManagers { get; set; }
        public List<SiteSiteSupervisorViewModel> siteSupervisors { get; set; }
        public List<SiteSiteAdminViewModel> siteAdmins { get; set; }
        public List<Entities.tb_stock_keeper_warehouse> warehouseStockKeepers { get; set; }
        public List<WarehouseQAQCViewModel> warehouseQAQCs { get; set; }
        public string[] projectManagers { get; set; }
        
        public ProjectViewModel()
        {
            project_site_managers = new List<SiteManagerViewModel>();
            siteSupervisors = new List<SiteSiteSupervisorViewModel>();
            siteAdmins = new List<SiteSiteAdminViewModel>();
            warehouseStockKeepers = new List<Entities.tb_stock_keeper_warehouse>();
            warehouseQAQCs = new List<WarehouseQAQCViewModel>();
            siteAdmin = new SiteSiteAdminViewModel();
        }

        public static List<ProjectViewModel> GetProjectListItemsBySiteSupervisor(bool isAdmin,string userId="")
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (isAdmin)
                {
                    return db.tb_project.OrderBy(s => s.project_full_name).Where(s => s.project_status == true && string.Compare(s.p_status, "Active") == 0).Select(s => new ProjectViewModel() { project_id = s.project_id, project_full_name = s.project_full_name }).ToList();
                }
                else
                {
                    return (from pro in db.tb_project
                                //join site in db.tb_site on pro.site_id equals site.site_id
                            join sitesupv in db.tbSiteSiteSupervisors on pro.project_id equals sitesupv.site_id
                            where pro.project_status == true && string.Compare(pro.p_status, "Active") == 0 && string.Compare(sitesupv.site_supervisor_id, userId) == 0
                            select new ProjectViewModel()
                            {
                                project_id = pro.project_id,
                                project_full_name = pro.project_full_name
                            }).ToList();
                }
            }
        }
        public static List<ProjectViewModel> GetProjectListItemByProjectManager(string userId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from proj in db.tb_project
                        join pm in db.tb_project_pm on proj.project_id equals pm.project_id
                        orderby proj.project_full_name
                        where proj.project_status == true && string.Compare(proj.p_status, "Active") == 0 && string.Compare(pm.project_manager_id, userId) == 0
                        select new ProjectViewModel()
                        {
                            project_id=proj.project_id,
                            project_full_name=proj.project_full_name,

                        }).ToList();
            }
        }
        public static List<ProjectViewModel> GetProjectListItemBySiteManager(string userId)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from proj in db.tb_project
                        join sm in db.tb_site_manager_project on proj.project_id equals sm.project_id
                        orderby proj.project_full_name
                        where proj.project_status == true && string.Compare(proj.p_status, "Active") == 0 && string.Compare(sm.site_manager, userId) == 0
                        select new ProjectViewModel()
                        {
                            project_id = proj.project_id,
                            project_full_name = proj.project_full_name,
                        }).ToList();
            }
        }
       
        public static ProjectViewModel GetProjectItem(string projectId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                ProjectViewModel model = new ProjectViewModel();
                model = CommonFunctions.GetProjectDetailbyId(projectId);
                return model;
            }
        }

    }
    public class ProjectPMViewModel
    {
        [Key]
        public string project_pm_id { get; set; }
        public string project_id { get; set; }
        public string project_manager_id { get; set; }
        public string proejct_manager_name { get; set; }
    }
}