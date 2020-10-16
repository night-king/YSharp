using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Dm.Model.V20151123;
using Aliyun.Acs.Dm.Transform.Model.V20151123;

namespace Aliyun.Acs.Dm.Transform.V20151123
{
    public class BatchSendMailResponseUnmarshaller
    {
        public static BatchSendMailResponse Unmarshall(UnmarshallerContext context)
        {
            BatchSendMailResponse batchSendMailResponse = new BatchSendMailResponse();
            batchSendMailResponse.HttpResponse = context.HttpResponse;
            batchSendMailResponse.RequestId = context.StringValue("BatchSendMail.RequestId");
            batchSendMailResponse.EnvId = context.StringValue("BatchSendMail.EnvId");
            return batchSendMailResponse;
        }
    }
}

