using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class FreeRoomModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ActivityDate> CurrentDates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate LastDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime To { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Duration
        {
            get
            {
                if (NextDate == null)
                    return int.MaxValue;

                var dur = NextDate.Begin - To;
                return (int) dur.TotalMinutes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DurationText
        {
            get
            {
                if (NextDate == null)
                    return "unendlich";

                var dur = NextDate.Begin - To;
                return dur.ToString(@"d\:hh\:mm");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomCharacteristicModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomCharacteristicModel()
        {
            Courses = new List<CourseSummaryModel>();
            CurrentOccurrences = new List<ActivityDate>();
            NextOccurrences = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> CurrentOccurrences { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> NextOccurrences { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseSummaryModel> Courses { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomStateModel()
        {
            Conflicts = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasConflicts { get { return Conflicts.Any(); }}

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ActivityDate> Conflicts { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomListStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomListStateModel()
        {
            RoomStates = new List<RoomStateModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityDateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RoomStateModel> RoomStates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomLookUpModel
    {
        public ActivityOrganiser Organiser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Date1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BeginHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BeginMinute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EndHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EndMinute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DayOfWeek { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Startdatum")]
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
        [Display(Name = "Enddatum")]
        public string NewDate2 { get; set; }


        public bool IsMonday { get; set; }
        public bool IsTuesday { get; set; }
        public bool IsWednesday { get; set; }
        public bool IsThursday { get; set; }
        public bool IsFriday { get; set; }
        public bool IsSaturday { get; set; }
        public bool IsSunday { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ICollection<Room> Rooms { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomDeleteModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomTransferModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Aus diesem Raum sollen alle Belegungen entfernt werden")]
        public Guid SourceRoomId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Diesem Raum sollen alle Belegungen zugeordnet werden")]
        public Guid TargetRoomId { get; set; }

        [Display(Name = "Von Datum")]
        public string StartDate { get; set; }

        [Display(Name = "Uhrzeit")]
        public string StartTime { get; set; }


        [Display(Name = "Bis Datum")]
        public string EndDate { get; set; }

        [Display(Name = "Uhrzeit")]
        public string EndTime { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomInfoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate CurrentDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextDate { get; set; }

        public ActivityDate PreviousDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool NeedInternalConfirmation { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class FreeRoomSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomInfoModel> AvailableRooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomInfoModel> FutureRooms { get; set; }
    }

    public class RoomSearchModel
    {
        public DateTime Date { get; set; }
        public TimeSpan Begin { get; set; }
        public TimeSpan End { get; set; }

        public string OrgName { get; set; }

        public string CampusName { get; set; }

        public string BuildingName { get; set; }

        public int MinCapacity { get; set; }

        public int MaxCapacity { get; set; }
    }

    public class RoomSearchModelApi
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string OrgName { get; set; }

        public string CampusName { get; set; }

        public string BuildingName { get; set; }

        public int MinCapacity { get; set; }

        public int MaxCapacity { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class RoomOccModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid RoomId { get; set;  }
        
        /// <summary>
        /// 
        /// </summary>
        public string State { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomMobileViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Faculty { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomMobileStateViewModel> SeminarRooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomMobileStateViewModel2> EDVRooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomMobileStateViewModel3> AllRooms { get; set; }

        
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomMobileStateViewModel //Das hier ist die Klasse für die Seminarräume
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AnzahlTische { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomMobileStateViewModel2 //EDV-Räume
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AnzahlTische { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomMobileStateViewModel3 //Alle Räume
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AnzahlTische { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Beschreibung { get; set; }

        
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomConflictViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomConflictViewModel()
        {
            ConflictDates = new List<RoomDateConflictViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RoomDateConflictViewModel> ConflictDates { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomDateConflictViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomDateConflictViewModel()
        {
            Conflicts = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> Conflicts { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomActivityModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> Dates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomDateSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomDateSummaryModel()
        {
            Dates = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Activity Activity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SlotCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> Dates { get; }
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomScheduleModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomScheduleModel()
        {
            RegularDates = new List<RoomDateSummaryModel>();
            SingleDates = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RoomDateSummaryModel> RegularDates { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> SingleDates { get; }
    }

    public class RoomSearchResultModel
    {
        public TimeSpan Begin { get; set; }

        public TimeSpan End { get; set; }

        public List<DateTime> DayList { get; set; }

        public List<Room> Rooms { get; set; }
    }


    public class StudyRoomViewModel
    {
        public ActivityOrganiser Organiser { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

    }

}
