<%@ Page Language="C#" AutoEventWireup="true" CodeFile="lookupemail.aspx.cs" Inherits="lookupemail" %>

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
            //var ob = eval('window.opener.document.getElementById(\'' + obj + '\')');
            //ob.value = ob.value + valu + ';';
            //window.close();

            var ob = eval('window.parent.document.getElementById(\'' + obj + '\')');
            ob.value = ob.value + valu + ';';
            window.parent.document.getElementById('masterpopupiframe').src = '';
            window.parent.document.getElementById('closebtn').click();
        }
   	function addinFullname(obj, valu) {
            try {
                var ob = eval('window.parent.document.getElementById(\'' + obj + '\').getAttribute(\'data-emailfullname\')');
                if (ob != undefined) {
                    var obname = eval('window.parent.document.getElementById(\'' + ob + '\')');
                    obname.value = valu;
                }
            } catch (err) { }
        }

        $(document).ready(function () {
            window.parent.document.getElementById('masterpopupiframe').className = "";
	    setTimeout(function(){
               $('#txt').focus();
               $('#txt').select();
            },200);

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
                        width: 133px;
                    }

                </style>
<div id="header">
<h1 style="color: #FFFFFF">Find Email.</h1>
</div>
<table  cellpadding=3 cellspacing=3 width="98%" style="margin-left:20px">
                <tr><td valign="top" class="style1">
                    ชื่อภาษาอังกฤษ</td><td>
                        <asp:TextBox ID="txt" runat="server" Font-Size="16px" Width="213px"></asp:TextBox>&nbsp; 
                        <asp:Button ID="Button5"
                            runat="server" Text="Find" onclick="Button5_Click" Width="106px" />

                    </td></tr>
                <tr><td colspan="2">
                        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
                            CellPadding="4" GridLines="None" 
                            onselectedindexchanged="GridView2_SelectedIndexChanged" Width="90%" 
                        Font-Size="Small" ForeColor="#333333">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:BoundField DataField="Full Name" HeaderText="Full Name">
                                <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Email" HeaderText="Email">
                                <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:CommandField HeaderText="Select" ShowSelectButton="True">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Font-Bold="True" ForeColor="#FF6600" />
                                </asp:CommandField>
                            </Columns>
                            <EditRowStyle BackColor="#2461BF" />
                            <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" 
                                HorizontalAlign="Left" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EFF3FB" />
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
</body>
</html>
