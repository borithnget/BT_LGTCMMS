using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ItemReturnViewModel
    {
        [Key]
        public string itemReturnId { get; set; }
        [Display(Name ="Return Item No.:")]
        public string itemReturnNumber { get; set; }
        public string itemReturnStatus { get; set; }
        [Display(Name = "Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
        public string strWarehouse { get; set; }
        public string strInvoiceDate { get; set; }
        public string strInvoiceNumber { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public List<InventoryDetailViewModel> inventoryDetails { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public ItemReturnViewModel()
        {
            inventories = new List<InventoryViewModel>();
            inventoryDetails = new List<InventoryDetailViewModel>();
            rejects = new List<RejectViewModel>();
        }

    }
}