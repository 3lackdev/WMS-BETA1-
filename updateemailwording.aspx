<%@ Page Language="C#" AutoEventWireup="true" CodeFile="updateemailwording.aspx.cs" Inherits="updateemailwording" %>

<%@ Register assembly="CuteEditor" namespace="CuteEditor" tagprefix="CE" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="Css/layout.css" rel="stylesheet" type="text/css" />
     <LINK href="codemirror/codemirror.css" rel="stylesheet">
<script src="js/jquery-1.11.3.min.js" type="text/javascript"></script>

    <style type="text/css">


a:link, a:visited {
/*color: #000000;*/
text-decoration: none;
}

a:link {
	text-decoration: none;
}
a	{
	outline: none;
	text-decoration: none;
	}
    h2	{
	color: #393939;
	font-size: 16px;
	font-weight: bold;
	line-height: 20px;
	margin-bottom: 10px;
	}
label	{
	line-height: 20px;
	}
label	{
	padding: 2px;
	}
        .style2
        {
            width: 324px;
        }
            
        .warenone
        {
            display:none;
        }
        

        

    </style>
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

          $(document).keydown(function (event) {

              //19 for Mac Command+S
              if (!(String.fromCharCode(event.which).toLowerCase() == 's' && event.ctrlKey) && !(event.which == 19)) return true;

              try {
                  if ($("#ContentPlaceHolder1_btn_save1").val() == 'Save') {
                      $("#ContentPlaceHolder1_btn_save1").focus();
                      $("#ContentPlaceHolder1_btn_save1").click();
                  }
              } catch (ex) { }

              event.preventDefault();
              return false;
          });



          function hilightclass(obj) {

              if (obj != "") {
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <asp:View ID="View1" runat="server">
                <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large" 
                    Font-Underline="True" ForeColor="#000099" Text="WorkFlow Step"></asp:Label>
                <br />
                <br />
                <asp:Label ID="lb_wfname" runat="server" Font-Bold="True" Font-Size="Large" 
                    ForeColor="#3333CC" Text="wfname"></asp:Label>
                &nbsp;&nbsp; &nbsp;&nbsp;
                <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                    Text="Template" />
                &nbsp;<asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="Email" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
                                    CssClass='<%# Eval("StepRowId") %>' 
                                    onmouseout="clarehilightclass(this.className);" 
                                    onmouseover="hilightclass(this.className);"></asp:Label>
                                &nbsp;&nbsp;&nbsp; &nbsp;
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("StepName") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="AfterSaveGoto">
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" 
                                    CssClass='<%# Eval("StepRowIdAfterSave") %>' 
                                    onmouseout="clarehilightclass(this.className);" 
                                    onmouseover="hilightclass(this.className);" Text='<%# Bind("aftersave") %>'></asp:Label>
                                <asp:Label ID="Label22" runat="server" Font-Italic="True" ForeColor="#3333FF" 
                                    Text='<%# Bind("callstartsave") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("aftersave") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" Width="170px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="AfterApproveGoto">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" 
                                    CssClass='<%# Eval("StepRowIdAfterApprove") %>' 
                                    onmouseout="clarehilightclass(this.className);" 
                                    onmouseover="hilightclass(this.className);" Text='<%# Bind("afterapprove") %>'></asp:Label>
                                <asp:Label ID="Label23" runat="server" Font-Italic="True" ForeColor="#3333FF" 
                                    Text='<%# Bind("callstartapprove") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("afterapprove") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" Width="180px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="AfterRejectGoto">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" 
                                    CssClass='<%# Eval("StepRowIdAfterReject") %>' 
                                    onmouseout="clarehilightclass(this.className);" 
                                    onmouseover="hilightclass(this.className);" Text='<%# Bind("afterreject") %>'></asp:Label>
                                <asp:Label ID="Label24" runat="server" Font-Italic="True" ForeColor="#3333FF" 
                                    Text='<%# Bind("callstartreject") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("afterreject") %>'></asp:TextBox>
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
                                <asp:LinkButton ID="LinkButton1" runat="server" 
                                    CssClass='<%# "click-"+Eval("StepRowId") %>' onclick="LinkButton1_Click">Seup Events</asp:LinkButton>
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
                    &nbsp;
                </center>
            </asp:View>
            <asp:View ID="View2" runat="server">
                <asp:Label ID="lb_title1" runat="server" Font-Bold="True" Font-Size="X-Large" 
                    Font-Underline="True" ForeColor="#000099" Text="Step &amp; Events"></asp:Label>
                <br />
                <br />
                <asp:Label ID="lb_eventname" runat="server" Font-Bold="True" Font-Size="Large" 
                    ForeColor="#3333CC" Text="eventname"></asp:Label>
                <br />
                <asp:Label ID="Label2" runat="server" Text="Step RowID : "></asp:Label>
                <asp:Label ID="rowid_txt" runat="server" Text="Label"></asp:Label>
                &nbsp;&nbsp;&nbsp;<asp:Button ID="btn_save0" runat="server" onclick="btn_save_Click" 
                    Text="Save" Width="108px" />
                &nbsp; &nbsp;
                <asp:Button ID="btn_back2" runat="server" onclick="btn_back_Click" Text="Back" 
                    Width="113px" />
                <br />
                <hr />
                <table width="99%">
                    <tr>
                        <td class="style4" valign="top">
                            <table border="1" cellpadding="0" cellspacing="0" width="99%">
                                <div ID="div_eventsave" runat="server" visible="false">
                                    <tr style="background-color:#6699FF">
                                        <td colspan="2" width="200px">
                                            <table width="300px">
                                                <tr>
                                                    <td>
                                                        <h2>
                                                            Event ปุ่ม Save</h2>
                                                    </td>
                                                    <td>
                                                        &nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <div ID="SaveConditionVal" runat="server" visible="false">
                                    </div>
                                    <div ID="SaveConditionValCC1" runat="server" visible="false">
                                    </div>
                                    <div ID="SaveConditionValInform" runat="server" visible="false">
                                    </div>
                                    <tr>
                                        <td bgcolor="#E7E7FF" class="style2" valign="top">
                                            EventAfterSaveEmailSubject<br /><em style="color:Gray">สามารถระบุค่าจาก Object 
                                            {ชื่อ Filed}</em></td>
                                        <td bgcolor="#E7E7FF">
                                            <asp:TextBox ID="txt_EventAfterSaveEmailSubject" runat="server" Width="98%">{sys_workflowname} [{sys_nextstepname}]</asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td bgcolor="#E7E7FF" class="style2" valign="top">
                                            EventAfterSaveEmailContent<br /><br /><em style="color:Gray">สามารถระบุค่าจาก 
                                            Object
                                            <br />
                                            ได้โดยใส่ {ชื่อ Filed}</em></td>
                                        <td bgcolor="#E7E7FF">
                                            <CE:Editor ID="txt_EventAfterSaveEmailContent" runat="server" 
                                                EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                                                EnableStripStyleTagsCodeInjection="False" Height="300px" 
                                                ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                                                StyleWithCSS="True" TabSpaces="10" 
                                                TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                                                Text="Dear All," Width="99%">
                                            </CE:Editor>
                                        </td>
                                    </tr>
                                    <div ID="divSave_informmailcontent" runat="server" visible="false">
                                        <tr>
                                            <td bgcolor="#E7E7FF" class="style2" valign="top">
                                                EventAfterSaveInformEmailContent<br />
                                                <br />
                                                <em style="color:Gray">สามารถระบุค่าจาก Object
                                                <br />
                                                ได้โดยใส่ {ชื่อ Filed}</em></td>
                                            <td bgcolor="#E7E7FF">
                                                <CE:Editor ID="txt_EventAfterSaveInformEmailContent" runat="server" 
                                                    EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                                                    EnableStripStyleTagsCodeInjection="False" Height="300px" 
                                                    ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                                                    StyleWithCSS="True" TabSpaces="10" 
                                                    TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                                                    Text="" Width="99%">
                                                </CE:Editor>
                                            </td>
                                        </tr>
                                    </div>
                                </div>
                                <div ID="div_eventapprove" runat="server" visible="false">
                                    <tr style="background-color:#6699FF">
                                        <td colspan="2" width="200px">
                                            <table width="300px">
                                                <tr>
                                                    <td>
                                                        <h2>
                                                            Event ปุ่ม Approve</h2>
                                                    </td>
                                                    <td>
                                                        &nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <div ID="ApproveConditionVal" runat="server" visible="false">
                                    </div>
                                    <div ID="ApproveConditionValCC1" runat="server" visible="false">
                                    </div>
                                    <div ID="ApproveConditionValInform" runat="server" visible="false">
                                    </div>
                                    <tr>
                                        <td bgcolor="#E7E7FF" class="style2" valign="top">
                                            EventAfterApproveEmailSubject<br /><em style="color:Gray">สามารถระบุค่าจาก 
                                            Object {ชื่อ Filed}</em></td>
                                        <td bgcolor="#E7E7FF">
                                            <asp:TextBox ID="txt_EventAfterApproveEmailSubject" runat="server" Width="98%">{sys_workflowname} [{sys_nextstepname}]</asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td bgcolor="#E7E7FF" class="style2" valign="top">
                                            EventAfterApproveEmailContent<br />
                                            <br />
                                            <em style="color:Gray">สามารถระบุค่าจาก Object
                                            <br />
                                            ได้โดยใส่ {ชื่อ Filed}</em></td>
                                        <td bgcolor="#E7E7FF">
                                            <CE:Editor ID="txt_EventAfterApproveEmailContent" runat="server" 
                                                EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                                                EnableStripStyleTagsCodeInjection="False" Height="300px" 
                                                ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                                                StyleWithCSS="True" TabSpaces="10" 
                                                TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                                                Text="Dear All," Width="99%">
                                            </CE:Editor>
                                        </td>
                                    </tr>
                                    <div ID="divApprove_informmailcontent" runat="server" visible="false">
                                        <tr>
                                            <td bgcolor="#E7E7FF" class="style2" valign="top">
                                                EventAfterApproveInoformEmailContent<br />
                                                <br />
                                                <em style="color:Gray">สามารถระบุค่าจาก Object
                                                <br />
                                                ได้โดยใส่ {ชื่อ Filed}</em></td>
                                            <td bgcolor="#E7E7FF">
                                                <CE:Editor ID="txt_EventAfterApproveInformEmailContent" runat="server" 
                                                    EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                                                    EnableStripStyleTagsCodeInjection="False" Height="300px" 
                                                    ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                                                    StyleWithCSS="True" TabSpaces="10" 
                                                    TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                                                    Text="" Width="99%">
                                                </CE:Editor>
                                            </td>
                                        </tr>
                                    </div>
                                </div>
                                <div ID="div_eventreject" runat="server" visible="false">
                                    <tr style="background-color:#6699FF">
                                        <td colspan="2" width="200px">
                                            <table width="300px">
                                                <tr>
                                                    <td>
                                                        <h2>
                                                            Event ปุ่ม Reject</h2>
                                                    </td>
                                                    <td>
                                                        &nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <div ID="RejectConditionVal" runat="server" visible="false">
                                    </div>
                                    <div ID="RejectConditionValCC1" runat="server" visible="false">
                                    </div>
                                    <div ID="RejectConditionValInform" runat="server" visible="false">
                                    </div>
                                    <tr>
                                        <td bgcolor="#E7E7FF" class="style2" valign="top">
                                            EventAfterRejectEmailSubject<br /><em style="color:Gray">สามารถระบุค่าจาก Object 
                                            {ชื่อ Filed}</em></td>
                                        <td bgcolor="#E7E7FF">
                                            <asp:TextBox ID="txt_EventAfterRejectEmailSubject" runat="server" Width="98%">{sys_workflowname} [{sys_currentstepname}]</asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td bgcolor="#E7E7FF" class="style2" valign="top">
                                            EventAfterRejectEmailContent<br />
                                            <br />
                                            <em style="color:Gray">สามารถระบุค่าจาก Object
                                            <br />
                                            ได้โดยใส่ {ชื่อ Filed}</em></td>
                                        <td bgcolor="#E7E7FF">
                                            <CE:Editor ID="txt_EventAfterRejectEmailContent" runat="server" 
                                                EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                                                EnableStripStyleTagsCodeInjection="False" Height="300px" 
                                                ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                                                StyleWithCSS="True" TabSpaces="10" 
                                                TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                                                Text="Dear All," Width="99%">
                                            </CE:Editor>
                                        </td>
                                    </tr>
                                    <div ID="divReject_informmailcontent" runat="server" visible="false">
                                        <tr>
                                            <td bgcolor="#E7E7FF" class="style2" valign="top">
                                                EventAfterRejectInformEmailContent<br />
                                                <br />
                                                <em style="color:Gray">สามารถระบุค่าจาก Object
                                                <br />
                                                ได้โดยใส่ {ชื่อ Filed}</em></td>
                                            <td bgcolor="#E7E7FF">
                                                <CE:Editor ID="txt_EventAfterRejectInformEmailContent" runat="server" 
                                                    EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
                                                    EnableStripStyleTagsCodeInjection="False" Height="300px" 
                                                    ShowCodeViewToolBar="true" ShowDecreaseButton="False" ShowTagSelector="False" 
                                                    StyleWithCSS="True" TabSpaces="10" 
                                                    TemplateItemList="FontSize,ParagraphMenu, Bold, Italic, Underline, Strikethrough, ForeColor, JustifyLeft, JustifyRight, JustifyCenter, JustifyFull, BulletedList, NumberedList, Indent, Outdent, Cut, Copy, Paste, Undo, Redo, ieSpellCheck,FontFacesMenu,FontSizesMenu" 
                                                    Text="" Width="99%">
                                                </CE:Editor>
                                            </td>
                                        </tr>
                                    </div>
                                </div>
                                <div ID="guideline" runat="server">
                                </div>
                            </table>
                        </td>
                    </tr>
                </table>
                <hr />
                <center>
                    <br />
                    <asp:Button ID="btn_save" runat="server" onclick="btn_save_Click" Text="Save" 
                        Width="108px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btn_back" runat="server" onclick="btn_back_Click" Text="Back" 
                        Width="113px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <br />
                    <br />
                    <asp:Label ID="lb_message" runat="server" Font-Bold="True" Font-Size="Medium" 
                        Text="Message" Visible="False"></asp:Label>
                </center>
            </asp:View>
            <asp:View ID="View3" runat="server">
                <asp:Label ID="lb_title2" runat="server" Font-Bold="True" Font-Size="X-Large" 
                    Font-Underline="True" ForeColor="#000099" Text="WorkFlow Step"></asp:Label>
                <br />
                <br />
                <asp:Label ID="lb_wfname0" runat="server" Font-Bold="True" Font-Size="Large" 
                    ForeColor="#3333CC" Text="wfname"></asp:Label>
                &nbsp;&nbsp; &nbsp;<asp:Button ID="btn_back3" runat="server" onclick="btn_back_Click" 
                    Text="Back" Width="113px" />
            </asp:View>
        </asp:MultiView>
    
    </div>
    </form>
</body>
</html>
