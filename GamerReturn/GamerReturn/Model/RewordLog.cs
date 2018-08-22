using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moqikaka.GamerReturn.Model
{
    /// <summary>
    /// 领取奖励日志记录数据实体
    /// </summary>
    public class RewordLog
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 合作商ID
        /// </summary>
        public string PartnerID { get; set; }

        /// <summary>
        /// 获取奖励时间
        /// </summary>
        public DateTime GetRewordTime { get; set; }


    }
}