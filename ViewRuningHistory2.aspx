<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="ViewRuningHistory2.aspx.cs" Inherits="ViewRuningHistory2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Css/layout.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1 {
            height: 18px;
            width: 225px;
        }

        .style2 {
            height: 18px;
            width: 825px;
        }

        .nna {
            display: none;
        }

        .dialog_wrapper {
            width: 100%;
            top: 0px;
            left: 0px;
            position: absolute;
            z-index: 5;
            display: block;
        }

        .dialog {
            position: fixed;
            left: 50%;
            width: 600px;
            margin-left: -300px;
            height: 300px;
            margin-top: 50px;
        }
    </style>
    <link href="bootstrap3/css/bootstrap.min.css" rel="stylesheet" />

    <script type="text/javascript" src="bootstrap4/jquery-3.2.1.slim.min.js"></script>
    <script type="text/javascript" src="bootstrap3/js/bootstrap.min.js"></script>


</head>
<body>

    

    <form id="form1" runat="server">
        <div>


           <script type="text/javascript">

               $(document).ready(function () {
                   $("#btnShowModal").click(function () {
                       $("#myModal").modal();
                   });

                   

               });

               function ShowPopup() {
                   $("#btnShowModal").click();
               }


           </script>



            <button id="btnShowModal" type="button" class="btn btn-info btn-lg" style="display: none;" >Open Small Modal</button>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">
                    <asp:Label ID="lb_title0" runat="server" Font-Bold="True" Font-Size="X-Large"
                        Font-Underline="True" ForeColor="#000099"
                        Text="WorkFlow running status"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
                    <br />
                    <table style="width: 755px; float: left">
                        <tr>
                            <td width="90px">ค้นหาจาก</td>
                            <td class="style2">
                                <asp:DropDownList ID="DropDownList2" runat="server">
                                    <asp:ListItem>2022</asp:ListItem>
                                    <asp:ListItem>2021</asp:ListItem>
                                    <asp:ListItem>2020</asp:ListItem>
                                    <asp:ListItem>2019</asp:ListItem>
                                    <asp:ListItem>2018</asp:ListItem>
                                    <asp:ListItem>2017</asp:ListItem>
                                    <asp:ListItem>2016</asp:ListItem>
                                    <asp:ListItem>2015</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:DropDownList ID="DropDownList1" runat="server">
                                    <asp:ListItem>Status</asp:ListItem>
                                    <asp:ListItem>WFName</asp:ListItem>
                                    <asp:ListItem Value="WorkFlowRunningStatus.StartRowID">StartRowID</asp:ListItem>
                                    <asp:ListItem Value="WorkFlowStep.StepName">CurrentStepName</asp:ListItem>
                                    <asp:ListItem Value="WorkFlowRunningStatus.UdBy">UdBy</asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="TextBox1" runat="server" Width="283px"></asp:TextBox>
                            </td>
                            <td class="style1">
                                <asp:Button ID="Button1" OnClientClick="document.getElementById('savepop').style.display = 'inline';" runat="server" OnClick="Button1_Click" Text="ค้นหา" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button5" runat="server" OnClick="Button1_Click"
                Text="Refresh" />
                            </td>
                        </tr>
                    </table>
                    <div style="float: right; width: 420px; margin-right: 20px">
                        <asp:CheckBox ID="showall" runat="server" Text="ShowAll" AutoPostBack="True"
                            OnCheckedChanged="showall_CheckedChanged" onclick="document.getElementById('savepop').style.display = 'inline';" />
                        &nbsp;<asp:LinkButton ID="LinkButton4" runat="server"
                            OnClick="LinkButton4_Click">#Show Deleted</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label5" runat="server" Text="พบรายการทั้งหมด :  200 รายการ"></asp:Label>
                        &nbsp;
            <asp:LinkButton ID="lnk_exp_csv" runat="server" OnClick="lnk_exp_csv_Click">Export CSV</asp:LinkButton>
                    </div>
                    <br style="clear: both" />
                    <asp:GridView ID="GridView1" runat="server" BackColor="LightGoldenrodYellow"
                        BorderColor="Tan" BorderWidth="1px" CellPadding="2"
                        GridLines="None" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                        Width="99%" ForeColor="Black" AutoGenerateColumns="False"
                        AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging"
                        PageSize="35" Font-Size="11px" DataKeyNames="UserSpecialAction"
                        OnRowDataBound="GridView1_RowDataBound" OnRowCommand="GridView1_RowCommand">
                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                        <Columns>
                            <asp:TemplateField HeaderText="View" ShowHeader="False">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink5" runat="server"
                                        NavigateUrl='<%# String.Format("WorkflowRuning.aspx?rowid=force&runingkey={0}&referencekey=view",Eval("StartRowID")) %>'
                                        Target="_blank" Font-Overline="False" ForeColor="#00CC66">View</asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" Width="50px" />
                                <ItemStyle Font-Bold="True" ForeColor="#3333CC" />
                            </asp:TemplateField>
                            <asp:CommandField HeaderText="History" SelectText="History"
                                ShowSelectButton="True">
                                <HeaderStyle HorizontalAlign="Left" Width="50px" />
                                <ItemStyle Font-Bold="True" ForeColor="#3333CC" />
                            </asp:CommandField>
                            <asp:TemplateField HeaderText="Spacial Action">
                                <ItemTemplate>
                                    <asp:Panel ID="Panel1" runat="server">
                                        <asp:HyperLink ID="HyperLink1" runat="server" Font-Bold="True"
                                            NavigateUrl='<%# String.Format("application/{1}/viewnotificationalert.aspx?startrowid={0}",Eval("StartRowID"),Eval("ApplicationFolder")) %>'
                                            Text="Alert" Target="_blank"></asp:HyperLink>
                                        <asp:Label ID="HyperLink2_" runat="server" Text="Label">|</asp:Label>
                                        <asp:HyperLink ID="HyperLink2" runat="server" Font-Bold="True"
                                            NavigateUrl='<%# String.Format("http://cmp.bst.co.th/CrytalReportPreview/Application/{1}/Report.aspx?rowid={0}",Eval("StartRowID"),Eval("ApplicationFolder")) %>'
                                            Target="_blank" Text="Report"></asp:HyperLink>
                                        <asp:Label ID="HyperLink3_" runat="server" Text="Label">&nbsp;|</asp:Label>
                                        <asp:HyperLink ID="HyperLink3" runat="server" Font-Bold="True"
                                            NavigateUrl='<%# Eval("StartRowID", "WorkflowRuning.aspx?rowid=force&runingkey={0}&referencekey=force") %>'
                                            Target="_blank" Text="Cur. flow"></asp:HyperLink>
                                        &nbsp;|
                            <asp:HyperLink ID="HyperLink4" runat="server" Font-Bold="True"
                                NavigateUrl='<%# Eval("StartRowID", "WorkflowRuning.aspx?rowid=force&runingkey={0}&referencekey=force&action=update") %>'
                                Target="_blank" Text="Up. Val"></asp:HyperLink>
                                        &nbsp;<asp:Label ID="lbLineShow" runat="server" Text="|"></asp:Label>
                                        <asp:LinkButton ID="LinkButton2" runat="server" Font-Bold="True"
                                            Font-Underline="False" ForeColor="Red" OnClick="LinkButton2_Click"
                                            OnClientClick="return confirm('ยืนยันการลบ')"
                                            ToolTip='<%# String.Format("{0},{1}",Eval("WFRowID"),Eval("StartRowID")) %>'>Del</asp:LinkButton>
                                        <asp:LinkButton ID="btnDel" runat="server" Font-Bold="True"
                                            Font-Underline="False" ForeColor="Red" 
                                            CommandName="OnDel" CommandArgument='<%# String.Format("{0},{1},{2}",Eval("WFRowID"),Eval("StartRowID"),Eval("ETC1")) %>'
                                            OnClick="btnDel_Click">Del</asp:LinkButton>
                                    </asp:Panel>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" Width="250px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="StartRowID" HeaderText="StartRowID">
                                <HeaderStyle HorizontalAlign="Left" Width="240px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC1" HeaderText="ETC1" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC2" HeaderText="ETC2" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC3" HeaderText="ETC3" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC4" HeaderText="ETC4" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC5" HeaderText="ETC5" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC6" HeaderText="ETC6" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC7" HeaderText="ETC7" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC8" HeaderText="ETC8" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC9" HeaderText="ETC9" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ETC10" HeaderText="ETC10" Visible="False" HtmlEncode="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="WFName" HeaderText="WFName">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Current StepName" HeaderText="Current StepName">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="UdBy" HeaderText="ResponseBy">
                                <HeaderStyle HorizontalAlign="Left" Width="80px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="UdDate" HeaderText="UdDate">
                                <HeaderStyle HorizontalAlign="Left" Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Status" HeaderText="Status">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                        </Columns>
                        <FooterStyle BackColor="Tan" />
                        <HeaderStyle BackColor="Tan" Font-Bold="True"
                            HorizontalAlign="Left" />
                        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue"
                            HorizontalAlign="Center" Font-Bold="True" Font-Size="Medium" />
                        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
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
            <asp:Button ID="btn_back1" runat="server" OnClick="btn_back1_Click" Text="Back"
                Width="113px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_view" runat="server" OnClick="btn_view_Click"
                Text="View Form" Width="115px" />
                    &nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button6" runat="server" OnClick="Button6_Click"
                Text="Refresh" />
                    &nbsp;&nbsp;
            <asp:LinkButton ID="delsub" runat="server" OnClick="delsub_Click"
                Visible="False">del</asp:LinkButton>
                    <br />
                    <br />
                    <asp:GridView ID="GridView2" runat="server" BackColor="LightGoldenrodYellow"
                        BorderColor="Tan" BorderWidth="1px" CellPadding="2"
                        GridLines="None"
                        Width="99%" ForeColor="Black" AutoGenerateColumns="False">
                        <AlternatingRowStyle BackColor="PaleGoldenrod" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HiddenField ID="HiddenField1" runat="server"
                                        Value='<%# bind("rowid") %>' />
                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click"
                                        OnClientClick="return confirm('Confirm?')">Resend</asp:LinkButton>
                                    &nbsp;|
                            <asp:LinkButton ID="LinkButton3" runat="server" OnClick="LinkButton3_Click">Rewind</asp:LinkButton>
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
                                HtmlEncode="False">
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
                        <FooterStyle BackColor="Tan" />
                        <HeaderStyle BackColor="Tan" Font-Bold="True"
                            HorizontalAlign="Left" />
                        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue"
                            HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                        <SortedAscendingCellStyle BackColor="#FAFAE7" />
                        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                        <SortedDescendingCellStyle BackColor="#E1DB9C" />
                        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                    </asp:GridView>
                    <br />
                    <br />
                    <br />
                    <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium"
                        ForeColor="#33CC33" Text="ReSent Completed" Visible="False"></asp:Label>
                    <br />
                </asp:View>
                <asp:View ID="View3" runat="server">
                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="X-Large"
                        Font-Underline="True" ForeColor="#000099"
                        Text="WorkFlow runing status (Rewind Step)"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="Label6" runat="server" Font-Bold="True" Text="Runing RowID :"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Back"
                Width="113px" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button3" runat="server" OnClick="btn_view_Click"
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

                    **&nbsp; กรณีต้องการทำ delegate ไปให้คนใหม่ ให้ทำการ แก้ไข Email ในช่อง Email To 
            ให้เป็น Email ของคนใหม่<br />
                    **&nbsp; การ Rewind จะสามารถทำได้ในกรณี Flow 
            ยังไม่จบเท่านั้น<br />
                    <br />
                    <asp:Button ID="Button4" runat="server" ForeColor="Red" OnClick="Button4_Click"
                        OnClientClick="return confirm('ยืนยันการย้อน step? หลังจากยืนยัน step ปัจจุบันจะอยู่ที่  '+document.getElementById('Label8').innerHTML);"
                        Text="Force Rewind Step" />
                    <br />

                    <br />
                </asp:View>
                <asp:View ID="View4" runat="server">
                    <asp:Label ID="Label16" runat="server" Font-Bold="True" Font-Size="X-Large"
                        Font-Underline="True" ForeColor="Red"
                        Text="WorkFlow runing status (Record Deleted)"></asp:Label>
                    &nbsp;
              <asp:Button ID="Button7" runat="server" OnClick="btn_back1_Click" Text="Back"
                  Width="113px" />
                    <br />
                    <br />
                    <asp:GridView ID="GridView3" runat="server" CellPadding="4" Font-Size="8pt"
                        ForeColor="#333333" GridLines="None" Width="100%">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </asp:GridView>
                </asp:View>
            </asp:MultiView>

        </div>

          <script type="text/javascript">

              function openModal() {
                  jQuery.noConflict();
                  $('#myModal').modal();
              }
          </script>


        <!-- Modal -->
    <div class="modal" id="myModal" role="dialog">
        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Confirm Delete</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-lg-3">
                            Doc No. :
                        </div>
                        <div class="col-lg-9">
                            <asp:Label ID="lbDocNo" runat="server" Text="-"></asp:Label>
                            <asp:HiddenField ID="hdWFID" runat="server" />
                            <asp:HiddenField ID="hdRUNID" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            Delete Reason
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <asp:TextBox ID="txtDeleteReason" CssClass="form-control" runat="server" TextMode="MultiLine" rows="3" BackColor="Yellow"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-3">
                            Delete By :
                        </div>
                        <div class="col-lg-9">
                            <asp:Label ID="lbDeleteBy" runat="server" Text="-"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-3">
                            Delete Date :
                        </div>
                        <div class="col-lg-9">
                            <asp:Label ID="lbDeleteDate" runat="server" Text="-"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                           <asp:Label ID="lbModalError" runat="server" ForeColor="Red" Text=""></asp:Label>
                        </div>
                       
                    </div>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="btSaveDel" class="btn btn-default btn-danger" runat="server" Text="Delete" OnClick="btSaveDel_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    </form>
    <div id="savepop" style="display: none">
        <div class="dim">
        </div>



        <div class="dialog_wrapper">
            <div class="dialog">
                <center>
                    <br />
                    <img id="ctl00_ContentPlaceHolder1_Image1" src="images/loadingc.gif" style="height: auto; width: auto; border-width: 0px;" /></center>
            </div>
        </div>
    </div>

   

    




</body>
</html>
