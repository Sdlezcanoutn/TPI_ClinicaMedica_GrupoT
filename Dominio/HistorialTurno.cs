using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class HistorialTurno
    {
        public int Id { get; set; }
        public Turno Turno { get; set; }
        public DateTime Fecha_Anterior { get; set; }
        public DateTime Hora_Inicio_Anterior { get; set; }
        public DateTime Hora_Fin_Anterior { get; set; }
        public EstadoTurno Estado_Anterior { get; set; }
        public DateTime Fecha_Cambio { get; set; }
    }
}
