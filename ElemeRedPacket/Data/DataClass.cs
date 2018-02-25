using System;
using System.Collections.Generic;
using System.Text;

namespace ElemeRedPacket.Data
{
    public class UserInfo
    {
        public string method = "phone";
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
        /// <summary>
        /// 1:领到红包
        /// 2：没有大红包了!
        /// 3:领完了或者
        /// 4:5个上限了
        /// 5:服务器异常
        /// </summary>
        public int code;
        public string msg;
        public override string ToString()
        {
            return "Code:" + code + ",Msg:" + msg;
        }
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
    public class AppConfig
    {
        public string DBString;
    }
}
