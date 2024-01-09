using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class StockDamageViewModel
    {
        [Key]
        public string stock_damage_id { get; set; }
        [Display(Name ="Stock Damage No.:")]
        public string stock_damage_number { get; set; }
        public string sd_status { get; set; }
        public Nullable<bool> status { get; set; }
        [Display(Name ="Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
        public string strWarehouse { get; set; }
        public string strInvoiceNumber { get; set; }
        public string strInvoiceDate { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public List<CategoryViewModel> itemTypes { get; set; }
        public List<InventoryDetailViewModel> inventoryDetails { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public StockDamageViewModel()
        {
            inventories = new List<InventoryViewModel>();
            itemTypes = new List<CategoryViewModel>();
            inventoryDetails = new List<InventoryDetailViewModel>();
            rejects = new List<RejectViewModel>();
        }
    }
}