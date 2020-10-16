using Aliyun.Acs.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aliyun.Acs.Dm.Transform.Model.V20151123
{
    public class SingleSendMailResponse : AcsResponse
    {
        private string envId;

        public string EnvId
        {
            get
            {
                return envId;
            }
            set
            {
                envId = value;
            }
        }
    }

}
