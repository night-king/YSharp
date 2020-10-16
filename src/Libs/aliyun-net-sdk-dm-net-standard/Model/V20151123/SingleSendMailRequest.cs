using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Core.Utils;
using Aliyun.Acs.Dm.Transform.Transform.V20151123;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.Acs.Dm.Transform.Model.V20151123
{

    public class SingleSendMailRequest : RpcAcsRequest<SingleSendMailResponse>
    {
        private long? ownerId;

        private string resourceOwnerAccount;

        private long? resourceOwnerId;

        private string accountName;

        private int? addressType;

        private string tagName;

        private bool? replyToAddress;

        private string toAddress;

        private string subject;

        private string htmlBody;

        private string textBody;

        private string fromAlias;

        private string replyAddress;

        private string replyAddressAlias;

        private string clickTrace;

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

        public bool? ReplyToAddress
        {
            get
            {
                return replyToAddress;
            }
            set
            {
                replyToAddress = value;
                DictionaryUtil.Add(base.QueryParameters, "ReplyToAddress", value.ToString());
            }
        }

        public string ToAddress
        {
            get
            {
                return toAddress;
            }
            set
            {
                toAddress = value;
                DictionaryUtil.Add(base.QueryParameters, "ToAddress", value);
            }
        }

        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
                DictionaryUtil.Add(base.QueryParameters, "Subject", value);
            }
        }

        public string HtmlBody
        {
            get
            {
                return htmlBody;
            }
            set
            {
                htmlBody = value;
                DictionaryUtil.Add(base.QueryParameters, "HtmlBody", value);
            }
        }

        public string TextBody
        {
            get
            {
                return textBody;
            }
            set
            {
                textBody = value;
                DictionaryUtil.Add(base.QueryParameters, "TextBody", value);
            }
        }

        public string FromAlias
        {
            get
            {
                return fromAlias;
            }
            set
            {
                fromAlias = value;
                DictionaryUtil.Add(base.QueryParameters, "FromAlias", value);
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

        public SingleSendMailRequest()
            : base("Dm", "2015-11-23", "SingleSendMail")
        {
            base.Method = MethodType.POST;
        }

        public override SingleSendMailResponse GetResponse(UnmarshallerContext unmarshallerContext)
        {
            return SingleSendMailResponseUnmarshaller.Unmarshall(unmarshallerContext);
        }
    }
}
