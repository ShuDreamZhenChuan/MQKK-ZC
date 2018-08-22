using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moqikaka.GamerReturn.Model
{
    /// <summary>
    /// 返回消息结构体
    /// </summary>
    public class ReturnMessageBody
    {
        /// <summary>
        /// 接口处理状态字段，表示接口内部是否正常处理用户的访问请求，并返回结果。
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 接口返回消息。
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 接口返回数据。
        /// </summary>
        public object Data { get; set; }
    }
}