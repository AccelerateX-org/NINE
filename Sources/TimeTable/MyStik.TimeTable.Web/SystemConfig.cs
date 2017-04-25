using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace MyStik.TimeTable.Web
{
    public class SystemConfig
    {
        public SystemConfig()
        {
            this.HasSystemWarning = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.system.warning"]);
            this.SystemWarning = System.Configuration.ConfigurationManager.AppSettings["nine.system.warning"];
        }


        public bool HasSystemWarning { get; private set; }

        public string SystemWarning { get; private set; }

        public int MailMaxReceiverCount
        {
            get
            {
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.mail.maxBcc"]))
                    return 1;
                    
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["nine.mail.maxBcc"]);
            }
        }


        public bool MailSubscriptionEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.mail.subscription.disabled"]))
                    return true;
                return false;
            }
        }

        public bool IsLotteryEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.lottery"]))
                    return false;

                bool val;
                return bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["nine.lottery"], out val) && val;
            }
        }

        public SmtpSection GetSmtpSettings()
        {
            return (SmtpSection)System.Configuration.ConfigurationManager.GetSection("system.net/mailsettings/smtp");
        }

        public void SaveSmtpSetting(SmtpSection section)
        {
            
        }
    }
}