using System.Net.Configuration;

namespace MyStik.TimeTable.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemConfig()
        {
            this.HasSystemWarning = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.system.warning"]);
            this.SystemWarning = System.Configuration.ConfigurationManager.AppSettings["nine.system.warning"];
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasSystemWarning { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string SystemWarning { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int MailMaxReceiverCount
        {
            get
            {
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.mail.maxBcc"]))
                    return 1;
                    
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["nine.mail.maxBcc"]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool MailSubscriptionEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["nine.mail.subscription.disabled"]))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SmtpSection GetSmtpSettings()
        {
            return (SmtpSection)System.Configuration.ConfigurationManager.GetSection("system.net/mailsettings/smtp");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        public void SaveSmtpSetting(SmtpSection section)
        {
            
        }
    }
}