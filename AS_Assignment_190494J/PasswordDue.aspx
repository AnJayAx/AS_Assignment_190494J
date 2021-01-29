<%@ Page Title="" Language="C#" MasterPageFile="~/Secret.Master" AutoEventWireup="true" CodeBehind="PasswordDue.aspx.cs" Inherits="AS_Assignment_190494J.PasswordDue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
    <script type="text/javascript">
        function validate() {
            var passStr = document.getElementById('<%=tb_NewPassword.ClientID%>').value;

            if (passStr.length < 8) {
                document.getElementById("<%=lb_NewPassError.ClientID%>").innerHTML = "Password requires at least 8 characters";
                document.getElementById("<%=lb_NewPassError.ClientID%>").style.color = "Red";
                return ("too short");
            }

            else if (passStr.search(/[0-9]/) == -1) {
                document.getElementById("<%=lb_NewPassError.ClientID%>").innerHTML = "Password requires at least 1 number";
                document.getElementById("<%=lb_NewPassError.ClientID%>").style.color = "Red";
                return ("no number");
            }

            else if (passStr.search(/[a-z]/) == -1) {
                document.getElementById("<%=lb_NewPassError.ClientID%>").innerHTML = "Password requires at least 1 lower case";
                document.getElementById("<%=lb_NewPassError.ClientID%>").style.color = "Red";
                return ("no lower caser");
            }

            else if (passStr.search(/[A-Z]/) == -1) {
                document.getElementById("<%=lb_NewPassError.ClientID%>").innerHTML = "Password requires at least 1 upper case";
                document.getElementById("<%=lb_NewPassError.ClientID%>").style.color = "Red";
                return ("no upper case");
            }


            else if (passStr.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("<%=lb_NewPassError.ClientID%>").innerHTML = "Password requires at least 1 special character";
                document.getElementById("<%=lb_NewPassError.ClientID%>").style.color = "Red";
                return ("no special character");
            }

            document.getElementById("<%=lb_NewPassError.ClientID%>").innerHTML = "Excellent"
            document.getElementById("<%=lb_NewPassError.ClientID%>").style.color = "Blue";
        }

        function alertSuccess() {
            Swal.fire({
                title: 'Congratulations!',
                text: 'You successfully updated your account!',
                icon: 'success'
            }).then(function () {
                window.location = "SignIn.aspx";
            })
        }

        function alertFailed() {
            Swal.fire({
                title: 'Failed!',
                text: 'Please make sure to fill up all the textboxes!',
                icon: 'error'
            })
        }

        function alertMinPassAge() {
            Swal.fire({
                title: 'Warning!',
                text: 'You can only reset your password 5 mins after last edited!',
                icon: 'warning'
            })
        }
    </script>

    <table class="nav-justified">
        <tr>
            <td>
                <h1>Reset Password</h1>
            </td>
        </tr>
        <tr>
            <td>
                <p>You have to change your password before you can log in again</p>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>New Password</td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="tb_NewPassword" runat="server" Width="200px"></asp:TextBox>
                <asp:Label ID="lb_NewPassError" runat="server" ForeColor="#990000"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Confirm New Password</td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="tb_ConfirmPass" runat="server" Width="200px"></asp:TextBox>
                <asp:Label ID="lb_ConfirmPassError" runat="server" ForeColor="#990000"></asp:Label>

            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btn_Submit" runat="server" Text="Submit" OnClick="btn_Submit_Click"></asp:Button></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>

</asp:Content>
