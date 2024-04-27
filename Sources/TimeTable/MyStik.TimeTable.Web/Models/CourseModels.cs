using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseSummaryModel
    {
        /// <summary> 
        /// </summary>
        public CourseSummaryModel()
        {
            Dates = new List<CourseDateModel>();
            Lecturers = new List<OrganiserMember>();
            Rooms = new List<Room>();
            VirtualRooms = new List<VirtualRoom>();
            Curricula = new List<Curriculum>();
            ConflictingDates = new Dictionary<ActivityDate, List<ActivityDate>>();
            Blocks = new List<CourseBlockModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Der sich gerade informiert
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseDateModel> Dates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OrganiserMember> Lecturers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Room> Rooms { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<VirtualRoom> VirtualRooms { get; set; }

        /// <summary>
        /// Die beteiligten Studiengänge
        /// </summary>
        public List<Curriculum> Curricula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        public bool ShowFaculty { get; set; }

        public bool IsSelectable
        {
            get { return Course.Occurrence.IsAvailable; }
        }


        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// Eine bestehende Einschreibung
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        // public SemesterGroup SemesterGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionCountFit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivitySummary Summary { get; set; }


        public List<ActivityDate> FittingDates { get; set; }
        public List<ActivityDate> NonFittingDates { get; set; }
        public Dictionary<ActivityDate, List<ActivityDate>> ConflictingDates { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string CourseType
        {
            get
            {
                return "Vorlesung";

            }
        }

        public CourseDateModel GetDefaultDate()
        {
            return Dates.OrderByDescending(x => x.Dates.Count).FirstOrDefault();
        }


        /// <summary>
        /// 
        /// </summary>
        public string DefaultDay
        {
            get
            {
                if (!Dates.Any())
                    return "Keine Termine";

                if (Dates.Count > 1)
                    return "Verschiedene";

                return Dates.First().DefaultDate.ToString("dddd", new CultureInfo("de-DE"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultTimespan
        {
            get
            {
                if (!Dates.Any())
                    return "Keine Termine";

                if (Dates.Count > 1)
                    return "Verschiedene";


                return string.Format("{0}-{1}",
                    Dates.First().StartTime.ToString(@"hh\:mm"),
                    Dates.First().EndTime.ToString(@"hh\:mm"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultRoom
        {
            get
            {
                if (!Dates.Any())
                    return "Keine Termine";

                if (Dates.Count > 1)
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

        public string DefaultVirtualRoom
        {
            get
            {
                if (!Dates.Any())
                    return "Keine Termine";

                if (Dates.Count > 1)
                    return "Verschiedene";

                if (!VirtualRooms.Any())
                    return "keine Raumangaben";

                var sb = new StringBuilder();
                foreach (var room in VirtualRooms)
                {
                    sb.Append(room.Name);
                    if (room != VirtualRooms.Last())
                    {
                        sb.Append(", ");
                    }
                }
                return sb.ToString();
            }
        }


        private ActivityDate _currentDate;

        public ActivityDate CurrentDate
        {
            get
            {
                return _currentDate ?? (_currentDate = Course.Dates.Where(x => x.End >= DateTime.Now)
                           .OrderBy(x => x.Begin).FirstOrDefault());
            }
        }

        public bool IsHost(string userId)
        {
            return Lecturers.Any(x => !string.IsNullOrEmpty(x.UserId) && x.UserId.Equals(userId));
        }

        public bool IsSubscriber(string userId)
        {
            return Course.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId));
        }

        public OccurrenceSubscription GetSubscription(string userId)
        {
            return Course.Occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userId));
        }

        public List<CourseBlockModel> Blocks { get; set; }

        public bool IsPureBlock()
        {
            return Blocks.Count() == 1;
        }

        public bool IsPureRegular()
        {
            return Course.Dates.Count() == Blocks.Count();
        }

        public bool IsPureWeekEndCourse()
        {
            var nWeekEnds =
                Blocks.Count(x => x.Dates.Any(d => d.Begin.DayOfWeek == DayOfWeek.Saturday || d.Begin.DayOfWeek == DayOfWeek.Sunday));

            return Blocks.Count() == nWeekEnds;
        }

        public SemesterDate GetSemesterSegment(Semester semester)
        {
            var block = Blocks.FirstOrDefault();
            if (block == null) { return null; }

            var orderedDates = block.Dates.OrderBy(x => x.Begin).ToList();

            foreach (var segement in semester.Dates.Where(x => x.HasCourses))
            {
                if (segement.From <= orderedDates.First().Begin && 
                    orderedDates.Last().End <= segement.To)
                    return segement;
            }

            return null;
        }

        public Room GetFavoriteRoom()
        {
            var maxUses = 1;
            Room favRoom = null;

            foreach (var room in Rooms)
            {
                var nUses = Course.Dates.Count(x => x.Rooms.Any(r => r.Id == room.Id));
                if (nUses > maxUses)
                {
                    maxUses = nUses;
                    favRoom = room;
                }
            }

            return favRoom;
        }

        public OrganiserMember GetFavoriteHost()
        {
            var maxUses = 1;
            OrganiserMember favMember = null;

            foreach (var host in Lecturers)
            {
                var nUses = Course.Dates.Count(x => x.Hosts.Any(r => r.Id == host.Id));
                if (nUses > maxUses)
                {
                    maxUses = nUses;
                    favMember = host;
                }
            }

            return favMember;
        }



    }

    public class CourseBlockModel
    {
        public CourseBlockModel()
        {
            Dates = new List<ActivityDate>();
        }

        public List<ActivityDate> Dates { get; set; }
    
    }


    /// <summary>
    /// 
    /// </summary>
    public class TopicSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SemesterTopic Topic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseSummaryModel> Courses { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseHistoryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Activity Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }
     
        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CourseDateModel()
        {
            Rooms = new List<Room>();
            Lecturers = new List<OrganiserMember>();
            Dates = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DefaultDate => Dates.First().Begin;

        /// <summary>
        /// Liste der konkreten Termine
        /// </summary>
        public List<ActivityDate> Dates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Room> Rooms { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<OrganiserMember> Lecturers { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseCharacteristicModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CourseCharacteristicModel()
        {
            Member = new List<CourseMemberModel>();
            CurrentDates = new List<CourseDateStateModel>();
            ExpiredDates = new List<CourseDateStateModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

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
        public BinaryStorage ModuleDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public bool IsWPM { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseMemberModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; } 

        public Student Student { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public enum SubscriptionState
    {
        /// <summary>
        /// 
        /// </summary>
        BeforeSubscriptionPhase,
        /// <summary>
        /// 
        /// </summary>
        DuringSubscriptionPhase,
        /// <summary>
        /// 
        /// </summary>
        AfterSubscriptionPhase,
        /// <summary>
        /// 
        /// </summary>
        DuringOccurrence,
        /// <summary>
        /// 
        /// </summary>
        AfterOccurrence
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDateStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityDateSummary Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SubscriptionState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel OccurrenceState { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseMoveDateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Activity Course { get; set; }

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
        public Guid OrganiserId2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId3 { get; set; }

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
        public ICollection<Guid> DozIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> RoomIds { get; set; }

        [Display(Name = "Beschreibung")]
        [AllowHtml]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Titel")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Kurzinformation")]
        public string ShortInfo { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseCreateModel
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
        [Display(Name="Bezeichnung")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Kurzname")]
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Semester")]
        public string Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wochentag im Semester")]
        public int DayOfWeek { get; set; }

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
        [Display(Name = "Dozentenkürzel")]
        public string Lecturer { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name =" Studiengruppe")]
        public string Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Anlegen von Terminen")]
        public int DateOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum EInzeltermin")]
        public DateTime Day { get; set; }
    
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDateInfoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityDateId { get; set; }

        public CourseSummaryModel Summary { get; set; }

        public ActivityOrganiser Organiser { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDeleteModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DeleteCancelled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DeleteHosting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool KeepOwnership { get; set; }

        public string Code { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDocumentUploadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datei")]
        public HttpPostedFileBase Document { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDetailViewModel
    {
        public CourseDetailViewModel()
        {
            DatesThisWeek = new List<ActivityDate>();
            DatesNextWeek = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        public CourseSummaryModel Summary { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public List<Institution> Institutions { get; set; }

        /// <summary>
        /// Die zugehörige Platzverlosung (falls vorhanden)
        /// </summary>
        public Lottery Lottery { get; set; }


        //public CurriculumRequirement Module { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextDate { get; set; }

        public List<ActivityDate> DatesThisWeek { get; set;  }
        public List<ActivityDate> DatesNextWeek { get; set;  }


        [AllowHtml]
        public string Description2 { get; set; }

        public int Capacity { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public OccurrenceCapacityOption SelectedOption { get; set; }


        [Display(Name="Nur für Studierende der angegebenen Studiengänge")]
        public bool IsCoterie { get; set; }

        [Display(Name = "Studierende der angegeben Studiengänge werden bevorzugt")]
        public bool HasHomeBias { get; set; }

        public  int optionsAccess { get; set; }

        public int optionsLimit { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDateInformationModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid DateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Titel")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Beschreibung")]
        [AllowHtml]
        public string DateDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Kurzinformation")]
        public string ShortInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDateCreatenModel
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
    public class CourseCreateModel2
    {
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
        public Guid OrganiserId2 { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CurriculumId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CurrGroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CapGroupId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Guid TopicId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ChapterId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> GroupIds { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseCreateModelExtended
    {
        public bool showDetails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> GroupIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> TopicIds { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }


        public Guid OrganiserId { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class CourseDateCreateModelExtended
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
    public class CourseCreateModel3
    {
        public CourseSummaryModel Summary { get; set; }


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
        public Guid OrgId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Guid> GroupIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CurriculumId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid GroupId { get; set; }
    }

    public class CourseLabelViewModel
    {
        public Course Course { get; set; }

        public List<Curriculum> Curricula { get; set;}

        public List<ActivityOrganiser> Organisers { get; set; }
    }

    public class CourseStateModel
    {
        public Course Course { get; set; }

        public Guid CourseId { get;set; }

        public int locked { get; set; }

        public int editable { get; set; }

        public int projected { get; set; }
    }


    public class CopyCourseModel
    {
        public ModuleSubject Subject { get; set; }

        public Course OriginCourse { get; set; }

        public Semester SourceSemester { get; set; }

        public Course Clone { get; set; }

        public Semester DestSemester { get; set; }

        public bool WithDates { get; set; }
    }

    public class CreateSubjectCourseModel
    {
        public ModuleSubject Subject { get; set; }
        
        public Semester DestSemester { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public Course DestCourse { get; set; }

    }
}