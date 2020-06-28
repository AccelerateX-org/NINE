using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourSummaryModel()
        {
            Dates = new List<CourseDateModel>();
            Lecturers = new List<OrganiserMember>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseDateModel> Dates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OrganiserMember> Lecturers { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourCharacteristicModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourCharacteristicModel()
        {
            CurrentSlots = new List<OfficeHourSlotViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OfficeHourSlotViewModel> CurrentSlots { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourDateSlotViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourDateSlotViewModel()
        {
            Slots = new List<SingleSlotViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate CurrentDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate PreviousDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SingleSlotViewModel> Slots { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SlotCapacity
        {
            get
            {
                if (CurrentDate.Slots.Any())
                {
                    var slot = CurrentDate.Slots.First();

                    switch (slot.Occurrence.Capacity)
                    {
                        case -1:
                            return "unbeschränkt";
                        default:
                            return string.Format("{0}", slot.Occurrence.Capacity);
                    }

                }


                switch (CurrentDate.Occurrence.Capacity)
                {
                    case -1:
                        return "unbeschränkt";
                    default:
                        return string.Format("{0}", CurrentDate.Occurrence.Capacity);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SubscriptionTime
        {
            get
            {
                if (CurrentDate.Occurrence.UntilIsRestricted)
                {
                    if (CurrentDate.Occurrence.UntilDateTime.HasValue)
                    {
                        return CurrentDate.Occurrence.UntilDateTime.Value.ToString();
                    }

                    if (CurrentDate.Occurrence.UntilTimeSpan.HasValue)
                    {
                        var lastDate =
                            CurrentDate.Begin.AddHours(-CurrentDate.Occurrence.UntilTimeSpan.Value.Hours)
                                .AddMinutes(-CurrentDate.Occurrence.UntilTimeSpan.Value.Minutes);
                        return string.Format("{0} - {1}", lastDate.Date.ToShortDateString(), lastDate.TimeOfDay);
                    }
                }

                return "Keine Einschränkung";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasSubscriptions
        {
            get { return Slots.Any(s => s.Member.Any()); }
        }
    }

    /// <summary>
    /// 
    /// </summary>

    public class SingleSlotViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SingleSlotViewModel()
        {
            Member = new List<CourseMemberModel>();
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
        public Occurrence Occurrence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseMemberModel> Member { get; set; }
    }


    /// <summary>
    /// Darstellug eines Sprechstundentermins
    /// </summary>
    public class OfficeHourDatePreviewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourDatePreviewModel()
        {
            Subscriptions = new List<OccurrenceSubscription>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OccurrenceSubscription> Subscriptions { get; private set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourViewModel()
        {
            SlotStates = new List<SlotViewModel>();
            Member = new List<CourseMemberModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextOccurrence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseDateStateModel Date { get; set; }

        /// <summary>
        /// Status des Termins
        /// </summary>
        public OccurrenceStateModel State { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public List<SlotViewModel> SlotStates { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseMemberModel> Member { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SlotViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SlotViewModel()
        {
            Member = new List<CourseMemberModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActivitySlot Slot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public CourseDateStateModel Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseMemberModel> Member { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourCreateModel
    {
        public OrganiserMember Member { get; set; }


        public string Name { get; set; }

        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OfficeHourId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NewDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wochentag")]
        public int DayOfWeek { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beginn")]
        public string StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende")]
        public string EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Letztes Datum")]
        public string NewDateEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl Plätze je Termin bzw. Slot")]
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Slotlänge")]
        public int SlotDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl Reserveslots")]
        public int SpareSlots { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Eintragung zeitlich beschränken")]
        public int SubscriptionLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anlegen von Terminen")]
        public int DateOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzeigetext")]
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWeekly { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool UseSlots { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl belegbarer Slots pro Termin")]
        public int SlotsPerDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl zukünftiger Slots")]
        public int MaxFutureSlots { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

    }



    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourSlotViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Until { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseMemberModel Member { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid DateOccurrenceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RowNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Occurrence Occurrence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate ActivityDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasSubscriptions
        {
            get
            {
                if (ActivityDate == null)
                    return false;

                int sum = ActivityDate.Occurrence.Subscriptions.Count +
                    ActivityDate.Slots.Sum(slot => slot.Occurrence.Subscriptions.Count);

                return sum > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsHistory { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourDateViewModel
    {
        public OfficeHourDateViewModel()
        {
            Subscriptions = new List<OccurrenceSubscription>();
            AvailableSlots = new List<ActivitySlot>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// Der Slot in dem ich eingetragen bin
        /// </summary>
        public ActivitySlot Slot { get; set; }

        /// <summary>
        /// meine Suubscription, die ich zurückgeben kann
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RowNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySlot> AvailableSlots { get; set; }

        public int AvailableSeats { get; set; }


        public List<OccurrenceSubscription> Subscriptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate AvailableDate { get; set; }

        public DateTime EndOfSubscriptionPeriod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        public DateTime Begin
        {
            get
            {
                if (Slot != null)
                    return Slot.Begin;
                return Date.Begin;
            }
        }

        public DateTime End
        {
            get
            {
                if (Slot != null)
                    return Slot.End;
                return Date.End;
            }
        }

        public bool IsAvailable
        {
            get
            {
                if (Date.Occurrence.IsCanceled)
                    return false;

                if ((AvailableSlots == null || AvailableSlots.Count == 0) && AvailableSeats == 0)
                    return false;

                if (EndOfSubscriptionPeriod < DateTime.Now)
                    return false;

                return true;
            }
        }

        public bool IsCancelled
        {
            get { return Date.Occurrence.IsCanceled; }
        }

        public bool IsFullyBooked
        {
            get { return (AvailableSlots == null || AvailableSlots.Count == 0) && AvailableSeats == 0; }
        }

        public bool IsExpired
        {
            get { return EndOfSubscriptionPeriod < DateTime.Now; }
        }

        public string Capacity
        {
            get
            {
                if (Date.Slots.Any())
                {
                    return Date.Slots.Count.ToString();
                }

                if (Date.Occurrence.Capacity < 1)
                    return "unbegrenzt";

                return Date.Occurrence.Capacity.ToString();
            }
        }

        public string EndOfSubscription
        {
            get
            {
                if (!Date.Occurrence.UntilIsRestricted)
                    return "unbeschränkt";

                return EndOfSubscriptionPeriod.ToString();
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourMoveDateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

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
        public bool AdjustSlotCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourSubscriptionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourSubscriptionViewModel()
        {
            Slots = new List<OfficeHourAvailableSlotViewModel>();
            Dates = new List<OfficeHourDateViewModel>();
            MyDates = new List<ActivityDate>();
            MySlots = new List<ActivitySlot>();
        }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OfficeHourAvailableSlotViewModel> Slots { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OfficeHourDateViewModel> Dates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> MyDates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySlot> MySlots { get; set; }

        public int FutureSubCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourAvailableSlotViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

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
        public Occurrence Occurrence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSubscribed { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourOverviewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourOverviewModel()
        {
            OfficeHours = new List<OfficeHourDateViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OfficeHourDateViewModel> OfficeHours { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour MyOfficeHour { get; set; }
    }


    public class OfficeHourDateSubscriptionViewModel
    {
        public ActivityDate Date { get; set; }

        public ActivitySlot Slot { get; set; }

        public OrganiserMember Host { get; set; }
        
        public string Description { get; set; }


        public Semester Semester { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public List<ActivitySlot> AvailableSlots { get; set; }

        public Guid SlotID { get; set; }

        public bool IsExpired { get; set; }

    }



}

