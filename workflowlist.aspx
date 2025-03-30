<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="workflowlist.aspx.cs" Inherits="workflowlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .style3
        {
            height: 26px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="WorkFlow List"></asp:Label>
            <br />
            <br />
            <table width="99%"><tr><td width="400px">    <asp:Button ID="btn_new" 
                    runat="server" Text="New Workflow" onclick="btn_new_Click" 
                    BackColor="#99CC00" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_back0" runat="server" onclick="btn_back0_Click" Text="Back" 
                Width="113px" /></td><td>
                   <div style="float:left"> <asp:CheckBox ID="meonly" runat="server" AutoPostBack="True" Checked="True" 
                        oncheckedchanged="meonly_CheckedChanged" Text="ดูเฉพาะของฉัน" />
                   </div>
                    <div style="float:right"><asp:LinkButton ID="LinkButton6" runat="server" 
                            Visible="False">Import Workflow</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </div>
                </td></tr></table>

    <br />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
        CellPadding="3" GridLines="Horizontal" Width="99%" DataKeyNames="useredit">
        <AlternatingRowStyle BackColor="#F7F7F7" />
        <Columns>
            <asp:BoundField DataField="WFRowId" HeaderText="WorkFlowID">
            <HeaderStyle HorizontalAlign="Left" Width="250px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="WorkFlow Name">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("WFName") %>' 
                        Font-Bold="True"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">Edit</asp:LinkButton>
                    &nbsp;&nbsp;<asp:LinkButton ID="LinkButton4" runat="server" 
                        onclick="LinkButton4_Click">History</asp:LinkButton>
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="LinkButton3" runat="server" onclick="LinkButton3_Click" 
                        onclientclick="javascript:return confirm(' Do you want copy?')">Copy</asp:LinkButton>
                    &nbsp;&nbsp;<asp:HyperLink ID="HyperLink5"  runat="server" 
                        Target="_blank" NavigateUrl='<%# Eval("WFRowId", "exportscript.aspx?wfrowid={0}") %>'>ExportWF</asp:HyperLink>
                    &nbsp;
                    <asp:LinkButton ID="LinkButton2" runat="server" ForeColor="#FF0066" 
                        onclick="LinkButton2_Click">View API</asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("WFName") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Seting">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" Font-Bold="True" 
                        ForeColor="#3333CC" 
                        NavigateUrl='<%# Eval("WFRowId", "WorkflowRuning.aspx?rowid={0}") %>' 
                        Target="_blank">Test Run</asp:HyperLink>
                    &nbsp;|&nbsp;<asp:HyperLink ID="lnk_template" runat="server" 
                        NavigateUrl='<%# Eval("WFRowId", "template.aspx?rowid={0}") %>' 
                        Target="_parent" Text="Template"></asp:HyperLink>
                    &nbsp;|
                    <asp:HyperLink ID="lnk_position" runat="server" 
                        NavigateUrl='<%# Eval("WFRowId", "Position.aspx?rowid={0}") %>' 
                        Target="_parent" Text="Position &amp; Email"></asp:HyperLink>
                    &nbsp;|
                    <asp:HyperLink ID="lnk_Step" runat="server" Target="_parent" 
                        NavigateUrl='<%# Eval("WFRowId", "step-events.aspx?rowid={0}") %>'>Step &amp; Events</asp:HyperLink>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Left" Width="350px" />
            </asp:TemplateField>
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
        </asp:View>
        <asp:View ID="View2" runat="server">
            สร้างใหม่<br />
            <br />
            &nbsp;<table width=99%><tr><td width="200px">
                <asp:Label ID="Label3" runat="server" Text="WorkFlowName"></asp:Label>
                </td><td>
                    <asp:TextBox ID="txt_workflowname" runat="server" Width="400px"></asp:TextBox>
                </td></tr><tr><td>
                    <asp:Label ID="Label2" runat="server" Text="SQL MailProfile"></asp:Label>
                    </td><td>
                        <asp:DropDownList ID="sqlmail" runat="server">
                        </asp:DropDownList>
                        &nbsp;<asp:Label ID="Label5" runat="server" 
                            Text="รายการมาจากระบบ Notification Service table : BST_NOTIFICATION.SystemAndMailProfile หากต้องการ Create profile ใหม่กรุณาติดต่อ CKP"></asp:Label>
                    </td></tr>
                    <tr><td>Sender Email Display </td><td>
                        <asp:TextBox ID="txt_sender" runat="server" Width="300px"></asp:TextBox>
                        &nbsp;ตัวอย่าง : BST Workflow &lt;no-reply_webwf@bst.co.th&gt;</td></tr>
                <tr>
                    <td>
                        <asp:Label ID="Label6" runat="server" Text="Customize Application Folder"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txt_applicationfolder" runat="server"></asp:TextBox>
                        &nbsp;<asp:Label ID="Label7" runat="server" 
                            Text="ระบุ customize application folder หากมีการ deverlop นอกเหนือ standard หากไม่มีไม่ต้องระบุ"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        UserView&nbsp;&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txt_userview" runat="server" TextMode="MultiLine" 
                            Width="308px"></asp:TextBox>
                        &nbsp;<font color="red">ไม่ระบุคือ ทุกคนสามารถ View</font>&nbsp;&nbsp;&nbsp;&nbsp; ตัวอย่างการระบุ 
                        Chakrapan_a;doltawat_s</td>
                </tr>
                <tr>
                    <td>
                        UserEdit&nbsp;&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txt_useredit" runat="server" TextMode="MultiLine" 
                            Width="308px"></asp:TextBox>
                        <font color="red">&nbsp;ไม่ระบุคือ ทุกคนสามารถ Edit</font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ตัวอย่างการระบุ 
                        Chakrapan_a;doltawat_s
                    </td>
                </tr>
                <tr>
                    <td>
                        User Can Use Special Action History&nbsp;&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txt_special" runat="server" TextMode="MultiLine" Width="308px"></asp:TextBox>
                        &nbsp;<font color="red">ไม่ระบุคือ ทุกคนสามารถ</font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ตัวอย่างการระบุ 
                        Chakrapan_a;doltawat_s</td>
                </tr>
                <tr>
                    <td>
                        Default Domain</td>
                    <td>
                        <asp:TextBox ID="txt_defaultdomain" runat="server" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
            <br />&nbsp;<br /><br /><asp:HiddenField 
                ID="HiddenField1" runat="server" />
            <br />
            <br />
            <asp:Button ID="btn_save" runat="server" onclick="btn_save_Click" Text="Save" />
            &nbsp;<asp:Button ID="btn_back" runat="server" onclick="btn_back_Click" 
                Text="Back" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_del" runat="server" onclick="btn_del_Click" 
                onclientclick="return confirm('ต้องการลบ?')" Text="Delete" />
            <br />
            <br />
        </asp:View>
        <asp:View ID="View3" runat="server">

            <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium" 
                Font-Underline="True" Text="Embedded Code สามารถเอาไปใช้ใน Guru ได้"></asp:Label>
            <br />
            <br />
            <br />
            Form<br />
            <asp:TextBox ID="emb" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Height="80px" TextMode="MultiLine" Width="90%"></asp:TextBox>
            <br />
            <asp:HyperLink ID="HyperLink2" runat="server" Target="_blank">OpenLink</asp:HyperLink>
            <br />
            <br />
            <br />
            History  >>> Option URL get parameter: distinct=fieldname , where fieldname=value<br />
            <asp:TextBox ID="emb0" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Height="80px" TextMode="MultiLine" Width="90%"></asp:TextBox>
            <br />
            <asp:HyperLink ID="HyperLink3" runat="server" Target="_blank">OpenLink</asp:HyperLink>
            <br />
            <br />
            <br />
            Update email wording / Email position<br />
            <asp:TextBox ID="emb2" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Height="80px" TextMode="MultiLine" Width="90%"></asp:TextBox>
            <br />
            <asp:HyperLink ID="HyperLink6" runat="server" Target="_blank">OpenLink</asp:HyperLink>
            <br />
            <br />
            <br />
            <b>Auto Start Flow by URL Template For GET Method</b><br />
            <asp:TextBox ID="emb1" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                Height="80px" TextMode="MultiLine" Width="90%"></asp:TextBox>
            <br />
            <br />
            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
            <br />
            <br />
            <asp:Literal ID="Literal2" runat="server"></asp:Literal>
            <br />
            <br />
            <asp:Button ID="btn_back1" runat="server" onclick="btn_back_Click" 
                Text="Back" />
            <br />
            <br />

        </asp:View>
        <asp:View ID="View4" runat="server">
            <br />
            ตั้งค่า การแสดงผล History Status&nbsp;
            <asp:HyperLink ID="HyperLink4" runat="server" 
                NavigateUrl="ViewRuningHistory.aspx" Target="_blank">&gt;&gt;&gt; เปิดหน้า History</asp:HyperLink>
            <br />
            <asp:HiddenField ID="HiddenField2" runat="server" />
            <br />
            <asp:Button ID="btn_save1" runat="server" onclick="btn_save0_Click" 
                Text="Save" />
&nbsp;
            <asp:Button ID="btn_back3" runat="server" onclick="btn_back_Click" 
                Text="Back" />
            &nbsp;
            <asp:Label ID="Label9" runat="server" Font-Size="15pt"></asp:Label>
            <br />
            <br />
            <table style="width:100%;">
                <tr>
                    <td bgcolor="#CCCCCC" width="200px">
                        Standard Column</td>
                    <td bgcolor="#CCCCCC" width="150">
                        Show Column</td>
                    <td bgcolor="#CCCCCC">
                        Width</td>
                </tr>
                <tr>
                    <td>
                        View</td>
                    <td>
                        <asp:CheckBox ID="ck_View" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_View_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        History</td>
                    <td>
                        <asp:CheckBox ID="ck_history" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_history_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        Spacial Action</td>
                    <td><asp:CheckBoxList ID="ck_spacialaction" runat="server" 
                            RepeatDirection="Horizontal">
                            <asp:ListItem>Standard</asp:ListItem>
                            <asp:ListItem>Full</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                    <td>
                        <asp:TextBox ID="txt_spacialaction_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        WFName</td>
                    <td>
                        <asp:CheckBox ID="ck_wfname" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_wfname_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        currentstepname</td>
                    <td>
                        <asp:CheckBox ID="ck_currentstepname" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_currentstepname_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td class="style3">
                        Response By</td>
                    <td class="style3">
                        <asp:CheckBox ID="ck_udby" runat="server" />
                    </td>
                    <td class="style3">
                        <asp:TextBox ID="txt_udby_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        Uddate</td>
                    <td>
                        <asp:CheckBox ID="ck_uddate" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_uddate_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        Flow Status</td>
                    <td>
                        <asp:CheckBox ID="ck_flowstatus" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_flowstatus_width" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
            </table>
            <br />
            <br />
            <br />
            <br />
            <table style="width:100%;">
                <tr>
                    <td bgcolor="#CCCCCC" width="100px">
                        ETC Number</td>
                    <td bgcolor="#CCCCCC" width="300">
                        Alias Name</td>
                    <td bgcolor="#CCCCCC" width="100">
                        Show&nbsp; Column</td>
                    <td bgcolor="#CCCCCC">
                        Width</td>
                </tr>
                <tr>
                    <td>
                        ETC1</td>
                    <td>
                        <asp:TextBox ID="txt_etc1" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc1" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width1" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC2</td>
                    <td>
                        <asp:TextBox ID="txt_etc2" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc2" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width2" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC3</td>
                    <td>
                        <asp:TextBox ID="txt_etc3" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc3" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width3" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC4</td>
                    <td>
                        <asp:TextBox ID="txt_etc4" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc4" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width4" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC5</td>
                    <td>
                        <asp:TextBox ID="txt_etc5" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc5" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width5" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC6</td>
                    <td>
                        <asp:TextBox ID="txt_etc6" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc6" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width6" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC7</td>
                    <td>
                        <asp:TextBox ID="txt_etc7" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc7" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width7" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC8</td>
                    <td>
                        <asp:TextBox ID="txt_etc8" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc8" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width8" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC9</td>
                    <td>
                        <asp:TextBox ID="txt_etc9" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc9" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width9" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        ETC10</td>
                    <td>
                        <asp:TextBox ID="txt_etc10" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="ck_etc10" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txt_width10" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;ตัวอย่าง 100</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
            </table>
            <br />
            <asp:Label ID="Label10" runat="server" Text="WHERE Fillter History (SQL)"></asp:Label>
&nbsp;
            <asp:TextBox ID="FilterHistory" runat="server" Width="500px"></asp:TextBox>
&nbsp;<asp:Label ID="Label11" runat="server" 
                Text=" Sample : etc1&lt;&gt;'' and etc2 &lt;&gt; ''"></asp:Label>
            <br />
            <br />
            <br />
            <asp:Label ID="Label8" runat="server" Font-Size="15pt"></asp:Label>
            <br />
            <br />
            <asp:Button ID="btn_save0" runat="server" onclick="btn_save0_Click" 
                Text="Save" />
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_back2" runat="server" onclick="btn_back_Click" 
                Text="Back" />
            <br />
            <br />
        </asp:View>
    </asp:MultiView>

</asp:Content>

