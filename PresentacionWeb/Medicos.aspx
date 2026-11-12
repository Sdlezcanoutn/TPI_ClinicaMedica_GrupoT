<%@ Page Title="Médicos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Medicos.aspx.cs" Inherits="PresentacionWeb.Medicos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <style>
        /* ============================================================
           SELECTOR DE ESPECIALIDADES
           ============================================================ */

        .lista-especialidades-medico {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

            /* Alinear la casilla junto al nombre */
            .lista-especialidades-medico > span {
                display: flex;
                align-items: center;
                gap: 10px;
                padding: 9px 12px;
                border-radius: 8px;
            }

                /* Resaltar la opción al pasar el mouse */
                .lista-especialidades-medico > span:hover {
                    background-color: #f1f5f9;
                }

            .lista-especialidades-medico input[type="checkbox"] {
                margin: 0;
                flex-shrink: 0;
                cursor: pointer;
            }

            .lista-especialidades-medico label {
                margin: 0;
                cursor: pointer;
            }

        /* ============================================================
           TAMAÑO ESTABLE DEL FORMULARIO DE MÉDICOS
           ============================================================ */

        /* Mantener una altura estable sin superar la pantalla */
        .modal-medico-fijo .modal-content {
            height: min(640px, calc(100dvh - 32px));
            display: flex;
            flex-direction: column;
        }

            /* Distribuir el contenido del UpdatePanel dentro del modal */
            .modal-medico-fijo .modal-content > div[id*="upMedico"] {
                display: flex;
                flex-direction: column;
                flex: 1 1 auto;
                min-height: 0;
            }

        /* Formulario fijo, sin desplazamiento */
        .modal-medico-fijo .modal-body {
            flex: 1 1 auto;
            min-height: 0;
            overflow: hidden;
        }

        /* Mantener el desplegable de especialidades con su propio desplazamiento */
        #panelEspecialidades {
            height: 130px;
            overflow-y: auto;
            overscroll-behavior: contain;
        }

        /* Mantener encabezado y botones en su lugar */
        .modal-medico-fijo .modal-header,
        .modal-medico-fijo .modal-footer {
            flex-shrink: 0;
        }
    </style>

    <!-- CONTENEDOR -->
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 1500px; margin: 0 auto;">

            <!-- TARJETA PANEL PRINCIPAL DASHBOARD -->
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">

                <!-- Encabezado integrado dentro del panel -->
                <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 pb-3 border-bottom">
                    <div>
                        <h2 class="fw-bold text-dark m-0">Gestión de Médicos</h2>
                        <p class="text-muted small m-0 mt-1">Administración del cuerpo médico y especialidades asignadas</p>
                    </div>
                    <div class="mt-3 mt-md-0">
                        <!-- Botón para abrir el formulario de alta de médicos -->
                        <asp:Button ID="btnAgregar" runat="server" Text="+ Nuevo Médico" CssClass="btn btn-clinica-primary px-4 py-2 rounded-3 shadow-sm"
                            OnClientClick="bootstrap.Modal.getOrCreateInstance(document.getElementById('modalMedico')).show(); return false;" />
                    </div>
                </div>

                <!-- Buscador y Filtro por Especialidad -->
                <div class="row g-3 mb-4">
                    <div class="col-md-6 col-lg-4">
                        <div class="input-group search-box">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="fa-solid fa-magnifying-glass"></i>
                            </span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control bg-light border-start-0" Placeholder="Buscar por Nombre o Matrícula..."></asp:TextBox>
                        </div>
                    </div>
                </div>

                <asp:Label
                    ID="lblErrorListado"
                    runat="server"
                    CssClass="alert alert-danger d-block mb-3"
                    Visible="false">
                </asp:Label>

                <!-- Tabla de Médicos -->
                <div class="table-responsive">
                    <asp:GridView ID="dgvMedicos" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-custom align-middle mb-0" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="Matricula" HeaderText="Matrícula" ItemStyle-CssClass="fw-semibold text-secondary" HeaderStyle-Width="120px" />
                            <asp:BoundField DataField="NombreCompleto" HeaderText="Médico" />
                            <asp:BoundField DataField="Especialidad" HeaderText="Especialidad" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <span class="badge bg-success-subtle text-success border border-success-subtle px-2 py-1 fw-medium">Activo</span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-outline-secondary me-1">
                                        <i class="fa-solid fa-pen-to-square me-1"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-outline-danger">
                                        <i class="fa-solid fa-user-xmark me-1"></i> Desactivar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center py-5 text-muted">
                                <i class="fa-solid fa-user-doctor fs-2 mb-2 d-block text-secondary opacity-50"></i>
                                <p class="m-0 fw-medium">No hay médicos registrados en el sistema.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

            </div>

        </div>
    </div>
    <!-- ============================================================ -->
    <!-- MODAL DE ALTA DE MÉDICOS -->
    <!-- ============================================================ -->
    <div class="modal fade" id="modalMedico" tabindex="-1"
        aria-labelledby="tituloModalMedico" aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg modal-medico-fijo">
            <div class="modal-content">

                <!-- Encabezado del modal -->
                <div class="modal-header">
                    <h5 class="modal-title fw-bold" id="tituloModalMedico">Nuevo Médico
                    </h5>
                    <button type="button" class="btn-close"
                        data-bs-dismiss="modal" aria-label="Cerrar">
                    </button>
                </div>

                <!-- ==================================================== -->
                <!-- DATOS DEL MÉDICO -->
                <!-- ==================================================== -->
                <asp:UpdatePanel ID="upMedico" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:Label
                            ID="lblErrorMedico"
                            runat="server"
                            CssClass="alert alert-danger d-block py-2 px-3 mb-2"
                            Visible="false" />

                        <div class="modal-body">
                            <div class="row g-2">

                                <!-- Nombre -->
                                <div class="col-md-6">
                                    <label class="form-label">Nombre *</label>
                                    <asp:TextBox ID="txtNombreMedico" runat="server"
                                        CssClass="form-control" MaxLength="100" />
                                </div>

                                <!-- Apellido -->
                                <div class="col-md-6">
                                    <label class="form-label">Apellido *</label>
                                    <asp:TextBox ID="txtApellidoMedico" runat="server"
                                        CssClass="form-control" MaxLength="100" />
                                </div>

                                <!-- DNI -->
                                <div class="col-md-6">
                                    <label class="form-label">DNI *</label>
                                    <asp:TextBox ID="txtDniMedico" runat="server"
                                        CssClass="form-control" MaxLength="20" />
                                </div>

                                <!-- Matrícula -->
                                <div class="col-md-6">
                                    <label class="form-label">Matrícula *</label>
                                    <asp:TextBox ID="txtMatriculaMedico" runat="server"
                                        CssClass="form-control" MaxLength="50" />
                                </div>

                                <!-- Teléfono opcional -->
                                <div class="col-md-6">
                                    <label class="form-label">Teléfono</label>
                                    <asp:TextBox ID="txtTelefonoMedico" runat="server"
                                        CssClass="form-control" MaxLength="30" />
                                </div>

                                <!-- Correo electrónico -->
                                <div class="col-md-6">
                                    <label class="form-label">Correo electrónico *</label>
                                    <asp:TextBox ID="txtEmailMedico" runat="server"
                                        CssClass="form-control" MaxLength="254" />
                                </div>

                                <!-- ============================================================ -->
                                <!-- SELECTOR DE ESPECIALIDADES CON BÚSQUEDA -->
                                <!-- ============================================================ -->
                                <div class="col-12">

                                    <label class="form-label fw-semibold">
                                        Especialidades *
   
                                        <span class="text-muted fw-normal small">(Podés seleccionar una o varias) </span>
                                    </label>

                                    <!-- Botón que abre y cierra el desplegable -->
                                    <button type="button"
                                        class="form-select text-start"
                                        id="btnSelectorEspecialidades"
                                        onclick="alternarEspecialidades()">
                                        Seleccionar especialidades
   
                                    </button>

                                    <!-- Contenedor desplegable -->
                                    <div id="panelEspecialidades"
                                        class="border rounded-3 p-3 mt-2 bg-white"
                                        style="display: none;">

                                        <!-- Buscador -->
                                        <input type="text"
                                            id="txtBuscarEspecialidadMedico"
                                            class="form-control w-100 mb-2"
                                            placeholder="Buscar especialidad..."
                                            oninput="filtrarEspecialidades()" />

                                        <!-- Especialidades cargadas desde la base de datos -->
                                        <asp:CheckBoxList ID="cblEspecialidades"
                                            runat="server"
                                            CssClass="lista-especialidades-medico"
                                            RepeatLayout="Table"
                                            RepeatDirection="Vertical" />

                                    </div>

                                    <!-- Especialidades seleccionadas -->
                                    <div id="especialidadesSeleccionadas"
                                        class="d-flex flex-wrap gap-2 mt-2">
                                    </div>

                                </div>

                            </div>
                        </div>

                        <!-- ==================================================== -->
                        <!-- ACCIONES DEL FORMULARIO -->
                        <!-- ==================================================== -->
                        <div class="modal-footer">
                            <button type="button"
                                class="btn btn-outline-secondary"
                                data-bs-dismiss="modal">
                                Cancelar
                   
                            </button>

                            <asp:Button ID="btnGuardarMedico"
                                runat="server"
                                Text="Guardar Médico"
                                CssClass="btn btn-clinica-primary"
                                OnClick="btnGuardarMedico_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <script>
    // ============================================================
    // NORMALIZACIÓN DE TEXTO PARA BÚSQUEDAS
    // ============================================================
    function normalizarTexto(texto) {
        // Ignorar mayúsculas, minúsculas y tildes.
        return (texto || "")
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .toLowerCase()
            .trim();
    }

    // ============================================================
    // ABRIR Y CERRAR EL DESPLEGABLE
    // ============================================================
    function alternarEspecialidades() {
        const panel = document.getElementById("panelEspecialidades");

        // Mostrar u ocultar las especialidades.
        panel.style.display =
            getComputedStyle(panel).display === "none" ? "block" : "none";
    }

    // ============================================================
    // FILTRAR ESPECIALIDADES EN TIEMPO REAL
    // ============================================================
    function filtrarEspecialidades() {

        // Obtener la búsqueda sin distinguir mayúsculas ni tildes.
        const busqueda = normalizarTexto(
            document.getElementById("txtBuscarEspecialidadMedico").value
        );

        // Obtener las filas de la tabla de especialidades.
        const filas = document.querySelectorAll(
            ".lista-especialidades-medico tr"
        );

        // Mostrar únicamente las especialidades que coincidan.
        filas.forEach(fila => {

            const nombre = normalizarTexto(fila.textContent);

            fila.style.display =
                nombre.includes(busqueda) ? "" : "none";
        });
    }

    // ============================================================
    // ACTUALIZAR LAS ESPECIALIDADES SELECCIONADAS
    // ============================================================
    function actualizarEspecialidadesSeleccionadas() {
        const contenedor = document.getElementById(
            "especialidadesSeleccionadas"
        );

        const boton = document.getElementById(
            "btnSelectorEspecialidades"
        );

        const seleccionadas = document.querySelectorAll(
            ".lista-especialidades-medico input[type='checkbox']:checked"
        );

        // Limpiar las etiquetas anteriores.
        contenedor.innerHTML = "";

        seleccionadas.forEach(checkbox => {
            // Buscar el nombre asociado a la casilla seleccionada.
            const etiqueta = document.querySelector(
                "label[for='" + checkbox.id + "']"
            );

            if (!etiqueta) return;

            // Crear una etiqueta visual para cada especialidad.
            const badge = document.createElement("span");
            badge.className =
                "badge bg-primary-subtle text-primary border p-2";
            badge.textContent = etiqueta.textContent;

            contenedor.appendChild(badge);
        });

        // Actualizar el texto del botón desplegable.
        boton.textContent = seleccionadas.length > 0
            ? seleccionadas.length + " especialidad(es) seleccionada(s)"
            : "Seleccionar especialidades";
    }

    // ============================================================
    // DETECTAR CAMBIOS EN LA SELECCIÓN
    // ============================================================
    document.addEventListener("change", function (evento) {
        if (evento.target.matches(
            ".lista-especialidades-medico input[type='checkbox']"
        )) {
            actualizarEspecialidadesSeleccionadas();
        }
    });
</script>


</asp:Content>
