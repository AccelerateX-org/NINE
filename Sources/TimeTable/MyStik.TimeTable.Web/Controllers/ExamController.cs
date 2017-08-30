using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ExamController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Thesis()
        {
            var org = GetMyOrganisation();

            ViewBag.UserRight = GetUserRight();
            ViewBag.Organiser = org;

            var model = new List<ThesisListViewModel>();

            var userService = new UserInfoService();

            // Alle Studiengänge
            var currs = Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList();

            // pro Studiengang das kriterium Abschlussarbeit
            foreach (var curriculum in currs)
            {
                var crit = curriculum.Criterias.FirstOrDefault(x => x.Name.Equals("Abschlussarbeit"));
                if (crit != null)
                {
                    foreach (var moduleAccreditation in crit.Accreditations)
                    {
                        var allThesis = Db.StudentExams.Where(x => x.Exam.Module.Id == moduleAccreditation.Module.Id);

                        foreach (var thesis in allThesis)
                        {
                            var listEntry = new ThesisListViewModel
                            {
                                Curriculum = curriculum,
                                Module = moduleAccreditation.Module,
                                Exam = thesis,
                                User = userService.GetUser(thesis.Examinee.UserId)
                            };

                            model.Add(listEntry);
                        }

                    }
                }
            }


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterThesis()
        {
            var org = GetMyOrganisation();

            var model = new List<ThesisCriteriaListModel>();

            var currs = Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList();
            foreach (var curriculum in currs)
            {
                var crit = curriculum.Criterias.FirstOrDefault(x => x.Name.Equals("Abschlussarbeit"));
                if (crit != null)
                {
                    foreach (var moduleAccreditation in crit.Accreditations)
                    {
                        model.Add(new ThesisCriteriaListModel
                        {
                            Curriculum = curriculum,
                            Criteria = crit,
                            Module = moduleAccreditation.Module
                        });
                    }
                }
                else
                {
                    model.Add(new ThesisCriteriaListModel
                    {
                        Curriculum = curriculum
                    });
                }
            }



            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateThesis(Guid id)
        {

            return View();
        }
    }
}