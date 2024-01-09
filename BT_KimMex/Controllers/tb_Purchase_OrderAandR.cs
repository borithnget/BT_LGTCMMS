using System;

namespace BT_KimMex.Controllers
{
    internal class tb_Purchase_OrderAandR
    {
        internal bool active;
        internal DateTime created_at;
        internal string created_by;
        internal string purchase_order_id;
        internal string purchase_order_number;
        internal bool status;
        internal string supplier_element;
        internal DateTime updated_at;
        internal string updated_by;

        public string Update_by { get; internal set; }
        public object Update_Date { get; internal set; }
        public string Upprove_by { get; internal set; }
        public DateTime Uprove_date { get; internal set; }
    }
}