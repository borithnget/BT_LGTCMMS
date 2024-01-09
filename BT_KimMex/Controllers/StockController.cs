using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using BT_KimMex.Class;
using BT_KimMex.Entities;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }

        // GET: Stock/Details/5
        public ActionResult CheckStockBalance(string id)
        {
            StockViewModel stock = new StockViewModel();
            List<STItemViewModel> stItems = new List<STItemViewModel>();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                stock.mrId = id;
                stock.mrNumber = db.tb_item_request.Find(id).ir_no;
                var whobj = (from mr in db.tb_item_request
                             join pro in db.tb_project on mr.ir_project_id equals pro.project_id into pp
                             from pro in pp.DefaultIfEmpty()
                             //join site in db.tb_site on pro.site_id equals site.site_id into st
                             //from site in st.DefaultIfEmpty()
                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into whh
                             from wh in whh.DefaultIfEmpty()
                             where string.Compare(mr.ir_id, id) == 0
                             select new { wh, pro }).FirstOrDefault();
                if (whobj != null)
                {
                    stock.warehouse = whobj.wh.warehouse_name;
                    stock.projectname = whobj.pro.project_full_name;
                }
                    

                List<Models.ItemRequestDetail2ViewModel> itemIDs = Class.ItemRequest.GetAllAvailableItembyStockTransfer(id, null);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true  && string.Compare(m.warehouse_id,whobj.wh.warehouse_id)!=0).ToList();
                foreach (var it in itemIDs)
                {
                    foreach (var wh in warehouses)
                    {

                        var irItem = (from inv in db.tb_inventory
                                      join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                      join pro in db.tb_product on inv.product_id equals pro.product_id
                                      orderby inv.inventory_date descending
                                      where inv.warehouse_id == wh.warehouse_id && inv.product_id == it.ir_item_id
                                      select new { inv, wah, pro }).FirstOrDefault();
                        if (irItem != null)
                        {

                            STItemViewModel item = new STItemViewModel();
                            item.itemID = irItem.inv.product_id;
                            item.itemCode = irItem.pro.product_code;
                            item.itemName = irItem.pro.product_name;
                            item.itemUnit = irItem.pro.product_unit;
                            item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                            item.Unit = it.ir_item_unit;//add new
                            item.unitName = db.tb_unit.Find(item.Unit).Name;
                            item.warehouseID = irItem.inv.warehouse_id;
                            item.warehouseName = irItem.wah.warehouse_name;
                            item.stockBalance = irItem.inv.total_quantity;
                            //item.requestQty = Class.CommonClass.ConvertMultipleUnit(irItem.inv.product_id, it.ir_item_unit, Convert.ToDecimal(it.remain_qty));
                            // item.requestQty = it.remain_qty;
                            item.requestQty = it.approved_qty;
                            item.requestUnit = it.ir_item_unit;
                            item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                            item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                            item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                            item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                            item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                            item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                            item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                            item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                            item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                            item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                            item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                            stItems.Add(item);

                            //add warehouse to list
                            var isExistWarehouse = stock.warehouses.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).FirstOrDefault()==null?false:true;
                            if (!isExistWarehouse)
                                stock.warehouses.Add(wh);
                        }


                    }
                }
                stock.stocks = stItems;
            }
            return View(stock);
        }
        public ActionResult CheckStockBalanceWorkshop(string id)
        {
            StockViewModel stock = new StockViewModel();
            List<STItemViewModel> stItems = new List<STItemViewModel>();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                stock.mrId = id;
                stock.mrNumber = db.tb_item_request.Find(id).ir_no;
                var whobj = (from mr in db.tb_item_request
                             join pro in db.tb_project on mr.ir_project_id equals pro.project_id into pp
                             from pro in pp.DefaultIfEmpty()
                             //join site in db.tb_site on pro.site_id equals site.site_id into st
                             //from site in st.DefaultIfEmpty()
                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into whh
                             from wh in whh.DefaultIfEmpty()
                             where string.Compare(mr.ir_id, id) == 0
                             select new { wh, pro }).FirstOrDefault();
                if (whobj != null)
                {
                    stock.warehouse = whobj.wh.warehouse_name;
                    stock.projectname = whobj.pro.project_full_name;
                }

                List<Models.ItemRequestDetail2ViewModel> itemIDs = Class.ItemRequest.GetAllAvailableItembyStockTransfer(id, null);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true).ToList();
                foreach (var it in itemIDs)
                {
                    var irItem = (from inv in db.tb_inventory
                                  join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                  join pro in db.tb_product on inv.product_id equals pro.product_id
                                  orderby inv.inventory_date descending
                                  where inv.warehouse_id == BT_KimMex.Class.EnumConstants.WORKSHOP && inv.product_id == it.ir_item_id
                                  select new { inv, wah, pro }).FirstOrDefault();
                    if (irItem != null)
                    {

                        STItemViewModel item = new STItemViewModel();
                        item.itemID = irItem.inv.product_id;
                        item.itemCode = irItem.pro.product_code;
                        item.itemName = irItem.pro.product_name;
                        item.itemUnit = irItem.pro.product_unit;
                        item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                        item.Unit = it.ir_item_unit;//add new
                        item.unitName = db.tb_unit.Find(item.Unit).Name;
                        item.warehouseID = irItem.inv.warehouse_id;
                        item.warehouseName = irItem.wah.warehouse_name;
                        item.stockBalance = irItem.inv.total_quantity;
                        //item.requestQty = Class.CommonClass.ConvertMultipleUnit(irItem.inv.product_id, it.ir_item_unit, Convert.ToDecimal(it.remain_qty));
                        // item.requestQty = it.remain_qty;
                        item.requestQty = it.approved_qty;
                        item.requestUnit = it.ir_item_unit;
                        item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                        item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                        item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                        item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                        item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                        item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                        item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                        item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                        item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                        item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                        item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                        stItems.Add(item);

                        
                    }
                }
                stock.stocks = stItems;
            }
            return View(stock);
        }
        public ActionResult WorkShopBalance()
        {
           
            return View();
            //return View(Class.Inventory.GetStockBalance(Class.EnumConstants.WORKSHOP));
        }
        [HttpPost]
        public ActionResult WorkShopBalanceAdjustmentAJAX(string product_code,decimal stock_balance)
        {
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                kim_mexEntities db= new kim_mexEntities();
                tb_product product = db.tb_product.Where(s => s.status == true && string.Compare(product_code.ToLower(), s.product_code.ToLower()) == 0).FirstOrDefault();
                if (product == null)
                {
                    isSuccess = false;
                    message = "This Product is not found.";
                }
                else
                {
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_status_id = "1";
                    inventory.inventory_date = CommonClass.ToLocalTime(DateTime.Now);
                    inventory.warehouse_id = EnumConstants.WORKSHOP;
                    inventory.product_id = product.product_id;
                    inventory.total_quantity = stock_balance;
                    inventory.in_quantity = 0;
                    inventory.out_quantity = 0;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }
            }catch(Exception ex)
            {
                isSuccess = false; message = ex.Message;
            }
            return Json(new {isSuccess=isSuccess, message=message},JsonRequestBehavior.AllowGet);
        }
        public ActionResult StockBalance()
        {
            return View();
        }

        public ActionResult GetSiteListItemsJSON()
        {
            return Json(new { data = this.GetSiteListItems() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWarehouseListItemsJSON()
        {            
            return Json(new { data=this.GetWarehouseListItems()}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllItemFromWarehousebySiteJSON(string id)
        {
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<InventoryViewModel> models = new List<InventoryViewModel>();
                //var whs = db.tb_warehouse.Where(s => s.warehouse_status == true && string.Compare(s.warehouse_site_id, id) == 0).ToList();
                var whs = (from wh in db.tb_warehouse
                           join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                           where wh.warehouse_status == true && string.Compare(wh.warehouse_site_id, id) == 0 && string.Compare(proj.p_status, "Completed") != 0
                           select new { wh }).ToList();
                foreach (var wh in whs)
                {
                    List<InventoryViewModel> inventories = this.GetAllItembyWarehouse(wh.wh.warehouse_id);
                    foreach(var inv in inventories)
                    {

                        var isExist = models.Where(s => string.Compare(s.product_id, inv.product_id) == 0).FirstOrDefault() == null ? false : true;
                        if (!isExist)
                            models.Add(inv);
                        else
                        {
                            var item = models.Where(s => string.Compare(s.product_id, inv.product_id) == 0).FirstOrDefault();
                            item.total_quantity = item.total_quantity + inv.total_quantity;
                        }
                    }
                }
                var jsonResult = Json(new { data = models }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Start Blocking item qty by warehouse
        [Authorize(Roles= "Admin,Site Stock Keeper")]
        public ActionResult ItemBlocking()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ItemBlockingViewModel> models = new List<ItemBlockingViewModel>();
                if (User.IsInRole(Role.SystemAdmin))
                {
                    models = (from ib in db.tb_item_blocking
                              join wh in db.tb_warehouse on ib.warehouse_id equals wh.warehouse_id
                              join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                              orderby ib.created_at descending
                              where ib.active == true
                              select new ItemBlockingViewModel()
                              {
                                  item_blocking_id=ib.item_blocking_id,
                                  item_blocking_number=ib.item_blocking_number,
                                  warehouse_id=ib.warehouse_id,
                                  warehouse_name=wh.warehouse_name,
                                  project_id=proj.project_id,
                                  project_name=proj.project_full_name,
                                  created_at=ib.updated_at,
                                  created_by=ib.updated_by,
                                  item_blocking_status=ib.item_blocking_status,
                              }).ToList();
                }else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    models = (from ib in db.tb_item_blocking
                              join wh in db.tb_warehouse on ib.warehouse_id equals wh.warehouse_id
                              join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                              join ssk in db.tb_stock_keeper_warehouse on wh.warehouse_id equals ssk.warehouse_id
                              orderby ib.created_at descending
                              where ib.active == true && string.Compare(ssk.stock_keeper,userid)==0
                              select new ItemBlockingViewModel()
                              {
                                  item_blocking_id = ib.item_blocking_id,
                                  item_blocking_number = ib.item_blocking_number,
                                  warehouse_id = ib.warehouse_id,
                                  warehouse_name = wh.warehouse_name,
                                  project_id = proj.project_id,
                                  project_name = proj.project_full_name,
                                  created_at = ib.updated_at,
                                  created_by = ib.updated_by,
                                  item_blocking_status = ib.item_blocking_status,
                              }).ToList();
                }

                return View(models);
            }
        }
        [Authorize(Roles = "Admin,Site Stock Keeper")]
        public ActionResult CreateItemBlocking()
        {
            ViewBag.Warehouses = CommonFunctions.GetWarehouseListItemsbySiteStockKeeper(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.SiteStockKeeper), User.Identity.GetUserId().ToString());
            return View();
        }
        [HttpPost]
        public ActionResult CreateItemBlocking(ItemBlockingViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_item_blocking ib = new tb_item_blocking();
                ib.item_blocking_id = Guid.NewGuid().ToString();
                ib.item_blocking_number = CommonClass.GenerateProcessNumber("IBQ");
                ib.warehouse_id = model.warehouse_id;
                ib.active = true;
                ib.item_blocking_status = Status.Active;
                ib.created_at = CommonClass.ToLocalTime(DateTime.Now);
                ib.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                ib.created_by = User.Identity.GetUserId().ToString();
                ib.updated_by = User.Identity.GetUserId().ToString();
                db.tb_item_blocking.Add(ib);

                foreach(var item in model.itemBlockingDetails)
                {
                    tb_item_blocking_detail ibd = new tb_item_blocking_detail();
                    ibd.item_blocking_detail_id = Guid.NewGuid().ToString();
                    ibd.item_blocking_id = ib.item_blocking_id;
                    ibd.warehouse_id = ib.warehouse_id;
                    ibd.item_blocking_date = CommonClass.ToLocalTime(DateTime.Now);
                    ibd.item_id = item.item_id;
                    ibd.is_block = true;
                    ibd.block_qty = item.block_qty;
                    ibd.active = true;
                    ibd.remark = item.remark;
                    db.tb_item_blocking_detail.Add(ibd);
                    db.SaveChanges();
                }

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ItemBlockingDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("ItemBlocking");
            return View(this.GetItemBlockingItem(id));
        }
        public ActionResult GetAllItemsbyWarehouseJson(string id)
        {
            kim_mexEntities db = new kim_mexEntities();
            List<Models.InventoryViewModel> models = new List<Models.InventoryViewModel>();
            ProjectViewModel project = (from pro in db.tb_project
                                        join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                        where string.Compare(wh.warehouse_id, id) == 0
                                        select new ProjectViewModel()
                                        {
                                            project_id = pro.project_id,
                                            project_full_name = pro.project_full_name,
                                        }).FirstOrDefault();
            models = this.GetAllItembyWarehouse(id);
            return Json(new { data = models,project=project }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ItemBlockingDeactive(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_item_blocking ib = db.tb_item_blocking.Find(id);
                ib.item_blocking_status = Status.DeActive;
                ib.updated_at =CommonClass.ToLocalTime(DateTime.Now);
                ib.updated_by = User.Identity.GetUserId();
                db.SaveChanges();

                var details = db.tb_item_blocking_detail.Where(w => w.active == true && string.Compare(w.item_blocking_id, ib.item_blocking_id) == 0).ToList();
                if (details.Any())
                {
                    foreach(var item in details)
                    {
                        string dib_id = item.item_blocking_detail_id;
                        tb_item_blocking_detail ibd = db.tb_item_blocking_detail.Find(dib_id);
                        ibd.active = false;
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ItemBlockingViewModel GetItemBlockingItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                ItemBlockingViewModel model = new ItemBlockingViewModel();
                model = (from ib in db.tb_item_blocking
                         join wh in db.tb_warehouse on ib.warehouse_id equals wh.warehouse_id
                         join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                         orderby ib.created_at descending
                         where string.Compare(ib.item_blocking_id,id)==0
                         select new ItemBlockingViewModel()
                         {
                             item_blocking_id = ib.item_blocking_id,
                             item_blocking_number = ib.item_blocking_number,
                             warehouse_id = ib.warehouse_id,
                             warehouse_name = wh.warehouse_name,
                             project_id = proj.project_id,
                             project_name = proj.project_full_name,
                             created_at = ib.updated_at,
                             created_by = ib.updated_by,
                             item_blocking_status=ib.item_blocking_status,
                         }).FirstOrDefault();
                model.itemBlockingDetails = (from ibd in db.tb_item_blocking_detail
                                             join item in db.tb_product on ibd.item_id equals item.product_id
                                             join unit in db.tb_unit on item.product_unit equals unit.Id
                                             orderby item.product_code
                                             where string.Compare(ibd.item_blocking_id, model.item_blocking_id) == 0
                                             select new ItemBlockingDetailViewModel()
                                             {
                                                 item_blocking_detail_id=ibd.item_blocking_detail_id,
                                                 item_blocking_id=ibd.item_blocking_id,
                                                 warehouse_id=ibd.warehouse_id,
                                                 warehouse_name=model.warehouse_name,
                                                 item_blocking_date=ibd.item_blocking_date,
                                                 item_id=ibd.item_id,
                                                 item_name=item.product_name,
                                                 item_unit=unit.Name,
                                                 item_code=item.product_code,
                                                 is_block=ibd.is_block,
                                                 block_qty=ibd.block_qty,
                                                 active=ibd.active,
                                                 remark=ibd.remark,
                                             }).ToList();
                return model;
            }
        }
        #endregion

        public List<Models.InventoryViewModel> GetAllItembyWarehouse(string id)
        {
            List<Models.InventoryViewModel> models = new List<Models.InventoryViewModel>();
            List<Models.InventoryViewModel> inventories = new List<Models.InventoryViewModel>();
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<Models.InventoryViewModel> orginalItems = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items1 = new List<Models.InventoryViewModel>();
                items1 = db.tb_inventory.OrderBy(x => x.product_id).ThenByDescending(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, id) == 0).Select(x => new Models.InventoryViewModel()
                {
                    inventory_date = x.inventory_date,
                    product_id = x.product_id,
                    warehouse_id = x.warehouse_id,
                    total_quantity = x.total_quantity,
                }).ToList();

                #region new enhance speed
                var groupItems = items1.GroupBy(s => s.product_id).Select(s => new { key = s.Key, item = s.OrderByDescending(x => x.inventory_date).FirstOrDefault() }).ToList();
                foreach(var item in groupItems)
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
                    if (iitem.total_quantity > 0)
                        models.Add(iitem);

                    items1.RemoveAll(s => string.Compare(s.product_id, item.item.product_id) == 0);
                }
                foreach(var item in items1.ToList())
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
                    if (iitem.total_quantity > 0)
                        models.Add(iitem);
                }
                #endregion
                //var dup = items1.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                //if (dup.Any())
                //{
                //    foreach (var ditem in dup)
                //    {
                //        Models.InventoryViewModel item = new Models.InventoryViewModel();
                //        item = items1.Where(x => string.Compare(x.product_id, ditem) == 0).First();
                //        items.Add(item);
                //    }
                //    foreach (var item in items1)
                //    {
                //        var checkItem = dup.Where(x => string.Compare(x, item.product_id) == 0).ToList();
                //        if (checkItem.Any()) { }
                //        else
                //            items.Add(new Models.InventoryViewModel() { inventory_date = item.inventory_date, product_id = item.product_id, warehouse_id = item.warehouse_id, total_quantity = item.total_quantity });
                //    }
                //}
                //else
                //    items = items1;

                //if (items.Any())
                //{
                //    foreach (var item in items)
                //    {
                //        if (item != null)
                //        {
                //            if (item.product_id != null)
                //            {
                //                Models.ProductViewModel product = Class.CommonClass.GetProductDetail(item.product_id);
                //                if (product != null)
                //                {
                //                    if (item.total_quantity>0)
                //                        orginalItems.Add(new Models.InventoryViewModel()
                //                        {
                //                            inventory_date = item.inventory_date,
                //                            product_id = item.product_id,
                //                            total_quantity = item.total_quantity,
                //                            warehouse_id = item.warehouse_id,
                //                            itemCode = Class.CommonClass.GetProductDetail(item.product_id).product_code,
                //                            itemName = Class.CommonClass.GetProductDetail(item.product_id).product_name,
                //                            itemUnit = Class.CommonClass.GetProductDetail(item.product_id).product_unit,
                //                            itemUnitName = Class.CommonClass.GetProductDetail(item.product_id).unit_name,
                //                        });
                //                }
                //            }

                //        }

                //    }
                //}

                //var duplicationItems = orginalItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                //if (duplicationItems.Any())
                //{
                //    foreach (var dItem in duplicationItems)
                //    {
                //        decimal totalQuantity = 0;
                //        var itemss = orginalItems.Where(x => string.Compare(x.product_id, dItem) == 0).Select(x => x.total_quantity).ToList();
                //        foreach (var quantiy in itemss)
                //            totalQuantity = totalQuantity + Convert.ToDecimal(quantiy);
                //        foreach (var item in orginalItems)
                //        {
                //            Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                //            iitem.inventory_date = item.inventory_date;
                //            iitem.product_id = item.product_id;
                //            iitem.itemCode = Class.CommonClass.GetProductDetail(iitem.product_id).product_code;
                //            iitem.itemName = Class.CommonClass.GetProductDetail(iitem.product_id).product_name;
                //            iitem.itemUnit = Class.CommonClass.GetProductDetail(iitem.product_id).product_unit;
                //            iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                //            iitem.warehouse_id = item.warehouse_id;
                //            if (string.Compare(item.product_id, dItem) == 0)
                //            {
                //                iitem.total_quantity = totalQuantity;
                //            }
                //            else
                //            {
                //                iitem.total_quantity = item.total_quantity;
                //            }
                //            if(iitem.total_quantity>0)
                //                models.Add(iitem);
                //        }
                //    }
                //}
                //else
                //    models = orginalItems;

                inventories = models.OrderBy(x => x.itemCode).ToList();
            }
            return inventories;
        }
        public List<WareHouseViewModel> GetWarehouseListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //return db.tb_warehouse.OrderBy(s => s.warehouse_name).Where(s => s.warehouse_status == true).Select(s => new WareHouseViewModel() { warehouse_id = s.warehouse_id, warehouse_name = s.warehouse_name }).ToList();
                if (User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteManager) || User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin) || User.IsInRole(Role.SiteStockKeeper) || User.IsInRole(Role.QAQCOfficer))
                {
                    List<WareHouseViewModel> wareHouses = new List<WareHouseViewModel>();
                    string userid = User.Identity.GetUserId();
                    if (User.IsInRole(Role.ProjectManager))
                    {
                        var models= (from wh in db.tb_warehouse
                                     join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                     join proj in db.tb_project_pm on pro.project_id equals proj.project_id
                                     where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0 && string.Compare(proj.project_manager_id,userid)==0
                                     select new WareHouseViewModel()
                                     {
                                         warehouse_id = wh.warehouse_id,
                                         warehouse_name = wh.warehouse_name,
                                     }).ToList();
                        foreach (var model in models)
                            wareHouses.Add(model);
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        var models = (from wh in db.tb_warehouse
                                      join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                      join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                      where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0 && string.Compare(sm.site_manager, userid) == 0
                                      select new WareHouseViewModel()
                                      {
                                          warehouse_id = wh.warehouse_id,
                                          warehouse_name = wh.warehouse_name,
                                      }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = wareHouses.Where(s => string.Compare(s.warehouse_id, model.warehouse_id) == 0).FirstOrDefault() == null ? false : true;
                            if(!isExist)
                                wareHouses.Add(model);
                        }
                            
                    }
                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        var models = (from wh in db.tb_warehouse
                                      join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                      join sup in db.tbSiteSiteSupervisors on pro.project_id equals sup.site_id
                                      where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0 && string.Compare(sup.site_supervisor_id, userid) == 0
                                      select new WareHouseViewModel()
                                      {
                                          warehouse_id = wh.warehouse_id,
                                          warehouse_name = wh.warehouse_name,
                                      }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = wareHouses.Where(s => string.Compare(s.warehouse_id, model.warehouse_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                wareHouses.Add(model);
                        }
                    }
                    if (User.IsInRole(Role.SiteAdmin))
                    {
                        var models = (from wh in db.tb_warehouse
                                      join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                      join sup in db.tb_site_site_admin on pro.project_id equals sup.site_id
                                      where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0 && string.Compare(sup.site_admin_id, userid) == 0
                                      select new WareHouseViewModel()
                                      {
                                          warehouse_id = wh.warehouse_id,
                                          warehouse_name = wh.warehouse_name,
                                      }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = wareHouses.Where(s => string.Compare(s.warehouse_id, model.warehouse_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                wareHouses.Add(model);
                        }
                    }
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        var models = (from wh in db.tb_warehouse
                                      join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                      join sup in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sup.warehouse_id
                                      where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0 && string.Compare(sup.stock_keeper, userid) == 0
                                      select new WareHouseViewModel()
                                      {
                                          warehouse_id = wh.warehouse_id,
                                          warehouse_name = wh.warehouse_name,
                                      }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = wareHouses.Where(s => string.Compare(s.warehouse_id, model.warehouse_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                wareHouses.Add(model);
                        }
                    }
                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        var models = (from wh in db.tb_warehouse
                                      join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                      join sup in db.tb_warehouse_qaqc on wh.warehouse_id equals sup.warehouse_id
                                      where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0 && string.Compare(sup.qaqc_id, userid) == 0
                                      select new WareHouseViewModel()
                                      {
                                          warehouse_id = wh.warehouse_id,
                                          warehouse_name = wh.warehouse_name,
                                      }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = wareHouses.Where(s => string.Compare(s.warehouse_id, model.warehouse_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                wareHouses.Add(model);
                        }
                    }

                    return wareHouses;
                }
                else
                {
                    return (from wh in db.tb_warehouse
                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                            where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0
                            select new WareHouseViewModel()
                            {
                                warehouse_id = wh.warehouse_id,
                                warehouse_name = wh.warehouse_name,
                            }).ToList();
                }
            }
        }
        public List<SiteViewModel> GetSiteListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string userid = User.Identity.GetUserId();
                List<SiteViewModel> models = new List<SiteViewModel>();
                
                if (User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteManager) || User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin) || User.IsInRole(Role.SiteStockKeeper) || User.IsInRole(Role.QAQCOfficer))
                {
                    if (User.IsInRole(Role.ProjectManager))
                    {
                        models = (from site in db.tb_site
                                    join pro in db.tb_project on site.site_id equals pro.site_id
                                    join pm in db.tb_project_pm on pro.project_id equals pm.project_id
                                    where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(pm.project_manager_id, userid) == 0
                                    select new SiteViewModel()
                                    {
                                        site_id = site.site_id,
                                        site_name = site.site_name
                                    }).ToList();
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        var objs = (from site in db.tb_site
                                  join pro in db.tb_project on site.site_id equals pro.site_id
                                  join pm in db.tb_site_manager_project on pro.project_id equals pm.project_id
                                  where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(pm.site_manager, userid) == 0
                                  select new SiteViewModel()
                                  {
                                      site_id = site.site_id,
                                      site_name = site.site_name
                                  }).ToList();
                        foreach(var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.site_id, obj.site_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        var objs = (from site in db.tb_site
                                    join pro in db.tb_project on site.site_id equals pro.site_id
                                    join pm in db.tbSiteSiteSupervisors on pro.project_id equals pm.site_id
                                    where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(pm.site_supervisor_id, userid) == 0
                                    select new SiteViewModel()
                                    {
                                        site_id = site.site_id,
                                        site_name = site.site_name
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.site_id, obj.site_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteAdmin))
                    {
                        var objs = (from site in db.tb_site
                                    join pro in db.tb_project on site.site_id equals pro.site_id
                                    join pm in db.tb_site_site_admin on pro.project_id equals pm.site_id
                                    where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(pm.site_admin_id, userid) == 0
                                    select new SiteViewModel()
                                    {
                                        site_id = site.site_id,
                                        site_name = site.site_name
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.site_id, obj.site_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        var objs = (from site in db.tb_site
                                    join pro in db.tb_project on site.site_id equals pro.site_id
                                    join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                    join pm in db.tb_stock_keeper_warehouse on wh.warehouse_id equals pm.warehouse_id
                                    where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(pm.stock_keeper, userid) == 0
                                    select new SiteViewModel()
                                    {
                                        site_id = site.site_id,
                                        site_name = site.site_name
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.site_id, obj.site_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        var objs = (from site in db.tb_site
                                    join pro in db.tb_project on site.site_id equals pro.site_id
                                    join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                    join pm in db.tb_warehouse_qaqc on wh.warehouse_id equals pm.warehouse_id
                                    where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(pm.qaqc_id, userid) == 0
                                    select new SiteViewModel()
                                    {
                                        site_id = site.site_id,
                                        site_name = site.site_name
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.site_id, obj.site_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                }
                else
                {
                    var sites = db.tb_site.Where(s => s.status == true).ToList();
                    foreach (var site in sites)
                    {
                        var projects = db.tb_project.Where(s => s.project_status == true && string.Compare(s.site_id, site.site_id) == 0 && string.Compare(s.p_status, ProjectStatus.Completed) != 0).ToList();
                        if (projects.Any())
                            models.Add(new SiteViewModel() { site_id = site.site_id, site_name = site.site_name });
                    }
                }
                    
                models = models.OrderBy(s => s.site_name).ToList();
                return models;
            }
        }
    }
}
