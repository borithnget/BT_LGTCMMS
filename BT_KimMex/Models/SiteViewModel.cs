using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class SiteViewModel
    {
        [Key]
        [Display(Name ="Site Id")]
        public string site_id { get; set; }
        [Required(ErrorMessage ="Site name is required.")]
        [Display(Name ="Site name:")]
        public string site_name { get; set; }
        [Display(Name ="Address:")]
        public string site_address { get; set; }
        [Display(Name ="Site code")]
        public string site_code { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> create_dated { get; set; }
        public Nullable<System.DateTime> updated_dated { get; set; }
        public string site_supervisor_id { get; set; }
        //[Required(ErrorMessage ="Site Admin is required.")]
        public string site_admin_id { get; set; }
        public SiteSiteSupervisorViewModel siteSupervisor { get; set; }
        public SiteSiteAdminViewModel siteAdmin { get; set; }
        public List<SiteSiteSupervisorViewModel> siteSupervisors { get; set; }
        public SiteViewModel()
        {
            siteSupervisors = new List<SiteSiteSupervisorViewModel>();
        }
        
        public static List<SiteViewModel> GetSiteList(string userId="")
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return db.tb_site.OrderBy(s => s.site_name).Where(s => s.status == true).Select(s => new SiteViewModel()
                    {
                        site_id = s.site_id,
                        site_name = s.site_name,
                    }).ToList();
                }
                else
                {
                    return (from skw in db.tb_stock_keeper_warehouse
                            join w in db.tb_warehouse on skw.warehouse_id equals w.warehouse_id
                            join s in db.tb_site on w.warehouse_site_id equals s.site_id
                            where string.Compare(skw.stock_keeper, userId) == 0
                            && w.warehouse_status == true && s.status == true
                            select new SiteViewModel()
                            {
                                site_id=s.site_id,
                                site_name=s.site_name,
                            }).ToList();
                }
                
            }
        }

    }
    public class SiteSiteSupervisorViewModel
    {
        [Key]
        public string site_site_supervisor_id { get; set; }
        public string site_id { get; set; }
        public string site_supervisor_id { get; set; }
        public string site_supervisor_name { get; set; }
    }
    public class SiteSiteAdminViewModel
    {
        [Key]
        public string site_site_admin_id { get; set; }
        public string site_id { get; set; }
        public string site_admin_id { get; set; }
        public string site_admin_fullname { get; set; }
    }
}