using System;
using System.Collections.Generic;
using System.Text;

namespace AppOfficina.Portable
{
    public class DisplayAlertMessage
    {
        public DisplayAlertMessage()
        {
            //this.Cancel = AppResources.OK;
        }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Cancel { get; set; }
        public string Accept { get; set; }

        public Action<bool> OnCompleted { get; set; }
    }
}
