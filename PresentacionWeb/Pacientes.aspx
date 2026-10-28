<%@ Page Title="Pacientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pacientes.aspx.cs" Inherits="PresentacionWeb.Pacientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Encabezado -->
    <div class="d-flex justify-content-between align-items-center my-4">
        <div>
            <h3 class="fw-bold text-dark m-0">Gestión de Pacientes</h3>
            <p class="text-muted small m-0">Administración de historias clínicas y datos personales</p>
        </div>
        <asp:Button ID="btnAgregar" runat="server" Text="+ Nuevo Paciente" CssClass="btn btn-clinica-primary px-3 shadow-sm" />
    </div>

    <!-- Contenedor principal -->
    <div class="card border-0 shadow-sm rounded-3 mb-4">
        <div class="card-body p-4">
            
            <!-- Buscador por DNI o Apellido -->
            <div class="row g-3 mb-4">
                <div class="col-md-6 col-lg-4">
                    <div class="input-group search-box">
                        <span class="input-group-text">
                            <i class="fa-solid fa-magnifying-glass"></i>
                        </span>
                        <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" Placeholder="Buscar por DNI o Apellido..."></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Tabla de Pacientes -->
            <div class="table-responsive">
                <asp:GridView ID="dgvPacientes" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-custom align-middle mb-0" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Dni" HeaderText="DNI" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="110px" />
                        <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre y Apellido" />
                        <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="ObraSocial" HeaderText="Obra Social" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-outline-secondary me-1">
                                    <i class="fa-solid fa-pen-to-square me-1"></i> Editar
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-outline-danger">
                                    <i class="fa-solid fa-trash me-1"></i> Eliminar
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

</asp:Content>