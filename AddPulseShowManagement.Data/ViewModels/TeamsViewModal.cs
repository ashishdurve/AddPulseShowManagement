using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddPulseShowManagement.Data.DataTableModels;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class TeamsViewModal:Teams
    {
        public List<Contestants> contestants=new List<Contestants>();  
    }
}
