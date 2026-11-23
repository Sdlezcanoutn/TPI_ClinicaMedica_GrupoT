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
    public partial class Horarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargar();
            }
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

                dgvHorarios.DataSource = horarios;
                dgvHorarios.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
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

            int id = int.Parse(boton.CommandArgument);

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
    }
}