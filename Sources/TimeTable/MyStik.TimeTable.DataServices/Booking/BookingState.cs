﻿using System.Collections.Generic;
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
        public BookingState()
        {
            Reasons = new List<string>();
            IsAvailable = true;
            IsUnrestricted = true;
            AvailableSeats = int.MaxValue;
        }

        public List<BookingList> BookingLists { get; set; }

        public BookingList LostBookings { get; set; }

        public Occurrence Occurrence { get; set; }

        public Student Student { get; set; }

        public bool IsAvailable { get; private set; }

        public bool IsUnrestricted { get; private set; }

        public int AvailableSeats { get; private set; }

        public List<string> Reasons { get; }

        public BookingList MyBookingList { get; set; }

        public void Init()
        {
            if (!Occurrence.IsAvailable)
            {
                IsAvailable = false;
                Reasons.Add("Eintragung ist von Admin-Seite aus gesperrt worden.");
            }

            if (MyBookingList == null)
            {
                IsAvailable = false;
                Reasons.Add("Diese Lehrveranstaltung steht für meinen Studiengang nicht zur Verfügung.");
            }

            if (MyBookingList != null && MyBookingList.Capacity >= 0)
            {
                IsUnrestricted = false;
                if (MyBookingList.Capacity == int.MaxValue)
                {
                    AvailableSeats = int.MaxValue;  // ohne Platzbescränkung ist die Anzahl der verfügbaren Plätze unerheblich
                }
                else
                {
                    AvailableSeats = MyBookingList.Capacity - MyBookingList.Participients.Count;
                }
            }
        }
    }
}
