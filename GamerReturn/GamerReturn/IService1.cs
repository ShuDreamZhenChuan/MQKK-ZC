using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GamerReturn
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    
    public interface IService1
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "getuser/{name}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetData(string name);



        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: 在此添加您的服务操作


        /// <summary>
        /// 验证登陆用户是否符合领取奖励的资格
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PartnerId"></param>
        /// <returns></returns>
        [OperationContract]

        [WebInvoke(Method = "POST",
            UriTemplate = "isoldgamer",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReturnMessageBody IsOldGamer(string UserId,string PartnerId);



        /// <summary>
        /// 用户确认领取奖励，领取成功后就不能再次领取
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PartnerId"></param>
        /// <returns></returns>
        [OperationContract]

        [WebInvoke(Method = "POST",
           UriTemplate = "getreward",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        ReturnMessageBody GetReward(string UserId, string PartnerId);

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
