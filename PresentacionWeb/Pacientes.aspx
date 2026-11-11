<%@ Page Title="Pacientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pacientes.aspx.cs" Inherits="PresentacionWeb.Pacientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 1500px; margin: 0 auto;">

            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Pacientes</h2>
                        <p class="text-muted small m-0 mt-1">Administración de historias clínicas y datos personales</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nuevo Paciente" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm" OnClick="btnAgregar_Click" />
                    </div>
                </div>

                <!-- Buscador por DNI o Apellido -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control bg-light border-start-0" Placeholder="Buscar por DNI o Apellido..."></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Tabla de Pacientes -->
                <div class="table-responsive">
                    <asp:GridView ID="dgvPacientes" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-custom align-middle mb-0" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="DNI" HeaderText="DNI" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="110px" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                            <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-outline-secondary me-1"
                                        CommandName="Editar" CommandArgument='<%# Eval("Id") %>' OnClick="btnEditar_Click">
                                        <i class="fa-solid fa-pen-to-square me-1"></i>Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-outline-danger"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' OnCommand="btnEliminar_Command"
                                        OnClientClick="return confirm('¿Estás seguro de que querés dar de baja a este paciente?');">
                                        <i class="fa-solid fa-trash me-1"></i>Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center py-5 text-muted">
                                <i class="fa-solid fa-user-slash fs-2 mb-2 d-block text-secondary opacity-50"></i>
                                <p class="m-0 fw-medium">No hay pacientes registrados en el sistema.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

            </div>

        </div>
    </div>

</asp:Content>
