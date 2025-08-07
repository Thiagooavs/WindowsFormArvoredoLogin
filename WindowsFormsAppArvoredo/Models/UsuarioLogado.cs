using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppArvoredo.Models
{
    public class UsuarioLogado
    {
        public static int ID { get; set; }
        public static string Login { get; set; }
        public static string Nome { get; set; }
        public static int NivelAcesso { get; set; }

        public static bool TemPermissaoAdmin()
        {
            return NivelAcesso >= 3;
        }

        public static bool TemPermissaoGerencial()
        {
            return NivelAcesso >= 2;
        }
    }
}
