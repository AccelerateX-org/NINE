using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UnionController : BaseController
    {
        /// <summary>
        /// Übersicht der Fachschaft
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            ActivityOrganiser union = null;

            if (id != null)
            {
                union = GetOrganiser(id.Value);
            }
            else
            {
                var org = GetMyOrganisation();

                if (org == null)
                    return View("NoOrg");

                var unionName = org.ShortName.Replace("FK", "FS");
                union = GetOrganiser(unionName);

                ViewBag.Faculty = org;
            }

            if (union == null)
                return View("NoUnion");

            var userRight = GetUserRight(union);

            ViewBag.Union = union;
            ViewBag.UserRight = userRight;

            if (userRight.IsSysAdmin)
                return View();

            var member = GetMember(AppUser.UserName, union.ShortName);
            if (member == null)
                return View("NoMember");

            // Fälle
            // Ich bin Admin        => Startseite mit Adminbereich
            // Ich bin Mitglied     => Startseite ohne Adminbereich
            // Ich habe angefragt   => No Entry
            if (member.IsAssociated)
            {
                return View("NoEntry");
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Member(Guid id)
        {
            var union = GetOrganiser(id);
            var userRight = GetUserRight(union);

            ViewBag.UserRight = userRight;
            ViewBag.Semester = SemesterService.GetSemester(DateTime.Today);

            return View(union);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Newsletter(Guid id)
        {
            var union = GetOrganiser(id);
            var userRight = GetUserRight(union);

            ViewBag.UserRight = userRight;
            ViewBag.Semester = SemesterService.GetSemester(DateTime.Today);
            ViewBag.Union = union;

            var newsletters = Db.Activities.OfType<Newsletter>().Where(x => x.Organiser.Id == union.Id).ToList();



            return View(newsletters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.Id == id);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var members = organiser.Members.OrderBy(m => m.Name);
            var myUser = UserManager.FindByName(User.Identity.Name);

            foreach (var member in members)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var org = member.Organiser;

                org.Members.Remove(member);
                Db.Members.Remove(member);

                Db.SaveChanges();

                return RedirectToAction("Member", new {id = org.Id});
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var model = new MemberUserViewModel
                {
                    MemberId = id,
                    Role = member.Role,
                    Description = member.Description,
                    UrlProfile = member.UrlProfile,
                    Name = member.Name,
                    ShortName = member.ShortName,
                };

                if (!string.IsNullOrEmpty(member.UserId))
                {
                    var user = UserManager.FindById(member.UserId);
                    if (user != null)
                    {
                        model.UserName = user.UserName;
                    }
                }

                ViewBag.Member = member;

                return View(model);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMember(MemberUserViewModel model)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == model.MemberId);

            if (member != null)
            {
                //member.IsAdmin = model.IsAdmin;
                member.Role = model.Role;

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Details", new {id = member.Organiser.Id});
            }

            return RedirectToAction("Index", "Students");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetMember(Guid id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.Id == id);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Benutzer als neues Mitglied eintragen
            var user = AppUser;
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            // nach dem Benutzer suchen
            var member = organiser.Members.FirstOrDefault(m => m.UserId.Equals(user.Id));
            if (member == null)
            {

                member = new OrganiserMember
                {
                    IsAdmin = false,
                    UserId = user.Id,
                    IsAssociated = true // "Gast"
                };

                organiser.Members.Add(member);
            }
            // Den Token generieren
            var token = System.Web.Security.Membership.GeneratePassword(8, 2);
            member.ShortName = token;

            Db.SaveChanges();

            // Eine E-Mail senden
            var mailModel = new OrgMemberMailModel
            {
                User = user,
                Organiser = organiser,
                Token = token,
            };

            new MailController().RegisterUnionEMail(mailModel).Deliver();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ConfirmMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            return View(member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmMember(Guid id, string token)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);
            var org = member.Organiser;

            if (member.ShortName.Equals(token))
            {
                member.ShortName = string.Empty;
                member.IsAssociated = false;

                Db.SaveChanges();
            }


            return RedirectToAction("Member", new {id = org.Id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddAdmin(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);
            var org = member.Organiser;

            member.IsAdmin = true;
            member.IsAssociated = false;
            Db.SaveChanges();

            return RedirectToAction("Member", new { id = org.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveAdmin(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);
            var org = member.Organiser;

            member.IsAdmin = false;
            Db.SaveChanges();

            return RedirectToAction("Member", new { id = org.Id });

        }


    }
}
