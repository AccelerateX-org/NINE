using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Migrations;
using MyStik.TimeTable.DataServices.Booking.Data;
using Curriculum = MyStik.TimeTable.Data.Curriculum;
using Occurrence = MyStik.TimeTable.Data.Occurrence;

namespace MyStik.TimeTable.DataServices.Booking
{
    /*
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
    */


    public class BookingServiceQuotas
    {
        private TimeTableDbContext Db;
        private Occurrence _occ;
        private List<BookingList> _lists;


        public BookingServiceQuotas(TimeTableDbContext db, Occurrence occ)
        {
            Db = db;
            _occ = occ;
            InitBookingLists();
        }


        private void InitBookingLists()
        {
            _lists = new List<BookingList>();
            if (_occ == null)
                return;

            // Ohne Einschränkungen => offene Liste => alle Eintragungen übernehmen
            if (!_occ.SeatQuotas.Any())
            {
                var list = new BookingList("Offene Liste")
                {
                    SeatQuota = null,
                    Occurrence = _occ
                };

                foreach (var subscription in _occ.Subscriptions)
                {
                    var student = GetStudent(subscription.UserId);
                    var booking = new Data.Booking
                    {
                        Student = student,
                        Subscription = subscription
                    };

                    list.Bookings.Add(booking);
                }

                _lists.Add(list);
                return;
            }

            // bei Kontingenten kann es auch zu verlorenen Einträgen kommen => separat speichern
            var lostBookings = new BookingList("Ohne Zuordnung")
            {
                IsLost = true,
                SeatQuota = null,
                Occurrence = _occ
            };

            // hier jedes Platzkontingent einzeln
            foreach (var quota in _occ.SeatQuotas)
            {
                var list = new BookingList(quota.Summary)
                {
                    SeatQuota = quota,
                    Occurrence = _occ
                };

                /*
                if (quota.Fractions.Any())
                {
                    foreach (var fraction in quota.Fractions)
                    {
                        if (fraction.Curriculum != null)
                        {
                            list.Curricula.Add(fraction.Curriculum);
                        }
                    }
                }
                else
                {
                    if (quota.Curriculum != null)
                    {
                        list.Curricula.Add(quota.Curriculum);
                    }
                }
                */

                _lists.Add(list);
            }

            var quotaService = new QuotaService();
            foreach (var subscription in _occ.Subscriptions)
            {
                var student = GetStudent(subscription.UserId);
                if (student?.Curriculum == null) continue;


                var booking = new Data.Booking
                {
                    Student = student,
                    Subscription = subscription
                };

                // erster Versuch => passend zum Studiengang
                var theList = lostBookings;
                foreach (var quota in _occ.SeatQuotas)
                {
                    var list = _lists.FirstOrDefault(x => x.SeatQuota != null && x.SeatQuota.Id == quota.Id);
                    var check = quotaService.IsAvailable(list.SeatQuota, student.Curriculum, new List<ItemLabel>());
                    if (check.Success)
                    {
                        theList = list;
                        break;
                    }
                    /*
                        else
                        {
                            // verlorene Eintragung
                            lostBookings.Bookings.Add(booking);
                        }
                        */
                }

                theList.Bookings.Add(booking);

                /*
                    var list = _lists.FirstOrDefault(x => x.SeatQuota?.Curriculum != null && x.SeatQuota.Curriculum.Id == student.Curriculum.Id);
                    if (list == null)
                    {
                        // zweiter Versuch: offene Liste ohne Angabe Studiengang
                        list = _lists.FirstOrDefault(x => x.SeatQuota?.Curriculum == null);
                    }

                    if (list != null)
                    {
                        list.Bookings.Add(booking);
                    }
                    else
                    {
                        // verlorene Eintragung
                        lostBookings.Bookings.Add(booking);
                    }
                    */
            }

            if (lostBookings.Bookings.Any())
            {
                _lists.Add(lostBookings);
            }

            return;
        }

        public BookingList GetBookingList(Student student)
        {
            if (student?.Curriculum == null)
                return null;

            var quotaService = new QuotaService();
            if (_occ.SeatQuotas.Any())
            {
                foreach (var quota in _occ.SeatQuotas)
                {
                    var list = _lists.FirstOrDefault(x => x.SeatQuota != null && x.SeatQuota.Id == quota.Id);
                    var check = quotaService.IsAvailable(list.SeatQuota, student.Curriculum, new List<ItemLabel>());
                    if (check.Success)
                    {
                        return list;
                    }
                }

                return null;
            }
            else
            {
                return _lists.FirstOrDefault();
            }


            /*
            // welche Liste gehört exakt zum Studiengang des Studierenden
            var list = _lists.FirstOrDefault(x => x.SeatQuota?.Curriculum != null && x.SeatQuota.Curriculum.Id == student.Curriculum.Id);
            if (list != null)
            {
                return list;
            }

            // Einschränkung ohne Studiengang
            list = _lists.FirstOrDefault(x => x.SeatQuota?.Curriculum == null && !x.IsLost);
            if (list != null)
            {
                return list;
            }


            // gar keine Einschränken
            list = _lists.FirstOrDefault(x => x.SeatQuota == null && !x.IsLost);
            return list;
            */
        }

        public List<BookingList> GetBookingLists()
        {
            return _lists;
        }

        private Student GetStudent(string userId)
        {
            return Db.Students.Where(x => x.UserId.Equals(userId)).OrderByDescending(x => x.Created)
                .FirstOrDefault();
        }

        public BookingTicket Subscribe(string userId, Course course)
        {
            var ticket = new BookingTicket();

            var student = GetStudent(userId);

            ticket.Course = course;
            ticket.UserId = userId;

            var occurrence = course.Occurrence;
            OccurrenceSubscription subscription = null;

            using (var transaction = Db.Database.BeginTransaction())
            {
                subscription = occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));

                var bookingLists = GetBookingLists();
                var bookingState = new BookingState
                {
                    Student = student,
                    Occurrence = occurrence,
                    BookingLists = bookingLists,
                    MyBookingList = GetBookingList(student)
                };
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



