using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class StockViewModel
    {
        public string mrId { get; set; }
        public string mrNumber { get; set; }
        public string warehouse { get; set; }
        public string projectname { get; set; }

        public List<STItemViewModel> stocks { get; set; }
        public List<Entities.tb_warehouse> warehouses { get; set; }
        public StockViewModel()
        {
            stocks = new List<STItemViewModel>();
            warehouses = new List<Entities.tb_warehouse>();
        }
    }
}