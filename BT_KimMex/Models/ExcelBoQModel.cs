using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ExcelBoQModel
    {
        public string itemCode { get; set; } //project short name
        public string categoryName { get; set; } //job category name
        public string subCategoryName { get; set; } //item type name
        public string subCategoryItem { get; set; } //item name
        public  string unit { get; set; }
        public  double unitPrice { get; set; }
        public string chartAccount { get; set; }
        public string remark { get; set; }
        public double qty { get; set; }
        public double amount { get; set; }
        public string jobCategoryRemark { get; set; }
        public double jobCategoryAmount { get; set; }
        
    }
}