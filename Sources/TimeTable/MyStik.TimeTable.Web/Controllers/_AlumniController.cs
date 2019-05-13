using MyStik.TimeTable.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class _AlumniController : BaseController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            IEnumerable<ApplicationUser> model = new List<ApplicationUser>();

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ApplicationUser model = new ApplicationUser()
            {
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSingle(ApplicationUser model)
        {
            // ToDo:
            // - Was wenn der User schon Alumni ist?
            ApplicationUser user = _db.Users.SingleOrDefault(u => 
                                                       u.FirstName.ToUpper().Equals(model.FirstName.ToUpper()) &&
                                                       u.LastName.ToUpper().Equals(model.LastName.ToUpper()) &&
                                                       u.Email.ToUpper().Equals(model.Email.ToUpper())
                                                       );

            AlumniFeedbackViewModel ret = new AlumniFeedbackViewModel();

            if (user != null)
            {
                _db.SaveChanges();
                ret.success.Add(user);
            }
            else 
            {
                ret.error.Add(model);
                // Nach Vorschlägen suchen
                ret.suggestion = _db.Users.Where(u =>
                                           u.FirstName.ToUpper().Contains(model.FirstName.ToUpper()) ||
                                           u.LastName.ToUpper().Contains(model.LastName.ToUpper()) ||
                                           u.Email.ToUpper().Contains(model.Email.ToUpper())).ToList();
            }

            return PartialView("_AlumniFeedback", ret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMulti(AlumniCreateFromFileModel model)
        {
            HttpPostedFileBase File = model.File;
            AlumniFeedbackViewModel ret = new AlumniFeedbackViewModel();

            if (File != null && File.ContentLength > 0)
            {
                var path = Path.GetFullPath(File.FileName);
                List<string> lines = new List<string>();

                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    
                    while((line = sr.ReadLine()) != null) 
                    {
                        lines.Add(line);
                    }
                }

                foreach (string line in lines)
                {
                    string[] data = line.Split(';');
                    ApplicationUser user = new ApplicationUser()
                    {
                        LastName = data[0],
                        FirstName = data[1],
                        Email = data[2]
                    };

                    ApplicationUser match = _db.Users.SingleOrDefault(u => 
                                                                        u.FirstName.ToUpper().Equals(user.FirstName.ToUpper()) &&
                                                                        u.LastName.ToUpper().Equals(user.LastName.ToUpper()) &&
                                                                        u.Email.ToUpper().Equals(user.Email.ToUpper())
                                                                        );
                    if (match != null)
                    {
                        _db.SaveChanges();
                        ret.success.Add(user);
                    }
                    else
                    {
                        // Suche nach Alternativen
                        List<ApplicationUser> suggest = _db.Users.Where(u =>
                                                                    u.FirstName.ToUpper().Contains(user.FirstName.ToUpper()) ||
                                                                    u.LastName.ToUpper().Contains(user.LastName.ToUpper()) ||
                                                                    u.Email.ToUpper().Contains(user.Email.ToUpper())
                                                                    ).ToList();
                        ret.suggestion.AddRange(suggest);
                        ret.error.Add(user);
                    }
                }

            }

            return PartialView("_AlumniFeedbackiFrame", ret);
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(AlumniMailModel model)
        {
            model.subject = "Melden Sie sich im NINE an!";
            model.message = "Sehr geehrte Damen und Herren,\n\nbitte melden Sie sich unter http://nine.wi.hm.edu an, um aktuelle Informationen zu Alumni-Veranstaltungen zu erhalten.\n\nMit freundlichen Grüßen\n\nIhre Fakultät 09";
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviteSend(AlumniMailModel model)
        {
            CustomMailModel mailModel = new CustomMailModel();

            mailModel.Subject = model.subject;
            mailModel.Body = model.message.Replace("\n", "<br />");
            mailModel.SenderUser = UserManager.FindByName(User.Identity.Name);

            foreach(string email in model.mailTo) {
                ApplicationUser tempUser = new ApplicationUser();
                tempUser.Email = email;

                mailModel.ReceiverUsers.Add(tempUser);
            }

            try
            {
                new MailController().CustomTextEmail(mailModel).Deliver();
            }
            catch (Exception ex)
            {
                model.sendException = ex;
            }

            return PartialView("_AlumniMailSent", model);
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TakeSuggestion(TakeSuggestionModel model)
        {
            foreach (string ID in model.suggestedIDs)
            {
                ApplicationUser user = _db.Users.SingleOrDefault(u => u.Id.Equals(ID));
                _db.SaveChanges();
            }

            return View("SuggestionSuccess");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(string ID)
        {
            ApplicationUser user = _db.Users.SingleOrDefault(u => u.Id.Equals(ID));

            if (user != null)
            {
                _db.SaveChanges();
            }

            return View(user);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SingleMail(string id)
        {
            ApplicationUser user = _db.Users.SingleOrDefault(u => u.Id.Equals(id));

            AlumniMailModel model = new AlumniMailModel();

            if (user != null)
            {
                model.mailTo.Add(user.Email);
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupMail()
        {
            return View("Index");
        }
    }


}