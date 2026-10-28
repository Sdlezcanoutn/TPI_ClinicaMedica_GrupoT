<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PresentacionWeb._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Banner Bienvenida -->
    <div class="card border-0 shadow-sm rounded-3 bg-white p-4 my-4">
        <div class="row align-items-center">
            <div class="col-md-8">
                <h2 class="fw-bold text-dark m-0">Bienvenido al Sistema de Gestión Médica</h2>
                <p class="text-muted mt-2 mb-0">Seleccioná un módulo desde el menú superior o utilizá los accesos directos a continuación para gestionar el sistema.</p>
            </div>
            <div class="col-md-4 text-md-end mt-3 mt-md-0">
                <span class="badge bg-light text-dark border p-2 fw-normal">
                    <i class="fa-regular fa-clock me-1"></i> Panel General
                </span>
            </div>
        </div>
    </div>

    <!-- Tarjetas de Acceso Directo (Dashboard) -->
    <div class="row g-4 mb-4">
        
        <!-- Turnos -->
        <div class="col-md-6 col-lg-3">
            <div class="card border-0 shadow-sm rounded-3 h-100 bg-white">
                <div class="card-body p-4 d-flex flex-column justify-content-between">
                    <div>
                        <div class="d-flex align-items-center justify-content-between mb-3">
                            <span class="p-3 bg-primary-subtle text-primary rounded-3">
                                <i class="fa-solid fa-calendar-check fs-4"></i>
                            </span>
                            <span class="text-muted small">Agendamiento</span>
                        </div>
                        <h5 class="fw-bold text-dark">Turnos</h5>
                        <p class="text-muted small mb-0">Administración de citas, horarios y atención a pacientes.</p>
                    </div>
                    <div class="mt-4 pt-2 border-top">
                        <a href="Turnos.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Turnos →</a>
                    </div>
                </div>
            </div>
        </div>

        <!-- Pacientes -->
        <div class="col-md-6 col-lg-3">
            <div class="card border-0 shadow-sm rounded-3 h-100 bg-white">
                <div class="card-body p-4 d-flex flex-column justify-content-between">
                    <div>
                        <div class="d-flex align-items-center justify-content-between mb-3">
                            <span class="p-3 bg-success-subtle text-success rounded-3">
                                <i class="fa-solid fa-user-group fs-4"></i>
                            </span>
                            <span class="text-muted small">Registros</span>
                        </div>
                        <h5 class="fw-bold text-dark">Pacientes</h5>
                        <p class="text-muted small mb-0">Gestión de datos personales e historias clínicas.</p>
                    </div>
                    <div class="mt-4 pt-2 border-top">
                        <a href="Pacientes.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Pacientes →</a>
                    </div>
                </div>
            </div>
        </div>

        <!-- Médicos -->
        <div class="col-md-6 col-lg-3">
            <div class="card border-0 shadow-sm rounded-3 h-100 bg-white">
                <div class="card-body p-4 d-flex flex-column justify-content-between">
                    <div>
                        <div class="d-flex align-items-center justify-content-between mb-3">
                            <span class="p-3 bg-info-subtle text-info-emphasis rounded-3">
                                <i class="fa-solid fa-user-doctor fs-4"></i>
                            </span>
                            <span class="text-muted small">Personal</span>
                        </div>
                        <h5 class="fw-bold text-dark">Médicos</h5>
                        <p class="text-muted small mb-0">Control del cuerpo médico, matrículas y estados.</p>
                    </div>
                    <div class="mt-4 pt-2 border-top">
                        <a href="Medicos.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Médicos →</a>
                    </div>
                </div>
            </div>
        </div>

        <!-- Especialidades -->
        <div class="col-md-6 col-lg-3">
            <div class="card border-0 shadow-sm rounded-3 h-100 bg-white">
                <div class="card-body p-4 d-flex flex-column justify-content-between">
                    <div>
                        <div class="d-flex align-items-center justify-content-between mb-3">
                            <span class="p-3 bg-warning-subtle text-warning-emphasis rounded-3">
                                <i class="fa-solid fa-stethoscope fs-4"></i>
                            </span>
                            <span class="text-muted small">Catálogo</span>
                        </div>
                        <h5 class="fw-bold text-dark">Especialidades</h5>
                        <p class="text-muted small mb-0">Configuración de áreas médicas de la clínica.</p>
                    </div>
                    <div class="mt-4 pt-2 border-top">
                        <a href="Especialidades.aspx" class="btn btn-sm btn-outline-secondary w-100">Ir a Especialidades →</a>
                    </div>
                </div>
            </div>
        </div>

    </div>

</asp:Content>