using log4net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Quartz;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace WSL.AIYISHENG.Schedule
{
    [DisallowConcurrentExecution] //禁止并发执行
    public class SaleOrderJob : IJob
    {
        ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {

            GetDataToK3();

            ToCrm();
        }

        public void GetDataToK3 (){
            try
            {
                string sql = $@"
                    SELECT  FBillNo,FJson
                      FROM  Interface_SaleOrder WITH(NOLOCK)
                     WHERE  FSTATUS IN (0,2)";

                DataTable dt = SqlHelper.Get(sql);
                if (dt.Rows.Count <= 0)
                {
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    string billNo = row["FBillNo"].ToString();
                    string json = row["FJson"].ToString();
                    string Id = "10";
                    string EntryId = "100";

                    string result = ToK3(json, out Id, out EntryId);

                    if (Id == "10")
                    {
                        sql = $@"
                            UPDATE Interface_SaleOrder 
                            SET FSTATUS = '2',
                                FMessage = '{result.Replace("'", "''")}'
                            WHERE FBILLNO ='{billNo}'";
                        SqlHelper.Update(sql);
                    }
                    else
                    {
                        sql = $@"
                            UPDATE Interface_SaleOrder 
                            SET FSTATUS = '1',
                                FID = '{Id}',
                                FEntryID = '{EntryId}',
                                FMessage = '{result.Replace("'", "''")}'
                            WHERE FBILLNO ='{billNo}'";
                        SqlHelper.Update(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("***ERROR***传递数据到K3***" + ex.Message + ex.StackTrace);
            }
        }

        public void ToCrm()
        {
            try
            {
                string sql = $@"
                    SELECT  FBillNo,FID,FEntryID
                      FROM  Interface_SaleOrder WITH(NOLOCK)
                     WHERE  FSTATUS = '1'
                       AND  FID <> 0";

                DataTable dt = SqlHelper.Get(sql);
                if (dt.Rows.Count <= 0)
                {
                    return;
                }

                //string url = "http://47.103.137.34:10000/aysdmp/base/processOrderRelatedPush.do";
                string url = "https://oms.aijiangkj.com/aysdmp/base/processOrderRelatedPush.do";

                foreach (DataRow row in dt.Rows)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("请求Url：" + url);
                    string billNo = row["FBillNo"].ToString();
                    string Id = row["FID"].ToString();
                    string entryId = row["FEntryID"].ToString();

                    var request = new
                    {
                        code = billNo,
                        id = Id,
                        entry = entryId
                    };

                    string requestJson = JsonConvert.SerializeObject(request);

                    sb.AppendLine("请求信息：" + requestJson);

                    string result = ApiHelper.HttpRequest(url, requestJson, "POST");
                    sb.AppendLine("返回信息：" + result);

                    LoggerHelper.Info(sb.ToString());

                    JObject val = JObject.Parse(result);
                    if (val["success"] != null)
                    {
                        if (Convert.ToBoolean(((object)val["success"]).ToString()))
                        {
                            sql = $@"
                            UPDATE  Interface_SaleOrder 
                               SET  FSTATUS = '3'
                             WHERE  FBILLNO ='{billNo}'";
                            SqlHelper.Update(sql);
                        }
                        else
                        {
                            sql = $@"
                            UPDATE  Interface_SaleOrder 
                               SET  FSTATUS = '4'
                             WHERE  FBILLNO ='{billNo}'";
                            SqlHelper.Update(sql);
                        }
                    }
                   
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("***ERROR***传递数据到CRM***" + ex.Message + ex.StackTrace);
            }
        }

        string ToK3(string json ,out string Id,out string EntryId)
        {
            Id = "10";
            EntryId = "100";
            return OneSync(json,out Id,out EntryId);
        }

        public string OneSync(
          string json,
          out string Id ,
          out string entryId
            )
        {
            string loginUrl = "http://47.102.116.183/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUser.common.kdsvc";
            string dbid = "6039ae76bf7c07";
            string user = "administrator";
            string password = "ays198368@";
            string saveUrl = "http://47.102.116.183/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save.common.kdsvc";
            Id = "10";
            entryId = "100";

            StringBuilder sb = new StringBuilder();
            try
            {
                LoggerHelper.Info(json);

                KingdeeHttpClient httpClient = new KingdeeHttpClient();
                httpClient.Url = loginUrl;
                List<object> Parameters = new List<object>();
                Parameters.Add(dbid);
                Parameters.Add(user);
                Parameters.Add(password);
                Parameters.Add(2052);
                httpClient.Content = JsonConvert.SerializeObject(Parameters);
                string loginResult = httpClient.AsyncRequest();
                var iResult = JObject.Parse(loginResult)["LoginResultType"].Value<int>();
                if (iResult == 1)
                {
                    httpClient.Url = saveUrl;
                    httpClient.Content = json;
                    var responseOut = httpClient.HttpPost();

                    LoggerHelper.Info(responseOut);

                    KingdeeSaveResponse response = JsonConvert.DeserializeObject<KingdeeSaveResponse>(responseOut);

                    ResponseStatus status = response.Result.ResponseStatus;

                    if (!status.IsSuccess)
                    {
                        foreach (var item in status.Errors)
                        {
                            sb.AppendLine(item.Message);
                        }
                    }
                    else
                    {
                        Id = response.Result.Id;

                        List<string> entryIdList = new List<string>();
                        foreach (var needReturnDataItem in response.Result.NeedReturnData)
                        {
                            foreach (var entry in needReturnDataItem.FSaleOrderEntry)
                            {
                                entryIdList.Add(entry.FEntryID);
                            }  
                        }

                        entryId = string.Join(",", entryIdList);

                    }

                    return responseOut;
                }
                else
                {
                    return loginResult;
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
        }

       
    }
}
