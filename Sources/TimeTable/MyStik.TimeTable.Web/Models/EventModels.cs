using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class EventViewModel
    {
        public Event Event { get; set; }

        public ActivityDate NextDate { get; set; }

        public OccurrenceStateModel State { get; set; }
    }

    public class EventCreateModel
    {
        public Guid CourseId { get; set; }

        public Guid OrgId { get; set; }

        [Display(Name = "Bezeichnung")]
        public string Name { get; set; }

        [Display(Name = "Kurzname")]
        public string ShortName { get; set; }

        [Display(Name = "Beginn")]
        public int StartTimeHour { get; set; }
        public int StartTimeMinute { get; set; }

        [Display(Name = "Ende")]
        public int EndTimeHour { get; set; }
        public int EndTimeMinute { get; set; }

        [Display(Name = "Raumnummer")]
        public string Room { get; set; }

        [Display(Name = "Veranstalter (Person)")]
        public string Lecturer { get; set; }

        [Display(Name = " Studiengruppe")]
        public string Group { get; set; }

        [Display(Name = "Restriktion für Eintragungen")]
        public int SubscriptionOption { get; set; }

        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Display(Name = "Datum und Beginn")]
        public DateTime Day { get; set; }

        [Display(Name = "Interne Veranstaltung")]
        public bool IsInternal { get; set; }

        [Display(Name = "Kapazität")]
        public int Capacity { get; set; }

        [Display(Name = "Eintragungen ab")]
        public bool IsSubscriptionStartRestricted { get; set; }
        public DateTime SubscriptionStartDay { get; set; }
        public int SubscriptionStartHour { get; set; }
        public int SubscriptionStartMinute { get; set; }

        [Display(Name = "Eintragungen bis")]
        public bool IsSubscriptionEndRestricted { get; set; }
        public DateTime SubscriptionEndDay { get; set; }
        public int SubscriptionEndHour { get; set; }
        public int SubscriptionEndMinute { get; set; }
    }

    public class EventCreateModel2
    {
        public Semester Semester { get; set; }

        public Guid SemesterId { get; set; }

        public Guid OrgId { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> GroupIds { get; set; }

        public Event Event { get; set; }

        public Guid EventId { get; set; }
    }


    public class EventCreateSingleModel : EventCreateModel
    {
        [Display(Name = "Wochentag im Semester")]
        public int DayOfWeek { get; set; }
    }

    public class EventCreateSeriesModel : EventCreateModel // Model zur Übetragung von Seriendaten
    {
        public string MainFrequency { get; set; } // Hauptfrequenz, z.B. Täglich oder Jährlich
        public int countDates { get; set; } // 
        public DateTime EndDay { get; set; }
        public string endType { get; set; }
        public int dailyCount { get; set; }
        public int weeklyCount { get; set; }
        public string[] weeklyDays { get; set; }
        public int countMonthDays { get; set; }
        public int countMonths { get; set; }
        public int monthDays { get; set; }
        public int monthDaysKind { get; set; }
        public int yearCount { get; set; }
        public int yearMonth { get; set; }
        public int yearDayInMonth { get; set; }
        public int yearDayCount { get; set; }
        public int yearDayKind { get; set; }
    }


    public class EventCharacteristicModel
    {
        public EventCharacteristicModel()
        {
            Member = new List<CourseMemberModel>();
            CurrentDates = new List<CourseDateStateModel>();
            ExpiredDates = new List<CourseDateStateModel>();
        }

        public Event Course { get; set; }

        public CourseDateStateModel NextDate { get; set; }

        public List<CourseMemberModel> Member { get; set; }

        public OccurrenceStateModel State { get; set; }

        public List<CourseDateStateModel> CurrentDates { get; set; }
        public List<CourseDateStateModel> ExpiredDates { get; set; }


        public string OccurrenceDescription
        {
            get
            {
                var sb = new StringBuilder();
                if (NextDate.Summary.Date.Occurrence.Capacity < 0)
                    sb.Append("Keine Kapazitätsbeschränkungen");
                else
                    sb.AppendFormat("Anzahl Plätze: {0}", Course.Occurrence.Capacity);

                if (NextDate.Summary.Date.Occurrence.FromIsRestricted)
                    sb.Append("Buchungsbeginn beschränkt");

                if (NextDate.Summary.Date.Occurrence.UntilIsRestricted)
                    sb.Append("Buchungsende beschränkt");

                return sb.ToString();
            }
        }
    }

    public class EventDateInfoModel
    {
        public EventDateInfoModel()
        {
            Member = new List<CourseMemberModel>();
        }

        public Event Course { get; set; }
        public ActivityDate Date { get; set; }

        public Guid ActivityDateId { get; set; }

        public List<CourseMemberModel> Member { get; set; }
    }

    public class EventMoveDateModel
    {
        public Event Event { get; set; }
        public ActivityDate Date { get; set; }

        public RoomListStateModel RoomList { get; set; }

        public LecturerListStateModel LecturerList { get; set; }

        public ICollection<OrganiserMember> Lecturers { get; set; }

        public ICollection<Room> Rooms { get; set; }

        public Guid ActivityId { get; set; }

        public Guid ActivityDateId { get; set; }

        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        public ICollection<Guid> RoomIds { get; set; }

        public ICollection<Guid> LecturerIds { get; set; }
    }

    public class EventDeleteModel
    {
        public Event Course { get; set; }
    }

    public class EventDetailViewModel
    {
        public Event Event { get; set; }

        public List<OccurrenceGroupCapacityModel> Groups { get; set; }

        public List<OrganiserMember> Lecturers { get; set; }

        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description2 { get; set; }



        public string Curriculum { get; set; }
        public string Group { get; set; }
    }



    public class EventMobileViewModel
    {   
        public ICollection<Milan> DatumFeiertage { get; set; }


        public string Faculty { get; set; }

       


       
        public ICollection<SemesterDate> SemesterDate { get; set; }
        public ICollection<EventStateMobileViewModel> Events { get; set; }
       
        public ICollection<EventMilan> EventVeranstaltung { get; set; }
    }



      


        public class EventStateMobileViewModel
        {
            public Event Event { get; set; }
            public string Dates { get; set; }
            public string Description { get; set; }
        }


        public class Milan
        {
            public Semester Semester { get; set; } 
            public DateTime Datum { get; set; }

            public string Feiertage { get; set; }
        }

        public class EventMilan
        {
            public Event Event { get; set; } 


        }

}