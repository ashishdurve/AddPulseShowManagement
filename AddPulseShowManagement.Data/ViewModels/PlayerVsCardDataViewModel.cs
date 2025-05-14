using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class PlayerVsCardDataViewModel
    {
        public int Seq { get; set; }
        public List<PlayerVsCardView> cardData = new List<PlayerVsCardView>();
    }
}
