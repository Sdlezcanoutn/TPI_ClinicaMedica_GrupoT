<%@ Page Title="Turnos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Turnos.aspx.cs" Inherits="PresentacionWeb.Turnos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div class="contenedor-modulo">
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Turnos</h2>
                        <p class="text-muted small m-0 mt-1">Agendamiento y control de citas médicas</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server"
                            Text="+ Nuevo Turno"
                            CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm"
                            OnClick="btnAgregar_Click" />
                    </div>
                </div>

                <!-- Filtros del listado -->

                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-3">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>

                            <asp:TextBox ID="txtBuscar" runat="server"
                                CssClass="form-control bg-light border-start-0"
                                Placeholder="Buscar por Paciente o Médico..."
                                AutoPostBack="false"
                                oninput="programarBusqueda()"
                                onkeydown="if (event.key === 'Enter') { event.preventDefault(); ejecutarBusqueda(); }" />

                            <!-- Botón oculto para ejecutar la búsqueda -->
                            <asp:LinkButton ID="btnBuscar" runat="server"
                                OnClick="btnBuscar_Click"
                                CssClass="boton-busqueda-oculto">
                            </asp:LinkButton>
                        </div>
                    </div>

                    <div class="col-md-6 col-lg-3">
                        <asp:DropDownList ID="ddlFiltroEspecialidad" runat="server"
                            CssClass="form-select bg-light"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="FiltroListado_Changed" />
                    </div>

                    <div class="col-md-6 col-lg-3">
                        <asp:DropDownList ID="ddlFiltroMedico" runat="server"
                            CssClass="form-select bg-light"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="FiltroListado_Changed" />
                    </div>

                    <div class="col-md-6 col-lg-3 d-flex gap-2">
                        <asp:TextBox ID="txtFiltroFecha" runat="server"
                            TextMode="Date"
                            CssClass="form-control bg-light"
                            AutoPostBack="true"
                            OnTextChanged="FiltroListado_Changed" />
                        <asp:LinkButton ID="btnLimpiarFiltros" runat="server"
                            CssClass="btn btn-outline-secondary"
                            ToolTip="Limpiar filtros"
                            OnClick="btnLimpiarFiltros_Click">
                            <i class="fa-solid fa-rotate-left"></i>
                        </asp:LinkButton>
                    </div>
                </div>

                <!-- Listado y alertas -->
                <asp:UpdatePanel ID="upListadoTurnos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:Label ID="lblErrorListadoTurno" runat="server"
                            CssClass="text-danger small d-block mb-2"
                            Visible="false" />

                        <div id="mensajeExitoTurno" class="alert alert-success d-none mb-3" role="alert"></div>

                        <!-- Tabla de Turnos -->
                        <div class="table-responsive">
                            <asp:GridView ID="dgvTurnos" runat="server" AutoGenerateColumns="False"
                                CssClass="table table-custom align-middle mb-0" GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="130px" />
                                    <asp:BoundField DataField="Hora" HeaderText="Hora" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="135px" />
                                    <asp:BoundField DataField="Paciente" HeaderText="Paciente" />
                                    <asp:BoundField DataField="Medico" HeaderText="Médico" />
                                    <asp:BoundField DataField="Especialidad" HeaderText="Especialidad" />
                                    <asp:TemplateField HeaderText="Estado" HeaderStyle-Width="140px">
                                        <ItemTemplate>
                                            <span class='<%# ObtenerClaseBadgeEstado(Eval("Estado").ToString()) %>'>
                                                <%# Eval("Estado") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                        <ItemTemplate>

                                            <div class="d-flex flex-column align-items-end gap-2">

                                                <!-- Acciones de gestión y consulta -->
                                                <div class="d-flex flex-wrap justify-content-end gap-2">

                                                    <asp:LinkButton ID="btnEditar" runat="server"
                                                        CssClass="btn btn-sm btn-outline-amber text-nowrap"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        Visible='<%# Eval("Estado").ToString() == "Nuevo" || Eval("Estado").ToString() == "Reprogramado" %>'
                                                        OnClick="btnEditar_Click">
                                                    <i class="fa-solid fa-calendar-days me-1"></i> Reprogramar
                                                </asp:LinkButton>

                                                    <asp:LinkButton ID="btnCancelar" runat="server"
                                                        CssClass="btn btn-sm btn-outline-danger text-nowrap"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        Visible='<%# Eval("Estado").ToString() == "Nuevo" || Eval("Estado").ToString() == "Reprogramado" %>'
                                                        OnClick="btnCancelar_Click">
                                                    <i class="fa-solid fa-calendar-xmark me-1"></i> Cancelar
                                                </asp:LinkButton>

                                                    <asp:LinkButton ID="btnVerDiagnostico" runat="server"
                                                        CssClass="btn btn-sm btn-outline-primary text-nowrap"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        Visible='<%# Eval("Estado").ToString() == "Cerrado" %>'
                                                        OnClick="btnVerDiagnostico_Click"
                                                        ToolTip="Consultar diagnóstico">
                                                    <i class="fa-solid fa-file-medical me-1"></i> Ver diagnóstico
                                                </asp:LinkButton>

                                                    <asp:LinkButton ID="btnHistorial" runat="server"
                                                        CssClass="btn btn-sm btn-outline-purple text-nowrap"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        OnClick="btnHistorial_Click"
                                                        ToolTip="Consultar historial">
                                                        <i class="fa-solid fa-clock-rotate-left me-1"></i> Historial
                                                    </asp:LinkButton>

                                                </div>

                                                <!-- Acciones de atención médica -->
                                                <asp:Panel runat="server"
                                                    CssClass="d-flex flex-wrap justify-content-end gap-2"
                                                    Visible='<%# Eval("Estado").ToString() == "Nuevo" || Eval("Estado").ToString() == "Reprogramado" %>'>

                                                    <asp:LinkButton ID="btnAtender" runat="server"
                                                        CssClass="btn btn-sm btn-outline-success text-nowrap"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        OnClick="btnAtender_Click"
                                                        ToolTip="Atender paciente">
                                                    <i class="fa-solid fa-stethoscope me-1"></i> Atender
                                                </asp:LinkButton>

                                                    <asp:LinkButton ID="btnInasistencia" runat="server"
                                                        CssClass="btn btn-sm btn-outline-secondary text-nowrap"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        OnClick="btnInasistencia_Click"
                                                        ToolTip="Registrar inasistencia">
                                                    <i class="fa-solid fa-user-xmark me-1"></i> No asistió
                                                </asp:LinkButton>

                                                </asp:Panel>

                                            </div>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center py-5 text-muted">
                                        <i class="fa-solid fa-calendar-minus fs-2 mb-2 d-block text-secondary opacity-50"></i>
                                        <p class="m-0 fw-medium">
                                            <asp:Label ID="lblSinResultados" runat="server" />
                                        </p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>

                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnBuscar" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ddlFiltroEspecialidad" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlFiltroMedico" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtFiltroFecha" EventName="TextChanged" />
                        <asp:PostBackTrigger ControlID="btnLimpiarFiltros" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>

        </div>
    </div>

    <!-- Modal Historial de Turno -->
    <div class="modal fade" id="modalHistorialTurno" tabindex="-1"
        aria-labelledby="tituloModalHistorial" aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title" id="tituloModalHistorial">Historial del turno
                    </h5>

                    <button type="button" class="btn-close"
                        data-bs-dismiss="modal" aria-label="Cerrar">
                    </button>
                </div>

                <asp:UpdatePanel ID="upHistorialTurno" runat="server"
                    UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="modal-body">

                            <!-- Datos del turno -->
                            <div class="bg-light rounded-3 p-3 mb-4">
                                <asp:Label ID="lblDatosHistorial" runat="server"
                                    CssClass="fw-semibold d-block" />

                                <asp:Label ID="lblFechaAltaHistorial" runat="server"
                                    CssClass="text-muted small d-block" />

                                <asp:Label ID="lblEstadoActualHistorial" runat="server"
                                    CssClass="small d-block mt-2" />
                            </div>

                            <!-- Modificaciones anteriores -->
                            <h6 class="fw-bold mb-3">Modificaciones anteriores</h6>

                            <div class="table-responsive">
                                <asp:GridView ID="dgvHistorialTurno" runat="server"
                                    AutoGenerateColumns="False"
                                    CssClass="table table-custom align-middle mb-0"
                                    GridLines="None">

                                    <Columns>

                                        <asp:BoundField
                                            DataField="Fecha_Cambio"
                                            HeaderText="Fecha de modificación"
                                            DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                                        <asp:BoundField
                                            DataField="Fecha_Anterior"
                                            HeaderText="Fecha anterior"
                                            DataFormatString="{0:dd/MM/yyyy}" />

                                        <asp:BoundField
                                            DataField="Hora_Inicio_Anterior"
                                            HeaderText="Desde"
                                            DataFormatString="{0:HH:mm}" />

                                        <asp:BoundField
                                            DataField="Hora_Fin_Anterior"
                                            HeaderText="Hasta"
                                            DataFormatString="{0:HH:mm}" />

                                        <asp:TemplateField HeaderText="Estado anterior">
                                            <ItemTemplate>
                                                <span class='<%# ObtenerClaseBadgeEstado(Eval("Estado_Anterior.Nombre").ToString()) %>'>
                                                    <%#: Eval("Estado_Anterior.Nombre") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>

                                    <EmptyDataTemplate>
                                        <div class="text-center py-4 text-muted">
                                            <i class="fa-solid fa-clock-rotate-left fs-3 mb-2 d-block"></i>
                                            <p class="m-0">
                                                Este turno aún no registra modificaciones.
                                           
                                            </p>
                                        </div>
                                    </EmptyDataTemplate>

                                </asp:GridView>
                            </div>

                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary"
                                data-bs-dismiss="modal">
                                Cerrar
                           
                            </button>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Confirmación de Cambio de Estado -->
    <div class="modal fade" id="modalConfirmarEstado" tabindex="-1"
        aria-labelledby="tituloModalEstado" aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <asp:UpdatePanel ID="upConfirmarEstado" runat="server"
                    UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="modal-header">
                            <h5 class="modal-title" id="tituloModalEstado">
                                <asp:Label ID="lblTituloEstado" runat="server" />
                            </h5>

                            <button type="button" class="btn-close"
                                data-bs-dismiss="modal" aria-label="Cerrar">
                            </button>
                        </div>

                        <asp:HiddenField ID="hfIdCambioEstado" runat="server" />

                        <div class="modal-body">

                            <p>
                                <asp:Label ID="lblMensajeEstado" runat="server" />
                            </p>

                            <p>
                                <strong>
                                    <asp:Label ID="lblDatosEstado" runat="server" />
                                </strong>
                            </p>

                            <div class="alert alert-warning">
                                <asp:Label ID="lblAdvertenciaEstado" runat="server" />
                            </div>

                            <p class="text-muted small mb-0">
                                El historial del turno se conservará.
                           
                            </p>

                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary"
                                data-bs-dismiss="modal">
                                Volver
                           
                            </button>

                            <asp:Button ID="btnConfirmarEstado" runat="server"
                                OnClick="btnConfirmarEstado_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Atención de Turno -->
    <div class="modal fade" id="modalAtenderTurno" tabindex="-1"
        aria-labelledby="tituloModalAtender" aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <asp:UpdatePanel ID="upAtenderTurno" runat="server"
                    UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="modal-header">
                            <h5 class="modal-title" id="tituloModalAtender">
                                <asp:Label ID="lblTituloAtencion" runat="server"
                                    Text="Atención del paciente" />
                            </h5>

                            <button type="button" class="btn-close"
                                data-bs-dismiss="modal" aria-label="Cerrar">
                            </button>
                        </div>

                        <asp:HiddenField ID="hfIdAtenderTurno" runat="server" />

                        <div class="modal-body formulario-modulo">

                            <asp:Label ID="lblErrorAtenderTurno" runat="server"
                                CssClass="text-danger small d-block mb-3"
                                Visible="false" />

                            <!-- Datos del turno -->
                            <div class="bg-light rounded-3 p-3 mb-4">

                                <asp:Label ID="lblDatosAtencion" runat="server"
                                    CssClass="fw-semibold d-block" />

                                <asp:Label ID="lblHorarioAtencion" runat="server"
                                    CssClass="text-muted small d-block mt-1" />

                            </div>

                            <!-- Motivo de consulta -->
                            <div class="mb-3">
                                <label class="form-label fw-semibold">
                                    Motivo de consulta
                               
                                </label>

                                <asp:Label ID="lblMotivoAtencion" runat="server"
                                    CssClass="d-block bg-light border rounded-3 p-3" />
                            </div>

                            <!-- Registro y consulta del diagnóstico -->
                            <div class="mb-3">
                                <asp:Label ID="lblTituloDiagnostico" runat="server"
                                    CssClass="form-label fw-semibold"
                                    Text="Diagnóstico y observaciones *" />

                                <asp:TextBox ID="txtDiagnostico" runat="server"
                                    CssClass="form-control w-100"
                                    TextMode="MultiLine"
                                    Rows="5"
                                    Style="max-width: 100%;"
                                    oninput="ocultarErrorAtencion();"
                                    onkeydown="ocultarErrorAtencion();"
                                    Placeholder="Ingrese el diagnóstico y las observaciones de la consulta..." />

                                <asp:Label ID="lblDiagnosticoRegistrado" runat="server"
                                    CssClass="d-block bg-light border rounded-3 p-3"
                                    Style="white-space: pre-wrap; overflow-wrap: anywhere; user-select: none;"
                                    Visible="false" />
                            </div>

                            <div id="avisoCierreTurno" runat="server"
                                class="alert alert-info mb-0">
                                Al guardar el diagnóstico, el turno quedará cerrado.
                           
                            </div>

                        </div>

                        <div class="modal-footer">

                            <button id="btnVolverAtencion" runat="server"
                                type="button" class="btn btn-secondary"
                                data-bs-dismiss="modal">
                                Volver
                       
                            </button>

                            <asp:Button ID="btnCerrarTurno" runat="server"
                                Text="Guardar y cerrar turno"
                                CssClass="btn btn-clinica-primary"
                                OnClick="btnCerrarTurno_Click" />

                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Alta de Turno -->
    <div class="modal fade" id="modalTurno" tabindex="-1"
        aria-labelledby="tituloModalTurno" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title" id="tituloModalTurno">Nuevo Turno</h5>
                    <button type="button" class="btn-close" onclick="cerrarModalTurno();" aria-label="Cerrar"></button>
                </div>

                <asp:UpdatePanel ID="upTurno" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdTurno" runat="server" Value="0" />

                        <div class="modal-body formulario-modulo">

                            <asp:Label ID="lblErrorTurno" runat="server"
                                CssClass="text-danger small d-block mb-3"
                                Visible="false" />

                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Paciente *</label>
                                    <asp:DropDownList ID="ddlPaciente" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="ddlPaciente_SelectedIndexChanged" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Especialidad *</label>
                                    <asp:DropDownList ID="ddlEspecialidad" runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlEspecialidad_SelectedIndexChanged" />
                                </div>

                                <div class="col-12">
                                    <asp:Panel ID="pnlSugerenciasTurno" runat="server" Visible="false"
                                        CssClass="border rounded-3 p-3 bg-light">
                                        <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center mb-2">
                                            <span class="small fw-semibold text-dark">
                                                <i class="fa-solid fa-clock me-1 text-primary"></i>
                                                Próximos horarios sugeridos (60 min)
                                            </span>
                                            <span class="small text-muted">Tocá una opción para autocompletar</span>
                                        </div>

                                        <asp:Repeater ID="rptSugerenciasTurno" runat="server"
                                            OnItemCommand="rptSugerenciasTurno_ItemCommand">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnSugerencia" runat="server"
                                                    CssClass="btn btn-sm btn-outline-primary me-2 mb-2 text-start"
                                                    CommandName="SeleccionarSugerencia"
                                                    CommandArgument='<%# Eval("Argumento") %>'>
                                                   <div class="fw-semibold"><%# Eval("MedicoTexto") %></div>
                                                   <div class="small"><%# Eval("FechaTexto") %> · <%# Eval("HorarioTexto") %></div>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <asp:Label ID="lblSinSugerencias" runat="server"
                                            CssClass="small text-muted d-block"
                                            Visible="false"
                                            Text="No hay horarios libres en los próximos 21 días para esta especialidad." />
                                    </asp:Panel>
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Médico *</label>
                                    <asp:DropDownList ID="ddlMedico" runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlMedico_SelectedIndexChanged" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Fecha *</label>
                                    <asp:TextBox ID="txtFechaTurno" runat="server"
                                        CssClass="form-control"
                                        TextMode="Date"
                                        AutoPostBack="True"
                                        OnTextChanged="txtFechaTurno_TextChanged" />
                                </div>

                                <div class="col-12">
                                    <label class="form-label">Horario disponible (60 min) *</label>
                                    <asp:DropDownList ID="ddlHorarioDisponible" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="-- Seleccione médico y fecha --" Value="0" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-12">
                                    <label class="form-label">Motivo de consulta *</label>
                                    <asp:TextBox ID="txtMotivoConsulta" runat="server"
                                        CssClass="form-control"
                                        TextMode="MultiLine"
                                        Rows="3"
                                        MaxLength="1000"
                                        Placeholder="Describa el motivo de la consulta..." />
                                </div>
                            </div>

                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" onclick="cerrarModalTurno();">Cancelar</button>
                            <asp:Button ID="btnGuardarTurno" runat="server"
                                Text="Guardar Turno"
                                CssClass="btn btn-clinica-primary"
                                OnClick="btnGuardarTurno_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <script>
        // ===== BÚSQUEDA DE TURNOS =====

        let temporizadorBusqueda;

        // --- Esperar antes de buscar ---
        function programarBusqueda() {
            clearTimeout(temporizadorBusqueda);

            temporizadorBusqueda = setTimeout(function () {
                ejecutarBusqueda();
            }, 300);
        }

        // --- Ejecutar búsqueda ---
        function ejecutarBusqueda() {
            clearTimeout(temporizadorBusqueda);
            __doPostBack('<%= btnBuscar.UniqueID %>', '');
        }

        // Muestra el cartel verde de confirmación en el listado y lo oculta a los 4 segundos
        function mostrarAlertaTurno(texto) {
            const alerta = document.getElementById("mensajeExitoTurno");
            if (!alerta) return;

            alerta.textContent = texto;
            alerta.classList.remove("d-none");

            // --- Desplazar la página hacia arriba ---
            setTimeout(function () {
                window.scrollTo({ top: 0, behavior: 'smooth' });
            }, 500);

            setTimeout(function () {
                alerta.classList.add("d-none");
            }, 4000);
        }

        // --- Cerrar modal de turno ---
        function cerrarModalTurno() {
            const modalEl = document.getElementById("modalTurno");

            if (modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl);

                if (modal) {
                    modal.hide();
                }

                modalEl.classList.remove("show");
                modalEl.style.display = "none";
                modalEl.setAttribute("aria-hidden", "true");
                modalEl.removeAttribute("aria-modal");
                modalEl.removeAttribute("role");
            }

            document.body.classList.remove("modal-open");
            document.body.style.removeProperty("overflow");
            document.body.style.removeProperty("padding-right");

            document.querySelectorAll(".modal-backdrop").forEach(function (backdrop) {
                backdrop.remove();
            });
        }

        // --- Mostrar confirmación de guardado ---
        function mostrarExitoTurno(texto) {
            cerrarModalTurno();
            mostrarAlertaTurno(texto);
        }

        // --- Ocultar errores del formulario de turnos ---
        function ocultarErrorTurno() {
            const lbl = document.getElementById("<%= lblErrorTurno.ClientID %>");

            if (lbl) {
                lbl.textContent = "";
                lbl.classList.remove("d-block");
                lbl.classList.add("d-none");
            }
        }

        // --- Ocultar errores del modal de atención ---
        function ocultarErrorAtencion() {
            const lbl = document.getElementById("<%= lblErrorAtenderTurno.ClientID %>");

            if (lbl) {
                lbl.textContent = "";
                lbl.classList.remove("d-block");
                lbl.classList.add("d-none");
            }
        }

        // Gris clarito solo si el combo está en "0"/vacío o si el input de fecha está vacío
        function actualizarEstiloPlaceholderSelects() {
            const selectIds = [
                "<%= ddlPaciente.ClientID %>",
                "<%= ddlEspecialidad.ClientID %>",
                "<%= ddlMedico.ClientID %>",
                "<%= ddlHorarioDisponible.ClientID %>"
            ];

            selectIds.forEach(function (id) {
                const ddl = document.getElementById(id);
                if (!ddl) return;

                const estaVacio = !ddl.value || ddl.value === "0" || ddl.selectedIndex <= 0;
                ddl.classList.toggle("placeholder-select", estaVacio);
            });

            const inpFecha = document.getElementById("<%= txtFechaTurno.ClientID %>");
            if (inpFecha) {
                const estaVacio = !inpFecha.value || inpFecha.value.trim() === "";
                inpFecha.classList.toggle("placeholder-time", estaVacio);
            }
        }

        function engancharEventosCamposTurno() {
            const campoIds = [
                "<%= ddlPaciente.ClientID %>",
                "<%= ddlEspecialidad.ClientID %>",
                "<%= ddlMedico.ClientID %>",
                "<%= ddlHorarioDisponible.ClientID %>",
                "<%= txtFechaTurno.ClientID %>"
            ];

            campoIds.forEach(function (id) {
                const el = document.getElementById(id);
                if (!el || el.dataset.placeholderBound === "1") return;

                el.dataset.placeholderBound = "1";
                el.addEventListener("change", actualizarEstiloPlaceholderSelects);
                el.addEventListener("input", actualizarEstiloPlaceholderSelects);
                el.addEventListener("blur", actualizarEstiloPlaceholderSelects);
            });
        }

        // --- Abrir modal en modo alta ---
        function prepararNuevoTurno() {
            document.getElementById("tituloModalTurno").textContent = "Nuevo Turno";
            mostrarModalTurno();
        }

        // Reabre el modal cuando ocurre un postback dentro del UpdatePanel
        function mostrarModalTurno() {
            engancharEventosCamposTurno();
            actualizarEstiloPlaceholderSelects();

            const modalEl = document.getElementById("modalTurno");
            if (!modalEl) return;

            modalEl.classList.add("show");
            modalEl.style.display = "block";
            modalEl.removeAttribute("aria-hidden");
            document.body.classList.add("modal-open");

            if (!document.querySelector(".modal-backdrop")) {
                const backdrop = document.createElement("div");
                backdrop.className = "modal-backdrop fade show";
                document.body.appendChild(backdrop);
            }

            bootstrap.Modal.getOrCreateInstance(modalEl);
        }

        (function () {
            const modalTurno = document.getElementById("modalTurno");
            if (modalTurno) {
                modalTurno.addEventListener("show.bs.modal", function () {
                    ocultarErrorTurno();
                    engancharEventosCamposTurno();
                    actualizarEstiloPlaceholderSelects();
                });
                modalTurno.addEventListener("input", ocultarErrorTurno);
            }

            engancharEventosCamposTurno();
            actualizarEstiloPlaceholderSelects();

            if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    engancharEventosCamposTurno();
                    actualizarEstiloPlaceholderSelects();
                });
            }
        })();
    </script>

</asp:Content>
