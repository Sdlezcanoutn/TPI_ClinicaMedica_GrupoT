<%@ Page Title="Especialidades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Especialidades.aspx.cs" Inherits="PresentacionWeb.Especialidades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%= ResolveUrl("~/Content/especialidades.css") %>" rel="stylesheet" />

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div class="contenedor-especialidades">

            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Especialidades</h2>
                        <p class="text-muted small m-0 mt-1">Listado y administración de especialidades médicas</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nueva Especialidad" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm" OnClientClick="prepararNuevaEspecialidad(); return false;" />
                    </div>
                </div>

                <!-- Buscador -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control bg-light border-start-0" Placeholder="Buscar por nombre..." AutoPostBack="false" oninput="programarBusqueda()" onkeydown="if (event.key === 'Enter') { event.preventDefault(); ejecutarBusqueda(); }"></asp:TextBox>
                            <!-- Botón oculto para ejecutar la búsqueda -->
                            <asp:LinkButton ID="btnBuscar" runat="server" OnClick="btnBuscar_Click" CssClass="boton-busqueda-oculto"></asp:LinkButton>
                        </div>
                    </div>
                </div>

                <!-- Filtro por estado -->
                <div class="d-flex gap-2 mb-4">

                    <asp:LinkButton ID="btnActivas" runat="server"
                        CssClass="btn btn-filtro-activo"
                        OnClick="btnActivas_Click">
        Activas
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnInactivas" runat="server"
                        CssClass="btn btn-outline-secondary"
                        OnClick="btnInactivas_Click">
        Inactivas
                    </asp:LinkButton>

                    <asp:HiddenField ID="hfEstado" runat="server" Value="1" />

                </div>

                <!-- Tabla -->
                <asp:UpdatePanel ID="upListado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div id="mensajeExito"
                            class="alert alert-success d-none mb-3"
                            role="alert">
                            Especialidad registrada correctamente.
                        </div>

                        <div class="table-responsive">
                            <asp:GridView ID="dgvEspecialidades" runat="server" AutoGenerateColumns="False"
                                CssClass="table table-custom align-middle mb-0" GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="80px" />
                                    <asp:BoundField DataField="Nombre" HeaderText="Especialidad" />
                                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                    <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">

                                        <ItemTemplate>

                                            <%-- Editar: solo especialidades activas --%>
                                            <asp:LinkButton ID="btnEditar" runat="server"
                                                CssClass="btn btn-sm btn-outline-secondary me-1"
                                                CommandArgument='<%# Eval("Id") %>'
                                                Visible='<%# Convert.ToBoolean(Eval("Estado")) %>'
                                                OnClick="btnEditar_Click">
                                        <i class="fa-solid fa-pen-to-square me-1"></i> Editar
                                            </asp:LinkButton>

                                            <%-- Dar de baja: solo especialidades activas --%>
                                            <asp:LinkButton ID="btnEliminar" runat="server"
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CommandArgument='<%# Eval("Id") %>'
                                                Visible='<%# Convert.ToBoolean(Eval("Estado")) %>'
                                                OnClick="btnEliminar_Click">
                                                <i class="fa-solid fa-trash me-1"></i> Dar de baja
                                            </asp:LinkButton>

                                            <%-- Reactivar: solo especialidades inactivas --%>
                                            <asp:LinkButton ID="btnReactivar" runat="server"
                                                CssClass="btn btn-sm btn-outline-success"
                                                CommandArgument='<%# Eval("Id") %>'
                                                Visible='<%# !Convert.ToBoolean(Eval("Estado")) %>'
                                                OnClick="btnReactivar_Click">
                                                <i class="fa-solid fa-rotate-left me-1"></i> Reactivar
                                            </asp:LinkButton>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center py-5 text-muted">
                                        <i class="fa-solid fa-folder-open fs-2 mb-2 d-block text-secondary opacity-50"></i>
                                        <p class="m-0 fw-medium">
                                            <asp:Label ID="lblSinResultados" runat="server"
                                                Text="No hay especialidades registradas por el momento." />
                                        </p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnBuscar" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>


                <!-- Modal Confirmación de Baja -->
                <div class="modal fade" id="modalBajaEspecialidad" tabindex="-1"
                    aria-labelledby="tituloModalBaja" aria-hidden="true">

                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title" id="tituloModalBaja">Dar de baja especialidad </h5>
                                <button type="button" class="btn-close"
                                    data-bs-dismiss="modal" aria-label="Cerrar">
                                </button>
                            </div>

                            <asp:UpdatePanel ID="upBajaEspecialidad" runat="server"
                                UpdateMode="Conditional">
                                <ContentTemplate>

                                    <asp:HiddenField ID="hfIdBaja" runat="server" />

                                    <div class="modal-body">
                                        <p>
                                            ¿Está seguro de que desea dar de baja la especialidad
                           
                                            <strong>
                                                <asp:Label ID="lblNombreBaja" runat="server" /></strong>?
                                        </p>

                                        <div class="alert alert-warning">
                                            <asp:Label ID="lblTurnosPendientes" runat="server" />
                                        </div>

                                        <p>
                                            Los turnos ya asignados se mantendrán y podrán
                            atenderse normalmente.
                       
                                        </p>

                                        <p class="text-muted small mb-0">
                                            Una vez confirmada la baja, no se podrán asignar
                            nuevos turnos en esta especialidad.
                       
                                        </p>
                                    </div>

                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary"
                                            data-bs-dismiss="modal">
                                            Cancelar
                       
                                        </button>

                                        <asp:Button ID="btnConfirmarBaja" runat="server"
                                            Text="Confirmar baja"
                                            CssClass="btn btn-danger"
                                            OnClick="btnConfirmarBaja_Click" />
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                </div>


                <!-- Modal Nueva Especialidad -->
                <div class="modal fade" id="modalEspecialidad" tabindex="-1"
                    aria-labelledby="tituloModalEspecialidad" aria-hidden="true">

                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title" id="tituloModalEspecialidad">Nueva Especialidad</h5>
                                <button type="button" class="btn-close"
                                    data-bs-dismiss="modal" aria-label="Cerrar">
                                </button>
                            </div>

                            <asp:UpdatePanel ID="upEspecialidad" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

                                    <asp:HiddenField ID="hfIdEspecialidad" runat="server" />

                                    <div class="modal-body">

                                        <div class="mb-3">
                                            <label class="form-label">Nombre</label>
                                            <asp:TextBox ID="txtNombre" runat="server"
                                                CssClass="form-control"
                                                MaxLength="100"
                                                oninput="actualizarContadores()" />

                                            <div class="text-muted small mt-1" style="max-width: 280px; text-align: right;">
                                                <span id="contadorNombre">0</span> / 100
                                           
                                            </div>
                                        </div>

                                        <div class="mb-3">
                                            <label class="form-label">Descripción</label>
                                            <asp:TextBox ID="txtDescripcion" runat="server"
                                                CssClass="form-control"
                                                TextMode="MultiLine"
                                                Rows="3"
                                                MaxLength="500"
                                                oninput="actualizarContadores()" />

                                            <div class="text-muted small mt-1" style="max-width: 280px; text-align: right;">
                                                <span id="contadorDescripcion">0</span> / 500
                                            </div>
                                        </div>

                                        <asp:Label ID="lblMensaje" runat="server"
                                            CssClass="text-danger small"
                                            Visible="false" />
                                    </div>

                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary"
                                            data-bs-dismiss="modal">
                                            Cancelar
                                        </button>

                                        <asp:Button ID="btnGuardar" runat="server"
                                            Text="Guardar"
                                            CssClass="btn btn-clinica-primary"
                                            OnClick="btnGuardar_Click" />
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                </div>

                <!-- Modal Confirmación de Reactivación -->
                <div class="modal fade" id="modalReactivarEspecialidad" tabindex="-1"
                    aria-labelledby="tituloModalReactivar" aria-hidden="true">

                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">

                            <div class="modal-header">
                                <h5 class="modal-title" id="tituloModalReactivar">Reactivar especialidad
                                </h5>
                                <button type="button" class="btn-close"
                                    data-bs-dismiss="modal" aria-label="Cerrar">
                                </button>
                            </div>

                            <asp:UpdatePanel ID="upReactivarEspecialidad" runat="server"
                                UpdateMode="Conditional">
                                <ContentTemplate>

                                    <asp:HiddenField ID="hfIdReactivar" runat="server" />

                                    <div class="modal-body">
                                        <p>
                                            ¿Está seguro de que desea reactivar la especialidad
                           
                                                        <strong>
                                                            <asp:Label ID="lblNombreReactivar" runat="server" /></strong>?
                                        </p>

                                        <p class="text-muted small mb-0">
                                            Una vez reactivada, la especialidad volverá a estar
                            disponible para la asignación de nuevos turnos.
                       
                                        </p>
                                    </div>

                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary"
                                            data-bs-dismiss="modal">
                                            Cancelar
                       
                                        </button>

                                        <asp:Button ID="btnConfirmarReactivar" runat="server"
                                            Text="Confirmar reactivación"
                                            CssClass="btn btn-success"
                                            OnClick="btnConfirmarReactivar_Click" />
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                </div>

            </div>
        </div>

        <script>
    // ============================================================
    // BÚSQUEDA DE ESPECIALIDADES
    // ============================================================

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

    // ============================================================
    // MODAL DE ESPECIALIDADES
    // ============================================================

    const modalEspecialidad = document.getElementById('modalEspecialidad');

    // --- Actualizar contadores de caracteres ---
    function actualizarContadores() {
        const nombre = document.getElementById('<%= txtNombre.ClientID %>');
        const descripcion = document.getElementById('<%= txtDescripcion.ClientID %>');

        if (nombre.value.length > 100) {
            nombre.value = nombre.value.substring(0, 100);
        }

        if (descripcion.value.length > 500) {
            descripcion.value = descripcion.value.substring(0, 500);
        }

        document.getElementById('contadorNombre').textContent = nombre.value.length;
        document.getElementById('contadorDescripcion').textContent = descripcion.value.length;
    }

    // --- Preparar formulario para nueva especialidad ---
    function prepararNuevaEspecialidad() {
        document.getElementById('<%= hfIdEspecialidad.ClientID %>').value = '';
        document.getElementById('<%= txtNombre.ClientID %>').value = '';
        document.getElementById('<%= txtDescripcion.ClientID %>').value = '';

        actualizarContadores();

        document.getElementById('tituloModalEspecialidad').textContent = 'Nueva Especialidad';
        document.getElementById('<%= btnGuardar.ClientID %>').value = 'Guardar';

        ocultarError();


        bootstrap.Modal.getOrCreateInstance(modalEspecialidad).show();
    }

    // --- Preparar formulario para editar especialidad ---
    function prepararEdicionEspecialidad() {
        document.getElementById('tituloModalEspecialidad').textContent = 'Editar Especialidad';

        document.getElementById('<%= btnGuardar.ClientID %>').value = 'Guardar cambios';

        actualizarContadores();

        bootstrap.Modal.getOrCreateInstance(modalEspecialidad).show();
    }

    // --- Ocultar mensajes de error ---
    function ocultarError() {
        var mensaje = document.getElementById('<%= lblMensaje.ClientID %>');

        if (mensaje) {
            mensaje.textContent = '';
            mensaje.style.display = 'none';
        }
    }

    // --- Limpiar campos al cerrar el modal ---
    modalEspecialidad.addEventListener('hidden.bs.modal', function () {
        document.getElementById('<%= txtNombre.ClientID %>').value = '';
        document.getElementById('<%= txtDescripcion.ClientID %>').value = '';

        actualizarContadores();
    });

    // --- Ocultar errores al abrir el modal ---
    modalEspecialidad.addEventListener('show.bs.modal', ocultarError);

    // --- Ocultar errores al modificar el nombre ---
    modalEspecialidad.addEventListener('input', function (event) {
        if (event.target.id === '<%= txtNombre.ClientID %>') {
            ocultarError();
        }
    });

    // --- Actualizar contadores después de un postback parcial ---
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        actualizarContadores();
    });
 
    // --- Cerrar modal y mostrar confirmación ---
    function mostrarExito(texto) {
        var elementoModal = document.getElementById('modalEspecialidad');
        var modal = bootstrap.Modal.getOrCreateInstance(elementoModal);

        modal.hide();

        var mensaje = document.getElementById('mensajeExito');
        mensaje.textContent = texto;
        mensaje.classList.remove('d-none');

        setTimeout(function () {
            mensaje.classList.add('d-none');
        }, 4000);
    }
        </script>
    </div>
</asp:Content>
