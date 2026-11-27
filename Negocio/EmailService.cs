using System;
using System.Net;
using System.Net.Mail;

namespace Negocio
{
    public class EmailService
    {
        // ===== ATRIBUTOS =====
        private MailMessage email;
        private SmtpClient server;

        // ===== CONSTRUCTOR =====
        public EmailService()
        {
            string usuario = Environment.GetEnvironmentVariable("GSH_EMAIL_USER", EnvironmentVariableTarget.User);
            string password = Environment.GetEnvironmentVariable("GSH_EMAIL_PASSWORD", EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("No se configuraron las credenciales del servicio de correo.");

            server = new SmtpClient();
            server.Host = "sandbox.smtp.mailtrap.io";
            server.Port = 2525;
            server.EnableSsl = true;
            server.UseDefaultCredentials = false;
            server.Credentials = new NetworkCredential(usuario, password);
        }

        // ===== MÉTODOS =====

        // --- Armar correo ---
        public void armarCorreo(string emailDestino, string asunto, string cuerpo)
        {
            email = new MailMessage();
            email.From = new MailAddress("gshclinica@gmail.com", "GSH Clínica Médica");
            email.To.Add(emailDestino);
            email.Subject = asunto;
            email.IsBodyHtml = true;
            email.Body = cuerpo;
        }

        // --- Enviar correo ---
        public void enviarEmail()
        {
            try
            {
                server.Send(email);
            }
            finally
            {
                email?.Dispose();
                server.Dispose();
            }
        }
    }
}