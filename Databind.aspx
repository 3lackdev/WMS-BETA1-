<%@ Page Title="" Language="C#"  ValidateRequest="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Databind.aspx.cs" Inherits="template" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script>
        $(document).keypress(function (e) {
            if (e.which == 13) {
                if (e.target.name.toString().indexOf("txt_tt") >= 0) {
                    $("#ContentPlaceHolder1_ss").click();
                    $("#ContentPlaceHolder1_ss").focus();
                    return false;
                }
            }
        });
 </script>
<style>

 .warenone
        {
            display:none;
        }</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <asp:Label ID="lb_title" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="DataBind Value"></asp:Label>
            <br />
            &nbsp;<table width="98%">
                <tr>
                    <td>
                        <asp:Button ID="btn_new" runat="server" onclick="btn_new_Click" 
                            Text="New Bind Data" BackColor="#669900" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
                        <asp:Button ID="btn_back3" runat="server" onclick="btn_back0_Click" Text="Back" 
                            Width="113px" />
                    </td>
                    <td width="250">
                        <asp:TextBox ID="txt_tt" runat="server" TabIndex="100"></asp:TextBox>
                        &nbsp;<asp:Button ID="ss" runat="server" onclick="Unnamed1_Click" TabIndex="101" 
                            Text="Search" />
                    </td>
                </tr>
            </table>
    <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
                CellPadding="3" GridLines="Horizontal" Width="99%">
                <AlternatingRowStyle BackColor="#F7F7F7" />
                <Columns>
                    <asp:BoundField DataField="BindRowID" HeaderText="BindRowID">
                    <ControlStyle Width="1px" />
            <HeaderStyle HorizontalAlign="Left" Width="0px" CssClass="warenone" 
                Wrap="False" />
            <ItemStyle CssClass="warenone" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="BindName">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("BindName") %>'></asp:Label>
                            &nbsp;
                            <asp:LinkButton ID="LinkButton1" runat="server" onclick="lnk_edit_Click">Edit</asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("BindName") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" Width="200px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Server" HeaderText="Server">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Sql" HeaderText="Sql">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataFieldText" HeaderText="DataFieldText">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataFieldValue" HeaderText="DataFieldValue">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="crby" HeaderText="Owner">
                    <HeaderStyle HorizontalAlign="Left" Width="80px" />
                    </asp:BoundField>
                </Columns>
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                <SortedAscendingCellStyle BackColor="#F4F4FD" />
                <SortedAscendingHeaderStyle BackColor="#5A4C9D" />
                <SortedDescendingCellStyle BackColor="#D8D8F0" />
                <SortedDescendingHeaderStyle BackColor="#3E3277" />
            </asp:GridView>
            <br />
                  <center> 
               <asp:Button ID="btn_back0" runat="server" onclick="btn_back0_Click" Text="Back" 
                Width="113px" /> </center>
            <br />
        </asp:View>
        <asp:View ID="View2" runat="server">
            สร้างใหม่<br />
            <br />
            <table style="width:100%;">
                <tr>
                    <td width="150">
                        <asp:Label ID="Label3" runat="server" Text="Bind Name"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txt_BindName" runat="server" Width="250px"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        Server&nbsp;&nbsp;&nbsp;</td>
                    <td>
                        <asp:DropDownList ID="dl_Server" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        Sql</td>
                    <td>
                        <asp:TextBox ID="txt_Sql" runat="server" Height="275px" TextMode="MultiLine" 
                            Width="90%"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        DataFieldTextField&nbsp;&nbsp;&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txt_DataFieldText" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        DataFieldValueField&nbsp;&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txt_DataFieldValue" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
            <br />
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <asp:Label ID="error" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <br />
            <asp:Button ID="btn_save0" runat="server" onclick="btn_save0_Click" 
                Text="Save" />
&nbsp;
            <asp:Button ID="btn_back" runat="server" onclick="btn_back_Click" Text="Back" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_del" runat="server" onclick="btn_del_Click" 
                onclientclick="return confirm('ต้องการลบ?')" Text="Delete" />
        </asp:View>
    </asp:MultiView>
<br />
    </asp:Content>

