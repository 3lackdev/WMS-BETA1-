<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

          <div style="height:300px"><div class="x63"><table summary="" border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td width="100%"><h1 class="x18">Login</h1></td></tr><tr><td class="x2i">
              <img src="images/t.gif" alt=""></td></tr></tbody></table></div><table summary="" border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr valign="top"><td width="100%"></td><td nowrap="nowrap"></td></tr></tbody></table><table summary="" border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="center"><table summary="" border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td align="right" nowrap="nowrap" width="37%"><span class="x8">Username</span></td><td width="12">
    <img src="images/t.gif" alt="" height="0" width="12"></td><td nowrap="nowrap" 
        valign="top" width="63%" align="left">
                  <asp:TextBox
            ID="username" runat="server" CssClass="x4" Width="220px"></asp:TextBox>
                  &nbsp;<asp:DropDownList ID="domain" runat="server" TabIndex="8">
            <asp:ListItem>BST.CO.TH</asp:ListItem>
     	    <asp:ListItem Value="BST.CO.TH">BSTS.CO.TH</asp:ListItem>
            <asp:ListItem>JBE.CO.TH</asp:ListItem>
           <asp:ListItem>GUEST</asp:ListItem>
        </asp:DropDownList>
              </td></tr><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td colspan="2"></td><td></td></tr><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td align="right" nowrap="nowrap" width="37%"><span class="x8">Password</span></td><td width="12">
    <img src="images/t.gif" alt="" height="0" width="12"></td><td nowrap="nowrap" 
        valign="top" width="63%" align="left">
                  <asp:TextBox
            ID="password" runat="server" CssClass="x4" TextMode="Password" Width="220px"></asp:TextBox></td></tr><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td colspan="2"></td>
                  <td align="left">
                      <asp:Label ID="Label1" runat="server" Font-Size="Small" ForeColor="#FF3300" 
                          Text="Wrong user or password !" Visible="False"></asp:Label>
                  </td></tr><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td class="x8" align="right" width="37%"><img src="images/t.gif" alt="" height="30" width="1"></td><td width="12">
    <img src="images/t.gif" alt="" height="0" width="12"></td><td align="left" width="63%">
        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/bLogio9S-.gif" 
                      onclick="ImageButton1_Click" />  &nbsp;<asp:CheckBox ID="chkRememberMe" 
                          runat="server" Checked="True" Text="Remember" />
                  </td></tr><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td class="x8" align="right" width="37%"></td><td width="12">
        <img src="images/t.gif" alt="" height="0" width="12"></td><td align="left" width="63%"></td></tr><tr><td width="20px"><img src="images/t.gif" alt="" height="1" width="20"></td><td class="x8" align="right" width="37%"><img src="images/t.gif" alt="" height="5" width="1"></td><td width="12">
    <img src="images/t.gif" alt="" height="0" width="12"></td><td align="left" width="63%">
                  <br />
                  <br />Ps. ใช้ username & password เดียวกับที่ใช้ logon เครื่องคอมพิวเตอร์
                  <br />
                  <asp:Button ID="Button1" runat="server" BackColor="White" BorderColor="#FF3300" 
                      BorderStyle="Solid" Text="Log out &gt; " />
              </td></tr></tbody></table></td></tr></tbody></table>
        </div><div></div><div><div><img src="images/t.gif" alt="" height="9" width="1"></div><div class="x2i"><img src="images/t.gif" alt="" height="1" width="1"></div><div><img src="images/t.gif" alt="" height="10" width="1"></div></div><div></div></div><table></table><table summary="" border="0" cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td><div class="xv"></div><div class="xx"></div></td><td class="xw"></td></tr></tbody></table>

<div style="display: none;" id="YontooInstallID">be43a597-b627-4519-8ebf-aaa309eaacf5</div>
    </div>
</asp:Content>

