using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
namespace BT_KimMex.Models
{
    public class WareHouseViewModel
    {
        [Key]
        public string warehouse_id { get; set; }
        [Required(ErrorMessage ="Warehouse Name is required.")]
        [Display(Name ="Warehouse Name:")]
        public string warehouse_name { get; set; }
        [Display(Name ="Telephone:")]
        public string warehouse_telephone { get; set; }
        [Display(Name ="Address:")]
        public string warehouse_address { get; set; }
        [Display(Name="Site Name:")]
        [Required(ErrorMessage ="Site Name is required.")]
        public string warehouse_site_id { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<bool> warehouse_status { get; set; }
        public string date { get; set; }
        [Display(Name ="Site Name:")]
        public string site_name { get; set; }
        [Display(Name = "Stock Keeper:")]
        [Required(ErrorMessage = "Stock Keeper is required.")]
        public string[] Stock_keeper_id { get; set; }
        [Display(Name = "Stock Keeper:")]
        public IEnumerable<String> list_stock_keeper { set; get; }
        public WarehouseQAQCViewModel warehouseQAQC { get; set; }
        public List<WarehouseQAQCViewModel> warehouseQAQCs { get; set; }
        
        public string qaqc_id { get; set; }
        public string warehouse_project_id { get; set; }
        public string warehouse_project_name { get; set; }
        public string warehouse_project_status { get; set; }
        public List<ProjectViewModel> siteProjects { get; set; }
        [Required(ErrorMessage = "QA/ QC officer is required.")]
        public string[] qaqcs { get; set; }
        //[Required(ErrorMessage = "Site Stock Keeper is required.")]
        //public string[] siteStockKeepers { get; set; }
        public static string GetStock_keeper_name(String id)
        {
            BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return db.tb_user_detail.Where(m => m.user_id == id).Select(m => m.user_first_name+" "+m.user_last_name).FirstOrDefault();
        }

        public static IEnumerable<SelectListItem> GetStockkeeper_name()
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
                             }).ToList().Select(p => new UserRolesViewModel()
                             {
                                 UserId = p.UserId,
                                 Username = p.Username,
                                 Email = p.Email,
                                 Role = string.Join(",", p.RoleName)
                             }).Where(p => p.Role == "Stock Keeper").OrderBy(p => p.Username);

        
            var list = new List<SelectListItem>();
            foreach (var i in userRoles)
            {
                list.Add(new SelectListItem
                {
                    Value = i.UserId.ToString(),
                    Text = i.Username
                });
            }
            return list;
        }
        public static IEnumerable<SelectListItem> GetSelectedStock_Keeper(string id)
        {

            var data = GetStockkeeper_name();
            var list = new List<SelectListItem>();
            foreach (var i in data)
            {
                if (i.Value == id)
                {
                    list.Add(new SelectListItem
                    {
                        Value = i.Value,
                        Text = i.Text,
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem
                    {
                        Value = i.Value,
                        Text = i.Text
                    });
                }
            }
            return list;
        }
        public WareHouseViewModel()
        {
            warehouseQAQCs = new List<WarehouseQAQCViewModel>();
        }
        public static WareHouseViewModel ConvertEntityToModel(tb_warehouse enity)
        {
            return new WareHouseViewModel()
            {
                warehouse_id=enity.warehouse_id,
                warehouse_name=enity.warehouse_name,
                warehouse_site_id=enity.warehouse_site_id,
                warehouse_project_id=enity.warehouse_project_id
            };
        }
    }

    public class WarehouseQAQCViewModel
    {
        [Key]
        public string warehouse_qaqc_id { get; set; }
        public string warehouse_id { get; set; }
        public string qaqc_id { get; set; }
        public string qaqc_name { get; set; }
    }

}