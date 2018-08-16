using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EventViewModel
    {
        public EventViewModel()
        {
            Dates = new List<EventDateStateViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Event Event { get; set; }

        public List<EventDateStateViewModel> Dates { get; private set; }
    }

    public class EventDateStateViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class EventCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrgId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Bezeichnung")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Kurzname")]
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beginn")]
        public int StartTimeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StartTimeMinute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende")]
        public int EndTimeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EndTimeMinute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Raumnummer")]
        public string Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Veranstalter (Person)")]
        public string Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Fakultät")]
        public string Faculty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = " Studiengruppe")]
        public string Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Restriktion für Eintragungen")]
        public int SubscriptionOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum und Beginn")]
        public DateTime Day { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Interne Veranstaltung")]
        public bool IsInternal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Kapazität")]
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Eintragungen ab")]
        public bool IsSubscriptionStartRestricted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime SubscriptionStartDay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionStartHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionStartMinute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Eintragungen bis")]
        public bool IsSubscriptionEndRestricted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime SubscriptionEndDay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionEndHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionEndMinute { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventCreateModel2
    {
        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Kurzbeschreibung { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Titel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Room> Raum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Raumnummer { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public List<ActivityOrganiser> Fakultät { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Beschreibung { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display (Name ="Datum")]
        public DateTime DatumEvent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AmInfoscreenAnzeigen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Buchen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> GroupIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CurriculumId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GroupId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventCreateSingleModel : EventCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wochentag im Semester")]
        public int DayOfWeek { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventCreateSeriesModel : EventCreateModel // Model zur Übetragung von Seriendaten
    {
        /// <summary>
        /// 
        /// </summary>
        public string MainFrequency { get; set; } // Hauptfrequenz, z.B. Täglich oder Jährlich

        /// <summary>
        /// 
        /// </summary>
        public int countDates { get; set; } // 

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string endType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int dailyCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int weeklyCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[] weeklyDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int countMonthDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int countMonths { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int monthDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int monthDaysKind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int yearCount { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int yearMonth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int yearDayInMonth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int yearDayCount { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int yearDayKind { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventCharacteristicModel
    {
        /// <summary>
        /// 
        /// </summary>
        public EventCharacteristicModel()
        {
            Member = new List<CourseMemberModel>();
            CurrentDates = new List<CourseDateStateModel>();
            ExpiredDates = new List<CourseDateStateModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Event Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseDateStateModel NextDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseMemberModel> Member { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseDateStateModel> CurrentDates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseDateStateModel> ExpiredDates { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public class EventDateInfoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public EventDateInfoModel()
        {
            Member = new List<CourseMemberModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Event Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityDateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseMemberModel> Member { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public class EventDateCreateModelExtended
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> DozIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> RoomIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<string> Dates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventMoveDateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RoomListStateModel RoomList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LecturerListStateModel LecturerList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OrganiserMember> Lecturers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Room> Rooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityDateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> RoomIds { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> DozIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> LecturerIds { get; set; }

        public Guid OrganiserId2 { get; set; }
        public Guid OrganiserId3 { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventDateCreatenModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Activity Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RoomListStateModel RoomList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LecturerListStateModel LecturerList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OrganiserMember> Lecturers { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ICollection<Room> Rooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> RoomIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> LecturerIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWeekly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId3 { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public class EventDeleteModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Event Course { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventDetailViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OccurrenceGroupCapacityModel> Groups { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OrganiserMember> Lecturers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description2 { get; set; }


        public ActivityDate Date { get; set; }

        public Guid OrganiserId { get; set; }

    }


    /// <summary>
    /// 
    /// </summary>
    public class EventMobileViewModel
    {   
        /// <summary>
        /// 
        /// </summary>
        public ICollection<Milan> DatumFeiertage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Faculty { get; set; }

       

        /// <summary>
        /// 
        /// </summary>
       
        public ICollection<SemesterDate> SemesterDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<EventStateMobileViewModel> Events { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public ICollection<EventMilan> EventVeranstaltung { get; set; }
    }



      

    /// <summary>
    /// 
    /// </summary>
    public class EventStateMobileViewModel
    {

        /// <summary>
        /// 
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Dates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public class Milan
    {
        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Feiertage { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventMilan
    {
        /// <summary>
        /// 
        /// </summary>
        public Event Event { get; set; } 


    }

}