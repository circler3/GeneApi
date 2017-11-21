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

namespace GeneApi.Controllers
{
    [Consumes("application/json", "multipart/form-data", "application/x-www-form-urlencoded")]
    [Produces("application/json", "application/octet-stream", "application/pdf")]
    [Route("api/[controller]")]
    public class PDFController : Controller
    {
        // GET: api/PDF
        [HttpGet]
        public ActionResult Get()
        {
            return Post(null);
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
            string[] name = collection["name"];

            using (MemoryStream ms = new MemoryStream())
            {
                BaseFont.AddToResourceSearch(Assembly.LoadFrom("iTextAsian.dll"));
                PdfReader reader = new PdfReader("template2.pdf");
                PdfStamper pdfStamper = new PdfStamper(reader, ms);
                AcroFields pdfFormFields = pdfStamper.AcroFields;//获取域的集合

                BaseFont baseFT = BaseFont.CreateFont("SIMHEI.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                pdfFormFields.AddSubstitutionFont(baseFT);//设置域的字体;生成文件几十K

                //内容填充字段
                pdfFormFields.SetField("送检编号", "1");
                pdfFormFields.SetField("送检单位", "1");
                pdfFormFields.SetField("样品名称", "2");
                pdfFormFields.SetField("鉴定项目", "3");
                pdfFormFields.SetField("鉴定单位", "4");
                pdfFormFields.SetField("年", "5");
                pdfFormFields.SetField("月", "6");
                pdfFormFields.SetField("日", "7");
                pdfFormFields.SetField("样品编号", "8");
                pdfFormFields.SetField("样品数量", "9");
                pdfFormFields.SetField("样品形态", "10");
                pdfFormFields.SetField("基因对数", "10");
                pdfFormFields.SetField("树种", "11");
                pdfFormFields.SetField("相似系数低", "12");
                pdfFormFields.SetField("相似系数高", "13");
                pdfFormFields.SetField("最高目标品种名称", "14");
                pdfFormFields.SetField("最高相似度", "15");
                pdfFormFields.SetField("相同不同", "16");
                pdfFormFields.SetField("联系人", "17");
                pdfFormFields.SetField("联系电话", "18");
                pdfFormFields.SetField("主检人", "19");
                pdfFormFields.SetField("技术负责人", "20");

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
