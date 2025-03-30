using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

public partial class openTableau : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //   /t/BSTGroup/views/Result2017VABindexSurvey/Summary
        if (Request.QueryString["TBview"] != null && Request.QueryString["Site"] != null)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://insight.bst.co.th/trusted");

            var postData = "username=MPO";
            postData += "&client_ip=guru.bst.co.th&target_site=" + Request.QueryString["Site"].ToString();
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            if (responseString.ToString() != "-1")
            {
                Response.Redirect("https://insight.bst.co.th/trusted/" + responseString + "/t/" + Request.QueryString["Site"].ToString()+"/"+Request.QueryString["TBview"].ToString() + "?iframeSizedToWindow=true&:embed=y&:showAppBanner=false&:display_count=no&:showVizHome=no#5");
            }
        }
        else
        {
            Response.Write("can't display");
        }

    }
}