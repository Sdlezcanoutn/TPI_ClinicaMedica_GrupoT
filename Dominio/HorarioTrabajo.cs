using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class HorarioTrabajo
    {
        public int Id { get; set; }
        public Medico Medico { get; set; }
        public string Dia_Semana { get; set; }
        public DateTime Hora_Entrada { get; set; }
        public DateTime Hora_Salida { get; set; }
        public bool Estado { get; set; }

    }
}
