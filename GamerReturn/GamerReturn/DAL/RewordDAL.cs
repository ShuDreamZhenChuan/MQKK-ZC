// ******************************************************
// 文件名（FileName）:               RewordDAL.cs  
// 功能描述（Description）:          此文件用户处理用户领取奖励相关的数据库交互
// 数据表（Tables）:                 rewordlog;
// 作者（Author）:                   ZhenChuan
// 日期（Create Date）:              2018-08-07
// 修改记录（Revision History）:     
// ******************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Moqikaka.GamerReturn.DAL;
using Moqikaka.GamerReturn.Model;

namespace Moqikaka.GamerReturn.DAL
{
    /// <summary>
    /// 奖励信息数据库操作类
    /// </summary>
    public class RewordDAL
    {
        /// <summary>
        /// 获取用户领取奖励的日志
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PartnerId"></param>
        /// <returns></returns>
        public List<RewordLog> GetRewordLog(string userId, string partnerId)
        {
            SqlHelper helper = new SqlHelper();  //sql操作对象
            
            //查询用户领取奖励记录的SQL语句
            string sqlString= "select UserID,PartnerId,GetRewordTime from rewordlog where UserId=@UserId and PartnerId=@PartnerId ";

            //构建参数列表
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("UserId", userId);
            pairs.Add("PartnerId", partnerId);

            //执行查询并获得数据表
            DataTable table = helper.MysqlDataAdapterWithSqlAndParameter(sqlString, pairs);
            
            //返回结果
            List<RewordLog> infoList = SqlHelper.ConvertModel<RewordLog>(table);
            return infoList;
        }

        public bool GetReword(string userId, string partnerId)
        {
            SqlHelper helper = new SqlHelper();  //sql操作对象

            //插入此用户的领取记录
            string sqlString= "INSERT INTO rewordlog (UserId,PartnerId,GetRewordTime) VALUES (@UserId,@PartnerId,NOW())";

            //构建参数列表
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("UserId", userId);
            pairs.Add("PartnerId", partnerId);

            //插入记录并返回结果
            if (helper.MysqlCommandWithSqlAndParameter(sqlString, pairs) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}