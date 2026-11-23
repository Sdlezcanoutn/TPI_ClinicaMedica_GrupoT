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
                            OnClientClick="prepararNuevoTurno(); return false;" />
                    </div>
                </div>

                <!-- Buscador y Filtro por Paciente / Médico -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server"
                                CssClass="form-control bg-light border-start-0"
                                Placeholder="Buscar por Paciente o Médico..."
                                AutoPostBack="True"
                                OnTextChanged="txtBuscar_TextChanged" />
                        </div>
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
                                            <asp:LinkButton ID="btnEditar" runat="server"
                                                CssClass="btn btn-sm btn-outline-secondary me-1"
                                                CommandArgument='<%# Eval("Id") %>'>
                                                <i class="fa-solid fa-pen-to-square me-1"></i> Editar
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
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <asp:UpdatePanel ID="upTurno" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="modal-body">

                            <asp:Label ID="lblErrorTurno" runat="server"
                                CssClass="text-danger small d-block mb-3"
                                Visible="false" />

                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Paciente *</label>
                                    <asp:DropDownList ID="ddlPaciente" runat="server" CssClass="form-select" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Especialidad *</label>
                                    <asp:DropDownList ID="ddlEspecialidad" runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlEspecialidad_SelectedIndexChanged" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Médico *</label>
                                    <asp:DropDownList ID="ddlMedico" runat="server" CssClass="form-select" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Fecha *</label>
                                    <asp:TextBox ID="txtFechaTurno" runat="server"
                                        CssClass="form-control"
                                        TextMode="Date" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Hora inicio *</label>
                                    <asp:TextBox ID="txtHoraInicio" runat="server"
                                        CssClass="form-control"
                                        TextMode="Time" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Hora fin *</label>
                                    <asp:TextBox ID="txtHoraFin" runat="server"
                                        CssClass="form-control"
                                        TextMode="Time" />
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
                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
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

    // Cierra el modal tras guardar y dispara el aviso de éxito
    function mostrarExitoTurno(texto) {
        const modalEl = document.getElementById("modalTurno");
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.hide();
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

    // Gris clarito solo si el combo está en "0"/vacío o si el input de fecha/hora está vacío
    function actualizarEstiloPlaceholderSelects() {
        const selectIds = [
            "<%= ddlPaciente.ClientID %>",
            "<%= ddlEspecialidad.ClientID %>",
            "<%= ddlMedico.ClientID %>"
        ];

        selectIds.forEach(function (id) {
            const ddl = document.getElementById(id);
            if (!ddl) return;

            const estaVacio = !ddl.value || ddl.value === "0" || ddl.selectedIndex <= 0;
            ddl.classList.toggle("placeholder-select", estaVacio);
        });

        const inputIds = [
            "<%= txtHoraInicio.ClientID %>",
            "<%= txtHoraFin.ClientID %>",
            "<%= txtFechaTurno.ClientID %>"
        ];

        inputIds.forEach(function (id) {
            const inp = document.getElementById(id);
            if (!inp) return;

            const estaVacio = !inp.value || inp.value.trim() === "";
            inp.classList.toggle("placeholder-time", estaVacio);
        });
    }

    function engancharEventosCamposTurno() {
        const campoIds = [
            "<%= ddlPaciente.ClientID %>",
            "<%= ddlEspecialidad.ClientID %>",
            "<%= ddlMedico.ClientID %>",
            "<%= txtHoraInicio.ClientID %>",
            "<%= txtHoraFin.ClientID %>",
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

    // Deja los campos del modal limpios para un nuevo turno
    function limpiarFormularioTurno() {
        const ddlPac = document.getElementById("<%= ddlPaciente.ClientID %>");
        const ddlEsp = document.getElementById("<%= ddlEspecialidad.ClientID %>");
        const ddlMed = document.getElementById("<%= ddlMedico.ClientID %>");

        if (ddlPac) ddlPac.selectedIndex = 0;
        if (ddlEsp) ddlEsp.selectedIndex = 0;
        if (ddlMed) ddlMed.selectedIndex = 0;

        const txtFecha = document.getElementById("<%= txtFechaTurno.ClientID %>");
        const txtInicio = document.getElementById("<%= txtHoraInicio.ClientID %>");
        const txtFin = document.getElementById("<%= txtHoraFin.ClientID %>");
        const txtMotivo = document.getElementById("<%= txtMotivoConsulta.ClientID %>");

        if (txtFecha) txtFecha.value = "";
        if (txtInicio) txtInicio.value = "";
        if (txtFin) txtFin.value = "";
        if (txtMotivo) txtMotivo.value = "";

        ocultarErrorTurno();
        actualizarEstiloPlaceholderSelects();
    }

    // Abre el modal en modo alta
    function prepararNuevoTurno() {
        limpiarFormularioTurno();
        document.getElementById("tituloModalTurno").textContent = "Nuevo Turno";
        document.getElementById("<%= btnGuardarTurno.ClientID %>").value = "Guardar Turno";

        bootstrap.Modal.getOrCreateInstance(
            document.getElementById("modalTurno")
        ).show();
    }

    // Reabre el modal cuando ocurre un postback dentro del UpdatePanel
    function mostrarModalTurno() {
        engancharEventosCamposTurno();
        actualizarEstiloPlaceholderSelects();
        bootstrap.Modal.getOrCreateInstance(
            document.getElementById("modalTurno")
        ).show();
    }

    (function () {
        const modalTurno = document.getElementById("modalTurno");
        if (modalTurno) {
            modalTurno.addEventListener("hidden.bs.modal", limpiarFormularioTurno);
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
