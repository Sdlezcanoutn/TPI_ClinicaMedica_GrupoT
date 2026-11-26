using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Pacientes : System.Web.UI.Page
    {
        // Carga inicial y sincronización visual del filtro por estado
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPacientes();
            }

            // Mantiene el botón activo resaltado en cada postback
            ActualizarEstiloFiltro();
        }

        // Obtiene los pacientes, aplica filtro por estado y texto de búsqueda, y actualiza la grilla
        private void CargarPacientes(string filtro = "")
        {
            PacienteNegocio negocio = new PacienteNegocio();

            try
            {
                lblErrorListado.Visible = false;
                dgvPacientes.Visible = true;

                // Filtra según el botón seleccionado (1 = Activos, 0 = Inactivos)
                bool mostrarActivos = hfEstado.Value == "1";
                List<Paciente> todos = negocio.listar();
                List<Paciente> lista = todos.Where(p => p.Estado == mostrarActivos).ToList();

                // Filtra por DNI o Apellido sin distinguir mayúsculas/minúsculas
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    string texto = filtro.Trim().ToLower();
                    lista = lista.FindAll(x =>
                        (!string.IsNullOrEmpty(x.DNI) && x.DNI.ToLower().Contains(texto)) ||
                        (!string.IsNullOrEmpty(x.Apellido) && x.Apellido.ToLower().Contains(texto))
                    );
                }

                dgvPacientes.DataSource = lista;
                dgvPacientes.DataBind();

                // Ajusta el mensaje vacío según el filtro o búsqueda
                if (lista.Count == 0 && dgvPacientes.Controls.Count > 0)
                {
                    Label lblSin = dgvPacientes.Controls[0].Controls[0].FindControl("lblSinResultados") as Label;
                    if (lblSin != null)
                    {
                        if (todos.Count == 0)
                            lblSin.Text = "No hay pacientes registrados en el sistema.";
                        else if (!string.IsNullOrWhiteSpace(filtro))
                            lblSin.Text = "No se encontraron pacientes que coincidan con la búsqueda.";
                        else if (mostrarActivos)
                            lblSin.Text = "No hay pacientes activos.";
                        else
                            lblSin.Text = "No hay pacientes inactivos.";
                    }
                }
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo cargar el listado de pacientes. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                dgvPacientes.Visible = false;
            }
        }

        // Recarga la grilla cuando cambia el texto del buscador
        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarPacientes(txtBuscar.Text);
            upListado.Update();
        }

        // Cambia el filtro a pacientes activos
        protected void btnActivos_Click(object sender, EventArgs e)
        {
            hfEstado.Value = "1";
            ActualizarEstiloFiltro();
            CargarPacientes(txtBuscar.Text);
            upListado.Update();
        }

        // Cambia el filtro a pacientes inactivos
        protected void btnInactivos_Click(object sender, EventArgs e)
        {
            hfEstado.Value = "0";
            ActualizarEstiloFiltro();
            CargarPacientes(txtBuscar.Text);
            upListado.Update();
        }

        // Alterna las clases CSS entre los botones Activos e Inactivos
        private void ActualizarEstiloFiltro()
        {
            bool mostrarActivos = hfEstado.Value == "1";

            btnActivos.CssClass = mostrarActivos
                ? "btn btn-filtro-activo"
                : "btn btn-outline-secondary";

            btnInactivos.CssClass = mostrarActivos
                ? "btn btn-outline-secondary"
                : "btn btn-filtro-activo";
        }

        // Carga los datos del paciente activo y abre el modal en modo edición
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                Paciente paciente = negocio.listar().Find(p => p.Id == idPaciente);

                if (paciente == null || !paciente.Estado)
                {
                    lblErrorListado.Text = "El paciente no existe o está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                // Completa los campos del modal con los datos actuales
                hfIdPaciente.Value = paciente.Id.ToString();
                txtNombrePaciente.Text = paciente.Nombre;
                txtApellidoPaciente.Text = paciente.Apellido;
                txtDniPaciente.Text = paciente.DNI;
                txtFechaNacimientoPaciente.Text = paciente.Fecha_Nacimiento.ToString("yyyy-MM-dd");
                txtTelefonoPaciente.Text = paciente.Telefono;
                txtEmailPaciente.Text = paciente.Email;
                txtDireccionPaciente.Text = paciente.Direccion;

                lblErrorPaciente.Visible = false;
                btnGuardarPaciente.Text = "Guardar Cambios";
                upPaciente.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirEditarPaciente",
                    "prepararEdicionPaciente();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron cargar los datos del paciente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        // Valida y guarda un paciente nuevo o modifica uno existente
        protected void btnGuardarPaciente_Click(object sender, EventArgs e)
        {
            string nombre = txtNombrePaciente.Text.Trim();
            string apellido = txtApellidoPaciente.Text.Trim();
            string dni = txtDniPaciente.Text.Trim();
            string fechaTexto = txtFechaNacimientoPaciente.Text.Trim();
            string telefono = txtTelefonoPaciente.Text.Trim();
            string email = txtEmailPaciente.Text.Trim();
            string direccion = txtDireccionPaciente.Text.Trim();

            // Valida campos obligatorios
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(fechaTexto) ||
                string.IsNullOrWhiteSpace(email))
            {
                MostrarErrorPaciente("Completá todos los campos obligatorios.");
                return;
            }

            // Valida formato de DNI (7 u 8 dígitos)
            if (dni.Length < 7 || dni.Length > 8 || !dni.All(char.IsDigit))
            {
                MostrarErrorPaciente("Ingresá un DNI válido de 7 u 8 dígitos numéricos.");
                return;
            }

            // Valida rango de fecha de nacimiento
            if (!DateTime.TryParse(fechaTexto, out DateTime fechaNacimiento) ||
                fechaNacimiento.Date > DateTime.Today ||
                fechaNacimiento.Year < 1900)
            {
                MostrarErrorPaciente("Ingresá una fecha de nacimiento válida.");
                return;
            }

            // Valida formato básico de correo electrónico
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MostrarErrorPaciente("Ingresá un correo electrónico válido.");
                return;
            }

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                bool esEdicion = int.TryParse(hfIdPaciente.Value, out int idActual) && idActual > 0;

                // Excluye el registro actual cuando se está editando
                if (negocio.existeDni(dni, esEdicion ? (int?)idActual : null))
                {
                    MostrarErrorPaciente("Ya existe un paciente registrado con ese DNI.");
                    return;
                }

                Paciente paciente = new Paciente
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    DNI = dni,
                    Fecha_Nacimiento = fechaNacimiento,
                    Telefono = telefono,
                    Email = email,
                    Direccion = direccion,
                    Estado = true
                };

                if (esEdicion)
                {
                    paciente.Id = idActual;
                    negocio.modificar(paciente);
                }
                else
                {
                    negocio.agregar(paciente);
                }

                // Refresca el listado y limpia el estado del formulario
                CargarPacientes(txtBuscar.Text);
                upListado.Update();

                hfIdPaciente.Value = "";
                lblErrorPaciente.Visible = false;
                upPaciente.Update();

                string mensajeExito = esEdicion
                    ? "Paciente modificado correctamente."
                    : "Paciente registrado correctamente.";

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "guardarPacienteOk",
                    $"mostrarExitoPaciente('{mensajeExito}');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                MostrarErrorPaciente("Ocurrió un error al guardar el paciente. Intentá nuevamente.");
            }
        }

        // Muestra un mensaje de error dentro del modal de alta/edición
        private void MostrarErrorPaciente(string mensaje)
        {
            lblErrorPaciente.Text = mensaje;
            lblErrorPaciente.Visible = true;
            upPaciente.Update();
        }

        // Prepara el modal de confirmación para dar de baja lógica a un paciente
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                Paciente paciente = negocio.listar().Find(p => p.Id == idPaciente);

                if (paciente == null || !paciente.Estado)
                {
                    lblErrorListado.Text = "El paciente no existe o ya está inactivo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                // Verifica si tiene turnos pendientes a partir de hoy (si los paneles existen en la página)
                TurnoNegocio turnoNegocio = new TurnoNegocio();
                int turnosPendientes = turnoNegocio.listar().Count(t =>
                    t.Paciente != null &&
                    t.Paciente.Id == idPaciente &&
                    t.Fecha.Date >= DateTime.Today &&
                    (t.Estado == null ||
                     (!t.Estado.Nombre.Equals("Cancelado", StringComparison.OrdinalIgnoreCase) &&
                      !t.Estado.Nombre.Equals("Atendido", StringComparison.OrdinalIgnoreCase) &&
                      !t.Estado.Nombre.Equals("Ausente", StringComparison.OrdinalIgnoreCase)))
                );

                if (pnlTurnosPendientesPaciente != null && lblAvisoTurnosPendientesPaciente != null)
                {
                    if (turnosPendientes > 0)
                    {
                        pnlTurnosPendientesPaciente.Visible = true;
                        lblAvisoTurnosPendientesPaciente.Text = $"Este paciente tiene {turnosPendientes} turno(s) pendiente(s).";
                    }
                    else
                    {
                        pnlTurnosPendientesPaciente.Visible = false;
                        lblAvisoTurnosPendientesPaciente.Text = "";
                    }
                }

                hfIdBajaPaciente.Value = paciente.Id.ToString();
                lblNombreBajaPaciente.Text = $"{paciente.Nombre} {paciente.Apellido}";
                upBajaPaciente.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirModalBajaPaciente",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaPaciente')).show();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron consultar los datos del paciente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        // Consulta los turnos del paciente (activo o inactivo) y abre el modal de historial
        protected void btnHistorial_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idPaciente))
                return;

            try
            {
                PacienteNegocio pacienteNegocio = new PacienteNegocio();
                Paciente paciente = pacienteNegocio.listar().Find(p => p.Id == idPaciente);

                // Permite ver el historial tanto de pacientes activos como inactivos
                if (paciente == null)
                {
                    lblErrorListado.Text = "No se encontró el paciente seleccionado.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                // Ordena los turnos del más reciente al más antiguo
                TurnoNegocio turnoNegocio = new TurnoNegocio();
                var historial = turnoNegocio.listar()
                    .Where(t => t.Paciente != null && t.Paciente.Id == idPaciente)
                    .OrderByDescending(t => t.Fecha)
                    .ThenByDescending(t => t.Hora_Inicio)
                    .Select(t => new
                    {
                        Fecha = t.Fecha.ToString("dd/MM/yyyy"),
                        Horario = $"{t.Hora_Inicio:HH:mm} - {t.Hora_Fin:HH:mm}",
                        Especialidad = t.Especialidad != null ? t.Especialidad.Nombre : "",
                        Medico = t.Medico != null ? $"{t.Medico.Apellido}, {t.Medico.Nombre}" : "",
                        Estado = t.Estado != null ? t.Estado.Nombre : "",
                        Motivo = t.Motivo_Consulta,
                        Diagnostico = string.IsNullOrWhiteSpace(t.Diagnostico) ? "-" : t.Diagnostico
                    })
                    .ToList();

                // Aclara en el encabezado si el paciente figura inactivo
                string estadoTexto = paciente.Estado ? "" : " (Inactivo)";
                lblSubtituloHistorial.Text = $"{paciente.Apellido}, {paciente.Nombre} — DNI {paciente.DNI}{estadoTexto}";
                dgvHistorialPaciente.DataSource = historial;
                dgvHistorialPaciente.DataBind();
                upHistorialPaciente.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirHistorialPaciente",
                    "abrirModalHistorialPaciente();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo cargar el historial del paciente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        // Ejecuta la baja lógica del paciente y actualiza el listado
        protected void btnConfirmarBajaPaciente_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfIdBajaPaciente.Value, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                negocio.eliminarLogico(idPaciente);

                hfIdBajaPaciente.Value = "";
                CargarPacientes(txtBuscar.Text);
                upListado.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "bajaPacienteOk",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaPaciente')).hide();" +
                    "mostrarAlertaPaciente('Paciente desactivado correctamente.');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo desactivar el paciente. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }


        }

        // Abre el modal de confirmación para reactivar un paciente inactivo
        protected void btnReactivar_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (!int.TryParse(btn.CommandArgument, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                Paciente paciente = negocio.listar().Find(p => p.Id == idPaciente);

                if (paciente == null)
                {
                    lblErrorListado.Text = "El paciente seleccionado no existe.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                if (paciente.Estado)
                {
                    lblErrorListado.Text = "El paciente ya se encuentra activo.";
                    lblErrorListado.Visible = true;
                    upListado.Update();
                    return;
                }

                hfIdReactivarPaciente.Value = paciente.Id.ToString();
                lblNombreReactivarPaciente.Text = $"{paciente.Nombre} {paciente.Apellido}";
                upReactivarPaciente.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "abrirModalReactivarPaciente",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalReactivarPaciente')).show();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudieron consultar los datos del paciente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }

        // Reactiva el paciente, refresca el listado y cierra el modal
        protected void btnConfirmarReactivarPaciente_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfIdReactivarPaciente.Value, out int idPaciente))
                return;

            try
            {
                PacienteNegocio negocio = new PacienteNegocio();
                negocio.reactivar(idPaciente);

                hfIdReactivarPaciente.Value = "";
                CargarPacientes(txtBuscar.Text);
                upListado.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "reactivarPacienteOk",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalReactivarPaciente')).hide();" +
                    "mostrarAlertaPaciente('Paciente reactivado correctamente.');",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListado.Text = "No se pudo reactivar el paciente. Intentá nuevamente.";
                lblErrorListado.Visible = true;
                upListado.Update();
            }
        }
    }
}