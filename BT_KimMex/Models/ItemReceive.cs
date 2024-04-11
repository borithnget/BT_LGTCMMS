using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Class;

namespace BT_KimMex.Models
{
    public class ItemReceiveViewModel
    {
        [Key]
        public string receive_item_voucher_id { get; set; }
        [Display(Name = "IR NO.:")]
        public string received_number { get; set; }
        public string received_status { get; set; }
        public string received_type { get; set; }
        [Display(Name = "Ref No.:")]
        public string ref_id { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_date_text { get; set; }
        [Display(Name = "Ref No.:")]
        public string ref_number { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public List<InventoryViewModel> receivedItems { get; set; }
        public List<tb_ire_attachment> attachments { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public List<ItemReceiveHistoryViewModel> histories { get; set; }
        public List<ItemReceiveViewModel> receivedHistories { get; set; }
        public List<tb_attachment> doAttachments { get; set; }
        public string supplier { get; set; }
        public string invoiceDate { get; set; }
        public string invoiceNumber { get; set; }
        public string supplier_id { get; set; }
        public string po_report_number { get; set; }
        public string history_qlty { get; set; }
        public string remain_qlty { get; set; }
        public string received_qlty { get; set; }
        public string mr_ref_number { get; set; }
        public string history_id { get; set; }
        public string history_name { get; set; }
        public string historyqlty { get; set; }
        public string remainqlty { get; set; }
        public string receiveqlty { get; set; }
        public string created_by { get; set; }
        public string approved_by { get; set; }
        public string checked_by { get; set; }
        public string project_full_name { get; set; }
        public string warehouse_id { get; set; }
        public int? ordering_number { get; set; }
        public string ref_number_date { get; set; }
        public string project_so_number { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public Nullable<bool> is_received_partial { get; set; }
        public string process_type { get; set; }
        public string created_by_text { get; set; }
        public string show_status_html { get; set; }
        public Nullable<DateTime> sending_date { get; set; }
        public Nullable<DateTime> returning_date { get; set; }
        public string sending_date_str { get; set; }
        public string returning_date_str { get; set; }
        public Nullable<bool> is_export_grn { get; set; }
        public Nullable<bool> is_export_do { get; set; }
        public Nullable<bool> is_export_signature { get; set; }
        public string approved_date_text { get; set; }
        public string checked_date_text { get; set; }
        public string created_by_signature { get; set; }
        public string approved_by_signature { get; set; }
        public string checked_by_signature { get; set; }

        
        public ItemReceiveViewModel()
        {
            inventories = new List<InventoryViewModel>();
            receivedItems = new List<InventoryViewModel>();
            attachments = new List<tb_ire_attachment>();
            rejects = new List<RejectViewModel>();
            receivedHistories = new List<ItemReceiveViewModel>();
            doAttachments = new List<tb_attachment>();
        }

        public static int CountApproval(bool isAdmin,bool isQAQC,bool isSM,string userId)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
                List<ItemReceiveViewModel> itemReceiveList = new List<ItemReceiveViewModel>();

                itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true && (string.Compare(m.received_status,Status.Pending)==0 || string.Compare(m.received_status,Status.Feedbacked)==0 || string.Compare(m.received_status,Status.Approved)==0) ).Select(x => new ItemReceiveViewModel()
                {
                    receive_item_voucher_id = x.receive_item_voucher_id,
                    received_number = x.received_number,
                    received_status = x.received_status,
                    received_type = x.received_type,
                    ref_id = x.ref_id,
                    created_date = x.created_date,
                    po_report_number = x.po_report_number,
                    supplier_id = x.supplier_id,
                    is_received_partial = x.is_received_partial

                }).ToList();


                foreach (var itemReceive in itemReceiveList)
                {
                    ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                    List<Models.ItemReceivedDetailViewModel> itemDetails = new List<ItemReceivedDetailViewModel>();

                    if (isAdmin)
                    {
                        itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                        itemReceives.Add(itemRec);
                    }
                    else
                    {

                        if (string.Compare(itemReceive.received_status, Status.Pending) == 0 || string.Compare(itemReceive.received_status, Status.Feedbacked) == 0)
                        {
                            if ((string.Compare(itemReceive.received_type, "Transfer Workshop") == 0 && CommonFunctions.isQCQAbyWorkshopTransfer(itemReceive.ref_id, userId))
                                || (string.Compare(itemReceive.received_type, "Stock Transfer") == 0 && CommonFunctions.isQAQCbyStockTransfer(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Stock Return") == 0 && CommonFunctions.isQAQCbyStockReturn(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Purchase Order") == 0 && CommonFunctions.isQAQCbyPurchaseOrder(itemReceive.ref_id, userId)))
                            {
                                itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                                itemReceives.Add(itemRec);
                            }
                        }

                        if (string.Compare(itemReceive.received_status, Status.Approved) == 0)
                        {
                            if ((string.Compare(itemReceive.received_type, "Purchase Order") == 0 && CommonFunctions.isSMinSitebyPurchaseRequisition(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Stock Transfer") == 0 && CommonFunctions.isSMinSitebyStockTransfer(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Stock Return") == 0 && CommonFunctions.isSMinSitebyStockReturn(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0 && CommonFunctions.isSMinSitebyWorkShopTransfer(itemReceive.ref_id, userId)))
                            {
                                itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                                itemReceives.Add(itemRec);
                            }
                        }

                    }
                }
                return itemReceives.Count();
            }
            catch(Exception ex)
            {

            }
            return 0;
        }

        
    }
    public class ItemReceiveAttachmentViewModel
    {
        [Key]
        public string ire_attachment_id { get; set; }
        [Display(Name ="Supporting Document:")]
        public string ire_attachment_name { get; set; }
        public string ire_attachment_extension { get; set; }
        public string ire_file_path { get; set; }
        public string ire_id { get; set; }
    }
    public class ItemReceivedDetailViewModel
    {
        public string ri_detail_id { get; set; }
        public string ri_ref_id { get; set; }
        public string ri_item_id { get; set; }
        public string ri_warehouse_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string unit { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public string supplier_id { get; set; }
        public string item_status { get; set; }
        public string remark { get; set; }
        public bool completed { get; set; }
        public string ir_no { get; set; }
        public string ir_status { get; set; }
        public string ir_id { get; set; }
        public string history_id { get; set; }
        public string history_name { get; set; }
        public Nullable<int> ordering_number { get; set; }
    }

    public class ItemReceiveHistoryViewModel
    {
        public string PurchaseOrderId { get; set; }
        public string ItemId { get; set; }
        public decimal Quantity { get; set; }
    }
    public class GoodReceivedNoteStatusViewModel
    {
        [Key]
        public string received_status_id { get; set; }
        public string received_id { get; set; }
        public string received_ref_id { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    }
}