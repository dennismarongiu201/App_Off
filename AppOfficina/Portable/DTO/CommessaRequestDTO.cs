using System;
using System.Collections.Generic;
using System.Text;

namespace AppOfficina.Portable.DTO
{
    public class CommessaRequestDTO
    {
        public string numeroCommessa { get; set; }
        public string numeroTarga { get; set; }
        public string Stato { get; set; }
    }

    public class CommessaClosedDTO
    {
        public string numeroCommessa { get; set; }
        
    }
}
