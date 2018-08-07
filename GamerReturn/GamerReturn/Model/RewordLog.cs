using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moqikaka.GamerReturn.Model
{
    /// <summary>
    /// 领取奖励日志记录
    /// </summary>
    public class RewordLog
    {
        public string UserID { get; set; }

        public string PartnerID { get; set; }

        public DateTime GetRewordTime { get; set; }


    }
}