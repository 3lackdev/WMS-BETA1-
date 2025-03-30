
Partial Class login
    Inherits BST.libDB

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Response.Write(Session("lastpage"))
        If Request.QueryString("logout") = Nothing Then
            Me.GetCurrentWindowAthen()

            If Session("lastpage") <> Nothing Then
                If Session("loginname").ToString().ToUpper() = "IUSR" Then
                    Session("loginname") = Nothing
                    Response.Redirect("login.aspx?logout=y")
                End If
                Dim las As String = CType(Session("lastpage").ToString(), String)
                Session("lastpage") = Nothing
                Response.Redirect(las)
            Else
                Response.Redirect("default.aspx")
            End If

        End If

        If Not IsPostBack Then

           
            If (Not (Request.Cookies("UserName")) Is Nothing) Then
                Try
                    username.Text = Request.Cookies("UserName").Value
                    domain.SelectedValue = Request.Cookies("Domain").Value
                    password.Attributes("value") = Request.Cookies("Password").Value
                    password.Text = Request.Cookies("Password").Value
                    If (username.Text <> "" And Session("lastpage") <> Nothing And Request.QueryString("clear") = Nothing) Then
                        Dim z As New System.Web.UI.ImageClickEventArgs(1, 1)
                        ImageButton1_Click(Nothing, Nothing)
                    End If
                Catch
                End Try
            Else
                username.Text = ""
                password.Text = ""
                password.Attributes("value") = ""
                domain.SelectedValue = ServerDomain
            End If

            If Request.QueryString("clear") <> Nothing Then
                If Request.QueryString("clear").ToString().ToLower() = "true" Then

                    Response.Cookies("UserName").Expires = DateTime.Now.AddDays(-30)
                    Response.Cookies("Password").Expires = DateTime.Now.AddDays(-30)
                    Response.Cookies("Domain").Expires = DateTime.Now.AddDays(-30)
                    Response.Cookies("UserName").Value = ""
                    Response.Cookies("Password").Value = ""
                    Response.Cookies("Domain").Value = ""
                    Session("loginfullname") = Nothing

                    Session("loginname") = Nothing
                    Session("logindomain") = Nothing
                    Button1.Visible = False
                    Try
                        username.Text = ""
                        password.Text = ""
                        password.Attributes("value") = ""
                        domain.SelectedValue = ServerDomain
                    Catch
                    End Try
                End If
            End If


            If Session("loginname") <> Nothing And password.Text <> "" Then
                Button1.Text = "Log out >> " + Session("loginname").ToString()
                Button1.Visible = True
            Else
                Button1.Visible = False
            End If

        End If
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click

        If username.Text.ToUpper().IndexOf("JBE.CO.TH") > 0 Then
            domain.SelectedValue = "JBE.CO.TH"
            username.Text = username.Text.ToLower().Replace("@jbe.co.th", "")

        End If

        If username.Text.ToUpper().IndexOf("BST.CO.TH") > 0 Then
            domain.SelectedValue = "BST.CO.TH"
            username.Text = username.Text.ToLower().Replace("@bst.co.th", "")
        End If

        ' Response.Write(domain.SelectedValue + username.Text + password.Text)


        Dim a As Boolean = IsAuthenticated(domain.SelectedValue, username.Text, password.Text)
        If a Then

            Session("loginfullname") = username.Text.Replace("@jbe.co.th", "").Replace("@bst.co.th", "")

            Session("loginname") = username.Text.Replace("@jbe.co.th", "").Replace("@bst.co.th", "")
            Session("logindomain") = domain.Text

            '########### Remember
            If chkRememberMe.Checked Then
                Response.Cookies("UserName").Expires = DateTime.Now.AddDays(30)
                Response.Cookies("Password").Expires = DateTime.Now.AddDays(30)
                Response.Cookies("Domain").Expires = DateTime.Now.AddDays(30)
                Response.Cookies("UserName").Value = username.Text.Trim
                Response.Cookies("Password").Value = password.Text.Trim
                Response.Cookies("Domain").Value = domain.SelectedValue.Trim
            Else
                Response.Cookies("UserName").Expires = DateTime.Now.AddDays(-1)
                Response.Cookies("Password").Expires = DateTime.Now.AddDays(-1)
                Response.Cookies("Domain").Expires = DateTime.Now.AddDays(-1)
                Response.Cookies("UserName").Value = ""
                Response.Cookies("Password").Value = ""
                Response.Cookies("Domain").Value = ""
            End If

            '###########

            If Session("lastpage") <> Nothing Then
                Dim las As String = CType(Session("lastpage").ToString(), String)
                Session("lastpage") = Nothing
                Response.Redirect(las.ToString().Replace("username=", "usernamexxx="))
            End If
            Response.Redirect("default.aspx")
        Else
            Label1.Visible = True
        End If
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Response.Cookies("UserName").Expires = DateTime.Now.AddDays(-1)
        Response.Cookies("Password").Expires = DateTime.Now.AddDays(-1)
        Response.Cookies("Domain").Expires = DateTime.Now.AddDays(-1)
        Response.Cookies("UserName").Value = Nothing
        Response.Cookies("Password").Value = Nothing
        Response.Cookies("Domain").Value = Nothing

        Session("loginfullname") = Nothing

        Session("loginname") = Nothing
        Session("logindomain") = Nothing

        Response.Redirect("login.aspx?logout=y")
    End Sub
End Class
