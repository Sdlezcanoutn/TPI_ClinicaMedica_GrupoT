using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre_Usuario { get; set; }
        public string Contraseña_Usuario_Hash { get; set; }
        public string Email { get; set; }
        public Perfil Perfil { get; set; }
        public bool Estado { get; set; }
    }
}
