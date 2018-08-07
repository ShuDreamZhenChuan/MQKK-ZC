// ******************************************************
// 文件名（FileName）:               LogininfoDAL.cs  
// 功能描述（Description）:          此文件用户处理用户登录信息的数据库交互
// 数据表（Tables）:                 userlogininfo;
// 作者（Author）:                   ZhenChuan
// 日期（Create Date）:              2018-08-07
// 修改记录（Revision History）:     
// ******************************************************
using Moqikaka.GamerReturn.Model;
using System.Collections.Generic;
using System.Data;

namespace Moqikaka.GamerReturn.DAL
{
    /// <summary>
    /// 登陆信息数据库操作类
    /// </summary>
    public class LoginInfoDAL
    {
        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns></returns>
        public List<LoginInfo> GetLoginInfo(string userId, string partnerId)
        {
            SqlHelper helper = new SqlHelper();
            
            //查询用户登陆记录的SQL语句
            string sqlString = "select * from userlogininfo where UserId=@UserId and PartnerId=@PartnerId order by LoginTime DESC ";

            //构建参数列表
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("UserId", userId);
            pairs.Add("PartnerId", partnerId);

            //执行查询并获得数据表
            DataTable table = helper.MysqlDataAdapterWithSqlAndParameter(sqlString, pairs);

            //返回结果
            List<LoginInfo>  infoList=SqlHelper.ConvertModel<LoginInfo>(table);
            return infoList;
        }
    }
}