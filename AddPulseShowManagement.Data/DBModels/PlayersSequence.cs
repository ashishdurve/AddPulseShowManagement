using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddPulseShowManagement.Data.DBModels
{
    public class PlayersSequence
    {
        [Key]
        public long PlayerSequenceID { get; set; }
        public long PlayerVsPlayerID { get; set; }
        public int? Sequence { get; set; }
        public string PlayerIDs { get; set; }
        public bool? IsActive { get; set; }
    }
}
