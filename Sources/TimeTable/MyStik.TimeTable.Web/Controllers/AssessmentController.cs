using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

        public ActionResult Details(Guid id)
        {
            var user = GetCurrentUser();

            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));

            ViewBag.UserRights = GetUserRight();
            ViewBag.Member = member;

            return View(assessment);
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

            ViewBag.UserRights = GetUserRight();

            return View(model);
        }

        public ActionResult Candidates(Guid id)
        {
            var model = Db.Assessments.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        public ActionResult Candidate(Guid id)
        {
            var candidate = Db.Candidatures.SingleOrDefault(x => x.Id == id);


            return View(candidate);
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


            var curr = Db.Curricula.SingleOrDefault(x =>
                x.ShortName.Equals(model.CurriculumShortName) && x.Organiser.Id == org.Id);
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

        public ActionResult EditStage(Guid id)
        {
            var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == id);

            var model = new AssessmentStageCreateModel
            {
                AssessmentId = stage.Assessment.Id,
                StageId = stage.Id,
                Name = stage.Name,
                Description = stage.Description,
                Start = stage.OpeningDateTime.ToString(),
                End = stage.ClosingDateTime.ToString(),
                Publish = stage.ReportingDateTime.ToString()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditStage(AssessmentStageCreateModel model)
        {
            var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == model.StageId);

            stage.Name = model.Name;
            stage.Description = model.Description;
            stage.OpeningDateTime = DateTime.Parse(model.Start);
            stage.ClosingDateTime = DateTime.Parse(model.End);
            stage.ReportingDateTime = DateTime.Parse(model.Publish);

            Db.SaveChanges();


            return RedirectToAction("Details", new {id = model.AssessmentId});
        }

        public ActionResult ClearCandidates(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var isAdmin = assessment.Committee.Members.Any(x => x.HasChair && x.Member.UserId == user.Id);
            if (!isAdmin)
                return View("_NoAccess");



            foreach (var candidature in assessment.Candidatures.ToList())
            {
                foreach (var stage in candidature.Stages.ToList())
                {
                    foreach (var material in stage.Material.ToList())
                    {
                        Db.Storages.Remove(material.Storage);
                        Db.CandidatureStageMaterial.Remove(material);
                    }

                    Db.CandidatureStages.Remove(stage);
                }

                Db.Candidatures.Remove(candidature);
            }

            Db.SaveChanges();


            return RedirectToAction("Details", new {id = id});
        }

        public ActionResult Delete(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var isAdmin = assessment.Committee.Members.Any(x => x.HasChair && x.Member.UserId == user.Id);
            if (!isAdmin)
                return View("_NoAccess");



            foreach (var candidature in assessment.Candidatures.ToList())
            {
                foreach (var stage in candidature.Stages.ToList())
                {
                    foreach (var material in stage.Material.ToList())
                    {
                        Db.Storages.Remove(material.Storage);
                        Db.CandidatureStageMaterial.Remove(material);
                    }

                    Db.CandidatureStages.Remove(stage);
                }

                Db.Candidatures.Remove(candidature);
            }


            foreach (var stage in assessment.Stages.ToList())
            {
                foreach (var material in stage.Material.ToList())
                {
                    Db.Storages.Remove(material.Storage);
                    Db.AssessmentStageMaterial.Remove(material);
                }

                Db.AssessmentStages.Remove(stage);
            }

            Db.Assessments.Remove(assessment);


            Db.SaveChanges();


            return RedirectToAction("Admin");
        }



        public ActionResult GetProfileImage(Guid id)
        {
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);
            var user = GetUser(candidature.UserId);
            return File(user.BinaryData, user.FileType);
        }



        public ActionResult AddMember(Guid id)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);


            var model = new AddCommitteeMemberModel()
            {
                Assessment = assessment,
                OrganiserId2 = assessment.Curriculum.Organiser.Id

            };

            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        [HttpPost]
        public ActionResult AddMembers(Guid AssessmentId, ICollection<Guid> DozIds)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == AssessmentId);


            if (DozIds == null || DozIds.Count == 0)
            {
                foreach (var member in assessment.Committee.Members.ToList())
                {
                    Db.CommitteeMember.Remove(member);
                }

                Db.SaveChanges();

                return PartialView("_SaveSuccess");
            }


            foreach (var member in assessment.Committee.Members.ToList())
            {
                var inList= DozIds.Contains(member.Member.Id);

                if (inList)
                {
                    DozIds.Remove(member.Member.Id);
                }
                else
                {
                    Db.CommitteeMember.Remove(member);
                }
            }

            // die verbleibenden sind neu

            foreach (var dozId in DozIds)
            {
                var cm = new CommitteeMember
                {
                    Member = Db.Members.SingleOrDefault(x => x.Id == dozId),
                    HasChair = false,
                    Committee = assessment.Committee
                };

                Db.CommitteeMember.Add(cm);
            }


            Db.SaveChanges();

            return Json(new { result = "Redirect", url = Url.Action("Details",  new {id = assessment.Id}) });

        }

        public ActionResult AddChair(Guid asid, Guid cmid)
        {
            var cm = Db.CommitteeMember.SingleOrDefault(x => x.Id == cmid);
            cm.HasChair = true;
            Db.SaveChanges();

            return RedirectToAction("Details", new {id = asid});
        }

        public ActionResult RemoveChair(Guid asid, Guid cmid)
        {
            var cm = Db.CommitteeMember.SingleOrDefault(x => x.Id == cmid);
            cm.HasChair = false;
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = asid });
        }


        public ActionResult DeleteMember(Guid asid, Guid cmid)
        {
            var cm = Db.CommitteeMember.SingleOrDefault(x => x.Id == cmid);

            Db.CommitteeMember.Remove(cm);

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = asid });
        }

        public FileResult DownloadCand(Guid id)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);


            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Name;Vorname;E-Mail");

            foreach (var stage in assessment.Stages)
            {
                writer.Write(
                    ";# Arbeiten in {0}", stage.Name);
            }

            writer.Write(Environment.NewLine);

            foreach(var candidate in assessment.Candidatures)
            {

                var user = UserManager.FindById(candidate.UserId);

                if (user != null)
                {
                    writer.Write("{0};{1};{2}",
                        user.LastName, user.FirstName, user.Email);
                }
                else
                {
                    writer.Write("kein Benutzerkonto;;");
                }



                foreach (var stage in assessment.Stages)
                {
                    var mStage = candidate.Stages.FirstOrDefault(x => x.AssessmentStage.Id == stage.Id);

                    if (mStage != null)
                    {
                        writer.Write(";{0}", mStage.Material.Count);
                    }
                    else
                    {
                        writer.Write(";{0}", 0);
                    }
                }

                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("TN");
            sb.Append(assessment.Name);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());

        }
    }
}