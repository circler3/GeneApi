using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using OfficeOpenXml;
using GeneApi.Models;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;

namespace GeneApi.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json", "multipart/form-data")]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private static Dictionary<string, DataEntry> profileDictionary = new Dictionary<string, DataEntry>();
        // GET: api/File
        [HttpGet]
        public ActionResult Get([FromQuery]string nameofComparer)
        {
            var entry = profileDictionary[HttpContext.Session.GetString("ProfileID")];
            
            return new JsonResult(profileDictionary[HttpContext.Session.GetString("ProfileID")]);
        }

        // GET: api/File/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(string id, [FromQuery]string nameofComparer)
        {
            return new JsonResult(profileDictionary[id]);
        }

        // POST: api/File
        [HttpPost]
        public ActionResult Post(string applier, string type, IFormCollection files)
        {
            return null;
        }

        // PUT: api/File/5
        [HttpPut]
        public void Put(string type, IFormCollection files)
        {
            var stream = files.Files[0].OpenReadStream();

            using (ExcelPackage excel = new ExcelPackage(stream))
            {
                try
                {
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    List<DataEntry> datalist = new List<DataEntry>();
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DataEntry entry = new DataEntry();
                        entry.user = "admin";
                        entry.type = type;
                        entry.company = "北京林业大学";
                        entry.updateAt = DateTime.UtcNow;
                        entry.createAt = DateTime.UtcNow;
                        entry.sample = worksheet.Cells[row, 1].Value.ToString();
                        Dictionary<string, int[]> num = new Dictionary<string, int[]>();
                        for (int col = 2; col <= ColCount; col += 2)
                        {
                            if (true)
                            {
                                var gene1 = Convert.ToInt32(worksheet.Cells[row, col].Value);
                                var gene2 = Convert.ToInt32(worksheet.Cells[row, col + 1].Value);
                                num.Add(worksheet.Cells[1, col].Value.ToString(), new int[] { gene1, gene2 });
                            }
                        }
                        entry.data = num;
                        datalist.Add(entry);
                    }

                    var client = new MongoClient("mongodb://222.31.160.146:27017");
                    var database = client.GetDatabase("gene");
                    var collection = database.GetCollection<BsonDocument>("library");
                    foreach (var n in datalist)
                    {
                        collection.InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(n)));
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
