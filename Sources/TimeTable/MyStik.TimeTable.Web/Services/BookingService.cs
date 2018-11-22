using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    public enum BookingType
    {
        green,
        yellow,
        red
    }

    public class Booking
    {
        public OccurrenceSubscription Subscription { get; set; }

        public Student Student { get; set; }
    }

    public class BookingList
    {
        public BookingList()
        {
            Bookings = new List<Booking>();
            Curricula = new List<Curriculum>();
        }

        public Occurrence Occurrence { get; set; }

        public List<Curriculum> Curricula { get; set; }

        public OccurrenceGroup Group { get; set; }

        public List<Booking> Bookings { get; }

        /// <summary>
        /// Anzahl der Pläatze
        /// </summary>
        public int Capacity
        {
            get
            {
                if (Group != null)
                    return Group.Capacity;
                return Occurrence.Capacity;
            }
        }

        public List<Booking> Participients
        {
            get { return Bookings.Where(x => !x.Subscription.OnWaitingList).ToList(); }
        }

        public List<Booking> Waitinglist
        {
            get { return Bookings.Where(x => x.Subscription.OnWaitingList).ToList(); }
        }

    }

    public class BookingState
    {
        private BookingList _myBookingList;

        public List<BookingList> BookingLists { get; set; }


        public Occurrence Occurrence { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// Verfügbar für Eintragung oder nicht
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>
        /// Keine Einschränkung hinsichtlich Platzangebots
        /// </summary>
        public bool IsUnrestricted { get; private set; }

        public int AvailableSeats { get; private set; }

        public List<string> Reasons { get; }

        public BookingList MyBookingList
        {
            get { return _myBookingList; }
        }

        public BookingType BookingType
        {
            get
            {
                if (Occurrence.HasHomeBias)
                {
                    if (Occurrence.IsCoterie)
                    {
                        return BookingType.red;
                    }

                    return BookingType.yellow;
                }

                return BookingType.green;
            }
        }


        public BookingState()
        {
            IsAvailable = true;
            IsUnrestricted = true;
            AvailableSeats = Int32.MaxValue;
            Reasons = new List<string>();
        }


        public void Init()
        {
            // Die Buchungsliste suchen
            switch (BookingType)
            {
                case BookingType.green:
                    _myBookingList = BookingLists.First();
                    break;
                case BookingType.yellow:
                    _myBookingList = BookingLists.FirstOrDefault(x => x.Curricula.Any(c => c.Id == Student.Curriculum.Id)) ??
                                     BookingLists.FirstOrDefault(x => !x.Curricula.Any());
                    break;
                case BookingType.red:
                    _myBookingList = BookingLists.FirstOrDefault(x => x.Curricula.Any(c => c.Id == Student.Curriculum.Id));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            // Verfügbarkeit
            // Eitragung gesperrt
            if (!Occurrence.IsAvailable)
            {
                IsAvailable = false;
                Reasons.Add("Eintragung ist von Adminseite aus gesperrt");
            }

            // Ampelsystem
            // rot = steht nur für Studiengänge zur Verfügung
            if (BookingType == BookingType.red && _myBookingList == null)
            {
                IsAvailable = false;
                Reasons.Add("Diese Lehrveranstaltung steht für meinen Studiengang nicht zur Verfügung");
            }

            // Platzangebot
            if (_myBookingList != null)
            {
                if (_myBookingList.Capacity >= 0)
                {
                    IsUnrestricted = false;
                    AvailableSeats = _myBookingList.Capacity - _myBookingList.Participients.Count;
                }
            }
        }
    }


    public class BookingService : BaseService
    {
        private Occurrence _occ;
        private List<BookingList> _list;
        private BookingList _globalMiscList;

        public BookingService(TimeTableDbContext db, Guid occId) : base(db)
        {
            _occ = Db.Occurrences.SingleOrDefault(x => x.Id == occId);
            _list = new List<BookingList>();
        }

        public BookingType BookingType
        {
            get
            {
                if (_occ.HasHomeBias)
                {
                    if (_occ.IsCoterie)
                    {
                        return BookingType.red;
                    }

                    return BookingType.yellow;
                }

                return BookingType.green;
            }
        }


        public List<BookingList> GetBookingLists()
        {
            if (BookingType == BookingType.green)
            {
                if (_occ.UseGroups)
                {
                    CreateListByCurriculum();
                    CreateMiscList();
                }
                else
                {
                    CreateMiscList();
                }
            }
            else
            {
                if (_occ.UseGroups)
                {
                    CreateListByCurriculum();
                    if (BookingType == BookingType.yellow)
                    {
                        CreateMiscList();
                    }
                }
                else
                {
                    CreateJoinedList();
                    if (BookingType == BookingType.yellow)
                    {
                        CreateMiscList();
                    }
                }
            }

            foreach (var subscription in _occ.Subscriptions)
            {
                var student = GetStudent(subscription.UserId);

                var booking = new Booking
                {
                    Subscription = subscription,
                    Student = student
                };

                var list = _list.FirstOrDefault(x => x.Curricula.Any(c => c.Id == student.Curriculum.Id));
                if (list == null)
                {
                    _globalMiscList.Bookings.Add(booking);
                }
                else
                {
                    list.Bookings.Add(booking);
                }
            }


            return _list;
        }

        /// <summary>
        /// Auf dieser Liste sind alle Einträge aus Studiengängen
        /// die nicht zugeordnet sind
        /// </summary>
        private void CreateMiscList()
        {
            var bookingList = new BookingList
            {
                Occurrence = _occ
            };

            _list.Add(bookingList);
            _globalMiscList = bookingList;
    }

    /// <summary>
    /// Ein Liste mit allen Studiengängen
    /// </summary>
    private void CreateJoinedList()
        {
            var bookingList = new BookingList
            {
                Occurrence = _occ
            };

            foreach (var occurrenceGroup in _occ.Groups)
            {
                var cur = occurrenceGroup.SemesterGroups.First().CapacityGroup.CurriculumGroup.Curriculum;

                if (!bookingList.Curricula.Contains(cur))
                {
                    bookingList.Curricula.Add(cur);
                }
            }

            _list.Add(bookingList);
        }

        /// <summary>
        /// Eine Liste pro Studiengang
        /// </summary>
        private void CreateListByCurriculum()
        {
            foreach (var occurrenceGroup in _occ.Groups)
            {
                // Annahme: in einer OccurrenceGroup stecken nur Semestergruppen eines Studiengangs!
                var cur = occurrenceGroup.SemesterGroups.First().CapacityGroup.CurriculumGroup.Curriculum;
                var bookingList = _list.SingleOrDefault(x => x.Curricula.Any(c => c.Id == cur.Id));

                if (bookingList == null)
                {
                    bookingList = new BookingList
                    {
                        Group = occurrenceGroup,
                        Occurrence = _occ
                    };

                    bookingList.Curricula.Add(cur);
                    _list.Add(bookingList);
                }
            }
        }


        private Student GetStudent(string userId)
        {
            return new StudentService(Db).GetCurrentStudent(userId);
        }
    }
}