using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Medico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Matricula { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Estado { get; set; }

        public List<Especialidad> Especialidades { get; set; } = new List<Especialidad>();
    }
}
