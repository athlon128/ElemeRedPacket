using ElemeRedPacket.Data;
using ElemeRedPacket.Model;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ElemeRedPacket.Controllers
{
    [Produces("application/json")]
    [Route("api/OneKey")]
    public class OneKeyController : Controller
    {
        [HttpGet]
        public JsonResult Get()
        {
            ResultMSG result = new ResultMSG();
            RedPacketHelper help = new RedPacketHelper();
            string mobile= Request.Query["mobile"].ToString();
            List<string> links = GetRealUrls();
            foreach (var item in links)
            {
                Console.WriteLine("             ---------            ");
                string sn = GetUrlParam(item, "sn");
                int luckyNumber = 0;
                if (int.TryParse(GetUrlParam(item, "lucky_number"), out luckyNumber))
                {
                    ResultMSG msg = help.OpenRedPacket(mobile, sn, luckyNumber);
                    Console.WriteLine(msg.ToString());
                    if (msg.code == 1|| msg.code==4)
                    {
                        return Json(msg);
                    }
                    
                }
                else
                    continue;
            }
            result.code = 3;
            result.msg = "没有领到红包";
            return Json(result); 
        }




        public List<string> GetRealUrls()
        {
            List<string> list = new List<string>();
            HtmlWeb wb = new HtmlWeb();
            string url = @"https://www.pinghongbao.com/eleme";
            HtmlDocument doc = wb.Load(url);
            HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a[@class='btn copybtn']");
            foreach (var item in links)
            {
                string link = item.GetAttributeValue("data-clipboard-text", string.Empty);
                list.Add(GetRealUrl(link));
            }
            return list;
        }

        private string GetRealUrl(string link)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.GetResponseStream();
            return response.ResponseUri.AbsoluteUri;
        }

        private string GetUrlParam(string url,string key)
        {
            Regex re = new Regex(@"(^|&)" + key + "=([^&]*)(&|$)", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(url);
            if (mc.Count > 0)
                return mc[0].Result("$2");
            else
                return string.Empty;

        }
    }
}
