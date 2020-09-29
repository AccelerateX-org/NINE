using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Booking.Data;

namespace MyStik.TimeTable.DataServices.Booking
{
    public class BookingService
    {
        private TimeTableDbContext Db;
        private Occurrence _occ;
        private BookingList _globalMiscList;
        private List<BookingList> _lists;

        public BookingService(TimeTableDbContext db)
        {
            Db = db;
        }

        public BookingType BookingType
        {
            get
            {
                return !_occ.HasHomeBias
                    ? BookingType.green
                    : (!_occ.IsCoterie ? BookingType.yellow : BookingType.red);
            }
        }
        public List<BookingList> GetBookingLists(Guid occId)
        {
            _lists = new List<BookingList>();

            _occ = Db.Occurrences.SingleOrDefault(x => x.Id == occId);
            if (_occ != null)
            {
                if (BookingType == BookingType.green)
                {
                    if (!_occ.UseGroups)
                    {
                        CreateMiscList("Offene Liste", false);
                    }
                    else
                    {
                        CreateListByCurriculum();
                        CreateMiscList("sonstige", true);
                    }
                }
                else
                {
                    if (_occ.UseGroups)
                    {
                        CreateListByCurriculum();
                    }
                    else
                    {
                        CreateJoinedList();
                    }

                    CreateMiscList("sonstige", true);
                }
            }

            foreach (var subscription in _occ.Subscriptions)
            {
                var student = GetStudent(subscription.UserId);
                var booking = new Data.Booking
                {
                    Student = student,
                    Subscription = subscription
                };

                if (student == null)
                {
                    _globalMiscList.Bookings.Add(booking);
                }
                else
                {
                    var list = _lists.FirstOrDefault(x => x.Curricula.Any(c => c.Id == student.Curriculum.Id));
                    if (list == null)
                    {
                        _globalMiscList.Bookings.Add(booking);
                    }
                    else
                    {
                        list.Bookings.Add(booking);
                    }
                }

            }

            return _lists;
        }

        private Student GetStudent(string userId)
        {
            return Db.Students.Where(x => x.UserId.Equals(userId)).OrderByDescending(x => x.Created)
                .FirstOrDefault();
        }

        private void CreateJoinedList()
        {
            var list = new BookingList("Gesamtliste");
            list.Occurrence = _occ;
            foreach (var occurrenceGroup in _occ.Groups.Where(x => x.SemesterGroups.Any()))
            {
                var curr = occurrenceGroup.SemesterGroups.First().CapacityGroup.CurriculumGroup.Curriculum;
                if (!list.Curricula.Contains(curr))
                {
                    list.Curricula.Add(curr);
                }
            }
            _lists.Add(list);
        }

        private void CreateListByCurriculum()
        {
            foreach (var occurrenceGroup in _occ.Groups.Where(x => x.SemesterGroups.Any()))
            {
                var curr = occurrenceGroup.SemesterGroups.First().CapacityGroup.CurriculumGroup.Curriculum;
                var list = _lists.FirstOrDefault(x => x.Curricula.Any(c => c.Id == curr.Id));
                if (list == null)
                {
                    list = new BookingList("");
                    list.Group = occurrenceGroup;
                    list.Occurrence = _occ;
                    list.Curricula.Add(curr);
                    _lists.Add(list);
                }
            }
        }

        private void CreateMiscList(string name, bool IsMisc)
        {
            var list = new BookingList(name);
            list.Occurrence = _occ;
            list.IsMisc = IsMisc;
            _lists.Add(list);
            _globalMiscList = list;
        }

        public BookingTicket Subscribe(string userId, Guid courseId)
        {
            var ticket = new BookingTicket();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var student = GetStudent(userId);

            ticket.Course = course;
            ticket.UserId = userId;

            var occurrence = course.Occurrence;
            OccurrenceSubscription subscription = null;

            using (var transaction = Db.Database.BeginTransaction())
            {
                subscription = occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));

                var bookingLists = GetBookingLists(occurrence.Id);
                var bookingState = new BookingState();
                bookingState.Student = student;
                bookingState.Occurrence = occurrence;
                bookingState.BookingLists = bookingLists;
                bookingState.Init();

                var myBookingList = bookingState.MyBookingList;

                if (subscription != null)
                {
                    DeleteSubscription(subscription);
                    if (myBookingList != null && !subscription.OnWaitingList)
                    {
                        var succeedingBooking = myBookingList.GetSucceedingBooking();
                        if (succeedingBooking != null)
                        {
                            succeedingBooking.Subscription.OnWaitingList = false;
                            ticket.SucceedingSubscription = succeedingBooking.Subscription;
                        }
                    }
                }
                else
                {
                    if (myBookingList != null)
                    {
                        subscription = new OccurrenceSubscription();
                        subscription.TimeStamp = DateTime.Now;
                        subscription.Occurrence = occurrence;
                        subscription.UserId = userId;
                        subscription.OnWaitingList = bookingState.AvailableSeats <= 0;
                        Db.Subscriptions.Add(subscription);
                        ticket.Subscription = subscription;
                    }
                }

                Db.SaveChanges();
                transaction.Commit();
            }

            return ticket;
        }

        public void DeleteSubscription(OccurrenceSubscription subscription)
        {
            var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
            foreach (var drawing in allDrawings)
            {
                Db.SubscriptionDrawings.Remove(drawing);
            }

            var bets = subscription.Bets.ToList();
            foreach (var bet in bets)
            {
                Db.LotteriyBets.Remove(bet);
            }

            Db.Subscriptions.Remove(subscription);

            Db.SaveChanges();
        }

    }
}



