<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="AS_Assignment_190494J.SignUp" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://www.google.com/recaptcha/api.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
    <script type="text/javascript">
        function validate() {
            var passStr = document.getElementById('<%=tb_Password.ClientID%>').value;

            if (passStr.length < 8) {
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").innerHTML = "Password requires at least 8 characters";
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").style.color = "Red";
                return ("too short");
            }

            else if (passStr.search(/[0-9]/) == -1) {
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").innerHTML = "Password requires at least 1 number";
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").style.color = "Red";
                return ("no number");
            }

            else if (passStr.search(/[a-z]/) == -1) {
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").innerHTML = "Password requires at least 1 lower case";
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").style.color = "Red";
                return ("no lower caser");
            }

            else if (passStr.search(/[A-Z]/) == -1) {
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").innerHTML = "Password requires at least 1 upper case";
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").style.color = "Red";
                return ("no upper case");
            }


            else if (passStr.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").innerHTML = "Password requires at least 1 special character";
                document.getElementById("<%=lb_PasswordCheck.ClientID%>").style.color = "Red";
                return ("no special character");
            }

            document.getElementById("<%=lb_PasswordCheck.ClientID%>").innerHTML = "Excellent"
            document.getElementById("<%=lb_PasswordCheck.ClientID%>").style.color = "Blue";
        }

        function alertSucess() {
            Swal.fire({
                title: 'Congratulations!',
                text: 'You successfully registered your account!',
                icon: 'success'
            }).then(function () {
                window.location = "SignIn.aspx";
            })
        }

        function alertInput(activationCode) {
            swal({
                title: "Enter your activation code!",
                text: "Check your email for your activation code and enter it below",
                type: "input",
                showCancelButton: true,
                closeOnConfirm: false,
                animation: "slide-from-top",
            },
                function (inputValue) {
                    if (inputValue === false) return false;

                    if (inputValue === "") {
                        swal.showInputError("You need to write something!");
                        return false
                    }

                    if (inputValue == activationCode) {
                        window.location = "SignIn.aspx";
                       
                    }
                    else {
                        swal.showInputError("Incorrect Code! Try again");
                        return false
                    }

                });
        }

        function alertFailed() {
            Swal.fire({
                title: 'Failed!',
                text: 'Please make sure to fill up all the textboxes!',
                icon: 'error'
            })
        }

        //function showpassword(checkbox) {
        //    var passwordtextbox = document.getelementbyid('tb_password');
        //    if (checkbox.checked == true) {
        //        passwordtextbox.setattribute("type", "text");
        //    }
        //    else {
        //        passwordtextbox.setattribute("type", "password");
        //    }
        //}

        $(function () {
            $('#toggle-password').on('change', function (e) {
                var _this = $(this);

                if (_this.is(':checked')) {
                    $('#<%=tb_Password.ClientID %>').attr({
                        'type': 'text'
                    });
                } else {
                    $('#<%=tb_Password.ClientID%>').attr({
                        'type': 'password'
                    });
                }
            })
        });
    </script>
    <h1>Registration</h1>
    <p>Please fill in all your credentials and make sure it is correct before submitting.</p>
    <table class="nav-justified">
        <tr>
            <td style="height: 19px; width: 592px;">&nbsp;</td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px;">First Name</td>
        </tr>
        <tr>
            <td style="width: 592px; height: 25px;">
                <asp:TextBox ID="tb_FirstName" runat="server" Width="400px"></asp:TextBox>
                <asp:Label ID="lb_FNameCheck" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px"></td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px">Last Name</td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px">
                <asp:TextBox ID="tb_LastName" runat="server" Width="400px"></asp:TextBox>
                <asp:Label ID="lb_LNameCheck" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px">&nbsp;</td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px">Date of Birth (DD/MM/YYYY)</td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px">
                <asp:TextBox ID="tb_BirthDate" runat="server" Width="150px" TextMode="Date" onkeydown="return false"></asp:TextBox>
                <asp:Label ID="lb_DOBCheck" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px">&nbsp;</td>
        </tr>
        <tr>
            <td style="height: 20px; width: 592px">Credit Card</td>
        </tr>
        <tr>
            <td style="height: 22px; width: 592px">
                <asp:TextBox ID="tb_CreditCard" runat="server" Width="400px"></asp:TextBox>
                <asp:Label ID="lb_CreditCheck" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 592px">&nbsp;</td>
        </tr>
        <tr>
            <td style="height: 20px; width: 592px">Email Address</td>
        </tr>
        <tr>
            <td style="width: 592px">
                <asp:TextBox ID="tb_Email" runat="server" Width="400px"></asp:TextBox>
                <asp:Label ID="lb_EmailCheck" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 20px; width: 592px"></td>
        </tr>
        <tr>
            <td style="width: 592px">Password</td>
        </tr>
        <tr>
            <td style="height: 22px; width: 592px">
                <asp:TextBox ID="tb_Password" runat="server" Width="400px" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                <asp:Label ID="lb_PasswordCheck" runat="server"></asp:Label>

            </td>
        </tr>
        <tr>
            <td style="width: 592px; height: 20px;">
                <%--onchange="document.getElementById('tb_Password').type = this.checked ? 'text' : 'password'--%>
                <input type="checkbox" id="toggle-password" />
                Show Password
                <%--<asp:CheckBox ID="cbShowPass" runat="server" OnCheckedChanged="cbShowPass_CheckedChanged" Text="Show Password" />--%>
                
            </td>
        </tr>
        <tr>
            <td style="width: 592px">&nbsp;</td>
        </tr>
        <tr>
            <td style="height: 20px; width: 592px">Confirm Password</td>
        </tr>
        <tr>
            <td style="width: 592px">
                <asp:TextBox ID="tb_ConfirmPassword" runat="server" Width="400px" TextMode="Password"></asp:TextBox>
                <asp:Label ID="lb_ConfirmPassCheck" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 592px">&nbsp;</td>
        </tr>
        <tr>
            <td class="auto-style4">
                <div class="g-recaptcha" data-sitekey="6Le0lBUaAAAAAG89A_rNrLfFifjflVPV4hGf4Ase"></div>
            </td>
        </tr>
        <tr>
            <td style="width: 592px">&nbsp;</td>
        </tr>
        <tr>
            <td style="width: 592px">
                <asp:Button ID="btn_Submit" runat="server" OnClick="btn_Submit_Click" Text="Submit" />
                <%--<asp:Button ID="btn_Login" runat="server" Text="Login Page" OnClick="btn_Login_Click" />--%>
            </td>
        </tr>
        <tr>
            <td style="height: 19px; width: 592px;">
                <asp:Label ID="lb_gScore" runat="server" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
