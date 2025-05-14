using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class CardDescription
    {
        [Key]
        public int CardID { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
