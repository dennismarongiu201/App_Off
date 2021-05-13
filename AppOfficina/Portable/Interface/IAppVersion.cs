using System;
using System.Collections.Generic;
using System.Text;

namespace AppOfficina.Portable.Interface
{
    public interface IAppVersion
    {
        string GetVersion();
        int GetBuild();
    }
}
