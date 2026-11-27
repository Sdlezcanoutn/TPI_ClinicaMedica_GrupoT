<%@ Page Title="Administración de usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="PresentacionWeb.Usuarios" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div class="contenedor-modulo">
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Administración de usuarios</h2>
                        <p class="text-muted small m-0 mt-1">Cuentas, perfiles y acceso a la clínica</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnNuevo" runat="server"
                            Text="+ Nuevo usuario"
                            CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm"
                            OnClick="btnNuevo_Click"
                            CausesValidation="false" />
                    </div>
                </div>

                <!-- Mensajes -->
                <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-info d-block mb-4" />

                <!-- Formulario -->
                <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card border shadow-sm rounded-3 mb-4">
                    <div class="card-body p-4">
                        <h4 class="fw-bold text-dark mb-3"><asp:Literal ID="litTitulo" runat="server" Text="Nuevo usuario" /></h4>
                        <asp:HiddenField ID="hfId" runat="server" />
                        <div class="row g-3 mb-3">
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Nombre de usuario</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control bg-light" MaxLength="80" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Email</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control bg-light" TextMode="Email" MaxLength="254" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Perfil</label>
                                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-select bg-light" AutoPostBack="true" OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged" />
                            </div>
                            <asp:Panel ID="pnlMedico" runat="server" CssClass="col-md-6" Visible="false">
                                <label class="form-label fw-semibold">Médico asociado</label>
                                <asp:DropDownList ID="ddlMedico" runat="server" CssClass="form-select bg-light" />
                            </asp:Panel>
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Contraseña (obligatoria al crear)</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control bg-light" TextMode="Password" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Confirmar contraseña</label>
                                <asp:TextBox ID="txtConfirmar" runat="server" CssClass="form-control bg-light" TextMode="Password" />
                            </div>
                        </div>
                        <p class="text-muted small mb-3">Al editar una cuenta, dejá los campos de contraseña vacíos para conservarla. Si ingresás una contraseña nueva, se restablecerá al guardar.</p>
                        <div class="d-flex gap-2">
                            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3" Text="Guardar" OnClick="btnGuardar_Click" />
                            <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-outline-secondary px-4 py-2 rounded-3" Text="Cancelar" OnClick="btnCancelar_Click" CausesValidation="false" />
                        </div>
                    </div>
                </asp:Panel>

                <!-- Buscador -->
                <div class="row g-2 align-items-center mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server"
                                CssClass="form-control bg-light border-start-0"
                                placeholder="Buscar usuario o email..." />
                        </div>
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnBuscar" runat="server"
                            Text="Buscar"
                            CssClass="btn btn-outline-secondary px-3 py-2 rounded-3"
                            OnClick="btnBuscar_Click"
                            CausesValidation="false" />
                    </div>
                </div>

                <!-- Tabla de Usuarios -->
                <div class="table-responsive">
                    <asp:GridView ID="dgvUsuarios" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-custom align-middle mb-0"
                        GridLines="None"
                        EmptyDataText="No se encontraron usuarios."
                        OnRowCommand="dgvUsuarios_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Nombre_Usuario" HeaderText="Usuario" />
                            <asp:BoundField DataField="Email" HeaderText="Correo" />
                            <asp:TemplateField HeaderText="Perfil">
                                <ItemTemplate><%# Eval("Perfil.Nombre") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <span class='<%# Convert.ToBoolean(Eval("Estado")) ? "badge bg-success-subtle text-success" : "badge bg-secondary-subtle text-secondary" %>'>
                                        <%# Convert.ToBoolean(Eval("Estado")) ? "Activo" : "Inactivo" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary me-1" CommandName="EditarUsuario" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">
                                        <i class="fa-solid fa-pen-to-square me-1"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="DesactivarUsuario" CommandArgument='<%# Eval("Id") %>' Visible='<%# Convert.ToBoolean(Eval("Estado")) %>' OnClientClick="return confirm('¿Desactivar esta cuenta?');" CausesValidation="false">
                                        <i class="fa-solid fa-user-xmark me-1"></i> Desactivar
                                    </asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-success" CommandName="ReactivarUsuario" CommandArgument='<%# Eval("Id") %>' Visible='<%# !Convert.ToBoolean(Eval("Estado")) %>' OnClientClick="return confirm('¿Reactivar esta cuenta?');" CausesValidation="false">
                                        <i class="fa-solid fa-rotate-left me-1"></i> Reactivar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>
    </div>

</asp:Content>