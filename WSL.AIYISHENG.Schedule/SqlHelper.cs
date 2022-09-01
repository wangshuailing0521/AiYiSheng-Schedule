using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSL.AIYISHENG.Schedule
{
    public class SqlHelper
    {
        
        //public static string connectionStrings = "Server=.;Database=AIS20210122161449;uid=sa;pwd=kingdee@123;";
        public static string connectionStrings = "Server=.;Database=AIS20210227102511;uid=sa;pwd=kingdee@123;";

        public static DataTable Get(string sql)
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

        public static int Update(string sql)
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
