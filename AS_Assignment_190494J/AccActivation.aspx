<%@ Page Title="" Language="C#" MasterPageFile="~/Secret.Master" AutoEventWireup="true" CodeBehind="AccActivation.aspx.cs" Inherits="AS_Assignment_190494J.AccActivation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
    <script>
        function alertSuccess() {
            Swal.fire({
                title: 'Congratulations!',
                text: 'You successfully activated your account!',
                icon: 'success'
            }).then(function () {
                window.location = "SignIn.aspx";
            })
        }

        function alertFailed() {
            Swal.fire({
                title: 'Failed!',
                text: 'Please enter the correct code!',
                icon: 'error'
            })
        }
    </script>
    <table class="nav-justified">
        <tr>
            <td><h1>Account Activation</h1></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lb_para" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="tb_verify" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnVerify" runat="server" Text="Verify" OnClick="btnVerify_Click" /></td>
        </tr>
    </table>

</asp:Content>
