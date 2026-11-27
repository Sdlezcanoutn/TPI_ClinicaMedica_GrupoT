<%@ Page Title="Empleados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Empleados.aspx.cs" Inherits="PresentacionWeb.Empleados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 1500px; margin: 0 auto;">

            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Empleados</h2>
                        <p class="text-muted small m-0 mt-1">Administración del personal de la clínica</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <asp:Button ID="btnAgregar" runat="server"
                            Text="+ Nuevo Empleado"
                            CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm"
                            OnClientClick="prepararNuevoEmpleado(); return false;" />
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

                <!-- Filtro de estado (Activos por defecto / Inactivos) -->
                <asp:HiddenField ID="hfEstado" runat="server" Value="1" />

                <div class="d-flex gap-2 mb-4">
                    <asp:LinkButton ID="btnActivos" runat="server"
                        CssClass="btn btn-sm btn-clinica-primary px-3 rounded-3"
                        OnClick="btnActivos_Click">
                        Activos
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnInactivos" runat="server"
                        CssClass="btn btn-sm btn-outline-secondary px-3 rounded-3"
                        OnClick="btnInactivos_Click">
                        Inactivos
                    </asp:LinkButton>
                </div>

                <!-- Listado y alertas -->
                <asp:UpdatePanel ID="upListado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:Label ID="lblErrorListado" runat="server"
                            CssClass="text-danger small d-block mb-2"
                            Visible="false" />

                        <div id="mensajeExitoEmpleado" class="alert alert-success d-none mb-3" role="alert"></div>

                        <!-- Tabla de Empleados -->
                        <div class="table-responsive">
                            <asp:GridView ID="dgvEmpleados" runat="server" AutoGenerateColumns="False"
                                CssClass="table table-custom align-middle mb-0" GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="DNI" HeaderText="DNI" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="110px" />
                                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                    <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                                    <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEditar" runat="server"
                                                CssClass="btn btn-sm btn-outline-secondary me-1"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnEditar_Click"
                                                Visible='<%# Convert.ToBoolean(Eval("Estado")) %>'>
                                                <i class="fa-solid fa-pen-to-square me-1"></i>Editar
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnEliminar" runat="server"
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnEliminar_Click"
                                                Visible='<%# Convert.ToBoolean(Eval("Estado")) %>'>
                                               <i class="fa-solid fa-user-xmark me-1"></i>Desactivar
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnReactivar" runat="server"
                                                CssClass="btn btn-sm btn-outline-success"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClick="btnReactivar_Click"
                                                Visible='<%# !Convert.ToBoolean(Eval("Estado")) %>'>
                                          
                                                
                                                
                                                <i class="fa-solid fa-rotate-left me-1"></i>Reactivar
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center py-5 text-muted">
                                        <i class="fa-solid fa-user-slash fs-2 mb-2 d-block text-secondary opacity-50"></i>
                                        <p class="m-0 fw-medium">
                                            <asp:Label ID="lblVacioEmpleados" runat="server" Text="No hay empleados registrados en el sistema." />
                                        </p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Alta / Edición de Empleado -->
    <div class="modal fade" id="modalEmpleado" tabindex="-1"
        aria-labelledby="tituloModalEmpleado" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title fw-bold" id="tituloModalEmpleado">Nuevo Empleado</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <asp:UpdatePanel ID="upEmpleado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdEmpleado" runat="server" />

                        <div class="modal-body">

                            <asp:Label ID="lblErrorEmpleado" runat="server"
                                CssClass="text-danger small d-block mb-3"
                                Visible="false" />

                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Nombre *</label>
                                    <asp:TextBox ID="txtNombreEmpleado" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">Apellido *</label>
                                    <asp:TextBox ID="txtApellidoEmpleado" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label">DNI *</label>
                                    <asp:TextBox ID="txtDniEmpleado" runat="server" CssClass="form-control" MaxLength="20" />
                                </div>
                            </div>

                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <asp:Button ID="btnGuardarEmpleado" runat="server"
                                Text="Guardar Empleado"
                                CssClass="btn btn-clinica-primary"
                                OnClick="btnGuardarEmpleado_Click" />
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Baja Lógica de Empleado -->
    <div class="modal fade" id="modalBajaEmpleado" tabindex="-1"
        aria-labelledby="tituloModalBajaEmpleado" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <asp:UpdatePanel ID="upBajaEmpleado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdBajaEmpleado" runat="server" />

                        <div class="modal-header">
                            <h5 class="modal-title fw-bold" id="tituloModalBajaEmpleado">Dar de baja empleado</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                        </div>

                        <div class="modal-body">
                            <p class="mb-2">
                                ¿Está seguro de que desea dar de baja al empleado
                            <strong>
                                <asp:Label ID="lblNombreBajaEmpleado" runat="server" /></strong>?
                            </p>
                            <p class="mb-1">El registro del empleado se conservará en el sistema.</p>
                            <p class="text-muted small mb-0">
                                Una vez confirmada la baja, el empleado pasará a figurar como inactivo y podrá reactivarse más adelante.
                            </p>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <asp:LinkButton ID="btnConfirmarBajaEmpleado" runat="server"
                                CssClass="btn btn-danger"
                                OnClick="btnConfirmarBajaEmpleado_Click">
                            Confirmar baja
                            </asp:LinkButton>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <!-- Modal Reactivación de Empleado -->
    <div class="modal fade" id="modalReactivarEmpleado" tabindex="-1"
        aria-labelledby="tituloModalReactivarEmpleado" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <asp:UpdatePanel ID="upReactivarEmpleado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:HiddenField ID="hfIdReactivarEmpleado" runat="server" />

                        <div class="modal-header">
                            <h5 class="modal-title fw-bold" id="tituloModalReactivarEmpleado">Reactivar empleado</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                        </div>

                        <div class="modal-body">
                            <p class="mb-2">
                                ¿Está seguro de que desea reactivar al empleado
                            <strong>
                                <asp:Label ID="lblNombreReactivarEmpleado" runat="server" /></strong>?
                            </p>
                            <p class="text-muted small mb-0">
                                Una vez reactivado, el empleado volverá a figurar como activo en la clínica.
                            </p>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <asp:LinkButton ID="btnConfirmarReactivarEmpleado" runat="server"
                                CssClass="btn btn-success"
                                OnClick="btnConfirmarReactivarEmpleado_Click">
                            Confirmar reactivación
                            </asp:LinkButton>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <script>
        // Muestra el cartel verde de confirmación en el listado y lo oculta a los 4 segundos
        function mostrarAlertaEmpleado(texto) {
            const alerta = document.getElementById("mensajeExitoEmpleado");
            if (!alerta) return;

            alerta.textContent = texto;
            alerta.classList.remove("d-none");

            setTimeout(function () {
                alerta.classList.add("d-none");
            }, 4000);
        }

        // Cierra el modal tras guardar y dispara el aviso de éxito
        function mostrarExitoEmpleado(texto) {
            const modalEl = document.getElementById("modalEmpleado");
            const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.hide();
            mostrarAlertaEmpleado(texto);
        }

        // Limpia y oculta errores previos del formulario
        function ocultarErrorEmpleado() {
            const lbl = document.getElementById("<%= lblErrorEmpleado.ClientID %>");
            if (lbl) {
                lbl.textContent = "";
                lbl.style.display = "none";
            }
        }

        // Deja el formulario listo para un alta limpia
        function limpiarFormularioEmpleado() {
            document.getElementById("<%= hfIdEmpleado.ClientID %>").value = "";
            document.getElementById("<%= txtNombreEmpleado.ClientID %>").value = "";
            document.getElementById("<%= txtApellidoEmpleado.ClientID %>").value = "";
            document.getElementById("<%= txtDniEmpleado.ClientID %>").value = "";
            ocultarErrorEmpleado();
        }

        // Abre el modal en modo alta
        function prepararNuevoEmpleado() {
            limpiarFormularioEmpleado();
            document.getElementById("tituloModalEmpleado").textContent = "Nuevo Empleado";
            document.getElementById("<%= btnGuardarEmpleado.ClientID %>").value = "Guardar Empleado";

            bootstrap.Modal.getOrCreateInstance(
                document.getElementById("modalEmpleado")
            ).show();
        }

        // Abre el modal en modo edición con los datos ya cargados desde el servidor
        function prepararEdicionEmpleado() {
            document.getElementById("tituloModalEmpleado").textContent = "Editar Empleado";
            document.getElementById("<%= btnGuardarEmpleado.ClientID %>").value = "Guardar Cambios";

            bootstrap.Modal.getOrCreateInstance(
                document.getElementById("modalEmpleado")
            ).show();
        }

        // Limpia al cerrar y oculta errores al abrir o escribir
        (function () {
            const modalEmpleado = document.getElementById("modalEmpleado");
            if (modalEmpleado) {
                modalEmpleado.addEventListener("hidden.bs.modal", limpiarFormularioEmpleado);
                modalEmpleado.addEventListener("show.bs.modal", ocultarErrorEmpleado);
                modalEmpleado.addEventListener("input", ocultarErrorEmpleado);
            }
        })();
    </script>

</asp:Content>
