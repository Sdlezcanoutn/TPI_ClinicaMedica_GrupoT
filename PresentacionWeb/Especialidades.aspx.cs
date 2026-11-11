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
    public partial class Especialidades : System.Web.UI.Page
    {
        // ============================================================
        // CARGA INICIAL DE ESPECIALIDADES
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargarEspecialidades();
            }

            // --- Mantener la apariencia del filtro seleccionado ---
            actualizarEstiloFiltro();
        }

        // ============================================================
        // ALTA Y MODIFICACIÓN DE ESPECIALIDADES
        // ============================================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Obtener datos del formulario ---
                Especialidad especialidad = new Especialidad();

                especialidad.Nombre = txtNombre.Text.Trim();
                especialidad.Descripcion = txtDescripcion.Text.Trim();

                EspecialidadNegocio negocio = new EspecialidadNegocio();
                bool esEdicion = !string.IsNullOrEmpty(hfIdEspecialidad.Value);

                // --- Registrar o modificar especialidad ---
                if (esEdicion)
                {
                    int id;

                    if (!int.TryParse(hfIdEspecialidad.Value, out id) || id <= 0)
                    {
                        throw new ArgumentException("La especialidad seleccionada no es válida.");
                    }

                    especialidad.Id = id;
                    negocio.modificar(especialidad);
                }
                else
                {
                    especialidad.Estado = true;
                    negocio.agregar(especialidad);
                }

                // --- Actualizar listado ---
                cargarEspecialidades();
                upListado.Update();

                // --- Limpiar formulario ---
                hfIdEspecialidad.Value = "";
                txtNombre.Text = "";
                txtDescripcion.Text = "";
                lblMensaje.Text = "";
                lblMensaje.Visible = false;

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    upEspecialidad,
                    upEspecialidad.GetType(),
                    "guardarExito",
                    esEdicion
                        ? "mostrarExito('Especialidad modificada correctamente.');"
                        : "mostrarExito('Especialidad registrada correctamente.');",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar errores de validación ---
                lblMensaje.Text = ex.Message;
                lblMensaje.Visible = true;

                // --- Mantener el texto del botón según la operación ---
                btnGuardar.Text = string.IsNullOrEmpty(hfIdEspecialidad.Value)
                    ? "Guardar"
                    : "Guardar cambios";
            }
        }

        // ============================================================
        // BÚSQUEDA DE ESPECIALIDADES
        // ============================================================

        private void cargarEspecialidades()
        {
            // --- Obtener listado de especialidades ---
            EspecialidadNegocio negocio = new EspecialidadNegocio();

            // --- Filtrar especialidades según el estado seleccionado ---
            bool mostrarActivas = hfEstado.Value == "1";

            List<Especialidad> lista = negocio.listar()
                .Where(e => e.Estado == mostrarActivas)
                .ToList();

            string busqueda = txtBuscar.Text.Trim();

            // --- Filtrar por nombre ---
            if (!string.IsNullOrEmpty(busqueda))
            {
                CompareInfo comparador = CultureInfo.GetCultureInfo("es-AR").CompareInfo;

                lista = lista.Where(e =>
                    comparador.IndexOf(
                        e.Nombre,
                        busqueda,
                        CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
                    ) >= 0
                ).ToList();
            }

            // --- Mostrar resultados en la tabla ---
            dgvEspecialidades.DataSource = lista;
            dgvEspecialidades.DataBind();

            // --- Mostrar mensaje según el resultado ---
            if (lista.Count == 0)
            {
                Label lblSinResultados = dgvEspecialidades.Controls[0]
                      .Controls[0].FindControl("lblSinResultados") as Label;

                if (lblSinResultados != null)
                {
                    lblSinResultados.Text = string.IsNullOrEmpty(busqueda)
                    ? (mostrarActivas
                        ? "No hay especialidades activas."
                        : "No hay especialidades inactivas.")
                    : "No se encontraron especialidades que coincidan con la búsqueda.";
                }
            }
        }

        // --- Ejecutar búsqueda ---
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarEspecialidades();
            upListado.Update();
        }

        // ============================================================
        // FILTRO POR ESTADO DE ESPECIALIDADES
        // ============================================================
        protected void btnActivas_Click(object sender, EventArgs e)
        {
            // --- Seleccionar especialidades activas ---
            hfEstado.Value = "1";

            // --- Actualizar apariencia y listado ---
            actualizarEstiloFiltro();
            cargarEspecialidades();
            upListado.Update();
        }

        protected void btnInactivas_Click(object sender, EventArgs e)
        {
            // --- Seleccionar especialidades inactivas ---
            hfEstado.Value = "0";

            // --- Actualizar apariencia y listado ---
            actualizarEstiloFiltro();
            cargarEspecialidades();
            upListado.Update();
        }

        // --- Actualizar apariencia del filtro ---
        private void actualizarEstiloFiltro()
        {
            bool mostrarActivas = hfEstado.Value == "1";

            btnActivas.CssClass = mostrarActivas
                ? "btn btn-filtro-activo"
                : "btn btn-outline-secondary";

            btnInactivas.CssClass = mostrarActivas
                ? "btn btn-outline-secondary"
                : "btn btn-filtro-activo";
        }

        // ============================================================
        // CARGA DE DATOS PARA EDICIÓN DE ESPECIALIDADES
        // ============================================================
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            // --- Obtener especialidad seleccionada ---
            LinkButton boton = (LinkButton)sender;
            int id = int.Parse(boton.CommandArgument);

            EspecialidadNegocio negocio = new EspecialidadNegocio();
            Especialidad especialidad = negocio.listar()
                .FirstOrDefault(esp => esp.Id == id);

            if (especialidad == null)
            {
                return;
            }

            // --- Cargar datos en el formulario ---
            hfIdEspecialidad.Value = especialidad.Id.ToString();
            txtNombre.Text = especialidad.Nombre;
            txtDescripcion.Text = especialidad.Descripcion;

            lblMensaje.Text = "";
            lblMensaje.Visible = false;

            upEspecialidad.Update();

            // --- Abrir modal en modo edición ---
            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                "abrirEdicion",
                "prepararEdicionEspecialidad();",
                true
            );
        }

        // ============================================================
        // PREPARACIÓN DE BAJA DE ESPECIALIDADES
        // ============================================================
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            // --- Obtener especialidad seleccionada ---
            LinkButton boton = (LinkButton)sender;
            int id = int.Parse(boton.CommandArgument);

            EspecialidadNegocio negocio = new EspecialidadNegocio();

            Especialidad especialidad = negocio.listar()
                .FirstOrDefault(esp => esp.Id == id);

            if (especialidad == null || !especialidad.Estado)
            {
                return;
            }

            // --- Consultar turnos pendientes ---
            int turnosPendientes = negocio.contarTurnosPendientes(id);

            // --- Cargar datos en el modal ---
            hfIdBaja.Value = especialidad.Id.ToString();
            lblNombreBaja.Text = especialidad.Nombre;

            lblTurnosPendientes.Text = turnosPendientes == 0
                ? "Esta especialidad no tiene turnos pendientes."
                : "Turnos pendientes: " + turnosPendientes;

            upBajaEspecialidad.Update();

            // --- Abrir modal de confirmación ---
            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                "abrirModalBaja",
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaEspecialidad')).show();",
                true
            );
        }

        // ============================================================
        // CONFIRMACIÓN DE BAJA DE ESPECIALIDADES
        // ============================================================
        protected void btnConfirmarBaja_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Obtener especialidad seleccionada ---
                int id = int.Parse(hfIdBaja.Value);

                // --- Ejecutar baja lógica ---
                EspecialidadNegocio negocio = new EspecialidadNegocio();
                negocio.darDeBaja(id);

                // --- Actualizar listado ---
                cargarEspecialidades();
                upListado.Update();

                // --- Limpiar selección ---
                hfIdBaja.Value = "";

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "bajaExitosa",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalBajaEspecialidad')).hide();" +
                    "var mensaje = document.getElementById('mensajeExito');" +
                    "mensaje.textContent = 'Especialidad dada de baja correctamente.';" +
                    "mensaje.classList.remove('d-none');" +
                    "setTimeout(function() { mensaje.classList.add('d-none'); }, 4000);",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar error ---
                lblTurnosPendientes.Text = ex.Message;
                upBajaEspecialidad.Update();
            }
        }

        // ============================================================
        // PREPARACIÓN DE REACTIVACIÓN DE ESPECIALIDADES
        // ============================================================
        protected void btnReactivar_Click(object sender, EventArgs e)
        {
            // --- Obtener especialidad seleccionada ---
            LinkButton boton = (LinkButton)sender;
            int id = int.Parse(boton.CommandArgument);

            EspecialidadNegocio negocio = new EspecialidadNegocio();

            Especialidad especialidad = negocio.listar()
                .FirstOrDefault(esp => esp.Id == id);

            if (especialidad == null || especialidad.Estado)
            {
                return;
            }

            // --- Cargar datos en el modal ---
            hfIdReactivar.Value = especialidad.Id.ToString();
            lblNombreReactivar.Text = especialidad.Nombre;

            upReactivarEspecialidad.Update();

            // --- Abrir modal de confirmación ---
            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                "abrirModalReactivar",
                "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalReactivarEspecialidad')).show();",
                true
            );
        }

        // ============================================================
        // CONFIRMACIÓN DE REACTIVACIÓN DE ESPECIALIDADES
        // ============================================================
        protected void btnConfirmarReactivar_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Obtener especialidad seleccionada ---
                int id;

                if (!int.TryParse(hfIdReactivar.Value, out id) || id <= 0)
                {
                    throw new ArgumentException("La especialidad seleccionada no es válida.");
                }

                // --- Ejecutar reactivación ---
                EspecialidadNegocio negocio = new EspecialidadNegocio();
                negocio.reactivar(id);

                // --- Actualizar listado ---
                cargarEspecialidades();
                upListado.Update();

                // --- Limpiar selección ---
                hfIdReactivar.Value = "";

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "reactivacionExitosa",
                    "bootstrap.Modal.getOrCreateInstance(document.getElementById('modalReactivarEspecialidad')).hide();" +
                    "var mensaje = document.getElementById('mensajeExito');" +
                    "mensaje.textContent = 'Especialidad reactivada correctamente.';" +
                    "mensaje.classList.remove('d-none');" +
                    "setTimeout(function() { mensaje.classList.add('d-none'); }, 4000);",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar error ---
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "errorReactivacion",
                    "alert('" + HttpUtility.JavaScriptStringEncode(ex.Message) + "');",
                    true
                );
            }
        }

    }
}
