using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class PlayerVsCardView
    {
        public int ID { get; set; }
        public int Seq { get; set; }
        public long ContestantID{ get; set; }
        public string Name{ get; set; }
        public string? TeamName { get; set; }
        public string PolarID { get; set; }
        public string ProfileImage { get; set; }
        public int LivePulse { get; set; }
        public int HighestPulse { get; set; }
        public int LowestPulse { get; set; }
        public int AvgPulse { get; set; }
        public string? GraphVal { get; set; }
        public string? XGraphVal { get; set; }
        public int IsAddedInfo { get; set; }
    }
}
