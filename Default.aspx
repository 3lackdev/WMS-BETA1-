<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="clear:both">Transaction Menu<hr /></div>
<div class="grid_7">
<a href="workflowhistory.aspx" class="dashboard-module" >
                	        <img src="images/Actions-view-list-icons-icon.png" 
        tppabs="images/form.png" width="64" height="64" 
        alt="ออกใบ" />
                	        <span title="WorkFlow History">WorkFlows Run 
    History</span>
    
                        </a>
                        
                        </div>
    <div style="clear:both"><br /><br />Master data<hr /></div>
<div class="grid_7">
<a href="workflowlist.aspx" class="dashboard-module" >
                	        <img src="images/form.png" 
        tppabs="images/form.png" width="64" height="64" 
        alt="Workflow" />
                	        <span title="สร้าง WorkFlow">WorkFlow</span>
                        </a>
                        <a href="Databind.aspx" class="dashboard-module" >
                	        <img src="images/list.png" 
        tppabs="images/list.png" width="64" height="64" 
        alt="DataBind Value" />
                	        <span title="DataBind Value">DataBind Value</span>
                        </a>
</div>
                        <div style="clear:both"></div>
                   </asp:Content>

