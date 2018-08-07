using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moqikaka.GamerReturn.Model
{
    public class ReturnMessageBody
    {
        private string status;

        private string msg;

        private object data;

        /// <summary>
        /// 接口处理状态字段，表示接口内部是否正常处理用户的访问请求，并返回结果。
        /// </summary>
        public string Status { get => status; set => status = value; }
        /// <summary>
        /// 接口返回消息。
        /// </summary>
        public string Msg { get => msg; set => msg = value; }
        /// <summary>
        /// 接口返回数据。
        /// </summary>
        public object Data { get => data; set => data = value; }
    }
}