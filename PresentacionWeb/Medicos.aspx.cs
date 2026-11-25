using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Medicos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ============================================================
            // CARGA INICIAL DE LA PÁGINA
            // ============================================================

            if (!IsPostBack)
            {
                // Cargar el listado de médicos registrados.
                cargarMedicos();

                // Cargar las especialidades disponibles para asignar.
                cargarEspecialidades();
            }
            // Mantener la apariencia del filtro seleccionado.
            actualizarEstiloFiltro();
        }

        // ============================================================
        // LISTADO DE MÉDICOS
        // ============================================================
        private void cargarMedicos()
        {
            // --- Crear instancia de la capa de negocio ---
            MedicoNegocio negocio = new MedicoNegocio();

            try
            {
                // --- Ocultar errores anteriores y habilitar la tabla ---
                lblErrorListado.Visible = false;
                dgvMedicos.Visible = true;

                // --- Filtrar médicos según el estado seleccionado ---
                bool mostrarActivos = hfEstado.Value == "1";

                // Obtiene todos los médicos de la base de datos
                List<Medico> todos = negocio.listar();

                // Filtra según el botón seleccionado
                List<Medico> lista = todos
                    .Where(m => m.Estado == mostrarActivos)
                    .ToList();

                // Obtener el texto ingresado en el buscador.
                string busqueda = txtBuscar.Text.Trim();

                // Buscar por nombre, apellido o matrícula (sin distinguir mayúsculas, minúsculas ni tildes)
                if (!string.IsNullOrEmpty(busqueda))
                {
                    CompareInfo comparador = CultureInfo.GetCultureInfo("es-AR").CompareInfo;

                    lista = lista.Where(m =>
                        comparador.IndexOf(
                            m.NombreCompleto ?? "",
                            busqueda,
                            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0
                        ||
                        comparador.IndexOf(
                            m.Matricula ?? "",
                            busqueda,
                            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0
                    ).ToList();
                }

                // --- Asignar los datos y actualizar la tabla ---
                dgvMedicos.DataSource = lista;
                dgvMedicos.DataBind();

                // Mostrar el mensaje correspondiente cuando no hay resultados.
                if (lista.Count == 0)
                {
                    Label lblSinResultados = dgvMedicos.Controls[0]
                        .Controls[0].FindControl("lblSinResultados") as Label;

                    if (lblSinResultados != null)
                    {
                        if(todos.Count == 0)
                        {
                            lblSinResultados.Text =
                                "No hay médicos registrados en el sistema.";
                        }
                        else if (!string.IsNullOrEmpty(busqueda))
                        {
                            lblSinResultados.Text =
                                "No se encontraron médicos que coincidan con la búsqueda.";
                        }
                        else if (mostrarActivos)
                        {
                            lblSinResultados.Text = "No hay médicos activos.";
                        }
                        else
                        {
                            lblSinResultados.Text = "No hay médicos inactivos.";
                        }
                    }
                }
            }
            catch (Exception)
            {
                // --- Mostrar un mensaje general sin exponer errores técnicos ---
                lblErrorListado.Text =
                    "No se pudo cargar el listado de médicos. Intente nuevamente.";

                lblErrorListado.Visible = true;

                // --- Ocultar la tabla si falla la consulta ---
                dgvMedicos.Visible = false;
            }
        }

        // ============================================================
        // BÚSQUEDA DE MÉDICOS
        // ============================================================
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarMedicos();
            upListado.Update();
        }

        // ============================================================
        // FILTRO POR ESTADO DE MÉDICOS
        // ============================================================
        protected void btnActivos_Click(object sender, EventArgs e)
        {
            // Seleccionar médicos activos.
            hfEstado.Value = "1";

            // Actualizar apariencia y listado.
            actualizarEstiloFiltro();
            cargarMedicos();
            upListado.Update();
        }

        protected void btnInactivos_Click(object sender, EventArgs e)
        {
            // Seleccionar médicos inactivos.
            hfEstado.Value = "0";

            // Actualizar apariencia y listado.
            actualizarEstiloFiltro();
            cargarMedicos();
            upListado.Update();
        }

        // ============================================================
        // APARIENCIA DE LOS FILTROS
        // ============================================================
        private void actualizarEstiloFiltro()
        {
            bool mostrarActivos = hfEstado.Value == "1";

            btnActivos.CssClass = mostrarActivos
                ? "btn btn-filtro-activo"
                : "btn btn-outline-secondary";

            btnInactivos.CssClass = mostrarActivos
                ? "btn btn-outline-secondary"
                : "btn btn-filtro-activo";
        }

        // ============================================================
        // CARGA DE ESPECIALIDADES
        // ============================================================
        private void cargarEspecialidades()
        {
            EspecialidadNegocio negocio = new EspecialidadNegocio();

            try
            {
                // Obtener las especialidades registradas en la base de datos.
                List<Especialidad> lista = negocio.listar();

                // Mostrar únicamente las especialidades activas.
                lista = lista.Where(e => e.Estado).ToList();

                // Configurar el listado de especialidades.
                cblEspecialidades.DataSource = lista;

                // Texto visible para el usuario.
                cblEspecialidades.DataTextField = "Nombre";

                // Identificador que utilizaremos al guardar el médico.
                cblEspecialidades.DataValueField = "Id";

                // Cargar los datos en el control.
                cblEspecialidades.DataBind();
            }
            catch (Exception)
            {
                // Evitar mostrar especialidades incompletas si ocurre un error.
                cblEspecialidades.Items.Clear();
            }
        }

        // ============================================================
        // CARGAR DATOS DEL MÉDICO PARA EDITAR
        // ============================================================
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            // --- Obtener el ID del médico seleccionado ---
            LinkButton boton = (LinkButton)sender;
            int idMedico = int.Parse(boton.CommandArgument);

            try
            {
                // --- Buscar el médico registrado ---
                MedicoNegocio negocio = new MedicoNegocio();
                Medico medico = negocio.listar().Find(m => m.Id == idMedico);

                if (medico == null || !medico.Estado)
                {
                    lblErrorListado.Text = "El médico no existe o está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdMedico.Value = medico.Id.ToString();

                // --- Cargar sus datos en el formulario ---
                txtNombreMedico.Text = medico.Nombre;
                txtApellidoMedico.Text = medico.Apellido;
                txtDniMedico.Text = medico.DNI;
                txtMatriculaMedico.Text = medico.Matricula;
                txtTelefonoMedico.Text = medico.Telefono;
                txtEmailMedico.Text = medico.Email;

                // --- Cargar las especialidades disponibles ---
                cargarEspecialidades();

                // --- Marcar las especialidades asignadas al médico ---
                foreach (ListItem item in cblEspecialidades.Items)
                {
                    int idEspecialidad = int.Parse(item.Value);

                    item.Selected = medico.Especialidades.Exists(
                        especialidad => especialidad.Id == idEspecialidad);
                }

                // --- Preparar el formulario para edición ---
                lblErrorMedico.Visible = false;
                btnGuardarMedico.Text = "Guardar Cambios";

                // --- Actualizar el contenido del formulario ---
                upMedico.Update();

                // --- Abrir el formulario de edición ---
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "abrirEditarMedico",
                    "prepararEdicionMedico();",
                    true
                );
            }
            catch (Exception)
            {
                lblErrorListado.Text =
                    "No se pudieron cargar los datos del médico. Intente nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        // ============================================================
        // ALTA Y MODIFICACIÓN DE MÉDICOS
        // ============================================================
        protected void btnGuardarMedico_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Obtener datos del formulario ---
                Medico medico = new Medico();

                medico.Nombre = txtNombreMedico.Text.Trim();
                medico.Apellido = txtApellidoMedico.Text.Trim();
                medico.DNI = txtDniMedico.Text.Trim();
                medico.Matricula = txtMatriculaMedico.Text.Trim();
                medico.Telefono = txtTelefonoMedico.Text.Trim();
                medico.Email = txtEmailMedico.Text.Trim();

                // --- Obtener las especialidades seleccionadas ---
                List<Especialidad> especialidades = new List<Especialidad>();

                foreach (ListItem item in cblEspecialidades.Items)
                {
                    if (item.Selected)
                    {
                        Especialidad especialidad = new Especialidad();

                        especialidad.Id = int.Parse(item.Value);
                        especialidad.Nombre = item.Text;

                        especialidades.Add(especialidad);
                    }
                }

                medico.Especialidades = especialidades;

                MedicoNegocio negocio = new MedicoNegocio();
                bool esEdicion = !string.IsNullOrEmpty(hfIdMedico.Value);

                // --- Registrar o modificar médico ---
                if (esEdicion)
                {
                    int id;

                    if (!int.TryParse(hfIdMedico.Value, out id) || id <= 0)
                    {
                        throw new ArgumentException("No se pudo identificar al médico seleccionado.");
                    }

                    medico.Id = id;
                    negocio.modificar(medico);
                }
                else
                {
                    medico.Estado = true;
                    negocio.agregar(medico);
                }

                // --- Actualizar listado ---
                cargarMedicos();
                upListado.Update();

                // --- Limpiar formulario ---
                hfIdMedico.Value = "";
                txtNombreMedico.Text = "";
                txtApellidoMedico.Text = "";
                txtDniMedico.Text = "";
                txtMatriculaMedico.Text = "";
                txtTelefonoMedico.Text = "";
                txtEmailMedico.Text = "";
                cblEspecialidades.ClearSelection();
                lblErrorMedico.Text = "";
                lblErrorMedico.Visible = false;

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    upMedico,
                    upMedico.GetType(),
                    "guardarExito",
                    esEdicion
                        ? "mostrarExitoMedico('Médico modificado correctamente.');"
                        : "mostrarExitoMedico('Médico registrado correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar errores de validación ---
                mostrarErrorMedico(ex.Message);
            }
            catch (Exception ex)
            {
                // --- Mostrar el error real para identificar el problema ---
                mostrarErrorMedico(ex.Message);
            }
        }

        // ============================================================
        // MENSAJES DE VALIDACIÓN DEL FORMULARIO
        // ============================================================
        private void mostrarErrorMedico(string mensaje)
        {
            // --- Mostrar el error dentro del formulario ---
            lblErrorMedico.Text = mensaje;
            lblErrorMedico.Visible = true;
        }

        // ============================================================
        // PREPARAR BAJA LÓGICA DEL MÉDICO
        // ============================================================
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            // --- Obtener el ID del médico seleccionado ---
            LinkButton boton = (LinkButton)sender;
            int idMedico = int.Parse(boton.CommandArgument);

            try
            {
                // --- Buscar el médico registrado ---
                MedicoNegocio negocio = new MedicoNegocio();
                Medico medico = negocio.listar().Find(m => m.Id == idMedico);

                if (medico == null || !medico.Estado)
                {
                    lblErrorListado.Text = "El médico no existe o está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                // --- Guardar el ID y mostrar el nombre del médico ---
                hfIdBaja.Value = idMedico.ToString();
                lblNombreBaja.Text = medico.NombreCompleto;

                // --- Consultar los turnos pendientes ---
                int turnosPendientes = negocio.contarTurnosPendientes(idMedico);

                // --- Permitir o bloquear la confirmación ---
                pnlBajaPermitida.Visible = turnosPendientes == 0;
                pnlBajaBloqueada.Visible = turnosPendientes > 0;
                btnConfirmarBaja.Visible = turnosPendientes == 0;

                lblTurnosPendientes.Text = turnosPendientes.ToString();

                // --- Actualizar y abrir el modal ---
                upBajaMedico.Update();

                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "abrirModalBaja",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaMedico')).show();",
                    true
                );
            }

            catch (Exception)
            {
                // --- Mostrar un mensaje si ocurre un error ---
                lblErrorListado.Text =
                    "No se pudieron consultar los datos del médico. Intente nuevamente.";
                lblErrorListado.Visible = true;
            }
        }

        // ============================================================
        // CONFIRMAR BAJA LÓGICA DEL MÉDICO
        // ============================================================
        protected void btnConfirmarBaja_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Obtener el médico seleccionado ---
                int idMedico = int.Parse(hfIdBaja.Value);

                // --- Ejecutar la baja lógica ---
                MedicoNegocio negocio = new MedicoNegocio();
                negocio.desactivar(idMedico);

                // --- Actualizar el listado ---
                cargarMedicos();
                upListado.Update();

                // --- Cerrar el modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "bajaMedicoExitosa",
                    "bootstrap.Modal.getOrCreateInstance(" +
                    "document.getElementById('modalBajaMedico')).hide();" +
                    "mostrarExitoListadoMedico('Médico desactivado correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar el motivo por el que no se pudo desactivar ---
                lblErrorListado.Text = ex.Message;
                lblErrorListado.Visible = true;
            }
            catch (Exception)
            {
                // --- Mostrar un mensaje general si ocurre un error ---
                lblErrorListado.Text =
                    "No se pudo desactivar el médico. Intente nuevamente.";
                lblErrorListado.Visible = true;
            }
        }
        // ============================================================
        // PREPARAR REACTIVACIÓN DEL MÉDICO
        // ============================================================
        protected void btnReactivar_Click(object sender, EventArgs e)
        {
            LinkButton boton = (LinkButton)sender;
            int idMedico = int.Parse(boton.CommandArgument);

            try
            {
                MedicoNegocio negocio = new MedicoNegocio();
                Medico medico = negocio.listar().Find(m => m.Id == idMedico);

                if (medico == null || medico.Estado)
                {
                    lblErrorListado.Text = "El médico no existe o ya está activo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdReactivar.Value = idMedico.ToString();
                lblNombreReactivar.Text = medico.NombreCompleto;

                upReactivarMedico.Update();

                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "abrirModalReactivar",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalReactivarMedico')).show();",
                    true
                );
            }
            catch (Exception)
            {
                lblErrorListado.Text =
                    "No se pudieron consultar los datos del médico. Intente nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        // ============================================================
        // CONFIRMAR REACTIVACIÓN DEL MÉDICO
        // ============================================================
        protected void btnConfirmarReactivar_Click(object sender, EventArgs e)
        {
            try
            {
                int idMedico = int.Parse(hfIdReactivar.Value);

                MedicoNegocio negocio = new MedicoNegocio();
                negocio.reactivar(idMedico);

                // --- Actualizar listado ---
                cargarMedicos();
                upListado.Update();

                // --- Limpiar selección ---
                hfIdReactivar.Value = "";

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "reactivacionMedicoExitosa",
                    "bootstrap.Modal.getOrCreateInstance(" +
                    "document.getElementById('modalReactivarMedico')).hide();" +
                    "mostrarExitoListadoMedico('Médico reactivado correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblErrorListado.Text = ex.Message;
                lblErrorListado.Visible = true;
                upListado.Update();
            }
            catch (Exception)
            {
                lblErrorListado.Text =
                    "No se pudo reactivar el médico. Intente nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }
    }
}

