using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElemeRedPacket.BLL
{
   public class AppBLL
    {
       static MySqlConnection con;// 
       static AppBLL()
        {
            con = new MySqlConnection(Program.Config.DBString);
            con.Open();
        }
        public static void InsertNewRecord(string mobile, bool isLucky, string amount,int result_code)
        {
            try
            {
                CheckCon();
                string sql = String.Format(@"insert into Record (mobile,is_lcuky,amount,result_code) values ('{0}',{1},{2},{3})", mobile, isLucky ? 1 : 0, amount, result_code);
                int count= MySqlHelper.ExecuteNonQuery(con, sql, null);
                Console.WriteLine("插入" + count + "条数据...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void CheckCon()
        {
            if(con.State!= System.Data.ConnectionState.Open)
            {
                con.Open();
            }
        }
    }
}
