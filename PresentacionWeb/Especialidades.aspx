<%@ Page Title="Especialidades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Especialidades.aspx.cs" Inherits="PresentacionWeb.Especialidades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 1500px; margin: 0 auto;">

            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Especialidades</h2>
                        <p class="text-muted small m-0 mt-1">Listado y administración de especialidades médicas</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nueva Especialidad" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm" OnClientClick="return false;" data-bs-toggle="modal" data-bs-target="#modalEspecialidad" />
                    </div>
                </div>

                <!-- Buscador -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control bg-light border-start-0" Placeholder="Buscar por nombre..."></asp:TextBox>
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
                <!-- Modal Nueva Especialidad -->
                <div class="modal fade" id="modalEspecialidad" tabindex="-1"
                    aria-labelledby="tituloModalEspecialidad" aria-hidden="true">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title" id="tituloModalEspecialidad">Nueva Especialidad
                                </h5>
                                <button type="button" class="btn-close"
                                    data-bs-dismiss="modal" aria-label="Cerrar">
                                </button>
                            </div>

                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label">Nombre</label>
                                    <asp:TextBox ID="txtNombre" runat="server"
                                        CssClass="form-control" MaxLength="100" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Descripción</label>
                                    <asp:TextBox ID="txtDescripcion" runat="server"
                                        CssClass="form-control"
                                        TextMode="MultiLine" Rows="3"
                                        MaxLength="500" />
                                </div>
                            </div>

                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary"
                                    data-bs-dismiss="modal">
                                    Cancelar</button>
                                <asp:Button ID="btnGuardar" runat="server"
                                    Text="Guardar"
                                    CssClass="btn btn-clinica-primary" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
