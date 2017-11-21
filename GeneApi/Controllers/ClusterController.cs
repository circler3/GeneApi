using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

namespace GeneApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ClusterController : Controller
    {
        // GET: api/Cluster
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        // GET: api/Cluster/5
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var client = new MongoClient("mongodb://222.31.160.146:27017");
            var database = client.GetDatabase("gene");
            var collection = database.GetCollection<BsonDocument>("libraries");
            var tempcollection = database.GetCollection<BsonDocument>("jujubenews");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse("5a13f6d78f3bcd208fdeb302"));
            dynamic target = tempcollection.Find(filter).First();
            var type = target["type"];
            filter = Builders<BsonDocument>.Filter.Eq("type", type);

            var sources = collection.Find(filter).ToList();
            Dictionary<string, float> dictionary = new Dictionary<string, float>();
            foreach (var n in sources)
            {
                var match = 0;
                var count = target["data"].Count;
                var tar = target["data"].ToList();
                for (int i = 0; i < count; i++)
                {
                    if( tar[i][1] == tar[i][2])
                    {
                        if (tar[i][1] == n["data"][i][1])
                        {
                            match++;
                        }
                        if (tar[i][1] == n["data"][i][2])
                        {
                            match++;
                        }
                    }
                    else
                    {
                        if (tar[i][1] == n["data"][i][1] || tar[i][1] == n["data"][i][2])
                        {
                            match++;
                        }
                        if (tar[i][2] == n["data"][i][1] || tar[i][2] == n["data"][i][2])
                        {
                            match++;
                        }
                    }
                }
                dictionary[n["sample"].AsString] = match / ((float)count * 2);
            }

            var list = dictionary.ToList();
            var node = new Models.TreeNode() { kv = list };
            Sort(node);
            
            return new JsonResult(BuildTree(node));
        }

        private JObject BuildTree(Models.TreeNode node)
        {
            JObject bson = new JObject();

            if (node.left != null)
            {
                bson.Add("name", "");

                var resu = new JArray();
                resu.Add(BuildTree(node.left));
                resu.Add(BuildTree(node.right));
                bson.Add("children", resu);
            }
            else
            {
                bson.Add("name", node.kv[0].Key);
                bson.Add("size", node.kv[0].Value);
            }
            return bson;
        }

        private void Sort(Models.TreeNode node)
        {
            if (node.kv.Count <= 1) return;
            var mean = node.kv.Average(x => x.Value);
            Models.TreeNode left = new Models.TreeNode();
            var right = new Models.TreeNode();
            if (node.kv.Count == 2 && node.kv[0].Value == node.kv[1].Value)
            {
                left.kv.Add(node.kv[0]);
                right.kv.Add(node.kv[1]);
            }
            else
            {
                foreach (var n in node.kv)
                {
                    if (n.Value < mean)
                    {
                        left.kv.Add(n);
                    }
                    else
                    {
                        right.kv.Add(n);
                    }
                }
            }
            if (left.kv.Count != 0 )
            {
                node.left = left;
                Sort(node.left);
            }
            if (right.kv.Count != 0)
            {
                node.right = right;
                Sort(node.right);
            }
        }

        // POST: api/Cluster
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Cluster/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
