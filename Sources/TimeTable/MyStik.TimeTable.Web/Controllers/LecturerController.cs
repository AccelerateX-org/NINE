using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LecturerController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new OrganiserViewModel();

            ViewBag.FacultyList = Db.Organisers.Where(o => o.IsFaculty && !o.IsStudent && o.Members.Any()).OrderBy(s => s.Name).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            ViewBag.UserRight = GetUserRight();

            model.Organiser = GetMyOrganisation();

            ViewBag.MenuId = "menu-lecturers";

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facultyId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Faculty(Guid facultyId)
        {
            var sem = GetSemester();

            var model = new List<LecturerViewModel>();

            var orgService = new OrganizerService(Db);
            var faculty = orgService.GetOrganiser(facultyId);

            // alle die einen termin haben, der zu einer aktuellen Semestergruppe gehört
            var activeLecturers = orgService.GetLecturers(faculty, sem);

            foreach (var lecturer in activeLecturers)
            {
                // Sprechstunde ggf. reparieren
                // gibt es eine Sprechstunde
                /*
                var oldOffieHours = 
                Db.Activities.OfType<OfficeHour>().Where(x =>
                    x.Semester.Id == sem.Id &&
                    x.Name.Equals("Sprechstunde") &&
                    x.ShortName.Equals(lecturer.ShortName) &&
                    x.Owners.All(m => m.Member.Id == lecturer.Id)).ToList();

                if (oldOffieHours.Any())
                {
                    if (oldOffieHours.Count > 1)
                    {
                        if (oldOffieHours.All(x => x.ByAgreement))
                        {
                            var toDelete = oldOffieHours.ToList();
                            var theOfficeHour = toDelete.First();
                            toDelete.Remove(theOfficeHour);
                            foreach (var officeHour in toDelete)
                            {
                                Db.Occurrences.Remove(officeHour.Occurrence);
                                Db.Activities.Remove(officeHour);
                            }

                            var owner = new ActivityOwner
                            {
                                Activity = theOfficeHour,
                                Member = lecturer
                            };
                            Db.ActivityOwners.Add(owner);
                            Db.SaveChanges();
                        }
                        else
                        {
                            // wenn es gemischt ist, dann alle mit Owner versehen
                            foreach (var offieHour in oldOffieHours)
                            {
                                var owner = new ActivityOwner
                                {
                                    Activity = offieHour,
                                    Member = lecturer
                                };
                                Db.ActivityOwners.Add(owner);
                                
                            }
                            Db.SaveChanges();
                        }
                    }
                    else
                    {
                        var theOfficeHour = oldOffieHours.First();
                        var owner = new ActivityOwner
                        {
                            Activity = theOfficeHour,
                            Member = lecturer
                        };
                        Db.ActivityOwners.Add(owner);
                        Db.SaveChanges();
                    }
                }
                */

                var viewModel = new LecturerViewModel
                {
                    Lecturer = lecturer,
                    OfficeHour = null, //myOfficeHour,
                    IsActive = true //myOfficeHour != null || hasDates
                };
                
                model.Add(viewModel);
            }

            ViewBag.UserRight = GetUserRight();
            ViewBag.Semester = sem;


            return PartialView("_ProfileList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Calendar(Guid id)
        {
            var model = Db.Members.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficeHour()
        {
            var semester = GetSemester();
            var member = GetMyMembership();

            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(x =>
                x.Semester.Id == semester.Id && x.Owners.Any(y => y.Member.Id == member.Id));

            if (officeHour == null)
                return View("Create");

            if (officeHour.ByAgreement)
                return View("ByAgreement", officeHour);

            if (officeHour.Dates.Any(x => x.Slots.Any()))
                return View("Slot", officeHour);

            return View("Open", officeHour);
        }
    }
}