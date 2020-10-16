using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Core.Utils;
using Aliyun.Acs.Dm.Transform.Model.V20151123;
using Aliyun.Acs.Dm.Transform.V20151123;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.Acs.Dm.Model.V20151123
{
    public class BatchSendMailRequest : RpcAcsRequest<BatchSendMailResponse>
    {
        private long? ownerId;

        private string resourceOwnerAccount;

        private long? resourceOwnerId;

        private string templateName;

        private string accountName;

        private string receiversName;

        private int? addressType;

        private string tagName;

        private string replyAddress;

        private string replyAddressAlias;

        private string clickTrace;

        public long? OwnerId
        {
            get
            {
                return ownerId;
            }
            set
            {
                ownerId = value;
                DictionaryUtil.Add(base.QueryParameters, "OwnerId", value.ToString());
            }
        }

        public string ResourceOwnerAccount
        {
            get
            {
                return resourceOwnerAccount;
            }
            set
            {
                resourceOwnerAccount = value;
                DictionaryUtil.Add(base.QueryParameters, "ResourceOwnerAccount", value);
            }
        }

        public long? ResourceOwnerId
        {
            get
            {
                return resourceOwnerId;
            }
            set
            {
                resourceOwnerId = value;
                DictionaryUtil.Add(base.QueryParameters, "ResourceOwnerId", value.ToString());
            }
        }

        public string TemplateName
        {
            get
            {
                return templateName;
            }
            set
            {
                templateName = value;
                DictionaryUtil.Add(base.QueryParameters, "TemplateName", value);
            }
        }

        public string AccountName
        {
            get
            {
                return accountName;
            }
            set
            {
                accountName = value;
                DictionaryUtil.Add(base.QueryParameters, "AccountName", value);
            }
        }

        public string ReceiversName
        {
            get
            {
                return receiversName;
            }
            set
            {
                receiversName = value;
                DictionaryUtil.Add(base.QueryParameters, "ReceiversName", value);
            }
        }

        public int? AddressType
        {
            get
            {
                return addressType;
            }
            set
            {
                addressType = value;
                DictionaryUtil.Add(base.QueryParameters, "AddressType", value.ToString());
            }
        }

        public string TagName
        {
            get
            {
                return tagName;
            }
            set
            {
                tagName = value;
                DictionaryUtil.Add(base.QueryParameters, "TagName", value);
            }
        }

        public string ReplyAddress
        {
            get
            {
                return replyAddress;
            }
            set
            {
                replyAddress = value;
                DictionaryUtil.Add(base.QueryParameters, "ReplyAddress", value);
            }
        }

        public string ReplyAddressAlias
        {
            get
            {
                return replyAddressAlias;
            }
            set
            {
                replyAddressAlias = value;
                DictionaryUtil.Add(base.QueryParameters, "ReplyAddressAlias", value);
            }
        }

        public string ClickTrace
        {
            get
            {
                return clickTrace;
            }
            set
            {
                clickTrace = value;
                DictionaryUtil.Add(base.QueryParameters, "ClickTrace", value);
            }
        }

        public BatchSendMailRequest()
            : base("Dm", "2015-11-23", "BatchSendMail")
        {
            base.Method = MethodType.POST;
        }

        public override BatchSendMailResponse GetResponse(UnmarshallerContext unmarshallerContext)
        {
            return BatchSendMailResponseUnmarshaller.Unmarshall(unmarshallerContext);
        }
    }
}