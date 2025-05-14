using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class Contestants
    {
        [Key]
        public long ContestantID { get; set; }
        public string PlayerID { get; set; }
        public string Name { get; set; }
        public long? ShowID { get; set; }
        public string PolarID { get; set; }
        public string? ProfileImage { get; set; }
        public int? AveragePulse { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? TimeStamp { get; set; }
        [NotMapped]
        public bool? Added { get; set; }
        [NotMapped]
        public bool? IsVotedOut { get; set; }
        [NotMapped]
        public long? TeamContestantID { get; set; }
        [NotMapped]
        public int? LivePulse { get; set; }
    }
}
