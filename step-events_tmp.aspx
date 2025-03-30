<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="step-events.aspx.cs" Inherits="step_events" %>

<%@ Register assembly="CuteEditor" namespace="CuteEditor" tagprefix="CE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .style3
        {
            height: 17px;
        }
        .style4
        {
   
        }
        
        .tbrow tr
        {
            height:25px;
        }
        .style5
        {
            height: 249px;
        }
        
        .warenone
        {
            display:none;
        }
    </style>
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="WorkFlow Step"></asp:Label>
            <br />
            <br />
            <asp:Label ID="lb_wfname" runat="server" Font-Bold="True" Font-Size="Large" 
                ForeColor="#3333CC" Text="wfname"></asp:Label>
            &nbsp;&nbsp;
    <asp:Button ID="btn_new" runat="server" Text="New Event" onclick="btn_new_Click" />
            &nbsp;<br />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
        CellPadding="3" GridLines="Horizontal" Width="99%" DataKeyNames="StepRowId">
        <AlternatingRowStyle BackColor="#F7F7F7" />
        <Columns>
            <asp:BoundField DataField="StepRowId" HeaderText="StepRowId">
            <ControlStyle Width="1px" />
            <HeaderStyle HorizontalAlign="Left" Width="0px" CssClass="warenone" 
                Wrap="False" />
            <ItemStyle CssClass="warenone" />
            </asp:BoundField>
            <asp:BoundField DataField="Seq" HeaderText="Seq">
            <HeaderStyle HorizontalAlign="Left" Width="35px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="StepName">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("StepName") %>'></asp:Label>
                    &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton2_Click">Edit</asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("StepName") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:BoundField DataField="aftersave" HeaderText="AfterSaveGoto" 
                HtmlEncode="False">
            <HeaderStyle HorizontalAlign="Left" Width="170px" />
            </asp:BoundField>
            <asp:BoundField DataField="afterapprove" HeaderText="AfterApproveGoto" 
                HtmlEncode="False">
            <HeaderStyle HorizontalAlign="Left" Width="180px" />
            </asp:BoundField>
            <asp:BoundField DataField="afterreject" HeaderText="AfterRejectGoto" 
                HtmlEncode="False">
            <HeaderStyle HorizontalAlign="Left" Width="150px" />
            </asp:BoundField>
            <asp:BoundField DataField="DefaultStart" HeaderText="DefaultStart" 
                HtmlEncode="False">
            <HeaderStyle HorizontalAlign="Left" Width="80px" />
            </asp:BoundField>
            <asp:BoundField DataField="DinamicKeyValue" HeaderText="D.KeyValue">
            <HeaderStyle Width="50px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Seting">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">Seup Events</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Left" Width="80px" />
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
            <br />
           <center> 
               <asp:Button ID="btn_back0" runat="server" onclick="btn_back0_Click" Text="Back" 
                Width="113px" /> &nbsp; </center>
            <br />
        </asp:View>
        <asp:View ID="View2" runat="server">
            <asp:Label ID="lb_title1" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="Step &amp; Events"></asp:Label>
            <br />
            <br />
            <asp:Label ID="lb_eventname" runat="server" Text="eventname" Font-Bold="True" 
                Font-Size="Large" ForeColor="#3333CC"></asp:Label><br />
            <asp:Label ID="Label1" runat="server" Text="Step RowID : "></asp:Label>
            <asp:Label ID="rowid_txt" runat="server" Text="Label"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_back2" runat="server" onclick="btn_back_Click" Text="Back" 
                Width="113px" />
            <br /><hr />
            <table width="99%"><tr><td valign="top" class="style4">
            <table width="99%" cellpadding="0" cellspacing="0" border=1 >
            <tr style="background-color:#6699FF"><td  colspan="2"><h2>ตั้งค่าใช้งานปุ่มและ Defaut 
                เริ่ม WF</h2></td></tr>
            <tr><td bgcolor="#E7E7FF" width="220px">EnableSave</td><td bgcolor="#E7E7FF">
                    <table width="100%"><tr><td width="100px"><asp:CheckBox ID="cb_enablesave" runat="server" Text="Enable" 
                    AutoPostBack="True" oncheckedchanged="cb_enablesave_CheckedChanged" /></td><td>
                            AliasName&nbsp;
                            <asp:TextBox ID="AliasNameSave" runat="server" Width="100px"></asp:TextBox>
                            &nbsp;CssStyle .Save</td></tr></table>
                </td></tr>
            <tr><td bgcolor="#E7E7FF">EnableApprove</td><td bgcolor="#E7E7FF">
                <table width="100%"><tr><td width="100px"><asp:CheckBox ID="cb_enableapprove" runat="server" Text="Enable" 
                    AutoPostBack="True" oncheckedchanged="cb_enableapprove_CheckedChanged" /></td><td>
                            AliasName&nbsp;
                            <asp:TextBox ID="AliasNameApprove" runat="server" Width="100px"></asp:TextBox>
                            &nbsp;CssStyle .Approve</td></tr></table>
                </td></tr>
            <tr><td bgcolor="#E7E7FF">EnableReject</td><td bgcolor="#E7E7FF">
                <table width="100%"><tr><td width="100px"><asp:CheckBox ID="cb_enablereject" runat="server" Text="Enable" 
                    oncheckedchanged="cb_enablereject_CheckedChanged" AutoPostBack="True" /></td><td>
                            AliasName&nbsp;
                            <asp:TextBox ID="AliasNameReject" runat="server" Width="100px"></asp:TextBox>
                            &nbsp;CssStyle .Reject</td></tr></table>
                </td></tr>
            <tr><td bgcolor="#E7E7FF">DefaultStart</td><td bgcolor="#E7E7FF">
                <asp:CheckBox ID="cb_defaultstart" runat="server" 
                    Text="Default to 1st Step for Start WF" />
                </td></tr>
                 <tr><td bgcolor="#E7E7FF">Dynamic Key</td><td bgcolor="#E7E7FF">
               
                       <asp:TextBox ID="DinamicKey" runat="server" Width="100px"></asp:TextBox>
               
                </td></tr>
                   <tr><td bgcolor="#E7E7FF">Choose Template</td><td bgcolor="#E7E7FF">
               
                       <asp:DropDownList ID="dl_templateid" runat="server" AutoPostBack="True" 
                           onselectedindexchanged="dl_templateid_SelectedIndexChanged" 
                           BackColor="#FFFF99">
                       </asp:DropDownList>
                         &nbsp;&nbsp;
                       <asp:CheckBox ID="passvalueoldtemplate" runat="server" 
                           Text="PastObjectValue From StepActionBefore" />
               
                </td></tr>
                <tr><td bgcolor="#E7E7FF">Javascript pagestart on-load</td><td bgcolor="#E7E7FF">  <asp:TextBox ID="txt_javaonload" runat="server" TextMode="MultiLine" 
                        Width="98%"></asp:TextBox></td</tr>
                <div runat="server" id="div_eventsave" visible="false">
                  
                <tr style="background-color:#6699FF"><td width="200px" colspan="2"><h2>Event ปุ่ม Save</h2></td></tr>
                <tr><td bgcolor="#E7E7FF" valign="top">EventBeforeSaveCallJavascriptFunction</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_EventBeforeSaveCallJavascriptFunction" runat="server" 
                        TextMode="MultiLine" Width="98%"></asp:TextBox>
                    </td></tr>
            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveMailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterSaveMailToPosition" runat="server" 
                    AutoPostBack="True" 
                    onselectedindexchanged="dl_EventAfterSaveMailToPosition_SelectedIndexChanged" 
                        BackColor="#FFCC66">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="SaveConditionVal" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledSaveValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledSaveValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> <em><br />Example : {Division}
                        ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp; ใส่ , 
                        เพื่อดึงหลายค่าได้ (in)</td></tr>
                </div>


               <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveCC1MailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterSaveCC1MailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                        
                        onselectedindexchanged="dl_EventAfterSaveCC1MailToPosition_SelectedIndexChanged">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="SaveConditionValCC1" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledSaveCC1ValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledSaveCC1ValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> 
                        <em>
                        <br />
                        Example : {Division} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp;&nbsp; 
                        ใส่ , เพื่อดึงหลายค่าได้ (in)</td></tr>
                </div>

                 <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveInformMailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterSaveInformMailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                        
                        
                        onselectedindexchanged="dl_EventAfterSaveInformMailToPosition_SelectedIndexChanged">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="SaveConditionValInform" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledSaveInformValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledSaveInformValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> 
                        <br />
                        <em>Example : {Division} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected 
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in)</td></tr>
                </div>
                 <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveEmailSubject<br /><em  style="color:Gray">สามารถระบุค่าจาก 
                       Object {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterSaveEmailSubject" runat="server"  
                    Width="98%">{sys_workflowname} [{sys_nextstepname}]</asp:TextBox>
                </td></tr>

            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveEmailContent<br /><br /><em  style="color:Gray">สามารถระบุค่าจาก 
                Object
                <br />
                ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterSaveEmailContent" runat="server" Height="200px" TextMode="MultiLine" 
                    Width="98%">Dear All,</asp:TextBox>
                </td></tr>
                
                <div runat="server" id="divSave_informmailcontent" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveInformEmailContent<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterSaveInformEmailContent" runat="server" Height="200px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
                </td></tr>
                </div>
            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveGotoStep</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterSaveGotoStep" runat="server" BackColor="#FFCC66" 
                        onselectedindexchanged="dl_EventAfterSaveGotoStep_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
                    &nbsp; <em>หากไม่ได้เลือก Flow จะจบทันที</em>
                </td></tr>
                <div runat="server" id="stepcondition_save" visible="false">
                    <tr>
                        <td bgcolor="#E7E7FF" valign="top">
                            DynamicFiledSaveValueTostep</td>
                        <td bgcolor="#E7E7FF">
                            <asp:TextBox ID="txt_DinamicFiledSaveValueToStep" runat="server" Width="200px"></asp:TextBox>
                            <em>
                            <br />
                            Example : {Step} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Step : Selected key</em>&nbsp;&nbsp; ใส่ 
                            % เพื่อดึงเป็นช่วงได้ (like)</td>
                    </tr>
                    </div>
            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveExecCommand<br />
                <br />
                <em  style="color:Gray">สามารถระบุค่าจาก Object
                <br />
                ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <br />
                <br />
                <asp:Label ID="Label3" runat="server" Text="Server"></asp:Label>
                &nbsp;<asp:DropDownList ID="dl_EventAfterSaveExecAtServer" runat="server">
                </asp:DropDownList>
                <br />
                <asp:TextBox ID="txt_EventAfterSaveExecCommand" runat="server" Height="150px" 
                    TextMode="MultiLine" Width="98%"></asp:TextBox>
                <br />
                ตัวอย่าง:&nbsp; Update table set field1=&#39;{input1}&#39;,field2=&#39;{input2}&#39; where 
                id={workflowid}<br />
                <br />
                </td></tr>
                </div>
                <div runat="server" id="div_eventapprove" visible="false">
            <tr style="background-color:#6699FF"><td width="200px" colspan="2"><h2>Event ปุ่ม Approve</h2></td></tr>
            <tr><td bgcolor="#E7E7FF" valign="top">EventBeforeApproveCallJavascriptFunction</td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventBeforeApproveCallJavascriptFunction" runat="server" 
                    TextMode="MultiLine" Width="98%"></asp:TextBox>
                </td></tr>
           <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveMailToPosition</td>
               <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterApproveMailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                       onselectedindexchanged="dl_EventAfterApproveMailToPosition_SelectedIndexChanged" 
                       BackColor="#FFCC66">
                </asp:DropDownList>
                </td></tr>
           <div runat="server" id="ApproveConditionVal" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledApproveValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledApproveValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> <em>Example : {Division}
                        <br />
                        ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp;&nbsp; ใส่ , 
                        เพื่อดึงหลายค่าได้ (in)</td></tr>
                </div>

                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveCC1MailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterApproveCC1MailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                        
                        
                        onselectedindexchanged="dl_EventAfterApproveCC1MailToPosition_SelectedIndexChanged">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="ApproveConditionValCC1" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledApproveCC1ValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledApproveCC1ValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> 
                        <br />
                        <em>Example : {Division} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected 
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in)</td></tr>
                </div>

                 <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveInformMailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterApproveInformMailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                        
                        
                        onselectedindexchanged="dl_EventAfterApproveInformMailToPosition_SelectedIndexChanged">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="ApproveConditionValInform" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledApproveInformValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledApproveInformValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> 
                        <br />
                        <em>Example : {Division} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected 
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in)</td></tr>
                </div>

                 <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveEmailSubject<br /><em  style="color:Gray">สามารถระบุค่าจาก 
                       Object {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterApproveEmailSubject" runat="server"  
                    Width="98%">{sys_workflowname} [{sys_nextstepname}]</asp:TextBox>
                </td></tr>

            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveEmailContent<br />
                <br />
                <em  style="color:Gray">สามารถระบุค่าจาก Object
                <br />
                ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterApproveEmailContent" runat="server" 
                    Height="200px" TextMode="MultiLine" 
                    Width="98%">Dear All,</asp:TextBox>
                </td></tr>
                <div runat="server" id="divApprove_informmailcontent" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveInoformEmailContent<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterApproveInformEmailContent" runat="server" 
                        Height="200px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
                </td></tr>
                </div>
            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveGotoStep</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterApproveGotoStep" runat="server" 
                        BackColor="#FFCC66" AutoPostBack="True" 
                        onselectedindexchanged="dl_EventAfterApproveGotoStep_SelectedIndexChanged">
                </asp:DropDownList>&nbsp; <em>หากไม่ได้เลือก Flow จะจบทันที</em>
                </td></tr>
                                <div runat="server" id="stepcondition_approve" visible="false">
                    <tr>
                        <td bgcolor="#E7E7FF" valign="top">
                            DynamicFiledApproveValueTostep</td>
                        <td bgcolor="#E7E7FF">
                            <asp:TextBox ID="txt_DinamicFiledApproveValueToStep" runat="server" 
                                Width="200px"></asp:TextBox>
                            <em>
                            <br />
                            Example : {Step} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Step : Selected key</em>&nbsp;&nbsp;&nbsp; 
                            ใส่ % เพื่อดึงเป็นช่วงได้ (like)</td>
                    </tr>
                    </div>
            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveExecCommand<br />
                <br />
                <em  style="color:Gray">สามารถระบุค่าจาก Object
                <br />
                ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Text="Server"></asp:Label>
                &nbsp;<asp:DropDownList ID="dl_EventAfterApproveExecAtServer" runat="server">
                </asp:DropDownList>
                <br />
                <asp:TextBox ID="txt_EventAfterApproveExecCommand" runat="server" Height="150px" 
                    TextMode="MultiLine" Width="98%"></asp:TextBox>
                <br />
                ตัวอย่าง:&nbsp; Update table set field1=&#39;{input1}&#39;,field2=&#39;{input2}&#39; where 
                id={workflowid}<br />
                <br />
                </td></tr>
                </div>
                <div runat="server" id="div_eventreject" visible="false">
            <tr style="background-color:#6699FF"><td width="200px" colspan="2"><h2 >Event ปุ่ม Reject</h2></td></tr>
            <tr><td bgcolor="#E7E7FF" valign="top">EventBeforeRejectCallJavascriptFunction</td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventBeforeRejectCallJavascriptFunction" runat="server" 
                    TextMode="MultiLine" Width="98%"></asp:TextBox>
                </td></tr>
           <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectMailToPosition</td>
               <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterRejectMailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                       onselectedindexchanged="dl_EventAfterRejectMailToPosition_SelectedIndexChanged" 
                       BackColor="#FFCC66">
                </asp:DropDownList>
                </td></tr>
             <div runat="server" id="RejectConditionVal" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledRejectValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledRejectValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> <em>Example : {Division}
                        <br />
                        ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp;&nbsp; ใส่ , 
                        เพื่อดึงหลายค่าได้ (in)</td></tr>
                </div>


                 <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectCC1MailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterRejectCC1MailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                        
                        
                        onselectedindexchanged="dl_EventAfterRejectCC1MailToPosition_SelectedIndexChanged">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="RejectConditionValCC1" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledRejectCC1ValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledRejectCC1ValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> 
                        <br />
                        <em>Example : {Division} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected 
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in)</td></tr>
                </div>

                 <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectInformMailToPosition</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterRejectInformMailToPosition" runat="server" 
                    AutoPostBack="True" 
                    
                        
                        
                        onselectedindexchanged="dl_EventAfterRejectInformMailToPosition_SelectedIndexChanged">
                </asp:DropDownList>
                </td></tr>
                <div runat="server" id="RejectConditionValInform" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledRejectInformValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledRejectInformValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> 
                        <br />
                        <em>Example : {Division} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected 
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in)</td></tr>
                </div>


                
       <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectEmailSubject<br /><em  style="color:Gray">สามารถระบุค่าจาก 
                       Object {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterRejectEmailSubject" runat="server"  
                    Width="98%">{sys_workflowname} [{sys_currentstepname}]</asp:TextBox>
                </td></tr>


            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectEmailContent<br />
                <br />
                <em  style="color:Gray">สามารถระบุค่าจาก Object
                <br />
                ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterRejectEmailContent" runat="server" 
                    Height="200px" TextMode="MultiLine" 
                    Width="98%">Dear All,</asp:TextBox>
                </td></tr>
                <div runat="server" id="divReject_informmailcontent" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectInformEmailContent<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                <asp:TextBox ID="txt_EventAfterRejectInformEmailContent" runat="server" 
                        Height="200px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
                </td></tr>
                </div>
            <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectGotoStep</td>
                <td bgcolor="#E7E7FF">
                <asp:DropDownList ID="dl_EventAfterRejectGotoStep" runat="server" 
                        BackColor="#FFCC66" AutoPostBack="True" 
                        onselectedindexchanged="dl_EventAfterRejectGotoStep_SelectedIndexChanged">
                </asp:DropDownList>&nbsp; <em>หากไม่ได้เลือก Flow จะจบทันที</em>
                </td></tr>
                                <div runat="server" id="stepcondition_reject" visible="false">
                    <tr>
                        <td bgcolor="#E7E7FF" valign="top">
                            DynamicFiledRejectValueTostep</td>
                        <td bgcolor="#E7E7FF">
                            <asp:TextBox ID="txt_DinamicFiledRejectValueToStep" runat="server" 
                                Width="200px"></asp:TextBox>
                            <em>
                            <br />
                            Example : {Step} ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Step : Selected key</em>&nbsp;&nbsp;&nbsp; 
                            ใส่ % เพื่อดึงเป็นช่วงได้ (like)</td>
                    </tr>
                    </div>
            <tr><td bgcolor="#E7E7FF" valign="top" class="style5">EventAfterRejectExecCommand<br />
                <br />
                <em style="color:Gray">สามารถระบุค่าจาก Object
                <br />
                ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF" class="style5">
                <br />
                <br />
                <asp:Label ID="Label4" runat="server" Text="Server"></asp:Label>
                &nbsp;<asp:DropDownList ID="dl_EventAfterRejectExecAtServer" runat="server">
                </asp:DropDownList>
                <br />
                <asp:TextBox ID="txt_EventAfterRejectExecCommand" runat="server" Height="150px" 
                    TextMode="MultiLine" Width="98%"></asp:TextBox>
                <br />
                ตัวอย่าง:&nbsp; Update table set field1=&#39;{input1}&#39;,field2=&#39;{input2}&#39; where 
                id={workflowid}<br />
                <br />
                </td></tr>
                </div>

                 <div runat="server" id="guideline" >
            <tr style="background-color:#6699FF"><td width="200px" colspan="2"><h2 >Guideline</h2></td></tr>
            <tr><td bgcolor="#E7E7FF" valign="top">Guideline Message<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">
                    <CE:Editor ID="FreeTextBox1" runat="server" 
                        CodeViewTemplateItemList="Cut,Copy,Paste,Find,ToFullPage,FromFullPage,SelectAll,SelectNone" 
                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="False" ShowDecreaseButton="False" 
                        ShowEnlargeButton="False" ShowTagSelector="False" ShowToolBar="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="1000"></CE:Editor>
                </td></tr>
            </div>

            </table>
            </td><td width="500px" style="background-color:#E7E7FF" valign="top">
                    <asp:Label ID="Label5" runat="server" 
                        Text="Enable Input : กำหนดเปิด Control ให้ Key ค่าได้"></asp:Label>
                    <br />
                    <hr />
                    <table style="width:100%;padding:2px"><tr style="font-weight:bold">
                        <td class="style3" bgcolor="#0066FF">
                        Select Enable Object</td>
                        <td class="style3" bgcolor="#0066FF">Require Filed</td><td class="style3" 
                            bgcolor="#0066FF">DataBinding</td><td class="style3" bgcolor="#0066FF">Default Value</td></tr>
                        <tr><td valign="top"><asp:CheckBoxList ID="cb_fieldlist" runat="server" 
                                CssClass="tbrow">
                    </asp:CheckBoxList></td><td valign="top">
                                <asp:CheckBoxList ID="cb_fieldrequire" runat="server" CssClass="tbrow">
                                </asp:CheckBoxList>
                            </td><td valign="top">
                                <asp:Table ID="Table1" runat="server" CssClass="tbrow" Width="99%">
                                    <asp:TableRow runat="server">
                                        <asp:TableCell runat="server"></asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                            </td><td valign="top">
                                   <asp:Table ID="Table2" runat="server" CssClass="tbrow" Width="99%">
                                    <asp:TableRow ID="TableRow1" runat="server">
                                        <asp:TableCell ID="TableCell1" runat="server"></asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </td></tr>
                    
                    </table>
                    <br />
                    <asp:Label ID="Label7" runat="server" Text="Enable ตัวจัดการ Upload file"></asp:Label>
                    <hr />
       
                    <table style="width:100%;padding:2px"><tr style="font-weight:bold">
                        <td class="style3" bgcolor="#0066FF">
                            Upload manager</td>
                        <td class="style3" bgcolor="#0066FF">Allow Option</td></tr>
                        <tr><td>
                            <asp:CheckBox ID="cb_uploadmgr" runat="server" Text="Enable" 
                                AutoPostBack="True" oncheckedchanged="cb_uploadmgr_CheckedChanged" />
                            &nbsp;Use</td><td>
                                <asp:CheckBoxList ID="cb_uploadot" runat="server" CssClass="tbrow" 
                                    Enabled="False">
                                    <asp:ListItem>Enable Upload More File</asp:ListItem>
                                    <asp:ListItem>Enable Delete File</asp:ListItem>
                                    <asp:ListItem Value="Auto Expand Content">Auto Expand Content</asp:ListItem>
                                </asp:CheckBoxList>
                            </td></tr>
                            </table>
                    <asp:Label ID="Label8" runat="server" 
                        Text="** การตั้งค่านี้จะไม่ทำงานถ้า Step นี้ถูก Default to 1st Step for Start WF" 
                        Font-Italic="True" ForeColor="#999999"></asp:Label>
                    <br />
                    <hr />
                    System variable<br />
                    <asp:TextBox ID="TextBox2" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="204px" ReadOnly="True" TextMode="MultiLine" Width="99%">{sys_date} =วันที่ขณะนั้น
{sys_time}  = เวลาขณะนั้น
{sys_username}  = ชื่อ Logon
{sys_workflowid}  = RowId ของ Workflow
{sys_startworkflowid}  = RowId ของการ Start WF
{sys_referenceeventhistory}  =RowId ของ Event hitory
{sys_workflowname}  =  ชื่อ  Workflow 
{sys_currentstepname}  = ชื่อ Step
{sys_nextstepname}  = ชื่อ Step ต่อไป 
{MailTo} = Email ของ MailTo
{CCMailTo} = Email ของ CCMailTo
SQL{command} = Call SQL command ที่ return ค่าเดียว</asp:TextBox>
                    <br />
                    <br />
                    <center>
                    <asp:Button ID="btn_preview" runat="server" Text="Preview Template" /></center>
                </td></tr></table>

                <hr />
                <center>
                    <br />
            <asp:Button ID="btn_save" runat="server" Text="Save" onclick="btn_save_Click" 
                        Width="108px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btn_back" runat="server" onclick="btn_back_Click" Text="Back" 
                        Width="113px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <input id="btn_howto" type="button" value="Advance guidline"  onclick="window.open('advance.aspx')" />
                    <br />
                    <br />
                    <asp:Label ID="lb_message" runat="server" Text="Message" Visible="False" 
                        Font-Bold="True" Font-Size="Medium"></asp:Label>
                    <br />
                    <br />
            </center>
        </asp:View>
        <asp:View ID="View3" runat="server">
            สร้างใหม่<br />
            <br />
            <asp:Label ID="Label6" runat="server" Text="EventName"></asp:Label>
            &nbsp;&nbsp;
            <asp:TextBox ID="txt_eventname" runat="server" Width="600px"></asp:TextBox>
            <br />
            Seq&nbsp;<asp:TextBox ID="txt_seq" runat="server"></asp:TextBox>
            <br />
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <br />
            <br />
            <asp:Button ID="btn_save0" runat="server" onclick="btn_save0_Click" 
                Text="Save" />
            &nbsp;
            <asp:Button ID="btn_back1" runat="server" onclick="btn_back_Click" 
                Text="Back" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_del" runat="server" onclick="btn_del_Click" 
                onclientclick="return confirm('ต้องการลบ?')" Text="Delete" />
        </asp:View>
    </asp:MultiView>
    </asp:Content>

