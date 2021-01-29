<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="AS_Assignment_190494J.SignIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?render=6Le1zTgaAAAAALxbir8hTgaBWp2SIiu0jHxmzClN"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
    <script type="text/javascript">
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

        function alertLockout30() {
            Swal.fire({
                title: 'Warning!',
                text: 'Your account has been locked out! Please try again in 30 seconds.',
                icon: 'warning'
            })
        }

        function alertLockout() {
            Swal.fire({
                title: 'Warning!',
                text: 'Your account has been locked out! Please try again later.',
                icon: 'warning'
            })
        }

        function alertVerify() {
            Swal.fire({
                title: 'Warning!',
                text: 'Please verify your account first before logging',
                icon: 'warning'
            })
        }

        function alertVerifyAccount() {
            Swal.fire({
                title: "Verify Account",
                text: "Please input your email",
                input: 'text',
                showCancelButton: true
            }).then((result) => {
                if (result.value) {
                    window.location = "VerifyAccount.aspx?emailagain=" + result.value;
                }
            });
        }
    </script>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Le1zTgaAAAAALxbir8hTgaBWp2SIiu0jHxmzClN', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }

        .auto-style2 {
            width: 200px;
        }

        .auto-style3 {
            width: 159px;
            height: 23px;
        }

        .auto-style4 {
            height: 23px;
        }
        .auto-style5 {
            width: 200px;
            height: 20px;
        }
        .auto-style6 {
            height: 20px;
        }
    </style>
    <div>
        <table class="auto-style1">
            <tr>
                <td class="auto-style2">
                    <h1>Login Page</h1>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lb_Error" runat="server"></asp:Label>
                    <asp:Label ID="lb_gScore" runat="server" Visible="False"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Email</td>
                <td>
                    <asp:TextBox ID="tb_Email" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">Password</td>
                <td>
                    <asp:TextBox ID="tb_Password" runat="server" TextMode="Password"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td><input type="checkbox" id="toggle-password" /> Show Password</td>
            </tr>

            <tr>
                <td class="auto-style5"></td>
                <td class="auto-style6"></td>
            </tr>
            <tr>
                <td class="auto-style3"></td>
                <td class="auto-style4">
                    <asp:Button ID="btn_Submit" runat="server" OnClick="btn_Submit_Click" Text="Sign In" />
                </td>
                <td class="auto-style4">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style3"></td>
                <td class="auto-style4">
                    <div>  
                        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
                    </div>
                </td>
                <td class="auto-style4"></td>
            </tr>
            <%--<tr>
                <td class="auto-style3">&nbsp;</td>
                <td class="auto-style4">
                    <asp:Button ID="btn_Register" runat="server" OnClick="btn_Register_Click" Text="Register an Account" />
                </td>
            </tr>--%>
            <tr>
                <td class="auto-style3">&nbsp;</td>
                <td class="auto-style4">
                    Did not verify account?</td>
                <td class="auto-style4">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style3">&nbsp;</td>
                <td class="auto-style4">
                    <asp:Button ID="btn_Verify" runat="server" OnClick="btn_Verify_Click" Text="Verify Account" />
                </td>
                <td class="auto-style4">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style3">&nbsp;</td>

            </tr>
        </table>

    </div>
</asp:Content>
