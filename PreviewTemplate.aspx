<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PreviewTemplate.aspx.cs" Inherits="PreviewTemplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <link rel="stylesheet" href="DatePicker/jquery-ui.css">
  <script src="DatePicker/jquery-1.10.2.js"></script>
  <script src="DatePicker/jquery-ui.js"></script>
  <script>
      $(function () {
          $(".datepicker").datepicker();
          $(".datepicker").datepicker("option", "dateFormat", "yy-mm-dd");

      });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Literal ID="content" runat="server"></asp:Literal>
</asp:Content>

