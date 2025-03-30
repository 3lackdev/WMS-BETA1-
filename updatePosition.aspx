<%@ Page Title="" Language="C#"  ValidateRequest="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="updatePosition.aspx.cs" Inherits="template" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <asp:Label ID="lb_title" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="Position and Email"></asp:Label>
            <br />
            <br />
            <asp:Label ID="lb_poname" runat="server" Font-Bold="True" Font-Size="Large" 
                ForeColor="#3333CC" Text="Position name"></asp:Label>
            &nbsp;&nbsp;
            <asp:Button ID="btn_new" runat="server" Text="New Position" 
                onclick="btn_new_Click" Visible="False" />
    <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
                CellPadding="3" GridLines="Horizontal" Width="99%">
                <AlternatingRowStyle BackColor="#F7F7F7" />
                <Columns>
                    <asp:BoundField DataField="PositionRowId" HeaderText="PrositionRowId">
                    <HeaderStyle HorizontalAlign="Left" Width="250px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="PositionName">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("PositionName") %>'></asp:Label>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnk_edit" runat="server" onclick="lnk_edit_Click">Edit</asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TemplateName") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="PositionEmail" HeaderText="PositionEmail" 
                        HtmlEncode="False">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="selectedkey" HeaderText="Selected Key">
                    <HeaderStyle HorizontalAlign="Left" Width="150px" />
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
            <asp:Label ID="Label3" runat="server" Text="PositionName"></asp:Label>
            &nbsp;&nbsp;
            <asp:TextBox ID="txt_templatename" runat="server"></asp:TextBox>
            <br />
            Email&nbsp;<asp:TextBox ID="txt_email" runat="server" Height="175px" 
                TextMode="MultiLine" Width="678px"></asp:TextBox>
            <br />
            **** {sys_logonemail}&nbsp; &gt;&gt; get email คน ที่ logon<br /> **** 
            {template_object}&nbsp; &gt;&gt; get email&nbsp; จากค่าใน Template<br />
            <br />
            Selected Key
            <asp:TextBox ID="txt_selectedkey" runat="server"></asp:TextBox>
            <br />
            <asp:HiddenField ID="HiddenField1" runat="server" />
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

