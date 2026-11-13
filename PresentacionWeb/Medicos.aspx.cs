using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using Negocio;

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

                // --- Obtener el listado de médicos desde la base de datos ---
                List<Medico> lista = negocio.listar();

                // --- Asignar los datos y actualizar la tabla ---
                dgvMedicos.DataSource = lista;
                dgvMedicos.DataBind();
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
        // PREPARAR FORMULARIO PARA NUEVO MÉDICO
        // ============================================================
        protected void btnNuevoMedico_Click(object sender, EventArgs e)
        {
            // --- Limpiar el identificador del médico ---
            hfIdMedico.Value = "";

            // --- Limpiar los datos del formulario ---
            txtNombreMedico.Text = "";
            txtApellidoMedico.Text = "";
            txtDniMedico.Text = "";
            txtMatriculaMedico.Text = "";
            txtTelefonoMedico.Text = "";
            txtEmailMedico.Text = "";

            // --- Restablecer las especialidades ---
            cargarEspecialidades();
            cblEspecialidades.ClearSelection();

            // --- Preparar el formulario para el alta ---
            lblErrorMedico.Visible = false;
            btnGuardarMedico.Text = "Guardar Médico";

            upMedico.Update();

            // --- Abrir el formulario ---
            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                "abrirNuevoMedico",
                "document.getElementById('tituloModalMedico').textContent = 'Nuevo Médico';" +
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalMedico')).show();",
                true
            );
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
            }
        }

        // ============================================================
        // ALTA Y MODIFICACIÓN DE MÉDICOS
        // ============================================================
        protected void btnGuardarMedico_Click(object sender, EventArgs e)
        {
            // --- Obtener los datos ingresados en el formulario ---
            string nombre = txtNombreMedico.Text.Trim();
            string apellido = txtApellidoMedico.Text.Trim();
            string dni = txtDniMedico.Text.Trim();
            string matricula = txtMatriculaMedico.Text.Trim();
            string telefono = txtTelefonoMedico.Text.Trim();
            string email = txtEmailMedico.Text.Trim();

            // ========================================================
            // 1. VALIDAR LOS CAMPOS OBLIGATORIOS
            // ========================================================

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(matricula) ||
                string.IsNullOrWhiteSpace(email))
            {
                mostrarErrorMedico("Completá todos los campos obligatorios.");
                return;
            }

            // --- Validar que el DNI contenga únicamente números ---
            if (dni.Length < 7 || dni.Length > 8 || !dni.All(char.IsDigit))
            {
                mostrarErrorMedico("Ingresá un DNI válido de 7 u 8 dígitos.");
                return;
            }

            // --- Validar el formato del correo electrónico ---
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                mostrarErrorMedico("Ingresá un correo electrónico válido.");
                return;
            }

            // ========================================================
            // 2. OBTENER LAS ESPECIALIDADES SELECCIONADAS
            // ========================================================

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

            // --- Exigir al menos una especialidad ---
            if (especialidades.Count == 0)
            {
                mostrarErrorMedico("Seleccioná al menos una especialidad.");
                return;
            }

            // ========================================================
            // 3. PREPARAR EL OBJETO MÉDICO
            // ========================================================

            Medico medico = new Medico();

            medico.Nombre = nombre;
            medico.Apellido = apellido;
            medico.DNI = dni;
            medico.Matricula = matricula;
            medico.Telefono = telefono;
            medico.Email = email;
            medico.Estado = true;
            medico.Especialidades = especialidades;

            // ========================================================
            // 4. REGISTRAR O MODIFICAR EL MÉDICO EN LA BASE DE DATOS
            // ========================================================

            try
            {
                // --- Crear instancia de la capa de negocio ---
                MedicoNegocio negocio = new MedicoNegocio();

                bool esEdicion = !string.IsNullOrEmpty(hfIdMedico.Value);

                if (esEdicion)
                {
                    // --- Modificar el médico seleccionado ---
                    medico.Id = int.Parse(hfIdMedico.Value);
                    negocio.modificar(medico);
                }
                else
                {
                    // --- Registrar un médico nuevo ---
                    negocio.agregar(medico);
                }

                // --- Limpiar el identificador después de guardar ---
                hfIdMedico.Value = "";

                // --- Actualizar el listado de médicos ---
                cargarMedicos();
                upListado.Update();

                // --- Limpiar el formulario después de guardar ---
                txtNombreMedico.Text = "";
                txtApellidoMedico.Text = "";
                txtDniMedico.Text = "";
                txtMatriculaMedico.Text = "";
                txtTelefonoMedico.Text = "";
                txtEmailMedico.Text = "";
                cblEspecialidades.ClearSelection();
                lblErrorMedico.Visible = false;

                // --- Cerrar el modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    upMedico,
                    upMedico.GetType(),
                    "guardarExito",
                    esEdicion ? "mostrarExitoMedico('Médico modificado correctamente.');"
                              : "mostrarExitoMedico('Médico registrado correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar errores de validación específicos ---
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

    }
}

