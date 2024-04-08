using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using MoreLinq;

namespace BT_KimMex.Models
{
    public class QuoteViewModel
    {
        [Key]
        public string quote_id { get; set; }
        public Nullable<int> number { get; set; }
        [Display(Name ="Quote No.:")]
        public string quote_no { get; set; }
        [Display(Name ="Supplier Name:")]
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public Nullable<bool> is_selected { get; set; }
        public Nullable<bool> status { get; set; }
        [Display(Name ="Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
        public string item_id { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> discount { get; set; }
        public Nullable<decimal> priceDiscount { get; set; }
        public Nullable<decimal> discountAmount { get; set; }
        public List<QuoteItemsViewModel> quoteItems { get; set; }
        public List<Entities.tb_attachment> attachments { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string quote_detail_id { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string approved_comment { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string checked_comment { get; set; }
        public string quote_status { get; set; }
        public decimal amount { get; set; }
        public string mr_id { get; set; }
        public string incoterm { get; set; }
        public string payment { get; set; }
        public string delivery { get; set; }
        public string shipment { get; set; }
        public string warranty { get; set; }
        public string vendor_ref { get; set; }
        public string mr_number { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public Nullable<bool> is_quote_complete { get; set; }
        public Nullable<decimal> grand_total_amount { get; set; }
        public Nullable<decimal> lump_sum_discount_amount { get; set; }
        public string grand_total_amount_text { get; set; }
        public string created_at_text { get; set; }
        public string show_quote_status { get; set; }
        public string pr_id { get; set; }
        public string pr_no { get; set; }

        //public string incoterm { get; set; }
        //public string payment { get; set; }
        //public string delivery { get; set; }
        //public string shipment { get; set; }
        //public string warranty { get; set; }
        //public string vendor_ref { get; set; }
        public List<QuoteItemAttachment> itemQuoteAttachments { get; set; }
        //add
        public List<ProductforQuoteViewModel> ProductforQuote { get; set; }
        //end

        public QuoteViewModel()
        {
            quoteItems = new List<QuoteItemsViewModel>();
            attachments = new List<Entities.tb_attachment>();
            //add
            ProductforQuote = new List<ProductforQuoteViewModel>();
            //end
        }

        public IEnumerable<ProductViewModel> products { get; set; }
        public IEnumerable<ProductViewModel> product { get; set; }
        public static QuoteViewModel GetQuoteDetail(string id)
        {
            QuoteViewModel quote = new QuoteViewModel();
            List<QuoteItemsViewModel> quoteItems = new List<QuoteItemsViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //quote= db.tb_quote
                //    .Join(db.tb_supplier, q => q.supplier_id, s => s.supplier_id, (q, s) => new { q, s })
                //    .Where(x => x.q.quote_id == id).Select(x => new QuoteViewModel() { quote_id = x.q.quote_id, quote_no = x.q.quote_no, supplier_id = x.q.supplier_id, created_date = x.q.created_date, supplier_name = x.s.supplier_name,created_by=x.q.created_by,updated_by=x.q.updated_by,quote_status=x.q.quote_status }).FirstOrDefault();
                //quoteItems = db.tb_quote_detail
                //    .Join(db.tb_product, qi => qi.item_id, it => it.product_id, (qi, it) => new { qi, it })
                //    .OrderBy(x=>x.it.product_code)
                //    .Where(x => x.qi.quote_id == quote.quote_id)
                //    .Select(x => new QuoteItemsViewModel() { quote_detail_id = x.qi.quote_detail_id, item_id = x.qi.item_id, product_code = x.it.product_code, product_name = x.it.product_name, product_unit = x.it.product_unit, price = x.qi.price, discount = x.qi.discount, remark = x.qi.remark }).ToList();
                quote = (from qquote in db.tb_quote
                         join supplier in db.tb_supplier on qquote.supplier_id equals supplier.supplier_id
                         join pr in db.tb_purchase_requisition on qquote.mr_id equals pr.purchase_requisition_id
                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                         join project in db.tb_project on mr.ir_project_id equals project.project_id
                         where string.Compare(qquote.quote_id, id) == 0
                         select new QuoteViewModel()
                         {
                             quote_id = qquote.quote_id,
                             quote_no = qquote.quote_no,
                             supplier_id = qquote.supplier_id,
                             created_date = qquote.created_date,
                             supplier_name = supplier.supplier_name,
                             quote_status = qquote.quote_status,
                             mr_id = qquote.mr_id,
                             mr_number = pr.purchase_requisition_number,
                             project_id = project.project_id,
                             project_fullname = project.project_full_name,
                             incoterm = qquote.incoterm,
                             payment = qquote.payment,
                             delivery = qquote.delivery,
                             shipment = qquote.shipment,
                             warranty = qquote.warranty,
                             vendor_ref = qquote.vendor_ref,
                             is_quote_complete = pr.is_quote_complete,
                             created_by = qquote.created_by,
                             updated_by = qquote.updated_by,
                             grand_total_amount = qquote.grand_total_amount,
                             lump_sum_discount_amount = qquote.lump_sum_discount_amount,
                         }).FirstOrDefault();

                quoteItems = (from qi in db.tb_quote_detail
                              join it in db.tb_product on qi.item_id equals it.product_id
                              join u in db.tb_unit on it.product_unit equals u.Id
                              orderby qi.ordering_number, it.product_code
                              where qi.quote_id == quote.quote_id
                              select new QuoteItemsViewModel()
                              {
                                  quote_detail_id = qi.quote_detail_id,
                                  item_id = qi.item_id,
                                  product_code = it.product_code,
                                  product_name = it.product_name,
                                  product_unit = it.product_unit,
                                  price = qi.price,
                                  discount = qi.discount,
                                  remark = qi.remark,
                                  unit_name = u.Name,
                                  qty = qi.qty,
                                  discount_amount = qi.discount_amount ?? 0
                              }).ToList();
                quote.quoteItems = quoteItems;
                quote.attachments = Class.Inventory.GetAttachments(quote.quote_id);
            }
            return quote;
        }

        public static QuoteViewModel GetSupplierQuoteByPRAndSupplier(string pr_id,string supplier_id)
        {
            QuoteViewModel model = new QuoteViewModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var quotesups = (from quote in db.tb_quote
                                     //join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                                 orderby quote.updated_date descending
                                 where quote.status == true && string.Compare(quote.mr_id, pr_id) == 0 && string.Compare(quote.supplier_id, supplier_id) == 0
                                 && (string.Compare(quote.quote_status, Status.Approved) == 0 || string.Compare(quote.quote_status, Status.Edit) == 0)
                                 select quote).FirstOrDefault();
                if(quotesups !=null )
                {
                    model = QuoteViewModel.GetQuoteDetail(quotesups.quote_id);
                }
            }
            catch (Exception ex) { }
            return model;
        }

    }
    public class QuoteItemsViewModel
    {
        [Key]
        public string quote_detail_id { get; set; }
        public string quote_id { get; set; }
        public string item_id { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> discount { get; set; }
        public string remark { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string product_unit { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public string unit_name { get; set; }
        public Nullable<decimal> qty { get; set; }
        public Nullable<decimal> discount_amount { get; set; }
        public Nullable<int> ordering_number { get; set; }

    }
    public class QuoteFilterResultModel
    {
        public tb_quote quote { get; set; }
        public tb_supplier supplier { get; set; }
        public tb_purchase_requisition pr { get; set; }
        public tb_item_request mr { get; set; }
        public tb_project project { get; set; }
    }
    public class QuoteItemAttachment
    {
        public string quote_attachment_id { get; set; }
        public string quote_attachment_name { get; set; }
        public string quote_attachment_extension { get; set; }
        public string quote_attachment_part { get; set; }
        public string quote_attachment_ref_id { get; set; }
    }

    //add new 9/7/18


    public class ProductforQuoteViewModel
    {
        [Key]
        public string quote_detail_id { get; set; }
        public string quote_id { get; set; }
        public string item_id { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> discount { get; set; }
        public string remark { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string product_unit { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public string product_id { get; set; }


        public string p_category_id { get; set; }


        public string p_category_name { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string Remark { get; set; }
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

    }
    public class CategoryforQuoteViewModel
    {
        [Key]
        public string p_category_id { get; set; }
   
        public string p_category_code { get; set; }
 
        public string p_category_name { get; set; }

        public string p_category_address { get; set; }
       
        public string chart_account { get; set; }
  
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<bool> status { get; set; }
    }
    //End
    public class ItemQuoteSupplierViewModel
    {
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemUnit { get; set; }
        public Nullable<decimal> ItemPrice { get; set; }
        public List<ItemQuoteSupplierDetailViewModel> Quotes { get; set; }
        public string[] removeAttachments { get; set; }
    }
    public class ItemQuoteSupplierDetailViewModel
    {
        public string QuoteID { get; set; }
        public string SupplierID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string[] Attachments { get; set; }
    }
    public class SupplierQuoteModel
    {
       
    }
}