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

        SqlHelper _helper = new SqlHelper();

        

        

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


        public ReturnMessageBody IsOldGamer(string UserId, string PartnerId)
        {
            //标准的响应Json格式

            ReturnMessageBody _return = new ReturnMessageBody();

            //从缓存中获取结果
            Object cacheobject=CacheHelper.GetCache(UserId + "@LoginTime");

            if (cacheobject != null)
            {

                DateTime _lastlogintime = (DateTime)cacheobject;

                bool IsCan = false;

                if ((DateTime.Now - _lastlogintime).TotalDays > 30)
                {
                    IsCan = true;
                }

                if (IsCan)
                {
                    CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 0);

                    
                    _return.Status = "Success";
                    _return.Msg = "成功";
                    _return.Data= "这个用户可以参加活动!";
                
                }
                else
                {

                    CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 0);

                    _return.Status = "Success";
                    _return.Msg= "成功";
                    _return.Data = "这个用户不可以参加活动!";
                  
                }
            }
            
            try
            {

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                string sql = "select * from userinfo where UserId=@UserId and PartnerId=@PartnerId and UserId not in(select UserId from rewordlog where UserId=@UserId) order by LoginTime DESC ";

                pairs.Add("UserId", UserId);
                pairs.Add("PartnerId", PartnerId);

                DataTable table = _helper.MysqlDataAdapterWithSqlAndParameter(sql, pairs);

                bool IsCan = false;

                if (table.Rows.Count > 0)
                {
                    DateTime _lastlogintime = (DateTime)(table.Rows[0]["LoginTime"]);

                    if ((DateTime.Now - _lastlogintime).TotalDays > 30)
                    {
                        IsCan = true;
                    }

                    if (IsCan)
                    {
                        CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 0);

                       
                        _return.Status = "Success";
                        _return.Msg= "成功";
                        _return.Data = "这个用户可以参加活动!";
                       
                    }
                    else
                    {

                        CacheHelper.SetCache(UserId + "@LoginTime", DateTime.Now, 0);

                        _return.Status= "Success";
                        _return.Msg= "成功";
                        _return.Data = "这个用户不可以参加活动!";
                       
                    }
                }
                else
                {
                  
                    _return.Status = "Success";
                    _return.Msg= "成功";
                    _return.Data = "用户不存在!";
                 

                }
            }
            catch (Exception ex)
            {
                
                _return.Status = "Failed";
                _return.Msg= "错误";
                _return.Data = "系统出错!请稍后再试";
              
            }

            return _return;

        }

        public ReturnMessageBody GetReward(string UserId, string PartnerId)
        {
            ReturnMessageBody _return = new ReturnMessageBody();

            Object cacheobject = CacheHelper.GetCache(UserId + "@ISGetReword");

            if (cacheobject != null)
            {
                bool result = (bool)cacheobject;

                if (result)
                {
                    _return.Status = "Success";
                    _return.Msg= "成功";
                    _return.Data = "已经领取过奖励,不能那个重复领取!";
                 
                }
            }

            try
            {

                CacheHelper.SetCache(UserId + "@ISGetReword", true,0);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                string sql = "INSERT INTO rewordlog (UserId,PlayerId,GetRewordTime) VALUES (@UserId,@PartnerId,NOW())";

                pairs.Add("UserId", UserId);
                pairs.Add("PartnerId", PartnerId);

                if (_helper.MysqlCommandWithSqlAndParameter(sql, pairs) > 0)
                {
                   
                    _return.Status = "Success";
                    _return.Msg= "成功";
                    _return.Data = "成功参加活动!";
                   
                }
                else
                {
                    CacheHelper.SetCache(UserId + "@ISGetReword", false,0);

                  
                    _return.Status = "Failed";
                    _return.Msg= "失败";
                    _return.Data = "参加活动失败!";
                   
                }
            }
            catch (Exception ex)
            {
                CacheHelper.SetCache(UserId + "@ISGetReword", false, 0);

              
                _return.Status = "Failed";
                _return.Msg= "错误";
                _return.Data = "系统出错!请稍后再试";
              
            }

            return _return;

        }
    }
}
