<%@ Page Title="Especialidades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Especialidades.aspx.cs" Inherits="PresentacionWeb.Especialidades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Encabezado -->
    <div class="d-flex justify-content-between align-items-center my-4">
        <div>
            <h3 class="fw-bold text-dark m-0">Gestión de Especialidades</h3>
            <p class="text-muted small m-0">Listado y administración de especialidades médicas</p>
        </div>
        <asp:Button ID="btnAgregar" runat="server" Text="+ Nueva Especialidad" CssClass="btn btn-clinica-primary px-3 shadow-sm" />
    </div>

    <!-- Contenedor -->
    <div class="card border-0 shadow-sm rounded-3 mb-4">
        <div class="card-body p-4">
            
            <!-- Buscador -->
            <div class="row g-3 mb-4">
                <div class="col-md-6 col-lg-4">
                    <div class="input-group search-box">
                        <span class="input-group-text">
                            <i class="fa-solid fa-magnifying-glass"></i>
                        </span>
                        <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" Placeholder="Buscar por nombre..."></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Tabla -->
            <div class="table-responsive">
                <asp:GridView ID="dgvEspecialidades" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-custom align-middle mb-0" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="80px" />
                        <asp:BoundField DataField="Nombre" HeaderText="Especialidad" />
                        <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
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
                            <i class="fa-solid fa-folder-open fs-2 mb-2 d-block text-secondary opacity-50"></i>
                            <p class="m-0 fw-medium">No hay especialidades registradas por el momento.</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

        </div>
    </div>

</asp:Content>