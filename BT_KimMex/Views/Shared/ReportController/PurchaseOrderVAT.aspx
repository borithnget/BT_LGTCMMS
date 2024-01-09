<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchaseOrderVAT.aspx.cs" Inherits="BT_KimMex.Views.Shared.ReportController.PurchaseOrderVAT" %>--%>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" style="text-align:center">
    <title>Purchase Order Report</title>
    
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string poId = string.Empty;
                string supplierId = string.Empty;
                string isselected = string.Empty;
                string reportPath = Server.MapPath("~/Reports/PurchaseOrderToSupplier.rdlc");
                if (Request.QueryString["id"] != null)
                    poId = Request.QueryString["id"].ToString();
                if (Request.QueryString["supplierId"] != null)
                    supplierId = Request.QueryString["supplierId"].ToString();
                //if (Request.QueryString["isselected"] != null)
                //    isselected = Request.QueryString["isselected"].ToString();


                if (!string.IsNullOrEmpty(poId) && !string.IsNullOrEmpty(supplierId))
                    BT_KimMex.Class.ReportHandler.GeneratePOSupplier(this.ReportViewer1, reportPath, poId, supplierId);
            }

            //if (!IsPostBack)
            //{
            //    string poId = string.Empty;
            //    string supplierId = string.Empty;
            //    string reportPath = Server.MapPath("~/Reports/PurchaseOrderToSupplierNotVAT.rdlc");
            //    if (Request.QueryString["id"] != null)
            //        poId = Request.QueryString["id"].ToString();
            //    if (Request.QueryString["supplierId"] != null)
            //        supplierId = Request.QueryString["supplierId"].ToString();

            //    if (!string.IsNullOrEmpty(poId) && !string.IsNullOrEmpty(supplierId))
            //        BT_KimMex.Class.ReportHandler.GeneratePOSupplierNotVAT(this.ReportViewer1, reportPath, poId, supplierId);
            //}

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1105px"  AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
    
    </div>
    </form>
    
</body>
</html>
