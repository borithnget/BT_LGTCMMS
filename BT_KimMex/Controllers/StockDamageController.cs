using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;
using System.Text.RegularExpressions;

namespace BT_KimMex.Controllers
{
    //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser,Stock Keeper")]
    [Authorize]
    public class StockDamageController : Controller
    {
        // GET: StockDamage
        public ActionResult Index()
        {
            return View();
        }
        //[Authorize(Roles = "Admin,Stock Keeper ")]
        public ActionResult Create()
        {
            //ViewBag.SDNo = this.GetStockDamageNumber();
            ViewBag.SDNo = Class.CommonClass.GenerateProcessNumber("SD");
            return View();
        }
        [HttpPost]
        public ActionResult Create(StockDamageViewModel model)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (model.inventories.Count == 0)
                {
                    return Json(new { result = "error", message = "There is no item damage." }, JsonRequestBehavior.AllowGet);
                }
                
                tb_stock_damage stockDamage = new tb_stock_damage();
                stockDamage.stock_damage_id = Guid.NewGuid().ToString();
                stockDamage.stock_damage_number = CommonClass.GenerateProcessNumber("SD");
                stockDamage.status = true;
                stockDamage.sd_status = "Pending";
                stockDamage.created_by = User.Identity.Name;
                stockDamage.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_stock_damage.Add(stockDamage);
                db.SaveChanges();
                Class.CommonClass.AutoGenerateStockInvoiceNumber(stockDamage.stock_damage_id, model.inventories);
                foreach (var inv in model.inventories)
                {
                    if(!string.IsNullOrEmpty(inv.warehouse_id) &&!string.IsNullOrEmpty(inv.product_id)&&(inv.total_quantity>inv.out_quantity))
                    {
                        //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.product_id && m.warehouse_id == inv.warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                        //tb_inventory inventory = new tb_inventory();
                        //inventory.inventory_id = Guid.NewGuid().ToString();
                        //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inventory.inventory_status_id = "3";
                        //inventory.warehouse_id = inv.warehouse_id;
                        //inventory.product_id = inv.product_id;
                        //inventory.in_quantity = 0;
                        //inventory.out_quantity = inv.out_quantity;
                        //inventory.total_quantity = totalQty - inventory.out_quantity;
                        //inventory.ref_id = stockDamage.stock_damage_id;
                        //inventory.remark = inv.remark;
                        //db.tb_inventory.Add(inventory);
                        //db.SaveChanges();

                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = stockDamage.stock_damage_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.item_status = "pending";
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.invoice_number =CommonClass.GetInvoiceNumber(stockDamage.stock_damage_id,inventoryDetail.inventory_warehouse_id,inventoryDetail.invoice_date);
                        inventoryDetail.inventory_type = "3";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();

                    }
                }
                
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockDamageViewModel model = new StockDamageViewModel();
            model = this.GetStockDamageDetail(id);
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockDamageViewModel model = new StockDamageViewModel();
            ViewBag.WarehouseID = Inventory.GetWarehousesList();
            model = this.GetStockDamageDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(StockDamageViewModel model)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (model.inventories.Count == 0)
                {
                    return Json(new { result = "error", message = "There is no item damage." }, JsonRequestBehavior.AllowGet);
                }
                string id = model.stock_damage_id;
                tb_stock_damage stockDamage = db.tb_stock_damage.Find(id);
                stockDamage.stock_damage_number = model.stock_damage_number;
                stockDamage.status = true;
                stockDamage.sd_status = "Pending";
                stockDamage.updated_by = User.Identity.Name;
                stockDamage.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(model.stock_damage_id))
                {
                    string damageId = model.stock_damage_id;
                    this.DeleteStockDamageDetail(damageId);
                    CommonClass.DeleteInvoiceNumber(damageId);
                    CommonClass.AutoGenerateStockInvoiceNumber(damageId, model.inventories);
                }
                foreach (var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && (inv.total_quantity > inv.out_quantity))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = stockDamage.stock_damage_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.item_status = "pending";
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(stockDamage.stock_damage_id, inventoryDetail.inventory_warehouse_id, inventoryDetail.invoice_date);
                        inventoryDetail.inventory_type = "3";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();

                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    tb_stock_damage stockDamage = db.tb_stock_damage.Find(id);
                    stockDamage.status = false;
                    stockDamage.updated_by = User.Identity.Name;
                    stockDamage.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Approve(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_damage stockDamage = db.tb_stock_damage.Find(id);
                stockDamage.sd_status = "Completed";
                stockDamage.approved_by = User.Identity.Name;
                stockDamage.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                this.InsertItemInventory(id);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Reject(string id,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    tb_stock_damage stockDamage = db.tb_stock_damage.Find(id);
                    stockDamage.sd_status = "Rejected";
                    stockDamage.approved_by = User.Identity.Name;
                    stockDamage.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();

                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = stockDamage.stock_damage_id;
                    reject.ref_type = "Stock Damage";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.Name;
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockDamageViewModel model = new StockDamageViewModel();
            ViewBag.WarehouseID = Inventory.GetWarehousesList();
            model = this.GetStockDamageDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<InventoryDetailViewModel> models)
        {
            try
            {
                int countApproved = 0;
                kim_mexEntities db = new kim_mexEntities();
                foreach(InventoryDetailViewModel item in models)
                {
                    string iid = item.inventory_detail_id;
                    tb_inventory_detail inventoryDetail = db.tb_inventory_detail.Find(iid);
                    inventoryDetail.item_status = item.item_status;
                    inventoryDetail.remark = item.remark;
                    db.SaveChanges();

                    if (string.Compare(item.item_status, "approved") == 0)
                    {
                        countApproved++;
                        decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inventoryDetail.inventory_item_id && m.warehouse_id == inventoryDetail.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = id;
                        inventory.inventory_status_id = "3";
                        inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                        inventory.product_id = inventoryDetail.inventory_item_id;
                        inventory.out_quantity = Class.CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id,inventoryDetail.unit,Convert.ToDecimal(inventoryDetail.quantity));
                        inventory.in_quantity = 0;
                        inventory.total_quantity = totalQty - inventory.out_quantity;
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                    }
                }
                tb_stock_damage stockDamage = db.tb_stock_damage.Find(id);
                stockDamage.sd_status = countApproved == models.Count() ? "Completed" : "Pending Feedback";
                stockDamage.approved_by = User.Identity.Name;
                stockDamage.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrepareFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockDamageViewModel model = new StockDamageViewModel();
            ViewBag.WarehouseID = Inventory.GetWarehousesList();
            model = this.GetStockDamageDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id,List<InventoryViewModel> models)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_stock_damage stockDamage = db.tb_stock_damage.Find(id);
                stockDamage.sd_status = models.Count() == 0 ? "Completed" : "Feedbacked";
                stockDamage.updated_by = User.Identity.Name;
                stockDamage.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                var inventories = db.tb_inventory_detail.Where(x => x.inventory_ref_id == id && string.Compare(x.item_status, "feedbacked") == 0).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.inventory_detail_id;
                    tb_inventory_detail inv = db.tb_inventory_detail.Where(x => x.inventory_detail_id == inventoryID).FirstOrDefault();
                    db.tb_inventory_detail.Remove(inv);
                    db.SaveChanges();
                }
                foreach (var inv in models)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && (inv.total_quantity > inv.out_quantity))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = stockDamage.stock_damage_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.item_status = "pending";
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
                        inventoryDetail.invoice_number = inv.invoice_number;
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetInventoryItem(string itemId,string warehouseId)
        {
            InventoryViewModel inventory = new InventoryViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                inventory = (from inv in db.tb_inventory
                             join item in db.tb_product on inv.product_id equals item.product_id
                             join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                             orderby inv.inventory_date descending
                             where string.Compare(inv.warehouse_id, warehouseId) == 0 && string.Compare(inv.product_id, itemId) == 0
                             select new InventoryViewModel()
                             {
                                 inventory_id = inv.inventory_id,
                                 inventory_date = inv.inventory_date,
                                 product_id = inv.product_id,
                                 itemName = item.product_name,
                                 itemCode = item.product_code,
                                 itemUnit = item.product_unit,
                                 warehouse_id = inv.warehouse_id,
                                 warehouseName = wah.warehouse_name,
                                 total_quantity = inv.total_quantity,
                                 in_quantity = inv.in_quantity,
                                 out_quantity = inv.out_quantity
                             }).FirstOrDefault();
            }
            return Json(new { result = "success", data = inventory }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockDamageDataTable(string status)
        {
            List<StockDamageViewModel> stockDamages = new List<StockDamageViewModel>();
            stockDamages = this.GetStockDamageList(status);
            return Json(new { data = stockDamages }, JsonRequestBehavior.AllowGet);
        }
        public string GetStockDamageNumber()
        {
            string stockDamageNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.tb_stock_damage orderby tbl.created_date descending select tbl.stock_damage_number).FirstOrDefault();
                if (number == null)
                    last_no = "001";
                else
                {
                    number = number.Substring(number.Length - 3, 3);
                    int num = Convert.ToInt32(number) + 1;
                    if (num.ToString().Length == 1) last_no = "00" + num;
                    else if (num.ToString().Length == 2) last_no = "0" + num;
                    else if (num.ToString().Length == 3) last_no = num.ToString();
                }
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                stockDamageNo = "SD-" + yy + "-" + mm + "-" + last_no;
            }
            return stockDamageNo;
        }
        public List<StockDamageViewModel> GetStockDamageList(string status)
        {
            string strWarehouse = string.Empty;
            string strInvoiceNumber = string.Empty;
            string strInvoiceDate = string.Empty;
            List<StockDamageViewModel> stockDamages = new List<StockDamageViewModel>();
            List<StockDamageViewModel> objs = new List<StockDamageViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if(string.Compare(status,"All")==0)
                    objs = db.tb_stock_damage.OrderBy(m => m.stock_damage_number).Where(m => m.status == true).Select(m => new StockDamageViewModel { stock_damage_id = m.stock_damage_id, stock_damage_number = m.stock_damage_number, created_date = m.created_date, sd_status = m.sd_status }).ToList();
                else
                    objs = db.tb_stock_damage.OrderBy(m => m.stock_damage_number).Where(m => m.status == true && m.sd_status == status).Select(m => new StockDamageViewModel { stock_damage_id = m.stock_damage_id, stock_damage_number = m.stock_damage_number, created_date = m.created_date, sd_status = m.sd_status }).ToList();
                foreach(var obj in objs)
                {
                    strWarehouse = string.Empty;
                    strInvoiceNumber = string.Empty;
                    strInvoiceDate = string.Empty;
                    var damageItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.stock_damage_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                    var dupWarehouses = damageItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceNumbers = damageItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceDate = damageItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    foreach (var wh in dupWarehouses)
                        strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Find(wh).warehouse_name);
                    foreach (var ivn in dupInvoiceNumbers)
                        strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                    foreach (var ivd in dupInvoiceDate)
                        strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                    foreach(var item in damageItems)
                    {
                        bool isDupWarehouse = dupWarehouses.Where(x => string.Compare(x, item.warehouseId) == 0).Count() > 0 ? true : false;
                        bool isDupIVN = dupInvoiceNumbers.Where(x => string.Compare(x, item.invoiceNumber) == 0).Count() > 0 ? true : false;
                        bool isDupIVD = dupInvoiceDate.Where(x => x == item.invoiceDate).Count() > 0 ? true : false;
                        if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Find(item.warehouseId).warehouse_name);
                        if (!isDupIVN) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, item.invoiceNumber);
                        if (!isDupIVD) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(item.invoiceDate).ToString("dd/MM/yyy"));
                    }
                    StockDamageViewModel stockDamage = new StockDamageViewModel();
                    stockDamage.stock_damage_id = obj.stock_damage_id;
                    stockDamage.stock_damage_number = obj.stock_damage_number;
                    stockDamage.created_date = obj.created_date;
                    stockDamage.sd_status = obj.sd_status;
                    stockDamage.strWarehouse = strWarehouse;
                    stockDamage.strInvoiceNumber = strInvoiceNumber;
                    stockDamage.strInvoiceDate = strInvoiceDate;
                    stockDamages.Add(stockDamage);
                }
            }
            return stockDamages;
        }
        public StockDamageViewModel GetStockDamageDetail(string id)
        {
            StockDamageViewModel stockDamage = new StockDamageViewModel();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                stockDamage = db.tb_stock_damage.Where(m => string.Compare(m.stock_damage_id, id) == 0).Select(m => new StockDamageViewModel() { stock_damage_id = m.stock_damage_id, stock_damage_number = m.stock_damage_number, created_date = m.created_date, sd_status = m.sd_status }).FirstOrDefault();
                stockDamage.inventories = Inventory.GetInventoryItem(stockDamage.stock_damage_id);
                stockDamage.inventoryDetails = Inventory.GetInventoryDetail(stockDamage.stock_damage_id);
                //stockDamage.itemTypes = this.GetStockDamageItemType(stockDamage.inventoryDetails);
                stockDamage.rejects = CommonClass.GetRejectByRequest(id);
            }
            return stockDamage;
        } 
        public List<CategoryViewModel> GetStockDamageItemType(List<InventoryDetailViewModel> inventories)
        {
            kim_mexEntities db = new kim_mexEntities();
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            var dupCategory = inventories.GroupBy(x => x.itemTypeId).Where(x => x.Count() > 0).Select(x => x.Key).ToList();
            if (dupCategory.Count > 0)
            {
                foreach (var categoryId in dupCategory)
                {
                    var cate = db.tb_product_category.Where(x => x.p_category_id == categoryId).FirstOrDefault();
                    CategoryViewModel category = new CategoryViewModel();
                    category.p_category_id = cate.p_category_id;
                    category.p_category_code = cate.p_category_code;
                    category.p_category_name = cate.p_category_name;
                    categories.Add(category);
                }
            }
            else
            {

            }
            return categories;
        }
        public void DeleteStockDamageDetail(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var inventories = db.tb_inventory_detail.Where(x => x.inventory_ref_id == id).ToList();
                foreach(var inventory in inventories)
                {
                    var inventoryID = inventory.inventory_detail_id;
                    tb_inventory_detail inv = db.tb_inventory_detail.Where(x => x.inventory_detail_id == inventoryID).FirstOrDefault();
                    db.tb_inventory_detail.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public void InsertItemInventory(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory_detail.Where(m => m.inventory_ref_id == id).ToList();
                foreach (var inv in inventories)
                {
                    decimal quantity = 0;
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == inv.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    string inventoryUnit = Regex.Replace(db.tb_product.Where(m => m.product_id == inv.inventory_item_id).Select(m => m.product_unit).FirstOrDefault(), @"\t|\n|\r", "");
                    if (string.Compare(inventoryUnit, inv.unit) == 0)
                        quantity =Convert.ToDecimal(inv.quantity);
                    else
                    {
                        var uom = db.tb_multiple_uom.Where(m => m.product_id == inv.inventory_item_id).FirstOrDefault();
                        if (uom!=null)
                        {
                            if (uom.uom1_id != null && uom.uom1_qty != null)
                            {
                                string uom1 = Regex.Replace(uom.uom1_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom1) == 0)
                                {
                                    quantity =Convert.ToDecimal(inv.quantity / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom2_id != null && uom.uom2_qty != null)
                            {
                                string uom2 = Regex.Replace(uom.uom2_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom2) == 0)
                                {
                                    quantity = Convert.ToDecimal((inv.quantity / uom.uom2_qty)/uom.uom1_qty);
                                }
                            }
                            else if (uom.uom3_id != null && uom.uom3_qty != null)
                            {
                                string uom3 = Regex.Replace(uom.uom3_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom3) == 0)
                                {
                                    quantity = Convert.ToDecimal(((inv.quantity /uom.uom3_qty)/ uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom4_id != null && uom.uom4_qty != null)
                            {
                                string uom4 = Regex.Replace(uom.uom4_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom4) == 0)
                                {
                                    quantity = Convert.ToDecimal((((inv.quantity /uom.uom4_qty)/ uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom5_id != null && uom.uom5_qty != null)
                            {
                                string uom5 = Regex.Replace(uom.uom5_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom5) == 0)
                                {
                                    quantity = Convert.ToDecimal(((((inv.quantity/uom.uom5_qty) / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                        }
                    }
                    if (quantity <= totalQty)
                    {
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = id;
                        inventory.inventory_status_id = "3";
                        inventory.warehouse_id = inv.inventory_warehouse_id;
                        inventory.product_id = inv.inventory_item_id;
                        inventory.out_quantity = quantity;
                        inventory.in_quantity = 0;
                        inventory.total_quantity = totalQty - inventory.out_quantity;
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}