using ElemeRedPacket.BLL;
using ElemeRedPacket.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace ElemeRedPacket.Model
{
    public class RedPacketHelper
    {
        static Dictionary<int, string> msgs = new Dictionary<int, string>();
        static List<UserInfo> userlist;
        static int workerID = 10;
        static RedPacketHelper()
        {
            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.json"), Encoding.Default);
            string cookies = sr.ReadToEnd();
            userlist = JsonConvert.DeserializeObject<List<UserInfo>>(cookies);
            msgs.Add(2, "你已经领过了!");
            msgs.Add(3, "领到了!");
            msgs.Add(4, "领到了!");
            msgs.Add(5, "每天最多领5个!");
        }
        public ResultMSG OpenRedPacket(string mobile, string sn, int luckyNumber)
        {
            Console.WriteLine("SN" + sn + "     ,luckyNumber" + luckyNumber);
            ResultMSG result = new ResultMSG();
            try
            {

                bool isLucky = false;
                bool isFinished = false;

                //处理红包
                RedPacketHelper rph = new RedPacketHelper();
                for (int i = 0; i < userlist.Count; i++)
                {
                    UserInfo user = isLucky ? userlist[GetWorkerID()] : userlist[i];
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
                        LuckyInfo luckyInfo;
                        switch (code)
                        {
                            case 3:
                            case 4:
                                luckyInfo = GetLastLuckyInfo(records);
                                result.code = 1;
                                result.msg = msgs[code] + luckyInfo.ToString();
                                InsertDB(mobile, true, luckyInfo.amount, result.code);
                                break;
                            case 5:
                                result.code = 4;
                                result.msg = msgs[code];
                                break;
                            default:
                                luckyInfo = GetLuckyInfo(records);
                                result.code = 3;
                                result.msg = msgs[code] + luckyInfo.ToString();
                                InsertDB(mobile, false, "0", result.code);
                                break;
                        }
                        Console.WriteLine(result.msg);
                        isFinished = true;
                        break;
                    }
                    else
                    {
                        if (count >= luckyNumber)
                        {

                            result.code = 2;
                            result.msg = "没有大红包了!" + GetLuckyInfo(records).ToString();
                            isFinished = true;
                            InsertDB(mobile, false, "0", result.code);
                            Console.WriteLine(result.msg);
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
                result.code = 5;
                result.msg = "输入异常或者服务器出错。";
                Console.WriteLine(result.msg);
                Console.WriteLine(ex);
            }
            return result;
        }
        private void ChangePhone(UserInfo data)
        {
            string url = "https://restapi.ele.me/v1/weixin/" + data.openId + "/phone";
            using (var client = new System.Net.WebClient())
            {
                JObject obj = new JObject();
                obj.Add("sign", data.sign);
                obj.Add("phone", data.phone);
                string reply = client.UploadString(url, "PUT", obj.ToString());
            }
        }
        private JObject OpenRedPacket(UserInfo data)
        {
            string url = "https://restapi.ele.me/marketing/promotion/weixin/" + data.openId;
            using (var client = new System.Net.WebClient())
            {
                string reply = client.UploadString(url, "POST", JsonConvert.SerializeObject(data));
                return JObject.Parse(reply);
            }
        }
        private string GetRandomMobile()
        {
            var prefixArray = new string[10] { "130", "131", "132", "133", "135", "137", "138", "170", "187", "189" };
            var i = new Random().Next(0, 10);
            var mobile = prefixArray[i];
            for (var j = 0; j < 8; j++)
            {
                mobile = mobile + new Random().Next(0, 10).ToString("0");
            }
            return mobile;
        }
        private int GetWorkerID()
        {
            workerID++;
            if (workerID > userlist.Count - 1)
                workerID = 10;
            return workerID;
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
        private void InsertDB(string mobile, bool isLucky, string amount,int code)
        {
            AppBLL.InsertNewRecord(mobile, isLucky, amount,code);
        }
    }


}
