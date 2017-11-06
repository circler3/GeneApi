using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneApi.Models
{
    public class DataEntry
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<int[]> GenesList{ get; set; }
        public Dictionary<string, List<int[]>> SourceDictionary { get; set; }
        public Dictionary<string, float> ResultDictionary { get; set; }
        public string Applier { get; set; }

        public DataEntry()
        {
            SourceDictionary = new Dictionary<string, List<int[]>>();
            ResultDictionary = new Dictionary<string, float>();
            GenesList = new List<int[]>();
        }
    }
}
