using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

namespace GamerReturn
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required),ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class Service1 : IService1
    {

        SqlHelper _helper = new SqlHelper(); //数据库操作对象

        public string GetData(string name)
        {
            return string.Format("You name is: {0}", name);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        /// <summary>
        /// 验证登陆用户是否符合领取奖励的资格
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PartnerId"></param>
        /// <returns></returns>
        public ReturnMessageBody IsOldGamer(string UserId, string PartnerId)
        {
            //构建标准的响应回复数据结构对象

            ReturnMessageBody _return = new ReturnMessageBody();

            //从缓存中获取结果,如果有结果，可省去数据库交互过程，提高效率
            Object cacheobject=CacheHelper.GetCache(UserId + "@LoginTime");

            if (cacheobject != null)
            {
                //获取用户上一次登陆时间
                DateTime _lastlogintime = (DateTime)cacheobject;

                bool IsCan = false;
                //判断登陆时间是否满足条件
                if ((DateTime.Now - _lastlogintime).TotalDays > 30)
                {
                    IsCan = true;
                }

                if (IsCan)
                {
                    //回复可以领取的消息
                    CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 3600);                
                    _return.Status = "Success";
                    _return.Msg = "成功";
                    _return.Data= "这个用户可以参加活动!";
                    return _return;
                
                }
                else
                {
                    //回复不可以领取的消息
                    CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now,3600);
                    _return.Status = "Success";
                    _return.Msg= "失败";
                    _return.Data = "这个用户不可以参加活动!";
                    return _return;

                }
            }
            
            try
            {
                //数据库参数对象列表,防止SQL注入
                Dictionary<string, string> pairs = new Dictionary<string, string>(); 

                //查询用户上次登陆时间的SQL语句
                string sql = "select * from userinfo where UserId=@UserId and PartnerId=@PartnerId and UserId not in(select UserId from rewordlog where UserId=@UserId) order by LoginTime DESC ";

                pairs.Add("UserId", UserId);
                pairs.Add("PartnerId", PartnerId);

                DataTable table = _helper.MysqlDataAdapterWithSqlAndParameter(sql, pairs);

                bool IsCan = false;

                if (table.Rows.Count > 0)
                {
                    //获取用户上一次登陆时间
                    DateTime _lastlogintime = (DateTime)(table.Rows[0]["LoginTime"]);

                    if ((DateTime.Now - _lastlogintime).TotalDays > 30)
                    {
                        IsCan = true;
                    }

                    if (IsCan)
                    {
                        //回复可以领取的消息
                        CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 3600);        
                        _return.Status = "Success";
                        _return.Msg= "成功";
                        _return.Data = "这个用户可以参加活动!";
                       
                    }
                    else
                    {
                        //回复不可以领取的消息
                        CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 3600);
                        _return.Status= "Success";
                        _return.Msg= "失败";
                        _return.Data = "这个用户不可以参加活动!";
                        
                    }
                }
                else
                {
                    //未查询到用户登陆信息，回复用户不存在的信息
                    _return.Status = "Success";
                    _return.Msg= "失败";
                    _return.Data = "用户不存在!";
                 
                }
            }
            catch (Exception ex)
            {
                //接口内部出现错误，回复系统出现错误
                _return.Status = "Failed";
                _return.Msg= "错误";
                _return.Data = "系统出错!请稍后再试";
              
            }

            return _return;

        }


        /// <summary>
        /// 用户确认领取奖励，领取成功后就不能再次领取
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PartnerId"></param>
        /// <returns></returns>
        public ReturnMessageBody GetReward(string UserId, string PartnerId)
        {
            //构建标准的响应回复数据结构对象
            ReturnMessageBody _return = new ReturnMessageBody();

            //从缓存中获取结果,如果有结果，可省去数据库交互过程，提高效率            
            Object cacheobject = CacheHelper.GetCache(UserId + "@ISGetReword");

            if (cacheobject != null)
            {
                //获取缓存中保存的是否领取的结果
                bool result = (bool)cacheobject;

                if (result)
                {
                    //回复已经领取不能重复领取的消息
                    _return.Status = "Success";
                    _return.Msg= "失败";
                    _return.Data = "已经领取过奖励,不能重复领取!";
                    return _return;
                 
                }
            }

            try
            {

                //在缓存中将此用户设置为已经领取的状态，防止其他访问进程重复领取此用户的奖励。
                CacheHelper.SetCache(UserId + "@ISGetReword", true,3600);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                //插入此用户的领取记录
                string sql = "INSERT INTO rewordlog (UserId,PartnerId,GetRewordTime) VALUES (@UserId,@PartnerId,NOW())";

                pairs.Add("UserId", UserId);
                pairs.Add("PartnerId", PartnerId);

                if (_helper.MysqlCommandWithSqlAndParameter(sql, pairs) > 0)
                {
                   //成功，则返回成功领取的消息
                    _return.Status = "Success";
                    _return.Msg= "成功";
                    _return.Data = "成功领取奖励!";
                   
                }
                else
                {
                    //失败，则先将缓存中用户的领取状态重新设置为未领取，然后返回领取失败的消息
                    CacheHelper.SetCache(UserId + "@ISGetReword", false,3600);
                    _return.Status = "Success";
                    _return.Msg= "失败";
                    _return.Data = "领取奖励失败!";
                   
                }
            }
            catch (Exception ex)
            {
                //系统出错，则先将缓存中用户的领取状态重新设置为未领取，然后返回系统出错的消息
                CacheHelper.SetCache(UserId + "@ISGetReword", false, 3600);
                _return.Status = "Failed";
                _return.Msg= "错误";
                _return.Data = "系统出错!请稍后再试";
              
            }

            return _return;
        }
    }
}
