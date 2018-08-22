using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Moqikaka.GamerReturn.Model
{
    class SqlHelper
    {
        static string mysqlConnection = "";  //数据库连接字符串

        /// <summary>
        /// 建立mysql连接
        /// </summary>
        public SqlHelper()
        {
            try
            {
                mysqlConnection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["sqlConnectionString"].ToString();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 传入参数建立SQL连接
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Database"></param>
        public SqlHelper(string IP, string userName, string passWord, string dataBase)
        {
            try
            {
                string connectionString = "datasource=" + IP + ";username=" + userName + ";password=" + passWord + ";database=" + dataBase + ";charset=gb2312";

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 执行sql语句无返回结果
        /// </summary>
        public int MysqlCommand(string MysqlCommand)
        {
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(SqlHelper.mysqlConnection))
                {
                    mysqlConnection.Open();
                    Console.WriteLine("MysqlConnection Opened.");
                    MySqlCommand mysqlCommand = new MySqlCommand(MysqlCommand, mysqlConnection);
                    return mysqlCommand.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("MySqlException Error:" + ex.ToString());

                if (Regex.IsMatch(ex.ToString(), ""))
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return -1;
        }

        /// <summary>
        /// 传入参数执行SQL语句无返回数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public int MysqlCommandWithSqlAndParameter(string sql, Dictionary<string, string> pairs)
        {
            using (MySqlConnection mysqlConnection = new MySqlConnection(SqlHelper.mysqlConnection))
            {
                try
                {
                    mysqlConnection.Open();
                    Console.WriteLine("MysqlConnection Opened.");
                    MySqlCommand mysqlCommand = new MySqlCommand(sql, mysqlConnection);
                    foreach (string key in pairs.Keys)
                    {
                        mysqlCommand.Parameters.AddRange(new MySqlParameter[] { new MySqlParameter(key, MySqlDbType.VarChar) { Value = pairs[key] } });
                    }
                    return mysqlCommand.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("MySqlException Error:" + ex.ToString());
                    if (Regex.IsMatch(ex.ToString(), ""))
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 执行select 语句返回执行结果
        /// </summary>
        public DataView MysqlDataAdapter(string table)
        {
            DataView dataView = new DataView();
            using (MySqlConnection mysqlConnection = new MySqlConnection(SqlHelper.mysqlConnection))
            {
                try
                {
                    mysqlConnection.Open();
                    MySqlDataAdapter mysqlDataAdapter = new MySqlDataAdapter("Select * from " + table, mysqlConnection);
                    DataSet dataSet = new DataSet();
                    mysqlDataAdapter.Fill(dataSet, table);
                    dataView = dataSet.Tables[table].DefaultView;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return dataView;
        }

        /// <summary>
        /// 传入参数执行select 句返回执行结果
        /// </summary>
        public DataTable MysqlDataAdapterWithSqlAndParameter(string sql, Dictionary<string, string> pairs)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection mysqlConnection = new MySqlConnection(SqlHelper.mysqlConnection))
            {
                try
                {
                    mysqlConnection.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = mysqlConnection;
                    command.CommandText = sql;
                    foreach (string key in pairs.Keys)
                    {
                        command.Parameters.AddRange(new MySqlParameter[] { new MySqlParameter(key, MySqlDbType.VarChar) { Value = pairs[key] } });
                    }
                    MySqlDataAdapter mysqlDataAdapter = new MySqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    mysqlDataAdapter.Fill(dataSet);
                    dataTable = dataSet.Tables[0];
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }         
            return dataTable;
        }

        /// <summary>
        /// 统计记录个数 参数：select count(*) from isns_users
        /// </summary>
        public long MysqlCountRow(string sql)
        {
            DataView dataView = new DataView();
            using (MySqlConnection mysqlConnection = new MySqlConnection(SqlHelper.mysqlConnection))
            {
                try
                {
                    mysqlConnection.Open();
                    MySqlCommand mycm = new MySqlCommand(sql, mysqlConnection);
                    // MySqlDataReader msdr = mycm.ExecuteReader();
                    long recordCount = (long)mycm.ExecuteScalar();
                    return recordCount;
                }
                catch (MySqlException)
                {
                    return -1;
                    // Console.WriteLine(ex.Message);
                }              
            }        
        }

        /// <summary>
        ///  将数据表查询对象封装成具体的数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertModel<T>(DataTable dt) where T : new()
        {
            List<T> models = new List<T>();
            T model = new T();
            Type t = model.GetType();
            int j = 0;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    model = new T();
                    foreach (PropertyInfo mi in t.GetProperties())
                    {

                        if ((j = dt.Columns.IndexOf(mi.Name)) > -1 && !(dt.Rows[i][j] is DBNull) && dt.Rows[i][j].ToString() != "")
                        {
                            Object obj = dt.Rows[i][j];
                            if (mi.PropertyType == dt.Columns[j].DataType)
                            {
                                mi.SetValue(model, obj, null);
                            }
                            else//model中有数据库中nullable类型的数据，需要转换后才能正确赋值，
                                //解决 从“System.*”到“System.Nullable`1[[System.*, mscorlib, ..]的强制转换无效。
                            {
                                if (mi.PropertyType.FullName.Contains("System.Decimal"))
                                {
                                    mi.SetValue(model, Convert.ToDecimal(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.DateTime"))
                                {
                                    mi.SetValue(model, Convert.ToDateTime(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.String"))
                                {
                                    mi.SetValue(model, Convert.ToString(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.Int32"))
                                {
                                    mi.SetValue(model, Convert.ToInt32(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.Int64"))
                                {
                                    mi.SetValue(model, Convert.ToInt64(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.Single"))
                                {
                                    mi.SetValue(model, Convert.ToSingle(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.Double"))
                                {
                                    mi.SetValue(model, Convert.ToDouble(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.Char"))
                                {
                                    mi.SetValue(model, Convert.ToChar(obj), null);
                                }
                                else if (mi.PropertyType.FullName.Contains("System.Boolean"))
                                {
                                    mi.SetValue(model, Convert.ToBoolean(obj), null);
                                }

                            }
                        }
                        if (j < 0)//没有获取此字段的数据，调试用
                        {

                        }
                    }
                    models.Add(model);
                }
            }
            catch
            {
                throw new Exception("属性包含不支持的数据类型!");
            }
            return models;
        }
    }
}


