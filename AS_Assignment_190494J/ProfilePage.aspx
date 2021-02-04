<%@ Page Title="" Language="C#" MasterPageFile="~/LoggedIn.Master" AutoEventWireup="true" CodeBehind="ProfilePage.aspx.cs" Inherits="AS_Assignment_190494J.ProfilePage" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://www.google.com/recaptcha/api.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.inputmask/3.3.4/jquery.inputmask.bundle.js"></script>
    <script type="text/javascript">
        function validate() {
            var passStr = document.getElementById('<%=tb_NewPass.ClientID%>').value;

            if (passStr.length < 8) {
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").innerHTML = "Password requires at least 8 characters";
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").style.color = "Red";
                return ("too short");
            }

            else if (passStr.search(/[0-9]/) == -1) {
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").innerHTML = "Password requires at least 1 number";
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").style.color = "Red";
                return ("no number");
            }

            else if (passStr.search(/[a-z]/) == -1) {
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").innerHTML = "Password requires at least 1 lower case";
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").style.color = "Red";
                return ("no lower caser");
            }

            else if (passStr.search(/[A-Z]/) == -1) {
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").innerHTML = "Password requires at least 1 upper case";
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").style.color = "Red";
                return ("no upper case");
            }


            else if (passStr.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").innerHTML = "Password requires at least 1 special character";
                document.getElementById("<%=lb_ErrorNewPass.ClientID%>").style.color = "Red";
                return ("no special character");
            }

            document.getElementById("<%=lb_ErrorNewPass.ClientID%>").innerHTML = "Excellent"
            document.getElementById("<%=lb_ErrorNewPass.ClientID%>").style.color = "Blue";
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
                text: 'Please make sure you have filled in correctly!',
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

        function toggleVisibility(me) {
            me.style.hidden = "";
            return true;
        }

        $(function () {
            $('#toggle-password').on('change', function (e) {
                var _this = $(this);

                if (_this.is(':checked')) {
                    $('#<%=tb_NewPass.ClientID %>').attr({
                        'type': 'text'
                    });
                } else {
                    $('#<%=tb_NewPass.ClientID%>').attr({
                        'type': 'password'
                    });
                }
            })
        });
    </script>
    <h1>Profile Page</h1>
    <table class="nav-justified">
        <tr>
            <td class="modal-sm" style="width: 301px">First Name</td>
            <td>Last Name</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:TextBox ID="tb_FName" runat="server" Width="200px" ReadOnly="True"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="tb_LName" runat="server" Width="200px" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">Email</td>
            <td>Date of Birth</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:TextBox ID="tb_Email" runat="server" Width="200px" ReadOnly="True"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="tb_DOB" runat="server" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="height: 20px; width: 301px"></td>
            <td style="height: 20px"></td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">Credit Card No.</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:TextBox ID="tb_CreditCard" runat="server" Width="200px" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px; height: 20px;"></td>
            <td style="height: 20px"></td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:Button ID="btn_ResetPass" runat="server" Text="Click to reset password" OnClick="btn_ResetPass_Click" onclientclick="toggleVisibility(this)"/>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:Label ID="lb_OldPass" runat="server" Text="Old Pass" Visible="False"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lb_NewPass" runat="server" Text="New Pass" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:TextBox ID="tb_OldPass" runat="server" Visible="False" Width="200px" TextMode="Password"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="tb_NewPass" runat="server" Visible="False" Width="200px" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                <asp:Label ID="lb_ErrorNewPass" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>

        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:Label ID="lb_ErrorPass" runat="server" Visible="False"></asp:Label>
            </td>
            <td>&nbsp;
            </td>
        </tr>

        <tr>
            <td class="modal-sm" style="width: 301px">&nbsp;</td>
            <td>
                <asp:Label ID="lb_ConfirmNewPass" runat="server" Text="Confirm New Pass" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">&nbsp;</td>
            <td>
                <asp:TextBox ID="tb_ConfirmNewPass" runat="server" Visible="False" Width="200px" TextMode="Password"></asp:TextBox>
                <asp:Label ID="lb_ErrorConfirmPass" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">
                <asp:Button ID="btn_UpdatePass" runat="server" OnClick="btn_UpdatePass_Click" Text="Reset Password" Visible="False" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="modal-sm" style="width: 301px">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    
</asp:Content>
