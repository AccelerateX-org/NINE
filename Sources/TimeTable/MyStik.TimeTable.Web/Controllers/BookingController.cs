using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class BookingController : BaseController
    {
        // GET: Booking
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SelectCourse(Guid id)
        {
            var user = GetCurrentUser();

            var student = StudentService.GetCurrentStudent(user.Id);

            var courseService = new CourseService(Db);

            var courseSummary = courseService.GetCourseSummary(id);

            var bookingService = new BookingService(Db, courseSummary.Course.Occurrence.Id);

            var bookingLists = bookingService.GetBookingLists();

            var subscriptionService = new SubscriptionService(Db);

            var subscription = subscriptionService.GetSubscription(courseSummary.Course.Occurrence.Id, user.Id);


            var bookingState = new BookingState
            {
                Student = student,
                Occurrence = courseSummary.Course.Occurrence,
                BookingLists = bookingLists
            };
            bookingState.Init();

            var model = new CourseSelectModel
            {
                User = user,
                Student = student,
                Summary = courseSummary,
                BookingState = bookingState,
                Subscription = subscription,
            };


            var userRights = GetUserRight(User.Identity.Name, courseSummary.Course);
            ViewBag.UserRight = userRights;

            return View("SelectCourse", model);
        }

        public PartialViewResult Subscribe(Guid id)
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);

            Occurrence occ = null;
            OccurrenceSubscription subscription = null;

            using (var transaction = Db.Database.BeginTransaction())
            {
                occ = Db.Occurrences.SingleOrDefault(x => x.Id == id);

                subscription = occ.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                if (subscription == null)
                {
                    // eintragen
                    // den Status aus den Buchungslisten ermitteln
                    // ermittle Buchungsliste
                    // wenn eine Liste
                    // wenn voll, dann Warteliste
                    // sonst Teilnehmer
                    // sonst
                    // Fehlermeldung an Benutzer mit Angabe des Grunds

                    var bookingService = new BookingService(Db, id);
                    var bookingLists = bookingService.GetBookingLists();
                    var bookingState = new BookingState
                    {
                        Student = student,
                        Occurrence = occ,
                        BookingLists = bookingLists
                    };
                    bookingState.Init();

                    var bookingList = bookingState.MyBookingList;

                    if (bookingList != null)
                    {
                        subscription = new OccurrenceSubscription
                        {
                            TimeStamp = DateTime.Now,
                            Occurrence = occ,
                            UserId = user.Id,
                            OnWaitingList = bookingState.AvailableSeats <= 0
                        };

                        Db.Subscriptions.Add(subscription);
                    }

                }
                else
                {
                    // austragen
                    var subscriptionService = new SubscriptionService(Db);
                    subscriptionService.DeleteSubscription(subscription);
                }

                Db.SaveChanges();
                transaction.Commit();
            }

            // jetzt neu abrufen und anzeigen
            subscription = occ.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

            if (subscription == null)
            {
                return PartialView("_Subscribe", occ);
            }
            
            return PartialView("_Discharge", occ);
        }

    }
}