using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Dm.Model.V20151123;
using Aliyun.Acs.Dm.Transform.Model.V20151123;

namespace Aliyun.Acs.Dm.Transform.Transform.V20151123
{
   public class SingleSendMailResponseUnmarshaller
    {
        public static SingleSendMailResponse Unmarshall(UnmarshallerContext context)
        {
            SingleSendMailResponse singleSendMailResponse = new SingleSendMailResponse();
            singleSendMailResponse.HttpResponse = context.HttpResponse;
            singleSendMailResponse.RequestId = context.StringValue("SingleSendMail.RequestId");
            singleSendMailResponse.EnvId = context.StringValue("SingleSendMail.EnvId");
            return singleSendMailResponse;
        }
    }
}
