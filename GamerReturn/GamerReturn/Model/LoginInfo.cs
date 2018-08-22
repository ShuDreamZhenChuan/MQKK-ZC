using System;

namespace Moqikaka.GamerReturn.Model
{
    /// <summary>
    /// 登录信息数据实体
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }      

        /// <summary>
        /// 合作商ID
        /// </summary>
        public string PartnerId { get; set; } 

        /// <summary>
        /// 游戏角色ID
        /// </summary>
        public string PlayerId { get; set; }    

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }   
    }
}
