<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Quote Report</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string id = string.Empty;
                string reportPath = Server.MapPath("~/Reports/Quote.rdlc");
                if (Request.QueryString["id"] != null)
                    id = Request.QueryString["id"].ToString();
                if (!string.IsNullOrEmpty(id))
                    BT_KimMex.Class.ReportHandler.GenerateQuoteReport(this.ReportViewer1, reportPath, id);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
