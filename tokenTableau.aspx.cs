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
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            | SecurityProtocolType.Tls11
            | SecurityProtocolType.Tls12
            | SecurityProtocolType.Ssl3;

            var request = (HttpWebRequest)WebRequest.Create("https://vmprdtableau01.bst.co.th/trusted");

            var postData = "username=guru_viewer";
            postData += "&client_ip=gurudev.bst.co.th&target_site=BSTGroup";
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
			Response.Write("Token : "+responseString);
			
			
           // if (responseString.ToString() != "-1")
           // {
           //     Response.Redirect("https://insight.bst.co.th/trusted/" + responseString + "/t/" + Request.QueryString["Site"].ToString()+"/"+Request.QueryString["TBview"].ToString() + "?iframeSizedToWindow=true&:embed=y&:showAppBanner=false&:display_count=no&:showVizHome=no#5");
           // }


    }
}