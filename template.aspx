<%@ Page Title="" Language="C#"  ValidateRequest="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="template.aspx.cs" Inherits="template" %>

<%@ Register assembly="CuteEditor" namespace="CuteEditor" tagprefix="CE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

<script>
    function checkdup() {
        var arr = [];
        var inputs, index;
        var doc = document.getElementById('CE_ContentPlaceHolder1_FreeTextBox1_ID_Frame').contentWindow.document;
        inputs = doc.getElementsByTagName('input');
        for (index = 0; index < inputs.length; ++index) {

                if (inputs[index].name != inputs[index].id) {
                    alert(inputs[index].name + " not match between ID and Name!!");
                    inputs[index].focus();
                    return false;
                }

                if (arr[inputs[index].name] == "Yes") {
                    alert(inputs[index].id + " Duplicate!!");
                    inputs[index].focus();
                    return false;
                }

                arr[inputs[index].name] = "Yes";

            }


            inputs = doc.getElementsByTagName('TEXTAREA');
            for (index = 0; index < inputs.length; ++index) {



                if (inputs[index].name != inputs[index].id) {
                    alert(inputs[index].name + " not match between ID and Name!!");
                    inputs[index].focus();
                    return false;
                }

                if (arr[inputs[index].name] == "Yes") {
                    alert(inputs[index].id + " Duplicate!!");
                    inputs[index].focus();
                    return false;
                }

                arr[inputs[index].name] = "Yes";

            }

            inputs = doc.getElementsByTagName('SELECT');
            for (index = 0; index < inputs.length; ++index) {



                if (inputs[index].name != inputs[index].id) {
                    alert(inputs[index].name + " not match between ID and Name!!");
                    inputs[index].focus();
                    return false;
                }

                if (arr[inputs[index].name] == "Yes") {
                    alert(inputs[index].id + " Duplicate!!");
                    inputs[index].focus();
                    return false;
                }

                arr[inputs[index].name] = "Yes";

            }


        return true;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <asp:Label ID="lb_title" runat="server" Font-Bold="True" Font-Size="X-Large" 
                Font-Underline="True" ForeColor="#000099" Text="Template"></asp:Label>
            <br />
            <br />
            <asp:Label ID="lb_wfname" runat="server" Font-Bold="True" Font-Size="Large" 
                ForeColor="#3333CC" Text="wfname"></asp:Label>
            &nbsp;&nbsp;
            <asp:Button ID="btn_new" runat="server" Text="New Template" 
                onclick="btn_new_Click" BackColor="#99CC00" />
    <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" 
                CellPadding="3" GridLines="Horizontal" Width="99%">
                <AlternatingRowStyle BackColor="#F7F7F7" />
                <Columns>
                    <asp:BoundField DataField="TemplateRowId" HeaderText="TemplateRowId">
                    <HeaderStyle HorizontalAlign="Left" Width="250px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="TemplateName">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("TemplateName") %>'></asp:Label>
                            &nbsp;&nbsp;
                            <asp:LinkButton ID="lnk_edit" runat="server" onclick="lnk_edit_Click">Edit</asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TemplateName") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Seting">
                        <ItemTemplate>
                            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                                Text="Edit Template" />
                            &nbsp;
                            <asp:HyperLink ID="HyperLink1" runat="server" 
                                NavigateUrl='<%# Eval("TemplateRowId", "PreviewTemplate.aspx?rowid={0}") %>' 
                                Target="_blank">Preview Template</asp:HyperLink>
                            &nbsp;|
                            <asp:HyperLink ID="HyperLink2" runat="server" 
                                NavigateUrl='<%# Eval("TemplateRowId", "PreviewTemplate.aspx?rowid={0}&type=obj") %>' 
                                Target="_blank">View Object Template</asp:HyperLink>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" Width="400px" />
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
                Width="113px" /> </center>
            <br />
        </asp:View>
          <asp:View ID="View2" runat="server">
              
              <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large" 
                  Font-Underline="True" ForeColor="#000099" Text="Template Design"></asp:Label>
              <br />
              <br />
              <asp:Label ID="lb_templatename" runat="server" Font-Bold="True" 
                  Font-Size="Large" ForeColor="#3333CC" Text="Templatename"></asp:Label>
            
              <br />
              <table style="width:100%"><tr><td><asp:Label ID="Label1" runat="server" Text="Temaplate RowID : "></asp:Label><asp:Label ID="rowid_txt" runat="server" Text="Label"></asp:Label></td>
                  <td width="200px">Include Css
                      <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" 
                          onselectedindexchanged="DropDownList_includeSave">
                          <asp:ListItem Value="">-- Standard --</asp:ListItem>
                          <asp:ListItem Value="Boostrap 3.4.1">Boostrap 3.4.1</asp:ListItem>
                          <asp:ListItem Value="Boostrap 4.0.0">Boostrap 4.0.0</asp:ListItem>
                      </asp:DropDownList>
                  </td><td width="200px">Include Lib
                      <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" 
                          onselectedindexchanged="DropDownList_includeSave">
                          <asp:ListItem Value="">-- Standard --</asp:ListItem>
                          <asp:ListItem Value="HtmlEditor">HtmlEditor</asp:ListItem>
                      </asp:DropDownList>
                  </td></tr></table>
              
            
              <CE:Editor ID="FreeTextBox1" runat="server" 
            EnableStripLinkTagsCodeInjection="False" EnableStripScriptTags="False" 
            StyleWithCSS="True" EnableStripStyleTagsCodeInjection="False" Height="700px" 
                  Width="99%" ></CE:Editor> 

             &nbsp;<br />
              <asp:Label ID="Label4" runat="server" ForeColor="#3333FF" 
                  Text="Spacial CSS Class :  datepicker , LookupEmail , LookupDataBind , htmledit"></asp:Label>
              &nbsp;[EX. title: <em>bindname=bbb&return={field1,1}{field2,2}</em> 
              &amp;hidecolumn=5,6&filter={HtmlInputID1,databindfieldname1}{HtmlInputID2,databindfieldname2}]<br />
              <asp:Label ID="Label6" runat="server" ForeColor="#3333FF" 
                  
                  Text="Spacial Input Attribute : data-emailfullname='Object ID' , group='Your groupname' , reqby='Object ID',enabledby='Object ID', ignoreval='value in val selector'  (Html Select object only)"></asp:Label>
              &nbsp;<br />
              <asp:Label ID="Label5" runat="server" ForeColor="#3333FF" 
                  Text="Checkbox can select only one choice "></asp:Label>
              [Set attribute group=xxx Ex. &lt;input type=checkbox id=test name=test group=AAA&gt; ]<br />
              <br />
              <br />
              <asp:Button ID="btn_save" runat="server" onclick="btn_save_Click" 
                  onclientclick="return checkdup();" Text="Save" Width="113px" />
              &nbsp;&nbsp;&nbsp;
              <asp:Button ID="btn_save1" runat="server" onclick="btn_save_Click" 
                  onclientclick="return checkdup();" Text="Save Template Only" Width="153px" />
              &nbsp;&nbsp;&nbsp;&nbsp;
              <asp:Button ID="btn_preview" runat="server" Text="Preview Template" />
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:Button ID="btn_back1" runat="server" onclick="btn_back_Click" Text="Back" 
                  Width="113px" />
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:Button ID="btn_preview0" runat="server" 
                  onclientclick="window.open('https://www.w3schools.com/html/default.asp');return false;" 
                  Text="Manual HTML / CSS" />
              <br />
              <br />
             
 
             
              <br />
              <br />
            
        </asp:View>
        <asp:View ID="View3" runat="server">
            <asp:Label ID="Label2" runat="server" Text="Temaplate RowID : "></asp:Label>
            <asp:Label ID="rowid_txt0" runat="server" Text="Label"></asp:Label>
        <table width="99%"><tr><td style="font-size: medium; background-color: #C0C0C0;" 
                width="50%">Input type</td>
            <td style="font-size: medium; background-color: #C0C0C0;">Select value Type</td></tr>
        <tr><td>
            <asp:CheckBoxList ID="cb_fieldlist" runat="server">
            </asp:CheckBoxList></td><td valign="top">
                <asp:CheckBoxList ID="cb_fieldlistselect" runat="server">
                </asp:CheckBoxList>
            </td></tr></table>
            <br />
            <asp:Button ID="btn_savefield" runat="server" Text="Save" Width="113px" 
                onclick="btn_savefield_Click" />
            <br />
        </asp:View>
        <asp:View ID="View4" runat="server">
            สร้างใหม่<br />
            <br />
            <asp:Label ID="Label3" runat="server" Text="TemplateName"></asp:Label>
            &nbsp;&nbsp;
            <asp:TextBox ID="txt_templatename" runat="server"></asp:TextBox>
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

