using System;
using System.Net;
using Dominio;

namespace Negocio
{
    public class NotificacionTurno
    {
        // ===== MÉTODOS PÚBLICOS =====

        // --- Enviar confirmación de turno ---
        public void enviarConfirmacion(Turno turno)
        {
            // Generar contenido de confirmación
            string contenido = $@"
                <p>Te confirmamos que tu turno ha sido registrado correctamente.</p>

                {generarDatosTurno(turno)}

                {generarLugarAtencion()}";

            // Enviar correo de confirmación
            enviarNotificacion(
                turno,
                "Confirmación de turno - GSH Clínica Médica",
                "¡Tu turno está confirmado!",
                "#198754",
                contenido);
        }

        // --- Enviar reprogramación de turno ---
        public void enviarReprogramacion(Turno turno, Turno turnoAnterior)
        {
            // Generar contenido con los datos anteriores y actualizados
            string contenido = $@"
                <p>Te informamos que tu turno ha sido reprogramado correctamente.</p>

                <div style='background-color:#fff3cd; padding:15px;
                            border:1px solid #ffe69c; margin-bottom:20px;'>

                    <strong style='color:#997404;'>
                        Fecha y horario anteriores
                    </strong>

                    <p style='margin-bottom:0;'>
                        {turnoAnterior.Fecha:dd/MM/yyyy}<br>
                        {turnoAnterior.Hora_Inicio:HH:mm} a
                        {turnoAnterior.Hora_Fin:HH:mm} hs.
                    </p>
                </div>

                {generarDatosTurno(turno)}

                <p>Recordá asistir en la nueva fecha y horario indicados.</p>

                {generarLugarAtencion()}";

            // Enviar correo de reprogramación
            enviarNotificacion(
                turno,
                "Reprogramación de turno - GSH Clínica Médica",
                "Tu turno ha sido reprogramado",
                "#997404",
                contenido);
        }

        // --- Enviar cancelación de turno ---
        public void enviarCancelacion(Turno turno)
        {
            // Generar contenido con los datos del turno cancelado
            string contenido = $@"
                <p>Te informamos que tu turno ha sido cancelado.</p>

                {generarDatosTurno(turno)}

                <div style='background-color:#f8d7da; padding:15px;
                            border:1px solid #f1aeb5; margin-top:20px;'>

                    <strong style='color:#842029;'>
                        Estado del turno: Cancelado
                    </strong>

                    <p style='margin-bottom:0;'>
                        El turno indicado ya no se encuentra vigente.
                    </p>
                </div>

                <p>
                    Si necesitás una nueva consulta médica,
                    podés solicitar otro turno.
                </p>";

            // Enviar correo de cancelación
            enviarNotificacion(
                turno,
                "Cancelación de turno - GSH Clínica Médica",
                "Tu turno ha sido cancelado",
                "#dc3545",
                contenido);
        }

        // ===== MÉTODOS PRIVADOS =====

        // --- Preparar y enviar notificación ---
        private void enviarNotificacion(Turno turno, string asunto,
            string titulo, string color, string contenido)
        {
            // Obtener y codificar el nombre del paciente
            string nombrePaciente = WebUtility.HtmlEncode(
                turno.Paciente.Nombre + " " + turno.Paciente.Apellido);

            // Generar el cuerpo completo del correo
            string cuerpo = generarPlantilla(
                titulo, color, contenido, nombrePaciente);

            // Preparar el servicio de correo
            EmailService emailService = new EmailService();

            emailService.armarCorreo(
                turno.Paciente.Email,
                asunto,
                cuerpo);

            // Enviar correo electrónico
            emailService.enviarEmail();
        }

        // --- Generar plantilla institucional ---
        private string generarPlantilla(string titulo, string color,
            string contenido, string nombrePaciente)
        {
            // Construir la estructura HTML común de las notificaciones
            return $@"
                <html>
                <body style='margin:0; padding:20px; background-color:#f4f8fa;
                             font-family:Arial,sans-serif; color:#334155;'>

                    <table width='100%' cellpadding='0' cellspacing='0'
                           style='max-width:600px; margin:auto; background-color:#ffffff;
                                  border:1px solid #dee2e6;'>

                        <!-- Encabezado institucional -->
                        <tr>
                            <td align='center' style='background-color:#1d6283; padding:25px;
                                                      text-align:center; color:#ffffff;'>
                                <h1 style='margin:0; font-size:24px;'>
                                    GSH Clínica Médica
                                </h1>
                                <p style='margin:8px 0 0;'>
                                    Gestión de turnos
                                </p>
                            </td>
                        </tr>

                        <!-- Contenido de la notificación -->
                        <tr>
                            <td style='padding:30px;'>
                                <h2 style='color:{color}; margin-top:0;'>
                                    {titulo}
                                </h2>

                                <p>Hola, <strong>{nombrePaciente}</strong>.</p>

                                {contenido}

                                <hr style='border:0; border-top:1px solid #dee2e6;
                                           margin:25px 0;'>

                                <p>
                                    Ante cualquier consulta, comunicate con
                                    nuestra Central de Turnos:
                                    <strong>0810&#8209;333&#8209;2211</strong>.
                                </p>

                                <p>Gracias por confiar en nosotros.</p>
                            </td>
                        </tr>

                        <!-- Pie institucional -->
                        <tr>
                            <td align='center' style='background-color:#1d6283; padding:20px;
                                                      text-align:center; color:#ffffff;
                                                      font-size:12px;'>
                                <p>
                                    Este correo fue generado automáticamente.
                                    Por favor, no respondas a este mensaje.
                                </p>
                                <p>
                                    © {DateTime.Now.Year} GSH Clínica Médica
                                </p>
                            </td>
                        </tr>

                    </table>
                </body>
                </html>";
        }

        // --- Generar datos del turno ---
        private string generarDatosTurno(Turno turno)
        {
            // Construir la tabla con la información del turno
            return $@"
                <table width='100%' cellpadding='10' cellspacing='0'
                       style='background-color:#f4f8fa;
                              border:1px solid #dee2e6;
                              font-size:14px;'>

                    <tr>
                        <td colspan='2' style='color:#1d6283;
                                               font-size:16px;'>
                            <strong>Datos del turno</strong>
                        </td>
                    </tr>

                    <tr>
                        <td><strong>N.º de turno</strong></td>
                        <td>{turno.Id:D3}</td>
                    </tr>

                    <tr>
                        <td><strong>Especialidad</strong></td>
                        <td>{WebUtility.HtmlEncode(turno.Especialidad.Nombre)}</td>
                    </tr>

                    <tr>
                        <td><strong>Profesional</strong></td>
                        <td>{WebUtility.HtmlEncode(turno.Medico.NombreCompleto)}</td>
                    </tr>

                    <tr>
                        <td><strong>Fecha</strong></td>
                        <td>{turno.Fecha:dd/MM/yyyy}</td>
                    </tr>

                    <tr>
                        <td><strong>Horario</strong></td>
                        <td>{turno.Hora_Inicio:HH:mm} a
                            {turno.Hora_Fin:HH:mm} hs.</td>
                    </tr>

                </table>";
        }

        // --- Generar lugar de atención ---
        private string generarLugarAtencion()
        {
            // Incorporar el domicilio institucional
            return @"
                <p style='margin-top:25px;'>
                    <strong>Lugar de atención</strong><br>
                    Av. Hipólito Yrigoyen 2886, General Pacheco, Buenos Aires.
                </p>";
        }
    }
}