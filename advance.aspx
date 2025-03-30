<%@ Page Language="C#" AutoEventWireup="true" CodeFile="advance.aspx.cs" Inherits="advance" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
    
    body
    {
margin: 0;
padding: 0;
font-family: tahoma,"Helvetica Neue", Helvetica, Arial, Verdana, sans-serif;
font-size: 14px;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h3>Create Notificaton Job</h3>
    <div>
    
        <p>** ใช้ในการตั้ง Schedule เพื่อส่ง Mail
          <br />
          <br />
          <br />
          **&nbsp; ชื่อระบบ&nbsp; ต้องมีอยู่ในระบบ Notification และผูก Profile แล้ว&nbsp;&nbsp; 
        สามารถเช็คได้ที่ <br />
        Server: Legal<br />
        DB : BST_Notificaton 
        <br />
        Table : SystemAndMailProfile
        <br />
        <br />
        <br />
        <br />
        insert into  BST_NOTIFICATION.dbo.Notification_Job (system,KeyRef, Subject, EmailTo, Msg, SendDate, SendTime,  CreatedDate, CreateBy) values ('ชื่อระบบ','เลขที่อ้างอิง','ชื่อ 
        subject Mail','Email ส่งถึง','เนื้อ Mail',getdate(),'เวลา',getdate(),&#39;ผู้ส่ง'); <br />
        <br />
        ตัวอย่าง<br />
        <br />
        insert into BST_NOTIFICATION.dbo.Notification_Job (system,KeyRef, Subject, 
        EmailTo, Msg, SendDate, SendTime, CreatedDate, CreateBy) values 
        (&#39;CA/PA&#39;,&#39;{sys_startworkflowid}&#39;,&#39;{subject}&#39;,&#39;{MailTo}&#39;,&#39;Body 
        test&#39;,getdate(),&#39;0800&#39;,getdate(),&#39;{sys_username}&#39;);<br />
        <br />
        <br />
        -----------------------------------<br />
        <br />
        การใช้ Function DateDiff<br />
        <br />
        BST_NOTIFICATION.dbo.DateDiff_Day(&#39;2015-10-10&#39;,5)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
        หมายถึงต้องการวันก่อนหน้า 5 วัน<br />
        <br />
        <br />
        ตัวอย่าง<br />
        <br />
        insert into BST_NOTIFICATION.dbo.Notification_Job (system,KeyRef, Subject, 
        EmailTo, Msg, SendDate, SendTime, CreatedDate, CreateBy) values 
        (&#39;CA/PA&#39;,&#39;{sys_startworkflowid}&#39;,&#39;{subject}&#39;,&#39;{MailTo}&#39;,&#39;Body 
        test&#39;,BST_NOTIFICATION.dbo.DateDiff_Day(&#39;{ValueDate}&#39;,5) ,&#39;0800&#39;,getdate(),&#39;{sys_username}&#39;);<br />
        <br />
      </p>
        <p>--------------------------------------</p>
        <p>ตัวอย่างการใช้ store เพื่อเอาค่าจาก object เข้าไปสร้าง alert</p>
        <p>BEGIN TRAN T1; <br />
          IF OBJECT_ID('tempdb..#Tmp', 'U') IS NOT NULL <br />
DROP TABLE #Tmp</p>
        <p>create table #Tmp(email nvarchar(150), duedate nvarchar(50), title nvarchar(450))<br />
          insert into #Tmp values<br />
          ('{S4_response1}','{S4_due1}','{S4_corrective1}'),<br />
          ('{S4_response2}','{S4_due2}','{S4_corrective2}'),<br />
          ('{S4_response3}','{S4_due3}','{S4_corrective3}'),<br />
          ('{S4_response4}','{S4_due4}','{S4_corrective4}'),<br />
          ('{S4_response5}','{S4_due5}','{S4_corrective5}'),<br />
          ('{S4_response6}','{S4_due6}','{S4_corrective6}'),<br />
          ('{S4_response7}','{S4_due7}','{S4_corrective7}'),<br />
          ('{S4_response8}','{S4_due8}','{S4_corrective8}'),<br />
          ('{S4_response9}','{S4_due9}','{S4_corrective9}')</p>
        <p>&nbsp;</p>
        <p>delete from #Tmp where email=''<br />
          delete from BST_NOTIFICATION.dbo.Notification_Job where system='PSSR' and KeyRef='{sys_startworkflowid}' and status='N';</p>
        <p>declare @email nvarchar(150)<br />
          declare @duedate nvarchar(150)<br />
          declare @title nvarchar(450)<br />
          DECLARE db_cursor CURSOR FOR </p>
        <p>select email,duedate from #Tmp</p>
        <p>OPEN db_cursor <br />
          FETCH NEXT FROM db_cursor INTO @email,@duedate ,@title</p>
        <p>WHILE @@FETCH_STATUS = 0 <br />
          BEGIN <br />
          insert into BST_NOTIFICATION.dbo.Notification_Job (system,KeyRef, Subject, EmailTo, Msg, SendDate, SendTime, CreatedDate, CreateBy) values ('PSSR','{sys_startworkflowid}','PSSR : {S1_PSSR_title}',@email,'Dear PSSR Responsible&lt;br /&gt;&lt;br /&gt;<br />
          CA/PA : &lt;b&gt;'+@title+'&lt;/b&gt;&lt;br /&gt;<br />
          Due date is coming soon.  in 7 days&lt;br /&gt;<br />
          Please specify new due date and reason if required.&lt;br /&gt;<br />
  &lt;br /&gt;<br />
          Best regards,&lt;br /&gt;<br />
          PSSR tracking system',BST_NOTIFICATION.dbo.DateDiff_Day(@duedate,7) ,'0800',getdate(),'{sys_username}');</p>
        <p> FETCH NEXT FROM db_cursor INTO @email,@duedate ,@title<br />
          END </p>
        <p>CLOSE db_cursor <br />
          DEALLOCATE db_cursor</p>
        <p>COMMIT TRAN T1;<br />
        </p>
        <p><br />
                    </p>
    </div>
    <hr />
    <h3>Button Css Style</h3>
    <div>
    
    CssStyle .Save
        .Approve .Reject<br />
        <br />
        ตัวอย่าง ต้องการใส่ Style ให้ปุ่ม Save, Approve, Reject<br />
        <br />
        ใส่ไว้ที่ Template ด้านบนสุดของ HTML<br />
        <br />
        <br />
        &lt;style&gt;<br />
        <br />
        .Save<br />
        {<br />
&nbsp;&nbsp;&nbsp; font-family: Verdana, Arial, Helvetica, sans-serif;
        <br />
&nbsp;&nbsp;&nbsp; font-size: 18px;
        <br />
&nbsp;&nbsp;&nbsp; border: 1px solid #999999;
        <br />
&nbsp;&nbsp;&nbsp; height: 30px;
        <br />
&nbsp;&nbsp;&nbsp; color: #666666;<br />
&nbsp;&nbsp;&nbsp;
        <br />
        }<br />
        <br />
        .Approve<br />
        {<br />
&nbsp;&nbsp;&nbsp; font-family: Verdana, Arial, Helvetica, sans-serif;
        <br />
&nbsp;&nbsp;&nbsp; font-size: 18px;
        <br />
&nbsp;&nbsp;&nbsp; border: 1px solid #999999;
        <br />
&nbsp;&nbsp;&nbsp; height: 30px;
        <br />
&nbsp;&nbsp;&nbsp; color: #666666;<br />
&nbsp;&nbsp;&nbsp;
        <br />
        }<br />
        <br />
        .Reject<br />
        {<br />
&nbsp;&nbsp;&nbsp; font-family: Verdana, Arial, Helvetica, sans-serif;
        <br />
&nbsp;&nbsp;&nbsp; font-size: 18px;
        <br />
&nbsp;&nbsp;&nbsp; border: 1px solid #999999;
        <br />
&nbsp;&nbsp;&nbsp; height: 30px;
        <br />
&nbsp;&nbsp;&nbsp; color: #666666;<br />
&nbsp;&nbsp;&nbsp;
        <br />
        }<br />
        <br />
        &lt;/style&gt;<br />
        <br />
        <br />
        <br />
        <hr />
    <h3>JavaScript Common function</h3>
        require(ObjID,Value) ;&nbsp;&nbsp;&nbsp; value = true,false<br />
        <br />
        show(ObjID,value);&nbsp;&nbsp; value = true,false<br />
        <br />
        <br />
        <br />
        <br />
    </div>
        <hr />
        <h3>Table Databindig</h3>
        <div>
        
       
        
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            data-triggerid&nbsp; คือ object ที่หากมีการเปลี่ยนแปลงข้อมูลเกิดขึ้น 
            ระบบจะสั่งให้ไปดึง databind
            <br />
            data-bindname คือ DataBind name ที่ ทำ SQL ไว้<br />
            data-filter คือ ส่งค่าการ object ที่กรอกมา ส่งไป filter โดยส่ง {ID 
            ที่1,FieldDataBind}{ID ที่2,FieldDataBind}</span><br />
            <br />
            ตัวอย่าง<br />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            <br />
            &lt;table&nbsp; class=&quot;TableDataBind&quot; id=&quot;TableDataBind0&quot; 
            data-triggerid=&quot;HtmlObjID1,#HtmlObjID2&quot; data-bindname=&quot;SQLBindName&quot; 
            data-filter=&quot;{HtmlObjID1,RECEIPT_NUM}{HtmlObjID2,WHSE_CODE}&quot;&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;tbody&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;tr&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;th&gt;&amp;nbsp;LINE_NUM&lt;/th&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;th&gt;&amp;nbsp;ITEM_DESCRIPTION&lt;/th&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;th&gt;&amp;nbsp;QUANTITY&lt;/th&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;th&gt;&amp;nbsp;AMOUNT&lt;/th&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/tr&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;tr class=&quot;TableDataBindRow&quot;&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;td&gt;&amp;nbsp;{LINE_NUM}&lt;/td&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;td&gt;&amp;nbsp;{ITEM_DESCRIPTION}&lt;/td&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;td&gt;&amp;nbsp;{QUANTITY}&lt;/td&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;td&gt;&amp;nbsp;{AMOUNT}&lt;/td&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/tr&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/tbody&gt;</span><br 
                style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial;" />
            <span style="color: rgb(0, 0, 0); font-family: &quot;Times New Roman&quot;; font-size: medium; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">
            &lt;/table&gt;<br />
            <br />
            </span>
        
       
        
        </div>
        <hr />
    <h3>Email Content</h3>
    <div>
    
        Dear All,
        <br />
        <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please review CA/PA number : {Number_code}<br />
        <br />
        Best Regards,<br />
        <br />
        <br />
        ***&nbsp; สามารถอ้างอิงค่าในทุก Object ที่อยู่ใน Template โดยใช้ {ตามด้วยชื่อ 
        Object}&nbsp;&nbsp; เช่น&nbsp;&nbsp;&nbsp; {name}<br />
        <br />
    
    </div>
        <hr />
    <br />
    <h3>Dynamic Selected Key</h3>
    <div>
    
    
        <br />
        ในการตั้งค่า&nbsp;
     Position and Email&nbsp;&nbsp; สามารถระบุ Dynamic เพื่อ Select Email 
        จากค่าที่ระบุจาก Selector ใน Temaplate ได้ โดยทำการระบุ Selected Key ที่ Menu Position and Email 
        <br />
        <br />
        เช่น<br />
        <br />
        Position: A1&nbsp; Email : <a href="mailto:AAA@bst.co.th">A1@bst.co.th</a>&nbsp; 
        ระบุ Selected Key เป็น A1<br />
        Position: A2&nbsp; Email : <a href="mailto:AAA@bst.co.th">A2@bst.co.th</a>&nbsp; 
        ระบุ Selected Key เป็น A2<br />
        Position: A3&nbsp; Email : <a href="mailto:AAA@bst.co.th">A3@bst.co.th</a>&nbsp; 
        ระบุ Selected Key เป็น A3<br />
        <br />
        <br />
        ที่การตั้งค่า Step &amp; Event<br />
        <br />
        ตัวอย่างในการตั้งค่า Event หลังจาก Save&nbsp; ให้ส่ง Mail เป็น Dynamic&nbsp; หาก 
        {A} มีค่า อะไรมาให้ไปหาจาก Position ที่มีค่า Selected Key ตรงกัน เพื่อส่ง Email 
        ให้คนๆ นั้น<br />
        <br />
        <table border="1" cellpadding="0" cellspacing="0" width="99%">
            <tr>
                <td bgcolor="#e7e7ff" valign="top">
                    EventAfterSaveMailToPosition</td>
                <td bgcolor="#e7e7ff">
                    <select id="ContentPlaceHolder1_dl_EventAfterRejectMailToPosition" 
                        name="ctl00$ContentPlaceHolder1$dl_EventAfterRejectMailToPosition" 
                        onchange="javascript:setTimeout('__doPostBack(\'ctl00$ContentPlaceHolder1$dl_EventAfterRejectMailToPosition\',\'\')', 0)" 
                        style="background-color: rgb(255, 204, 102);">
                        <option value="Please select">Please select</option>
                        <option selected="selected" value="UseKey">Use Dynamic Selected Key</option>  
                    </select>
                </td>
            </tr>
            <tr>
                <td bgcolor="#e7e7ff" valign="top">
                    DynamicFiledSaveValueToselect</td>
                <td bgcolor="#e7e7ff">
                    <input id="ContentPlaceHolder1_txt_DynamicFiledRejectValueToselect" 
                        name="ctl00$ContentPlaceHolder1$txt_DynamicFiledRejectValueToselect" 
                        style="width: 200px;" type="text" value="{A}" /> <em>
                    Example : {Division}
                    <br />
                    ระบบจะเลือกให้ตามค่าที่ระบุไว้ใน Position : Selected key</em>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
        <br />
    
    
    </div>


        <hr />
    <br />
    <h3>Function Auto Number</h3>
    <div>
    
    
<br /> โครงสร้าง : <b> SQL{AutoNumber 'Field ETC','Format',running digit}</b>
<br /> Field ETC  คือ  ETC field ในระบบ จะมีให้ ETC1-ETC5  โดยต้องใช้ SQL update ค่าเพื่อ show ในหน้า Tracking history 
<br /> Format คือ รูปแบบของ Running number โดยสามารถใช้ YY เพื่อแทน ปี  หรือ dd เพื่อแทน  วันได้ 
<br />
<br /> ตัวอย่าง : SQL{AutoNumber 'ETC1','ware-YY-',2}  
<br /> ผลลัพธ์  :  ware-17-01
<br />
<br />
<br />
</div>


    </div>
    </form>
</body>
</html>
