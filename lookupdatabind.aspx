<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupdatabind.aspx.cs" Inherits="lookupemail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <script src="DatePicker/jquery-1.10.2.js"></script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script language="javascript">
        function addin(obj, valu) {
           // var ob = eval('window.opener.document.getElementById(\'' + obj + '\')');
          //  ob.value = valu;
          //  window.opener.$('#' + obj).change();

            try {
                var ob = eval('window.parent.document.getElementById(\'' + obj + '\')');

                ob.value = valu;
                window.parent.$('#' + obj).change();
            } catch (e) { alert(e.Message); }

            window.parent.document.getElementById('masterpopupiframe').src = '';
            window.parent.document.getElementById('closebtn').click();
        }

        function overay() {
            document.getElementById('savepop').style.display = "inline";
        }

        $(document).ready(function () {
            window.parent.document.getElementById('masterpopupiframe').className = "";
        });
</script>
    
                <style>
                
                
#header {
    background: #3498db none repeat scroll 0 0;
    margin: auto;
    opacity: 0.9;
    padding: 5px;
}

                    .style1
                    {
                        width: 55px;
                    }

                </style>
<div id="header">
<h3 style="color: #FFFFFF">Find Data Value.</h3>
</div>
<table  cellpadding=3 cellspacing=3 width="98%" style="margin-left:20px">
                <tr><td valign="top" class="style1">
                    ค้นหา</td><td>
                        <asp:DropDownList ID="DropDownList1" runat="server">
                        </asp:DropDownList>
&nbsp;<asp:TextBox ID="txt" runat="server" Font-Size="16px" Width="213px"></asp:TextBox>&nbsp; 
                        <asp:Button ID="Button5"
                            runat="server" Text="Find" onclick="Button5_Click" Width="106px" />

                    </td></tr>
                <tr><td colspan="2">
                        <asp:GridView ID="GridView2" runat="server" 
                            CellPadding="4" GridLines="None" 
                            onselectedindexchanged="GridView2_SelectedIndexChanged" Width="99%" 
                        Font-Size="Small" ForeColor="#333333" onrowdatabound="GridView2_RowDataBound">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField HeaderText="Select" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                                            CommandName="Select" onclientclick="overay()" Text="Select"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ControlStyle Width="50px" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Font-Bold="True" ForeColor="#FF6600" />
                                </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#2461BF" />
                            <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" 
                                HorizontalAlign="Left" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EFF3FB" HorizontalAlign="Left" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
      
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#4870BE" />
      
                        </asp:GridView>
                    <br />
                    <asp:Label ID="Label4" runat="server"></asp:Label>
                    </td></tr>
                </table>
    <table border="0" cellpadding="0" cellspacing="0"  id="id-form">
        <tr>
            <td colspan=3 align=left>
                <asp:Literal ID="msg" runat="server" 
            Text="&lt;h4 class=&quot;alert_success&quot;&gt;A Success Message&lt;/h4&gt;" 
            Visible="False"></asp:Literal>
                <asp:Literal ID="error" runat="server" Text="&lt;h4 class=&quot;alert_error&quot;&gt;An Error Message&lt;/h4&gt;
" Visible="False"></asp:Literal>
            </td>
        </tr>
    </table>
</div>
    </form>
     <style  type="text/css"> 
.dim
{
            height:100%;
            width:100%;
            position:fixed;
            left:0;
            top:0;
            z-index:1 !important;
            background-color:black;
            filter: alpha(opacity=60); /* internet explorer */
-khtml-opacity: 0.60;      /* khtml, old safari */
-moz-opacity: 0.60;       /* mozilla, netscape */
opacity: 0.60;           /* fx, safari, opera */
}
 
 
.dialog_wrapper { width: 100%; top: 0px; left: 0px; position: absolute; z-index: 5; display: block;  }
 
.dialog {
	position: fixed;
	left: 50%;
	width: 600px;
	margin-left: -300px;
	height:300px;
	margin-top:50px;
}
</style>
<div id="savepop" style="display:none">
<div class="dim" >
</div>
    <div class="dialog_wrapper">
<div class="dialog"><center><br />
    <img id="ctl00_ContentPlaceHolder1_Image1" src="images/loadingc.gif" style="height:auto;width:auto;border-width:0px;" /></center></div>
</div>
</div>
</body>
</html>
