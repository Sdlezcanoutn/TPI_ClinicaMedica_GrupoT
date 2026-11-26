using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Helpers
{
    public static class PasswordHelper
    {
        private const int Iteraciones = 600000;
        private const int TamanoSal = 16;
        private const int TamanoHash = 32;

        public static string GenerarHash(string contraseña)
        {
            if (string.IsNullOrWhiteSpace(contraseña))
                throw new ArgumentException("La contraseña es obligatoria.");

            byte[] sal = new byte[TamanoSal];

            using (RandomNumberGenerator generador = RandomNumberGenerator.Create())
            {
                generador.GetBytes(sal);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                contraseña, sal, Iteraciones, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(TamanoHash);

                return "PBKDF2-SHA256$" +
                       Iteraciones + "$" +
                       Convert.ToBase64String(sal) + "$" +
                       Convert.ToBase64String(hash);
            }
        }

        public static bool Verificar(string contraseña, string hashGuardado)
        {
            if (string.IsNullOrEmpty(contraseña) ||
                string.IsNullOrEmpty(hashGuardado))
                return false;

            try
            {
                string[] partes = hashGuardado.Split('$');

                if (partes.Length != 4 || partes[0] != "PBKDF2-SHA256")
                    return false;

                int iteraciones;

                if (!int.TryParse(partes[1], out iteraciones) ||
                    iteraciones < 100000 || iteraciones > 2000000)
                    return false;

                byte[] sal = Convert.FromBase64String(partes[2]);
                byte[] hashEsperado = Convert.FromBase64String(partes[3]);

                if (sal.Length != TamanoSal ||
                    hashEsperado.Length != TamanoHash)
                    return false;

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    contraseña, sal, iteraciones, HashAlgorithmName.SHA256))
                {
                    byte[] hashCalculado = pbkdf2.GetBytes(hashEsperado.Length);

                    int diferencia = 0;

                    for (int i = 0; i < hashEsperado.Length; i++)
                        diferencia |= hashEsperado[i] ^ hashCalculado[i];

                    return diferencia == 0;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
