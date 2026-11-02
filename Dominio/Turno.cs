using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Turno
    {
        public int Id { get; set; }
        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Hora_Inicio { get; set; }
        public DateTime Hora_Fin { get; set; }
        public EstadoTurno Estado { get; set; }
        public string Motivo_Consulta { get; set; }
        public string Diagnostico { get; set; }
        public bool Email_Enviado { get; set; }
    }
}
