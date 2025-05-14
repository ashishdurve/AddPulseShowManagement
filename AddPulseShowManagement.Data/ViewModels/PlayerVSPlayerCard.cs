using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddPulseShowManagement.Data.DataTableModels;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class PlayerVSPlayerCard
    {
        public string? Player { get; set; }
        public List<Contestants> contestants { get; set; }
    }

    public class PlayerVSPlayerList
    {
        public long TeamID{ get; set; }
        public List<Contestants> contestants { get; set; }
    }

}
