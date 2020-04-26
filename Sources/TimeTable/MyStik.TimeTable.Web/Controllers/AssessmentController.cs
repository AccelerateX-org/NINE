using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AssessmentController : BaseController
    {
        // GET: Assessment
        public ActionResult Index(Guid? id)
        {
            if (id == null)
            {
                var all = Db.Assessments.ToList();

                return View("Overview", all);
            }

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new AssessmentOverviewModel
            {
                Curriculum = curr,
                Assessments = Db.Assessments.Where(x => x.Curriculum.Id == curr.Id).ToList()
            };


            return View(model);
        }

        public ActionResult Details()
        {

            return View();
        }


        public ActionResult Start(Guid id)
        {
            var model = Db.Assessments.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        public ActionResult Admin()
        {
            var org = GetMyOrganisation();


            var model = new AssessmentOverviewModel
            {
                Organiser = org,
                Assessments = Db.Assessments.Where(x => x.Curriculum.Organiser.Id == org.Id).ToList()
            };



            return View(model);
        }

        public ActionResult Candidates(Guid id)
        {
            var model = Db.Assessments.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new AssessmentCreateModel
            {
                Name = "TEST Auswahlverfahren",
                Description = "TEST TEST TEST",
                CurriculumShortName = "",
                SemesterName = "WiSe 2020",
                Stage1Name = "Mappe hochladenn",
                Stage1Start = "25.04.2020",
                Stage1End = "01.05.2020",
                Stage2Name = "Aufnahmeprüfung",
                Stage2Start = "03.05.2020",
                Stage2End = "08.05.2020",
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AssessmentCreateModel model)
        {
            var member = GetMyMembership();
            var org = GetMyOrganisation();


            var curr = Db.Curricula.SingleOrDefault(x => x.ShortName.Equals(model.CurriculumShortName) && x.Organiser.Id == org.Id);
            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(model.SemesterName));

            var start1 = DateTime.Parse(model.Stage1Start);
            var end1 = DateTime.Parse(model.Stage1End);

            var start2 = DateTime.Parse(model.Stage2Start);
            var end2 = DateTime.Parse(model.Stage2End);


            var assessment = new Assessment
            {
                Name = model.Name,
                Description = model.Description,
                Curriculum = curr,
                Semester = sem
            };


            var committee = new Committee
            {
                Name = "Aufnahmekommission für " + model.Name,
                Curriculum = curr
            };

            var comMember = new CommitteeMember
            {
                Member = member,
                HasChair = true
            };

            committee.Members = new List<CommitteeMember>();
            committee.Members.Add(comMember);
            assessment.Committee = committee;


            var stage1 = new AssessmentStage
            {
                Assessment = assessment,
                Name = model.Stage1Name,
                OpeningDateTime = start1,
                ClosingDateTime = end1
            };

            var stage2 = new AssessmentStage
            {
                Assessment = assessment,
                Name = model.Stage2Name,
                OpeningDateTime = start2,
                ClosingDateTime = end2
            };

            assessment.Stages = new List<AssessmentStage>();
            assessment.Stages.Add(stage1);
            assessment.Stages.Add(stage2);

            Db.CommitteeMember.Add(comMember);
            Db.Committees.Add(committee);
            Db.AssessmentStages.Add(stage1);
            Db.AssessmentStages.Add(stage2);
            Db.Assessments.Add(assessment);

            Db.SaveChanges();


            return RedirectToAction("Admin", new {id = curr.Id});
        }
    }
}