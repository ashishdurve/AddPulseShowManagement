using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddPulseShowManagement.Data.DataTableModels;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class ContestantsView: Contestants
    {
        public int LivePulse { get; set; }
        public int HighestPulse { get; set; }
        public int LowestPulse { get; set; }
        public int AvgPulse { get; set; }
        public string GraphVal { get; set; }
        public string XGraphVal { get; set; }
    }
}
