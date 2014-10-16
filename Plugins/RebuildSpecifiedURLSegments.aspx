<%@ Page Language="c#" Codebehind="RebuildSpecifiedURLSegments.aspx.cs" AutoEventWireup="False" Inherits="Nergard.EPi.Plugins.RebuildSpecifiedURLSegments.Plugins.RebuildSpecifiedURLSegments" Title="" %>
<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" assembly="EPiServer.UI" %>

<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <div class="epi-buttonDefault">
        ContentReference: <asp:TextBox id="txtReference" runat="server" />
        <EPiServerUI:ToolButton id="ButtonRebuild" onclick="ButtonRebuild_Click" runat="server" text="Rebuild" tooltip="Rebuild" SkinID="Check" />
    </div>
    <table class="epi-default">
       <tr>
           <th><episerver:translate text="/admin/rebuildurlsegments/status" runat="server" id="TranslateStatus" /></th>
           <th><episerver:translate text="/admin/rebuildurlsegments/pages" runat="server" id="Translate1" /></th>
           <th><episerver:translate text="/admin/rebuildurlsegments/pagestoreplace" runat="server" id="TranslatePagesToReplace" /></th>
       </tr>
       <tr>
           <td><asp:Label ID="LabelStatus" runat="server" /></td>
           <td><asp:Label ID="LabelStatusPages" runat="server" /></td>
           <td><asp:Label ID="LabelPagesToReplace" runat="server" /></td>
       </tr>
     </table>
</asp:Content>