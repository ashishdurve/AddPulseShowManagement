using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Data.ViewModels
{
    public class LiveData
    {
        public double plot0 { get; set; }

        [JsonProperty("scale-x")]
        public string scalex { get; set; }
    }

}
