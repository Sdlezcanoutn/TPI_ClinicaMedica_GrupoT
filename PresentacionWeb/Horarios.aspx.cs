using Dominio;
using Negocio;
using PresentacionWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Horarios : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargar();
            }
        }

        protected override bool TienePermiso(Usuario usuario)
        {
            return usuario.Perfil != null &&
                (usuario.Perfil.Nombre == "Administrador" ||
                 usuario.Perfil.Nombre == "Recepcionista");
        }

        private void cargar()
        {
            HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();

            try
            {
                List<HorarioTrabajo> horarios = negocio.listar();

                // Filtro por nombre o apellido del médico
                string medico = txtBuscarMedico.Text.Trim();

                if (!string.IsNullOrEmpty(medico))
                {
                    CompareInfo comparador = CultureInfo.GetCultureInfo("es-AR").CompareInfo;

                    horarios = horarios.FindAll(h => 
                        comparador.IndexOf(h.Medico.Nombre + " " + h.Medico.Apellido, medico,
                        CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0
                        );
                }

                //Filtro por día de la semana
                string dia = ddlDiaSemana.SelectedValue;

                if (!string.IsNullOrEmpty(dia))
                {
                    horarios = horarios.FindAll(h => h.Dia_Semana == dia);
                }

                // Filtro por estado del horario
                string estado = ddlEstado.SelectedValue;

                if (estado == "1")
                {
                    horarios = horarios.FindAll(h => h.Estado);
                }
                else if (estado == "0")
                {
                    horarios = horarios.FindAll(h => !h.Estado);
                }

                dgvHorarios.DataSource = horarios;
                dgvHorarios.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        /* ============================================================
           EVENTOS DE FILTROS
           ============================================================ */
        protected void txtBuscarMedico_TextChanged(object sender, EventArgs e)
        {
            cargar();
        }
        protected void ddlDiaSemana_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargar();
        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            cargar();
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FormHorario.aspx");
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            LinkButton boton = (LinkButton)sender;

            int id;


            if (boton == null ||
                !int.TryParse(boton.CommandArgument, out id) ||
                id <= 0)
            {
                // Mostrar un mensaje o registrar el error.
                System.Diagnostics.Debug.WriteLine("No se pudo editar: ID de horario inválido.");
                return;
            }

            Response.Redirect("~/FormHorario.aspx?id=" + id);
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            LinkButton boton = (LinkButton)sender;

            int id = int.Parse(boton.CommandArgument);

            try
            {
                HorarioTrabajoNegocio negocio = new HorarioTrabajoNegocio();
                negocio.eliminar(id);

                cargar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        protected void ddlEstado_Filtros_Changed(object sender, EventArgs e)
        {
            cargar();
        }
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarMedico.Text = "";
            ddlDiaSemana.SelectedIndex = 0;
            ddlEstado.SelectedValue = "1";

            cargar();
        }
    }
}