using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GamerReturnTest
{
    class Program
    {

       
        static void Main(string[] args)
        {
             Console.WriteLine("GamerReturn接口测试开始！");

             Console.WriteLine("开始测试环境初始化>>>");

             InitTestData();

             Console.WriteLine("测试环境初始化成功!");

             Console.WriteLine("开始测试");

            List<object> testData = new List<object>();

            testData.Add("zhenchuan777:1");
            //testData.Add("zhenchuan777:1");
            //testData.Add("zhenchuan777:1");
            testData.Add("zhaosi5567:0");
            //testData.Add("zhaosi5567:0");
            //testData.Add("zhaosi5567:0");
            testData.Add("zhangsan123:0");
            //testData.Add("zhangsan123:0");
            //testData.Add("zhangsan123:0");


            //foreach (string value in testData)
            //{
            //    string[] ddd = value.Split(':');

            //    gamergetreword(ddd[0], ddd[1]);
            //}

            int num = 1000;
            RunMutiThread(
                // 执行并发测试代码 
                (object_p1) =>
                {
                    string ctx = object_p1 as string;

                    string[] ddd = ctx.Split(':');

                    gamergetreword(ddd[0], ddd[1]);

                    return true;

                },
                // 记录每次测试日志
                (begin_i, index_i, object_funcWork_Result) =>
                {
                    string ret = object_funcWork_Result as string;
                   
                },
                // 最大并发数目
                num,
                // 测试参数列表
                testData);
        }

    


        private static void gamergetreword(string UserId, string PartnerId)
        {
            Console.WriteLine(UserId+"用户登陆,查询是否可以领取回归奖励");

            string parameterjson = "{\"UserId\":\""+ UserId + "\",\"PartnerId\":\""+PartnerId+"\"}";

            string returnjson=HttpServerHelper.HttpPostRequestJson("isoldgamer", parameterjson, Encoding.UTF8);
           
            dynamic result=Newtonsoft.Json.JsonConvert.DeserializeObject(returnjson);

            if ((string)(result["Status"]) == "Success")
            {
                if ((string)(result["Data"]) == "这个用户可以参加活动!")
                {
                     Console.WriteLine("用户" + UserId + "可以领取回归奖励");

                    returnjson = HttpServerHelper.HttpPostRequestJson("getreward", parameterjson, Encoding.UTF8);

                    result = Newtonsoft.Json.JsonConvert.DeserializeObject(returnjson);

                    if ((string)(result["Status"]) == "Success")
                    {
                         Console.WriteLine("用户" + UserId + result["Data"]);
                    }
                }
                else if((string)(result["Data"]) == "这个用户不可以参加活动!")
                {
                     Console.WriteLine("用户" + UserId + "没有领取回归奖励的资格");

                }
            }
        }



        private static void InitTestData()
        {
            SqlHelper _helper = new SqlHelper();

            string sql = "delete from rewordlog ";

            _helper.MysqlCommand(sql);

        }



        static void RunMutiThread(
           Func<object, object> func_work, // 并发测试函数<传入参数，返回结果>
           Action<DateTime, int, object> func_log, // 日志函数<本次开始时间，本次顺序编号，本次测试执行结果>
           int num, // 理论最大并发数量
           List<object> data_list) // 参数列表
        {
            object flag_lock = 1;
            int flag_num = 0;
            AutoResetEvent wait = new AutoResetEvent(false);

            DateTime begin = DateTime.Now;

            Random rd_root= new Random(999);
            //PublicLogger.Logger_PublicProxy.Info(string.Format("begin test: user's count ={0}", num.ToString("00000")));
            for (int i = 0; i <= num; i++)
            {
                int count = data_list.Count;


                Random rd = new Random(rd_root.Next(10000));

                int index_i = (int)(rd.Next(3));
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    // 获取本次参数
                    DateTime begin_i = DateTime.Now;
                    object data = data_list.Count > 1 ? data_list[index_i] : data_list[0];

                    try
                    {
                        #region more code
                        // 执行并发业务
                        object result = func_work.Invoke(data);
                        // 记录并发日志
                        func_log.Invoke(begin_i, index_i, result);

                        lock (flag_lock)
                        {
                            flag_num++;
                            Console.WriteLine("当前已执行并发任务数："+ flag_num);
                            if (flag_num == num)
                            {
                                // 所有并发子进程执行完毕
                                wait.Set();
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //PublicLogger.Logger_PublicProxy.Info(string.Format("{0}/{1} user to login, result = {2}"
                        //        , index_i.ToString("00000")
                        //        , num.ToString("00000")
                        //        , "Exception: " + ex.Message)
                        //        , begin_i);
                    }
                }, index_i);
            }

            // 等待并发线程结束
            wait.WaitOne();

            //PublicLogger.Logger_PublicProxy.Info(string.Format("end test: user's count ={0}\r\n", num.ToString("00000")), begin);
        }
    }
}
