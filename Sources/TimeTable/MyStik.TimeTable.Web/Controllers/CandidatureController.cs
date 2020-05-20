using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CandidatureController : BaseController
    {
        // GET: Candidature
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var cand = Db.Candidatures.Where(x => x.UserId.Equals(user.Id)).ToList();

            if (cand.Any())
                return View(cand);

            return RedirectToAction("Index", "Assessment");
        }

        public ActionResult Create(Guid id)
        {
            var user = GetCurrentUser();

            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == id);

            var candidature =
                Db.Candidatures.SingleOrDefault(x => x.Assessment.Id == assessment.Id && x.UserId.Equals(user.Id));

            if (candidature == null)
            {
                return View(assessment);
                candidature = new Candidature
                {
                    Assessment = assessment,
                    Joined = DateTime.Now,
                    UserId = user.Id,
                };

                Db.Candidatures.Add(candidature);
                Db.SaveChanges();
            }

            return RedirectToAction("MyRoom", new {id = candidature.Id});
        }

        [HttpPost]
        public ActionResult Create(Assessment model)
        {
            var user = GetCurrentUser();

            var assessment = Db.Assessments.SingleOrDefault(x => x.Id == model.Id);

            var candidature =
                Db.Candidatures.SingleOrDefault(x => x.Assessment.Id == assessment.Id && x.UserId.Equals(user.Id));

            if (candidature == null)
            {
                candidature = new Candidature
                {
                    Assessment = assessment,
                    Joined = DateTime.Now,
                    UserId = user.Id,
                };

                Db.Candidatures.Add(candidature);
                Db.SaveChanges();
            }

            return RedirectToAction("MyRoom", new { id = candidature.Id });
        }

        public ActionResult MyRoom(Guid id)
        {
            var user = GetCurrentUser();
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);

            if (!user.Id.Equals(candidature.UserId))
            {
                return View("_NoAccess");
            }


            return View(candidature);
        }

        public ActionResult TextInput(Guid id)
        {
            var user = GetCurrentUser();
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);
            if (!user.Id.Equals(candidature.UserId))
            {
                return View("_NoAccess");
            }


            return View(candidature);
        }

        [HttpPost]
        public ActionResult TextInput(Candidature model)
        {
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == model.Id);

            candidature.Characteristics = model.Characteristics;
            candidature.Motivation = model.Motivation;

            Db.SaveChanges();

            return RedirectToAction("MyRoom", new {id = candidature.Id});
        }

        public ActionResult Stage(Guid candId, Guid stageId)
        {
            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == candId);

            var round = candidature.Stages.SingleOrDefault(x => x.AssessmentStage.Id == stageId);

            if (round == null)
            {
                var stage = Db.AssessmentStages.SingleOrDefault(x => x.Id == stageId);
                round = new CandidatureStage
                {
                    AssessmentStage = stage,
                    Candidature = candidature,
                    Material = new List<CandidatureStageMaterial>()
                };
                Db.CandidatureStages.Add(round);
                Db.SaveChanges();

            }

            return View(round);
        }

        public ActionResult UploadMaterial(Guid id)
        {
            var stage = Db.CandidatureStages.SingleOrDefault(x => x.Id == id);

            var model = new CandidatureUploadMaterialModel
            {
                Stage = stage
            };


            return View(model);
        }


        [HttpPost]
        public ActionResult UploadMaterial(CandidatureUploadMaterialModel model)
        {
            var stage = Db.CandidatureStages.SingleOrDefault(x => x.Id == model.Stage.Id);

            var material = new CandidatureStageMaterial
            {
                Stage = stage,
            };

            Db.CandidatureStageMaterial.Add(material);

            if (model.File != null)
            {
                var storage = new BinaryStorage
                {
                    Category = "Material",
                    FileType = model.File.ContentType,
                    BinaryData = new byte[model.File.ContentLength],
                    Created = DateTime.Now,
                    Name = model.Name,
                    Description = model.Description
                };

                model.File.InputStream.Read(storage.BinaryData, 0, model.File.ContentLength);

                Db.Storages.Add(storage);

                material.Storage = storage;
            }

            Db.SaveChanges();

            return RedirectToAction("Stage", new {candId = stage.Candidature.Id, stageId = stage.AssessmentStage.Id});
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, Guid id)
        {
            var stage = Db.CandidatureStages.SingleOrDefault(x => x.Id == id);

            var material = new CandidatureStageMaterial
            {
                Stage = stage,
            };

            Db.CandidatureStageMaterial.Add(material);

            if (file != null)
            {
                var storage = new BinaryStorage
                {
                    Category = "Material",
                    FileType = file.ContentType,
                    BinaryData = new byte[file.ContentLength],
                    Created = DateTime.Now,
                    Name = string.Empty,
                    Description = string.Empty
                };

                file.InputStream.Read(storage.BinaryData, 0, file.ContentLength);

                Db.Storages.Add(storage);

                material.Storage = storage;
            }

            Db.SaveChanges();

            return null;
        }


        [HttpPost]
        public ActionResult DeleteMaterial(Guid matId)
        {
            var mat = Db.CandidatureStageMaterial.SingleOrDefault(x => x.Id == matId);

            Db.Storages.Remove(mat.Storage);
            Db.CandidatureStageMaterial.Remove(mat);

            Db.SaveChanges();

            return null;
        }

        [HttpPost]
        public PartialViewResult EditMaterial(Guid matId, string matTitle, string matDesc)
        {
            var mat = Db.CandidatureStageMaterial.SingleOrDefault(x => x.Id == matId);

            mat.Storage.Name = matTitle;
            mat.Storage.Description = matDesc;

            Db.SaveChanges();

            return PartialView("_MaterialBox", mat);
        }


        [HttpPost]
        public ActionResult UploadProfileImage(HttpPostedFileBase file)
        {
            var user = GetCurrentUser();

            var userDb = new ApplicationDbContext();

            var currentUser = userDb.Users.SingleOrDefault(u => u.Id.Equals(user.Id));

            if (file != null && currentUser != null)
            {
                currentUser.FileType = file.ContentType;
                currentUser.BinaryData = new byte[file.ContentLength];

                file.InputStream.Read(currentUser.BinaryData, 0, file.ContentLength);

                userDb.SaveChanges();
            }


            return null;
        }

        public ActionResult GetProfileImage()
        {
            var user = GetCurrentUser();
            return File(user.BinaryData, user.FileType);
        }

        public ActionResult DeleteProfileImage(Guid id)
        {

            var user = GetCurrentUser();

            var userDb = new ApplicationDbContext();

            var currentUser = userDb.Users.SingleOrDefault(u => u.Id.Equals(user.Id));

            if (currentUser != null)
            {
                currentUser.FileType = String.Empty;
                currentUser.BinaryData = null;

                userDb.SaveChanges();
            }

            var candidature = Db.Candidatures.SingleOrDefault(x => x.Id == id);

            return RedirectToAction("MyRoom", new { id = candidature.Id });
        }
    }
}