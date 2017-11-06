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
using DatabaseInvoke;
using Newtonsoft.Json;

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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/File/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/File
        [HttpPost]
        public ActionResult Post(string applier, string type, IFormCollection files)
        {

            var stream = files.Files[0].OpenReadStream();
            DataEntry entry = new DataEntry();
            using (ExcelPackage excel = new ExcelPackage(stream))
            {
                try
                {
                    entry.GenesList = new List<int[]>();
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    int row = 2;
                    entry.Name = worksheet.Cells[row, 1].Value.ToString();
                    for (int col = 2; col <= ColCount; col += 2)
                    {
                        if (true)
                        {
                            var gene1 = Convert.ToInt32(worksheet.Cells[row, col].Value);
                            var gene2 = Convert.ToInt32(worksheet.Cells[row, col + 1].Value);
                            entry.GenesList.Add(new int[] { gene1, gene2 });
                        }
                    }
                    var profileID = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("ProfileID", profileID);
                    profileDictionary[profileID] = entry;
                }
                catch (Exception)
                {

                    throw;
                }
                using (SqlManipulation sm = new SqlManipulation(@"Server=127.0.0.1;Port=5432;Database=genelib;User Id=postgres;Password = CCBFU6233;", SqlType.PostgresQL))
                {
                    sm.Init();
                    var result = sm.ExcuteQuery($"select * from {type}");
                    foreach (System.Data.DataRow n in result.Rows)
                    {
                        var name = n[0].ToString();
                        var list = new List<int[]>();
                        for (int i = 1; i < result.Columns.Count;)
                        {
                            list.Add(n[i++] as int[]);
                        }
                        entry.SourceDictionary[name] = list;
                    }
                }
                Compare(entry);
                return new JsonResult(entry.ResultDictionary);
            }
        }

        private void Compare(DataEntry entry)
        {
            if (entry.SourceDictionary == null || entry.SourceDictionary.Count == 0) return;
            foreach (var n in entry.SourceDictionary)
            {
                int totalcount = entry.GenesList.Count;
                int samecount = 0;
                for (int i = 0; i < totalcount; i++)
                {
                    //两位点相等
                    if(entry.GenesList[i][0] == entry.GenesList[i][1])
                    {
                        if (entry.GenesList[i][0] == n.Value[i][0])
                        {
                            samecount++;
                        }
                        if (entry.GenesList[i][0] == n.Value[i][1])
                        {
                            samecount++;
                        }
                    }
                    else
                    {
                        if (entry.GenesList[i][0] == n.Value[i][0] || entry.GenesList[i][0] == n.Value[i][1])
                        {
                            samecount++;
                        }
                        if (entry.GenesList[i][1] == n.Value[i][1] || entry.GenesList[i][1] == n.Value[i][1])
                        {
                            samecount++;
                        }
                    }
                }
                entry.ResultDictionary[n.Key] = (float)samecount / (totalcount * 2);
            }
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
                        entry.GenesList = new List<int[]>();
                        entry.Name = worksheet.Cells[row, 1].Value.ToString();
                        for (int col = 2; col <= ColCount; col += 2)
                        {
                            if (true)
                            {
                                var gene1 = Convert.ToInt32(worksheet.Cells[row, col].Value);
                                var gene2 = Convert.ToInt32(worksheet.Cells[row, col + 1].Value);
                                entry.GenesList.Add(new int[] { gene1, gene2 });
                            }
                        }
                        datalist.Add(entry);
                    }
                    using (SqlManipulation sm = new SqlManipulation(@"Server=127.0.0.1;Port=5432;Database=genelib;User Id=postgres;Password = CCBFU6233;", SqlType.PostgresQL))
                    {
                        sm.Init();
                        foreach (var n in datalist)
                        {

                            var sql = $"insert into {type} values('{n.Name}',";
                            foreach (var xn in n.GenesList)
                            {
                                sql += $"'{{{xn[0]},{xn[1]}}}',";
                            }
                            sql = sql.Substring(0, sql.Length - 1);
                            sql += ")";
                            sm.ExcuteNonQuery(sql);
                        }
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
