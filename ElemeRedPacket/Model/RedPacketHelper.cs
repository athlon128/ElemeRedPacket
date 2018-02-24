using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace ElemeRedPacket.Model
{
    public class RedPacketHelper
    {
        public void ChangePhone(UserInfo data)
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

        public JObject OpenRedPacket(UserInfo data)
        {
            string url = "https://restapi.ele.me/marketing/promotion/weixin/" + data.openId;
            using (var client = new System.Net.WebClient())
            {
                string reply = client.UploadString(url, "POST", JsonConvert.SerializeObject(data));
                return  JObject.Parse(reply);
            }
        }

        public string GetRandomMobile()
        {
            var prefixArray = new string[10]{ "130", "131", "132", "133", "135", "137", "138", "170", "187", "189"};
            var i =  new Random().Next(0,10);
            var mobile = prefixArray[i];
            for (var j = 0; j < 8; j++)
            {
                mobile = mobile + new Random().Next(0,10).ToString("0");
            }
            return mobile;
        }
    }

    public class UserInfo
    {
        public string method="phone";
        public string device_id;
        public int platform;
        public string hardware_id;
        public string track_id = "undefined";
        public string unionid = "fuck";
        public string openId;
        public string sign;
        public string group_sn;
        public string weixin_avatar;
        public string weixin_username;
        public string phone;
    }

    public class ResultMSG
    {
        public int code;
        public string msg;
    }
    public class LuckyInfo
    {
        public string sns_username;
        public string amount;
        public override string ToString()
        {
            return "\n手气最佳：" + sns_username + "\n红包金额：" + amount;
        }
    }
}
