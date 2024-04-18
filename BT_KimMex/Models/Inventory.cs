using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MoreLinq;

namespace BT_KimMex.Models
{
    public class InventoryViewModel
    {
        [Key]
        public string inventory_id { get; set; }
        public string inventory_detail_id { get; set; }
        public Nullable<decimal> history_quantity { get; set; }
        public Nullable<System.DateTime> inventory_date { get; set; }
        public string inventory_status_id { get; set; }
        public string warehouse_id { get; set; }
        public string product_id { get; set; }
        public string status { get; set; }
        public string unit { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
        public Nullable<decimal> in_quantity { get; set; }
        public Nullable<decimal> out_quantity { get; set; }
        public string ref_id { get; set; }
        public string warehouseName { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string remark { get; set; }
        public string itemTypeId { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public string uom1_id { get; set; }
        public Nullable<decimal> uom1_qty { get; set; }
        public string uom2_id { get; set; }
        public Nullable<decimal> uom2_qty { get; set; }
        public string uom3_id { get; set; }
        public Nullable<decimal> uom3_qty { get; set; }
        public string uom4_id { get; set; }
        public Nullable<decimal> uom4_qty { get; set; }
        public string uom5_id { get; set; }
        public Nullable<decimal> uom5_qty { get; set; }
        public ProductViewModel uom { get; set; }
        public string item_status { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public bool completed { get; set; }
        public string itemUnitName { get; set; }
        public string unitName { get; set; }
        public int totalReceived { get; set; }
        public string from_warehouse_id { get; set; }
        public string from_warehouse_name { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public List<InventoryViewModel> histories { get; set; }
        public InventoryViewModel history { get; set; }
        public string brand { get; set; }
        public string reason { get; set; }
        public tb_inventory inventory { get; set; }
        public tb_product product { get; set; }
        public tb_unit unitEntity { get; set; }
        public InventoryViewModel()
        {
            uom = new ProductViewModel();
            histories = new List<InventoryViewModel>();
            inventory = new tb_inventory();
            product = new tb_product();
            unitEntity = new tb_unit();
            
        }

        public static List<InventoryViewModel> GetInventoryItemsByRefId(string id)
        {
            List<InventoryViewModel> models = new List<InventoryViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                models = (from inv in db.tb_inventory
                          join pro in db.tb_product on inv.product_id equals pro.product_id
                          join un in db.tb_unit on pro.product_unit equals un.Id
                          where string.Compare(inv.ref_id,id) == 0
                          select new InventoryViewModel { 
                              inventory= inv,
                              product= pro,
                              unitEntity= un 
                          }).ToList();
            }
            catch(Exception ex)
            {

            }
            return models;
        }

        public static List<InventoryViewModel> GetStockBalancebyWarehouse(string id, DateTime dateFrom, DateTime dateTo)
        {
            
            List<Models.InventoryViewModel> models = new List<Models.InventoryViewModel>();
            List<Models.InventoryViewModel> inventories = new List<Models.InventoryViewModel>();
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                DateTime newDateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                DateTime newDateTo = dateTo.AddDays(1).AddMilliseconds(-1);
                List<Models.InventoryViewModel> orginalItems = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items1 = new List<Models.InventoryViewModel>();

                items1 = db.tb_inventory.OrderBy(x => x.product_id).ThenByDescending(x => x.inventory_date)
                    .Where(x => string.Compare(x.warehouse_id, id) == 0 && x.inventory_date >= newDateFrom && x.inventory_date <= newDateTo)
                    .Select(x => new Models.InventoryViewModel()
                    {
                        inventory_date = x.inventory_date,
                        product_id = x.product_id,
                        warehouse_id = x.warehouse_id,
                        total_quantity = x.total_quantity,
                        in_quantity = x.in_quantity,
                        out_quantity = x.out_quantity,
                        inventory_status_id = x.inventory_status_id,
                    }).ToList();

                #region new enhance speed
                var groupItems = items1.GroupBy(s => s.product_id).Select(s => new { key = s.Key, item = s.OrderByDescending(x => x.inventory_date).FirstOrDefault() }).ToList();
                foreach (var item in groupItems)
                {
                    var product = Class.CommonClass.GetProductDetail(item.item.product_id);
                    Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                    iitem.inventory_date = item.item.inventory_date;
                    iitem.product_id = item.item.product_id;
                    iitem.itemCode = product.product_code;
                    iitem.itemName = product.product_name;
                    iitem.itemUnit = product.product_unit;
                    iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                    iitem.warehouse_id = item.item.warehouse_id;
                    iitem.total_quantity = item.item.total_quantity;
                    //if (iitem.total_quantity > 0)
                    models.Add(iitem);

                    items1.RemoveAll(s => string.Compare(s.product_id, item.item.product_id) == 0);
                }
                foreach (var item in items1)
                {
                    var product = Class.CommonClass.GetProductDetail(item.product_id);
                    Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                    iitem.inventory_date = item.inventory_date;
                    iitem.product_id = item.product_id;
                    iitem.itemCode = product.product_code;
                    iitem.itemName = product.product_name;
                    iitem.itemUnit = product.product_unit;
                    iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                    iitem.warehouse_id = item.warehouse_id;
                    iitem.total_quantity = item.total_quantity;
                    //if (iitem.total_quantity > 0)
                    models.Add(iitem);
                }
                #endregion



                inventories = models.OrderBy(x => x.itemCode).ToList().DistinctBy(s => s.product_id).ToList();
            }
            return inventories;
        }

        public static List<WarehouseStockBalanceModel> GetStockBalanceWarehouseV2(string id, DateTime dateFrom, DateTime dateTo)
        {
            List<WarehouseStockBalanceModel> models = new List<WarehouseStockBalanceModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                DateTime newDateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                DateTime newDateTo = dateTo.AddDays(1).AddMilliseconds(-1);

                IQueryable<Models.InventoryViewModel> objs;
                List<Models.StockBalanceMonthyViewModel> originalItems = new List<Models.StockBalanceMonthyViewModel>();
                List<Models.StockBalanceMonthyViewModel> stockBalances = new List<Models.StockBalanceMonthyViewModel>();

                objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, id) == 0 && x.inventory_date >= newDateFrom && x.inventory_date <= newDateTo && string.Compare(x.inventory_status_id, "1") != 0).Select(x => new Models.InventoryViewModel()
                {
                    inventory_id = x.inventory_id,
                    inventory_date = x.inventory_date,
                    inventory_status_id = x.inventory_status_id,
                    warehouse_id = x.warehouse_id,
                    product_id = x.product_id,
                    total_quantity = x.total_quantity,
                    in_quantity = x.in_quantity,
                    out_quantity = x.out_quantity,
                });

                if (objs.Any())
                {
                    double BigBalance = 0;
                    double EndingBalance = 0;
                    double ReceiveBalance = 0;
                    double IssueReturnBalance = 0;
                    double ReturnBalance = 0;
                    double TransferBalance = 0;
                    double DamangeBalance = 0;
                    double IssueBalance = 0;
                    double TransferIn = 0;
                    double IN = 0;
                    double UPrice = 0;
                    double TotalIN = 0;
                    double TotalOut = 0;

                    var duplicateItems = objs.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateItems.Any())
                    {
                        foreach (var dupitem in duplicateItems)
                        {
                            BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                            TotalIN = 0; TotalOut = 0;
                            var items = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                            BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                            EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);
                            foreach (var item in items)
                            {
                                if (string.Compare(item.inventory_status_id, "1") != 0) //except stock adjustment
                                {
                                    TotalIN = TotalIN + Convert.ToDouble(item.in_quantity);
                                    TotalOut = TotalOut + Convert.ToDouble(item.out_quantity);
                                }
                                //stock in 
                                if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                {
                                    ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                {
                                    IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                }
                                //stock out
                                else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                {
                                    ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                {
                                    TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                {
                                    DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                {
                                    IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                }


                                TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                    x.invoice_date >= newDateFrom &&
                                                                    x.invoice_date <= newDateTo &&
                                                                    x.st_warehouse_id == id &&
                                                                    x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                IN = (double)((from PO in db.tb_purchase_order_detail
                                               join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                               join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                               join P in db.tb_project on PR.ir_project_id equals P.project_id
                                               join S in db.tb_site on P.site_id equals S.site_id
                                               join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                               where PO.item_status == "approved" &&
                                               PO.item_id == item.product_id &&
                                                     (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                     POD.created_date >= newDateFrom && POD.created_date <= newDateTo &&
                                                     W.warehouse_id == id
                                               select PO.quantity).Sum() ?? 0
                                                  );
                                UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                            }
                            originalItems.Add(new Models.StockBalanceMonthyViewModel()
                            {
                                ItemId = dupitem,
                                BigBalance = BigBalance,
                                EndingBalance = EndingBalance,
                                ReceivedBalance = ReceiveBalance,
                                IssueReturnBalance = IssueReturnBalance,
                                ReturnBalance = ReturnBalance,
                                TransferBalance = TransferBalance,
                                DamangeBalance = DamangeBalance,
                                IssueBalance = IssueBalance,
                                TransferIn = TransferIn,
                                IN = IN,
                                UPrice = UPrice,
                                TotalIn = TotalIN,
                                TotalOut = TotalOut,
                            });
                        }

                        foreach (var item in objs)
                        {
                            var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                            if (isDuplicate == null)
                            {
                                BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                TotalIN = 0; TotalOut = 0;
                                BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                EndingBalance = Convert.ToDouble(item.total_quantity);
                                if (string.Compare(item.inventory_status_id, "1") != 0) //except stock adjustment
                                {
                                    TotalIN = TotalIN + Convert.ToDouble(item.in_quantity);
                                    TotalOut = TotalOut + Convert.ToDouble(item.out_quantity);
                                }
                                //stock in 
                                if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                {
                                    ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                {
                                    IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                }
                                //stock out
                                else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                {
                                    ReturnBalance = Convert.ToDouble(item.out_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                {
                                    TransferBalance = Convert.ToDouble(item.out_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                {
                                    DamangeBalance = Convert.ToDouble(item.out_quantity);
                                }
                                else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                {
                                    IssueBalance = Convert.ToDouble(item.out_quantity);
                                }

                                TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                    x.invoice_date >= newDateFrom &&
                                                                    x.invoice_date <= newDateTo &&
                                                                    x.st_warehouse_id == id &&
                                                                    x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                IN = (double)((from PO in db.tb_purchase_order_detail
                                               join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                               join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                               join P in db.tb_project on PR.ir_project_id equals P.project_id
                                               join S in db.tb_site on P.site_id equals S.site_id
                                               join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                               where PO.item_status == "approved" &&
                                               PO.item_id == item.product_id &&
                                                     (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                     POD.created_date >= newDateFrom && POD.created_date <= newDateTo &&
                                                     W.warehouse_id == id
                                               select PO.quantity).Sum() ?? 0
                                                  );
                                UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                {
                                    ItemId = item.product_id,
                                    BigBalance = BigBalance,
                                    EndingBalance = EndingBalance,
                                    ReceivedBalance = ReceiveBalance,
                                    IssueReturnBalance = IssueReturnBalance,
                                    ReturnBalance = ReturnBalance,
                                    TransferBalance = TransferBalance,
                                    DamangeBalance = DamangeBalance,
                                    IssueBalance = IssueBalance,
                                    TransferIn = TransferIn,
                                    IN = IN,
                                    UPrice = UPrice,
                                    TotalIn = TotalIN,
                                    TotalOut = TotalOut
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in objs)
                        {
                            BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                            TotalIN = 0;
                            TotalOut = 0;

                            BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                            EndingBalance = Convert.ToDouble(item.total_quantity);
                            if (string.Compare(item.inventory_status_id, "1") != 0) //except stock adjustment
                            {
                                TotalIN = TotalIN + Convert.ToDouble(item.in_quantity);
                                TotalOut = TotalOut + Convert.ToDouble(item.out_quantity);
                            }
                            //stock in 
                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                            {
                                ReceiveBalance = Convert.ToDouble(item.in_quantity);
                            }
                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                            {
                                IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                            }
                            //stock out
                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                            {
                                ReturnBalance = Convert.ToDouble(item.out_quantity);
                            }
                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                            {
                                TransferBalance = Convert.ToDouble(item.out_quantity);
                            }
                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                            {
                                DamangeBalance = Convert.ToDouble(item.out_quantity);
                            }
                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                            {
                                IssueBalance = Convert.ToDouble(item.out_quantity);
                            }

                            var TransferInObj = db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                    x.invoice_date >= newDateFrom &&
                                                                    x.invoice_date <= newDateTo &&
                                                                    x.st_warehouse_id == id &&
                                                                    x.item_status == "approved").FirstOrDefault();
                            if (TransferInObj != null)
                                TransferIn = (double)TransferInObj.quantity;

                            IN = (double)((from PO in db.tb_purchase_order_detail
                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                           join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                           join P in db.tb_project on PR.ir_project_id equals P.project_id
                                           join S in db.tb_site on P.site_id equals S.site_id
                                           join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                           where PO.item_status == "approved" &&
                                           PO.item_id == item.product_id &&
                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                 POD.created_date >= newDateFrom && POD.created_date <= newDateTo &&
                                                 W.warehouse_id == id
                                           select PO.quantity).FirstOrDefault() ?? 0
                                                  );
                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                            originalItems.Add(new Models.StockBalanceMonthyViewModel()
                            {
                                ItemId = item.product_id,
                                BigBalance = BigBalance,
                                EndingBalance = EndingBalance,
                                ReceivedBalance = ReceiveBalance,
                                IssueReturnBalance = IssueReturnBalance,
                                ReturnBalance = ReturnBalance,
                                TransferBalance = TransferBalance,
                                DamangeBalance = DamangeBalance,
                                IssueBalance = IssueBalance,
                                TransferIn = TransferIn,
                                IN = IN,
                                UPrice = UPrice,
                                TotalIn = TotalIN,
                                TotalOut = TotalOut
                            });
                        }
                    }


                    List<Models.StockBalanceMonthyViewModel> secondHandItems = new List<Models.StockBalanceMonthyViewModel>();
                    var duplicateItemInventory = originalItems.GroupBy(x => x.ItemId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateItemInventory.Any())
                    {
                        foreach (var item in duplicateItemInventory)
                        {
                            BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                            TotalIN = 0; TotalOut = 0;
                            var dupItems = originalItems.Where(x => string.Compare(x.ItemId, item) == 0).ToList();
                            foreach (var ii in dupItems)
                            {
                                BigBalance = BigBalance + ii.BigBalance;
                                EndingBalance = EndingBalance + ii.EndingBalance;
                                ReceiveBalance = ReceiveBalance + ii.ReceivedBalance;
                                IssueReturnBalance = IssueReturnBalance + ii.IssueReturnBalance;
                                ReturnBalance = ReturnBalance + ii.ReturnBalance;
                                TransferBalance = TransferBalance + ii.TransferBalance;
                                DamangeBalance = DamangeBalance + ii.DamangeBalance;
                                IssueBalance = IssueBalance + ii.IssueBalance;
                                TransferIn = TransferIn + ii.TransferIn;
                                IN = IN + ii.IN;
                                TotalIN = TotalIN + ii.TotalIn;
                                TotalOut = TotalOut = ii.TotalOut;
                            }
                            secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                            {
                                ItemId = item,
                                BigBalance = BigBalance,
                                EndingBalance = EndingBalance,
                                ReceivedBalance = ReceiveBalance,
                                IssueReturnBalance = IssueReturnBalance,
                                ReturnBalance = ReturnBalance,
                                TransferBalance = TransferBalance,
                                DamangeBalance = DamangeBalance,
                                IssueBalance = IssueBalance,
                                TransferIn = TransferIn,
                                IN = IN,
                                UPrice = UPrice,
                                TotalIn = TotalIN,
                                TotalOut = TotalOut
                            });
                        }

                        foreach (var item in originalItems)
                        {
                            var isDuplicate = duplicateItemInventory.Where(x => string.Compare(x, item.ItemId) == 0).FirstOrDefault();
                            if (isDuplicate == null)
                            {
                                secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                {
                                    ItemId = item.ItemId,
                                    BigBalance = item.BigBalance,
                                    EndingBalance = item.EndingBalance,
                                    ReceivedBalance = item.ReceivedBalance,
                                    IssueReturnBalance = item.IssueReturnBalance,
                                    ReturnBalance = item.ReturnBalance,
                                    TransferBalance = item.TransferBalance,
                                    DamangeBalance = item.DamangeBalance,
                                    IssueBalance = item.IssueBalance,
                                    TransferIn = TransferIn,
                                    IN = IN,
                                    UPrice = item.UPrice,
                                    TotalIn = item.TotalIn,
                                    TotalOut = item.TotalOut
                                });
                            }
                        }
                    }
                    else
                        secondHandItems = originalItems;
                    foreach (var item in secondHandItems)
                    {
                        var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                        stockBalances.Add(new Models.StockBalanceMonthyViewModel()
                        {
                            ItemId = item.ItemId,
                            ItemCode = product.product_code,
                            ItemName = product.product_name,
                            ItemNumber = product.product_number,
                            ItemUnit = product.product_unit,
                            BigBalance = item.BigBalance,
                            EndingBalance = item.EndingBalance,
                            ReceivedBalance = item.ReceivedBalance,
                            IssueReturnBalance = item.IssueReturnBalance,
                            ReturnBalance = item.ReturnBalance,
                            TransferBalance = item.TransferBalance,
                            DamangeBalance = item.DamangeBalance,
                            IssueBalance = item.IssueBalance,
                            TransferIn = TransferIn,
                            IN = IN,
                            //UPrice = UPrice
                            UPrice = item.UPrice,
                            TotalOut = item.TotalOut,
                            TotalIn = item.TotalIn
                        }); ;
                    }
                    stockBalances = stockBalances.OrderBy(x => x.ItemNumber).ToList();

                    

                    foreach (var item in stockBalances)
                    {
                        WarehouseStockBalanceModel result = new WarehouseStockBalanceModel();
                        var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();


                        result.itemId = product.product_id;
                        result.itemCode = product.product_code;
                        result.itemName = product.product_name;
                        result.itemUnit = product.product_unit;
                        result.labourHour =Convert.ToString(product.labour_hour);
                        result.bigBalance=Convert.ToString(item.BigBalance);
                        //update formula
                        result.endingBalance = Convert.ToString(item.EndingBalance);
                        result.endingBalance = Convert.ToString((item.BigBalance + item.TotalIn) - item.TotalOut);
                        result.inReceivedBalance = Convert.ToString(item.ReceivedBalance);
                        result.inIssueReturnBalance =Convert.ToString(item.IssueReturnBalance);
                        result.outReturnBalance = Convert.ToString(item.ReturnBalance);
                        result.outTransferBalance = Convert.ToString(item.TransferBalance);
                        result.outDamageBalance = Convert.ToString(item.DamangeBalance);
                        result.outIssueBalance =Convert.ToString(item.IssueBalance);
                        //dtr["total_in"] = item.ReceivedBalance + item.IssueReturnBalance;
                        //dtr["total_out"] = item.ReturnBalance + item.TransferBalance + item.DamangeBalance + item.IssueBalance;
                        result.totalIn =Convert.ToString(item.TotalIn);
                        result.totalOut =Convert.ToString(item.TotalOut);

                        result.inTransfer =Convert.ToString(item.TransferIn);
                        result.inn = Convert.ToString(item.IN);
                        result.unitPrice =Convert.ToString(item.UPrice);
                        result.amount =Convert.ToString(item.EndingBalance * item.UPrice);

                        models.Add(result);
                    }
                
                }

            }
            catch(Exception ex)
            {

            }

            return models;
        }
    }
    public class InventoryDetailViewModel
    {
        [Key]
        public string inventory_detail_id { get; set; }
        public string inventory_ref_id { get; set; }
        public string inventory_item_id { get; set; }
        public string inventory_warehouse_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> stock_balance { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string warehouseName { get; set; }
        public string remark { get; set; }
        public string itemTypeId { get; set; }
        public string itemTypeName { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public decimal unit_price { get; set; }
        public string unit { get; set; }
        public string unitName { get; set; }
        public string itemunitname { get; set; }
        public ProductViewModel uom { get; set; }
        public string item_status { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
        public Nullable<decimal> item_labour_hour { get; set; }
        public Nullable<DateTime> inventory_date { get; set; }
        public InventoryDetailViewModel()
        {
            uom = new ProductViewModel();
            
        }

        public static List<InventoryDetailViewModel> GetInventoryDetailByRefId(string refId)
        {
            List<InventoryDetailViewModel> models = new List<InventoryDetailViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();

            }catch(Exception ex)
            {

            }
            return models;
        }
    }
}