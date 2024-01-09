<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockBalanceByDateandWarehouse.aspx.cs" Inherits="BT_KimMex.Views.Shared.ReportController.StockBalanceByDateandWarehouse" %>--%>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>StockBalanceByDate and warehouse Report</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            //if (!IsPostBack){
            //    string reportPath = Server.MapPath("~/Reports/StockBalanceByDateandWarehouse.rdlc");
            //    BT_KimMex.Class.ReportHandler.GenerateStockBalanceByDateandWarehouseReport(this.ReportViewer1, reportPath);

            //}

            if (!IsPostBack)
            {
                DateTime date = DateTime.Now;
                string WarehouseId = string.Empty;
                string project = string.Empty;
                int reportType = 0;
                string reportPath = Server.MapPath("~/Reports/StockBalanceByDateandWarehouseStockKeeper.rdlc");
                if (Request.QueryString["date"] != null)
                    date = Convert.ToDateTime(Request.QueryString["date"]);
                if (Request.QueryString["WarehouseId"] != null)
                    WarehouseId = Request.QueryString["WarehouseId"].ToString();
                if (Request.QueryString["project"] != null)
                    project = Request.QueryString["project"].ToString();
                if (Request.QueryString["report"] != null)
                {
                    reportType = Convert.ToInt32(Request.QueryString["report"]);
                }
                if(reportType>0)
                    reportPath = Server.MapPath("~/Reports/StockBalanceByDateandWarehousebyStockController.rdlc");

                //if (!string.IsNullOrEmpty(WarehouseId))
                if(date != null)
                    BT_KimMex.Class.ReportHandler.GenerateMonthlyStockBalance(this.ReportViewer1, reportPath ,date , WarehouseId ,project);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1105px" AsyncRendering="False" SizeToReportContent="True" >
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>

