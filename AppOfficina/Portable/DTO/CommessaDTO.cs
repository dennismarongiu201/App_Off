using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Windows.Documents.Flow.Model;

namespace AppOfficina.Portable.DTO
{
    public class CommessaDTO
    {
        public string NumeroCommessa { get; set; }
        public string NomeCommessa { get; set; }
        public string TargaCommessa { get; set; }
        public string DescrizioneVeicolo { get; set; }
        public string Colore { get; set; }
        public string Telaio { get; set; }

        public string stato { get; set; }
        public string codice{ get; set; }
        public string ragioneSociale{ get; set; }
        public string telefono{ get; set; }
        public string email{ get; set; }
        public string codiceFiscale{ get; set; }


        public System.Drawing.Color ColorStates { get; set; }

        //public Color ColorStates { get; set; }
    }
}
