using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using iTextSharp.LGPLv2.Core;
using System.IO;

using System.Reflection;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Cors;
using MongoDB.Driver;
using MongoDB.Bson;

namespace GeneApi.Controllers
{
    [Consumes("application/json", "multipart/form-data", "application/x-www-form-urlencoded")]
    [Produces("application/json", "application/octet-stream", "application/pdf")]
    [Route("api/[controller]")]
    public class PDFController : Controller
    {
        // GET: api/PDF
        [HttpGet]
        public string Get()
        {
            return "AVB";
        }

        // GET: api/PDF/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PDF
        [HttpPost]
        [EnableCors("any")]
        public ActionResult Post(IFormCollection collection)
        {
            string name = collection["name"];
            string[] names = name.Split(',');

            string result = collection["result"];
            string[] results = result.Split(',');

            string sample = collection["sample"];
            string type = collection["type"];

            var client = new MongoClient("mongodb://222.31.160.146:27017");
            var database = client.GetDatabase("gene");
            var source = database.GetCollection<BsonDocument>("jujubenews");
            var filter = Builders<BsonDocument>.Filter.Eq("sample", sample);
            var target = source.Find(filter).FirstOrDefault() ;

            using (MemoryStream ms = new MemoryStream())
            {
                BaseFont.AddToResourceSearch(Assembly.LoadFrom("iTextAsian.dll"));
                PdfReader reader = new PdfReader("template2.pdf");
                PdfStamper pdfStamper = new PdfStamper(reader, ms);
                AcroFields pdfFormFields = pdfStamper.AcroFields;//获取域的集合

                BaseFont baseFT = BaseFont.CreateFont("SIMHEI.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                pdfFormFields.AddSubstitutionFont(baseFT);//设置域的字体;生成文件几十K

                //内容填充字段
                pdfFormFields.SetField("送检编号", target.GetValue("_id").ToString());
                pdfFormFields.SetField("送检单位", target.GetValue("company").ToString());
                pdfFormFields.SetField("样品名称", sample);
                pdfFormFields.SetField("鉴定项目", "SSR");
                pdfFormFields.SetField("鉴定单位", "北京林业大学");
                pdfFormFields.SetField("年", DateTime.Now.Year.ToString());
                pdfFormFields.SetField("月", DateTime.Now.Month.ToString());
                pdfFormFields.SetField("日", DateTime.Now.Day.ToString());
                pdfFormFields.SetField("样品编号", "8");
                pdfFormFields.SetField("样品数量", "9");
                pdfFormFields.SetField("样品形态", "凝胶");
                pdfFormFields.SetField("基因对数", target.GetValue("data").AsBsonArray.Count.ToString());
                pdfFormFields.SetField("树种", type);
                pdfFormFields.SetField("相似系数低", results[results.Length - 1] + "%");
                pdfFormFields.SetField("相似系数高", results[0] + "%");
                pdfFormFields.SetField("最高目标品种名称", names[0]);
                pdfFormFields.SetField("最高相似度", results[0] + "%");
                pdfFormFields.SetField("相同不同", results[0] == "100.00" ? "相同":"不同");
                pdfFormFields.SetField("联系人", "admin");
                pdfFormFields.SetField("联系电话", "1861234567");
                pdfFormFields.SetField("主检人", "admin");
                pdfFormFields.SetField("技术负责人", "admin");

                pdfStamper.FormFlattening = true;//设置true PDF文件不能编辑
                reader.Close();
                pdfStamper.Close();
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileContentResult(ms.ToArray(), "application/pdf"); ;
                HttpContext.Response.ContentType = "application/pdf";

                return stream;
            } 
        }

        // PUT: api/PDF/5
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
