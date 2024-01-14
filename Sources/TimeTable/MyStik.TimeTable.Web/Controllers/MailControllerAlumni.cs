using ActionMailer.Net.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{

    /// <summary>
    /// Hier werden Mails direkt versendet 
    /// </summary>
    public partial class MailController : MailerBase
    {
        /// <summary>
        /// Validierung einer E-Mail Adresse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult AlumniInvitationEMail(Alumnus alumnus)
        {
            InitSenderTopic(MAIL_SECTION_ALUMNI);

            To.Add(alumnus.Email);
            Subject = "Ihr Zugang zum Alumni-Portal";

            return Email("AlumniInvitationEMail", alumnus);
        }


    }
}