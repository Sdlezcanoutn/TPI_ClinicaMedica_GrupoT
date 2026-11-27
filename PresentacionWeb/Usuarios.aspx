<%@ Page Title="Administración de usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="PresentacionWeb.Usuarios" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <div><h2>Administración de usuarios</h2><p class="text-muted">Cuentas, perfiles y acceso a la clínica.</p></div>
        <asp:Button ID="btnNuevo" runat="server" Text="Nuevo usuario" CssClass="btn btn-primary" OnClick="btnNuevo_Click" CausesValidation="false" />
    </div>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-info d-block" />
    <div class="card mb-4"><div class="card-body">
        <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
            <h4><asp:Literal ID="litTitulo" runat="server" Text="Nuevo usuario" /></h4>
            <asp:HiddenField ID="hfId" runat="server" />
            <div class="row g-3 mb-3">
                <div class="col-md-6"><label class="form-label">Nombre de usuario</label><asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="80" /></div>
                <div class="col-md-6"><label class="form-label">Email</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="254" /></div>
                <div class="col-md-6"><label class="form-label">Perfil</label><asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged" /></div>
                <asp:Panel ID="pnlMedico" runat="server" CssClass="col-md-6" Visible="false"><label class="form-label">Médico asociado</label><asp:DropDownList ID="ddlMedico" runat="server" CssClass="form-select" /></asp:Panel>
                <div class="col-md-6"><label class="form-label">Contraseña (obligatoria al crear)</label><asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" /></div>
                <div class="col-md-6"><label class="form-label">Confirmar contraseña</label><asp:TextBox ID="txtConfirmar" runat="server" CssClass="form-control" TextMode="Password" /></div>
            </div>
            <p class="text-muted small">Al editar una cuenta, dejá los campos de contraseña vacíos para conservarla. Si ingresás una contraseña nueva, se restablecerá al guardar.</p>
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-success" Text="Guardar" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-outline-secondary ms-2" Text="Cancelar" OnClick="btnCancelar_Click" CausesValidation="false" />
        </asp:Panel>
    </div></div>
    <div class="row mb-3"><div class="col-md-6"><asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Buscar usuario o email" /></div><div class="col-md-2"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-outline-primary" OnClick="btnBuscar_Click" CausesValidation="false" /></div></div>
    <div class="table-responsive">
        <asp:GridView ID="dgvUsuarios" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover" GridLines="None" EmptyDataText="No se encontraron usuarios." OnRowCommand="dgvUsuarios_RowCommand">
            <Columns>
                <asp:BoundField DataField="Nombre_Usuario" HeaderText="Usuario" />
                <asp:BoundField DataField="Email" HeaderText="Correo" />
                <asp:TemplateField HeaderText="Perfil"><ItemTemplate><%# Eval("Perfil.Nombre") %></ItemTemplate></asp:TemplateField>
                <asp:TemplateField HeaderText="Estado"><ItemTemplate><%# Convert.ToBoolean(Eval("Estado")) ? "Activo" : "Inactivo" %></ItemTemplate></asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones"><ItemTemplate>
                    <asp:LinkButton runat="server" Text="Editar" CssClass="btn btn-sm btn-outline-primary me-2" CommandName="EditarUsuario" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" />
                    <asp:LinkButton runat="server" Text="Desactivar" CssClass="btn btn-sm btn-outline-danger" CommandName="DesactivarUsuario" CommandArgument='<%# Eval("Id") %>' Visible='<%# Convert.ToBoolean(Eval("Estado")) %>' OnClientClick="return confirm('¿Desactivar esta cuenta?');" CausesValidation="false" />
                    <asp:LinkButton runat="server" Text="Reactivar" CssClass="btn btn-sm btn-outline-success" CommandName="ReactivarUsuario" CommandArgument='<%# Eval("Id") %>' Visible='<%# !Convert.ToBoolean(Eval("Estado")) %>' OnClientClick="return confirm('¿Reactivar esta cuenta?');" CausesValidation="false" />
                </ItemTemplate></asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>