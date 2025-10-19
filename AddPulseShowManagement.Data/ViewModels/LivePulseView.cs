using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class LivePulseView
    {
        public long? PlayerID { get; set; }
        public long? TeamID { get; set; }
        public int? LivePulse { get; set; }
        public int? NormalPulse { get; set; }
        public int IsVoted { get; set; }
        public string Name { get; set; }
    }
}
