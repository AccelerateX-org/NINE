using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class LecturerController : BaseController
    {
        // GET: Lecturer
        public ActionResult Index()
        {
            var model = new OrganiserViewModel();

            //ViewBag.FacultyList = Db.Organisers.Where(o => o.IsFaculty && !o.IsStudent && o.Members.Any()).OrderBy(s => s.Name).Select(f => new SelectListItem
            ViewBag.FacultyList = Db.Organisers.Where(o => o.ShortName.Equals("FK 09")).OrderBy(s => s.Name).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            ViewBag.UserRight = GetUserRight();

            model.Organiser = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            ViewBag.MenuId = "menu-lecturers";

            return View(model);
        }


        
 public ActionResult IndexMobil()
    
    {     var model       = new LecturerViewModelMobil();

            
            model.myLecturer  = new List<LecturerStateViewMobilModel>();
            model.Lecturer = new List<LecturerStateViewModelMobil2>();

            //var Lecturer = Db.Lecturer.Where(L => L.Number.StartsWith("A")).OrderBy(A => Z.Number).ToList();

       
            var Lecturer = new LecturerStateViewMobilModel
            {
                DozentName  = "Anzinger" ,  
                Kontakt = "anzinger@hm.edu",
                Sprechstunde =  "Dienstag 14:00-16:00", 
              
                
                Raum = new Room{
                Number = "R BG.090"}

                
            };

             var Lecturer1 = new LecturerStateViewModelMobil2 
             {

     


                DozentName  = "Anzinger" , 
                Kontakt = "anzinger@hm.edu",
                Sprechstunde =  "Dienstag 14:00-16:00", 
              
                
                Raum = new Room{
                Number = "R BG.090"}
             };


           


           model.Lecturer.Add(Lecturer1);
           model.myLecturer.Add(Lecturer);





            return View(model);
        }
      
       

        [HttpPost]
        public PartialViewResult Faculty(Guid facultyId)
        {
            var sem = GetSemester();

            var model = new List<LecturerViewModel>();

            //Aktiv = alle, die eine Sprechstunde anbieten
            /*
            var officeHours = Db.Activities.OfType<OfficeHour>().Where(oh => oh.Semester.Id == sem.Id).ToList();

            foreach (var officeHour in officeHours)
            {
                if (officeHour.Dates.Any())
                {
                    var lecturer = officeHour.Dates.First().Hosts.FirstOrDefault();
                    if (lecturer != null)
                    {
                        var viewModel = new LecturerViewModel
                        {
                            Lecturer = lecturer,
                            User = lecturer.UserId != null ? UserManager.FindById(lecturer.UserId) : null,
                            OfficeHour = officeHour
                        };

                        model.Add(viewModel);
                    }
                }
                else
                {
                    var fak = Db.Organisers.SingleOrDefault(o => o.Id == facultyId);
                    if (fak != null)
                    {
                        var lecturer = fak.Members.SingleOrDefault(m => m.ShortName.Equals(officeHour.ShortName));
                        if (lecturer != null)
                        {
                            var viewModel = new LecturerViewModel
                            {
                                Lecturer = lecturer,
                                User = lecturer.UserId != null ? UserManager.FindById(lecturer.UserId) : null,
                                OfficeHour = officeHour
                            };

                            model.Add(viewModel);
                        }
                    }
                }
            }

            if (!officeHours.Any())
            {
                var fak = Db.Organisers.SingleOrDefault(o => o.Id == facultyId);
                if (fak != null)
                {
                    foreach (var lec in fak.Members)
                    {
                        var viewModel = new LecturerViewModel
                        {
                            Lecturer = lec,
                            User = lec.UserId != null ? UserManager.FindById(lec.UserId) : null,
                            OfficeHour = null
                        };

                        model.Add(viewModel);
                    }
                }
            }

            model = model.OrderBy(m => m.Lecturer.Name).ToList();
            */

            // alle die einen termin haben, der zu einer aktuellen Semestergruppe gehört
            var activeLecturers =
            Db.Members.Where(m => m.Organiser.Id == facultyId && m.Dates.Any(d => d.Activity.SemesterGroups.Any(s => s.Semester.Id == sem.Id))).OrderBy(m => m.Name)
                .ToList();

            foreach (var lecturer in activeLecturers)
            {
                var viewModel = new LecturerViewModel
                {
                    Lecturer = lecturer,
                    User = lecturer.UserId != null ? UserManager.FindById(lecturer.UserId) : null,
                    OfficeHour =  Db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                        a.Semester.Id == sem.Id &&
                        a.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)))
                };
                
                model.Add(viewModel);
            }

            ViewBag.UserRight = GetUserRight();


            return PartialView("_ProfileList", model);
        }

        public ActionResult Calendar(Guid id)
        {
            var model = Db.Members.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

    }
}