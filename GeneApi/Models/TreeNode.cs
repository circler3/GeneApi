using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneApi.Models
{
    public class TreeNode
    {
        public string name { get; set; }
        public string size { get; set; }
        public List<KeyValuePair<string, float>> kv = new List<KeyValuePair<string, float>>();
        public TreeNode left { get; set; }
        public TreeNode right { get; set; }
    }
}
