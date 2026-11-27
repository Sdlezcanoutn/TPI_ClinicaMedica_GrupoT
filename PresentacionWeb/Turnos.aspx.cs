using Dominio;
using Negocio;
using PresentacionWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Turnos : PaginaProtegida
    {
        // Propiedades de acceso y seguridad
        private bool EsAdministrador
        {
            get
            {
                return UsuarioActual != null &&
                       UsuarioActual.Perfil != null &&
                       UsuarioActual.Perfil.Nombre == "Administrador";
            }
        }

        private bool EsRecepcionista
        {
            get
            {
                return UsuarioActual != null &&
                       UsuarioActual.Perfil != null &&
                       UsuarioActual.Perfil.Nombre == "Recepcionista";
            }
        }

        private bool EsMedico
        {
            get
            {
                return UsuarioActual != null &&
                       UsuarioActual.Perfil != null &&
                       UsuarioActual.Perfil.Nombre == "Médico";
            }
        }

        protected bool PuedeGestionarTurnos
        {
            get
            {
                return EsAdministrador || EsRecepcionista;
            }
        }

        protected bool PuedeRegistrarAtencion
        {
            get
            {
                return EsAdministrador || EsMedico;
            }
        }

        private bool PuedeCambiarEstado(Turno turno, string nuevoEstado)
        {
            if (!PuedeAccederTurno(turno))
                return false;

            if (nuevoEstado == "Cancelado")
                return PuedeGestionarTurnos;

            if (nuevoEstado == "No asistió")
                return EsAdministrador || EsMedico;

            return false;
        }
        private int? IdMedicoActual
        {
            get
            {
                if (!EsMedico)
                    return null;

                MedicoNegocio negocio = new MedicoNegocio();
                return negocio.obtenerIdPorUsuario(UsuarioActual.Id);
            }
        }

        // ===== IDENTIFICADOR DEL TURNO =====
        private int IdTurno
        {
            get
            {
                return int.TryParse(hfIdTurno.Value, out int id) ? id : 0;
            }
            set
            {
                hfIdTurno.Value = value.ToString();
            }
        }

        #region Eventos de página y controles

        // Se ejecuta al cargar la página; solo en la primera carga (!IsPostBack) inicializa combos, horarios y grilla
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAgregar.Visible = PuedeGestionarTurnos;

            if (!IsPostBack)
            {
                CargarCombos();
                CargarHorariosDisponibles();
                CargarTurnos();
            }
        }

        protected override bool TienePermiso(Usuario usuario)
        {
            return usuario.Perfil != null && 
                (usuario.Perfil.Nombre == "Administrador" || 
                 usuario.Perfil.Nombre == "Recepcionista" || 
                 usuario.Perfil.Nombre == "Médico");
        }

        // --- Ejecutar búsqueda de turnos ---
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            AplicarFiltrosListado();
            upListadoTurnos.Update();
        }

        // Filtra el listado cuando cambia especialidad, médico o fecha en los filtros
        protected void FiltroListado_Changed(object sender, EventArgs e)
        {
            AplicarFiltrosListado();
            upListadoTurnos.Update();
        }

        // Resetea todos los filtros del listado y vuelve a mostrar todos los turnos
        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = string.Empty;
            ddlFiltroEspecialidad.SelectedIndex = 0;
            ddlFiltroMedico.SelectedIndex = 0;
            txtFiltroFecha.Text = string.Empty;

            CargarTurnos();
            upListadoTurnos.Update();
        }

        // --- Actualizar disponibilidad al cambiar el paciente ---
        protected void ddlPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idEspecialidad = int.TryParse(
                ddlEspecialidad.SelectedValue, out int id) ? id : 0;

            CargarHorariosDisponibles();
            CargarSugerenciasEspecialidad(idEspecialidad);

            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoPaciente",
                "mostrarModalTurno();",
                true
            );
        }

        // Al cambiar especialidad en el modal: filtra médicos, actualiza horarios, busca hasta 3 sugerencias y reabre el modal
        protected void ddlEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idEspecialidad = int.TryParse(ddlEspecialidad.SelectedValue, out int id) ? id : 0;

            CargarMedicosPorEspecialidad(idEspecialidad);
            CargarHorariosDisponibles();
            CargarSugerenciasEspecialidad(idEspecialidad);

            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoEspecialidad",
                "mostrarModalTurno();",
                true
            );
        }

        // Recalcula los horarios disponibles de 60 min cuando cambia el médico seleccionado
        protected void ddlMedico_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarHorariosDisponibles();
            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoMedico",
                "mostrarModalTurno();",
                true
            );
        }

        // Recalcula los horarios disponibles de 60 min cuando cambia la fecha elegida
        protected void txtFechaTurno_TextChanged(object sender, EventArgs e)
        {
            CargarHorariosDisponibles();
            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "reabrirTurnoFecha",
                "mostrarModalTurno();",
                true
            );
        }

        // Autocompleta médico, fecha y horario cuando el usuario hace clic en una sugerencia del Repeater
        protected void rptSugerenciasTurno_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "SeleccionarSugerencia")
                return;

            // CommandArgument viene armado como: idMedico|yyyy-MM-dd|horaInicio|horaFin
            string[] partes = (e.CommandArgument?.ToString() ?? string.Empty).Split('|');
            if (partes.Length != 4)
                return;

            string idMedico = partes[0];
            string fechaIso = partes[1];
            string horarioValor = $"{partes[2]}|{partes[3]}";

            if (ddlMedico.Items.FindByValue(idMedico) != null)
            {
                ddlMedico.SelectedValue = idMedico;
            }

            txtFechaTurno.Text = fechaIso;
            CargarHorariosDisponibles();

            if (ddlHorarioDisponible.Items.FindByValue(horarioValor) != null)
            {
                ddlHorarioDisponible.SelectedValue = horarioValor;
            }

            lblErrorTurno.Visible = false;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "seleccionarSugerenciaTurno",
                "mostrarModalTurno();",
                true
            );
        }

        // --- Preparar formulario para un nuevo turno ---
        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validar si el perfil del usuario puede hacer esta acción
            if (!PuedeGestionarTurnos)
            {
                Response.StatusCode = 403;
                return;
            }

            IdTurno = 0;

            // --- Habilitar campos del formulario ---
            ddlPaciente.Enabled = true;
            ddlEspecialidad.Enabled = true;
            ddlMedico.Enabled = true;
            txtMotivoConsulta.ReadOnly = false;

            // --- Restablecer campos ---
            ddlPaciente.SelectedIndex = 0;
            ddlEspecialidad.SelectedIndex = 0;

            CargarMedicosPorEspecialidad(0);

            txtFechaTurno.Text = string.Empty;
            txtMotivoConsulta.Text = string.Empty;

            CargarHorariosDisponibles();
            CargarSugerenciasEspecialidad(0);

            lblErrorTurno.Visible = false;
            btnGuardarTurno.Text = "Guardar Turno";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "nuevoTurno",
                "prepararNuevoTurno();",
                true
            );
        }

        // --- Recuperar turno para reprogramar ---
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            // Validar si el perfil del usuario puede hacer esta acción
            if (!PuedeGestionarTurnos)
            {
                Response.StatusCode = 403;
                return;
            }

            LinkButton boton = (LinkButton)sender;
            int id = int.Parse(boton.CommandArgument);

            TurnoNegocio negocio = new TurnoNegocio();
            Turno turno = negocio.listar().FirstOrDefault(t => t.Id == id);

            if (turno == null)
                return;

            if (turno.Estado.Nombre != "Nuevo" &&
                turno.Estado.Nombre != "Reprogramado")
            {
                lblErrorListadoTurno.Text =
                    "Solo se pueden reprogramar turnos nuevos o reprogramados.";

                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
                return;
            }

            // --- Verificar disponibilidad de las entidades ---
            CargarMedicosPorEspecialidad(turno.Especialidad.Id);

            if (ddlPaciente.Items.FindByValue(turno.Paciente.Id.ToString()) == null ||
                ddlEspecialidad.Items.FindByValue(turno.Especialidad.Id.ToString()) == null ||
                ddlMedico.Items.FindByValue(turno.Medico.Id.ToString()) == null)
            {
                lblErrorListadoTurno.Text =
                    "El paciente, el médico y la especialidad deben estar activos y correctamente asociados.";

                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
                return;
            }

            IdTurno = turno.Id;

            // --- Cargar datos del turno ---
            ddlPaciente.SelectedValue = turno.Paciente.Id.ToString();
            ddlEspecialidad.SelectedValue = turno.Especialidad.Id.ToString();
            ddlMedico.SelectedValue = turno.Medico.Id.ToString();

            txtFechaTurno.Text = turno.Fecha.ToString("yyyy-MM-dd");
            txtMotivoConsulta.Text = turno.Motivo_Consulta;

            // --- Bloquear campos que no se pueden modificar ---
            ddlPaciente.Enabled = false;
            ddlEspecialidad.Enabled = false;
            ddlMedico.Enabled = false;
            txtMotivoConsulta.ReadOnly = true;

            // --- Cargar horarios disponibles ---
            CargarHorariosDisponibles();

            string horarioActual =
                $"{turno.Hora_Inicio:HH:mm}|{turno.Hora_Fin:HH:mm}";

            if (ddlHorarioDisponible.Items.FindByValue(horarioActual) != null)
            {
                ddlHorarioDisponible.SelectedValue = horarioActual;
            }

            // --- Preparar modal de reprogramación ---
            pnlSugerenciasTurno.Visible = false;
            lblErrorTurno.Visible = false;
            lblErrorListadoTurno.Visible = false;
            btnGuardarTurno.Text = "Reprogramar Turno";

            upTurno.Update();
            upListadoTurnos.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "editarTurno",
                "document.getElementById('tituloModalTurno').textContent = 'Reprogramar Turno'; mostrarModalTurno();",
                true
            );
        }

        // ===== ATENCIÓN, CONSULTA Y CIERRE DE TURNOS =====

        // --- Preparar atención del paciente ---
        protected void btnAtender_Click(object sender, EventArgs e)
        {
            PrepararModalAtencion(sender, false);
        }

        // --- Consultar diagnóstico registrado ---
        protected void btnVerDiagnostico_Click(object sender, EventArgs e)
        {
            PrepararModalAtencion(sender, true);
        }

        // --- Preparar modal de atención o consulta ---
        private void PrepararModalAtencion(object sender, bool soloLectura)
        {
            try
            {
                LinkButton boton = (LinkButton)sender;
                int id = int.Parse(boton.CommandArgument);

                TurnoNegocio negocio = new TurnoNegocio();
                Turno turno = negocio.listar()
                    .FirstOrDefault(t => t.Id == id);

                if (turno == null)
                    throw new ArgumentException(
                        "No se encontró el turno seleccionado.");

                if (!PuedeAccederTurno(turno))
                {
                    Response.StatusCode = 403;
                    return;
                }

                if (!soloLectura && !PuedeRegistrarAtencion)
                {
                    Response.StatusCode = 403;
                    return;
                }

                // --- Validar según la acción seleccionada ---
                if (soloLectura)
                {
                    if (turno.Estado.Nombre != "Cerrado")
                    {
                        throw new ArgumentException(
                            "Solo se pueden consultar diagnósticos de turnos cerrados.");
                    }
                }
                else
                {
                    if (turno.Estado.Nombre != "Nuevo" &&
                        turno.Estado.Nombre != "Reprogramado")
                    {
                        throw new ArgumentException(
                            "Solo se pueden atender turnos nuevos o reprogramados.");
                    }

                    if (turno.Fecha.Date.Add(turno.Hora_Inicio.TimeOfDay) > DateTime.Now)
                    {
                        throw new ArgumentException(
                            "No se puede atender un turno antes de que comience.");
                    }
                }

                // --- Conservar identificador del turno ---
                hfIdAtenderTurno.Value = soloLectura
                    ? string.Empty : turno.Id.ToString();

                // --- Cargar datos del turno ---
                lblDatosAtencion.Text = Server.HtmlEncode(
                    $"Turno #{turno.Id} - " +
                    $"{turno.Paciente.Apellido}, {turno.Paciente.Nombre} - " +
                    $"{turno.Especialidad.Nombre}");

                lblHorarioAtencion.Text =
                    $"{turno.Fecha:dd/MM/yyyy} - " +
                    $"{turno.Hora_Inicio:HH:mm} a {turno.Hora_Fin:HH:mm}";

                lblMotivoAtencion.Text =
                    Server.HtmlEncode(turno.Motivo_Consulta);

                // --- Configurar modo del formulario ---
                lblTituloAtencion.Text = soloLectura
                    ? "Diagnóstico del paciente"
                    : "Atención del paciente";

                lblTituloDiagnostico.Text = soloLectura
                    ? "Diagnóstico y observaciones"
                    : "Diagnóstico y observaciones *";

                // --- Alternar entre registro y consulta del diagnóstico ---
                txtDiagnostico.Visible = !soloLectura;
                txtDiagnostico.Text = string.Empty;

                lblDiagnosticoRegistrado.Visible = soloLectura;
                lblDiagnosticoRegistrado.Text = soloLectura
                    ? Server.HtmlEncode(turno.Diagnostico)
                    : string.Empty;

                avisoCierreTurno.Visible = !soloLectura;
                btnCerrarTurno.Visible = !soloLectura;

                btnVolverAtencion.InnerText = soloLectura
                    ? "Cerrar" : "Volver";

                // --- Restablecer mensajes de error ---
                lblErrorAtenderTurno.Visible = false;
                lblErrorListadoTurno.Visible = false;

                upAtenderTurno.Update();
                upListadoTurnos.Update();

                // --- Abrir modal ---
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "mostrarAtenderTurno",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalAtenderTurno')).show();",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblErrorListadoTurno.Text = Server.HtmlEncode(ex.Message);
                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();

                lblErrorListadoTurno.Text =
                    "No se pudo recuperar el turno seleccionado.";

                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
            }
        }

        // --- Guardar diagnóstico y cerrar turno ---
        protected void btnCerrarTurno_Click(object sender, EventArgs e)
        {
            // Validar si el perfil del usuario puede hacer esta acción
            if (!PuedeRegistrarAtencion)
            {
                Response.StatusCode = 403;
                return;
            }

            try
            {
                int id = int.TryParse(hfIdAtenderTurno.Value, out int valor)
                    ? valor : 0;

                // --- Recuperar diagnóstico ---
                string diagnostico = txtDiagnostico.Text.Trim();

                // --- Guardar diagnóstico y cerrar turno ---
                TurnoNegocio negocio = new TurnoNegocio();
                Turno turno = negocio.listar()
                    .FirstOrDefault(t => t.Id == id);

                // Comprobar nuevamente el permiso antes de guardar
                if (!PuedeRegistrarAtencion || !PuedeAccederTurno(turno))
                {
                    Response.StatusCode = 403;
                    return;
                }

                negocio.cerrar(id, diagnostico);

                // --- Restablecer formulario ---
                hfIdAtenderTurno.Value = string.Empty;
                txtDiagnostico.Text = string.Empty;
                lblErrorAtenderTurno.Visible = false;

                // --- Actualizar listado ---
                AplicarFiltrosListado();
                upListadoTurnos.Update();
                upAtenderTurno.Update();

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "cerrarTurnoOk",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalAtenderTurno')).hide();" +
                    "mostrarAlertaTurno('Diagnóstico registrado y turno cerrado correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblErrorAtenderTurno.Text = Server.HtmlEncode(ex.Message);
                lblErrorAtenderTurno.Visible = true;
                upAtenderTurno.Update();
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();

                lblErrorAtenderTurno.Text =
                    "Ocurrió un error al cerrar el turno. Intentá nuevamente.";

                lblErrorAtenderTurno.Visible = true;
                upAtenderTurno.Update();
            }
        }

        // ===== CONSULTA DEL HISTORIAL DE TURNOS =====

        // --- Mostrar historial del turno seleccionado ---
        protected void btnHistorial_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton boton = (LinkButton)sender;
                int id = int.Parse(boton.CommandArgument);

                TurnoNegocio negocio = new TurnoNegocio();

                Turno turno = negocio.listar()
                    .FirstOrDefault(t => t.Id == id);

                if (turno == null)
                    throw new ArgumentException("No se encontró el turno seleccionado.");

                // Validar si el perfil del usuario puede hacer esta acción
                if (!PuedeAccederTurno(turno))
                {
                    Response.StatusCode = 403;
                    return;
                }

                // --- Mostrar datos actuales del turno ---
                lblDatosHistorial.Text = Server.HtmlEncode(
                    $"Turno #{turno.Id} - " +
                    $"{turno.Paciente.Apellido}, {turno.Paciente.Nombre} - " +
                    $"{turno.Especialidad.Nombre}");

                lblFechaAltaHistorial.Text =
                    $"Fecha de alta: {turno.Fecha_Alta:dd/MM/yyyy}";

                lblEstadoActualHistorial.Text = Server.HtmlEncode(
                    $"Fecha actual: {turno.Fecha:dd/MM/yyyy} - " +
                    $"{turno.Hora_Inicio:HH:mm} a {turno.Hora_Fin:HH:mm} - " +
                    $"Estado: {turno.Estado.Nombre}");

                // --- Cargar modificaciones anteriores ---
                dgvHistorialTurno.DataSource = negocio.listarHistorial(id);
                dgvHistorialTurno.DataBind();

                lblErrorListadoTurno.Visible = false;

                upHistorialTurno.Update();
                upListadoTurnos.Update();

                // --- Abrir modal de historial ---
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "mostrarHistorialTurno",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalHistorialTurno')).show();",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblErrorListadoTurno.Text = Server.HtmlEncode(ex.Message);
                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
            }
            catch (Exception)
            {
                lblErrorListadoTurno.Text =
                    "No se pudo consultar el historial del turno.";

                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
            }
        }

        // ===== CONFIRMACIÓN DE CAMBIOS DE ESTADO =====

        // --- Preparar cancelación ---
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            PrepararCambioEstado(sender, "Cancelado");
        }

        // --- Preparar inasistencia ---
        protected void btnInasistencia_Click(object sender, EventArgs e)
        {
            PrepararCambioEstado(sender, "No asistió");
        }

        // --- Preparar modal de confirmación ---
        private void PrepararCambioEstado(object sender, string nuevoEstado)
        {
            try
            {
                LinkButton boton = (LinkButton)sender;
                int id = int.Parse(boton.CommandArgument);

                TurnoNegocio negocio = new TurnoNegocio();
                Turno turno = negocio.listar().FirstOrDefault(t => t.Id == id);

                if (turno == null)
                    throw new ArgumentException("No se encontró el turno seleccionado.");

                // Validamos que el perfil tenga permisos para realizar esta acción
                if (!PuedeCambiarEstado(turno, nuevoEstado))
                {
                    Response.StatusCode = 403;
                    return;
                }

                // --- Validar estado del turno ---
                if (turno.Estado.Nombre != "Nuevo" &&
                    turno.Estado.Nombre != "Reprogramado")
                {
                    throw new ArgumentException(
                        "Solo se pueden modificar turnos nuevos o reprogramados.");
                }

                // --- Validar horario de inasistencia ---
                if (nuevoEstado == "No asistió" &&
                    turno.Fecha.Date.Add(turno.Hora_Fin.TimeOfDay) > DateTime.Now)
                {
                    throw new ArgumentException(
                        "No se puede registrar una inasistencia antes de que finalice el turno.");
                }

                // --- Conservar turno y acción seleccionada ---
                hfIdCambioEstado.Value = turno.Id.ToString();
                ViewState["NuevoEstadoTurno"] = nuevoEstado;

                lblDatosEstado.Text = Server.HtmlEncode(
                    $"{turno.Paciente.Apellido}, {turno.Paciente.Nombre} - " +
                    $"{turno.Fecha:dd/MM/yyyy} - " +
                    $"{turno.Hora_Inicio:HH:mm} a {turno.Hora_Fin:HH:mm}");

                // --- Configurar contenido del modal ---
                if (nuevoEstado == "Cancelado")
                {
                    lblTituloEstado.Text = "Cancelar turno";
                    lblMensajeEstado.Text =
                        "¿Está seguro de que desea cancelar el siguiente turno?";

                    lblAdvertenciaEstado.Text =
                        "El horario quedará disponible para una nueva asignación.";

                    btnConfirmarEstado.Text = "Confirmar cancelación";
                    btnConfirmarEstado.CssClass = "btn btn-danger";
                }
                else
                {
                    lblTituloEstado.Text = "Registrar inasistencia";
                    lblMensajeEstado.Text =
                        "¿Está seguro de que el paciente no asistió al siguiente turno?";

                    lblAdvertenciaEstado.Text =
                        "El turno quedará registrado como No asistió.";

                    btnConfirmarEstado.Text = "Confirmar inasistencia";
                    btnConfirmarEstado.CssClass = "btn btn-secondary";
                }

                lblErrorListadoTurno.Visible = false;

                upConfirmarEstado.Update();
                upListadoTurnos.Update();

                // --- Abrir modal ---
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "mostrarCambioEstado",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalConfirmarEstado')).show();",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblErrorListadoTurno.Text = Server.HtmlEncode(ex.Message);
                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();

                lblErrorListadoTurno.Text =
                    "No se pudo recuperar el turno seleccionado.";

                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();
            }
        }

        // --- Confirmar cambio de estado ---
        protected void btnConfirmarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.TryParse(hfIdCambioEstado.Value, out int valor)
                    ? valor : 0;

                string nuevoEstado = ViewState["NuevoEstadoTurno"] as string;

                TurnoNegocio negocio = new TurnoNegocio();
                Turno turno = negocio.listar()
                    .FirstOrDefault(t => t.Id == id);

                // Validamos que el perfil tenga permisos para realizar esta acción
                if (!PuedeCambiarEstado(turno, nuevoEstado))
                {
                    Response.StatusCode = 403;
                    return;
                }

                string mensaje;

                // --- Ejecutar acción seleccionada ---
                if (nuevoEstado == "Cancelado")
                {
                    // --- Cancelar turno y recuperar sus datos ---
                    Turno turnoCancelado = negocio.cancelar(id);

                    mensaje = $"Turno #{turnoCancelado.Id:D3} cancelado correctamente.";

                    // --- Enviar correo de cancelación ---
                    bool emailEnviado = false;

                    try
                    {
                        NotificacionTurno notificacion = new NotificacionTurno();
                        notificacion.enviarCancelacion(turnoCancelado);

                        emailEnviado = true;
                        mensaje += " Correo de cancelación enviado.";
                    }
                    catch (Exception ex)
                    {
                        Session["error"] = ex.ToString();

                        mensaje += " No se pudo enviar el correo de cancelación.";
                    }

                    // --- Actualizar estado del envío ---
                    try
                    {
                        negocio.actualizarEmailEnviado(turnoCancelado.Id, emailEnviado);
                    }
                    catch (Exception ex)
                    {
                        Session["error"] = ex.ToString();

                        mensaje += " No se pudo actualizar el estado del correo.";
                    }
                }
                else if (nuevoEstado == "No asistió")
                {
                    negocio.registrarInasistencia(id);
                    mensaje = "Inasistencia registrada correctamente.";
                }
                else
                {
                    throw new ArgumentException("La acción seleccionada no es válida.");
                }

                // --- Actualizar listado ---
                hfIdCambioEstado.Value = string.Empty;
                ViewState.Remove("NuevoEstadoTurno");

                AplicarFiltrosListado();
                upListadoTurnos.Update();

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "cambioEstadoOk",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalConfirmarEstado')).hide();" +
                    $"mostrarAlertaTurno('{mensaje}');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                lblErrorListadoTurno.Text = Server.HtmlEncode(ex.Message);
                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "cambioEstadoError",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalConfirmarEstado')).hide();",
                    true
                );
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();

                lblErrorListadoTurno.Text =
                    "Ocurrió un error al modificar el estado del turno. Intentá nuevamente.";

                lblErrorListadoTurno.Visible = true;
                upListadoTurnos.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "cambioEstadoError",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalConfirmarEstado')).hide();",
                    true
                );
            }
        }

        // Valida los datos del modal, registra el nuevo turno en BD, refresca la grilla y muestra aviso de éxito
        protected void btnGuardarTurno_Click(object sender, EventArgs e)
        {
            // Validar si el perfil del usuario puede hacer esta acción
            if (!PuedeGestionarTurnos)
            {
                Response.StatusCode = 403;
                return;
            }

            int idPaciente = int.TryParse(ddlPaciente.SelectedValue, out int pId) ? pId : 0;
            int idEspecialidad = int.TryParse(ddlEspecialidad.SelectedValue, out int eId) ? eId : 0;
            int idMedico = int.TryParse(ddlMedico.SelectedValue, out int mId) ? mId : 0;
            string motivo = txtMotivoConsulta.Text.Trim();

            // --- Validar campos obligatorios ---
            List<string> camposFaltantes = new List<string>();

            if (idPaciente <= 0)
                camposFaltantes.Add("paciente");

            if (idEspecialidad <= 0)
                camposFaltantes.Add("especialidad");

            if (idMedico <= 0)
                camposFaltantes.Add("médico");

            if (string.IsNullOrWhiteSpace(txtFechaTurno.Text))
                camposFaltantes.Add("fecha");

            if (string.IsNullOrWhiteSpace(ddlHorarioDisponible.SelectedValue) ||
                ddlHorarioDisponible.SelectedValue == "0")
                camposFaltantes.Add("horario");

            if (string.IsNullOrWhiteSpace(motivo))
                camposFaltantes.Add("motivo de consulta");

            if (camposFaltantes.Count > 0)
            {
                string mensaje = camposFaltantes.Count == 1
                    ? "Completá el siguiente campo: "
                    : "Completá los siguientes campos: ";

                MostrarErrorTurno(mensaje + string.Join(", ", camposFaltantes) + ".");
                return;
            }

            if (!DateTime.TryParseExact(
                    txtFechaTurno.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaSeleccionada))
            {
                MostrarErrorTurno("Ingresá una fecha válida.");
                return;
            }

            string horarioSeleccionado = ddlHorarioDisponible.SelectedValue;
            if (string.IsNullOrEmpty(horarioSeleccionado) || horarioSeleccionado == "0" || !horarioSeleccionado.Contains("|"))
            {
                MostrarErrorTurno("Seleccioná un horario disponible.");
                return;
            }

            string[] partesHora = horarioSeleccionado.Split('|');
            if (!TimeSpan.TryParse(partesHora[0], out TimeSpan tsInicio) ||
                !TimeSpan.TryParse(partesHora[1], out TimeSpan tsFin))
            {
                MostrarErrorTurno("El horario seleccionado no es válido.");
                return;
            }

            try
            {
                Turno nuevo = new Turno
                {
                    Id = IdTurno,
                    Paciente = new Paciente { Id = idPaciente },
                    Especialidad = new Especialidad { Id = idEspecialidad },
                    Medico = new Medico { Id = idMedico },
                    Fecha = fechaSeleccionada.Date,
                    Hora_Inicio = fechaSeleccionada.Date.Add(tsInicio),
                    Hora_Fin = fechaSeleccionada.Date.Add(tsFin),
                    Motivo_Consulta = motivo
                };

                TurnoNegocio negocio = new TurnoNegocio();
                string mensaje;

                // --- Diferenciar alta de reprogramación ---
                if (IdTurno == 0)
                {
                    negocio.agregar(nuevo);

                    mensaje = $"Turno #{nuevo.Id:D3} registrado correctamente.";

                    // --- Enviar correo de confirmación ---
                    bool emailEnviado = false;

                    try
                    {
                        NotificacionTurno notificacion = new NotificacionTurno();
                        notificacion.enviarConfirmacion(nuevo);

                        emailEnviado = true;
                        mensaje += " Correo de confirmación enviado.";
                    }
                    catch (Exception ex)
                    {
                        Session["error"] = ex.ToString();

                        mensaje += " No se pudo enviar el correo de confirmación.";
                    }

                    // --- Actualizar estado del envío ---
                    try
                    {
                        negocio.actualizarEmailEnviado(nuevo.Id, emailEnviado);
                    }
                    catch (Exception ex)
                    {
                        Session["error"] = ex.ToString();

                        mensaje += " No se pudo actualizar el estado del correo.";
                    }
                }
                else
                {
                    Turno turnoAnterior = negocio.reprogramar(nuevo);

                    // --- Completar los datos del turno reprogramado ---
                    nuevo.Paciente = turnoAnterior.Paciente;
                    nuevo.Medico = turnoAnterior.Medico;
                    nuevo.Especialidad = turnoAnterior.Especialidad;

                    mensaje = $"Turno #{nuevo.Id:D3} reprogramado correctamente.";

                    // --- Enviar correo de reprogramación ---
                    bool emailEnviado = false;

                    try
                    {
                        NotificacionTurno notificacion = new NotificacionTurno();
                        notificacion.enviarReprogramacion(nuevo, turnoAnterior);

                        emailEnviado = true;
                        mensaje += " Correo de reprogramación enviado.";
                    }
                    catch (Exception ex)
                    {
                        Session["error"] = ex.ToString();

                        mensaje += " No se pudo enviar el correo de reprogramación.";
                    }

                    // --- Actualizar estado del envío ---
                    try
                    {
                        negocio.actualizarEmailEnviado(nuevo.Id, emailEnviado);
                    }
                    catch (Exception ex)
                    {
                        Session["error"] = ex.ToString();

                        mensaje += " No se pudo actualizar el estado del correo.";
                    }
                }

                AplicarFiltrosListado();
                upListadoTurnos.Update();

                lblErrorTurno.Visible = false;
                upTurno.Update();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "guardarTurnoOk",
                    $"mostrarExitoTurno('{mensaje}');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                MostrarErrorTurno(ex.Message);
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                MostrarErrorTurno("Ocurrió un error al guardar el turno. Intentá nuevamente.");
            }
        }

        #endregion

        #region Métodos auxiliares privados

        // Carga pacientes activos, especialidades activas y médicos tanto para el modal como para los filtros
        private void CargarCombos()
        {
            PacienteNegocio pacienteNegocio = new PacienteNegocio();
            List<Paciente> pacientesActivos = pacienteNegocio.listar()
                .Where(p => p.Estado)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToList();

            ddlPaciente.DataSource = pacientesActivos.Select(p => new
            {
                p.Id,
                Texto = $"{p.Apellido}, {p.Nombre} (DNI {p.DNI})"
            }).ToList();
            ddlPaciente.DataValueField = "Id";
            ddlPaciente.DataTextField = "Texto";
            ddlPaciente.DataBind();
            ddlPaciente.Items.Insert(0, new ListItem("-- Seleccione un paciente --", "0"));

            EspecialidadNegocio especialidadNegocio = new EspecialidadNegocio();
            List<Especialidad> especialidadesActivas = especialidadNegocio.listar()
                .Where(esp => esp.Estado)
                .OrderBy(esp => esp.Nombre)
                .ToList();

            ddlEspecialidad.DataSource = especialidadesActivas;
            ddlEspecialidad.DataValueField = "Id";
            ddlEspecialidad.DataTextField = "Nombre";
            ddlEspecialidad.DataBind();
            ddlEspecialidad.Items.Insert(0, new ListItem("-- Seleccione una especialidad --", "0"));

            ddlFiltroEspecialidad.DataSource = especialidadesActivas;
            ddlFiltroEspecialidad.DataValueField = "Id";
            ddlFiltroEspecialidad.DataTextField = "Nombre";
            ddlFiltroEspecialidad.DataBind();
            ddlFiltroEspecialidad.Items.Insert(0, new ListItem("Todas las especialidades", "0"));

            MedicoNegocio medicoNegocio = new MedicoNegocio();
            List<Medico> todosMedicosActivos = medicoNegocio.listar()
                .Where(m => m.Estado)
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .ToList();

            ddlFiltroMedico.DataSource = todosMedicosActivos.Select(m => new
            {
                m.Id,
                Texto = $"{m.Apellido}, {m.Nombre}"
            }).ToList();
            ddlFiltroMedico.DataValueField = "Id";
            ddlFiltroMedico.DataTextField = "Texto";
            ddlFiltroMedico.DataBind();
            ddlFiltroMedico.Items.Insert(0, new ListItem("Todos los médicos", "0"));

            CargarMedicosPorEspecialidad(0);
        }

        // Carga en ddlMedico solo los médicos activos (y si idEspecialidad > 0, filtrados por esa especialidad)
        private void CargarMedicosPorEspecialidad(int idEspecialidad)
        {
            MedicoNegocio medicoNegocio = new MedicoNegocio();
            List<Medico> medicosActivos = medicoNegocio.listar()
                .Where(m => m.Estado)
                .ToList();

            if (idEspecialidad > 0)
            {
                medicosActivos = medicosActivos
                    .Where(m => m.Especialidades != null &&
                                m.Especialidades.Any(esp => esp.Id == idEspecialidad))
                    .ToList();
            }

            ddlMedico.DataSource = medicosActivos
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .Select(m => new
                {
                    m.Id,
                    Texto = $"{m.Apellido}, {m.Nombre} (Mat. {m.Matricula})"
                }).ToList();
            ddlMedico.DataValueField = "Id";
            ddlMedico.DataTextField = "Texto";
            ddlMedico.DataBind();
            ddlMedico.Items.Insert(0, new ListItem("-- Seleccione un médico --", "0"));
        }

        // Lista turnos aplicando filtros opcionales de texto, especialidad, médico y fecha, y arma las filas de la grilla
        private void CargarTurnos(string filtro = "", int idEspecialidad = 0, int idMedico = 0, DateTime? fechaFiltro = null)
        {
            TurnoNegocio negocio = new TurnoNegocio();

            try
            {
                lblErrorListadoTurno.Visible = false;
                dgvTurnos.Visible = true;

                // --- Obtener todos los turnos registrados ---
                List<Turno> todos = negocio.listar();
                List<Turno> lista = todos;

                if (EsMedico)
                {
                    int? idMedicoSesion = IdMedicoActual;

                    lista = lista.Where(t =>
                        idMedicoSesion.HasValue &&
                        t.Medico != null &&
                        t.Medico.Id == idMedicoSesion.Value
                    ).ToList();
                }

                // --- Filtrar por texto ---
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    string busqueda = filtro.Trim();

                    CompareInfo comparador = CultureInfo.GetCultureInfo("es-AR").CompareInfo;
                    CompareOptions opciones = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

                    lista = lista.Where(t =>
                        (t.Paciente != null &&
                            (comparador.IndexOf(
                                $"{t.Paciente.Nombre} {t.Paciente.Apellido}",
                                busqueda, opciones) >= 0 ||
                             comparador.IndexOf(
                                $"{t.Paciente.Apellido}, {t.Paciente.Nombre}",
                                busqueda, opciones) >= 0 ||
                             comparador.IndexOf(
                                t.Paciente.DNI ?? "",
                                busqueda, opciones) >= 0))
                        ||
                        (t.Medico != null &&
                            (comparador.IndexOf(
                                $"{t.Medico.Nombre} {t.Medico.Apellido}",
                                busqueda, opciones) >= 0 ||
                             comparador.IndexOf(
                                $"{t.Medico.Apellido}, {t.Medico.Nombre}",
                                busqueda, opciones) >= 0))
                        ||
                        (t.Especialidad != null &&
                            comparador.IndexOf(
                                t.Especialidad.Nombre ?? "",
                                busqueda, opciones) >= 0)
                    ).ToList();
                }

                if (idEspecialidad > 0)
                {
                    lista = lista.FindAll(t => t.Especialidad != null && t.Especialidad.Id == idEspecialidad);
                }

                if (idMedico > 0)
                {
                    lista = lista.FindAll(t => t.Medico != null && t.Medico.Id == idMedico);
                }

                if (fechaFiltro.HasValue)
                {
                    DateTime fechaBuscada = fechaFiltro.Value.Date;
                    lista = lista.FindAll(t => t.Fecha.Date == fechaBuscada);
                }

                List<TurnoFila> filas = lista.Select(t => new TurnoFila
                {
                    Id = t.Id,
                    Fecha = t.Fecha.ToString("dd/MM/yyyy"),
                    Hora = $"{t.Hora_Inicio:HH:mm} - {t.Hora_Fin:HH:mm}",
                    Paciente = t.Paciente != null ? $"{t.Paciente.Apellido}, {t.Paciente.Nombre}" : "",
                    Medico = t.Medico != null ? $"{t.Medico.Apellido}, {t.Medico.Nombre}" : "",
                    Especialidad = t.Especialidad != null ? t.Especialidad.Nombre : "",
                    Estado = t.Estado != null ? t.Estado.Nombre : "Nuevo"
                }).ToList();

                // --- Mostrar resultados en la tabla ---
                dgvTurnos.DataSource = filas;
                dgvTurnos.DataBind();

                // --- Mostrar mensaje según el resultado ---
                if (lista.Count == 0)
                {
                    Label lblSinResultados = dgvTurnos.Controls[0]
                        .Controls[0].FindControl("lblSinResultados") as Label;

                    if (lblSinResultados != null)
                    {
                        bool hayBusqueda = !string.IsNullOrWhiteSpace(filtro);

                        bool hayFiltros = idEspecialidad > 0 ||
                                          idMedico > 0 ||
                                          fechaFiltro.HasValue;

                        if (todos.Count == 0)
                        {
                            lblSinResultados.Text =
                                "No hay turnos registrados en el sistema.";
                        }
                        else if (hayBusqueda && hayFiltros)
                        {
                            lblSinResultados.Text =
                                "No se encontraron turnos que coincidan con la búsqueda y los filtros seleccionados.";
                        }
                        else if (hayBusqueda)
                        {
                            lblSinResultados.Text =
                                "No se encontraron turnos que coincidan con la búsqueda.";
                        }
                        else if (hayFiltros)
                        {
                            lblSinResultados.Text =
                                "No se encontraron turnos que coincidan con los filtros seleccionados.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Session["error"] = ex.ToString();
                lblErrorListadoTurno.Text = "No se pudo cargar el listado de turnos. Intentá nuevamente.";
                lblErrorListadoTurno.Visible = true;
                dgvTurnos.Visible = false;
            }
        }

        private bool PuedeAccederTurno(Turno turno)
        {
            if (turno == null)
                return false;

            if (EsAdministrador || EsRecepcionista)
                return true;

            if (!EsMedico)
                return false;

            int? idMedico = IdMedicoActual;

            return idMedico.HasValue && turno.Medico != null && turno.Medico.Id == idMedico.Value;
        }

        // Lee los valores actuales de los controles de filtro y llama a CargarTurnos
        private void AplicarFiltrosListado()
        {
            int idEspecialidad = int.TryParse(ddlFiltroEspecialidad.SelectedValue, out int espId) ? espId : 0;
            int idMedico = int.TryParse(ddlFiltroMedico.SelectedValue, out int medId) ? medId : 0;

            DateTime? fechaFiltro = null;
            if (!string.IsNullOrWhiteSpace(txtFiltroFecha.Text) &&
                DateTime.TryParseExact(
                    txtFiltroFecha.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaParseada))
            {
                fechaFiltro = fechaParseada.Date;
            }

            CargarTurnos(txtBuscar.Text, idEspecialidad, idMedico, fechaFiltro);
        }

        // Muestra un mensaje de error dentro del modal y lo mantiene abierto
        private void MostrarErrorTurno(string mensaje)
        {
            lblErrorTurno.Text = mensaje;
            lblErrorTurno.Visible = true;
            upTurno.Update();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "errorTurnoModal",
                "mostrarModalTurno();",
                true
            );
        }

        // --- Obtener color según el estado del turno ---
        protected string ObtenerClaseBadgeEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return "badge bg-success";

            switch (estado.Trim().ToLower())
            {
                case "nuevo":
                    return "badge bg-success";

                case "reprogramado":
                    return "badge bg-warning text-dark";

                case "cancelado":
                    return "badge bg-danger";

                case "no asistió":
                case "no asistio":
                    return "badge bg-secondary";

                case "cerrado":
                    return "badge bg-primary";

                default:
                    return "badge bg-secondary";
            }
        }

        // Llena ddlHorarioDisponible con los bloques libres de 60 min para el médico y la fecha elegidos
        private void CargarHorariosDisponibles()
        {
            ddlHorarioDisponible.Items.Clear();

            int idMedico = int.TryParse(ddlMedico.SelectedValue, out int mId) ? mId : 0;
            int idPaciente = int.TryParse(ddlPaciente.SelectedValue, out int pId) ? pId : 0;

            if (idMedico <= 0 || !DateTime.TryParseExact(
                    txtFechaTurno.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaSeleccionada))
            {
                ddlHorarioDisponible.Items.Insert(0, new ListItem("-- Seleccione médico y fecha --", "0"));
                return;
            }

            if (fechaSeleccionada.Date < DateTime.Today)
            {
                ddlHorarioDisponible.Items.Insert(0, new ListItem("-- No se permiten fechas pasadas --", "0"));
                return;
            }

            TurnoNegocio turnoNegocio = new TurnoNegocio();

            var disponibles = turnoNegocio.obtenerHorariosDisponibles(
            idMedico, fechaSeleccionada.Date, idPaciente, IdTurno);

            if (disponibles.Count == 0)
            {
                ddlHorarioDisponible.Items.Insert(0, new ListItem("-- Sin horarios disponibles ese día --", "0"));
                return;
            }

            foreach (var slot in disponibles)
            {
                string texto = slot.Etiqueta;
                string valor = $"{slot.HoraInicio:hh\\:mm}|{slot.HoraFin:hh\\:mm}";
                ddlHorarioDisponible.Items.Add(new ListItem(texto, valor));
            }

            ddlHorarioDisponible.Items.Insert(0, new ListItem("-- Seleccione un horario --", "0"));
        }

        // Busca los primeros 3 bloques libres de 60 min en los próximos 21 días para médicos activos de la especialidad
        private void CargarSugerenciasEspecialidad(int idEspecialidad)
        {
            if (idEspecialidad <= 0)
            {
                pnlSugerenciasTurno.Visible = false;
                rptSugerenciasTurno.DataSource = null;
                rptSugerenciasTurno.DataBind();
                lblSinSugerencias.Visible = false;
                return;
            }

            int idPaciente = int.TryParse(ddlPaciente.SelectedValue, out int pId) ? pId : 0;

            MedicoNegocio medicoNegocio = new MedicoNegocio();
            List<Medico> medicosEspecialidad = medicoNegocio.listar()
                .Where(m => m.Estado &&
                            m.Especialidades != null &&
                            m.Especialidades.Any(esp => esp.Id == idEspecialidad))
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .ToList();

            var sugerencias = new List<SugerenciaTurnoItem>();
            CultureInfo culturaAr = new CultureInfo("es-AR");
            TurnoNegocio turnoNegocio = new TurnoNegocio();

            for (int offset = 0; offset < 21 && sugerencias.Count < 3; offset++)
            {
                DateTime fechaBuscada = DateTime.Today.AddDays(offset);

                foreach (Medico medico in medicosEspecialidad)
                {
                    if (sugerencias.Count >= 3)
                        break;

                    var slots = turnoNegocio.obtenerHorariosDisponibles(medico.Id, fechaBuscada, idPaciente);
                    foreach (var slot in slots)
                    {
                        if (sugerencias.Count >= 3)
                            break;

                        string horarioTxt = slot.Etiqueta;
                        string horarioVal = $"{slot.HoraInicio:hh\\:mm}|{slot.HoraFin:hh\\:mm}";
                        string fechaTexto = culturaAr.TextInfo.ToTitleCase(
                            fechaBuscada.ToString("ddd dd/MM", culturaAr));

                        sugerencias.Add(new SugerenciaTurnoItem
                        {
                            MedicoTexto = $"{medico.Apellido}, {medico.Nombre}",
                            FechaTexto = fechaTexto,
                            HorarioTexto = horarioTxt,
                            Argumento = $"{medico.Id}|{fechaBuscada:yyyy-MM-dd}|{horarioVal}"
                        });
                    }
                }
            }

            pnlSugerenciasTurno.Visible = true;
            rptSugerenciasTurno.DataSource = sugerencias;
            rptSugerenciasTurno.DataBind();
            lblSinSugerencias.Visible = sugerencias.Count == 0;
        }

        #endregion

        #region Clases auxiliares privadas

        // Modelo liviano para bindear cada fila de la grilla dgvTurnos
        private class TurnoFila
        {
            public int Id { get; set; }
            public string Fecha { get; set; }
            public string Hora { get; set; }
            public string Paciente { get; set; }
            public string Medico { get; set; }
            public string Especialidad { get; set; }
            public string Estado { get; set; }
        }

        // Modelo liviano para cada botón de sugerencia rápida en rptSugerenciasTurno
        private class SugerenciaTurnoItem
        {
            public string MedicoTexto { get; set; }
            public string FechaTexto { get; set; }
            public string HorarioTexto { get; set; }
            public string Argumento { get; set; }
        }

        #endregion
    }
}