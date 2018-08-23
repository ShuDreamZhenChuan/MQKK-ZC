// ******************************************************
// 文件名（FileName）:               GameReturnService.svc.cs  
// 功能描述（Description）:          此文件用户实现GameReturn.svc服务的接口响应
// 数据表（Tables）:                 nothing
// 作者（Author）:                   ZhenChuan
// 日期（Create Date）:              2018-08-07
// 修改记录（Revision History）:     
// ******************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using Moqikaka.GamerReturn.DAL;
using Moqikaka.GamerReturn.Model;

namespace Moqikaka.GamerReturn
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required),ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class ReturnRewordService : IReturnRewordService
    {
        LoginInfoDAL loginInfoDal = new LoginInfoDAL();  //登录信息操作对象
        RewordDAL rewordDal = new RewordDAL();           //奖励信息操作对象
        int rewordDayNum=30;                             //领取奖励的天数条件
        Mutex mutex = new Mutex();

        /// <summary>
        /// 获取示例数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetData(string name)
        {
            return string.Format("You name is: {0}", name);
        }

        /// <summary>
        /// 验证登陆用户是否符合领取奖励的资格
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public ReturnMessageBody IsOldGamer(string userId, string partnerId)
        {
            //构建标准的响应回复数据结构对象
            ReturnMessageBody returnMsg = new ReturnMessageBody();
              
            //从缓存中获取结果,如果有结果，可省去数据库交互过程，提高效率
            Object cacheobject=CacheHelper.GetCache(userId + "@LoginTime");

            if (cacheobject != null)
            {
                //用户上一次的登陆时间
                DateTime lastLoginTime = (DateTime)cacheobject;

                bool IsCan = false;
              
                //判断登陆时间是否满足条件
                if ((DateTime.Now - lastLoginTime).TotalDays > rewordDayNum)
                {
                    //如果已有领取记录则无法领取
                    if (rewordDal.GetRewordLog(userId, partnerId).Count > 0)
                        IsCan = false;
                    else
                        IsCan = true;
                }
                if (IsCan)
                {
                   
                    CacheHelper.SetCache(userId + "@LoginTime", DateTime.Now, 3600);
                    
                    //回复可以领取的消息
                    returnMsg.Status = "Success";
                    returnMsg.Msg = "成功";
                    returnMsg.Data= "这个用户可以参加活动!";
                    return returnMsg;
                }
                else
                {
                   
                    CacheHelper.SetCache(userId + "@LoginTime", DateTime.Now,3600);
                    
                    //回复不可以领取的消息
                    returnMsg.Status = "Success";
                    returnMsg.Msg= "失败";
                    returnMsg.Data = "这个用户不可以参加活动!";
                    return returnMsg;
                }
            }    
            try
            {
                //获取登录记录
                List<LoginInfo> logininfolist=loginInfoDal.GetLoginInfo(userId, partnerId);

                if (logininfolist.Count> 0)
                {
                    bool IsCan = false;
                    //获取用户上一次登陆时间
                    DateTime lastLoginTime = logininfolist[0].LoginTime;

                    //判断条件
                    if ((DateTime.Now - lastLoginTime).TotalDays > rewordDayNum)
                    {
                        IsCan = true;
                    }
                    if (IsCan)
                    {
                        CacheHelper.SetCache(userId + "@LoginTime", DateTime.Now, 3600);
                        
                        //回复可以领取的消息
                        returnMsg.Status = "Success";
                        returnMsg.Msg= "成功";
                        returnMsg.Data = "这个用户可以参加活动!";           
                    }
                    else
                    {
                       
                        CacheHelper.SetCache(userId + "@LoginTime", DateTime.Now, 3600);
                        
                        //回复不可以领取的消息
                        returnMsg.Status= "Success";
                        returnMsg.Msg= "失败";
                        returnMsg.Data = "这个用户不可以参加活动!";       
                    }
                }
                else
                {
                    //未查询到用户登陆信息，回复用户不存在的信息
                    returnMsg.Status = "Success";
                    returnMsg.Msg= "失败";
                    returnMsg.Data = "用户不存在!";                 
                }
            }
            catch (Exception ex)
            {
                //接口内部出现错误，回复系统出现错误
                returnMsg.Status = "Failed";
                returnMsg.Msg= "错误";
                returnMsg.Data = "系统出错!请稍后再试";       
            }
            return returnMsg;
        }

        /// <summary>
        /// 用户确认领取奖励，领取成功后就不能再次领取
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public ReturnMessageBody GetReward(string userId, string partnerId)
        {
            //修改了一个BUG
            //又修改了一个BUG
            //增加了一个新功能

            //构建标准的响应回复数据结构对象
            ReturnMessageBody returnMsg = new ReturnMessageBody();
            try
            {
                //线程同步处理,同一时间只运行一个线程执行以下过程
                mutex.WaitOne();
                //从缓存中获取结果,如果有结果，可省去数据库交互过程，提高效率       
                Object cacheobject = CacheHelper.GetCache(userId + "@ISGetReword");

                if (cacheobject != null)
                {
                    //获取缓存中保存的是否领取的结果
                    bool result = (bool)cacheobject;
                    if (result)
                    {
                        //回复已经领取不能重复领取的消息
                        returnMsg.Status = "Success";
                        returnMsg.Msg = "失败";
                        returnMsg.Data = "已经领取过奖励,不能重复领取!";
                        mutex.ReleaseMutex();
                        return returnMsg;
                    }
                }
                else
                {
                    if (rewordDal.GetRewordLog(userId, partnerId).Count > 0)
                    {
                        //回复已经领取不能重复领取的消息
                        returnMsg.Status = "Success";
                        returnMsg.Msg = "失败";
                        returnMsg.Data = "已经领取过奖励,不能重复领取!";
                        mutex.ReleaseMutex();
                        return returnMsg;
                    }
                    else
                    {
                        List<LoginInfo> logininfolist = loginInfoDal.GetLoginInfo(userId, partnerId);
                        if (logininfolist.Count > 0)
                        {
                            bool IsCan = false;

                            //获取用户上一次登陆时间
                            DateTime lastLoginTime = logininfolist[0].LoginTime;
                            if ((DateTime.Now - lastLoginTime).TotalDays > 30)
                            {
                                IsCan = true;
                            }
                            if (!IsCan)
                            {
                                CacheHelper.SetCache(userId + "@LoginTime", DateTime.Now, 3600);

                                //回复不可以领取的消息
                                returnMsg.Status = "Success";
                                returnMsg.Msg = "失败";
                                returnMsg.Data = "这个用户不可以领取奖励!";
                                mutex.ReleaseMutex();
                                return returnMsg;
                            }
                        }
                    }
                }
                //在缓存中将此用户设置为已经领取的状态，防止其他访问进程重复领取此用户的奖励。
                CacheHelper.SetCache(userId + "@ISGetReword", true, 3600);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
            try
            {
                if (rewordDal.GetReword(userId, partnerId))
                {
                    //成功，则返回成功领取的消息
                    returnMsg.Status = "Success";
                    returnMsg.Msg = "成功";
                    returnMsg.Data = "成功领取奖励!";
                }
                else
                {
                    //失败，则先将缓存中用户的领取状态重新设置为未领取，然后返回领取失败的消息
                    CacheHelper.SetCache(userId + "@ISGetReword", false, 3600);

                    returnMsg.Status = "Success";
                    returnMsg.Msg = "失败";
                    returnMsg.Data = "领取奖励失败!";
                }
            }
            catch (Exception ex)
            {
                //系统出错，则先将缓存中用户的领取状态重新设置为未领取，然后返回系统出错的消息
                CacheHelper.SetCache(userId + "@ISGetReword", false, 3600);

                returnMsg.Status = "Failed";
                returnMsg.Msg = "错误";
                returnMsg.Data = "系统出错!请稍后再试";
            }
           

            return returnMsg;
        }
    }
}
