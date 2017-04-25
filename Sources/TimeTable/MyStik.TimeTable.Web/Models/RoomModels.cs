using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class FreeRoomModel
    {
        public Room Room { get; set; }

        public IEnumerable<ActivityDate> CurrentDates { get; set; }

        public ActivityDate LastDate { get; set; }

        public ActivityDate NextDate { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

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

    public class RoomCharacteristicModel
    {
        public RoomCharacteristicModel()
        {
            Courses = new List<CourseSummaryModel>();
            CurrentOccurrences = new List<ActivityDate>();
            NextOccurrences = new List<ActivityDate>();
        }

        public Room Room { get; set; }

        public List<ActivityDate> CurrentOccurrences { get; set; }

        public List<ActivityDate> NextOccurrences { get; set; }

        public List<CourseSummaryModel> Courses { get; set; }

    }

    public class RoomStateModel
    {
        public RoomStateModel()
        {
            Conflicts = new List<ActivityDate>();
        }

        public Room Room { get; set; }

        public bool HasConflicts { get { return Conflicts.Any(); }}

        public IEnumerable<ActivityDate> Conflicts { get; set; }
    }

    public class RoomListStateModel
    {
        public RoomListStateModel()
        {
            RoomStates = new List<RoomStateModel>();
        }

        public Guid ActivityDateId { get; set; }

        public List<RoomStateModel> RoomStates { get; set; }
    }

    public class RoomLookUpModel
    {
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public int BeginHour { get; set; }
        public int BeginMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }

        public int DayOfWeek { get; set; }

        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }

    public class RoomDeleteModel
    {
        public Room Room { get; set; }
    }

    public class RoomTransferModel
    {
        [Display(Name = "Aus diesem Raum sollen alle Belegungen entfernt werden")]
        public Guid SourceRoomId { get; set; }

        [Display(Name = "Diesem Raum sollen alle Belegungen zugeorndet werden")]
        public Guid TargetRoomId { get; set; }
    }

    public class RoomInfoModel
    {
        public Room Room { get; set; }

        public ActivityDate CurrentDate { get; set; }

        public ActivityDate NextDate { get; set; }

        public bool NeedInternalConfirmation { get; set; }

    }

    public class FreeRoomSummaryModel
    {
        public ICollection<RoomInfoModel> AvailableRooms { get; set; }
        public ICollection<RoomInfoModel> FutureRooms { get; set; }
    }

    public class RoomOccModel
    {
        public Guid RoomId { get; set;  }
        public string State { get; set; }
    }

    public class RoomMobileViewModel
    {
        public string Faculty { get; set;  }

        public ICollection<RoomMobileStateViewModel> SeminarRooms { get; set; }

        public ICollection<RoomMobileStateViewModel2> EDVRooms { get; set; }

        public ICollection<RoomMobileStateViewModel3> AllRooms { get; set; }

        
    }

    public class RoomMobileStateViewModel //Das hier ist die Klasse für die Seminarräume
    {
        public Room Room { get; set; }

        public int AnzahlTische { get; set; }

        public string Status { get; set; }
        
    }

    public class RoomMobileStateViewModel2 //EDV-Räume
    {
        public Room Room { get; set; }

        public int AnzahlTische { get; set; }

        public string Status { get; set; }
    }

    public class RoomMobileStateViewModel3 //Alle Räume
    {
        public Room Room { get; set; }

        public int AnzahlTische { get; set; }

        public string Beschreibung { get; set; }

        
    }

    public class RoomConflictViewModel
    {
        public RoomConflictViewModel()
        {
            ConflictDates = new List<RoomDateConflictViewModel>();
        }

        public Room Room { get; set; }

        public List<RoomDateConflictViewModel> ConflictDates { get; private set; }
    }

    public class RoomDateConflictViewModel
    {
        public RoomDateConflictViewModel()
        {
            Conflicts = new List<ActivityDate>();
        }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public List<ActivityDate> Conflicts { get; private set; }
    }

    

}
