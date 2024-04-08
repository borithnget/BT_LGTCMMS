using BT_KimMex.Entities;
using BT_KimMex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BT_KimMex.Class
{
    public static class CommonClass
    {
        private static Entities.kim_mexEntities db = new kim_mexEntities();
        public static List<BT_KimMex.Models.WareHouseViewModel> Warehouses()
        {
            List<BT_KimMex.Models.WareHouseViewModel> warehouses = new List<Models.WareHouseViewModel>();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                
                    warehouses = db.tb_warehouse.OrderBy(x => x.warehouse_name).Where(x => x.warehouse_status == true).Select(x => new Models.WareHouseViewModel() { warehouse_id = x.warehouse_id, warehouse_name = x.warehouse_name }).ToList();
            }
            return warehouses;
        }
        
        public static List<BT_KimMex.Models.SupplierViewModel> Suppliers()
        {
            List<BT_KimMex.Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel() { supplier_id = x.supplier_id, supplier_name = x.supplier_name }).ToList();
            }
            return suppliers;
        }
        public static List<BT_KimMex.Models.StockUsageBySiteWithRemainBalanceViewModel> Site()
        {
            List<BT_KimMex.Models.StockUsageBySiteWithRemainBalanceViewModel> site = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                site = db.tb_site.OrderBy(x => x.site_name).Where(x => x.status == true).Select(x => new Models.StockUsageBySiteWithRemainBalanceViewModel() { site_id = x.site_id, Site_name = x.site_name }).ToList();
            }
            return site;
        }
        public static BT_KimMex.Models.PurchaseOrderViewModel GetPOSupplierItem(string id)
        {
            BT_KimMex.Models.PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            List<PurchaseOrderItemSupplier> poSuppliers = new List<PurchaseOrderItemSupplier>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                model = (from po in db.tb_purchase_order
                         join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                         join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                         join proj in db.tb_project on ir.ir_project_id equals proj.project_id
                         where po.purchase_order_id == id
                         select new PurchaseOrderViewModel() {soNo = proj.project_no, project_short_name = proj.project_short_name, ir_noMR =ir.ir_no, purchase_order_id = po.purchase_order_id, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = proj.project_full_name, purchase_oder_number = po.purchase_oder_number, purchase_order_status = po.purchase_order_status, created_date = po.created_date, checked_date = po.checked_date, approved_date = po.approved_date }).FirstOrDefault();
                if (model != null)
                {
                    if (string.Compare(model.purchase_order_status, "Completed") == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "approved"
                                     select new PurchaseOrderDetailViewModel()
                                     {
                                         brand_id = item.brand_id,
                                         item_vat = dPO.item_vat,
                                         po_detail_id = dPO.po_detail_id,
                                         purchase_order_id = dPO.purchase_order_id,
                                         item_id = dPO.item_id, quantity = dPO.quantity,
                                         product_code = item.product_code,
                                         product_name = item.product_name,
                                         product_unit = item.product_unit,
                                         unit_price = dPO.unit_price,
                                         item_status = dPO.item_status,
                                         po_quantity = dPO.po_quantity,
                                         po_unit = dPO.po_unit,
                                         original_price=dPO.original_price,
                                         discount_percentage=dPO.discount_percentage,
                                         lump_sum_discount_amount=dPO.lump_sum_discount_amount,
                                     }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() { po_supplier_id = pos.po_supplier_id, po_detail_id = pos.po_detail_id, unit_price = pos.unit_price, supplier_id = sup.supplier_id, supplier_name = sup.supplier_name, sup_number = pos.sup_number, is_selected = pos.is_selected, Reason = pos.Reason, po_quantity = pos.po_qty, supplier_address = sup.supplier_address, supplier_phone = sup.supplier_phone, supplier_email = sup.supplier_email, discount = sup.discount }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() { supplier_id = sup, supplier_name = supplier.supplier_name, supplier_email = supplier.supplier_email, supplier_address = supplier.supplier_address, supplier_phone = supplier.supplier_phone, discount = supplier.discount });
                            }
                        }
                    }
                    else if (string.Compare(model.purchase_order_status, "Approved") == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "Pending"
                                     select new PurchaseOrderDetailViewModel() {
                                         brand_id = item.brand_id,
                                         item_vat = dPO.item_vat,
                                         po_detail_id = dPO.po_detail_id,
                                         purchase_order_id = dPO.purchase_order_id,
                                         item_id = dPO.item_id,
                                         quantity = dPO.quantity,
                                         product_code = item.product_code,
                                         product_name = item.product_name,
                                         product_unit = item.product_unit,
                                         unit_price = dPO.unit_price,
                                         item_status = dPO.item_status,
                                         po_quantity = dPO.po_quantity,
                                         po_unit = dPO.po_unit,
                                         original_price = dPO.original_price,
                                         discount_percentage = dPO.discount_percentage,
                                         lump_sum_discount_amount=dPO.lump_sum_discount_amount,
                                     }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() { po_supplier_id = pos.po_supplier_id, po_detail_id = pos.po_detail_id, unit_price = pos.unit_price, supplier_id = sup.supplier_id, supplier_name = sup.supplier_name, sup_number = pos.sup_number, is_selected = pos.is_selected, Reason = pos.Reason, po_quantity = pos.po_qty, supplier_address = sup.supplier_address, supplier_phone = sup.supplier_phone, supplier_email = sup.supplier_email, discount = sup.discount }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() { supplier_id = sup, supplier_name = supplier.supplier_name, supplier_email = supplier.supplier_email, supplier_address = supplier.supplier_address, supplier_phone = supplier.supplier_phone, discount = supplier.discount });
                            }
                        }
                    }else if(string.Compare(model.purchase_order_status, Status.MREditted) == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && (dPO.item_status == "Pending" || string.Compare(dPO.item_status, "approved")==0)
                                     select new PurchaseOrderDetailViewModel()
                                     {
                                         brand_id = item.brand_id,
                                         item_vat = dPO.item_vat,
                                         po_detail_id = dPO.po_detail_id,
                                         purchase_order_id = dPO.purchase_order_id,
                                         item_id = dPO.item_id,
                                         quantity = dPO.quantity,
                                         product_code = item.product_code,
                                         product_name = item.product_name,
                                         product_unit = item.product_unit,
                                         unit_price = dPO.unit_price,
                                         item_status = dPO.item_status,
                                         po_quantity = dPO.po_quantity,
                                         po_unit = dPO.po_unit,
                                         original_price = dPO.original_price,
                                         discount_percentage = dPO.discount_percentage,
                                         lump_sum_discount_amount = dPO.lump_sum_discount_amount,
                                     }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() { po_supplier_id = pos.po_supplier_id, po_detail_id = pos.po_detail_id, unit_price = pos.unit_price, supplier_id = sup.supplier_id, supplier_name = sup.supplier_name, sup_number = pos.sup_number, is_selected = pos.is_selected, Reason = pos.Reason, po_quantity = pos.po_qty, supplier_address = sup.supplier_address, supplier_phone = sup.supplier_phone, supplier_email = sup.supplier_email, discount = sup.discount }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() { supplier_id = sup, supplier_name = supplier.supplier_name, supplier_email = supplier.supplier_email, supplier_address = supplier.supplier_address, supplier_phone = supplier.supplier_phone, discount = supplier.discount });
                            }
                        }
                    }
                }
            }
            return model;
        }
        public static BT_KimMex.Models.StockMovementViewModel GetStockMovement(string id)
        {
            BT_KimMex.Models.StockMovementViewModel model = new StockMovementViewModel();

            using (kim_mexEntities db = new kim_mexEntities())
            {


            }
            return model;
        }
        public static string GetUserFullname(string username)
        {
          string fullname = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var user = db.AspNetUsers.Where(x => string.Compare(x.Id, username) == 0).FirstOrDefault();
                if (user != null)
                {
                    string userId = user.Id;
                    var userDetail = db.tb_user_detail.Where(x => string.Compare(x.user_id, userId) == 0).FirstOrDefault();
                    if (userDetail != null)
                        fullname = userDetail.user_first_name + " " + userDetail.user_last_name;
                }
            }
            return fullname;
        }
        public static string GetUserSignature(string userId)
        {
            string fullname = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var userDetail = db.tb_user_detail.Where(x => string.Compare(x.user_id, userId) == 0).FirstOrDefault();
                if (userDetail != null)
                    fullname = userDetail.user_signature;
            }
            return fullname;
        }
        public static string GetUserId(string username)
        {
            string userId = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                userId = db.AspNetUsers.Where(x => string.Compare(x.UserName, username) == 0).FirstOrDefault() == null ? string.Empty : db.AspNetUsers.Where(x => string.Compare(x.UserName, username) == 0).FirstOrDefault().Id;
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetUserId", ex.StackTrace, ex.Message);
            }
            return userId;
        }
        public static string getUserSignaturebyAttachmentId(string id)
        {
            string signatureUrl = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var attachment = db.tb_attachment.Find(id);
                if (attachment != null)
                {
                    signatureUrl= string.Format("/Documents/Signature/{0}{1}", attachment.attachment_id, attachment.attachment_extension);
                }
                  
            }catch(Exception ex)
            {

            }
            return signatureUrl;
        }
        public static List<RejectViewModel> GetRejectByRequest(string id)
        {
            List<RejectViewModel> rejects = new List<RejectViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var rejs = db.tb_reject.Where(x => x.ref_id == id).ToList();
                if (rejs.Any())
                {
                    foreach (var r in rejs)
                    {
                        rejects.Add(new RejectViewModel() { comment = r.comment, rejected_by = CommonClass.GetUserFullname(r.rejected_by) });
                    }
                }
            }
            return rejects;
        }
        public static List<BOQViewModel> GetProjectsDropdownList()
        {
            List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                boqs = (from boq in db.tb_build_of_quantity
                        join project in db.tb_project on boq.project_id equals project.project_id
                        where boq.status == true && boq.boq_status == "Completed"
                        select new BT_KimMex.Models.BOQViewModel() { boq_id = boq.boq_id, project_id = boq.project_id, project_full_name = project.project_full_name }).ToList();
            }
            return boqs;
        }
       
        public static List<SelectListItem> ClassType
        {
            get
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    return db.tb_class_type.Select(x => new SelectListItem
                    {
                        Value = x.class_type_name == null ? string.Empty : x.class_type_name,
                        Text = x.class_type_name == null ? string.Empty : x.class_type_name,
                    }).ToList();
                }
            }
        }
        #region added jul 20 2018
        public static List<ProjectViewModel> GetAllProject(string userid=null)
        {
            List<ProjectViewModel> models = new List<ProjectViewModel>();
            if(string.IsNullOrEmpty(userid))
                models = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.p_status == "Active" && x.project_status == true).Select(x => new ProjectViewModel() { project_id = x.project_id, project_no = x.project_no, project_short_name = x.project_short_name, project_full_name = x.project_full_name }).ToList();
            else
            {
                models = (from pro in db.tb_project
                          //join site in db.tb_site on pro.site_id equals site.site_id
                          join sitesup in db.tbSiteSiteSupervisors on pro.project_id equals sitesup.site_id
                          where string.Compare(pro.p_status,"Active")==0 && pro.project_status==true &&  string.Compare(sitesup.site_supervisor_id, userid) == 0
                          select new ProjectViewModel()
                          {
                              project_id = pro.project_id,
                              project_no = pro.project_no,
                              project_short_name = pro.project_short_name,
                              project_full_name = pro.project_full_name
                          }).ToList();
            }
            return models;
        }
        #endregion

        public static List<StockBalanceBywarehouseViewModel> GetStockBalanceByWarehouse()
        {

            List<BT_KimMex.Models.StockBalanceBywarehouseViewModel> stockW = new List<BT_KimMex.Models.StockBalanceBywarehouseViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {

                //stockW = (from ware in db.tb_warehouse
                //          join inVet in db.tb_inventory on ware.warehouse_id equals inVet.warehouse_id
                //          where inVet.warehouse_id == ware.warehouse_id
                //          select new BT_KimMex.Models.StockBalanceBywarehouseViewModel()
                //          {
                //              warehouseName = ware.warehouse_name,
                //              warehouse_id = ware.warehouse_id,

                //          }).ToList();

                //stockW = (from invet in db.tb_inventory
                //          join ware in db.tb_warehouse on invet.inventory_id equals ware.warehouse_id
                //          where ware.warehouse_id == invet.warehouse_id
                //          select new BT_KimMex.Models.StockBalanceBywarehouseViewModel()
                //          {
                //              inventory_id=invet.inventory_id,
                //              warehouse_id=ware.warehouse_id,
                //              warehouseName=ware.warehouse_name,

                //          }
                //          ).ToList();

                stockW = (from ware in db.tb_warehouse
                          orderby ware.warehouse_name
                          where ware.warehouse_status == true
                          select new BT_KimMex.Models.StockBalanceBywarehouseViewModel()
                          {
                              warehouseName = ware.warehouse_name,
                              warehouse_id = ware.warehouse_id,
                          }
                         ).ToList();
            }
            return stockW;
        }
        //kosal
        public static List<StockBalanceByDateandWarehouseViewModel> GetStockBalanceByDateandWarehouse(string userid=null)
        {

            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockW = new List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(userid))
                {
                    stockW = (from ware in db.tb_warehouse
                              orderby ware.warehouse_name
                              where ware.warehouse_status == true
                              select new BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel()
                              {
                                  warehouseName = ware.warehouse_name,
                                  warehouse_id = ware.warehouse_id,
                              }).ToList();
                }
                else
                {
                    stockW = (from wh in db.tb_warehouse
                              join pr in db.tb_project on wh.warehouse_project_id equals pr.project_id into proj
                              from pr in proj.DefaultIfEmpty()
                              join sk in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sk.warehouse_id
                              orderby wh.warehouse_name
                              where wh.warehouse_status == true && pr.project_status == true && string.Compare(sk.stock_keeper, userid) == 0 && string.Compare(pr.p_status,ProjectStatus.Active)==0
                              select new StockBalanceByDateandWarehouseViewModel()
                              {
                                  warehouse_id = wh.warehouse_id,
                                  warehouseName = wh.warehouse_name
                              }).ToList();
                }
            }
            return stockW;
        }
        public static List<StockBalanceByDateandWarehouseViewModel> GetStockBalanceByDateandWarehouseProject(string userid=null)
        {
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockWP = new List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(userid))
                {
                    stockWP = (from ware in db.tb_project
                               where ware.project_status == true
                               select new BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel()
                               {
                                   projectName = ware.project_full_name,
                                   project_id = ware.project_id,
                               }).ToList();
                }
                else
                {
                    stockWP= (from wh in db.tb_warehouse
                              join pr in db.tb_project on wh.warehouse_project_id equals pr.project_id into proj
                              from pr in proj.DefaultIfEmpty()
                              join sk in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sk.warehouse_id
                              where wh.warehouse_status == true && pr.project_status == true && string.Compare(sk.stock_keeper, userid) == 0 && string.Compare(pr.p_status, ProjectStatus.Active) == 0
                              select new StockBalanceByDateandWarehouseViewModel()
                              {
                                  project_id=pr.project_id,
                                  projectName=pr.project_full_name,
                              }).ToList();
                }


            }
            return stockWP;
        }
        public static string GetJobCategoryCode(string projectId, string jobCategoryId)
        {
            string jobCategoryCode = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                jobCategoryCode = (from boq in db.tb_build_of_quantity
                                   join boqJob in db.tb_boq_detail1 on boq.boq_id equals boqJob.boq_id
                                   where boq.project_id == projectId && boq.status == true && boqJob.job_category_id == jobCategoryId
                                   select boqJob.job_category_code).FirstOrDefault();
            }
            return jobCategoryCode;
        }
        public static List<Models.UnitViewModel> GetAllUnits()
        {
            List<UnitViewModel> units = new List<UnitViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var unitList = db.tb_unit.OrderBy(m => m.Name).Where(m => m.status == true).ToList();
                if (unitList.Any())
                {
                    foreach (var unit in unitList)
                    {
                        units.Add(new UnitViewModel() { Id = unit.Id, Name = Regex.Replace(unit.Name.Trim(), @"\t|\n|\r", "") });
                    }
                }
            }
            return units;
        }
        public static decimal ConvertMultipleUnit(string productId, string unit, decimal qty, string baseUnit = null)
        {
            decimal quantity = 0;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string defaultUnit = db.tb_product.Where(x => string.Compare(x.product_id, productId) == 0).Select(x => x.product_unit).FirstOrDefault();
                unit = string.IsNullOrEmpty(unit) ? string.Empty : Regex.Replace(unit.Trim(), @"\t|\n|\r", "");
                defaultUnit = Regex.Replace(defaultUnit.Trim(), @"\t|\n|\r", "");
                if (string.Compare(unit, defaultUnit) == 0 || string.Compare(unit, baseUnit) == 0)
                    quantity = qty;
                else
                {
                    var uom = db.tb_multiple_uom.Where(x => string.Compare(x.product_id, productId) == 0).Select(x => new ProductViewModel()
                    { uom1_id = x.uom1_id,
                      uom1_qty = x.uom1_qty,
                      uom2_id = x.uom2_id,
                      uom2_qty = x.uom2_qty,
                      uom3_id = x.uom3_id,
                      uom3_qty = x.uom3_qty,
                      uom4_id = x.uom4_id,
                      uom4_qty = x.uom4_qty,
                      uom5_id = x.uom5_id,
                      uom5_qty = x.uom5_qty
                    }).FirstOrDefault();
                    if (uom != null)
                    {
                        if (uom.uom1_id != null && uom.uom1_qty != null)
                        {
                            string uom1 = Regex.Replace(uom.uom1_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom1) == 0)
                            {
                                quantity = Convert.ToDecimal(qty / uom.uom1_qty);
                            }
                        }
                        if (uom.uom2_id != null && uom.uom2_qty != null)
                        {
                            string uom2 = Regex.Replace(uom.uom2_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom2) == 0)
                            {
                                quantity = Convert.ToDecimal((qty / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                        if (uom.uom3_id != null && uom.uom3_qty != null)
                        {
                            string uom3 = Regex.Replace(uom.uom3_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom3) == 0)
                            {
                                quantity = Convert.ToDecimal(((qty / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                        if (uom.uom4_id != null && uom.uom4_qty != null)
                        {
                            string uom4 = Regex.Replace(uom.uom4_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom4) == 0)
                            {
                                quantity = Convert.ToDecimal((((qty / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                        if (uom.uom5_id != null && uom.uom5_qty != null)
                        {
                            string uom5 = Regex.Replace(uom.uom5_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom5) == 0)
                            {
                                quantity = Convert.ToDecimal(((((qty / uom.uom5_qty) / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                    }
                }
            }
            return quantity;
        }
        public static decimal ConvertMultipleUnit1(string productId, string unit, decimal qty, string baseUnit = null)
        {
            decimal quantity = 0;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string defaultUnit = db.tb_product.Where(x => string.Compare(x.product_id, productId) == 0).Select(x => x.product_unit).FirstOrDefault();
                unit = string.IsNullOrEmpty(unit) ? string.Empty : Regex.Replace(unit.Trim(), @"\t|\n|\r", "");
                defaultUnit = Regex.Replace(defaultUnit.Trim(), @"\t|\n|\r", "");
                if (string.Compare(unit, defaultUnit) == 0 || string.Compare(unit, baseUnit) == 0)
                    quantity = qty;
                else
                {
                    var uom = db.tb_multiple_uom.Where(x => string.Compare(x.product_id, productId) == 0).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                    if (uom != null)
                    {
                        if (uom.uom1_id != null && uom.uom1_qty != null)
                        {
                            string uom1 = Regex.Replace(uom.uom1_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom1) == 0)
                            {
                                quantity = Convert.ToDecimal(qty / uom.uom1_qty);
                            }
                        }
                        if (uom.uom2_id != null && uom.uom2_qty != null)
                        {
                            string uom2 = Regex.Replace(uom.uom2_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom2) == 0)
                            {
                                quantity = Convert.ToDecimal((qty / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                        if (uom.uom3_id != null && uom.uom3_qty != null)
                        {
                            string uom3 = Regex.Replace(uom.uom3_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom3) == 0)
                            {
                                quantity = Convert.ToDecimal(((qty / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                        if (uom.uom4_id != null && uom.uom4_qty != null)
                        {
                            string uom4 = Regex.Replace(uom.uom4_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom4) == 0)
                            {
                                quantity = Convert.ToDecimal((((qty / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                        if (uom.uom5_id != null && uom.uom5_qty != null)
                        {
                            string uom5 = Regex.Replace(uom.uom5_id.Trim(), @"\t|\n|\r", "");
                            if (string.Compare(unit, uom5) == 0)
                            {
                                quantity = Convert.ToDecimal(((((qty / uom.uom5_qty) / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                            }
                        }
                    }
                }
            }
            return quantity;
        }
        public static List<ItemRequestViewModel> GetAvailableItemRequestList()
        {
            List<ItemRequestViewModel> itemRequests = new List<ItemRequestViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemRequestDetail2ViewModel> originalIR = new List<ItemRequestDetail2ViewModel>();
                List<ItemRequestDetail2ViewModel> itemRequestTransactionRefs = new List<ItemRequestDetail2ViewModel>();
                var objs = db.tb_item_request.OrderBy(x => x.created_date).Where(x => x.status == true && string.Compare(x.ir_status, "Approved") == 0).ToList();
                if (objs.Any())
                {
                    foreach (var obj in objs)
                    {
                        originalIR = new List<ItemRequestDetail2ViewModel>();
                        originalIR = CommonClass.GetItemRequestItems(obj.ir_id);
                        itemRequestTransactionRefs = new List<ItemRequestDetail2ViewModel>();
                        #region check in purchase order
                        var purchaseOrders = db.tb_purchase_order.OrderBy(x => x.created_by).Where(m => m.status == true && string.Compare(m.purchase_order_status, "Rejected") != 0 && string.Compare(m.item_request_id, obj.ir_id) == 0).ToList();
                        if (purchaseOrders.Any())
                        {
                            List<ItemRequestDetail2ViewModel> purchaseOrderIR = new List<ItemRequestDetail2ViewModel>();
                            List<ItemRequestDetail2ViewModel> purchaseOrderConverted = new List<ItemRequestDetail2ViewModel>();

                            foreach (var purchaseOrder in purchaseOrders)
                            {
                                var poDetails = db.tb_purchase_order_detail.Where(m => string.Compare(m.purchase_order_id, purchaseOrder.purchase_order_id) == 0 && string.Compare(m.item_status, "rejected") != 0 && m.status == true).ToList();
                                if (poDetails.Any())
                                {
                                    foreach (var pos in poDetails)
                                    {
                                        decimal quantity = Convert.ToDecimal((Convert.ToDecimal(pos.po_quantity) == 0 || pos.po_quantity == null) ? pos.quantity : pos.po_quantity);
                                        string unit = string.IsNullOrEmpty(pos.po_unit) ? Regex.Replace(pos.item_unit.Trim(), @"\t|\n|\r", "") : Regex.Replace(pos.po_unit.Trim(), @"\t|\n|\r", "");

                                        ItemRequestDetail2ViewModel item = new ItemRequestDetail2ViewModel();
                                        item.ir_item_id = pos.item_id;
                                        /*
                                        item.approved_qty = (Convert.ToDecimal(pos.po_quantity) == 0 || pos.po_quantity == null) ? pos.quantity : pos.po_quantity;
                                        item.ir_item_unit = string.IsNullOrEmpty(pos.po_unit) ? Regex.Replace(pos.item_unit.Trim(), @"\t|\n|\r", "") : Regex.Replace(pos.po_unit.Trim(), @"\t|\n|\r", "");
                                        purchaseOrderIR.Add(item);   
                                     */
                                        item.approved_qty = CommonClass.ConvertMultipleUnit(pos.item_id, unit, quantity);
                                        itemRequestTransactionRefs.Add(item);
                                    }
                                }
                            }

                            /*
                            decimal totalPurchaseOrderQuatity = 0;
                            var duplicationItems = purchaseOrderIR.GroupBy(x => new { x.ir_item_id, x.ir_item_unit }).Where(x => x.Count() > 1).Select(x => new { x.Key }).ToList();
                            if (duplicationItems.Any())
                            {
                                foreach (var item in duplicationItems)
                                {
                                    totalPurchaseOrderQuatity = 0;
                                    var duplicatePOItems = purchaseOrderIR.Where(x => string.Compare(x.ir_item_id, item.Key.ir_item_id) == 0).Select(x => x.approved_qty).ToList();
                                    foreach (var poQty in duplicatePOItems)
                                        totalPurchaseOrderQuatity = totalPurchaseOrderQuatity + Convert.ToDecimal(poQty);
                                    ItemRequestDetail2ViewModel irItem = new ItemRequestDetail2ViewModel();
                                    irItem.ir_item_id = item.Key.ir_item_id;
                                    irItem.approved_qty = totalPurchaseOrderQuatity;
                                    irItem.ir_item_unit = item.Key.ir_item_unit;
                                    purchaseOrderConverted.Add(irItem);
                                    foreach (var poItem in purchaseOrderIR)
                                    {
                                        if (string.Compare(poItem.ir_item_id, item.Key.ir_item_id) == 0)
                                            break;
                                        else
                                        {
                                            irItem = new ItemRequestDetail2ViewModel();
                                            irItem.ir_item_id = poItem.ir_item_id;
                                            irItem.ir_item_unit = poItem.ir_item_unit;
                                            irItem.approved_qty = poItem.approved_qty;
                                            purchaseOrderConverted.Add(irItem);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                purchaseOrderConverted = purchaseOrderIR;
                            }
                            int count = 0;
                            foreach (ItemRequestDetail2ViewModel ir in originalIR)
                            {
                                decimal poQuantity = 0;
                                decimal irQuantity = CommonClass.ConvertMultipleUnit(ir.ir_item_id, ir.ir_item_unit, Convert.ToDecimal(ir.approved_qty));
                                var item = purchaseOrderConverted.Where(x => string.Compare(ir.ir_item_id, x.ir_item_id) == 0).ToList();
                                if (item.Any())
                                {
                                    foreach (var po in item)
                                    {
                                        poQuantity = poQuantity + CommonClass.ConvertMultipleUnit(po.ir_item_id, po.ir_item_unit, Convert.ToDecimal(po.approved_qty));
                                    }
                                }
                                if (poQuantity < irQuantity)
                                    count++;
                            }
                            
                            if (count > 0)
                                itemRequests.Add(new ItemRequestViewModel() { ir_id = obj.ir_id, ir_no = obj.ir_no });
                            */
                        }
                        #endregion
                        #region check with stock transfer
                        var stockTransfers = db.tb_stock_transfer_voucher.OrderBy(x => x.created_by).Where(x => x.status == true && string.Compare(x.stock_transfer_status, "Rejected") != 0 && string.Compare(x.item_request_id, obj.ir_id) == 0).ToList();
                        if (stockTransfers.Any())
                        {
                            foreach (var stockTransfer in stockTransfers)
                            {
                                var stockTransferDetails = db.tb_inventory.Where(x => string.Compare(x.ref_id, stockTransfer.stock_transfer_id) == 0).ToList();
                                if (stockTransferDetails.Any())
                                {
                                    foreach (var stockTransferDetail in stockTransferDetails)
                                    {
                                        itemRequestTransactionRefs.Add(new ItemRequestDetail2ViewModel()
                                        {
                                            ir_item_id = stockTransferDetail.product_id,
                                            approved_qty = stockTransferDetail.out_quantity,
                                        });
                                    }

                                }
                            }
                        }
                        #endregion
                        if (itemRequestTransactionRefs.Any())
                        {
                            int count = 0;
                            foreach (ItemRequestDetail2ViewModel item in originalIR)
                            {
                                decimal requestQuantity = CommonClass.ConvertMultipleUnit(item.ir_item_id, item.ir_item_unit, Convert.ToDecimal(item.approved_qty));
                                var results = itemRequestTransactionRefs.Where(x => string.Compare(x.ir_item_id, item.ir_item_id) == 0).ToList();
                                if (results.Any())
                                {
                                    foreach (var rs in results)
                                        requestQuantity = requestQuantity - Convert.ToDecimal(rs.approved_qty);
                                    if (requestQuantity > 0)
                                        itemRequests.Add(new ItemRequestViewModel() { ir_id = obj.ir_id, ir_no = obj.ir_no });
                                }
                                else
                                    count++;
                            }
                            if (count > 0)
                                itemRequests.Add(new ItemRequestViewModel() { ir_id = obj.ir_id, ir_no = obj.ir_no });
                        }
                        else
                            itemRequests.Add(new ItemRequestViewModel() { ir_id = obj.ir_id, ir_no = obj.ir_no });
                    }
                }
            }
            return itemRequests;
        }
        public static List<ItemRequestDetail2ViewModel> GetItemRequestItems(string id)
        {
            List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //get all original item request
                var irJobCategories = db.tb_ir_detail1.Where(x => string.Compare(x.ir_id, id) == 0).ToList();
                if (irJobCategories.Any())
                {
                    foreach (var job in irJobCategories)
                    {
                        items = db.tb_ir_detail2.Where(x => string.Compare(x.ir_detail1_id, job.ir_detail1_id) == 0 && x.is_approved == true)
                            .Select(x => new ItemRequestDetail2ViewModel()
                            {
                                ir_item_id = x.ir_item_id,
                                ir_item_unit = x.ir_item_unit,
                                ir_qty = x.ir_qty,
                                approved_qty = x.approved_qty
                            }).ToList();
                    }
                }
            }
            return items;
        }
        public static List<ItemRequestDetail2ViewModel> GetAvailableRequestItemDetails(string id)
        {
            List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemRequestDetail2ViewModel> originalIR = new List<ItemRequestDetail2ViewModel>();
                List<ItemRequestDetail2ViewModel> itemRequestTransactionRefs = new List<ItemRequestDetail2ViewModel>();
                originalIR = CommonClass.GetItemRequestItems(id);

                #region check in purchase order
                var purchaseOrders = db.tb_purchase_order.OrderBy(x => x.created_by).Where(m => m.status == true && string.Compare(m.purchase_order_status, "Rejected") != 0 && string.Compare(m.item_request_id, id) == 0).ToList();
                if (purchaseOrders.Any())
                {
                    foreach (var purchaseOrder in purchaseOrders)
                    {
                        var poDetails = db.tb_purchase_order_detail.Where(m => string.Compare(m.purchase_order_id, purchaseOrder.purchase_order_id) == 0 && string.Compare(m.item_status, "rejected") != 0 && m.status == true).ToList();
                        if (poDetails.Any())
                        {
                            foreach (var pos in poDetails)
                            {
                                decimal quantity = Convert.ToDecimal((Convert.ToDecimal(pos.po_quantity) == 0 || pos.po_quantity == null) ? pos.quantity : pos.po_quantity);
                                string unit = string.IsNullOrEmpty(pos.po_unit) ? Regex.Replace(pos.item_unit.Trim(), @"\t|\n|\r", "") : Regex.Replace(pos.po_unit.Trim(), @"\t|\n|\r", "");

                                ItemRequestDetail2ViewModel item = new ItemRequestDetail2ViewModel();
                                item.ir_item_id = pos.item_id;
                                item.approved_qty = CommonClass.ConvertMultipleUnit(pos.item_id, unit, quantity);
                                itemRequestTransactionRefs.Add(item);
                            }
                        }
                    }
                }
                #endregion
                #region check with stock transfer
                var stockTransfers = db.tb_stock_transfer_voucher.OrderBy(x => x.created_by).Where(x => x.status == true && string.Compare(x.stock_transfer_status, "Rejected") != 0 && string.Compare(x.item_request_id, id) == 0).ToList();
                if (stockTransfers.Any())
                {
                    foreach (var stockTransfer in stockTransfers)
                    {
                        var stockTransferDetails = db.tb_inventory.Where(x => string.Compare(x.ref_id, stockTransfer.stock_transfer_id) == 0).ToList();
                        if (stockTransferDetails.Any())
                        {
                            foreach (var stockTransferDetail in stockTransferDetails)
                            {
                                itemRequestTransactionRefs.Add(new ItemRequestDetail2ViewModel()
                                {
                                    ir_item_id = stockTransferDetail.product_id,
                                    approved_qty = stockTransferDetail.out_quantity,
                                });
                            }
                        }
                    }
                }
                #endregion
                if (itemRequestTransactionRefs.Any())
                {
                    foreach (ItemRequestDetail2ViewModel item in originalIR)
                    {
                        decimal requestQuantity = CommonClass.ConvertMultipleUnit(item.ir_item_id, item.ir_item_unit, Convert.ToDecimal(item.approved_qty));
                        var results = itemRequestTransactionRefs.Where(x => string.Compare(x.ir_item_id, item.ir_item_id) == 0).ToList();
                        if (results.Any())
                        {
                            foreach (var rs in results)
                                requestQuantity = requestQuantity - Convert.ToDecimal(rs.approved_qty);
                            if (requestQuantity > 0)
                                items.Add(new ItemRequestDetail2ViewModel() { ir_item_id = item.ir_item_id, ir_qty = requestQuantity, ir_item_unit = item.ir_item_unit });
                        }
                        else
                            items.Add(new ItemRequestDetail2ViewModel() { ir_item_id = item.ir_item_id, ir_qty = requestQuantity, ir_item_unit = item.ir_item_unit });
                    }
                }
                else
                    items = originalIR;
            }
            return items;
        }
        public static Models.ProductViewModel GetProductDetail(string id)
        {
            ProductViewModel product = new ProductViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                product = (from item in db.tb_product
                           //join type in db.tb_brand on item.brand_id equals type.brand_id
                           join unit in db.tb_unit on item.product_unit equals unit.Id
                           where string.Compare(item.product_id, id) == 0
                           select new Models.ProductViewModel()
                           {
                               product_id = item.product_id,
                               product_code = item.product_code,
                               product_name = item.product_name,
                               product_unit = item.product_unit,
                               unit_name=unit.Name,
                               unit_price = item.unit_price,
                               //p_category_name = type.brand_name
                           }).FirstOrDefault();
            }
            return product;
        }
        public static void AutoGenerateStockInvoiceNumber(string id, List<InventoryViewModel> inventories)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<InventoryViewModel> items = new List<InventoryViewModel>();
                foreach (var inv in inventories)
                {
                    InventoryViewModel item = new InventoryViewModel();
                    item.warehouse_id = inv.warehouse_id;
                    item.invoice_number = inv.invoice_number;
                    item.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now.Date) : inv.invoice_date;
                    items.Add(item);
                }

                var duplicateWarehouses = items.GroupBy(x => x.warehouse_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (duplicateWarehouses.Any())
                {
                    foreach (var warehouse in duplicateWarehouses)
                    {
                        var warehouseItems = items.Where(x => string.Compare(x.warehouse_id, warehouse) == 0).ToList();
                        //get duplicate invoice
                        var dupInoviceDates = warehouseItems.GroupBy(x => x.invoice_date).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (dupInoviceDates.Any())
                        {
                            foreach (var dIVD in dupInoviceDates)
                            {
                                var obj = warehouseItems.Where(x => dIVD == x.invoice_date && !string.IsNullOrEmpty(x.invoice_number)).FirstOrDefault();
                                if (obj != null)
                                {
                                    tb_invoice invoice = new tb_invoice();
                                    invoice.invoice_id = Guid.NewGuid().ToString();
                                    invoice.warehouse_id = warehouse;
                                    invoice.invoice_date = dIVD;
                                    invoice.invoice_number = obj.invoice_number;
                                    invoice.ref_id = id;
                                    db.tb_invoice.Add(invoice);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    tb_invoice invoice = new tb_invoice();
                                    invoice.invoice_id = Guid.NewGuid().ToString();
                                    invoice.warehouse_id = warehouse;
                                    invoice.invoice_date = dIVD;
                                    invoice.invoice_number = GenerateInvoiceNumber(warehouse, dIVD);
                                    invoice.ref_id = id;
                                    db.tb_invoice.Add(invoice);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var item in warehouseItems)
                            {
                                bool isDupInvoiceDate = dupInoviceDates.Where(x => x == item.invoice_date).Count() > 0 ? true : false;
                                if (!isDupInvoiceDate)
                                {
                                    tb_invoice invoice = new tb_invoice();
                                    invoice.invoice_id = Guid.NewGuid().ToString();
                                    invoice.warehouse_id = item.warehouse_id;
                                    invoice.invoice_date = item.invoice_date;
                                    if (string.IsNullOrEmpty(item.invoice_number))
                                        invoice.invoice_number = GenerateInvoiceNumber(item.warehouse_id, item.invoice_date);
                                    else
                                        invoice.invoice_number = item.invoice_number;
                                    invoice.ref_id = id;
                                    db.tb_invoice.Add(invoice);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            //if don't have duplicate invoice date
                            foreach (var item in warehouseItems)
                            {
                                tb_invoice invoice = new tb_invoice();
                                invoice.invoice_id = Guid.NewGuid().ToString();
                                invoice.warehouse_id = item.warehouse_id;
                                invoice.invoice_date = item.invoice_date;
                                if (string.IsNullOrEmpty(item.invoice_number))
                                    invoice.invoice_number = GenerateInvoiceNumber(item.warehouse_id, item.invoice_date);
                                else
                                    invoice.invoice_number = item.invoice_number;
                                invoice.ref_id = id;
                                db.tb_invoice.Add(invoice);
                                db.SaveChanges();
                            }
                        }
                    }
                    foreach (var item in items)
                    {
                        bool isDupWarehouse = duplicateWarehouses.Where(x => string.Compare(x, item.warehouse_id) == 0).Count() > 0 ? true : false;
                        if (!isDupWarehouse)
                        {
                            tb_invoice invoice = new tb_invoice();
                            invoice.invoice_id = Guid.NewGuid().ToString();
                            invoice.warehouse_id = item.warehouse_id;
                            invoice.invoice_date = item.invoice_date;
                            if (string.IsNullOrEmpty(item.invoice_number))
                                invoice.invoice_number = GenerateInvoiceNumber(item.warehouse_id, item.invoice_date);
                            else
                                invoice.invoice_number = item.invoice_number;
                            invoice.ref_id = id;
                            db.tb_invoice.Add(invoice);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    //if don't have warehouse duplicate
                    foreach (var item in items)
                    {
                        tb_invoice invoice = new tb_invoice();
                        invoice.invoice_id = Guid.NewGuid().ToString();
                        invoice.warehouse_id = item.warehouse_id;
                        invoice.invoice_date = item.invoice_date;
                        if (string.IsNullOrEmpty(item.invoice_number))
                            invoice.invoice_number = GenerateInvoiceNumber(item.warehouse_id, item.invoice_date);
                        else
                            invoice.invoice_number = item.invoice_number;
                        invoice.ref_id = id;
                        db.tb_invoice.Add(invoice);
                        db.SaveChanges();
                    }
                }
            }
        }
        public static string GenerateInvoiceNumber(string warehouseId, DateTime? invoiceDate)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string invoiceNumber = string.Empty;
                DateTime? date = invoiceDate == null ? DateTime.Now : invoiceDate;
                string YY = date.Value.Year.ToString().Substring(2, 2);
                string MM = date.Value.Month.ToString().Count() == 1 ? "0" + date.Value.Month.ToString() : date.Value.Month.ToString();
                string invoiceCompare = string.Format("IVN{0}{1}", YY, MM);
                string lastInvoiceNumber = db.tb_invoice.OrderByDescending(x => x.invoice_date).Where(x => x.invoice_number.Contains(invoiceCompare) && string.Compare(x.warehouse_id, warehouseId) == 0).Select(x => x.invoice_number).FirstOrDefault();
                int lastDigit = lastInvoiceNumber == null ? 1 : Convert.ToInt32(lastInvoiceNumber.Substring(lastInvoiceNumber.Count() - 3, 3)) + 1;
                string lastSplit = lastDigit.ToString().Length == 1 ? string.Format("00{0}", lastDigit.ToString()) : lastDigit.ToString().Length == 2 ? string.Format("0{0}", lastDigit.ToString()) : lastDigit.ToString();
                invoiceNumber = string.Format("IVN{0}{1}{2}", YY, MM, lastSplit);
                return invoiceNumber;
            }
        }
        public static string GetInvoiceNumber(string id, string warehouseId, DateTime? invoiceDate)
        {
            string invoiceNumber = string.Empty;
            string invo = "";
            using (kim_mexEntities db = new kim_mexEntities())
            {
               // invoiceNumber = db.tb_invoice.Where(x => string.Compare(x.ref_id, id) == 0 && string.Compare(x.warehouse_id, warehouseId) == 0 && x.invoice_date == invoiceDate).Select(x => x.invoice_number).FirstOrDefault() != null ? db.tb_invoice.Where(x => string.Compare(x.ref_id, id) == 0 && string.Compare(x.warehouse_id, warehouseId) == 0 && x.invoice_date == invoiceDate).Select(x => x.invoice_number).FirstOrDefault().ToString() : string.Empty;
                //invoiceNumber = db.tb_invoice.Where(x => string.Compare(x.ref_id, id) == 0 && string.Compare(x.warehouse_id, warehouseId) == 0 ).Select(x => x.invoice_number).FirstOrDefault() != null ? db.tb_invoice.Where(x => string.Compare(x.ref_id, id) == 0 && string.Compare(x.warehouse_id, warehouseId) == 0 ).Select(x => x.invoice_number).FirstOrDefault().ToString() : string.Empty;
                invo = (from a in db.tb_invoice where (a.ref_id == id && a.warehouse_id == warehouseId) select a.invoice_number).FirstOrDefault();
            }
            return invo;
        }
        public static void DeleteInvoiceNumber(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var objs = db.tb_invoice.Where(x => string.Compare(x.ref_id, id) == 0).Select(x => x.invoice_id).ToList();
                foreach (var obj in objs)
                {
                    tb_invoice invoice = db.tb_invoice.Find(obj);
                    db.tb_invoice.Remove(invoice);
                    db.SaveChanges();
                }
            }
        }
        public static string GetPurchaseOrderNumber(string id, string supplierId, bool isVAT)
        {
            string purchaseOrderNumber = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var purchaseOrderReports = db.tb_po_report.Where(x => string.Compare(x.po_ref_id, id) == 0 && string.Compare(x.po_supplier_id, supplierId) == 0 && x.vat_status == isVAT).Select(x => x.po_report_number).ToList();
                if (purchaseOrderReports.Any())
                    foreach (var poNumber in purchaseOrderReports)
                        purchaseOrderNumber = string.Format("{0} {1},", purchaseOrderNumber, poNumber);
            }
            return purchaseOrderNumber;
        }
        #region Quote Supplier
        public static List<QuoteViewModel> GetItemPricebySupplier(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<QuoteViewModel> itemQuotes = new List<QuoteViewModel>();
                var quotes = (from qd in db.tb_quote_detail
                              join q in db.tb_quote on qd.quote_id equals q.quote_id
                              join sup in db.tb_supplier on q.supplier_id equals sup.supplier_id
                              where string.Compare(qd.item_id, id) == 0 && q.status == true
                              select new QuoteViewModel()
                              {
                                  quote_id = q.quote_id,
                                  quote_no = q.quote_no,
                                  supplier_id = q.supplier_id,
                                  supplier_name = sup.supplier_name,
                                  price = qd.price,
                                  created_date = q.created_date,
                                  quote_detail_id = qd.quote_detail_id,
                                  itemQuoteAttachments = db.tb_item_quote_attachment.OrderBy(o => o.quote_attachment_name).Where(w => string.Compare(w.quote_attachment_ref_id, qd.quote_detail_id) == 0).Select(s => new QuoteItemAttachment()
                                  {
                                      quote_attachment_id = s.quote_attachment_id,
                                      quote_attachment_name = s.quote_attachment_name,
                                      quote_attachment_extension = s.quote_attachment_extension,
                                      quote_attachment_part = s.quote_attachment_part,
                                  }).ToList(),
                              }).ToList();
                var dupSuppliers = quotes.GroupBy(g => g.supplier_id).Where(c => c.Count() > 1).Select(s => s.Key).ToList();
                if (dupSuppliers.Any())
                {
                    foreach (var dupSupplier in dupSuppliers)
                    {
                        var quote = quotes.OrderByDescending(q => q.created_date).Where(s => string.Compare(s.supplier_id, dupSupplier) == 0).FirstOrDefault();
                        itemQuotes.Add(new QuoteViewModel()
                        {
                            quote_id = quote.quote_id,
                            quote_no = quote.quote_no,
                            supplier_id = quote.supplier_id,
                            supplier_name = quote.supplier_name,
                            price = quote.price,
                            itemQuoteAttachments = quote.itemQuoteAttachments,
                        });
                    }
                    foreach (var item in quotes)
                    {
                        bool isDuplication = dupSuppliers.Where(s => string.Compare(s, item.supplier_id) == 0).Count() > 0 ? true : false;
                        if (!isDuplication)
                            itemQuotes.Add(new QuoteViewModel()
                            {
                                quote_id = item.quote_id,
                                quote_no = item.quote_no,
                                supplier_id = item.supplier_id,
                                supplier_name = item.supplier_name,
                                price = item.price,
                                itemQuoteAttachments = item.itemQuoteAttachments,
                            });
                    }
                }
                else
                {
                    foreach (var item in quotes)
                    {
                        itemQuotes.Add(new QuoteViewModel()
                        {
                            quote_id = item.quote_id,
                            quote_no = item.quote_no,
                            supplier_id = item.supplier_id,
                            supplier_name = item.supplier_name,
                            price = item.price,
                            itemQuoteAttachments = item.itemQuoteAttachments,
                        });
                    }
                }
                itemQuotes = itemQuotes.OrderBy(q => q.quote_no).ToList();
                return itemQuotes;
            }
        }
        public static List<QuoteItemAttachment> GetQuoteItemAttachments(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<QuoteItemAttachment> attachments = new List<QuoteItemAttachment>();
                attachments = db.tb_item_quote_attachment.OrderBy(o => o.quote_attachment_name).Where(w => string.Compare(w.quote_attachment_ref_id, id) == 0)
                    .Select(s => new QuoteItemAttachment()
                    {
                        quote_attachment_id = s.quote_attachment_id,
                        quote_attachment_name = s.quote_attachment_name,
                        quote_attachment_extension = s.quote_attachment_extension,
                        quote_attachment_part = s.quote_attachment_part,
                    }).ToList();
                return attachments;
            }
        }
        public static string GenerateQuoteNumber()
        {
            string quoteNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", quoteNum;
                string number = (from tbl in db.tb_quote orderby tbl.created_date descending select tbl.quote_no).FirstOrDefault();
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
                string yy = (DateTime.Now.Year).ToString().Substring(2, 2);
                string mm = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                quoteNo = "QU-" + yy + "-" + mm + "-" + last_no;
            }
            return quoteNo;
        }
        public static QuoteViewModel GetQuoteNumberSupplier(string supplierName)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                QuoteViewModel quote = new QuoteViewModel();
                quote.supplier_id = db.tb_supplier.Where(w => w.status == true && string.Compare(w.supplier_name, supplierName) == 0).Select(s => s.supplier_id).FirstOrDefault();
                var quoteItem = db.tb_quote.OrderByDescending(o => o.created_date).Where(w => w.status == true && string.Compare(w.supplier_id, quote.supplier_id) == 0).Select(s => new { s }).FirstOrDefault();
                if (quoteItem == null)
                    quote.quote_no = GenerateQuoteNumber();
                else
                {
                    quote.quote_id = quoteItem.s.quote_id.ToString();
                    quote.quote_no = quoteItem.s.quote_no.ToString();
                }
                return quote;
            }
        }
        #endregion
        public static DateTime ToLocalTime(DateTime utcDate)
        {
            var localTimeZoneId = "SE Asia Standard Time";
            //var localTimeZoneId = "Pacific Standard Time";
            //var localTimeZoneId = "Tokyo Standard Time";
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZoneId);
            var localTime = TimeZoneInfo.ConvertTime(utcDate, localTimeZone);
            return localTime;
        }
        public static string GenerateProcessNumber(string type)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string processNumber = string.Empty;
                string yy = CommonClass.ToLocalTime(DateTime.Now).Year.ToString().Substring(2, 2);
                string mm = CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? string.Format("0{0}", CommonClass.ToLocalTime(DateTime.Now).Month.ToString()) : CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                string strCompare = string.Format("{0}-{1}-{2}-", type, yy, mm);
                int numLast = 0;
                var lastRecord = string.Empty;

                switch (type)
                {
                    case "MR":
                        lastRecord = db.tb_item_request.OrderByDescending(o => o.ir_no).Where(w => w.ir_no.Contains(strCompare) && w.status == true).Select(s => s.ir_no).FirstOrDefault();
                        break;
                    case "PR":
                        lastRecord = db.tb_purchase_requisition.OrderByDescending(o => o.created_at).Where(w => w.purchase_requisition_number.Contains(strCompare) && w.status == true).Select(s => s.purchase_requisition_number).FirstOrDefault();
                        break;
                    //case "POL":
                    case "QN":
                        lastRecord = db.tb_purchase_order.OrderByDescending(o => o.created_date).Where(w => w.purchase_oder_number.Contains(strCompare) && w.status == true).Select(s => s.purchase_oder_number).FirstOrDefault();
                        break;
                    case "IRe":
                        lastRecord = db.tb_receive_item_voucher.OrderByDescending(o => o.created_date).Where(w => w.received_number.Contains(strCompare) && w.status == true).Select(s => s.received_number).FirstOrDefault();
                        break;
                    case "IRT":
                        lastRecord = db.tb_item_return.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.item_return_number.Contains(strCompare)).Select(s => s.item_return_number).FirstOrDefault();
                        break;
                    case "ST":
                        lastRecord = db.tb_stock_transfer_voucher.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.stock_transfer_no.Contains(strCompare)).Select(s => s.stock_transfer_no).FirstOrDefault();
                        break;
                    case "SD":
                        lastRecord = db.tb_stock_damage.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.stock_damage_number.Contains(strCompare)).Select(s => s.stock_damage_number).FirstOrDefault();
                        break;
                    case "SI":
                    case "WOI":
                        lastRecord = db.tb_stock_issue.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.stock_issue_number.Contains(strCompare)).Select(s => s.stock_issue_number).FirstOrDefault();
                        break;
                    case "SIR":
                    case "STR":
                        lastRecord = db.tb_stock_issue_return.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.issue_return_number.Contains(strCompare)).Select(s => s.issue_return_number).FirstOrDefault();
                        break;
                    case "SA":
                        lastRecord = db.tb_stock_adjustment.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.stock_adjuctment_code.Contains(strCompare)).Select(s => s.stock_adjuctment_code).FirstOrDefault();
                        break;
                    case "TW":
                        lastRecord = db.transferformmainstocks.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.stock_transfer_no.Contains(strCompare)).Select(s => s.stock_transfer_no).FirstOrDefault();
                        break;
                    case "RW":
                        lastRecord = db.tb_return_main_stock.OrderByDescending(o => o.create_date).Where(w => w.status == true && w.return_main_stock_no.Contains(strCompare)).Select(s => s.return_main_stock_no).FirstOrDefault();
                        break;
                    case "WOR":
                        lastRecord = db.tb_workorder_returned.OrderByDescending(o => o.created_date).Where(w => w.status == true && w.workorder_returned_number.Contains(strCompare)).Select(s => s.workorder_returned_number).FirstOrDefault();
                        break;
                    case "MCO":
                        lastRecord = db.tb_mr_cut_off.OrderByDescending(o => o.created_at).Where(w => w.active == true && w.mr_cut_off_number.Contains(strCompare)).Select(s => s.mr_cut_off_number).FirstOrDefault();
                        break;
                    case "IBQ":
                        lastRecord = db.tb_item_blocking.OrderByDescending(o => o.created_at).Where(w => w.active == true && w.item_blocking_number.Contains(strCompare)).Select(s => s.item_blocking_number).FirstOrDefault();
                        break;
                    default:
                        break;
                }

                if (lastRecord == null || string.IsNullOrEmpty(lastRecord))
                    numLast = 1;
                else
                {
                    string strLastNumber = lastRecord.ToString();
                    string[] splitNumber = strLastNumber.Split('-');
                    numLast = Convert.ToInt32(splitNumber[splitNumber.Length - 1]) + 1;
                }
                string strPR = numLast.ToString().Length == 1 ? string.Format("00{0}", numLast.ToString()) : numLast.ToString().Length == 2 ? string.Format("0{0}", numLast.ToString()) : numLast.ToString();
                processNumber = string.Format("{0}-{1}-{2}-{3}", type, yy, mm, strPR);
                return processNumber;
            }
        }
        public static int GetItemUnitLevel(string itemId, string unit)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int level = 0;
                unit = Regex.Replace(unit, @"\t|\n|\r", "");
                string defaultUnit = Regex.Replace(db.tb_product.Where(w => string.Compare(itemId, w.product_id) == 0).Select(s => s.product_unit).FirstOrDefault().ToString(), @"\t|\n|\r", "");
                if (string.Compare(unit, defaultUnit) == 0)
                {
                    level = 0;
                }
                else
                {
                    var itemUOM = db.tb_multiple_uom.Where(w => string.Compare(itemId, w.product_id) == 0).FirstOrDefault();
                    if (itemUOM != null)
                    {
                        if (string.Compare(unit, Regex.Replace(itemUOM.uom1_id, @"\t|\n|\r", "")) == 0)
                            level = 1;
                        else if (string.Compare(unit, Regex.Replace(itemUOM.uom2_id, @"\t|\n|\r", "")) == 0)
                            level = 2;
                        else if (string.Compare(unit, Regex.Replace(itemUOM.uom3_id, @"\t|\n|\r", "")) == 0)
                            level = 3;
                        else if (string.Compare(unit, Regex.Replace(itemUOM.uom4_id, @"\t|\n|\r", "")) == 0)
                            level = 4;
                        else if (string.Compare(unit, Regex.Replace(itemUOM.uom2_id, @"\t|\n|\r", "")) == 0)
                            level = 5;
                    }
                }
                return level;
            }

        }
        public static string warehouseName(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var getWarehouse = db.tb_warehouse.Where(x => x.warehouse_id == id).FirstOrDefault();
                return getWarehouse == null ? "" : getWarehouse.warehouse_name;
            }
        }
        public static string projectName(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var getproject = db.tb_project.Where(x => x.project_id == id).FirstOrDefault();
                return getproject == null ? "" : getproject.project_full_name;
            }
        }

        public static string GetWarehouseIDbyPurchaseRequisition(string puchaseRequisitionID)
        {
            string warehouseId = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                warehouseId = (from pr in db.tb_item_request
                               join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                               //join site in db.tb_site on pro.site_id equals site.site_id
                               //join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                               join warehouse in db.tb_warehouse on pro.project_id equals warehouse.warehouse_project_id into pwh from warehouse in pwh.DefaultIfEmpty()
                               where string.Compare(pr.ir_id, puchaseRequisitionID) == 0
                               select warehouse.warehouse_id).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetWarehouseIDbyPurchaseRequisition", ex.StackTrace, ex.Message);
            }
            return warehouseId;
        }
        public static string GetWareshouseIdbyPurchaseOrder(string purchaseOrderID)
        {
            string warehouseId = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                warehouseId = (from po in db.tb_purchase_order
                               join pr in db.tb_item_request on po.item_request_id equals pr.ir_id
                               join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                               join site in db.tb_site on pro.site_id equals site.site_id
                               join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                               where string.Compare(po.purchase_order_id, purchaseOrderID) == 0
                               select warehouse.warehouse_id).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetWareshouseIdbyPurchaseOrder", ex.StackTrace, ex.Message);
            }
            return warehouseId;
        }

        public static string GetWarehouseIdbyStockTransfer(string stockTransferID)
        {
            string warehouseID = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                warehouseID = (from st in db.tb_stock_transfer_voucher
                               join pr in db.tb_item_request on st.item_request_id equals pr.ir_id
                               join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                               join site in db.tb_site on pro.site_id equals site.site_id
                               join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                               where string.Compare(st.stock_transfer_id, stockTransferID) == 0
                               select warehouse.warehouse_id).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetWarehouseIdbyStockTransfer", ex.StackTrace, ex.Message);
            }
            return warehouseID;
        }//Rathana Add 10.04.2019

        public static string GetWarehouseIdbyTransferFromMainStock(string transferFromMainStockID)
        {
            string warehouseID = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                warehouseID = (from st in db.transferformmainstocks
                               join pr in db.tb_item_request on st.item_request_id equals pr.ir_id
                               join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                               join site in db.tb_site on pro.site_id equals site.site_id
                               join warehouse in db.tb_warehouse on site.site_id equals warehouse.warehouse_site_id
                               where string.Compare(st.stock_transfer_id, transferFromMainStockID) == 0
                               select warehouse.warehouse_id).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetWarehouseIdbytransferFromMainStock", ex.StackTrace, ex.Message);
            }
            return warehouseID;
        }


        public static string GeneratePOLNumberWithProjectShortNameByID(string projectID)
        {
            if (!string.IsNullOrEmpty(projectID))
            {
                string POL = GenerateProcessNumber("POL");
                string ProjectShortName = db.tb_project.FirstOrDefault(x => x.project_id == projectID).project_short_name;

                var splitPOL = POL.Split('-');
                return $"{splitPOL[0]}-{ProjectShortName}-{splitPOL[1]}-{splitPOL[2]}-{splitPOL[3]}";
            }
            return "";
        }

        public static string ConvertUserDetailIDToName(string id)
        {
            using (var db = new Entities.kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return db.tb_user_detail.Where(x => x.user_detail_id == id).Select(x => x.user_first_name + " " + x.user_last_name).FirstOrDefault();
                }
                else
                {
                    return "";
                }
            }
        }
        public static string GetUserFullnameByUserId(string id)
        {
            using (var db = new Entities.kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return db.tb_user_detail.Where(x => x.user_id == id).Select(x => x.user_first_name + " " + x.user_last_name).FirstOrDefault();
                }
                else
                {
                    return "";
                }
            }
        }
        //End Rathan Add    


        //Seakly 

        public static string ConvertWarehouseName(string data)
        {
            string warehouseId = Class.CommonClass.GetWarehouseIDbyPurchaseRequisition(data);
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var getWarehouse = db.tb_warehouse.Where(x => x.warehouse_id == warehouseId).FirstOrDefault();
                return getWarehouse == null ? "" : getWarehouse.warehouse_name;
            }
        }
        //seakly
        //public static List<Models.PurchaseOrderSupplier> GetPurchaseOrderbySupplier(string id,string poID=null,string poReportNumber=null)
        //{
        //    List<Models.PurchaseOrderSupplier> purchaseOrders = new List<PurchaseOrderSupplier>();
        //    try
        //    {
        //        kim_mexEntities db = new kim_mexEntities();
        //        purchaseOrders = (from po in db.tb_po_report
        //                          join poo in db.tb_purchase_order on po.po_ref_id equals poo.purchase_order_id
        //                          join pr in db.tb_item_request on poo.item_request_id equals pr.ir_id
        //                          join pro in db.tb_project on pr.ir_project_id equals pro.project_id
        //                          join site in db.tb_site on pro.site_id equals site.site_id
        //                          join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
        //                          orderby po.created_date descending
        //                          where string.Compare(po.po_supplier_id, id) == 0 && po.is_completed == false && (string.Compare(poo.purchase_order_status,"Completed")==0 || string.Compare(poo.purchase_order_status,"Approved")==0)
        //                          select new PurchaseOrderSupplier()
        //                          {
        //                              purchaseOrderId=po.po_ref_id,
        //                              purchaseOrderNumber=po.po_report_number,
        //                              poSupplierId=po.po_supplier_id,
        //                              warehouseId=wh.warehouse_id,
        //                              created_date=po.created_date,
        //                              //isVAT=Convert.ToBoolean(po.vat_status),
        //                              itemRequestNumber=pr.ir_no,
        //                          }).ToList();

        //        if(!string.IsNullOrEmpty(poID) && !string.IsNullOrEmpty(poReportNumber))
        //        {
        //            bool isExist=Convert.ToBoolean(purchaseOrders.Where(x=>string.Compare(x.purchaseOrderId,poID)==0 && string.Compare(x.purchaseOrderNumber,poReportNumber)==0).FirstOrDefault());
        //            if (!isExist)
        //            {
        //                var po = (from poo in db.tb_purchase_order
        //                          join pr in db.tb_item_request on poo.item_request_id equals pr.ir_id
        //                          join pro in db.tb_project on pr.ir_project_id equals pro.project_id
        //                          join site in db.tb_site on pro.site_id equals site.site_id
        //                          join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
        //                          where string.Compare(poo.purchase_order_id, poID) == 0
        //                          select new { poo, wh,pr }).FirstOrDefault();
        //                purchaseOrders.Add(new PurchaseOrderSupplier() { purchaseOrderId = poID, purchaseOrderNumber = poReportNumber, poSupplierId = id ,warehouseId=po.wh.warehouse_id,created_date=po.poo.created_date,itemRequestNumber=po.pr.ir_no});
        //            }
        //        }
        //        purchaseOrders = purchaseOrders.OrderByDescending(o => o.created_date).ToList();

        //    }catch(Exception ex)
        //    {
        //        ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetPurchaseOrderbySupplier", ex.StackTrace, ex.Message);
        //    }
        //    return purchaseOrders;
        //}
        //added by TTerd Apr 05 2020
        public static List<Models.PurchaseOrderSupplier> GetPurchaseOrderbySupplier(string id, string poID = null, string poReportNumber = null)
        {
            List<Models.PurchaseOrderSupplier> purchaseOrders = new List<PurchaseOrderSupplier>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                //purchaseOrders = (from po in db.tb_po_report
                //                  join poo in db.tb_purchase_order on po.po_ref_id equals poo.purchase_order_id
                //                  join pr in db.tb_item_request on poo.item_request_id equals pr.ir_id
                //                  join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                //                  join site in db.tb_site on pro.site_id equals site.site_id
                //                  join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                //                  orderby po.created_date descending
                //                  where string.Compare(po.po_supplier_id, id) == 0 && po.is_completed == false && (string.Compare(poo.purchase_order_status, "Completed") == 0 || string.Compare(poo.purchase_order_status, "Approved") == 0)
                //                  select new PurchaseOrderSupplier()
                //                  {
                //                      purchaseOrderId = po.po_ref_id,
                //                      purchaseOrderNumber = po.po_report_number,
                //                      poSupplierId = po.po_supplier_id,
                //                      warehouseId = wh.warehouse_id,
                //                      created_date = po.created_date,
                //                      //isVAT=Convert.ToBoolean(po.vat_status),
                //                      itemRequestNumber = pr.ir_no,
                //                  }).ToList();

                if(string.Compare(id,"0")==0)
                {
                    purchaseOrders = (from pod in db.tb_purchase_request_detail
                                      join po in db.tb_purchase_request on pod.purchase_request_id equals po.pruchase_request_id
                                      join por in db.tb_po_report on pod.po_report_id equals por.po_report_id
                                      join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                      join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                      join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                      join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                      orderby por.created_date descending
                                      where por.is_completed == false
                                      //&& (string.Compare(po.purchase_request_status, Status.Approved) == 0 || string.Compare(po.purchase_request_status, Status.Completed) == 0)
                                      && string.Compare(pod.status,Status.Approved)==0
                                      select new PurchaseOrderSupplier()
                                      {
                                          purchaseOrderId = pod.purchase_request_id,
                                          purchaseOrderNumber = por.po_report_number,
                                          poSupplierId = por.po_supplier_id,
                                          warehouseId = wh.warehouse_id,
                                          created_date = po.created_date,
                                          itemRequestNumber = pr.purchase_requisition_number,
                                          poReportNumber = por.po_report_id,
                                      }).ToList();
                }
                else
                {
                    purchaseOrders = (from pod in db.tb_purchase_request_detail
                                      join po in db.tb_purchase_request on pod.purchase_request_id equals po.pruchase_request_id
                                      join por in db.tb_po_report on pod.po_report_id equals por.po_report_id
                                      join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                      join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                      join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                      join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                      orderby por.created_date descending
                                      where string.Compare(por.po_supplier_id, id) == 0 && por.is_completed == false
                                      //&& (string.Compare(po.purchase_request_status, Status.Approved) == 0 || string.Compare(po.purchase_request_status, Status.Completed) == 0)
                                      && string.Compare(pod.status, Status.Approved) == 0
                                      select new PurchaseOrderSupplier()
                                      {
                                          purchaseOrderId = pod.purchase_request_id,
                                          purchaseOrderNumber = por.po_report_number,
                                          poSupplierId = por.po_supplier_id,
                                          warehouseId = wh.warehouse_id,
                                          created_date = po.created_date,
                                          itemRequestNumber = pr.purchase_requisition_number,
                                          poReportNumber = por.po_report_id,
                                      }).ToList();
                }
                

                if (!string.IsNullOrEmpty(poID) && !string.IsNullOrEmpty(poReportNumber))
                {
                    bool isExist = Convert.ToBoolean(purchaseOrders.Where(x => string.Compare(x.purchaseOrderId, poID) == 0 && string.Compare(x.purchaseOrderNumber, poReportNumber) == 0).FirstOrDefault());
                    if (!isExist)
                    {
                        //var po = (from poo in db.tb_purchase_order
                        //          join pr in db.tb_item_request on poo.item_request_id equals pr.ir_id
                        //          join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                        //          join site in db.tb_site on pro.site_id equals site.site_id
                        //          join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                        //          where string.Compare(poo.purchase_order_id, poID) == 0
                        //          select new { poo, wh, pr }).FirstOrDefault();

                        var po = (from pod in db.tb_purchase_request_detail
                                  join poo in db.tb_purchase_request on pod.purchase_request_id equals poo.pruchase_request_id
                                  join por in db.tb_po_report on pod.po_report_id equals por.po_report_id
                                  join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                  join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                  join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                  join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                  join site in db.tb_site on pro.site_id equals site.site_id
                                  join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                  where string.Compare(pod.purchase_request_id, poID) == 0
                                  select new { poo, wh, pr,mr,por }).FirstOrDefault();

                        purchaseOrders.Add(new PurchaseOrderSupplier() {
                            purchaseOrderId = poID,
                            purchaseOrderNumber = poReportNumber,
                            poSupplierId = id, warehouseId = po.wh.warehouse_id,
                            created_date = po.poo.created_date,
                            itemRequestNumber = po.mr.ir_no,
                            poReportNumber=po.por.po_report_id,
                        });
                    }
                }
                purchaseOrders = purchaseOrders.OrderByDescending(o => o.created_date).ToList();

            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "CommonClass.cs", "GetPurchaseOrderbySupplier", ex.StackTrace, ex.Message);
            }
            return purchaseOrders;
        }
        public static void UpdateMaterialRequestStatus(string materialId,string status,string createdBy)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_request materialReqeust = db.tb_item_request.Find(materialId);
                materialReqeust.po_status = status;
                materialReqeust.st_status = status;
                db.SaveChanges();

                tb_mr_related_status mrStatus = new tb_mr_related_status();
                mrStatus.mr_id = materialReqeust.ir_id;
                mrStatus.po_status = status;
                mrStatus.st_status = status;
                mrStatus.active = true;
                mrStatus.created_at = CommonClass.ToLocalTime(DateTime.Now);
                mrStatus.created_by = createdBy;
                MRRelatedStatusModel.SaveMRRelatedStatus(mrStatus);
            }
        }
        public static void SubmitProcessWorkflow(int menuId,string refId,string status,string createdBy,DateTime? createdAt,string remark="")
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_procress_workflow pwf = new tb_procress_workflow();
                pwf.menu_id = menuId;
                pwf.ref_id = refId;
                pwf.status = status;
                pwf.created_at = createdAt;
                pwf.created_by = createdBy;
                pwf.remark = remark;
                db.tb_procress_workflow.Add(pwf);
                db.SaveChanges();
            }
        }
        public static int GetSytemMenuIdbyControllerName(string name)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_system_menu.Where(s => string.Compare(s.path, name) == 0 && s.keepAlive == true).Select(s => s.id).FirstOrDefault();
            }
        }
        public static List<tb_purpose> GetPurposeListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_purpose.OrderBy(s => s.purpose_name).ToList();
            }
        }
        public static bool isFileIsImageByExtenstion(string postedFileExtension)
        {
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
    }
}