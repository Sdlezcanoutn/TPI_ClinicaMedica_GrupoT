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
                        <asp:Button ID="btnAgregar" runat="server"
                            Text="+ Nuevo Paciente"
                            CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm"
                            OnClientClick="prepararNuevoPaciente(); return false;" />
                    </div>
                </div>

                <!-- Buscador por DNI o Apellido -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server"
                                CssClass="form-control bg-light border-start-0"
                                Placeholder="Buscar por DNI o Apellido..."
                                AutoPostBack="True"
                                OnTextChanged="txtBuscar_TextChanged" />
                        </div>
                    </div>
                </div>

                <!-- Listado y alertas -->
                <asp:UpdatePanel ID="upListado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:Label ID="lblErrorListado" runat="server"
                            CssClass="text-danger small d-block mb-2"
                            Visible="false" />

                        <div id="mensajeExitoPaciente" class="alert alert-success d-none mb-3" role="alert"></div>

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
                                    <asp:BoundField DataField="Direccion" HeaderText="Dirección" />
                                    <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnHistorial" runat="server"
                                                CssClass="btn btn-sm btn-outline-info me-1"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnHistorial_Click">
                                                <i class="fa-solid fa-clock-rotate-left me-1"></i>Ver historial
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEditar" runat="server"
                                                CssClass="btn btn-sm btn-outline-secondary me-1"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnEditar_Click">
                                                <i class="fa-solid fa-pen-to-square me-1"></i>Editar
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnEliminar" runat="server"
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnEliminar_Click">
                                                <i class="fa-solid fa-user-xmark me-1"></i>Desactivar
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

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Alta / Edición de Paciente -->
    <div class="modal fade" id="modalPaciente" tabindex="-1"
        aria-labelledby="tituloModalPaciente" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title fw-bold" id="tituloModalPaciente">Nuevo Paciente</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <asp:UpdatePanel ID="upPaciente" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdPaciente" runat="server" />

                        <div class="modal-body">

                            <asp:Label ID="lblErrorPaciente" runat="server"
                                CssClass="text-danger small d-block mb-3"
                                Visible="false" />

                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Nombre *</label>
                                    <asp:TextBox ID="txtNombrePaciente" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Apellido *</label>
                                    <asp:TextBox ID="txtApellidoPaciente" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">DNI *</label>
                                    <asp:TextBox ID="txtDniPaciente" runat="server" CssClass="form-control" MaxLength="8" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Fecha de nacimiento *</label>
                                    <asp:TextBox ID="txtFechaNacimientoPaciente" runat="server" TextMode="Date" CssClass="form-control" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Teléfono</label>
                                    <asp:TextBox ID="txtTelefonoPaciente" runat="server" CssClass="form-control" MaxLength="30" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Correo electrónico *</label>
                                    <asp:TextBox ID="txtEmailPaciente" runat="server" CssClass="form-control" MaxLength="254" />
                                </div>

                                <div class="col-12">
                                    <label class="form-label">Dirección</label>
                                    <asp:TextBox ID="txtDireccionPaciente" runat="server" CssClass="form-control" MaxLength="200" />
                                </div>
                            </div>

                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <asp:Button ID="btnGuardarPaciente" runat="server"
                                Text="Guardar Paciente"
                                CssClass="btn btn-clinica-primary"
                                OnClick="btnGuardarPaciente_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>
    <!-- Modal Historial de Turnos del Paciente -->
    <div class="modal fade" id="modalHistorialPaciente" tabindex="-1"
        aria-labelledby="tituloModalHistorialPaciente" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-xl">
            <div class="modal-content">

                <div class="modal-header">
                    <div>
                        <h5 class="modal-title fw-bold mb-0" id="tituloModalHistorialPaciente">Historial del Paciente</h5>
                        <small class="text-muted">
                            <asp:Label ID="lblSubtituloHistorial" runat="server" />
                        </small>
                    </div>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <asp:UpdatePanel ID="upHistorialPaciente" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="modal-body">
                            <div class="table-responsive">
                                <asp:GridView ID="dgvHistorialPaciente" runat="server" AutoGenerateColumns="False"
                                    CssClass="table table-custom align-middle mb-0" GridLines="None">
                                    <Columns>
                                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" />
                                        <asp:BoundField DataField="Horario" HeaderText="Horario" />
                                        <asp:BoundField DataField="Especialidad" HeaderText="Especialidad" />
                                        <asp:BoundField DataField="Medico" HeaderText="Médico" />
                                        <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                        <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                                        <asp:BoundField DataField="Diagnostico" HeaderText="Diagnóstico" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="text-center py-4 text-muted">
                                            <p class="m-0">Este paciente no tiene turnos registrados.</p>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cerrar</button>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Baja Lógica de Paciente -->
    <div class="modal fade" id="modalBajaPaciente" tabindex="-1"
        aria-labelledby="tituloModalBajaPaciente" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <asp:UpdatePanel ID="upBajaPaciente" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdBajaPaciente" runat="server" />

                        <div class="modal-header">
                            <h5 class="modal-title fw-bold" id="tituloModalBajaPaciente">Desactivar paciente</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                        </div>

                        <div class="modal-body">
                            <p class="mb-2">
                                Paciente: <strong>
                                    <asp:Label ID="lblNombreBajaPaciente" runat="server" /></strong>
                            </p>
                            <p class="text-muted mb-0">
                                ¿Confirmás que querés dar de baja a este paciente?
                            </p>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <asp:Button ID="btnConfirmarBajaPaciente" runat="server"
                                Text="Confirmar baja"
                                CssClass="btn btn-danger"
                                OnClick="btnConfirmarBajaPaciente_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <script>
    // Muestra el cartel verde de confirmación en el listado y lo oculta a los 4 segundos
    function mostrarAlertaPaciente(texto) {
        const alerta = document.getElementById("mensajeExitoPaciente");
        if (!alerta) return;

        alerta.textContent = texto;
        alerta.classList.remove("d-none");

        setTimeout(function () {
            alerta.classList.add("d-none");
        }, 4000);
    }

    // Cierra el modal de alta/edición tras guardar y dispara el mensaje de éxito
    function mostrarExitoPaciente(texto) {
        const modalEl = document.getElementById("modalPaciente");
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.hide();
        mostrarAlertaPaciente(texto);
    }

    // Limpia y oculta errores previos del formulario para que no queden colgados
    function ocultarErrorPaciente() {
        const lbl = document.getElementById("<%= lblErrorPaciente.ClientID %>");
        if (lbl) {
            lbl.textContent = "";
            lbl.style.display = "none";
        }
    }

    // Resetea inputs y el HiddenField del ID (vuelve a modo alta limpio)
    function limpiarFormularioPaciente() {
        document.getElementById("<%= hfIdPaciente.ClientID %>").value = "";
        document.getElementById("<%= txtNombrePaciente.ClientID %>").value = "";
        document.getElementById("<%= txtApellidoPaciente.ClientID %>").value = "";
        document.getElementById("<%= txtDniPaciente.ClientID %>").value = "";
        document.getElementById("<%= txtFechaNacimientoPaciente.ClientID %>").value = "";
        document.getElementById("<%= txtTelefonoPaciente.ClientID %>").value = "";
        document.getElementById("<%= txtEmailPaciente.ClientID %>").value = "";
        document.getElementById("<%= txtDireccionPaciente.ClientID %>").value = "";
        ocultarErrorPaciente();
    }

    // Abre el modal configurado para crear un paciente nuevo
    function prepararNuevoPaciente() {
        limpiarFormularioPaciente();
        document.getElementById("tituloModalPaciente").textContent = "Nuevo Paciente";
        document.getElementById("<%= btnGuardarPaciente.ClientID %>").value = "Guardar Paciente";

        bootstrap.Modal.getOrCreateInstance(
            document.getElementById("modalPaciente")
        ).show();
    }


    // Abre el modal en modo edición (los campos ya se cargaron desde el code-behind)
    function prepararEdicionPaciente() {
        document.getElementById("tituloModalPaciente").textContent = "Editar Paciente";
        document.getElementById("<%= btnGuardarPaciente.ClientID %>").value = "Guardar Cambios";

        bootstrap.Modal.getOrCreateInstance(
            document.getElementById("modalPaciente")
        ).show();
    }

    // Registra eventos del modal para limpiar al cerrar y borrar errores al abrir o escribir
    (function () {
        const modalPaciente = document.getElementById("modalPaciente");
        if (modalPaciente) {
            modalPaciente.addEventListener("hidden.bs.modal", limpiarFormularioPaciente);
            modalPaciente.addEventListener("show.bs.modal", ocultarErrorPaciente);
            modalPaciente.addEventListener("input", ocultarErrorPaciente);
        }
    })();

    function abrirModalHistorialPaciente() {
       bootstrap.Modal.getOrCreateInstance(
         document.getElementById("modalHistorialPaciente")
       ).show();
    }
    </script>

</asp:Content>
