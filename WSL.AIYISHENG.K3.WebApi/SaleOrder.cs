using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSL.AIYISHENG.K3.WebApi
{
    public class FSaleDeptId
    {
        /// <summary>
        /// 
        /// </summary>
        public string FNumber { get; set; }
    }

    public class FSalerId
    {
        /// <summary>
        /// 
        /// </summary>
        public int FNumber { get; set; }
    }

    public class FSaleOrgId
    {
        /// <summary>
        /// 
        /// </summary>
        public int FNumber { get; set; }
    }

    public class FSettleCurrId
    {
        /// <summary>
        /// 
        /// </summary>
        public string FNumber { get; set; }
    }

    public class FSaleOrderFinance
    {
        /// <summary>
        /// 
        /// </summary>
        public string FIsIncludedTax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FSettleCurrId FSettleCurrId { get; set; }
    }

    public class FCustId
    {
        /// <summary>
        /// 
        /// </summary>
        public int FNumber { get; set; }
    }

    public class FMaterialId
    {
        /// <summary>
        /// 
        /// </summary>
        public int FNumber { get; set; }
    }

    public class FPriceUnitId
    {
        /// <summary>
        /// 
        /// </summary>
        public string FNumber { get; set; }
    }

    public class FUnitId
    {
        /// <summary>
        /// 
        /// </summary>
        public string FNumber { get; set; }
    }

    public class FSaleOrderEntryItem
    {
        /// <summary>
        /// 
        /// </summary>
        public FMaterialId FMaterialId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FQty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FTaxPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FEntryTaxRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FPriceUnitId FPriceUnitId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double F_ays_Decimal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_ays_Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FUnitId FUnitId { get; set; }
    }

    public class Model
    {
        /// <summary>
        /// 
        /// </summary>
        public string F_AYS_DATE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FSaleDeptId FSaleDeptId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FSalerId FSalerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FSaleOrgId FSaleOrgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FSaleOrderFinance FSaleOrderFinance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FCustId FCustId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FBILLNO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FSaleOrderEntryItem> FSaleOrderEntry { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> NeedReturnFields { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Model Model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsAutoSubmitAndAudit { get; set; }
    }

    public class SaleOrder
    {
        /// <summary>
        /// 
        /// </summary>
        public string formid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }

}