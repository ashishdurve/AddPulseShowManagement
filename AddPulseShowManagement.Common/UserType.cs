using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Common
{
    public enum UserType
    {
        [Description("Admin")]
        Admin =1,
        [Description("Manager")]
        Managaer
    }
}
