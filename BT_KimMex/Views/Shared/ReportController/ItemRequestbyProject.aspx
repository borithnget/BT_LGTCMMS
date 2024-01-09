<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BOQ vs Item Request by Project Report</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string projectId = string.Empty;
                string reportPath = Server.MapPath("~/Reports/Table12ItemRequestbyProject.rdlc");
                if (Request.QueryString["projectId"] != null)
                    projectId = Request.QueryString["projectId"].ToString();
                if (!string.IsNullOrEmpty(projectId))
                    BT_KimMex.Class.ReportHandler.GenerateItemRequestbyProject(this.ReportViewer1, reportPath, projectId);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1></h1>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1105px"  AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
        <br />
    </div>
    </form>
</body>
</html>
