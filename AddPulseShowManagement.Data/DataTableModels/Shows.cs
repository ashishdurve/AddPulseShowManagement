using System.ComponentModel.DataAnnotations;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class Shows
    {
        [Key]
        public long ShowID { get; set; }
        public string ShowName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public bool? IsActive { get; set; }

    }
}
