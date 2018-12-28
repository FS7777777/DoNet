using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace runscript
{
    public class sqlconn
    {
        private string OSQL_PATH;
        private string Server;
        private string UserId;
        private string Password;

        //构造函数
        public sqlconn(string server, string userId, string password)
        {
            Server = server;
            UserId = userId;
            Password = password;
            OSQL_PATH = ConfigurationManager.AppSettings["OSQL_PATH"];
        }

        //连接数据库
        public SqlCommand Command(string dbname, string sql)
        {
            string conn = string.Format(@"server={0};uid={1};pwd={2};persist security info=false", Server, UserId, Password);
            SqlConnection mySqlConnection = new SqlConnection(conn);
            SqlCommand Command = null;
            try
            {
                Command = new SqlCommand(sql, mySqlConnection);
                Command.Connection.Open();
                Command.Connection.ChangeDatabase(dbname);
                Command.CommandText = sql;
                return Command;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            finally
            {
                if (Command != null)
                    ;//Command.Connection.Close();
            }
        }

        //调用osql
        /// <param name="dbname">数据库名</param>
        /// <param name="scriptfile">要执行的包含路径的脚本文件名，例：C:\sql\create_table1.sql</param>
        public bool ExecuteSqlScript(string dbname, string scriptfile)
        {
            try
            {
                System.Diagnostics.Process sqlProcess = new System.Diagnostics.Process();
                sqlProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //调用进程的窗口状态，隐藏为后台 
                sqlProcess.StartInfo.FileName = OSQL_PATH;
                sqlProcess.StartInfo.Arguments = string.Format(@"-S {0} -U {1} -P {2} -d {3} -i {4}", Server, UserId, Password, dbname, scriptfile);
                sqlProcess.Start();
                sqlProcess.WaitForExit();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
