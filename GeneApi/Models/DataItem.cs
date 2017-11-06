using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneApi.Models
{
    public class DataItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
