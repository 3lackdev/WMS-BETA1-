<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="workflowhistory.aspx.cs" Inherits="workflowhistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <style type="text/css">
        .style1
        {
            height: 18px;
        }
        .style2
        {
            height: 18px;
            width: 425px;
        }

    .nna
    {
        display:none;
    }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
        <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" 
        Text="WorkFlow runing status"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_back0" runat="server" onclick="btn_back0_Click" Text="Back" 
                Width="113px" TabIndex="2" />
    <br />
    <table style="width: 99%; float:left"><tr><td class="style1" width="50px">ค้นหาจาก</td>
        <td width="450px">
            <asp:DropDownList ID="DropDownList1" runat="server">
                <asp:ListItem>WFName</asp:ListItem>
                <asp:ListItem Value="ETC1">Number</asp:ListItem>
                <asp:ListItem>Status</asp:ListItem>
                <asp:ListItem Value="WorkFlowRunningStatus.StartRowID">StartRowID</asp:ListItem>
                <asp:ListItem>CurrentStepName</asp:ListItem>
                <asp:ListItem Value="WorkFlowRunningStatus.UdBy">UdBy</asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox ID="TextBox1" runat="server" Width="283px"></asp:TextBox>
        </td><td class="style1" width="80px">
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="ค้นหา" 
                TabIndex="1" />
        </td><td >
            <asp:CheckBox ID="meonly" runat="server" Checked="True" Text="ดูเฉพาะของฉัน" 
                AutoPostBack="True" oncheckedchanged="meonly_CheckedChanged" />
            &nbsp;&nbsp;
            <asp:DropDownList ID="wflist" runat="server" AutoPostBack="True" 
                onselectedindexchanged="wflist_SelectedIndexChanged">
            </asp:DropDownList>
        </td><td>     <div style="float:right">
            <asp:Label ID="Label5" runat="server" Text="พบรายการทั้งหมด :  200 รายการ"></asp:Label></div></td></tr></table>
            <br />
       
    <br />
     <asp:GridView ID="GridView1" runat="server" CellPadding="4" 
                GridLines="None" onselectedindexchanged="GridView1_SelectedIndexChanged" 
                Width="99%" ForeColor="#333333" AutoGenerateColumns="False" 
                AllowPaging="True" onpageindexchanging="GridView1_PageIndexChanging" 
                PageSize="40" DataKeyNames="UserSpecialAction" 
                onrowdatabound="GridView1_RowDataBound">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:CommandField HeaderText="History" ShowSelectButton="True" 
                        SelectText="View">
                    <HeaderStyle Width="70px" HorizontalAlign="Left" />
                    <ItemStyle Font-Bold="True" ForeColor="#3333CC" />
                    </asp:CommandField>
                    <asp:TemplateField HeaderText="Special Action">
                        <ItemTemplate>
                            <asp:Panel ID="Panel1" runat="server">
                            
                            <asp:HyperLink ID="HyperLink1" runat="server" Font-Bold="True" 
                                NavigateUrl='<%# String.Format("application/{1}/viewnotificationalert.aspx?startrowid={0}",Eval("StartRowID"),Eval("ApplicationFolder")) %>' 
                                Text="Notice" Target="_blank"></asp:HyperLink>
                            &nbsp;|
                            <asp:HyperLink ID="HyperLink3" runat="server" Font-Bold="True" 
                                NavigateUrl='<%# Eval("StartRowID", "WorkflowRuning.aspx?rowid=force&runingkey={0}&referencekey=force") %>' 
                                Target="_blank" Text="Cur. flow"></asp:HyperLink>
                            &nbsp;|
                            <asp:HyperLink ID="HyperLink4" runat="server" Font-Bold="True" 
                                NavigateUrl='<%# Eval("StartRowID", "WorkflowRuning.aspx?rowid=force&runingkey={0}&referencekey=force&action=update") %>' 
                                Target="_blank" Text="Ud. Value"></asp:HyperLink>
                            &nbsp;|
                            <asp:LinkButton ID="LinkButton2" runat="server" Font-Bold="True" 
                                Font-Underline="False" ForeColor="Red" onclick="LinkButton2_Click" 
                                onclientclick="return confirm('ยืนยันการลบ')" 
                                ToolTip='<%# String.Format("{0},{1}",Eval("WFRowID"),Eval("StartRowID")) %>'>Del</asp:LinkButton>
                                </asp:Panel>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" Width="220px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="StartRowID" HeaderText="StartRowID">
                    <HeaderStyle HorizontalAlign="Left" Width="300px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ETC1" HeaderText="ETC1">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="WFName" HeaderText="WFName">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Current StepName">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("[Current StepName]") %>'></asp:Label>
                            &nbsp;
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" 
                                Text='<%# Bind("[Current StepName]") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="UdBy" HeaderText="UdBy">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="UdDate" HeaderText="UdDate">
                    <HeaderStyle HorizontalAlign="Left" Width="150px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Status" HeaderText="Status">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" 
                    HorizontalAlign="Left" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" 
                    HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" ForeColor="#333333" Font-Bold="True" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
        </asp:View>
        <asp:View ID="View2" runat="server">
            <asp:Label ID="lb_title1" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" 
                Text="WorkFlow runing status (History)"></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Runing RowID :"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_back1" runat="server" onclick="btn_back1_Click" Text="Back" 
                Width="113px" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_view" runat="server" onclick="btn_view_Click" 
                Text="View Current" Width="113px" />
            <br />
            <br />
            <asp:GridView ID="GridView2" runat="server" BackColor="White" 
                BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
                GridLines="Horizontal" 
                Width="99%" AutoGenerateColumns="False">
                <AlternatingRowStyle BackColor="#F7F7F7" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HiddenField ID="HiddenField1" runat="server" 
                                Value='<%# bind("rowid") %>' />
                            <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click" 
                                onclientclick="return confirm('Confirm?')">Resent</asp:LinkButton>
                            &nbsp;|
                            <asp:LinkButton ID="LinkButton3" runat="server" onclick="LinkButton3_Click">Rewind</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StepName" HeaderText="ทำ Action ที่ Step">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="crby" HeaderText="ทำ Action โดย">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="crdate" HeaderText="ทำ Action เมื่อ">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Event" HeaderText="ทำ Action ที่ปุ่ม">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SentEmailTo" HeaderText="ส่ง Action ไปต่อที่">
                    <HeaderStyle HorizontalAlign="Left" />
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Email" HeaderText="ส่งไปต่อที่ Email " 
                        HtmlEncode="False" >
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CCEmail" HeaderText="ส่งไปต่อที่ CCEmail" 
                        HtmlEncode="False">
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="NextStep" HeaderText="Start Step ต่อไปที่">
                    <HeaderStyle HorizontalAlign="Left" />
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" 
                    HorizontalAlign="Left" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                <SortedAscendingCellStyle BackColor="#F4F4FD" />
                <SortedAscendingHeaderStyle BackColor="#5A4C9D" />
                <SortedDescendingCellStyle BackColor="#D8D8F0" />
                <SortedDescendingHeaderStyle BackColor="#3E3277" />
            </asp:GridView>
            <br />
            <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium" 
                ForeColor="#33CC33" Text="ReSent Completed" Visible="False"></asp:Label>
        </asp:View>
        <asp:View ID="View3" runat="server">
        <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" 
                Text="WorkFlow runing status (Rewind Step)"></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label6" runat="server" Font-Bold="True" Text="Runing RowID :"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="Back" 
                Width="113px" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button3" runat="server" onclick="btn_view_Click" 
                Text="View Current" Width="113px" />
            <br />
            <br />
 
            <br />
            <br />
            <asp:Label ID="Label7" runat="server" Text="Rewind to Next Step"></asp:Label>
            &nbsp;:
            <asp:Label ID="Label8" runat="server" Font-Bold="True" Text="Label"></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label9" runat="server" Text="Email To "></asp:Label>
            &nbsp;:&nbsp;
            <asp:TextBox ID="TextBox2" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Width="800px"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label10" runat="server" Text="CC Email To "></asp:Label>
            &nbsp;:&nbsp;
            <asp:TextBox ID="TextBox3" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Width="800px"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label11" runat="server" Text="Label" Visible="False"></asp:Label>
            <br />
            <asp:Label ID="Label12" runat="server" Text="Label" Visible="False"></asp:Label>
            <br />
            <asp:Label ID="Label13" runat="server" Text="Label" Visible="False"></asp:Label>
            <br />
            <asp:Label ID="Label14" runat="server" Text="Label" Visible="False"></asp:Label>
            <br />
            <asp:Label ID="Label15" runat="server" Text="Label" Visible="False"></asp:Label>
            <asp:TextBox ID="TextBox4" runat="server" TextMode="MultiLine" Visible="False" 
                Width="500px"></asp:TextBox>
            <br />
            <asp:Button ID="Button4" runat="server" ForeColor="Red" onclick="Button4_Click" 
                onclientclick="return confirm('ยืนยันการย้อน step? หลังจากยืนยัน step ปัจจุบันจะอยู่ step นี้');" 
                Text="Force Rewind Step" />
            <br />
 
        <br />
        </asp:View>
    </asp:MultiView>
    
</asp:Content>

