using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class DashboardConfigurations
    {
        [Key]
        public long ConfigID { get; set; }
        public long? ShowID { get; set; }
        public long? UserID { get; set; }
        public int? Card1 { get; set; }
        public int? Card2 { get; set; }
        public int? Card3 { get; set; }
        public bool? IsActive { get; set; }
        public long? TeamCardID { get; set; }
        public long? TeamCardID2 { get; set; }
        public long? TeamCardID3 { get; set; }



        public long? TimeInSeconds { get; set; }
        public int? TimePeriod { get; set; }
        public int? RefreshTimePeriod { get; set; }
        public string? TimePeriodUnit { get; set; }
        public int? Version { get; set; }
    }
}
