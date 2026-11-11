<%@ Page Title="Turnos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Turnos.aspx.cs" Inherits="PresentacionWeb.Turnos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 1500px; margin: 0 auto;">

            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 dashboard-panel p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Turnos</h2>
                        <p class="text-muted small m-0 mt-1">Agendamiento y control de citas médicas</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nuevo Turno" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm" />
                    </div>
                </div>

                <!-- Buscador y Filtro por Paciente / Médico -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control bg-light border-start-0" Placeholder="Buscar por Paciente o Médico..."></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Tabla de Turnos -->
                <div class="table-responsive">
                    <asp:GridView ID="dgvTurnos" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-custom align-middle mb-0" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="FechaHora" HeaderText="Fecha y Hora" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="160px" />
                            <asp:BoundField DataField="Paciente" HeaderText="Paciente" />
                            <asp:BoundField DataField="Medico" HeaderText="Médico" />
                            <asp:BoundField DataField="Especialidad" HeaderText="Especialidad" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <span class="badge bg-warning-subtle text-warning-emphasis border border-warning-subtle px-2 py-1 fw-medium">Pendiente</span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-outline-secondary me-1">
                                    <i class="fa-solid fa-pen-to-square me-1"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-sm btn-outline-danger">
                                    <i class="fa-solid fa-calendar-xmark me-1"></i> Cancelar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center py-5 text-muted">
                                <i class="fa-solid fa-calendar-minus fs-2 mb-2 d-block text-secondary opacity-50"></i>
                                <p class="m-0 fw-medium">No hay turnos agendados por el momento.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

            </div>

        </div>
    </div>

</asp:Content>
