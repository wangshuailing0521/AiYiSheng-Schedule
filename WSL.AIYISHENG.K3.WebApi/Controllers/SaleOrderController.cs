using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace WSL.AIYISHENG.K3.WebApi.Controllers
{
    [WebApiTracker]
    public class SaleOrderController : ApiController
    {
        StringBuilder logInfo = new StringBuilder();
        private string request = "";
        private string response = "";
        //string connectionStrings = "Server=172.17.73.17;Database=YSYS;uid=sa;pwd=Q3ek9!8#1zf;";
        //string connectionStrings = "Server=.;Database=AIS20210701181229;uid=sa;pwd=kingdee@123;";
        string connectionStrings = "Server=.;Database=AIS20210227102511;uid=sa;pwd=kingdee@123;";

        //AIS20210602134721

        // POST api/<controller>
        [HttpPost]
        public void Execute([FromBody]dynamic json)
        {
            JObject objRetutrn = new JObject();
          
            try
            {
                string jsonString = JsonConvert.SerializeObject(json);
                request = jsonString;
                
                ToInterfaceTable(jsonString);

                objRetutrn.Add("status", "1");
                objRetutrn.Add("message", "success");
                response = JsonConvert.SerializeObject(objRetutrn);

                logInfo.AppendLine($@"请求信息:{request}");
                logInfo.AppendLine($@"返回信息:{response}");
                LoggerHelper.Info(logInfo.ToString());

            }
            catch (Exception ex)
            {
                objRetutrn.Add("status", "2");
                objRetutrn.Add("message", ex.Message);
                response = JsonConvert.SerializeObject(objRetutrn);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($@"请求信息:{request}");
                sb.AppendLine($@"返回信息:{response}");
                sb.AppendLine($@"详细信息:{ex.Message + ex.StackTrace}");
                LoggerHelper.Error(sb.ToString());
            }

            HttpContext.Current.Response.ClearHeaders();//这里是关键，清除在返回前已经设置好的标头信息
            HttpContext.Current.Response.BufferOutput = true;//设置输出缓冲
            if (!HttpContext.Current.Response.IsRequestBeingRedirected)//做判断,防止重复
            {
                HttpContext.Current.Response.ContentType = "application/Json";
                HttpContext.Current.Response.Write(response);
                HttpContext.Current.Response.End();
            }

        }

        private void ToInterfaceTable(string json)
        {
            StringBuilder sb = new StringBuilder();

            
            SaleOrder orderObject = JsonConvert.DeserializeObject<SaleOrder>(json);
            Model model = orderObject.data.Model;

            string sql = "";
            //sql = $@"SELECT 1 FROM Interface_SaleOrder WITH(NOLOCK) WHERE FBillNo = '{model.FBILLNO}'";
            //DataTable dt = Get(sql);
            //if (dt.Rows.Count > 0)
            //{
            //    throw new Exception($@"单号{model.FBILLNO}已经存在!");
            //}

            sql = $@"
                    INSERT INTO Interface_SaleOrder
                    SELECT '{model.FBILLNO}','{model.FDate}','{model.FSaleOrgId.FNumber}',
                           '{model.FSaleDeptId.FNumber}','{model.FSalerId.FNumber}','{model.FCustId.FNumber}',
                           '{model.FSaleOrderFinance.FIsIncludedTax}','{model.FSaleOrderFinance.FSettleCurrId.FNumber}',
                           '{model.F_AYS_DATE}','0','',0,'','{json.Replace("'","''")}'
                    ";
            sb.AppendLine(sql);

            //List<FSaleOrderEntryItem> entrys = model.FSaleOrderEntry;
            //foreach (var entry in entrys)
            //{
            //    sql = $@"
            //        INSERT INTO Interface_SaleOrderEntry
            //        SELECT '{model.FBILLNO}','{entry.FMaterialId.FNumber}',{entry.FQty},
            //               {entry.FTaxPrice},{entry.FEntryTaxRate},'{entry.FPriceUnitId.FNumber}',
            //               {entry.F_ays_Decimal},'{entry.F_ays_Text}','{entry.FUnitId.FNumber}'
            //        ";
            //    sb.AppendLine(sql);
            //}

            int count = Insert(sb.ToString());
            if (count <= 0)
            {
                throw new Exception("更新数据库失败");
            }
        }

        public DataTable Get(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection1 = new SqlConnection(connectionStrings))
            {
                sqlConnection1.Open();      //打开数据库连接
                SqlCommand cmd = new SqlCommand(sql, sqlConnection1);  //执行SQL命令
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dataSet = new DataSet();
                da.Fill(dataSet);
                dt = dataSet.Tables[0];
            }
            return dt;
        }

        private int Insert(string sql)
        {
            int Succnum = 0;
            using (SqlConnection sqlConnection1 = new SqlConnection(connectionStrings))
            {
                sqlConnection1.Open();      //打开数据库连接
                SqlCommand sqlCommand1 = new SqlCommand(sql, sqlConnection1);  //执行SQL命令
                Succnum = sqlCommand1.ExecuteNonQuery();
            }               
            return Succnum;
        }
    }
}