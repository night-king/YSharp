using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace YSharp.SDK.Helpers
{
    public class AliyunMailHelper
    {

        public string AliyunAccessKey { private set; get; }

        public string AliyunAccessKeySecret { private set; get; }

        public string AliyunMailAccount { private set; get; }

        public string AliyunMailDisplayName { private set; get; }

        public string AliyunMailSMTPPassword { private set; get; }
        public string AliyunMailReply { private set; get; }
        public AliyunMailHelper(string aliAccessKey, string aliAccessKeySecret, string aliMailAccount, string aliMailDisplayName, string aliMailSMTPPassword, string aliMailReply)
        {
            AliyunAccessKey = aliAccessKey;
            AliyunAccessKeySecret = aliAccessKeySecret;
            AliyunMailAccount = aliMailAccount;
            AliyunMailDisplayName = aliMailDisplayName;
            AliyunMailSMTPPassword = aliMailSMTPPassword;
            AliyunMailReply = aliMailReply;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="diaplayName"></param>
        /// <param name="ccList"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="linkUrl"></param>
        /// <param name="linkMessage"></param>
        /// <param name="errMsg"></param>
        /// <param name="bccList"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public bool SendEmail(string to, string diaplayName, IEnumerable<string> ccList, string subject, string content, out string errMsg,
                              IEnumerable<string> bccList = null, Dictionary<string, KeyValuePair<string, byte[]>> attachments = null)
        {
            bool result = true;
            var accessKey = AliyunAccessKey;
            var accessSecret = AliyunAccessKeySecret;
            var aliMailAccount = AliyunMailAccount;
            var aliMailDisPlayName = string.IsNullOrEmpty(diaplayName) ? AliyunMailDisplayName : diaplayName;
            var aliMailSMTPPassword = AliyunMailSMTPPassword;
            var aliMailReply = AliyunMailReply;

            try
            {
                MailMessage mailMsg = new MailMessage();
                mailMsg.To.Add(new MailAddress(to));
                mailMsg.From = new MailAddress(aliMailAccount, aliMailDisPlayName);
                //可选，设置回信地址 
                if (!string.IsNullOrEmpty(aliMailReply))
                {
                    var replies = aliMailReply.Split(new string[] { ",", "，", "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (replies.Length > 0)
                    {
                        foreach (var reply in replies)
                        {

                            mailMsg.ReplyToList.Add(reply);
                        }
                    }
                }
                // 邮件主题
                mailMsg.Subject = subject;
                // 邮件正文内容
                string html = content;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                // 添加附件
                var mss = new List<MemoryStream>();
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        var name = attachment.Key;
                        var ms = new MemoryStream(attachment.Value.Value);
                        mss.Add(ms);
                        mailMsg.Attachments.Add(new Attachment(ms, name, "application/pdf"));
                    }
                }


                try
                {
                    if (ccList != null && ccList.Count() > 0)
                    {
                        foreach (var email in ccList)
                        {
                            mailMsg.CC.Add(new MailAddress(email.Trim()));
                        }
                    }
                }
                catch { }

                try
                {
                    if (bccList != null && bccList.Count() > 0)
                    {
                        foreach (var email in bccList)
                        {
                            mailMsg.Bcc.Add(new MailAddress(email.Trim()));
                        }
                    }
                }
                catch { }
                //邮件推送的SMTP地址和端口
                //SmtpClient smtpClient = new SmtpClient("smtpdm.aliyun.com", 25);
                //C#官方文档介绍说明不支持隐式TLS方式，即465端口，需要使用25或者80端口(ECS不支持25端口)，另外需增加一行 smtpClient.EnableSsl = true; 故使用SMTP加密方式需要修改如下：
                SmtpClient smtpClient = new SmtpClient("smtpdm.aliyun.com", 80);
                smtpClient.EnableSsl = true;
                // 使用SMTP用户名和密码进行验证
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(aliMailAccount, aliMailSMTPPassword);
                smtpClient.Credentials = credentials;
                smtpClient.Send(mailMsg);
                if (mss.Count > 0)
                {
                    foreach (var ms in mss)
                    {
                        ms.Dispose();
                    }
                }
                errMsg = "OK";
                result = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                result = false;
            }
            return result;
        }

    }
}
