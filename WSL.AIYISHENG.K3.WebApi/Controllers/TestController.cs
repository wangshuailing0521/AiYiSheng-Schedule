using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WSL.AIYISHENG.K3.WebApi.Controllers
{
    public class TestController : ApiController
    {
        [HttpPost]
        public void Execute([FromBody] dynamic json)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            LoggerHelper.Info(jsonString);

            JObject objRetutrn = new JObject();
            objRetutrn.Add("status", "200");
            objRetutrn.Add("message", "success");

            try
            {
                JObject request = JObject.Parse(jsonString);
                string type = request["type"].ToString();
            }
            catch (Exception ex)
            {

                throw;
            }


            string response = JsonConvert.SerializeObject(objRetutrn);

            HttpContext.Current.Response.ClearHeaders();//这里是关键，清除在返回前已经设置好的标头信息
            HttpContext.Current.Response.BufferOutput = true;//设置输出缓冲
            if (!HttpContext.Current.Response.IsRequestBeingRedirected)//做判断,防止重复
            {
                HttpContext.Current.Response.ContentType = "application/Json";
                HttpContext.Current.Response.Write(response);
                HttpContext.Current.Response.End();
            }
        }
        
    }
}