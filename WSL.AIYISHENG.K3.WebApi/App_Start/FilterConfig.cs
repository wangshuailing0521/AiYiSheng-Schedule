using System.Web;
using System.Web.Mvc;

namespace WSL.AIYISHENG.K3.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
