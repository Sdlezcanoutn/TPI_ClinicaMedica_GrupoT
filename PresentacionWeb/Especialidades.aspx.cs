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
        // ALTA DE ESPECIALIDADES
        // ============================================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Obtener datos del formulario ---
                Especialidad nueva = new Especialidad();

                nueva.Nombre = txtNombre.Text.Trim();
                nueva.Descripcion = txtDescripcion.Text.Trim();
                nueva.Estado = true;

                // --- Validar y registrar especialidad ---
                EspecialidadNegocio negocio = new EspecialidadNegocio();
                negocio.agregar(nueva);

                // --- Actualizar listado ---
                dgvEspecialidades.DataSource = negocio.listar();
                dgvEspecialidades.DataBind();

                upListado.Update();

                // --- Limpiar formulario ---
                txtNombre.Text = "";
                txtDescripcion.Text = "";
                lblMensaje.Text = "";
                lblMensaje.Visible = false;

                // --- Cerrar modal y mostrar confirmación ---
                ScriptManager.RegisterStartupScript(
                    upEspecialidad,
                    upEspecialidad.GetType(),
                    "guardarExito",
                    "mostrarExito();",
                    true
                );
            }
            catch (ArgumentException ex)
            {
                // --- Mostrar errores de validación ---
                lblMensaje.Text = ex.Message;
                lblMensaje.Visible = true;
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
    }
}
