using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Booking.Data;

namespace MyStik.TimeTable.DataServices.Booking
{
    public enum BookingType
    {
        green,
        yellow,
        red
    }

    public class BookingState
    {
        private BookingList _myBookingList;

        public BookingState()
        {
            Reasons = new List<string>();
            IsAvailable = true;
            IsUnrestricted = true;
            AvailableSeats = int.MaxValue;
        }

        public List<BookingList> BookingLists { get; set; }

        public Occurrence Occurrence { get; set; }

        public Student Student { get; set; }

        public bool IsAvailable { get; private set; }

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
                return !Occurrence.HasHomeBias
                    ? BookingType.green
                    : (!Occurrence.IsCoterie ? BookingType.yellow : BookingType.red);
            }
        }

        public void Init()
        {
            if (Student != null)
            {
                switch (BookingType)
                {
                    case BookingType.green:
                    {
                        _myBookingList = BookingLists.FirstOrDefault();
                        break;
                    }
                    case BookingType.yellow:
                    {
                        var list = BookingLists.FirstOrDefault(x =>
                            x.Curricula.Any(c => c.Id == Student.Curriculum.Id));
                        if (list != null)
                        {
                            _myBookingList = list;
                        }
                        else
                        {
                            _myBookingList = BookingLists.FirstOrDefault(x => !x.Curricula.Any());
                        }

                        break;
                    }
                    case BookingType.red:
                    {
                        _myBookingList = BookingLists.FirstOrDefault(x =>
                            x.Curricula.Any(c => c.Id == Student.Curriculum.Id));
                        break;
                    }
                }
            }

            if (!Occurrence.IsAvailable)
            {
                IsAvailable = false;
                Reasons.Add("Eintragung ist von Admin-Seite aus gesperrt worden.");
            }

            if (BookingType == BookingType.red && _myBookingList == null)
            {
                IsAvailable = false;
                Reasons.Add("Diese Lehrveranstaltung steht für meinen Studiengang nicht zur Verfügung.");
            }

            if (_myBookingList != null && _myBookingList.Capacity >= 0)
            {
                IsUnrestricted = false;
                if (_myBookingList.Capacity == int.MaxValue)
                {
                    AvailableSeats = int.MaxValue;  // ohne Platzbescränkung ist die Anzahl der verfügbaren Plätze unerheblich
                }
                else
                {
                    AvailableSeats = _myBookingList.Capacity - _myBookingList.Participients.Count;
                }
            }
        }
    }
}
