using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class Teams
    {
        [Key]
        public long TeamID { get; set; }
        public string TeamName { get; set; }
        [NotMapped]
        public string? ShowName { get; set; }

        public long? ShowID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public bool? IsActive { get; set; }

    }
}
