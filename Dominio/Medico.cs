using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Medico
    {
        // ============================================================
        // DATOS DEL MÉDICO
        // ============================================================
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Matricula { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Estado { get; set; }

        // ============================================================
        // ESPECIALIDADES ASIGNADAS
        // ============================================================

        // --- Un médico puede tener una o varias especialidades ---
        public List<Especialidad> Especialidades { get; set; } = new List<Especialidad>();

        // ============================================================
        // PROPIEDADES PARA EL LISTADO
        // ============================================================

        // --- Mostrar nombre y apellido en una sola columna ---
        public string NombreCompleto
        {
            get { return Nombre + " " + Apellido; }
        }

        // --- Mostrar las especialidades separadas por comas ---
        public string Especialidad
        {
            get { return string.Join(", ", Especialidades.Select(e => e.Nombre)); }
        }
    }
}

