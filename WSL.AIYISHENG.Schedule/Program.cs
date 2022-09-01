using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Topshelf;

namespace WSL.AIYISHENG.Schedule
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));

            //SaleOrderJob saleOrderJob = new SaleOrderJob();
            //saleOrderJob.ToCrm();

            HostFactory.Run(x =>
            {
                    //x.UseLog4Net();

                    x.Service<ServiceRunner>();

                x.SetDescription("定时同步销售订单应收单作废");
                x.SetDisplayName("定时同步销售订单应收单作废");
                x.SetServiceName("定时同步销售订单应收单作废");

                x.EnablePauseAndContinue();
            });
        }
    }
}
