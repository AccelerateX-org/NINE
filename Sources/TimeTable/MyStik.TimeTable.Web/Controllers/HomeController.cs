using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    //[CookieConsent]
    public class HomeController : BaseController
    {
        private HomeViewModel GetModel()
        {
            // wir reparieren
            var visbleFaculties = Db.Organisers.Where(x => x.IsVisible).OrderBy(x => x.ShortName).ToList();
            if (!visbleFaculties.Any())
            {
                var fk09 = Db.Organisers.FirstOrDefault(x => x.ShortName.Equals("FK 09"));
                if (fk09 != null)
                {
                    fk09.IsVisible = true;
                    fk09.HtmlColor = "#009B71";
                    fk09.SupportEMail = "itsupport@wi.hm.edu";
                    fk09.SupportUrl = "http://www.wi.hm.edu/mein_studium/stundenplaene/stundenplaene_nine.de.html";
                    Db.SaveChanges();
                }

                var fk10 = Db.Organisers.FirstOrDefault(x => x.ShortName.Equals("FK 10"));
                if (fk10 != null)
                {
                    fk10.IsVisible = true;
                    fk10.HtmlColor = "#008E7D";
                    fk10.SupportEMail = "";
                    fk10.SupportUrl = "http://www.bwl.hm.edu/";
                    Db.SaveChanges();
                }

                var fk11 = Db.Organisers.FirstOrDefault(x => x.ShortName.Equals("FK 11"));
                if (fk11 != null)
                {
                    fk11.IsVisible = true;
                    fk11.HtmlColor = "#EC7404";
                    fk11.SupportEMail = "";
                    fk11.SupportUrl = "http://www.sw.hm.edu/nine/nine_1.de.html";
                    Db.SaveChanges();
                }

                visbleFaculties = Db.Organisers.Where(x => x.IsVisible).ToList();
            }

            var model = new HomeViewModel();
            foreach (var faculty in visbleFaculties)
            {
                model.Faculties.Add(new FacultyViewModel
                {
                    Organiser = faculty
                });
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            var semester = GetSemester();

            var model = GetModel();

            model.Semester = semester;
            foreach (var faculty in model.Faculties)
            {
                faculty.StudentCount = Db.Subscriptions.OfType<SemesterSubscription>().Count(x =>
                    x.SemesterGroup.Semester.Id == semester.Id &&
                    x.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == faculty.Organiser.Id);
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult PrivacyStatement()
        {
            var model = GetModel();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Contact(ContactMailModel model)
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Send(model.Email, "hinz@hm.edu", "[fillter] " + model.Subject, model.Body);

            return View("ThankYou");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Missing()
        {
            return View();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Imprint()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult TestLab()
        {
            return View();
        }
    }
}