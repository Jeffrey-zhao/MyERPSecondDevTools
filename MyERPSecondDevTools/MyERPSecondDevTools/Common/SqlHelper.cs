using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Common
{
    public class SqlHelper
    {
        private SqlHelper() { }

        public SqlHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// ERP数据库连接字符串
        /// </summary>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// 执行查询语句返回结果集合的第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if(parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 执行参数化SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行读取操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> ExecuteReader(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            Dictionary<string, string> dict = new Dictionary<string, string>();
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                dict.Add(dataReader.GetName(i), dataReader[dataReader.GetName(i)].ToString());
                            }
                            result.Add(dict);
                        }
                    }
                }

                return result;
            }
        }
    }
}
