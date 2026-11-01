<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PresentacionWeb._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CARRUSEL DE IMAGENES -->
    <div class="full-bleed-carousel mb-5">
        <div id="heroCarousel" class="carousel slide carousel-fade shadow-sm" data-bs-ride="carousel" data-bs-interval="4000">

            <div class="carousel-indicators">
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="1" aria-label="Slide 2"></button>
                <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="2" aria-label="Slide 3"></button>
            </div>

            <div class="carousel-inner">

                <!-- Slide 1 -->
                <div class="carousel-item active" data-bs-interval="3000">
                    <img src="/Images/slide-1.jpg" class="d-block w-100 carousel-img" alt="Medicina de Vanguardia">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge bg-primary mb-2 px-3 py-2">Excelencia Médica</span>
                        <h2 class="fw-bold text-white fs-3">Medicina del futuro, centrada en vos</h2>
                        <p class="text-light mb-0">Innovación, tecnología y calidez humana al servicio de la salud integral.</p>
                    </div>
                </div>

                <!-- Slide 2 -->
                <div class="carousel-item" data-bs-interval="3000">
                    <img src="/Images/slide-2.jpg" class="d-block w-100 carousel-img" alt="Tecnología y Diagnóstico">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge bg-info text-dark mb-2 px-3 py-2">Infraestructura</span>
                        <h2 class="fw-bold text-white fs-3">Tecnología de punta para diagnóstico preciso</h2>
                        <p class="text-light mb-0">Consultorios y equipamiento de última generación para tu tranquilidad.</p>
                    </div>
                </div>

                <!-- Slide 3 -->
                <div class="carousel-item" data-bs-interval="3000">
                    <img src="/Images/slide-3.jpg" class="d-block w-100 carousel-img" alt="Atención Profesional">
                    <div class="carousel-caption hero-caption-wide">
                        <span class="badge bg-success mb-2 px-3 py-2">Equipo Profesional</span>
                        <h2 class="fw-bold text-white fs-3">Especialistas comprometidos con tu bienestar</h2>
                        <p class="text-light mb-0">Atención integral y multidisciplinaria todos los días del año.</p>
                    </div>
                </div>

            </div>

            <button class="carousel-control-prev" type="button" data-bs-target="#heroCarousel" data-bs-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Anterior</span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-target="#heroCarousel" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Siguiente</span>
            </button>
        </div>
    </div>

    <!-- CONTENEDOR FLUIDO ANCHO PARA EL DASHBOARD -->
    <div class="container-fluid px-4 px-lg-5 mb-5">

        <!-- Banner Bienvenida -->
        <div class="my-5">
            <div class="row">
                <div class="col-xl-9">
                    <h2 class="fw-bold text-dark display-6 mb-2">
                        Te damos la bienvenida a la medicina del futuro, centrada en vos y con el sentido humano de siempre
                    </h2>
                    <div class="divider-line mb-4"></div>
                    <p class="text-muted fs-5 lh-base mb-3">
                        Hace más de 170 años que transformamos la salud con innovación, excelencia y compromiso. Ese legado nos impulsa a seguir siendo pioneros en cuidados y tratamientos de vanguardia, marcando el rumbo de la medicina en la Argentina.
                    </p>
                </div>
            </div>
        </div>

        <!-- Tarjetas de Acceso Directo -->
        <div class="row g-4">
            
            <!-- Turnos -->
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
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
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
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
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
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
            <div class="col-md-6 col-xl-3">
                <div class="card dashboard-glass-card border-0 shadow-sm rounded-3 h-100">
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

    </div>

</asp:Content>