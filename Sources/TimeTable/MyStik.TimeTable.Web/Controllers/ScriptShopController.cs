using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ScriptShopController : BaseController
    {
        // GET: ScriptShop
        public ActionResult History()
        {
            // Alle OrderBaskets
            var user = GetCurrentUser();

            var baskets = Db.OrderBaskets.Where(x => x.UserId.Equals(user.Id)).ToList();

            var model = baskets;


            return View(model);
        }



        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var students = GetCurrentStudent(user.Id);
            var student = students.FirstOrDefault();

            var org = student.Curriculum.Organiser;

            // Der Bestellzeitraum, welcher als erstes in der Zukunft endet
            var period = Db.OrderPeriods.Where(x => x.Organiser.Id == org.Id && x.End >= DateTime.Today)
                .OrderByDescending(x => x.End).ToList().LastOrDefault();

            if (period == null)
            {
                // Alle meine Baskets
                var baskets = Db.OrderBaskets.Where(x => x.UserId.Equals(user.Id)).ToList();

                var historyModel = new ScriptOrderHistoryModel();

                historyModel.Baskets = baskets;

                var currentSemester = SemesterService.GetSemester(DateTime.Today);
                var nextSemester = SemesterService.GetNextSemester(currentSemester);

                ViewBag.CurrentSemester = currentSemester;
                ViewBag.NextSemester = nextSemester;

                return View(historyModel);
            }

            if (period.Begin > DateTime.Today)
            {
                return View("_FutureOrderPeriod", period);
            }

            // alle Skripte für alle Lehveranstaltungen, die im zugehörigen Semester belegt sind


            var semester = period.Semester;

            var basket = period.Baskets.SingleOrDefault(x => x.UserId.Equals(user.Id));

            if (basket == null)
            {
                basket = new OrderBasket
                {
                    OrderPeriod = period,
                    UserId = user.Id,
                    Orders = new List<ScriptOrder>()
                };

                Db.OrderBaskets.Add(basket);
                Db.SaveChanges();
            }


            // Alle gebuchten Lehrveranstaltungen
            var courseService = new CourseInfoService(Db);

            var model = new ScriptShopPeriodModel();
            model.Period = period;
            model.User = user;


            var courses = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();

            foreach (var course in courses)
            {
                // die Zusammenfassung
                var summary = courseService.GetCourseSummary(course);

                var courseModel = new ScriptShopCourseModel();
                courseModel.CourseSummary = summary;

                // Skripte
                var scripPublishings = Db.ScriptPublishings.Where(x => x.Course.Id == course.Id).ToList();

                foreach (var scriptPublishing in scripPublishings)
                {
                    var orders = basket.Orders.Where(x => x.ScriptDocument.Id == scriptPublishing.ScriptDocument.Id).ToList();

                    var docModel = new ScriptShopDocumentModel();


                    docModel.Publishing = scriptPublishing;
                    docModel.Orders = orders;

                    courseModel.Documents.Add(docModel);
                }


                model.Courses.Add(courseModel);

            }

            return View("Basket", model);
        }

        public ActionResult Inventory(Guid id)
        {
            var semester = SemesterService.GetSemester(id);

            var docs = Db.ScriptDocuments.Where(x => 
                    x.Publishings.Any(p => 
                        p.Course.SemesterGroups.Any(g => 
                            g.Semester.Id == id)))
                .ToList();


            ViewBag.Semester = semester;

            return View(docs);
        }

        public ActionResult Order(Guid id)
        {
            var pub = Db.ScriptPublishings.SingleOrDefault(x => x.Id == id);

            return View(pub);
        }



        public ActionResult OrderConfirmed(Guid id)
        {
            var user = GetCurrentUser();
            var student = GetCurrentStudent(user.Id).FirstOrDefault();
            var org = student.Curriculum.Organiser;

            // Der Bestellzeitraum, welcher als erstes in der Zukunft endet
            var period = Db.OrderPeriods.Where(x => x.Organiser.Id == org.Id && x.End >= DateTime.Today)
                .OrderByDescending(x => x.End).Include(orderPeriod => orderPeriod.Baskets.Select(orderBasket =>
                    orderBasket.Orders.Select(scriptOrder => scriptOrder.ScriptDocument))).ToList().LastOrDefault();

            if (period == null)
            {
                return View("_NoOrderPeriod");
            }

            if (period.Begin > DateTime.Today)
            {
                return View("_FutureOrderPeriod", period);
            }


            var pub = Db.ScriptPublishings.SingleOrDefault(x => x.Id == id);


            var basket = period.Baskets.SingleOrDefault(x => x.UserId.Equals(user.Id));


            var alreadyOrdered = basket.Orders.Any(x => x.ScriptDocument.Id == pub.ScriptDocument.Id);

            if (alreadyOrdered)
                return RedirectToAction("Index");


            var order = new ScriptOrder();

            order.ScriptDocument = pub.ScriptDocument;
            order.OrderBasket = basket;
            order.OrderedAt = DateTime.Now;


            Db.ScriptOrders.Add(order);
            Db.SaveChanges();

            return RedirectToAction("Index");
        }



        public ActionResult CancelOrder(Guid id)
        {
            var order = Db.ScriptOrders.SingleOrDefault(x => x.Id == id);

            return View(order);
        }



        public ActionResult CancelOrderConfirmed(Guid id)
        {
            var order = Db.ScriptOrders.SingleOrDefault(x => x.Id == id);

            Db.ScriptOrders.Remove(order);
            Db.SaveChanges();


            return RedirectToAction("Index");
        }



    }
}