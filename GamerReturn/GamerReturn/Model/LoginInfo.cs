using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moqikaka.GamerReturn.Model
{
    public class LoginInfo
    {
        public string UserId { get; set; }      //登录用户ID

        public string PartnerId { get; set; }   //合作商ID

        public string PlayerId { get; set; }    //角色ID

        public DateTime LoginTime { get; set; }   //登录时间
    }
}
