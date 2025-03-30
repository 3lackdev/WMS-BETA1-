<%@ Page Language="C#"  AutoEventWireup="true" CodeFile="WorkflowRuning.aspx.cs" Inherits="WorkflowRuning" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="CACHE-CONTROL" content="NO-CACHE">
<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title><asp:Literal ID="wftitle" runat="server"></asp:Literal></title>
    <link href="Css/layout.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="DatePicker/jquery-ui.css">

  <script src="DatePicker/jquery-1.10.2.js"></script>
  <script src="DatePicker/jquery-ui.js"></script>
    <script src="js/jquery.popupoverlay.js"></script>
  <script type="text/javascript" src="Inputmark/dist/jquery.inputmask.bundle.js" charset="utf-8"></script>


  <script>


      function TableDataBind(id, triggerid) {
          try {
              var filterfield = $('#' + id).attr('data-filter');
              var bindname = $('#' + id).attr('data-bindname');
              if (filterfield == undefined) {
                  filterfield = '';
              }
              else {

                  var fied = filterfield.split('{');
                  for (var j = 0; j < fied.length; j++) {
                      if (fied[j].indexOf(',') > 0) {
                          var fresult = fied[j].split(',');

                          try {
                              var v = $('#' + fresult[0]).val();
                              filterfield = filterfield.replace('{' + fresult[0], '{' + v);
                          } catch (ex) { }
                      }
                  }

                  filterfield = "&filter=" + filterfield;

              }


              $("#" + id).find(".TableDataBindRow").hide();
              $("." + id + "-row").remove();
              $.getJSON("TableDataBind.aspx?dat=" + Date.now() + "&bindname=" + bindname + filterfield, function (data) {


                  for (var i = 0; i < data.length; i++) {
                      var $tr = $("#" + id).find(".TableDataBindRow").clone();
                      $tr.removeClass('TableDataBindRow');
                      $tr.attr('id', id + '-row' + i);
                      $tr.addClass(id + "-row");

                      $tr.children('td').each(function () {

                          var fied = $(this).text().split('{');
                          for (var j = 0; j < fied.length; j++) {
                              if (fied[j].indexOf('}') > 0) {
                                  var fresult = fied[j].replace('}', '');
                                  try {
                                      $(this).text(eval('data[i].' + fresult));
                                      $(this).addClass(id + "-col");
                                  } catch (ex) { }
                              }
                          }


                      });
                      $tr.show();
                      $('#' + id).append($tr);
                      $tr = null;
                  }

                  $('#' + id).show();
                  $('#' + id + '-htbb').val($('#' + id).html());
                  if (data.length > 0)
                      $('#' + id).show();
                  else
                      $('#' + id).hide();

                 

              });
          } catch (exx) { alert(exx);  }
      }


      function masterpopupiframeURL(url) {
          var doc = document.getElementById('masterpopupiframe');
          if (doc != null) {
              doc.className = "loading";
              doc.src = url;
          }
      }

      $(function () {
          $(".datepicker").datepicker();
          $(".datepicker").datepicker("option", "dateFormat", "yy-mm-dd");

          $(".LookupEmail").each(function () {

              $(this).click(function () { LookupEmail(this.id) });
              $(this).addClass('masterpopup_open');
              $(this).css("background", " url(images/find1.png) no-repeat right");
              if ($(this).attr('accesskey') == 'require')
                  $(this).css("background-color", 'yellow');
              $(this).css("padding-right", "15px");
              $(this).css("background-size", "contain");

              if ($(this).prop("tagName") == "TEXTAREA") {
                  $(this).css("background-size", "");
              }

              if ($(this).attr('disabled') == 'disabled') {
                  $(this).css("background", "#FAF8F8 url(images/find1.png) no-repeat right");
              }


          });


          $(".LookupDataBind").each(function () {
              $(this).click(function () { LookupDataBind(this.id, this.title) });
              $(this).addClass('masterpopup_open')
              $(this).css("background", " url(images/find1.png) no-repeat right");
              if ($(this).attr('accesskey') == 'require')
                  $(this).css("background-color", 'yellow');
              $(this).css("padding-right", "15px");
              $(this).css("background-size", "contain");

              if ($(this).attr('disabled') == 'disabled') {
                  $(this).css("background", "#FAF8F8 url(images/find1.png) no-repeat right");
              }


          });


          $(".TableDataBind").each(function (index) {
              try {
                  var triggerid = $(this).attr('data-triggerid');
                  //$(this).attr('id', 'TableDataBind-' + index);
                  if (!$('#' + triggerid).prop("disabled")) {
                      var newid = $(this).prop('id');
                      $(this).before('<input type=hidden id=' + newid + '-htbb name=' + newid + '-htbb />');
                      $("#" + newid).find(".TableDataBindRow").hide();
                      $('#' + newid + '-htbb').val($('#' + newid).html());
                      $('#' + triggerid).change(function () { TableDataBind(newid, this.id) });
                      if ($('.' + newid + '-row').length == 0) {
                          $(this).hide();
                      }
                  }
              } catch (exx) { alert(exx); }
          });

          $("[data-mark]").each(function () {
              try {
                  var mark = $(this).attr("data-mark");
                  $(this).inputmask(mark);
              } catch (exx) { }
          });


          $(".htmledit").each(function () {
              if (!$(this).prop("disabled")) {
                  $('#' + $(this).prop("id")).summernote({
  height: 150,   
  codemirror: { 
    theme: 'monokai'
  }
});
              }
              else {
                var html = $(this).val();
                var viewableText = $("<div>");
                viewableText.html(html);
                $(this).replaceWith(viewableText);
              }

          });

      });

      function LookupEmail(obj) {
          masterpopupiframeURL('lookupemail.aspx?parent=' + obj);
         
         // window.open('lookupemail.aspx?parent=' + obj, 'looup', 'toolbar=yes,scrollbars=yes,resizable=yes,width=800,height=500');
      }
      function LookupDataBind(obj, title) {
          
 	$('.modal-title').text('Find value.');
          try{
              var sp = title.split('&filter=');
            
          if (sp.length >1) {
              var a = sp[1].split('&');
              var co = a[0].split('{');
              for (i = 0; i < co.length; i++) {
                  var result = co[i].replace('}', '').split(',');
                  if (result.length > 1) {
                      title = title.replace('{' + result[0] + ',' + result[1], '{' + $('#' + result[0]).val() + ',' + result[1]);
                  }
              }
              
          }
          }catch(e){alert(e);}
          masterpopupiframeURL('lookupdatabind.aspx?parent=' + obj + '&' + title);
          //window.open('lookupdatabind.aspx?parent=' + obj+'&'+title, 'looup', 'toolbar=yes,scrollbars=yes,resizable=yes,width=1200,height=500');
      }


      function clearValue(objID) {
          var ID = '#' + objID;
          if ($(ID).attr("type") == "checkbox") {
              $(ID).prop('checked', '');

          }
          else {
              if ($(this).prop('disabled')) {
                  $(this).val('');
              }
          }
      }

      function checkGroupHasRequire(objID) {
          var ID = '#' + objID;
          var moveObj = false;
          if ($(ID).attr('group')) {
              $('input,select,textarea').filter('[group=' + $(ID).attr('group') + ']').each(function () {
                  if ($(this).prop('checked')) {
                      moveObj = true;
                  }
              });
              if (moveObj) {
                  $('input,select,textarea').filter('[group=' + $(ID).attr('group') + ']').each(function () {
                      if (!$(this).prop('checked')) {
                          $(this).removeAttr('accesskey');
                          $(this).css('background-color', '');
                      }
                      else {
                          $(this).attr('accesskey', 'require');
                          $(this).css('background-color', 'yellow');
                      }
                  });
              }
          }

       
      }


      function checkGroup(objID) {
          var ID = '#' + objID;
          var req = false;
          if ($(ID).attr('group')) {
              $('input[type=checkbox]').filter('[group=' + $(ID).attr('group') + ']').each(function () {
                  if ($(ID).attr('id') != $(this).attr('id')) {
                      clearValue($(this).attr('id'));
                      checkReqby($(this).attr('id'));
                      if ($(this).attr('accesskey') == 'require') {
                          req = true;
                          $(this).removeAttr('accesskey');
                          $(this).css('background-color', '');

                      }
                  }

              });


              if (req) {
                      $(this).attr('accesskey', 'require');
                      $(this).css('background-color', 'yellow');

              }
          }
  
      }

function checkReqby(objID) {
          var ID = '#' + objID;
          checkEnabledby($(ID).attr('id'));
        //  if ($(ID).attr('reqby')) {


          $('input,select,textarea').filter('[reqby~=' + $(ID).attr('id') + ']').each(function () {
              var reqq = $(ID).attr('id');
             // old reqq = $(this).attr('reqby');
              //alert($(ID).attr('id'));
              //alert($(this).attr('reqby'));
              // $('input,select,textarea').filter(function () { return ($(this).attr('reqby').indexOf($(ID).attr('id')) != -1) }).each(function () {
              //$('input,select,textarea').filter(function () { return $(this).attr('reqby').find($(ID).attr('id')) >= 0 }).each(function () {
              if ($(ID).attr('reqby') != $(this).attr('id')) {

                  var tmp = $(this).attr('ignoreval');
                  var ignore = false;
                  if (tmp != '' && tmp != undefined) {
                      ignore = (tmp.indexOf($(ID).val()) >= 0);
                  }

                  if ((($('#' + reqq).attr("type") == 'checkbox' && $('#' + reqq).prop("checked") == true) || ($('#' + reqq).val() != '' && $('#' + reqq).attr("type") != 'checkbox' && $('#' + reqq).val() != undefined)) && (!ignore)) {

                      $(this).attr('accesskey', 'require');
                      $(this).css('background-color', 'yellow');
                      //$(this).removeAttr('disabled');


                  } else {
                      $(this).attr('accesskey', '');
                      $(this).css('background-color', '');
                      //$(this).attr('disabled', 'disabled');


                      if ($(this).attr("type") == 'checkbox') {
                          $(this).prop("checked", "");
                      }
                      else {
                          if ($(this).prop('disabled')) {
                              $(this).val('');
                          }
                      }
                  }
              }

          });


          //}
      }

 function checkEnabledby(objID) {
          var ID = '#' + objID;
         
	    if($(ID).prop('disabled'))
 	    {
  		return;
	    }

            $('input,select,textarea').filter('[enabledby~=' + $(ID).attr('id') + ']').each(function () {
                var enabb = $(ID).attr('id');
                //old enabb = $(this).attr('enabledby');
              if ($(ID).attr('enabledby') != $(this).attr('id')) {

                  if ((($('#' + enabb).attr("type") == 'checkbox' && $('#' + enabb).prop("checked") == true) || ($('#' + enabb).val() != '' && $('#' + enabb).attr("type") != 'checkbox' && $('#' + enabb).val() != undefined))) {
                      $(this).css('background-color', '');
                      $(this).removeAttr('disabled');

                  } else {
                      $(this).css('background-color', '');
                      $(this).attr('disabled', 'disabled');

                    
                  }
              }

          });

      }


      $(document).ready(function () {


          $('input[type=checkbox],select').each(function () {
              $(this).change(function () {
                  checkGroup($(this).attr('id'));
                  checkReqby($(this).attr('id'));
                  checkGroupHasRequire($(this).attr('id'));

              });
              checkEnabledby($(this).attr('id'));
          });

          $('.defaultpostback').each(function () {
              if ($(this).prop('disabled')) {
                  $(this).removeClass('defaultpostback');
              }
              else {
                  $(this).attr('readonly', true);
                  $(this).attr('placeholder', 'Automatic value');
                  $(this).css('background-color', 'lightgreen');
                  $(this).css('color', '#ffffff');
              }
          });
          /* $('input[type=checkbox]').each(function () {
          if ($(this).attr('group')) //### have group
          {
          $(this).click(function () {
          var id = $(this).attr("id");
          var req = false;
          var reqby = "";
          var reqbyparent = "";
          $('input[type=checkbox]').filter('[group=' + $(this).attr('group') + ']').each(function () {

          if ($(this).attr("id") != id)  //### unchecked object
          {

          $(this).prop('checked', '');
          $(this).change();
          if ($(this).attr("accesskey") == 'require') {
          req = true;
          $(this).removeAttr('accesskey');
          $(this).css('background-color', '');
          }

          }
          else  //###  curent checked object
          {
          alert($(this).attr('reqby'));
          }
          });

          if (req) {
          $(this).attr('accesskey', 'require');
          $(this).css('background-color', 'yellow');
          }



          })
          }
          else //######### no have group
          {
          }

          });
          */
          /*
          //*****************
          $('input,select').each(function () {
          if ($(this).attr('reqby')) {
          if ($(this).attr("reqby") == $(this).attr("id")) {
          $(this).change(function () {
          //var ignore = ($(this).attr('ignoreval') == $(this).val());
          var tmp = $(this).attr('ignoreval');
          var ignore = false;
          if (tmp != '' && tmp != undefined) {
          ignore = (tmp.indexOf($(this).val()) >= 0);
          }

          alert($(this).attr("id"));

          $('input,select,textarea').filter('[reqby=' + $(this).attr('reqby') + ']').each(function () {
          if ($(this).attr("reqby") != $(this).attr("id")) {
          if ((($('#' + $(this).attr('reqby')).attr("type") == 'checkbox' && $('#' + $(this).attr('reqby')).prop("checked") == true) || ($('#' + $(this).attr('reqby')).val() != '' && $('#' + $(this).attr('reqby')).attr("type") != 'checkbox' && $('#' + $(this).attr('reqby')).val() != undefined)) && (!ignore)) {
          $(this).attr('accesskey', 'require');
          $(this).css('background-color', 'yellow');

          }
          else if ($(this).attr('reqby') != $(this).attr('id')) {
          $(this).attr('accesskey', '');
          $(this).css('background-color', '');

          //alert($(this).attr("id"));
          //if ($(this).attr("type") == 'checkbox') {
          //     $(this).prop("checked", "");
          // }
          // else {
          //     $(this).val('');
          // }



          }
          }

          });
          });
          }

          }
          });
          */
          //******************


          $('input[type=file]').each(function () {
              try {
                  if ($('#upload_' + $(this).attr('id')).length > 0) {

                      $(this).attr('accesskey', '');
                      $(this).css('border', '');
                      $(this).css('background-color', '');
                  }
              } catch (ex) { alert(ex); }
          });

      });

      function require(obj, val) {
      try{
          if (val) {
              $('#' + obj).attr('accesskey', 'require');
              $('#' + obj).css('background-color', 'yellow');
          }
          else {
              $('#' + obj).attr('accesskey', '');
              $('#' + obj).css('background-color', '');
          }
          }catch(ex){}
      }

      function show(obj, v) {
          try {

              var checksheet = document.getElementById(obj);
              if (v == '') {
                  if (checksheet.style.display == '')
                      checksheet.style.display = 'none';
                  else
                      checksheet.style.display = '';

                  return;
              }

              if (v || v == 'true') {
                  checksheet.style.display = '';
                  return;
              }

              if (!v || v == 'false') {
                  checksheet.style.display = 'none';
                  return;
              }
          } catch (ex) {return;}
      }

  </script>
</head>
<body style="margin-left:5px;margin-top:0px;margin-right:5px">
    <form id="form1" runat="server" enctype="multipart/form-data">
<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
<asp:HiddenField ID="system_loginname" runat="server" />
    <asp:HiddenField ID="system_loginfullname" runat="server" />
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server"> 
        <div>



    <script language="javascript">

        function hiddenbtn() {

            try { document.getElementById("pleasewait").innerHTML = "<b>(ห้ามปิดหรือออกจากหน้าจอจนกว่าจะประมวนผลเสร็จ)</b> Please wait button action is executing...";} catch (e) { }
            try { document.getElementById("btn_save").style.display = "none";} catch (e) { }
            try { document.getElementById("btn_approve").style.display = "none";} catch (e) { }
            try { document.getElementById("btn_reject").style.display = "none";} catch (e) { }

            document.getElementById('savepop').style.display = "inline";
            return true;
        }

        function OnSaveCheck() {
var pass = onSaveClick();
            if (pass || typeof pass=='undefined') {
                if (checkRequireField()) {
                    return hiddenbtn();
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }

            
        }

        function OnApproveCheck() {
var pass = onApproveClick();
            if (pass  || typeof pass=='undefined') {

                if (checkRequireField()) {

                    return hiddenbtn();

                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }

        }

        function OnRejectCheck() {
var pass = onRejectClick();
            if (pass  || typeof pass=='undefined') {

                if (checkRequireField()) {

                    return hiddenbtn();

                } else {
                    return false; 
                }
            }
            else {
                return false;
            }

        }


        function checkRequireField() {
            for (i = 0; i < document.forms["form1"].length; i++) {
                try {
                    var attribute = document.forms["form1"].elements[i].getAttribute("accesskey");
                    var id = document.forms["form1"].elements[i].getAttribute("id");
                    if (attribute != null) {
                        
                        if (id != null && attribute == "require" && document.forms["form1"].elements[i].disabled ==false) {

                            try {
                                if (document.forms["form1"].elements[i].type == 'checkbox' && document.forms["form1"].elements[i].value == "") {
                                    document.forms["form1"].elements[i].setAttribute("value", " ");
                                }
                            } catch (exx) {
                            }
                            

                            if (document.forms["form1"].elements[i].value == "" || document.forms["form1"].elements[i].value == undefined || (document.forms["form1"].elements[i].type == 'checkbox' && !document.forms["form1"].elements[i].checked)) {

                                alert("Please enter value of " + document.forms["form1"].elements[i].getAttribute("id"));
                                try { 
				document.forms["form1"].elements[i].focus(); 
 $("#"+document.forms["form1"].elements[i].getAttribute("id")).tooltip({
        items: 'input,select,textarea',
        content: "กรุณาระบุ Field Required นี้ด้วย!!!!",
	tooltipClass:"tooll",
    
    open: function (event, ui) {
            setTimeout(function () {
                $(ui.tooltip).hide('slideDown');
               $("#"+document.forms["form1"].elements[i].getAttribute("id")).tooltip("disable");
            }, 4000);
        }
    
  }).tooltip("open");


				} catch (ee) { }
                                return false;
                            }
                        }
                    }
                }
                catch (err) { alert(err.message);return false;}
            }
            return true;

        }

        function showguidelineall() {

            var a = document.getElementById("guidelinecontent");
            var b = document.getElementById("linkmore");

            if (a && b.innerHTML == "Read More") {
                a.setAttribute("style", "overflow: hidden;");
                b.innerHTML="Hide content";
            }
            else {
                a.setAttribute("style", "overflow: hidden;max-height:200px");
                b.innerHTML = "Read More";
                b.focus();
            }
            return false;
        }

    </script>

        <asp:Literal ID="otherscript" runat="server"></asp:Literal>
        <div style="text-align:left" class="topmenu">
            <asp:Label ID="lb_currentstep" runat="server" 
                Text="Step : " Font-Bold="True" Font-Size="Medium" ForeColor="#999999"></asp:Label>
                
                <asp:LinkButton ID="ln_switchuser" runat="server" Font-Bold="True" style="float:right;padding-right:5px" 
                Font-Size="Medium" ForeColor="#999999" onclick="ln_switchuser_Click">Switch User</asp:LinkButton>
            <asp:Label ID="Label3" ForeColor="#ffffff" Font-Size="Medium" Font-Bold="True" 
                style="float:right;" runat="server" Width="20px" Text=" -"></asp:Label>
                 <asp:LinkButton ID="LinkButton3" runat="server" Font-Bold="True" style="float:right;padding-right:5px" 
                Font-Size="Medium" ForeColor="#999999">Historical</asp:LinkButton>
            <asp:Label ID="Label5" runat="server" Text="You Are : " Font-Bold="True" style="float:right;padding-right:25px" 
                Font-Size="Medium" ForeColor="#999999"></asp:Label>

            <asp:Panel ID="childflow" Visible="false" style="z-index:99;border:solid 1px;border-color:gray;width: auto; height: 30px; bottom: 5px;padding-left: 10px; padding-top: 10px; position: fixed; background-color: wheat;" runat="server">
            </asp:Panel>
</div> 
                <div runat="server" id="guideline" class="guideline" visible="false">
                <br />
                <section style="background: #f4f4f4 none repeat scroll 0 0;border-color: rgba(0, 0, 0, 0.3);border-radius: 0.3em;border-style: solid;border-width: 1px;box-shadow: 1px 1px 3px rgba(0, 0, 0, 0.2) inset;color: rgba(0, 0, 0, 0.8);margin: 0 0 1.5em;padding: 1.42857em;">
							<h2>Guideline : How to do</h2>
                            <div id="guidelinecontent" style="max-height:200px;overflow:hidden">
							<asp:Literal runat="server" id="guideline_content"></asp:Literal>
                            </div><br /><br />
<asp:LinkButton id="linkmore" runat="server" onclientclick="return showguidelineall()">Read More</asp:LinkButton>
						</section>
                </div>
        <hr class="topmenu" />
        <asp:Literal ID="content" runat="server"></asp:Literal>

    <div id="uploadfile" runat="server">
      <br />
    <br />


     <asp:UpdatePanel ID="UpdatePanel1" runat="server">     
     <ContentTemplate>
       <style>
.overlay 
{
 position:fixed;
 background-color: white;
 top: 100px;
 left: 0px;
 width: 98%;
 height: 100%;
 opacity: 0.3;
 -moz-opacity: 0.3;
 filter: alpha(opacity=30);
 -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=30)";
 z-index: 1000;
}
.non
{
    display:none;
}

.tooll{

}


</style>
        <hr />
        
    <div><h4><asp:LinkButton ID="LinkButton1" runat="server" 
            onclick="LinkButton1_Click">จัดการ File Upload Status</asp:LinkButton></h4></div>
    <div runat="server" id="filemgr" visible="false" class="upload" style="width:97%;border:1px solid;min-height:150px;padding:10px 10px 10px 10px;">
    
        <div style="min-height:70px"><asp:Literal ID="content_upload" runat="server"></asp:Literal>

            <br />
            <asp:Button ID="Button4" runat="server" BackColor="#CCCCCC" 
                Text="Delete Selected" ForeColor="#FF3300" onclick="Button4_Click" 
                onclientclick="return confirm('Comfirm delete?')" />
            <br /><br />

            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" 
                CellPadding="4" ForeColor="Black" GridLines="Horizontal" Width="98%">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="id">
                    <ControlStyle Width="1px" />
                    <HeaderStyle Width="1px" />
                    <ItemStyle Width="1px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Select">
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" />
                        </EditItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" Width="60px" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFields="id" 
                        DataNavigateUrlFormatString="download.aspx?id={0}" DataTextField="name" 
                        HeaderText="FileName" Target="_blank">
                    <HeaderStyle HorizontalAlign="Left" />
                    </asp:HyperLinkField>
                    <asp:BoundField DataField="CreateBy" HeaderText="Upload By">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CreateDate" HeaderText="Upload Date">
                    <HeaderStyle HorizontalAlign="Left" Width="150px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Delete">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton2_Click" 
                                onclientclick="return confirm('Delete ?')">Delete</asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" Width="50px" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Label ID="Label2" runat="server" Font-Italic="True" Font-Size="Small" 
                        ForeColor="Gray" Text="ยังไม่มี File upload "></asp:Label>
                </EmptyDataTemplate>
                <FooterStyle BackColor="#CCCC99" ForeColor="Black" />
                <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
                <SelectedRowStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F7F7F7" />
                <SortedAscendingHeaderStyle BackColor="#4B4B4B" />
                <SortedDescendingCellStyle BackColor="#E5E5E5" />
                <SortedDescendingHeaderStyle BackColor="#242121" />
            </asp:GridView>
            <br />
        </div>
        <div runat="server" id="uploadmore">
        Upload More File :

        <asp:FileUpload ID="FileUpload1" runat="server" />
        &nbsp;
        <asp:Button ID="Button3" runat="server" onclick="Button3_Click" Text="Upload" /><br /><br />
</div>
        
            </div>

            <div runat="server" id="utility_update" visible=false>
 <script>

                $(document).ready(function () {

                    $('input,textarea').each(function () {
                        $(this).removeAttr('disabled');
			$(this).removeAttr('readonly');
                    });

                });

            </script>
            <br /><br />
            <hr />
                <asp:Label ID="Label4" runat="server" Text="Update Field ETC" Font-Bold="True" 
                    Font-Size="15px"></asp:Label>
                    &nbsp;<br />
                    <br />
                <table style="width:100%;">
                    <tr>
                        <td bgcolor="#CCCCCC" class="style1" width="100px">
                            ETC Name</td>
                        <td bgcolor="#CCCCCC" class="style1" width="300">
                            Value</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc1" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc1" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc2" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc2" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc3" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc3" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc4" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc4" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc5" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc5" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc6" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc6" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc7" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc7" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc8" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc8" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc9" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc9" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_etc10" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txt_etc10" runat="server" Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
                <br />
            </div>
            </ContentTemplate>  
            <Triggers>
            <asp:PostBackTrigger ControlID="Button3" />
            </Triggers>
      </asp:UpdatePanel>
      <asp:UpdateProgress ID="UpdateProgress2" DynamicLayout="true" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter=5 >
    <ProgressTemplate><div class="overlay"><center><br /><br /><img src="Images/ajaxloader.gif" alt="" /></center></div></ProgressTemplate>
</asp:UpdateProgress>

    </div>
        <br />
    <div style="text-align:center">
        <br />
        <br />

        <asp:Button ID="btn_save" runat="server" Text="&nbsp;&nbsp;&nbsp;Save&nbsp;&nbsp;&nbsp;" style="width:auto"
             BorderStyle="Solid" BorderWidth="1px" Font-Bold="True" 
            Height="30px" onclick="btn_save_Click" 
            onclientclick="return checkRequireField()" CssClass="Save" />&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btn_approve" runat="server" Text="&nbsp;&nbsp;&nbsp;Approve&nbsp;&nbsp;&nbsp;" style="width:auto" 
             BorderStyle="Solid" BorderWidth="1px" Font-Bold="True" 
             Height="30px" onclick="btn_approve_Click" 
            CssClass="Approve" />
&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btn_reject" runat="server" Text="&nbsp;&nbsp;&nbsp;Reject&nbsp;&nbsp;&nbsp;" style="width:auto" 
            BorderStyle="Solid" BorderWidth="1px" Font-Bold="True" 
            Height="30px" onclick="btn_reject_Click" 
            CssClass="Reject" />
        <asp:HiddenField ID="currentstep_rowid" runat="server" />
        <br /><p id="pleasewait">&nbsp;</p>
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <br />
        <br />
        <br />
        <br />
        <br />
        </div>
    </div>
        </asp:View>
        <asp:View ID="View2" runat="server">
        <div align="center">
        
            <asp:HyperLink ID="hp1" runat="server" ForeColor="White" NavigateUrl="#">Result</asp:HyperLink>
        
            <br />
            <asp:Button ID="Button2" runat="server" onclick="Button1_Click" 
                Text="Back to home" Visible="False" />
            <br />
            <br />
            <br />
            <a name="message"></a>
            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="XX-Large" 
                ForeColor="#33CC33" 
                Text="Action Completed &lt;br /&gt; Next step will be started......." 
                Visible="False"></asp:Label>

                  <div>
                
                <style>
                
                
#container1 {
    box-shadow: 0 0 10px 5px rgba(0, 0, 0, 0.2);
    margin: 25px auto;
    max-width: 745px;
    min-width: 300px;
    width: 55%;
}
#header {
    background: #3498db none repeat scroll 0 0;
    display: block;
    height: 65px;
    margin: auto;
    opacity: 0.9;
    padding: 15px 35px;
}

#content1 {
    background: #ffffff none repeat scroll 0 0;
    display: block;
    padding: 15px 35px;
}
                </style>
                <section id="container1">
<div id="header">
<h1 style="color: #FFFFFF">Action Successfully.<asp:Label ID="lb_tiger" 
        runat="server" ForeColor="Red"></asp:Label>
    </h1>
<h2>&nbsp;</h2>
</div>
<div id="content1">
<div id="form">
<div id="easy">
    <br />
    <br />
    Action successed next process will be started. Now 
    <a href='javascript:window.close()'>
    <u>you can click here for close page</u></a> or click 
    button below for back.<br />
    <br />
</div>
</div>
</div>
</section>
                
                </div>
            <br />
            <br />
            <br />
            <br />
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                Text="Back" BorderStyle="Solid" BorderWidth="1px" Width="100px" />
            <br />
        
        </div>
        </asp:View>
    </asp:MultiView>
   
    </form>
    <style  type="text/css"> 
.dim
{
            height:100%;
            width:100%;
            position:fixed;
            left:0;
            top:0;
            z-index:1 !important;
            background-color:black;
            filter: alpha(opacity=60); /* internet explorer */
-khtml-opacity: 0.60;      /* khtml, old safari */
-moz-opacity: 0.60;       /* mozilla, netscape */
opacity: 0.60;           /* fx, safari, opera */
}
 
 
.dialog_wrapper { width: 100%; top: 0px; left: 0px; position: absolute; z-index: 5; display: block;  }
 
.dialog {
	position: fixed;
	left: 50%;
	width: 600px;
	margin-left: -300px;
	height:300px;
	margin-top:50px;
}
#childflow a:hover
{
    color:orange;
}
.loading
{
   background:url("images/ajaxloader.gif") no-repeat;
   background-position-x:center;
   background-position-y:center;
   z-index:1000;
}
</style>
<div id="savepop" style="display:none">
<div class="dim" >
</div>  

<div class="dialog_wrapper">
<div class="dialog"><center><br />
    <img id="ctl00_ContentPlaceHolder1_Image1" src="images/loadingc.gif" style="height:auto;width:auto;border-width:0px;" /></center></div>
</div>
</div>


<!--###############################    popup -->
<div id="masterpopup" class="well" style="width:90%;overflow:hidden;height:90%;background-color:#ffffff;margin-top:20px;display:none">
<iframe name="masterpopupiframe" id="masterpopupiframe" src="" style="overflow:auto; border: 0px solid;height: 90%;width:100%"></iframe>
    <center><hr />
        <button class="masterpopup_close btn btn-default" id="closebtn">Close</button></center>
</div>

<script language="javascript">
    $(document).ready(function () {

        $('#masterpopup').popup({
            focusdelay: 200,
            outline: true,
            vertical: 'top'
        });


    });
</script>

<style type="text/css">
    #masterpopup_background { -webkit-transition: all 0.3s 0.3s;-moz-transition: all 0.3s 0.3s;transition: all 0.3s 0.3s;}
    #masterpopup,#masterpopup_wrapper {-webkit-transition: all 0.4s; -moz-transition: all 0.4s;transition: all 0.4s;}
    #masterpopup {-webkit-transform: translateX(0) translateY(-40%);-moz-transform: translateX(0) translateY(-40%); -ms-transform: translateX(0) translateY(-40%);transform: translateX(0) translateY(-40%);}
    .popup_visible #masterpopup {-webkit-transform: translateX(0) translateY(0);-moz-transform: translateX(0) translateY(0);-ms-transform: translateX(0) translateY(0);transform: translateX(0) translateY(0);}
</style>
<!--###############################    popup -->

</body>
</html>
