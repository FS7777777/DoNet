
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;


namespace runscript
{
    class Program
    {
        //连接数据库，并查询现有数据库
        private static void ExecSql(sqlconn Sqlconn, List<UserMsg> ListArry, string DbName)
        {
            try
            {
                string sql = "select FCusId,FDBName from CusDBSpace";
                SqlCommand Command = Sqlconn.Command(DbName, sql);
                SqlDataReader reader = Command.ExecuteReader();
                //从数据库中读取数据流存入reader中  
                while (reader.Read())
                {
                    UserMsg userMsg = new UserMsg();
                    userMsg.FCusId = reader.GetInt32(reader.GetOrdinal("FCusId")).ToString();
                    userMsg.FDBName = reader.GetString(reader.GetOrdinal("FDBName"));
                    ListArry.Add(userMsg);
                }
                //关闭
                Command.Connection.Close();
            }
            catch (Exception e)
            {
                Write(e.ToString(),"错误日志");
                Console.WriteLine("发生错误，请查看日志记录！");
            }
            finally
            {
                
            }
        }

        //写错误日志到文件
        public static void Write(string e,string name)
        {
            FileStream fs = new FileStream(DateTime.Now.ToString("yyyyMMddhhmmss") + name + ".txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(e);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        static void Main(string[] args)
        {
            List<UserMsg> ListArry= new List<UserMsg>();
            string DbName = ConfigurationManager.AppSettings["dbname"];
            string Server = ConfigurationManager.AppSettings["Server"];
            string UserId = ConfigurationManager.AppSettings["UserId"];
            string Password = ConfigurationManager.AppSettings["Password"];
            string scriptfile = ConfigurationManager.AppSettings["SQL_SCRIPT"];
            sqlconn Sqlconn = new sqlconn(Server, UserId, Password);
            ExecSql(Sqlconn,ListArry,DbName);
            try
            {
                //执行sqlFile
                foreach (UserMsg obj in ListArry)
                {
                    Sqlconn.ExecuteSqlScript(obj.FDBName, scriptfile);
                    Console.WriteLine("数据库：{0} --执行脚本{1}", obj.FDBName, scriptfile);
                }
                Console.WriteLine("执行完毕！回车关闭！");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Write(e.ToString(), "错误日志");
            }
        }

    }
}
