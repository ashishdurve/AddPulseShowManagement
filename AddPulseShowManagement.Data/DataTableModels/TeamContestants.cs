using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class TeamContestants
    {
        [Key]
        public long TeamContestantID { get; set; }
        public long? TeamID { get; set; }
        public long? ContestantID { get; set; }
        [NotMapped]
        public string? ContestantName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsVotedOut { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
