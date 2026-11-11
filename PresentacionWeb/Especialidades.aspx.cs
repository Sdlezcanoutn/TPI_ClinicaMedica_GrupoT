using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

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
                EspecialidadNegocio negocio = new EspecialidadNegocio();

                dgvEspecialidades.DataSource = negocio.listar();
                dgvEspecialidades.DataBind();
            }
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
            List<Especialidad> lista = negocio.listar();

            string busqueda = txtBuscar.Text.Trim();

            // --- Filtrar por nombre ---
            if (!string.IsNullOrEmpty(busqueda))
            {
                lista = lista.Where(e =>
                    e.Nombre.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0
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
                        ? "No hay especialidades registradas por el momento."
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
    }
}
