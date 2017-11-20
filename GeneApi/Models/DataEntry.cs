using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneApi.Models
{
    public class DataEntry
    {
        public string user { get; set; }
        public string sample { get; set; }
        public string company { get; set; }
        public string type { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime updateAt { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime　createAt { get; set; }
        public Dictionary<string, int[]> data { get; set; }
    }
}
