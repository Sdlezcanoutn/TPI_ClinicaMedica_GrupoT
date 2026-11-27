<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PresentacionWeb.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-5">

                <div class="card shadow-sm">
                    <div class="card-body p-4">

                        <h3 class="text-center mb-4"> Iniciar sesión </h3>

                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtUsuario" CssClass="form-label" Text="Nombre de usuario" />
                            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" MaxLength="50" />
                        </div>

                        <div class="mb-3">
                            <asp:Label runat="server" AssociatedControlID="txtContraseña" CssClass="form-label" Text="Contraseña" />
                            <asp:TextBox ID="txtContraseña" runat="server" TextMode="Password" CssClass="form-control" />
                        </div>

                        <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block mb-3" Visible="false" />
                        <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="btn btn-primary w-100" OnClick="btnIngresar_Click" />

                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>