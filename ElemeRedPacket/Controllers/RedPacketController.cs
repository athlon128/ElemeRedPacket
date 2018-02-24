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

        static Dictionary<int, string> msgs = new Dictionary<int, string>();
        static List<UserInfo> userlist;
        static RedPacketController()
        {
            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.json"), Encoding.Default);
            string cookies = sr.ReadToEnd();
            userlist = JsonConvert.DeserializeObject<List<UserInfo>>(cookies);
            msgs.Add(2, "你已经领过了!");
            msgs.Add(3, "领到了!");
            msgs.Add(4, "领到了!");
            msgs.Add(5, "每天最多领5个!");
        }

        // GET: api/RedPacket
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // POST: api/RedPacket
        [HttpPost]
        public JsonResult Post()
        {
            //获取post数据输入
            string data;
            ResultMSG result = new ResultMSG();
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
                {
                    data = reader.ReadToEnd();
                }
                JObject submit = JObject.Parse(data);
                string mobile = submit["mobile"].ToString();
                string sn = submit["sn"].ToString();
                int luckyNumber = submit.Value<int>("luckyNumber");
                bool isLucky = false;
                bool isFinished = false;


                //处理红包
                RedPacketHelper rph = new RedPacketHelper();
                for (int i = 0; i < userlist.Count; i++)
                {
                    UserInfo user = isLucky ? userlist[userlist.Count - 1] : userlist[i];
                    user.phone = isLucky ? mobile : rph.GetRandomMobile();
                    user.group_sn = sn;
                    rph.ChangePhone(user);
                    JObject ret = rph.OpenRedPacket(user);
                    int code = int.Parse(ret["ret_code"].ToString());
                    JArray records = JArray.Parse(ret["promotion_records"].ToString());
                    int count = records.Count;
                    Console.WriteLine("当前第" + count + "个红包");
                    if (isLucky)
                    {
                        LuckyInfo luckyInfo=GetLastLuckyInfo(records);
                        result.code = 1;
                        result.msg = msgs[code] + luckyInfo.ToString();
                        isFinished = true;
                        InsertDB(mobile, true, luckyInfo.amount);
                        break;
                    }
                    else
                    {
                        if (count >= luckyNumber)
                        {
                            result.code = 2;
                            result.msg = "没有大红包了!" + GetLuckyInfo(records).ToString();
                            isFinished = true;
                            InsertDB(mobile, false, "0");
                            break;
                        }
                        else if (count == luckyNumber - 1)
                        {
                            isLucky = true;
                            Console.WriteLine("马上要来大红包了...");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.code = 3;
                result.msg = "输入异常或者服务器出错。";
            }
            return Json(result);
        }

        private LuckyInfo GetLuckyInfo(JArray records)
        {
            LuckyInfo info = new LuckyInfo();
            foreach (JObject item in records)
            {
                if (item.Value<bool>("is_lucky"))
                {
                    info.sns_username = item.Value<string>("sns_username");
                    info.amount = item.Value<string>("amount");
                    break;
                }
            }
            return info;
        }
        private LuckyInfo GetLastLuckyInfo(JArray records)
        {
            LuckyInfo info = new LuckyInfo();
            info.sns_username = records[records.Count - 1].Value<string>("sns_username");
            info.amount = records[records.Count - 1].Value<string>("amount");
            return info;
        }
        private void InsertDB(string mobile,bool isLucky,string amount)
        {
            try
            {

                using (MySqlConnection con = new MySqlConnection("server=*;database=eleme;uid=root;pwd=*;charset='gbk';SslMode=None"))
                {
                    con.Open();
                    string sql = String.Format(@"insert into Record (mobile,is_lcuky,amount) values ('{0}',{1},{2})", mobile, isLucky ? 1 : 0, amount);
                    MySqlHelper.ExecuteNonQuery(con, sql, null);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
