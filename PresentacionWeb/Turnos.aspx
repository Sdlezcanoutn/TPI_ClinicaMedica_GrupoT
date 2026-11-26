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
                        <asp:Button ID="btnAgregar" runat="server"
                        Text="+ Nuevo Turno"
                        CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm"
                        OnClick="btnAgregar_Click" />
                    </div>
                </div>

                <!-- Listado, filtros y alertas -->
                <asp:UpdatePanel ID="upListadoTurnos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="row g-3 mb-4">
                            <div class="col-md-6 col-lg-3">
                                <div class="input-group search-box">
                                    <span class="input-group-text bg-light border-end-0">
                                        <i class="fa-solid fa-magnifying-glass"></i>
                                    </span>
                                    <asp:TextBox ID="txtBuscar" runat="server"
                                        CssClass="form-control bg-light border-start-0"
                                        Placeholder="Buscar por Paciente o Médico..."
                                        AutoPostBack="true"
                                        OnTextChanged="txtBuscar_TextChanged" />
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
                                            <asp:LinkButton ID="btnEditar" runat="server"
                                                CssClass="btn btn-sm btn-outline-secondary me-1"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnEditar_Click">
                                                <i class="fa-solid fa-calendar-days me-1"></i> Reprogramar
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnCancelar" runat="server"
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CommandArgument='<%# Eval("Id") %>'>
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
                    <h5 class="modal-title fw-bold" id="tituloModalTurno">Nuevo Turno</h5>
                    <button type="button" class="btn-close" onclick="cerrarModalTurno();" aria-label="Cerrar"></button>
                </div>

                <asp:UpdatePanel ID="upTurno" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdTurno" runat="server" Value="0" />

                        <div class="modal-body">

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
                            <button type="button" class="btn btn-outline-secondary" onclick="cerrarModalTurno();">Cancelar</button>
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
    // Muestra el cartel verde de confirmación en el listado y lo oculta a los 4 segundos
    function mostrarAlertaTurno(texto) {
        const alerta = document.getElementById("mensajeExitoTurno");
        if (!alerta) return;

        alerta.textContent = texto;
        alerta.classList.remove("d-none");

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

        document.querySelectorAll(".modal-backdrop").forEach(function(backdrop) {
            backdrop.remove();
        });
    }

    // --- Mostrar confirmación de guardado ---
    function mostrarExitoTurno(texto) {
        cerrarModalTurno();
        mostrarAlertaTurno(texto);
    }

    // Limpia y oculta errores previos del modal
    function ocultarErrorTurno() {
        const lbl = document.getElementById("<%= lblErrorTurno.ClientID %>");
        if (lbl) {
            lbl.textContent = "";
            lbl.style.display = "none";
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
