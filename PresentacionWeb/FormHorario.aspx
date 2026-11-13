<%@ Page Title="Registrar Horario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FormHorario.aspx.cs" Inherits="PresentacionWeb.FormHorario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 px-lg-5 my-5">
        <div style="max-width: 800px; margin: 0 auto;">
    
            <div class="card border-0 shadow-sm rounded-4 bg-white p-4 p-md-5">
                <div class="mb-4 pb-3 border-bottom">
                    <h2 class="fw-bold text-dark m-0">Nuevo Horario</h2>
                    <p class="text-muted small m-0 mt-1">Complete los datos para registrar el horario de atención médica.</p>
                </div>
    
                <!-- Etiqueta para mostrar los errores de validación -->
                <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger fw-semibold mb-3 d-block"></asp:Label>
    
                <!-- Formulario -->
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Médico</label>
                        <asp:DropDownList ID="ddlMedico" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
    
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Día de la semana</label>
                        <asp:DropDownList ID="ddlDiaSemana" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Lunes" Value="1" />
                            <asp:ListItem Text="Martes" Value="2" />
                            <asp:ListItem Text="Miércoles" Value="3" />
                            <asp:ListItem Text="Jueves" Value="4" />
                            <asp:ListItem Text="Viernes" Value="5" />
                            <asp:ListItem Text="Sábado" Value="6" />
                            <asp:ListItem Text="Domingo" Value="7" />
                        </asp:DropDownList>
                    </div>
    
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Hora de entrada</label>
                        <asp:TextBox ID="txtHoraEntrada" runat="server" CssClass="form-control bg-light" TextMode="Time"></asp:TextBox>
                    </div>
    
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Hora de salida</label>
                        <asp:TextBox ID="txtHoraSalida" runat="server" CssClass="form-control bg-light" TextMode="Time"></asp:TextBox>
                    </div>
    
                    <!-- Botones de Acción -->
                    <div class="col-12 mt-4 pt-3 border-top d-flex justify-content-end gap-2">
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary px-4 rounded-3" CausesValidation="false" OnClick="btnCancelar_Click" />
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Horario" CssClass="btn btn-clinica-primary px-4 rounded-3 shadow-sm" OnClick="btnGuardar_Click" />
                    </div>
                </div>
    
            </div>
        </div>
    </div>
</asp:Content>
