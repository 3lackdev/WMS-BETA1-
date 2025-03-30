<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="step-events.aspx.cs" Inherits="step_events" %>

<%@ Register assembly="CuteEditor" namespace="CuteEditor" tagprefix="CE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

<LINK href="codemirror/codemirror.css" rel="stylesheet">
<LINK href="codemirror/fullscreen.css" rel="stylesheet">
<LINK href="codemirror/simplescrollbars.css" rel="stylesheet">
<SCRIPT src="codemirror/codemirror.js"></SCRIPT>
<SCRIPT src="codemirror/javascript.js"></SCRIPT>
<SCRIPT src="codemirror/fullscreen.js"></SCRIPT>
<SCRIPT src="codemirror/simplescrollbars.js"></SCRIPT>


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
        

          #overlay {
    position: fixed;
    display: none;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: #cccccc;
    z-index: 10;
    cursor: pointer;
}
.textover
{
  position: absolute;
    z-index:1000000 ;
    right:10px;
    width:900px !important;

}

    </style>

    <script>
        var currentobj = "";
        function on(obj) {
            try {
                document.getElementById("overlay").style.display = "block";
                currentobj = obj.id;
                $('#' + obj.id).addClass('textover');
                $('#' + obj.id).focus();
                $('#' + obj.id).val($('#' + obj.id).val());
            } catch (ex) { }
        }

        function off(obj) {
            try {
                document.getElementById("overlay").style.display = "none";
                $('#' + currentobj).removeClass('textover');
                currentobj = '';
                $('#' + currentobj).css('width', '');
            } catch (ex) { }
        }

        $(document).keypress(function (e) {
            if (e.which == 13) {
   
                if (e.target.name.toString().indexOf("find_txt") >= 0) {
                    $("#find_field").click();
                    return false;
                }


                if (e.target.name.toString().indexOf("txt_tt") >= 0) {
                    $("#ContentPlaceHolder1_ss").click();
                    $("#ContentPlaceHolder1_ss").focus();
                    return false;
                }
            }
        });

        $(document).keydown(function (event) {

            //19 for Mac Command+S
            if (!(String.fromCharCode(event.which).toLowerCase() == 's' && event.ctrlKey) && !(event.which == 19)) return true;

            try {
                
                if ($("#ContentPlaceHolder1_btn_save1").val() == 'Save' || $("#ContentPlaceHolder1_btn_save1").val() == 'Save All') {
                    $("#ContentPlaceHolder1_btn_save1").focus();
                    $("#ContentPlaceHolder1_btn_save1").click();
                }
            } catch (ex) {  }

            event.preventDefault();
            return false;
        });



        function hilightclass(obj) {
            
            if (obj!="") {
                $("." + obj).css("background-color", "gray");
                $("." + obj).css("color", "#ffffff");
            }
        }

        function clarehilightclass(obj) {
            if (obj != "") {
                $("." + obj).css("background-color", "");
                $("." + obj).css("color", "");
            }


        }


        function findfield() {

            var txt = $('#ContentPlaceHolder1_find_txt').val();



         //   $('.tbrow').each(function () {
         //       $(this).children().children().show();
          //  });

            //input[type=checkbox]
            if (txt != '') {
                var i = 0;
                $('#ContentPlaceHolder1_cb_fieldlist label').each(function () {
                    $(this).parent().parent().show();
                    $('#ContentPlaceHolder1_cb_fieldrequire_' + i).parent().parent().show();
                    if ($(this).text().indexOf(txt) <= 0) {
                        $(this).parent().parent().hide();
                        $('#ContentPlaceHolder1_cb_fieldrequire_' + i).parent().parent().hide();
                        try {
                            $('#ContentPlaceHolder1_Table1 tr').eq(i).hide();
                            $('#ContentPlaceHolder1_default-' + i).parent().parent().hide();
                        } catch (ex) { }


                    }
                    i++;
                });

            }
            else {
                   $('.tbrow').each(function () {
                       $(this).children().children().show();
                  });
            }








           }

           function ckall() {
               
               $('#ContentPlaceHolder1_cb_fieldlist input[type=checkbox]').each(function () {
                   if ($(this).parent().parent().css('DISPLAY') != 'none') {
                       $(this).prop('checked', 'checked');
                   }
               });
          }

    </script>
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="WorkFlow Step"></asp:Label>
            <br />
            <br />
            <table width=98%><tr><td><asp:Label ID="lb_wfname" runat="server" Font-Bold="True" Font-Size="Large" 
                ForeColor="#3333CC" Text="wfname" ></asp:Label>
            &nbsp;&nbsp; <asp:Button ID="btn_new" runat="server" Text="New Event" 
                    onclick="btn_new_Click" BackColor="#99CC00" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_back3" runat="server" onclick="btn_back0_Click" Text="Back" 
                    Width="113px" />
                </td><td width=250 > <asp:TextBox ID="txt_tt" runat="server" TabIndex="100"></asp:TextBox>&nbsp;<asp:Button 
                        runat="server" Text="Search" ID="ss" onclick="Unnamed1_Click" TabIndex="101"></asp:Button></td></tr></table>
            
   
               
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
        CellPadding="3" GridLines="Horizontal" Width="99%" 
                DataKeyNames="StepRowId,StepName">
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
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("StepName") %>' 
                        CssClass='<%# Eval("StepRowId") %>' onmouseover="hilightclass(this.className);" onmouseout="clarehilightclass(this.className);" ></asp:Label>
                    &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton2_Click" Text="Edit" 
                       ></asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton ID="LinkButton3" runat="server" onclick="LinkButton3_Click" 
                        onclientclick="return confirm('Confirm copy?');" Text="Copy"></asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("StepName") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AfterSaveGoto">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("aftersave") %>' CssClass='<%# Eval("StepRowIdAfterSave") %>' onmouseover="hilightclass(this.className);" onmouseout="clarehilightclass(this.className);"></asp:Label>
                    <asp:Label ID="Label17" runat="server" Font-Italic="True" ForeColor="#3333FF" 
                        Text='<%# Bind("callstartsave") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("aftersave") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" Width="170px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AfterApproveGoto">
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("afterapprove") %>' CssClass='<%# Eval("StepRowIdAfterApprove") %>' onmouseover="hilightclass(this.className);" onmouseout="clarehilightclass(this.className);"></asp:Label>
                    <asp:Label ID="Label18" runat="server" Font-Italic="True" ForeColor="#3333FF" 
                        Text='<%# Bind("callstartapprove") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("afterapprove") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" Width="180px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AfterRejectGoto">
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("afterreject") %>' CssClass='<%# Eval("StepRowIdAfterReject") %>' onmouseover="hilightclass(this.className);" onmouseout="clarehilightclass(this.className);"></asp:Label>
                    <asp:Label ID="Label19" runat="server" Font-Italic="True" ForeColor="#3333FF" 
                        Text='<%# Bind("callstartreject") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("afterreject") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Left" Width="150px" />
            </asp:TemplateField>
            <asp:BoundField DataField="DefaultStart" HeaderText="DefaultStart" 
                HtmlEncode="False">
            <HeaderStyle HorizontalAlign="Left" Width="80px" />
            </asp:BoundField>
            <asp:BoundField DataField="DinamicKeyValue" HeaderText="D.KeyValue">
            <HeaderStyle Width="50px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Seting">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click" 
                        CssClass='<%# "click-"+Eval("StepRowId") %>'>Seup Events</asp:LinkButton>
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
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_save1" runat="server" onclick="btn_save_Click" Text="Save All" 
                Width="108px" />
            &nbsp;&nbsp;&nbsp;<asp:Button ID="btn_save2" runat="server" onclick="btn_save_Click" 
                Text="Save leftpart only" Width="108px" />
            &nbsp;&nbsp;
            <asp:Label ID="lb_message0" runat="server" Font-Bold="True" Font-Size="Medium" 
                Text="Message" Visible="False"></asp:Label>
            <br />
            <hr />
            <table width="99%"><tr><td valign="top" class="style4">
            <table width="99%" cellpadding="0" cellspacing="0" border=1 >
            <tr style="background-color:#6699FF"><td  colspan="2"><h2>ตั้งค่าใช้งานปุ่มและ Defaut 
                เริ่ม WF</h2></td></tr>
            <tr><td bgcolor="#E7E7FF" width="220px">EnableSave&nbsp;&nbsp;
                <font style="color:Gray;width:100px">(CssStyle .Save)</font></td><td bgcolor="#E7E7FF">
                    <table width="100%"><tr><td width="80px"><asp:CheckBox ID="cb_enablesave" runat="server" Text="Enable" 
                    AutoPostBack="True" oncheckedchanged="cb_enablesave_CheckedChanged" />
                        </td><td>
                            AliasName
                            <br />
                            <asp:TextBox ID="AliasNameSave" runat="server" Width="100px"></asp:TextBox>
                            </td><td>
                            <asp:Label ID="Label9" runat="server" Text="Trigger"></asp:Label>
                            &nbsp;condition (Auto Press)<br />
                            <asp:TextBox ID="TiggerSave" runat="server" Width="170px"></asp:TextBox>
                            <asp:CheckBox ID="HideButtonSave" runat="server" Text="HideBtn" />
                        </td></tr></table>
                </td></tr>
            <tr><td bgcolor="#E7E7FF">EnableApprove&nbsp; <font style="color:Gray;width:100px">
                (CssStyle .Approve)</font></td><td bgcolor="#E7E7FF">
                <table width="100%"><tr><td width="80px"><asp:CheckBox ID="cb_enableapprove" runat="server" Text="Enable" 
                    AutoPostBack="True" oncheckedchanged="cb_enableapprove_CheckedChanged" />&nbsp;</td><td>
                            AliasName
                            <br />
                            <asp:TextBox ID="AliasNameApprove" runat="server" Width="100px"></asp:TextBox>
                            </td><td>
                        <asp:Label ID="Label12" runat="server" Text="Trigger"></asp:Label>
                        &nbsp;condition (Auto Press)<br />
                        <asp:TextBox ID="TiggerApprove" runat="server" Width="170px"></asp:TextBox>
                        <asp:CheckBox ID="HideButtonApprove" runat="server" Text="HideBtn" />
                    </td></tr></table>
                </td></tr>
            <tr><td bgcolor="#E7E7FF">EnableReject&nbsp; <font style="color:Gray;width:100px">
                (CssStyle .Reject)</font></td><td bgcolor="#E7E7FF">
                <table width="100%"><tr><td width="80px"><asp:CheckBox ID="cb_enablereject" runat="server" Text="Enable" 
                    oncheckedchanged="cb_enablereject_CheckedChanged" AutoPostBack="True" />&nbsp;</td><td>
                            AliasName&nbsp;
                            <br />
                            <asp:TextBox ID="AliasNameReject" runat="server" Width="100px"></asp:TextBox>
                            </td><td>
                        <asp:Label ID="Label13" runat="server" Text="Trigger"></asp:Label>
                        &nbsp;condition (Auto Press)<br />
                        <asp:TextBox ID="TiggerReject" runat="server" Width="170px"></asp:TextBox>
                        <asp:CheckBox ID="HideButtonReject" runat="server" Text="HideBtn" />
                    </td></tr></table>
                </td></tr>
            <tr><td bgcolor="#E7E7FF">DefaultStart</td><td bgcolor="#E7E7FF">
                <asp:CheckBox ID="cb_defaultstart" runat="server" 
                    Text="Default to 1st Step for Start WF" />
                </td></tr>
                 <tr><td bgcolor="#E7E7FF">Dynamic Key</td><td bgcolor="#E7E7FF" height="25">
               
                       <asp:TextBox ID="DinamicKey" runat="server" Width="150px"></asp:TextBox>
               
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
                <tr><td bgcolor="#E7E7FF">StepAutoStart </td><td bgcolor="#E7E7FF">
                    <asp:RadioButtonList ID="rb_stepautostart" runat="server" 
                        onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" 
                        AutoPostBack="True">
                        <asp:ListItem Selected="True">Not Use</asp:ListItem>
                        <asp:ListItem Value="continue">Continue main WF from this step to when child WF Finished (Resume)</asp:ListItem>
                        <asp:ListItem Value="condition">AutoStart to be child WF from this step by Condition</asp:ListItem>
                    </asp:RadioButtonList>
                    </td></tr>
  <div runat="server" id="div_TriggerStartStepByCondition" visible="false"> <tr><td bgcolor="#E7E7FF">TriggerStartStepByCondition<em><br />
      <br />
      Example : {Division}=&#39;MT&#39;</em></td><td bgcolor="#E7E7FF" 
                         height="25px">
       <br />
                       <asp:TextBox ID="txt_TiggerStartStepCondition" runat="server" 
              Width="250px" BackColor="#FFFF99"></asp:TextBox>
               
                       &nbsp;<em>&nbsp;&nbsp; Auto 
       <asp:Label ID="Label14" runat="server" Text="Press Button"></asp:Label>
       &nbsp;&nbsp;
       <asp:DropDownList ID="dl_TiggerStartStepConditionButton" runat="server" 
              BackColor="#FFFF99">
           <asp:ListItem>------</asp:ListItem>
           <asp:ListItem>Save</asp:ListItem>
           <asp:ListItem>Approve</asp:ListItem>
           <asp:ListItem>Reject</asp:ListItem>
       </asp:DropDownList>
       <br />
       <br />
       ใช้กรณีให้ start อัตโนมัตเป็น Flow ย่อยขึ้นมา run ขนานกับ Flow 
       หลักที่คนละTemplate&nbsp; </em>
               
                </td></tr></div>
               <div runat="server" id="div_TriggerStartStepByCondition2" visible="false"> 
                <tr bgcolor="#E7E7FF"><td style="height: 30px"><em>Auto
                    <asp:Label ID="Label16" runat="server" Text="Press Button"></asp:Label>
                    &nbsp;to continue</em></td><td><em>Auto
                    <asp:Label ID="Label15" runat="server" Text="Press Button"></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="dl_TiggerStartStepConditionButton0" runat="server" 
                            BackColor="#FFFF99">
                        <asp:ListItem>------</asp:ListItem>
                        <asp:ListItem>Save</asp:ListItem>
                        <asp:ListItem>Approve</asp:ListItem>
                        <asp:ListItem>Reject</asp:ListItem>
                    </asp:DropDownList>
                    </em></td></tr></div>

                <tr><td bgcolor="#E7E7FF" valign="top">
                    <br />
                    Javascript pagestart on-load<br />
                    <br />
                    Fullscreen press F11/Esc&nbsp;&nbsp;&nbsp; 
                    <br />
                    <br />
                    &nbsp;
                    <asp:HyperLink ID="HyperLink1" runat="server" BackColor="#FFCC66" 
                        ForeColor="#003366" NavigateUrl="https://www.w3schools.com/jquery/" 
                        Target="_blank">Manual Jquery</asp:HyperLink>
                    <br />
                    </td><td bgcolor="#E7E7FF">  <asp:TextBox ID="txt_javaonload" runat="server" TextMode="MultiLine" 
                        Width="98%"></asp:TextBox>
                        
                        &nbsp;</td</tr><div runat="server" id="div_eventsave" visible="false">
                  
                <tr style="background-color:#6699FF"><td width="200px" colspan="2">
                    <table width="700px">
                        <tr>
                            <td width=200>
                                <h2>
                                    Event ปุ่ม Save</h2>
                            </td>
                            <td>
                                &nbsp;&nbsp;&nbsp;
                                <asp:CheckBox ID="CheckTiggerSave" runat="server" 
                                    Text="Check Trigger&nbsp;condition (Auto Press)" />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:CheckBox ID="CheckConditionAutoStartSave" runat="server" 
                                    Text="Check Condition AutoStart Child WF" />
                            </td>
                        </tr>
                    </table>
                    </td></tr>
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
                    &nbsp;<asp:CheckBox ID="ck_Save_dont_send" runat="server" Text="Don't Send Email" />
                </td></tr>
                <div runat="server" id="SaveConditionVal" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledSaveValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledSaveValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> <em><br />Example : {Division}
                        ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp; ใส่ , 
                        เพื่อดึงหลายค่าได้ (in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
                        ใส่ , เพื่อดึงหลายค่าได้ (in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
               
<CE:Editor ID="txt_EventAfterSaveEmailContent" runat="server" 

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="10" 
                        
                        TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                        Text="Dear All,">
</CE:Editor>

                </td></tr>
                
                <div runat="server" id="divSave_informmailcontent" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveInformEmailContent<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">

                    <CE:Editor ID="txt_EventAfterSaveInformEmailContent" runat="server" 

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="10" 
                        
                        TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                        Text="">
                    </CE:Editor>
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
                     <tr><td bgcolor="#E7E7FF" valign="top">EventAfterSaveUpdateValueETC<br /><em  style="color:Gray">สามารถระบุค่าจาก 
                       Object {ชื่อ Filed}<br />
                         <br />
                         System view show on
                         <br />
                         history status page</em></td>
                <td bgcolor="#E7E7FF">
             
                    <br />
                    <table style="width:100%;">
                        <tr>
                            <td bgcolor="#CCCCCC" width="100">
                                ETC Number</td>
                            <td bgcolor="#CCCCCC">
                                Value</td>
                        </tr>
                        <tr>
                            <td>
                                ETC1</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterSaveUpdateValueETC1" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC2</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterSaveUpdateValueETC2" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC3</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterSaveUpdateValueETC3" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC4</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterSaveUpdateValueETC4" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC5</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterSaveUpdateValueETC5" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                    </table>
             
                </td></tr>
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
            <tr style="background-color:#6699FF"><td width="200px" colspan="2">
               <table width="700px">
                        <tr>
                            <td width=200>
                            <h2>
                                Event ปุ่ม Approve</h2>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="CheckTiggerApprove" runat="server" 
                                Text="Check Trigger&nbsp;condition (Auto Press)" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:CheckBox ID="CheckConditionAutoStartApprove" runat="server" 
                                    Text="Check Condition AutoStart Child WF" />
                        </td>
                    </tr>
                </table>
                </td></tr>
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
                   &nbsp;<asp:CheckBox ID="ck_Approve_dont_send" runat="server" 
                       Text="Don't Send Email" />
                </td></tr>
           <div runat="server" id="ApproveConditionVal" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledApproveValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledApproveValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> <em>Example : {Division}
                        <br />
                        ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp;&nbsp; ใส่ , 
                        เพื่อดึงหลายค่าได้ (in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
  
                        <CE:Editor ID="txt_EventAfterApproveEmailContent" runat="server" 

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="10" 
                        
                            TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                            Text="Dear All,">
                    </CE:Editor>
                </td></tr>
                <div runat="server" id="divApprove_informmailcontent" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveInoformEmailContent<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">

                      <CE:Editor ID="txt_EventAfterApproveInformEmailContent" runat="server" 

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="10" 
                        
                          TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                          Text="">
                    </CE:Editor>

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
                     <tr><td bgcolor="#E7E7FF" valign="top">EventAfterApproveUpdateValueETC<br /><em  style="color:Gray">สามารถระบุค่าจาก 
                       Object {ชื่อ Filed}<br />
                         <br />
                         System view show on
                         <br />
                         history status page</em></td>
                <td bgcolor="#E7E7FF">
             
                    <br />
                    <table style="width:100%;">
                        <tr>
                            <td bgcolor="#CCCCCC" width="100">
                                ETC Number</td>
                            <td bgcolor="#CCCCCC">
                                Value</td>
                        </tr>
                        <tr>
                            <td>
                                ETC1</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterApproveUpdateValueETC1" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC2</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterApproveUpdateValueETC2" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC3</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterApproveUpdateValueETC3" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC4</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterApproveUpdateValueETC4" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC5</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterApproveUpdateValueETC5" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                    </table>
             
                </td></tr>
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
            <tr style="background-color:#6699FF"><td width="200px" colspan="2">
                <table width="700px">
                    <tr>
                        <td width="200px">
                            <h2>
                                Event ปุ่ม Reject</h2>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="CheckTiggerReject" runat="server" 
                                Text="Check Trigger&nbsp;condition (Auto Press)" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:CheckBox ID="CheckConditionAutoStartReject" runat="server" 
                                    Text="Check Condition AutoStart Child WF" />
                        </td>
                    </tr>
                </table>
                </td></tr>
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
                   &nbsp;<asp:CheckBox ID="ck_Reject_dont_send" runat="server" 
                       Text="Don't Send Email" />
                </td></tr>
             <div runat="server" id="RejectConditionVal" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">DynamicFiledRejectValueToselect</td>
                    <td bgcolor="#E7E7FF">
                    <asp:TextBox ID="txt_DinamicFiledRejectValueToselect" runat="server" 
                        Width="200px"></asp:TextBox> <em>Example : {Division}
                        <br />
                        ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>&nbsp;&nbsp; ใส่ , 
                        เพื่อดึงหลายค่าได้ (in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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
                        key</em>&nbsp;&nbsp; ใส่ , เพื่อดึงหลายค่าได้(in) or ใส่ % เพื่อเลือกค่า (like)</td></tr>
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

                         <CE:Editor ID="txt_EventAfterRejectEmailContent" runat="server" 

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="10" 
                        
                             TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                             Text="Dear All,">
                    </CE:Editor>


                </td></tr>
                <div runat="server" id="divReject_informmailcontent" visible="false">
                <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectInformEmailContent<br />
                    <br />
                    <em  style="color:Gray">สามารถระบุค่าจาก Object
                    <br />
                    ได้โดยใส่ {ชื่อ Filed}</em></td>
                <td bgcolor="#E7E7FF">

                       <CE:Editor ID="txt_EventAfterRejectInformEmailContent" runat="server" 

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="10" 
                        
                           TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                           Text="">
                    </CE:Editor>

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
                     <tr><td bgcolor="#E7E7FF" valign="top">EventAfterRejectUpdateValueETC<br /><em  style="color:Gray">สามารถระบุค่าจาก 
                       Object {ชื่อ Filed}<br />
                         <br />
                         System view show on
                         <br />
                         history status page</em></td>
                <td bgcolor="#E7E7FF">
             
                    <br />
                    <table style="width:100%;">
                        <tr>
                            <td bgcolor="#CCCCCC" width="100">
                                ETC Number</td>
                            <td bgcolor="#CCCCCC">
                                Value</td>
                        </tr>
                        <tr>
                            <td>
                                ETC1</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterRejectUpdateValueETC1" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC2</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterRejectUpdateValueETC2" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC3</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterRejectUpdateValueETC3" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC4</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterRejectUpdateValueETC4" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                ETC5</td>
                            <td>
                                <asp:TextBox ID="txt_EventAfterRejectUpdateValueETC5" runat="server" 
                                    Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                    </table>
             
                </td></tr>
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

                        EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                        EnableStripStyleTagsCodeInjection="False" Height="300px" 
                        ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                        StyleWithCSS="True" Width="99%" TabSpaces="1000" 
                        TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu"></CE:Editor>
                </td></tr>
            </div>

            </table>
            </td><td width="500px" style="background-color:#E7E7FF" valign="top">
                    <asp:TextBox ID="find_txt" runat="server" BorderStyle="Dotted" 
                        BorderWidth="1px" Width="70px"></asp:TextBox>
&nbsp;<input id="find_field" type="button" value="Find" style="height: 20px" onclick="findfield()" />&nbsp;<asp:Label 
                        ID="Label5" runat="server" 
                        Text="Enable Input : กำหนดเปิด Control ให้ Key ค่าได้"></asp:Label>
                    <br />
                    <hr />
                    
                    <table style="width:100%;padding:2px">
                        <tr style="font-weight:bold">
                            <td bgcolor="#0066FF" class="style3">
                                <a  onclick="ckall()" style="cursor:pointer">Select Enable Object</a></td>
                            <td bgcolor="#0066FF" class="style3">
                                Require Filed</td>
                            <td bgcolor="#0066FF" class="style3">
                                DataBinding</td>
                            <td bgcolor="#0066FF" width="50px">
                                Default Value</td>
                                <td bgcolor="#0066FF" class="style3" title="Checkbox หากต้องการใส่ใส่ค่า default ตอนกดปุ่ม Action Save/Approve/Reject">
                                 E.Af</td>
                             
                        </tr>
                        <tr>
                            <td valign="top">
                                <asp:CheckBoxList ID="cb_fieldlist" runat="server" CssClass="tbrow">
                                </asp:CheckBoxList>
                            </td>
                            <td valign="top">
                                <asp:CheckBoxList ID="cb_fieldrequire" runat="server" CssClass="tbrow">
                                </asp:CheckBoxList>
                            </td>
                            <td valign="top">
                                <asp:Table ID="Table1" runat="server" CssClass="tbrow" Width="99%">
                                    <asp:TableRow runat="server">
                                        <asp:TableCell runat="server"></asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                            </td>
                            <td valign="top">
                                <asp:Table ID="Table2" runat="server" CssClass="tbrow" Width="99%">
                                    <asp:TableRow ID="TableRow1" runat="server">
                                        <asp:TableCell ID="TableCell1" runat="server"></asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </td>
                            <td valign="top">
                                <asp:CheckBoxList ID="cb_fieldafter" runat="server" CssClass="tbrow">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Label ID="Label7" runat="server" Text="Enable ตัวจัดการ Upload file"></asp:Label>
                    <hr />
                    <table style="width:100%;padding:2px">
                        <tr style="font-weight:bold">
                            <td bgcolor="#0066FF" class="style3">
                                Upload manager</td>
                            <td bgcolor="#0066FF" class="style3">
                                Allow Option</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cb_uploadmgr" runat="server" AutoPostBack="True" 
                                    oncheckedchanged="cb_uploadmgr_CheckedChanged" Text="Enable" />
                                &nbsp;Use</td>
                            <td>
                                <asp:CheckBoxList ID="cb_uploadot" runat="server" CssClass="tbrow" 
                                    Enabled="False">
                                    <asp:ListItem>Enable Upload More File</asp:ListItem>
                                    <asp:ListItem>Enable Delete File</asp:ListItem>
                                    <asp:ListItem Value="Auto Expand Content">Auto Expand Content</asp:ListItem>
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="Label8" runat="server" Font-Italic="True" ForeColor="#999999" 
                        Text="** การตั้งค่านี้จะไม่ทำงานถ้า Step นี้ถูก Default to 1st Step for Start WF"></asp:Label>
                    <br />
                    <hr />
                    System variable<br />
                    <asp:TextBox ID="TextBox2" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="204px" ReadOnly="True" TextMode="MultiLine" Width="99%">{sys_date} =วันที่ขณะนั้น
{sys_time}  = เวลาขณะนั้น
{sys_username}  = ชื่อ Logon
{sys_logonemail} = get Email คนที่ logon
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
            <table width=95%>
            <tr><td><asp:Label ID="Label6" runat="server" Text="EventName"></asp:Label></td><td><asp:TextBox ID="txt_eventname" runat="server" Width="600px"></asp:TextBox></td></tr>
            <tr><td> Seq</td><td><asp:TextBox ID="txt_seq" runat="server"></asp:TextBox></td></tr>
            <tr><td> Group Name</td><td><asp:TextBox ID="txt_group" runat="server" 
                    Width="200px"></asp:TextBox></td></tr>
            </table>
            
  
            <br />
           &nbsp;
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
    <div id="overlay" onclick="off(this)">

</div>
    </asp:Content>

