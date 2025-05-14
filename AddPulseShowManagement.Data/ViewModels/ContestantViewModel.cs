using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddPulseShowManagement.Data.DataTableModels;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class ContestantViewModel: Contestants
    {

        public string? ShowName { get; set; }
        public string? TeamName { get; set; }
        public string? Gval { get; set; }
        public string? XGraphVal { get; set; }
    }
}
