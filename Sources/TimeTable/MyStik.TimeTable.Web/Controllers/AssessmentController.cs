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
using MyStik.TimeTable.Web.Services;

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
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var user = GetCurrentUser();
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");

            ViewBag.UserRights = GetUserRight();
            ViewBag.Member = member;

            return View(assessment);
        }

        public ActionResult Candidate(Guid id)
        {
            var candidate = Db.Candidatures.SingleOrDefault(x => x.Id == id);

            var user = GetCurrentUser();
            var assessment = candidate.Assessment;
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");

            ViewBag.UserRights = GetUserRight();
            ViewBag.Member = member;

            return View(candidate);
        }
        public ActionResult CandidateDetails(Guid id)
        {
            var candidate = Db.Candidatures.SingleOrDefault(x => x.Id == id);

            var user = GetCurrentUser();
            var assessment = candidate.Assessment;
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");

            ViewBag.Assessments = Db.Assessments.ToList();
            ViewBag.UserRights = GetUserRight();
            ViewBag.Member = member;

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


            Db.CommitteeMember.Add(comMember);
            Db.Committees.Add(committee);

            Db.Assessments.Add(assessment);

            Db.SaveChanges();


            return RedirectToAction("Details", new {id = assessment.Id});
        }


        public ActionResult Edit(Guid id)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var model = new AssessmentCreateModel
            {
                AssessmentId = assessment.Id,
                Name = assessment.Name,
                Description = assessment.Description,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AssessmentCreateModel model)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == model.AssessmentId);

            assessment.Name = model.Name;
            assessment.Description = model.Description;

            Db.SaveChanges();


            return RedirectToAction("Details", new { id = assessment.Id });
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
                Publish = stage.ReportingDateTime.ToString(),
                IsAvailable = stage.IsAvailable,
                FileTypes = stage.FileTypes,
                MaxFileCount = stage.MaxFileCount,
                NaxPxSize = stage.NaxPxSize
            };

            if (string.IsNullOrEmpty(model.FileTypes))
            {
                model.FileTypes = ".png,.jpg,.gif,image/*";
            }

            if (model.MaxFileCount == 0)
            {
                model.MaxFileCount = 25;
            }


            if (model.NaxPxSize == 0)
            {
                model.NaxPxSize = 1920;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditStage(AssessmentStageCreateModel model)
        {
            if (string.IsNullOrEmpty(model.FileTypes))
            {
                model.FileTypes = ".png,.jpg,.gif,image/*";
            }

            if (model.MaxFileCount == 0)
            {
                model.MaxFileCount = 25;
            }

            if (model.NaxPxSize == 0)
            {
                model.NaxPxSize = 1920;
            }



            var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == model.StageId);

            stage.Name = model.Name;
            stage.Description = model.Description;
            stage.OpeningDateTime = DateTime.Parse(model.Start);
            stage.ClosingDateTime = DateTime.Parse(model.End);
            stage.ReportingDateTime = DateTime.Parse(model.Publish);
            stage.IsAvailable = model.IsAvailable;
            stage.FileTypes = model.FileTypes;
            stage.MaxFileCount = model.MaxFileCount;
            stage.NaxPxSize = model.NaxPxSize;

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

        public ActionResult Clear(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var userRight = GetUserRight();


            var isAdmin = userRight.IsOrgAdmin;
            if (!isAdmin)
                return View("_NoAccess");



            foreach (var candidature in assessment.Candidatures.ToList())
            {
                foreach (var stage in candidature.Stages.ToList())
                {
                    foreach (var material in stage.Material.ToList())
                    {
                        if (material.Storage != null)
                        {
                            Db.Storages.Remove(material.Storage);
                        }

                        Db.CandidatureStageMaterial.Remove(material);
                    }

                    Db.CandidatureStages.Remove(stage);
                }

                Db.Candidatures.Remove(candidature);
            }


            /*
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
            */


            Db.SaveChanges();


            return RedirectToAction("Admin");
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
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!userRight.Member.IsAdmin)
                return null;


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

                var candUser = UserManager.FindById(candidate.UserId);

                if (candUser != null)
                {
                    writer.Write("{0};{1};{2}",
                        candUser.LastName, candUser.FirstName, candUser.Email);
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

        public ActionResult Stage(Guid id)
        {
            var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == id);

            return View(stage);
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, Guid id)
        {
            var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == id);

            if (file == null || stage == null)
                return null;

            var material = new AssessmentStageMaterial
            {
                Stage = stage,
            };

            Db.AssessmentStageMaterial.Add(material);

            var storage = new BinaryStorage
            {
                Category = "Material",
                FileType = file.ContentType,
                BinaryData = new byte[file.ContentLength],
                Created = DateTime.Now,
                Name = file.FileName,
                Description = string.Empty
            };

            file.InputStream.Read(storage.BinaryData, 0, file.ContentLength);

            Db.Storages.Add(storage);

            material.Storage = storage;

            Db.SaveChanges();

            return null;
        }

        [HttpPost]
        public ActionResult DeleteMaterial(Guid matId)
        {
            var mat = Db.AssessmentStageMaterial.SingleOrDefault(x => x.Id == matId);

            if (mat.Storage != null)
            {
                Db.Storages.Remove(mat.Storage);
            }

            Db.AssessmentStageMaterial.Remove(mat);

            Db.SaveChanges();

            return null;
        }

        [HttpPost]
        public PartialViewResult EditMaterial(Guid matId, string matTitle, string matDesc)
        {
            var mat = Db.AssessmentStageMaterial.SingleOrDefault(x => x.Id == matId);

            mat.Storage.Name = matTitle;
            mat.Storage.Description = matDesc;

            Db.SaveChanges();

            return PartialView("_MaterialBox", mat);
        }

        public ActionResult UploadResults(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!userRight.Member.IsAdmin)
                return View("_NoAccess");

            ViewBag.UserRights = GetUserRight();
            ViewBag.Member = member;

            return View(assessment);
        }

        [HttpPost]
        public ActionResult UploadResults(Assessment model, HttpPostedFileBase resultFile)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == model.Id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));

            var userInfoService = new UserInfoService();
            var errors = new List<string>();


            if (resultFile == null)
                return null;
            
            var bytes = new byte[resultFile.ContentLength];
            resultFile.InputStream.Read(bytes, 0, resultFile.ContentLength);

            var stream = new System.IO.MemoryStream(bytes);
            var reader = new System.IO.StreamReader(stream, Encoding.Default);
            var text = reader.ReadToEnd();

            string[] lines = text.Split('\n');

            foreach (var line in lines)
            {
                string newline = line.Trim();

                if (!string.IsNullOrEmpty(newline))
                {
                    string[] words = newline.Split(';');

                    var firstName = words[0].Trim();
                    var lastName = words[1].Trim();
                    var email = words[2].Trim();

                    var candUser = userInfoService.GetUserByEmail(email);

                    if (candUser == null)
                    {
                        var msg = $"Kein Benutzer für E-Mail Adresse {email} vorhanden";
                        errors.Add(msg);
                        continue;
                    }

                    if (!(candUser.FirstName.Trim().ToLower().Equals(firstName.ToLower()) &&
                          candUser.LastName.Trim().ToLower().Equals(lastName.ToLower())))
                    {
                        var msg = $"Angaben für den Benutzer {email} stimmen nicht überein: Datei: \"{firstName}\" \"{lastName}\", Datenbank: \"{candUser.FirstName}\" \"{candUser.LastName}\"";
                        errors.Add(msg);
                        continue;

                    }


                    var candidate = assessment.Candidatures.SingleOrDefault(x => x.UserId.Equals(candUser.Id));

                    if (candidate == null)
                    {
                        var msg = $"Benutzer {email} nicht im Verfahren dabei";
                        errors.Add(msg);
                        continue;
                    }

                    candidate.IsAccepted = true;
                }
            }


            if (errors.Any())
            {
                return View("_Errors", errors);
            }

            Db.SaveChanges();

            return RedirectToAction("Acceptance", new {id = assessment.Id});
        }


        public ActionResult Acceptance(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");


            var model = new AssessmenSummaryModel
            {
                Assessment = assessment,
                Candidates = new List<AssessmentCandidateViewModel>()
            };

            var userInfoService = new UserInfoService();
            var officeHour = GetOfficeHour(assessment);

            foreach (var candidature in assessment.Candidatures)
            {
                var candUser = userInfoService.GetUser(candidature.UserId);

                var ohDate = candUser != null ?
                    officeHour.Dates.FirstOrDefault(x =>
                    x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(candUser.Id)))
                    : null;


                var candModel = new AssessmentCandidateViewModel
                {
                    Candidature = candidature,
                    User = candUser,
                    Date = ohDate
                };

                model.Candidates.Add(candModel);
            }

            return View(model);
        }

        public ActionResult Reset(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!userRight.Member.IsAdmin)
                return View("_NoAccess");

            foreach (var candidature in assessment.Candidatures)
            {
                candidature.IsAccepted = null;
            }

            Db.SaveChanges();

            return RedirectToAction("Acceptance", new { id = assessment.Id });
        }


        public ActionResult ClearDates(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");


            var userInfoService = new UserInfoService();
            var officeHour = GetOfficeHour(assessment);

            foreach (var candidature in assessment.Candidatures)
            {
                var candUser = userInfoService.GetUser(candidature.UserId);

                var ohDate = candUser != null ?
                    officeHour.Dates.FirstOrDefault(x =>
                        x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(candUser.Id)))
                    : null;

                if (ohDate != null)
                {
                    Db.ActivityDates.Remove(ohDate);
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Details", new {id = assessment.Id});
        }



        public ActionResult UploadDates(Guid id)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!userRight.Member.IsAdmin)
                return View("_NoAccess");


            return View(assessment);
        }

        [HttpPost]
        public ActionResult UploadDates(Assessment model, HttpPostedFileBase resultFile)
        {
            var user = GetCurrentUser();
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == model.Id);
            var member = assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));

            var userInfoService = new UserInfoService();
            var errors = new List<string>();


            if (resultFile == null)
                return null;

            var officeHour = GetOfficeHour(assessment);


            var bytes = new byte[resultFile.ContentLength];
            resultFile.InputStream.Read(bytes, 0, resultFile.ContentLength);

            var stream = new System.IO.MemoryStream(bytes);
            var reader = new System.IO.StreamReader(stream, Encoding.Default);
            var text = reader.ReadToEnd();

            string[] lines = text.Split('\n');

            foreach (var line in lines)
            {
                string newline = line.Trim();

                if (!string.IsNullOrEmpty(newline))
                {
                    string[] words = newline.Split(';');

                    var firstName = words[0].Trim();
                    var lastName = words[1].Trim();
                    var email = words[2].Trim();
                    var date = DateTime.ParseExact(words[3].Trim(), "dd.MM.yyyy", null);
                    var from = TimeSpan.ParseExact(words[4].Trim(), @"h\:mm", null);
                    var until = TimeSpan.ParseExact(words[5].Trim(), @"h\:mm", null);

                    var candUser = userInfoService.GetUserByEmail(email);

                    if (candUser == null)
                    {
                        var msg = $"Kein Benutzer für E-Mail Adresse {email} vorhanden";
                        errors.Add(msg);
                        continue;
                    }

                    if (!(candUser.FirstName.Trim().ToLower().Equals(firstName.ToLower()) &&
                          candUser.LastName.Trim().ToLower().Equals(lastName.ToLower())))
                    {
                        var msg = $"Angaben für den Benutzer {email} stimmen nicht überein: Datei: {firstName} {lastName}, Datenbank: {candUser.FirstName} {candUser.LastName}";
                        errors.Add(msg);
                        continue;

                    }


                    var candidate = assessment.Candidatures.SingleOrDefault(x => x.UserId.Equals(candUser.Id));

                    if (candidate == null)
                    {
                        var msg = $"Benutzer {email} nicht im Verfahren dabei";
                        errors.Add(msg);
                        continue;
                    }


                    if (candidate.IsAccepted != true)
                    {
                        var msg = $"Benutzer {email} wurde bisher nicht als angenommen markiert";
                        errors.Add(msg);
                        continue;
                    }


                    // so jetzt Datum und Uhrzeit Termin finden hinzufügen und/oder Termin anlegen
                    var begin = date.Add(from);
                    var end = date.Add(until);

                    var ohDate = officeHour.Dates.SingleOrDefault(x => x.Begin == begin && x.End == end);

                    if (ohDate == null)
                    {
                        ohDate = new ActivityDate
                        {
                            Activity = officeHour,
                            Begin = begin,
                            End = end,
                            Occurrence = new Occurrence
                            {
                                IsAvailable = true,
                                Capacity = 0,
                                FromIsRestricted = false,
                                UntilIsRestricted = false,
                                UntilTimeSpan = null,
                                IsCanceled = false,
                                IsMoved = false,
                            }
                        };

                        Db.ActivityDates.Add(ohDate);
                    }


                    // die subscription suchen
                    var ohSubDate = officeHour.Dates.FirstOrDefault(x =>
                        x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(candUser.Id)));

                    if (ohSubDate == null)
                    {
                        var ohSubscription = new OccurrenceSubscription
                        {
                            Occurrence = ohDate.Occurrence,
                            OnWaitingList = false,
                            UserId = candUser.Id,
                            TimeStamp = DateTime.Now
                        };

                        Db.Subscriptions.Add(ohSubscription);

                    }
                    else
                    {
                        var msg = $"Benutzer {email} hat schon einen Termin am {ohSubDate.Begin}";
                        errors.Add(msg);
                        continue;
                    }
                }
            }

            if (errors.Any())
            {
                return View("_Errors", errors);
            }

            Db.SaveChanges();

            return RedirectToAction("Acceptance", new { id = assessment.Id });
        }


        private OfficeHour GetOfficeHour(Assessment assessment)
        {
            var ohName = $"Eignungsgespräche {assessment.Name}";

            var oh = Db.Activities.OfType<OfficeHour>().FirstOrDefault(x => x.Name.Equals(ohName));

            if (oh != null)
                return oh;

            // Neue Sprechstunde anlegen
            // Alle Kommittee-Mitglieder werden zu Ownern
            oh = new OfficeHour
            {
                Name = ohName,
                Semester = assessment.Semester,
                IsInternal = true,
                FutureSubscriptions = 1,
                Description = "Eignungsgespräche",
                Organiser = assessment.Curriculum.Organiser,
                Occurrence = new Occurrence
                {
                    IsAvailable = true,
                    Capacity = 4,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    UntilTimeSpan = null,
                    IsCanceled = false,
                    IsMoved = false,
                }
            };

            foreach (var committeeMember in assessment.Committee.Members)
            {
                ActivityOwner owner = new ActivityOwner
                {
                    Activity = oh,
                    Member = committeeMember.Member,
                    IsLocked = false
                };

                oh.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }


            Db.Activities.Add(oh);
            Db.SaveChanges();

            return oh;
        }

        public ActionResult ChangeAssessment(Guid candId, Guid asId)
        {
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == candId);
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == asId);


            candidature.Assessment = assessment;
            Db.SaveChanges();


            return RedirectToAction("CandidateDetails", new {id = candidature.Id});
        }



        public ActionResult CreateStage(Guid id)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var model = new AssessmentStageCreateModel
            {
                AssessmentId = assessment.Id,
                Name = "Neue Stufe",
                Description = "",
                Start = DateTime.Now.ToString(),
                End = DateTime.Now.ToString(),
                Publish = DateTime.Now.ToString(),
                IsAvailable = false,
                FileTypes = ".png,.jpg,.gif,image/*",
                MaxFileCount = 25,
                NaxPxSize = 1920
            };

            return View(model);

        }

        [HttpPost]
        public ActionResult CreateStage(AssessmentStageCreateModel model)
        {
            if (string.IsNullOrEmpty(model.FileTypes))
            {
                model.FileTypes = ".png,.jpg,.gif,image/*";
            }

            if (model.MaxFileCount == 0)
            {
                model.MaxFileCount = 25;
            }

            if (model.NaxPxSize == 0)
            {
                model.NaxPxSize = 1920;
            }


            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == model.AssessmentId);

            var stage = new AssessmentStage();

            stage.Assessment = assessment;
            stage.Name = model.Name;
            stage.Description = model.Description;
            stage.OpeningDateTime = DateTime.Parse(model.Start);
            stage.ClosingDateTime = DateTime.Parse(model.End);
            stage.ReportingDateTime = DateTime.Parse(model.Publish);
            stage.IsAvailable = model.IsAvailable;
            stage.FileTypes = model.FileTypes;
            stage.MaxFileCount = model.MaxFileCount;
            stage.NaxPxSize = model.NaxPxSize;

            Db.AssessmentStages.Add(stage);
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = model.AssessmentId });
        }

        public ActionResult DeleteStage(Guid id)
        {
            var userRight = GetUserRight();

            var isAdmin = userRight.IsOrgAdmin;
            if (!isAdmin)
                return View("_NoAccess");


            var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == id);

            var assessment = stage.Assessment;

            // die Stufe muss leer sein
            if (assessment.Candidatures.Any(x => x.Stages.Any(y => y.AssessmentStage.Id == id)))
            {
                return RedirectToAction("Details", new { id = assessment.Id });
            }

            // kann gelöscht werden

            foreach (var stageMaterial in stage.Material.ToList())
            {
                if (stageMaterial.Storage != null)
                {
                    Db.Storages.Remove(stageMaterial.Storage);
                }

                Db.AssessmentStageMaterial.Remove(stageMaterial);
            }


            Db.AssessmentStages.Remove(stage);
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = assessment.Id });

        }

        public ActionResult Export(Guid id)
        {
            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var model = new AssessmentStageCreateModel
            {
                AssessmentId = assessment.Id,
                Name = assessment.Name,
            };

            return View(model);
        }

        public ActionResult SetAcceptance(Guid id)
        {
            var user = GetCurrentUser();
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);
            var member = candidature.Assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");


            candidature.IsAccepted = true;
            Db.SaveChanges();

            return RedirectToAction("Acceptance", new {id = candidature.Assessment.Id});
        }

        public ActionResult RejectAcceptance(Guid id)
        {
            var user = GetCurrentUser();
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);
            var member = candidature.Assessment.Committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));
            var userRight = GetUserRight();

            if (!(userRight.Member.IsAdmin || member != null))
                return View("_NoAccess");


            candidature.IsAccepted = false;
            Db.SaveChanges();

            return RedirectToAction("Acceptance", new { id = candidature.Assessment.Id });
        }



    }
}