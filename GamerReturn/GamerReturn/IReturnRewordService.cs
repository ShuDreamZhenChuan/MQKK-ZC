using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Moqikaka.GamerReturn.Model;

namespace Moqikaka.GamerReturn
{
    [ServiceContract]
    
    public interface IReturnRewordService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "getuser/{name}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetData(string name);

        /// <summary>
        /// 验证登陆用户是否符合领取奖励的资格
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        [OperationContract]

        [WebInvoke(Method = "POST",
            UriTemplate = "isoldgamer",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReturnMessageBody IsOldGamer(string userId,string partnerId);

        /// <summary>
        /// 用户确认领取奖励，领取成功后就不能再次领取
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        [OperationContract]

        [WebInvoke(Method = "POST",
           UriTemplate = "getreward",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        ReturnMessageBody GetReward(string userId, string partnerId);

    }

    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
