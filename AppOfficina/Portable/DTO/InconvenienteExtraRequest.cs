using System;
using System.Collections.Generic;
using System.Text;

namespace AppOfficina.Portable.DTO
{
    public class InconvenienteExtraRequest
    {
        public string NumeroCommessa { get; set; }
        public string Titolo{ get; set; }
        public string Descrizione{ get; set; }

        public List<string> Base64Foto { get; set; }
    }
}
