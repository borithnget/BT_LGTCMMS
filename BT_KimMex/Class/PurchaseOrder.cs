using BT_KimMex.Entities;
using BT_KimMex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Class
{
    public class PurchaseOrder
    {
        public void InitialPurchaseOrderPONumber(string id)
        {
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                Models.PurchaseOrderViewModel purchaseOrder = new PurchaseOrderViewModel();
                List<ItemSupplier> purchaseOrderDetails = new List<ItemSupplier>();
                purchaseOrder = this.GetPOSupplierItem(id);
                purchaseOrderDetails = this.GetPurchaseOrderSupplierItem(id);
                foreach(var supplier in purchaseOrder.poSuppliers)
                {
                    var sup = db.tb_supplier.Find(supplier.supplier_id);
                    int countVATItem = 0;
                    int countNonVATItem = 0;
                    var items = purchaseOrderDetails.Where(x => string.Compare(x.supplierId, supplier.supplier_id) == 0).ToList();
                    if (items.Any())
                    {
                        foreach(var item in items)
                        {
                            if (item.isVAT) countVATItem++;
                            else countNonVATItem++;
                        }
                    }
                    //if (countVATItem > 0) SaveVATPONumber(id, supplier.supplier_id);
                    //if (countNonVATItem > 0) SaveNonVATPONumber(id, supplier.supplier_id);
                    if (countVATItem > 0) GeneratePOReportNumber(id, supplier.supplier_id,true,Convert.ToBoolean(sup.is_local));
                    if (countNonVATItem > 0) GeneratePOReportNumber(id, supplier.supplier_id, false, Convert.ToBoolean(sup.is_local));
                }
            }
        }
        public PurchaseOrderViewModel GetPOSupplierItem(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            List<PurchaseOrderItemSupplier> poSuppliers = new List<PurchaseOrderItemSupplier>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                model = (from po in db.tb_purchase_order
                         join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                         join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                         join proj in db.tb_project on ir.ir_project_id equals proj.project_id
                         where po.purchase_order_id == id
                         select new PurchaseOrderViewModel() { 
                             purchase_order_id = po.purchase_order_id, 
                             item_request_id = po.item_request_id, 
                             ir_no = pr.purchase_requisition_number, project_full_name = proj.project_full_name, purchase_oder_number = po.purchase_oder_number, purchase_order_status = po.purchase_order_status, created_date = po.created_date }).FirstOrDefault();
                if (model != null)
                {
                    if (string.Compare(model.purchase_order_status, "Completed") == 0 || string.Compare(model.purchase_order_status, Status.Checked) == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "approved"
                                     select new PurchaseOrderDetailViewModel() { po_detail_id = dPO.po_detail_id, purchase_order_id = dPO.purchase_order_id, item_id = dPO.item_id, quantity = dPO.quantity, product_code = item.product_code, product_name = item.product_name, product_unit = item.product_unit, unit_price = dPO.unit_price, item_status = dPO.item_status, po_quantity = dPO.po_quantity }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() { 
                                        po_supplier_id = pos.po_supplier_id, 
                                        po_detail_id = pos.po_detail_id, 
                                        unit_price = pos.unit_price,
                                        supplier_id = sup.supplier_id, 
                                        supplier_name = sup.supplier_name, 
                                        sup_number = pos.sup_number, 
                                        is_selected = pos.is_selected, 
                                        vat = pos.vat, Reason = pos.Reason, po_quantity = pos.po_qty, supplier_address = sup.supplier_address, supplier_phone = sup.supplier_phone, supplier_email = sup.supplier_email }).FirstOrDefault();
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
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() { supplier_id = sup, supplier_name = supplier.supplier_name, supplier_email = supplier.supplier_email, supplier_address = supplier.supplier_address, supplier_phone = supplier.supplier_phone });
                            }

                        }
                    }
                    else if (string.Compare(model.purchase_order_status, "Approved") == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "Pending"
                                     select new PurchaseOrderDetailViewModel() { po_detail_id = dPO.po_detail_id, purchase_order_id = dPO.purchase_order_id, item_id = dPO.item_id, quantity = dPO.quantity, product_code = item.product_code, product_name = item.product_name, product_unit = item.product_unit, unit_price = dPO.unit_price, item_status = dPO.item_status, po_quantity = dPO.po_quantity }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() { po_supplier_id = pos.po_supplier_id, po_detail_id = pos.po_detail_id, unit_price = pos.unit_price, supplier_id = sup.supplier_id, supplier_name = sup.supplier_name, sup_number = pos.sup_number, is_selected = pos.is_selected, Reason = pos.Reason, po_quantity = pos.po_qty, supplier_address = sup.supplier_address, supplier_phone = sup.supplier_phone, supplier_email = sup.supplier_email }).FirstOrDefault();
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
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() { supplier_id = sup, supplier_name = supplier.supplier_name, supplier_email = supplier.supplier_email, supplier_address = supplier.supplier_address, supplier_phone = supplier.supplier_phone });
                            }

                        }
                    }

                }
            }
            return model;
        }
        public List<ItemSupplier> GetPurchaseOrderSupplierItem(string id)
        {
            List<ItemSupplier> models = new List<ItemSupplier>();
            using (kim_mexEntities db=new kim_mexEntities())
            {
                string purchaseOrderStatus = db.tb_purchase_order.Where(x => string.Compare(x.purchase_order_id, id) == 0).Select(x => x.purchase_order_status).FirstOrDefault();
                IQueryable<PurchaseOrderDetailViewModel> purchaseOrderItemDetails;
                if (string.Compare(purchaseOrderStatus, "Approved") == 0 )
                {
                    purchaseOrderItemDetails = db.tb_purchase_order_detail.Where(x => string.Compare(x.purchase_order_id, id) == 0 && x.status == true && string.Compare(x.item_status, "Pending") == 0).Select(x=>new PurchaseOrderDetailViewModel() { po_detail_id=x.po_detail_id,item_id=x.item_id });
                }
                else if(string.Compare(purchaseOrderStatus, "Completed") == 0 || string.Compare(purchaseOrderStatus, Status.Checked) == 0)
                {
                    purchaseOrderItemDetails = db.tb_purchase_order_detail.Where(x => string.Compare(x.purchase_order_id, id) == 0 && x.status == true && string.Compare(x.item_status, "approved") == 0).Select(x => new PurchaseOrderDetailViewModel() { po_detail_id = x.po_detail_id,item_id=x.item_id });
                }
                else
                {
                    purchaseOrderItemDetails = null;
                }
                if (purchaseOrderItemDetails.Any())
                {
                    foreach(var podetail in purchaseOrderItemDetails)
                    {
                        var itemSupplier = db.tb_po_supplier.Where(x => string.Compare(x.po_detail_id, podetail.po_detail_id) == 0 && x.is_selected == true).FirstOrDefault();
                        models.Add(new ItemSupplier() { itemId = podetail.item_id, supplierId = itemSupplier.supplier_id, isVAT = Convert.ToBoolean(itemSupplier.vat) });
                    }
                }
            }
            return models;
        }
        public void SaveVATPONumber(string purchaseOrderId,string supplierId)
        {
            using (kim_mexEntities db = new kim_mexEntities()) {
                string yy = (DateTime.Now.Year).ToString().Substring(2, 2);
                string mm = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                string poCompare = "PO " + yy + "-" + mm + "-";
                string poLastNumber = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare) && tbl.vat_status==true select tbl.po_report_number).FirstOrDefault();
                int lastDigit = poLastNumber == null ? 1: Convert.ToInt32(poLastNumber.Substring(poLastNumber.Count() - 3, 3)) + 1;
                string poReportNumber = string.Empty;
                string lastSplit = lastDigit.ToString().Length == 1 ? "00" + lastDigit : lastDigit.ToString().Length == 2 ? "0" + lastDigit : lastDigit.ToString();
                poReportNumber = "PO " + yy + "-" + mm + "-" + lastSplit;
                tb_po_report poReport = new tb_po_report();
                poReport.po_report_id = Guid.NewGuid().ToString();
                poReport.po_report_number = poReportNumber;
                poReport.po_ref_id =purchaseOrderId;
                poReport.po_supplier_id = supplierId;
                poReport.created_date = DateTime.Now;
                poReport.vat_status = true;
                db.tb_po_report.Add(poReport);
                db.SaveChanges();
            }
        }
        #region new process added by Terd Apr 30 2020
        public void GeneratePOReportNumber(string quoteId,string supplierId,bool isVAT,bool isLOP)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string yy = (DateTime.Now.Year).ToString().Substring(2, 2);
                string mm = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                string poLastNumber = (from tbl in db.tb_po_report
                                       orderby tbl.created_date descending
                                       where tbl.is_lpo.HasValue && tbl.is_lpo == isLOP && tbl.vat_status==isVAT
                                       select tbl.po_report_number).FirstOrDefault();
                int lastDigit = 0;
                if (poLastNumber == null)
                    lastDigit = 1;
                else
                {
                    string[] splitnumber = poLastNumber.Split('-');
                    lastDigit = Convert.ToInt32(splitnumber[1]) == 9999 ? 1 : Convert.ToInt32(splitnumber[1]) + 1;
                }
                string poReportNumber = string.Empty;
                string lastSplit = string.Empty;
                switch (lastDigit.ToString().Length)
                {
                    case 1:
                        lastSplit = "000" + lastDigit;
                        break;
                    case 2:
                        lastSplit = "00" + lastDigit;
                        break;
                    case 3:
                        lastSplit = "0" + lastDigit;
                        break;
                    default:
                        lastSplit = lastDigit.ToString();
                        break;
                }
                poReportNumber = isLOP ? isVAT? string.Format("LPO{0}{1}-{2}", yy, mm, lastSplit) : string.Format("LPOH{0}{1}-{2}", yy, mm, lastSplit) :isVAT? string.Format("LGT{0}{1}-{2}", yy, mm, lastSplit) : string.Format("OPOH{0}{1}-{2}", yy, mm, lastSplit);
                tb_po_report poReport = new tb_po_report();
                poReport.po_report_id = Guid.NewGuid().ToString();
                poReport.po_report_number = poReportNumber;
                poReport.po_ref_id = quoteId;
                poReport.po_supplier_id = supplierId;
                poReport.created_date = DateTime.Now;
                poReport.vat_status = isVAT;
                poReport.is_lpo = isLOP;
                db.tb_po_report.Add(poReport);
                db.SaveChanges();
            }
        }
        #endregion
        public void SaveNonVATPONumber(string purchaseOrderId,string supplierId)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string yy = (DateTime.Now.Year).ToString().Substring(2, 2);
                string mm = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                string poCompare = "PO/NT " + yy + "-" + mm + "-";
                string poLastNumber = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare) && tbl.vat_status == false select tbl.po_report_number).FirstOrDefault();
                int lastDigit = poLastNumber == null ? 1 : Convert.ToInt32(poLastNumber.Substring(poLastNumber.Count() - 3, 3)) + 1;
                string poReportNumber = string.Empty;
                string lastSplit = lastDigit.ToString().Length == 1 ? "00" + lastDigit : lastDigit.ToString().Length == 2 ? "0" + lastDigit : lastDigit.ToString();
                poReportNumber = "PO/NT " + yy + "-" + mm + "-" + lastSplit;
                tb_po_report poReport = new tb_po_report();
                poReport.po_report_id = Guid.NewGuid().ToString();
                poReport.po_report_number = poReportNumber;
                poReport.po_ref_id = purchaseOrderId;
                poReport.po_supplier_id = supplierId;
                poReport.created_date = DateTime.Now;
                poReport.vat_status = false;
                db.tb_po_report.Add(poReport);
                db.SaveChanges();
            }
        }
        public static List<Models.PurchaseOrderViewModel> GetPurchaseOrderDropdownList()
        {
            List<Models.PurchaseOrderViewModel> models = new List<Models.PurchaseOrderViewModel>();
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    /*
                    models = db.tb_purchase_order.OrderByDescending(m => m.created_date).Where(x => (string.Compare(x.purchase_order_status, "Completed") == 0 || string.Compare(x.purchase_order_status, "Approved") == 0) && x.status == true && x.is_completed == false).Select(x => new PurchaseOrderViewModel()
                    {
                        purchase_order_id = x.purchase_order_id,
                        purchase_oder_number = x.purchase_oder_number,
                        //warehouseID = Class.CommonClass.GetWarehouseIDbyPurchaseRequisition(x.item_request_id)
                    }).ToList();
                    */
                    var purchaseOrders = db.tb_purchase_order.OrderByDescending(m => m.created_date).Where(x => (string.Compare(x.purchase_order_status, "Completed") == 0 || string.Compare(x.purchase_order_status, "Approved") == 0) && x.status == true && x.is_completed == false).ToList();
                    foreach(var purchaseOrder in purchaseOrders)
                    {
                        PurchaseOrderViewModel model = new PurchaseOrderViewModel();
                        model.purchase_order_id = purchaseOrder.purchase_order_id;
                        model.purchase_oder_number = purchaseOrder.purchase_oder_number;
                        model.warehouseID = Class.CommonClass.GetWarehouseIDbyPurchaseRequisition(purchaseOrder.item_request_id);
                        models.Add(model);
                    }
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "PurchaseOrder.cs", "GetPurchaseOrderDropdownList", ex.StackTrace, ex.Message);
            }
            return models;
        }
    }
    public class ItemSupplier
    {
        public string itemId { get; set; }
        public string supplierId { get; set; }
        public bool isVAT { get; set; }
    }
}