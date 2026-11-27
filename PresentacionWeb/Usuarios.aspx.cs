using Dominio;
using Negocio;
using PresentacionWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PresentacionWeb
{
    public partial class Usuarios : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPerfiles();
                CargarMedicos();
                CargarLista();
            }
        }

        private void CargarPerfiles()
        {
            ddlPerfil.DataSource = new PerfilNegocio().listar();
            ddlPerfil.DataTextField = "Nombre";
            ddlPerfil.DataValueField = "Id";
            ddlPerfil.DataBind();
            ddlPerfil.Items.Insert(0, new ListItem("-- Seleccioná un perfil --", "0"));
        }

        private void CargarMedicos()
        {
            ddlMedico.DataSource = new MedicoNegocio().listar()
                .Where(m => m.Estado)
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .Select(m => new { m.Id, Texto = m.Apellido + ", " + m.Nombre + " (" + m.Matricula + ")" })
                .ToList();
            ddlMedico.DataTextField = "Texto";
            ddlMedico.DataValueField = "Id";
            ddlMedico.DataBind();
            ddlMedico.Items.Insert(0, new ListItem("-- Seleccioná un médico --", "0"));
        }

        private void CargarLista()
        {
            var lista = new UsuarioNegocio().listar();
            string busqueda = txtBuscar.Text.Trim();
            if (!string.IsNullOrWhiteSpace(busqueda))
                lista = lista.Where(u =>
                    (u.Nombre_Usuario ?? "").IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (u.Email ?? "").IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            dgvUsuarios.DataSource = lista;
            dgvUsuarios.DataBind();
        }

        private void MostrarMensaje(string mensaje, bool error)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblMensaje.CssClass = error ? "alert alert-danger d-block" : "alert alert-success d-block";
            lblMensaje.Visible = true;
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            hfId.Value = "";
            txtNombre.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtConfirmar.Text = "";
            ddlPerfil.SelectedIndex = 0;
            ddlMedico.SelectedIndex = 0;
            pnlMedico.Visible = false;
            litTitulo.Text = "Nuevo usuario";
            lblMensaje.Visible = false;
            pnlFormulario.Visible = true;
        }

        protected void ddlPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            Perfil perfil = new PerfilNegocio().listar()
                .FirstOrDefault(p => p.Id.ToString() == ddlPerfil.SelectedValue);
            pnlMedico.Visible = perfil != null && perfil.Nombre == "Médico";
            if (!pnlMedico.Visible) ddlMedico.SelectedIndex = 0;
            // Mantener abierto el formulario después del postback.
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int idPerfil, idMedico, idUsuario;
                if (!int.TryParse(ddlPerfil.SelectedValue, out idPerfil) || idPerfil <= 0)
                    throw new ArgumentException("Seleccioná un perfil.");
                int? medico = null;
                Perfil perfil = new PerfilNegocio().listar().FirstOrDefault(p => p.Id == idPerfil);
                if (perfil != null && perfil.Nombre == "Médico")
                {
                    if (!int.TryParse(ddlMedico.SelectedValue, out idMedico) || idMedico <= 0)
                        throw new ArgumentException("Seleccioná un médico.");
                    medico = idMedico;
                }

                Usuario usuario = new Usuario
                {
                    Nombre_Usuario = txtNombre.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Perfil = new Perfil { Id = idPerfil }
                };
                UsuarioNegocio negocio = new UsuarioNegocio();
                string clave = txtPassword.Text;
                bool edicion = int.TryParse(hfId.Value, out idUsuario) && idUsuario > 0;
                if (edicion) usuario.Id = idUsuario;
                if (!edicion && string.IsNullOrWhiteSpace(clave))
                    throw new ArgumentException("Ingresá una contraseña para la nueva cuenta.");
                if (!string.IsNullOrEmpty(clave) && clave != txtConfirmar.Text)
                    throw new ArgumentException("Las contraseñas no coinciden.");
                if (string.IsNullOrEmpty(clave) && !string.IsNullOrEmpty(txtConfirmar.Text))
                    throw new ArgumentException("Ingresá también la nueva contraseña.");

                if (edicion)
                {
                    // Primero actualizar el usuario. Si se restablece la clave,
                    // se hace en una segunda operación independiente.
                    negocio.modificar(usuario, medico);
                    if (!string.IsNullOrEmpty(clave)) negocio.cambiarContraseña(idUsuario, clave);
                }
                else negocio.agregar(usuario, clave, medico);

                pnlFormulario.Visible = false;
                hfId.Value = "";
                CargarLista();
                MostrarMensaje("Usuario guardado correctamente.", false);
            }
            catch (ArgumentException ex)
            {
                pnlFormulario.Visible = true;
                MostrarMensaje(ex.Message, true);
            }
            catch (Exception)
            {
                pnlFormulario.Visible = true;
                MostrarMensaje("No se pudo guardar el usuario. Revisá los datos e intentá nuevamente.", true);
            }
        }

        protected void dgvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0) return;
            UsuarioNegocio negocio = new UsuarioNegocio();
            try
            {
                if (e.CommandName == "EditarUsuario")
                {
                    Usuario usuario = negocio.listar().FirstOrDefault(u => u.Id == id);

                    if (usuario == null) 
                        throw new ArgumentException("No se encontró el usuario.");

                    hfId.Value = id.ToString();
                    txtNombre.Text = usuario.Nombre_Usuario;
                    txtEmail.Text = usuario.Email;
                    ddlPerfil.SelectedValue = usuario.Perfil.Id.ToString();
                    ddlPerfil_SelectedIndexChanged(sender, EventArgs.Empty);
                    int? medico = negocio.obtenerIdMedicoAsociado(id);
                    if (medico.HasValue && ddlMedico.Items.FindByValue(medico.Value.ToString()) != null)
                        ddlMedico.SelectedValue = medico.Value.ToString();
                    txtPassword.Text = "";
                    txtConfirmar.Text = "";
                    litTitulo.Text = "Editar usuario";
                    lblMensaje.Visible = false;
                    pnlFormulario.Visible = true;
                    return;
                }
                if (e.CommandName == "DesactivarUsuario")
                    negocio.desactivar(id, UsuarioActual.Id);
                else if (e.CommandName == "ReactivarUsuario")
                    negocio.reactivar(id);
                else return;
                CargarLista();
                MostrarMensaje("Estado del usuario actualizado correctamente.", false);
            }
            catch (ArgumentException ex) { MostrarMensaje(ex.Message, true); }
            catch (Exception) { MostrarMensaje("No se pudo completar la operación.", true); }
        }

        protected void btnBuscar_Click(object sender, EventArgs e) { CargarLista(); }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
            hfId.Value = "";
            lblMensaje.Visible = false;
        }
    }
}