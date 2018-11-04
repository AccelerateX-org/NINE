using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AdvertisementsController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id).ToList();


            ViewBag.Organiser = org;
            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        public ActionResult Overview()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult Create()
        {
            var model = new AdvertisementCreateModel();

            model.ExpiryDate = DateTime.Today.AddDays(28).ToShortDateString();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public ActionResult Create(AdvertisementCreateModel model)
        {
            var member = GetMyMembership();

            var adv = new Advertisement
            {
                Title = model.Title,
                Description = model.Description,
                Owner = member,
                Created = DateTime.Now,
                VisibleUntil = DateTime.Parse(model.ExpiryDate),
                ForAdvancement = model.ForAdvancement,
                ForInternship = model.ForInternship,
                ForStayAbroad = model.ForStayAbroad,
                ForThesis = model.ForThesis,
                ForCompetition = model.ForCompetition,
                ForTutor = model.ForTutor,
                ForWorkingStudent = model.ForWorkingStudent,
            };

            Db.Advertisements.Add(adv);

            if (model.Attachment1 != null)
            {
                var storage = new BinaryStorage
                {
                    Category = "Ausschreibung",
                    FileType = model.Attachment1.ContentType,
                    BinaryData = new byte[model.Attachment1.ContentLength],
                };

                model.Attachment1.InputStream.Read(storage.BinaryData, 0, model.Attachment1.ContentLength);

                Db.Storages.Add(storage);

                adv.Attachment = storage;
            }

            Db.SaveChanges();



            return RedirectToAction("Index");
        }

        public ActionResult Details(Guid id)
        {
            var model = Db.Advertisements.SingleOrDefault(x => x.Id == id);

            var member = GetMyMembership();
            if (member.Id == model.Owner.Id)
            {
                ViewBag.ItsMe = true;
            }
            else
            {
                ViewBag.ItsMe = false;
            }

            return View(model);
        }


        public ActionResult Edit(Guid id)
        {
            var adv = Db.Advertisements.SingleOrDefault(x => x.Id == id);

            var model = new AdvertisementCreateModel();

            model.Id = adv.Id;
            model.Description = adv.Description;
            model.ExpiryDate = adv.VisibleUntil.ToShortDateString();
            model.ForAdvancement = adv.ForAdvancement;
            model.ForInternship = adv.ForInternship;
            model.ForStayAbroad = adv.ForStayAbroad;
            model.ForThesis = adv.ForThesis;
            model.ForCompetition = adv.ForCompetition;
            model.ForTutor = adv.ForTutor;
            model.ForWorkingStudent = adv.ForWorkingStudent;
            model.Title = adv.Title;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdvertisementCreateModel model)
        {
            var adv = Db.Advertisements.SingleOrDefault(x => x.Id == model.Id);

            if (adv != null)
            {
                adv.Title = model.Title;
                adv.Description = model.Description;
                adv.VisibleUntil = DateTime.Parse(model.ExpiryDate);
                adv.ForAdvancement = model.ForAdvancement;
                adv.ForInternship = model.ForInternship;
                adv.ForStayAbroad = model.ForStayAbroad;
                adv.ForThesis = model.ForThesis;
                adv.ForCompetition = model.ForCompetition;
                adv.ForTutor = model.ForTutor;
                adv.ForWorkingStudent = model.ForWorkingStudent;


                if (model.Attachment1 != null)
                {
                    if (adv.Attachment != null)
                    {
                        // Löschen
                        var bs = Db.Storages.SingleOrDefault(x => x.Id == adv.Attachment.Id);
                        if (bs != null)
                        {
                            Db.Storages.Remove(bs);
                        }
                    }

                    var storage = new BinaryStorage
                    {
                        Category = "Ausschreibung",
                        FileType = model.Attachment1.ContentType,
                        BinaryData = new byte[model.Attachment1.ContentLength],
                    };

                    model.Attachment1.InputStream.Read(storage.BinaryData, 0, model.Attachment1.ContentLength);

                    Db.Storages.Add(storage);

                    adv.Attachment = storage;
                }

                Db.SaveChanges();

            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var adv = Db.Advertisements.SingleOrDefault(x => x.Id == id);

            if (adv != null)
            {
                if (adv.Attachment != null)
                {
                    // Löschen
                    var bs = Db.Storages.SingleOrDefault(x => x.Id == adv.Attachment.Id);
                    if (bs != null)
                    {
                        Db.Storages.Remove(bs);
                    }
                }

                Db.Advertisements.Remove(adv);
                Db.SaveChanges();
            }


            return RedirectToAction("Index");
        }


        public FileResult ShowFile(Guid id)
        {
            var storage = Db.Storages.SingleOrDefault(x => x.Id == id);

            storage.AccessCount++;
            Db.SaveChanges();


            return File(storage.BinaryData, storage.FileType);
        }


        public ActionResult Thesis()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForThesis).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }

        public ActionResult Internship()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForInternship).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }

        public ActionResult Abroad()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForStayAbroad).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }

        public ActionResult Advancement()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForAdvancement).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }

        public ActionResult Tutor()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForTutor).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }

        public ActionResult Competition()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForCompetition).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }


        public ActionResult WorkingStudent()
        {
            var org = GetMyOrganisation();

            var model = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id && x.ForWorkingStudent).OrderByDescending(x => x.Created).ToList();

            return View(model);
        }


    }
}