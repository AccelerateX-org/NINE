using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class CourseViewModel
    {
        public Course Course { get; set; }

        public OccurrenceStateModel State { get; set; }
    }


    public class CourseSummaryModel
    {
        public CourseSummaryModel()
        {
            Dates = new List<CourseDateModel>();
            Lecturers = new List<OrganiserMember>();
            Rooms = new List<Room>();
        }
        public Activity Course { get; set; }

        public bool IsBlock { get; set; }

        public List<CourseDateModel> Dates { get; set; }

        public List<OrganiserMember> Lecturers { get; set; }

        public List<Room> Rooms { get; set; }

        public OccurrenceStateModel State { get; set; }

        public SemesterGroup SemesterGroup { get; set; }

        public int SubscriptionCountFit { get; set; }
    }

    public class CourseHistoryModel
    {
        public Activity Course { get; set; }

        public Semester Semester { get; set; }
        
    }


    public class CourseDateModel
    {
        public CourseDateModel()
        {
            Rooms = new List<Room>();
            Lecturers = new List<OrganiserMember>();
        }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTime DefaultDate { get; set; }

        public List<Room> Rooms { get; set; }

        public List<OrganiserMember> Lecturers { get; set; }

    }

    public class CourseCharacteristicModel
    {
        public CourseCharacteristicModel()
        {
            Member = new List<CourseMemberModel>();
            CurrentDates = new List<CourseDateStateModel>();
            ExpiredDates = new List<CourseDateStateModel>();
        }

        public Course Course { get; set; }

        public CourseDateStateModel NextDate { get; set; }

        public List<CourseMemberModel> Member { get; set; }

        public OccurrenceStateModel State { get; set; }

        public List<CourseDateStateModel> CurrentDates { get; set; }
        public List<CourseDateStateModel> ExpiredDates { get; set; }


        public BinaryStorage ModuleDescription { get; set; }


        public string OccurrenceDescription
        {
            get
            {
                var sb = new StringBuilder();
                if (Course.Occurrence.Capacity < 0)
                    sb.Append("Keine Kapazitätsbeschränkungen");
                else
                    sb.AppendFormat("Anzahl Plätze: {0}", Course.Occurrence.Capacity);

                if (Course.Occurrence.FromIsRestricted)
                    sb.Append("Buchungsbeginn beschränkt");

                if (Course.Occurrence.UntilIsRestricted)
                    sb.Append("Buchungsende beschränkt");

                return sb.ToString();
            }
        }

        public bool IsWPM { get; set; }
    }

    public class CourseMemberModel
    {
        public int Number { get; set; }
        public ApplicationUser User { get; set; }
        public OccurrenceSubscription Subscription { get; set; } 

    }

    public enum SubscriptionState
    {
        BeforeSubscriptionPhase,
        DuringSubscriptionPhase,
        AfterSubscriptionPhase,
        DuringOccurrence,
        AfterOccurrence
    }

    public class CourseDateStateModel
    {
        public ActivityDateSummary Summary { get; set; }

        public SubscriptionState State { get; set; }

        public OccurrenceStateModel OccurrenceState { get; set; }
    }

    public class CourseMoveDateModel
    {
        public Course Course { get; set; }
        public ActivityDate Date { get; set; }

        public Guid ActivityDateId { get; set; }

        public Guid OrganiserId2 { get; set; }
        public Guid OrganiserId3 { get; set; }


        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        public ICollection<Guid> DozIds { get; set; }
        public ICollection<Guid> RoomIds { get; set; }
    }


    public class CourseCreateModel
    {
        public Guid CourseId { get; set; }

        public Guid OrgId { get; set; }

        [Display(Name="Bezeichnung")]
        public string Name { get; set; }

        [Display(Name="Kurzname")]
        public string ShortName { get; set; }

        [Display(Name="Semester")]
        public string Semester { get; set; }

        [Display(Name = "Wochentag im Semester")]
        public int DayOfWeek { get; set; }

        [Display(Name = "Beginn")]
        public int StartTimeHour { get; set; }
        public int StartTimeMinute { get; set; }

        [Display(Name = "Ende")]
        public int EndTimeHour { get; set; }
        public int EndTimeMinute { get; set; }

        [Display(Name = "Raumnummer")]
        public string Room { get; set; }

        [Display(Name = "Dozentenkürzel")]
        public string Lecturer { get; set; }

        [Display(Name =" Studiengruppe")]
        public string Group { get; set; }

        [Display(Name="Anlegen von Terminen")]
        public int DateOption { get; set; }

        [Display(Name="Beschreibung")]
        public string Description { get; set; }

        [Display(Name = "Datum EInzeltermin")]
        public DateTime Day { get; set; }
    
    }

    public class CourseDateInfoModel
    {
        public Course Course { get; set; }
        public ActivityDate Date { get; set; }

        public Guid ActivityDateId { get; set; }
    }

    public class CourseDeleteModel
    {
        public Course Course { get; set; }
    }


    public class CourseDocumentUploadModel
    {
        public Guid CourseId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Display(Name = "Datei")]
        public HttpPostedFileBase Document { get; set; }

    }

    public class CourseDetailViewModel
    {
        public Course Course { get; set; }

        public List<CourseDateModel> DateSummary { get; set; }


        public List<OrganiserMember> Lecturers { get; set; }

        public List<Room> Rooms { get; set; }

        public ActivityDate NextDate { get; set; }

        public List<SubscriptionDetailViewModel> ParticipantList { get; set; }

        public List<SubscriptionDetailViewModel> WaitingList { get; set; }

        public List<OccurrenceGroupCapacityModel> Groups { get; set; }

        public List<OccurrenceCapacityOption> CapacitySettings { get; set; }

        public OccurrenceCapacityOption SelectedOption { get; set; }

        public OccurrenceStateModel State { get; set; }
            
        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description2 { get; set; }

        public Guid CurriculumId { get; set; }
        public Guid OrganiserId { get; set; }

        public Guid GroupId { get; set; }

        public Guid SemesterId { get; set; }

        public int Capacity { get; set; }

        public string CourseType
        {
            get
            {
                return "Vorlesung";
                
            } 
        }

        public string DefaultDay
        {
            get
            {
                if (!DateSummary.Any())
                    return "Keine Termine";

                if (DateSummary.Count > 1)
                    return "Verschiedene";

                return DateSummary.First().DefaultDate.ToString("dddd", new CultureInfo("de-DE") );
            }
        }

        public string DefaultTimespan
        {
            get
            {
                if (!DateSummary.Any())
                    return "Keine Termine";

                if (DateSummary.Count > 1)
                    return "Verschiedene";


                return string.Format("{0}-{1}", 
                    DateSummary.First().StartTime.ToString(@"hh\:mm"),
                    DateSummary.First().EndTime.ToString(@"hh\:mm"));
            }
        }

        public string DefaultRoom
        {
            get
            {
                if (!DateSummary.Any())
                    return "Keine Termine";

                if (DateSummary.Count > 1)
                    return "Verschiedene";

                if (!Rooms.Any())
                    return "keine Raumangaben";

                var sb = new StringBuilder();
                foreach (var room in Rooms)
                {
                    sb.Append(room.Number);
                    if (room != Rooms.Last())
                    {
                        sb.Append(", ");
                    }
                }
                return sb.ToString();
            }
        }

        public string Title
        {
            get { return Course.Name; }
        }
    }

    public class CourseDateInformationModel
    {
        public Guid DateId { get; set; }

        [Display(Name="Titel")]
        public string Title { get; set; }

        [Display(Name="Beschreibung")]
        [AllowHtml]
        public string DateDescription { get; set; }

        [Display(Name = "Kurzinformation")]
        public string ShortInfo { get; set; }

        public ActivityDate Date { get; set; }
        
    }

    public class CourseDateCreatenModel
    {
        public Course Course { get; set; }

        public Guid CourseId { get; set; }

        public RoomListStateModel RoomList { get; set; }

        public LecturerListStateModel LecturerList { get; set; }

        public ICollection<OrganiserMember> Lecturers { get; set; }

        public ICollection<Room> Rooms { get; set; }


        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        public ICollection<Guid> RoomIds { get; set; }

        public ICollection<Guid> LecturerIds { get; set; }

        public bool IsWeekly { get; set; }

    }

    public class CourseCreateModel2
    {
        public Guid SemesterId { get; set; }
        
        public Guid OrganiserId { get; set; }
        public Guid OrganiserId2 { get; set; }

        public Guid OrganiserId3 { get; set; }

        public Guid CurriculumId { get; set; }

        public Guid GroupId { get; set; }



        public string ShortName { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> GroupIds { get; set; }
    }


    public class CourseCreateModelExtended
    {
        public string ShortName { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> GroupIds { get; set; }
        public ICollection<Guid> DozIds { get; set; }
        public ICollection<Guid> RoomIds { get; set; }

        public ICollection<string> Dates { get; set; }
    }

    public class CourseCreateModel3
    {
        public Semester Semester { get; set; }

        public Guid SemesterId { get; set; }

        public Guid OrgId { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> GroupIds { get; set; }

        public Course Course { get; set; }

        public Guid CourseId { get; set; }

        public Guid CurriculumId { get; set; }
        public Guid OrganiserId { get; set; }
        public Guid GroupId { get; set; }
    }

}