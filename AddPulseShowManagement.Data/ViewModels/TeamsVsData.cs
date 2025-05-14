using AddPulseShowManagement.Data.DataTableModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class TeamsVsData
    {
        public List<ContestantsView> contestantsView = new List<ContestantsView>();
        public int highTeamPulse { get; set; }
        public string? highPulseName { get; set; }
        public string? LowPulseName { get; set; }
        public int lowTeamPulse { get; set; }
        public int avgTeamPulse { get; set; }
        public Teams team = new Teams();
    }
}
