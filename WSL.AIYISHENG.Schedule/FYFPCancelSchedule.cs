using log4net;

using Newtonsoft.Json.Linq;

using Quartz;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WSL.AIYISHENG.Schedule
{
	[DisallowConcurrentExecution] //禁止并发执行
	[Description("费用应收单发票号码作废执行计划")]
    public class FYFPCancelSchedule : IJob
	{
        ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            ToInterface();
        }

		private void ToInterface()
		{
			List<CancelModel> data = this.GetData();
			bool flag = data.Count <= 0;
			if (!flag)
			{
				for (int i = 0; i < data.Count; i++)
				{
					this.dBLog = new DBLog();
					this.dBLog.FInvocation = "Kingdee";
					this.dBLog.FInterfaceType = "FYInvoiceCancel";
					this.dBLog.FBeginTime = DateTime.Now.ToString();
					StringBuilder stringBuilder = new StringBuilder();
					CancelModel cancelModel = data[i];
					stringBuilder.AppendLine("");
					stringBuilder.AppendLine("接口方向：Kingdee --> ");
					stringBuilder.AppendLine("接口名称：费用应收单发票号码作废API");
					try
					{
						this.Require(cancelModel, stringBuilder);
						this.UpdateStatus(cancelModel.fpNo);
						this.dBLog.FStatus = "S";
						this.dBLog.FMessage = "成功";
						LoggerHelper.Info(stringBuilder.ToString());
					}
					catch (Exception ex)
					{
						this.dBLog.FStatus = "E";
						this.dBLog.FMessage = ex.Message;
						this.dBLog.FStackMessage = ex.ToString();
						stringBuilder.AppendLine("错误信息：" + ex.Message.ToString());
						LoggerHelper.Error(stringBuilder.ToString(), ex);
					}
					finally
					{
						this.dBLog.Insert();
					}
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002B08 File Offset: 0x00000D08
		private void Require(CancelModel item, StringBuilder sb)
		{
			string text = "https://oms.aijiangkj.com/aysdmp/base/deleteOrderInvoicePush.do";
			string text2 = string.Join(",", new string[]
			{
				item.fpNo
			});

			List<string> noList = item.fpNo.Split(',').ToList();

			var o = new
			{
				invoiceCodes = noList
			};
			sb.AppendLine("请求Url：" + text);
			string text3 = o.ToJSON();
			sb.AppendLine("请求信息：" + text3);
			this.dBLog.FOperationType = text;
			this.dBLog.FBillNo = text2;
			this.dBLog.FRequestMessage = text3;
			string text4 = ApiHelper.HttpRequest(text, text3, "POST", "application/json", null);
			sb.AppendLine("返回信息：" + text4);
			this.dBLog.FResponseMessage = text4;
			this.dBLog.FEndTime = DateTime.Now.ToString();
			JObject jobject = JObject.Parse(text4);
			bool flag = jobject["success"] != null;
			if (!flag)
			{
				throw new Exception(text4);
			}
			bool flag2 = !Convert.ToBoolean(jobject["success"].ToString());
			if (flag2)
			{
				throw new Exception(text4);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002C48 File Offset: 0x00000E48
		private List<CancelModel> GetData()
		{
			List<CancelModel> list = new List<CancelModel>();
			string text = "\r\n                SELECT  DISTINCT \r\n                        E.FIVNUMBER\r\n                  FROM  T_AR_RECEIVABLE A\r\n                        INNER JOIN t_AR_receivableEntry B \r\n                        ON A.FID = B.FID\r\n                        INNER JOIN T_IV_SALEEXINVENTRY_LK C\r\n                        ON C.FSID = B.FENTRYID AND C.FSBILLID = B.FID\r\n                        INNER JOIN T_IV_SALEEXINVENTRY D\r\n                        ON C.FENTRYID = D.FENTRYID\r\n                        INNER JOIN T_IV_SALEEXINV E\r\n                        ON D.FID = E.FID\r\n                 WHERE  1=1\r\n                   AND  ISNULL(B.FORDERNUMBER,'') <> '' --销售订单号不为空\r\n                   AND  ISNULL(E.FIVNUMBER,'') <> '' --销售发票上的发票号码不为空\r\n                   AND  A.FDocumentStatus = 'C' --应收单已审核\r\n                   AND  ISNULL(E.FFPStatus,'0') = '1' --销售发票上的传递状态为已传递\r\n                   AND  E.FGTSTATUS  = '2' --发票作废\r\n                   AND  ISNULL(E.FFPCancelStatus,'0')  = '0' --销售发票上的作废传递状态为未传递\r\n                ";
			DataTable dt = SqlHelper.Get(text);
			foreach (DataRow item in dt.Rows)
			{
				list.Add(new CancelModel
				{
					fpNo = item["FIVNUMBER"].ToString()
				});
			}
			return list;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002CE8 File Offset: 0x00000EE8
		private void UpdateStatus(string billNo)
		{
			string text = "\r\n                UPDATE  E\r\n                   SET  E.FFPCancelStatus = '1'  \r\n                  FROM  T_IV_SALEEXINV E\r\n                 WHERE  E.FIVNUMBER = '" + billNo + "'";
			SqlHelper.Update(text);
		}


		// Token: 0x04000018 RID: 24
		private DBLog dBLog = new DBLog();
	}
}
