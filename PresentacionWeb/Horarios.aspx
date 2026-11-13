<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Horarios.aspx.cs" Inherits="PresentacionWeb.Horarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 1500px; margin: 0 auto;">
    
            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">
    
                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Horarios</h2>
                        <p class="text-muted small m-0 mt-1">Listado y administración de horarios de atención médica</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nuevo Horario" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm" />
                    </div>
                </div>
    
                <!-- Buscador -->
                <div class="row g-3 mb-4 align-items-end">

                    <!-- Filtro por médico -->
                    <div class="col-md-6 col-lg-4">
                        <asp:Label Text="Médico" runat="server"/>
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>    
                            <asp:TextBox ID="txtBuscarMedico" runat="server" CssClass="form-control bg-light border-start-0" Placeholder="Nombre o apellido del médico..."></asp:TextBox>
                        </div>
                    </div>

                    <!-- Filtro por día -->
                    <div class="col-md-6 col-lg-4">
                        <asp:Label Text="Día de la semana" runat="server"/>
                        <asp:DropDownList ID="ddlDiaSemana" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Todos los días" Value="" />
                            <asp:ListItem Text="Lunes" Value="1" />
                            <asp:ListItem Text="Martes" Value="2" />
                            <asp:ListItem Text="Miércoles" Value="3" />
                            <asp:ListItem Text="Jueves" Value="4" />
                            <asp:ListItem Text="Viernes" Value="5" />
                            <asp:ListItem Text="Sábado" Value="6" />
                            <asp:ListItem Text="Domingo" Value="7" />
                        </asp:DropDownList>
                    </div>

                    <!-- Botón Buscar -->
                    <div class="col-md-12 col-lg-4">
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary w-100 py-2 rounded-3 shadow-sm" OnClick="btnBuscar_Click" />
                    </div>
                </div>
    
                <!-- Tabla -->
                <div class="table-responsive">
                    <asp:GridView ID="dgvHorarios" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-custom align-middle mb-0" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="80px" />
                            <asp:TemplateField HeaderText="Médico">
                                <ItemTemplate>
                                    <%# Eval("Medico.Nombre") %>
                                    <%# Eval("Medico.Apellido") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Dia_Semana" HeaderText="Día" />
                            <asp:BoundField DataField="Hora_Entrada" HeaderText="Hora de Entrada" DataFormatString="{0:HH:mm}" />
                            <asp:BoundField DataField="Hora_Salida" HeaderText="Hora de Salida" DataFormatString="{0:HH:mm}"/>
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
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
                                <p class="m-0 fw-medium">No hay horarios de trabajo registrados por el momento.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
    
            </div>
    
        </div>
    </div>
</asp:Content>
