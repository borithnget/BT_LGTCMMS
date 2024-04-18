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
    public class WarehouseStockBalanceModel
    {
        public string itemId { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string labourHour { get; set; }
        public string bigBalance { get; set; }
        public string endingBalance { get; set; }
        public string inReceivedBalance { get; set; }
        public string inIssueReturnBalance { get; set; }
        public string outReturnBalance { get; set; }
        public string outTransferBalance { get; set; }
        public string outDamageBalance { get; set; }
        public string outIssueBalance { get; set; }
        public string totalIn { get; set; }
        public string totalOut { get; set; }
        public string inTransfer { get; set; }
        public string inn { get; set; }
        public string unitPrice { get; set; }
        public string amount { get; set; }
    }
}