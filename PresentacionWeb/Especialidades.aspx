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
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nueva Especialidad" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm" OnClientClick="bootstrap.Modal.getOrCreateInstance(document.getElementById('modalEspecialidad')).show(); return false;" />
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
                            <asp:LinkButton ID="btnBuscar" runat="server" OnClick="btnBuscar_Click" Style="display: none;"></asp:LinkButton>
                        </div>
                    </div>
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

                            <asp:UpdatePanel ID="upEspecialidad" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

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
    });

    // --- Ocultar errores al abrir el modal ---
    modalEspecialidad.addEventListener('show.bs.modal', ocultarError);

    // --- Ocultar errores al modificar el nombre ---
    modalEspecialidad.addEventListener('input', function (event) {
        if (event.target.id === '<%= txtNombre.ClientID %>') {
            ocultarError();
        }
    });
 
    // --- Cerrar modal y mostrar confirmación ---
    function mostrarExito() {
        var elementoModal = document.getElementById('modalEspecialidad');
        var modal = bootstrap.Modal.getOrCreateInstance(elementoModal);

        modal.hide();

        var mensaje = document.getElementById('mensajeExito');
        mensaje.classList.remove('d-none');

        setTimeout(function () {
            mensaje.classList.add('d-none');
        }, 4000);
    }
        </script>
    </div>
</asp:Content>
