using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class ItemReturnController : Controller
    {
        // GET: ItemReturn
        public ActionResult Index()
        {
            return View(GetItemReturnToSupplierListItems());
        }

        // GET: ItemReturn/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            ItemReturnViewModel model = new ItemReturnViewModel();
            model = this.GetReturnItemDetail(id);
            return View(model);
        }

        // GET: ItemReturn/Create
        public ActionResult Create()
        {
            //ViewBag.Number = this.GetReturnItemNumber();
            ViewBag.Number = Class.CommonClass.GenerateProcessNumber("IRT");
            return View();
        }

        // POST: ItemReturn/Create
        [HttpPost]
        public ActionResult Create(ItemReturnViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                if (model.inventories.Count() == 0)
                    return Json(new { result = "error", message = "No return item." }, JsonRequestBehavior.AllowGet);
                var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id,x.supplier_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplicate Item,Supplier and Warehouse." }, JsonRequestBehavior.AllowGet);
                }
                tb_item_return itemReturn = new tb_item_return();
                itemReturn.item_return_id = Guid.NewGuid().ToString();
                itemReturn.item_return_number = CommonClass.GenerateProcessNumber("IRT");
                itemReturn.item_return_status = Class.Status.Pending;
                itemReturn.status = true;
                itemReturn.created_by = User.Identity.GetUserId();
                itemReturn.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                itemReturn.updated_by = User.Identity.GetUserId();
                itemReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_item_return.Add(itemReturn);
                db.SaveChanges();
                CommonClass.AutoGenerateStockInvoiceNumber(itemReturn.item_return_id, model.inventories);
                foreach (var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >=CommonClass.ConvertMultipleUnit(inv.product_id,inv.unit,Convert.ToDecimal(inv.out_quantity)) &&!string.IsNullOrEmpty(inv.supplier_id))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = itemReturn.item_return_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.supplier_id = inv.supplier_id;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.invoice_number =CommonClass.GetInvoiceNumber(itemReturn.item_return_id,inventoryDetail.inventory_warehouse_id,inventoryDetail.invoice_date);
                        inventoryDetail.item_status = Class.Status.Pending;
                        inventoryDetail.inventory_type = "8";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = "error" },JsonRequestBehavior.AllowGet);
            }
        }

        // GET: ItemReturn/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            ItemReturnViewModel model = new ItemReturnViewModel();
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            List<SupplierViewModel> suppliers = new List<SupplierViewModel>();
            model = this.GetReturnItemDetail(id);
            warehouses = CommonClass.Warehouses();
            suppliers = CommonClass.Suppliers();
            ViewBag.WarehouseID = warehouses;
            ViewBag.SupplierID = suppliers;
            return View(model);
        }

        // POST: ItemReturn/Edit/5
        [HttpPost]
        public ActionResult Edit(ItemReturnViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                if (model.inventories.Count() == 0)
                    return Json(new { result = "error", message = "No return item." }, JsonRequestBehavior.AllowGet);
                var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id, x.supplier_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplicate Item,Supplier and Warehouse." }, JsonRequestBehavior.AllowGet);
                }
                string id = model.itemReturnId;
                tb_item_return itemReturn = db.tb_item_return.Find(id);
                itemReturn.item_return_status = "Pending";
                itemReturn.updated_by = User.Identity.Name;
                itemReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(model.itemReturnId))
                {
                    string returnId = model.itemReturnId;
                    this.DeleteReturnItemDetail(returnId);
                    CommonClass.DeleteInvoiceNumber(returnId);
                    CommonClass.AutoGenerateStockInvoiceNumber(returnId, model.inventories);
                }
                foreach (var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >=CommonClass.ConvertMultipleUnit(inv.product_id,inv.unit,Convert.ToDecimal(inv.out_quantity)) && !string.IsNullOrEmpty(inv.supplier_id))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = itemReturn.item_return_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.supplier_id = inv.supplier_id;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(itemReturn.item_return_id,inventoryDetail.inventory_warehouse_id,inventoryDetail.invoice_date);
                        inventoryDetail.item_status = "pending";
                        inventoryDetail.inventory_item_id = "8";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = "error" },JsonRequestBehavior.AllowGet);
            }
        }

        // GET: ItemReturn/Delete/5
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_return itemReturn = db.tb_item_return.Find(id);
                itemReturn.status = false;
                itemReturn.updated_by = User.Identity.Name;
                itemReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approve(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_return itemReturn = db.tb_item_return.Find(id);
                itemReturn.item_return_status = Status.Completed;
                itemReturn.checked_by = User.Identity.GetUserId();
                itemReturn.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                this.InsertItemInventory(id);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Reject(string id,string comment)
        {
            using (kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_return returnItem = db.tb_item_return.Find(id);
                returnItem.item_return_status = Status.Rejected;
                returnItem.checked_by = User.Identity.GetUserId();
                returnItem.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = returnItem.item_return_id;
                reject.ref_type = "Return Item";
                reject.comment = comment;
                reject.rejected_by = User.Identity.GetUserId();
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_reject.Add(reject);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            ItemReturnViewModel model = new ItemReturnViewModel();
            model = this.GetReturnItemDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<Models.InventoryDetailViewModel> models)
        {
            try
            {
                int countApproved = 0;
                kim_mexEntities db = new kim_mexEntities();
                foreach(InventoryDetailViewModel item in models)
                {
                    string idd = item.inventory_detail_id;
                    tb_inventory_detail inventoryDetail = db.tb_inventory_detail.Find(idd);
                    inventoryDetail.item_status = item.item_status;
                    inventoryDetail.remark = item.remark;
                    db.SaveChanges();

                    if (string.Compare(item.item_status, Status.Approved) == 0)
                    {
                        countApproved++;
                        //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inventoryDetail.inventory_item_id && m.warehouse_id == inventoryDetail.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                        //tb_inventory inventory = new tb_inventory();
                        //inventory.inventory_id = Guid.NewGuid().ToString();
                        //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inventory.ref_id = id;
                        //inventory.inventory_status_id = "8";
                        //inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                        //inventory.product_id = inventoryDetail.inventory_item_id;
                        //inventory.out_quantity = CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit, Convert.ToDecimal(inventoryDetail.quantity));
                        //inventory.in_quantity = 0;
                        //inventory.total_quantity = totalQty - inventory.out_quantity;
                        //db.tb_inventory.Add(inventory);
                        //db.SaveChanges();
                    }

                }
                tb_item_return itemReturn = db.tb_item_return.Find(id);
                itemReturn.item_return_status = countApproved == models.Count ? Status.Approved : Status.PendingFeedback;
                itemReturn.approved_by = User.Identity.Name;
                itemReturn.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
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
            ItemReturnViewModel model = new ItemReturnViewModel();
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            List<SupplierViewModel> suppliers = new List<SupplierViewModel>();
            model = this.GetReturnItemDetail(id);
            warehouses = CommonClass.Warehouses();
            suppliers = CommonClass.Suppliers();
            //ViewBag.WarehouseID = warehouses;
            ViewBag.WarehouseID = this.GetWarehouseList(0);
            ViewBag.SupplierID = suppliers;
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id,List<InventoryViewModel> models)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                //delete old item return detail
                
                //if no item to summit from feedback
                tb_item_return itemReturn = db.tb_item_return.Find(id);
                itemReturn.item_return_status = models.Count() == 0 ? Status.Approved : Status.Feedbacked;
                itemReturn.updated_by = User.Identity.Name;
                itemReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                var oldItemReturn = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0 && string.Compare(x.item_status, Status.Feedbacked) == 0).Select(x => x.inventory_detail_id).ToList();
                foreach (var oId in oldItemReturn)
                {
                    tb_inventory_detail detail = db.tb_inventory_detail.Find(oId);
                    db.tb_inventory_detail.Remove(detail);
                    db.SaveChanges();
                }
                foreach (var inv in models)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >= CommonClass.ConvertMultipleUnit(inv.product_id, inv.unit, Convert.ToDecimal(inv.out_quantity)) && !string.IsNullOrEmpty(inv.supplier_id))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = itemReturn.item_return_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.supplier_id = inv.supplier_id;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.invoice_number = inv.invoice_number;
                        inventoryDetail.item_status = Status.Pending;
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
        public ActionResult GetItemReturnDataTable(string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ItemReturnViewModel> itemReturns = new List<ItemReturnViewModel>();
                List<ItemReturnViewModel> objs = new List<ItemReturnViewModel>();
                string strWarehouse = string.Empty;
                string strInvoiceNumber = string.Empty;
                string strInvoiceDate = string.Empty;
                if (string.Compare(status, "All")==0)
                {
                    objs = db.tb_item_return.OrderBy(x => x.item_return_number).Where(x => x.status == true).Select(x => new ItemReturnViewModel() { itemReturnId = x.item_return_id, itemReturnNumber = x.item_return_number, itemReturnStatus = x.item_return_status, created_date = x.created_date }).ToList();
                }
                else
                {
                    objs = db.tb_item_return.OrderBy(x => x.item_return_number).Where(x => x.status == true && x.item_return_status==status).Select(x => new ItemReturnViewModel() { itemReturnId = x.item_return_id, itemReturnNumber = x.item_return_number, itemReturnStatus = x.item_return_status, created_date = x.created_date }).ToList();
                }
                foreach(ItemReturnViewModel obj in objs)
                {
                    strWarehouse = string.Empty;
                    strInvoiceNumber = string.Empty;
                    strInvoiceDate = string.Empty;
                    ItemReturnViewModel itemReturn = new ItemReturnViewModel();
                    itemReturn.itemReturnId = obj.itemReturnId;
                    itemReturn.itemReturnNumber = obj.itemReturnNumber;
                    itemReturn.itemReturnStatus = obj.itemReturnStatus;
                    itemReturn.created_date = obj.created_date;
                    var oobjs = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.itemReturnId) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceDate = x.invoice_date, invoiceNumber = x.invoice_number }).ToList();
                    var dupWarehouse = oobjs.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceNumber = oobjs.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceDate = oobjs.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupWarehouse.Any())
                        foreach(var warehouseId in dupWarehouse)
                            strWarehouse=string.Format("{0} {1},",strWarehouse,db.tb_warehouse.Where(x=>string.Compare(x.warehouse_id,warehouseId)==0).Select(x=>x.warehouse_name).FirstOrDefault().ToString());
                    if (dupInvoiceNumber.Any())
                        foreach (var invoiceNumber in dupInvoiceNumber)
                            strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, invoiceNumber);
                    if (dupInvoiceDate.Any())
                        foreach (var invoiceDate in dupInvoiceDate)
                            strInvoiceDate = string.Format("{0} {1},", strInvoiceDate,Convert.ToDateTime(invoiceDate).ToString("dd/MM/yyy"));
                    foreach(var oobj in oobjs)
                    {
                        bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, oobj.warehouseId) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, oobj.invoiceNumber) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == oobj.invoiceDate).Count() > 0 ? true : false;
                        if(!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, oobj.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString());
                        if(!isDupInvoiceNumber) strInvoiceNumber= string.Format("{0} {1},", strInvoiceNumber, oobj.invoiceNumber);
                        if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate,Convert.ToDateTime(oobj.invoiceDate).ToString("dd/MM/yyyy"));
                    }
                    itemReturn.strWarehouse = strWarehouse;
                    itemReturn.strInvoiceDate = strInvoiceDate;
                    itemReturn.strInvoiceNumber = strInvoiceNumber;
                    itemReturns.Add(itemReturn);
                }
                return Json(new { data = itemReturns }, JsonRequestBehavior.AllowGet);
            }
        }
        public string GetReturnItemNumber()
        {
            string returnItemNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "";
                string number = (from tbl in db.tb_item_return orderby tbl.created_date descending select tbl.item_return_number).FirstOrDefault();
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
                returnItemNo = "IRT-" + yy + "-" + mm + "-" + last_no;
            }
            return returnItemNo;
        }
        public List<ItemReturnViewModel> GetReturnItemList()
        {
            List<ItemReturnViewModel> itemReturns = new List<ItemReturnViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                itemReturns = db.tb_item_return.OrderBy(x => x.item_return_number).Where(x => x.status == true).Select(x => new ItemReturnViewModel() { itemReturnId = x.item_return_id, itemReturnNumber = x.item_return_number, itemReturnStatus = x.item_return_status, created_date = x.created_date }).ToList();
            }
            return itemReturns;
        }
        public ItemReturnViewModel GetReturnItemDetail(string id)
        {
            ItemReturnViewModel model = new ItemReturnViewModel();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                model = db.tb_item_return.Where(x => string.Compare(x.item_return_id, id) == 0).Select(x => new ItemReturnViewModel() { itemReturnId = x.item_return_id, itemReturnNumber = x.item_return_number, itemReturnStatus = x.item_return_status, created_date = x.created_date }).FirstOrDefault();
                model.inventories = Inventory.GetInventoryItem(model.itemReturnId);
                model.inventoryDetails = Inventory.GetInventoryDetail(model.itemReturnId);
                model.rejects = CommonClass.GetRejectByRequest(id);
            }
            return model;
        }
        public void DeleteReturnItemDetail(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory_detail.Where(x => x.inventory_ref_id == id).ToList();
                foreach (var inventory in inventories)
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
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == inv.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.ref_id = id;
                    inventory.inventory_status_id = "8";
                    inventory.warehouse_id = inv.inventory_warehouse_id;
                    inventory.product_id = inv.inventory_item_id;
                    inventory.out_quantity =CommonClass.ConvertMultipleUnit(inv.inventory_item_id,inv.unit,Convert.ToDecimal(inv.quantity));
                    inventory.in_quantity = 0;
                    inventory.total_quantity = totalQty - inventory.out_quantity;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }
            }
        }

        public List<ItemReturnViewModel> GetItemReturnToSupplierListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemReturnViewModel> itemReturns = new List<ItemReturnViewModel>();
                List<ItemReturnViewModel> objs = new List<ItemReturnViewModel>();
                string strWarehouse = string.Empty;
                string strInvoiceNumber = string.Empty;
                string strInvoiceDate = string.Empty;
                if (User.IsInRole(Role.SystemAdmin))
                {
                    objs = db.tb_item_return.OrderBy(x => x.item_return_number).Where(x => x.status == true).Select(x => new ItemReturnViewModel() { itemReturnId = x.item_return_id, itemReturnNumber = x.item_return_number, itemReturnStatus = x.item_return_status, created_date = x.created_date }).ToList();
                }
                else
                {
                    objs = db.tb_item_return.OrderBy(x => x.item_return_number).Where(x => x.status == true).Select(x => new ItemReturnViewModel() { itemReturnId = x.item_return_id, itemReturnNumber = x.item_return_number, itemReturnStatus = x.item_return_status, created_date = x.created_date }).ToList();
                }
                foreach (ItemReturnViewModel obj in objs)
                {
                    strWarehouse = string.Empty;
                    strInvoiceNumber = string.Empty;
                    strInvoiceDate = string.Empty;
                    ItemReturnViewModel itemReturn = new ItemReturnViewModel();
                    itemReturn.itemReturnId = obj.itemReturnId;
                    itemReturn.itemReturnNumber = obj.itemReturnNumber;
                    itemReturn.itemReturnStatus = obj.itemReturnStatus;
                    itemReturn.created_date = obj.created_date;
                    var oobjs = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.itemReturnId) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceDate = x.invoice_date, invoiceNumber = x.invoice_number }).ToList();
                    var dupWarehouse = oobjs.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceNumber = oobjs.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceDate = oobjs.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupWarehouse.Any())
                        foreach (var warehouseId in dupWarehouse)
                            strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString());
                    if (dupInvoiceNumber.Any())
                        foreach (var invoiceNumber in dupInvoiceNumber)
                            strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, invoiceNumber);
                    if (dupInvoiceDate.Any())
                        foreach (var invoiceDate in dupInvoiceDate)
                            strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(invoiceDate).ToString("dd/MM/yyy"));
                    foreach (var oobj in oobjs)
                    {
                        bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, oobj.warehouseId) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, oobj.invoiceNumber) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == oobj.invoiceDate).Count() > 0 ? true : false;
                        if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, oobj.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString());
                        if (!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, oobj.invoiceNumber);
                        if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(oobj.invoiceDate).ToString("dd/MM/yyyy"));
                    }
                    itemReturn.strWarehouse = strWarehouse;
                    itemReturn.strInvoiceDate = strInvoiceDate;
                    itemReturn.strInvoiceNumber = strInvoiceNumber;
                    itemReturns.Add(itemReturn);
                }
                return itemReturns;
            }
        }
        public List<tb_warehouse> GetWarehouseList(int iid = 0)
        {
            string id = User.Identity.GetUserId();

            List<tb_warehouse> warehouses = new List<tb_warehouse>();

            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (iid == 0)
                {
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        warehouses = (from wh in db.tb_warehouse
                                      join stock_keeper in db.tb_stock_keeper_warehouse on wh.warehouse_id equals stock_keeper.warehouse_id
                                      where wh.warehouse_status == true && stock_keeper.stock_keeper == id
                                      select wh).ToList();
                        return warehouses;
                    }
                    warehouses = db.tb_warehouse.OrderBy(m => m.warehouse_name).Where(m => m.warehouse_status == true).ToList();
                }
                else
                    warehouses = db.tb_warehouse.OrderBy(m => m.warehouse_name).Where(m => m.warehouse_status == true).ToList();
            }
            return warehouses;
        }
        public ActionResult CancelRequest(string id,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_return itemReturn = db.tb_item_return.Find(id);
                itemReturn.item_return_status = Status.RequestCancelled;
                itemReturn.updated_by = User.Identity.GetUserId();
                itemReturn.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
