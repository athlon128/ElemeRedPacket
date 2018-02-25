using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ElemeRedPacket.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElemeRedPacket.Controllers
{
    [Produces("application/json")]
    [Route("api/RedPacket")]
    public class RedPacketController : Controller
    {
        // GET: api/RedPacket
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1",""};
        }

        // POST: api/RedPacket
        [HttpPost]
        public JsonResult Post()
        {
            RedPacketHelper help = new RedPacketHelper();
            //获取post数据输入
            string data;
            using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
            {
                data = reader.ReadToEnd();
            }
            JObject submit = JObject.Parse(data);
            string mobile = submit["mobile"].ToString();
            string sn = submit["sn"].ToString();
            int luckyNumber = submit.Value<int>("luckyNumber");
            return Json(help.OpenRedPacket( mobile, sn, luckyNumber));
        }



    }
}
