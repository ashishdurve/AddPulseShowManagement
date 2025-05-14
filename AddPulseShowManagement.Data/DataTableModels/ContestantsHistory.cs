using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class ContestantsHistory
    {
        [Key]
        public long ContestantHistoryID { get; set; }
        public long? ContestantID { get; set; }
        public string DocumentID { get; set; }
        public long? PlayerID { get; set; }
        public string PolarID { get; set; }
        public int? LivePulse { get; set; }
        public int? AveragePulse { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
