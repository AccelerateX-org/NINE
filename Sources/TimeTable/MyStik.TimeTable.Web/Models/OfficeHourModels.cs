using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OfficeHourSummaryModel
    {
        public OfficeHourSummaryModel()
        {
            Dates = new List<CourseDateModel>();
            Lecturers = new List<OrganiserMember>();
        }
        public OfficeHour OfficeHour { get; set; }

        public List<CourseDateModel> Dates { get; set; }

        public List<OrganiserMember> Lecturers { get; set; }
    }

    public class OfficeHourCharacteristicModel
    {
        public OfficeHourCharacteristicModel()
        {
            CurrentSlots = new List<OfficeHourSlotViewModel>();
        }

        public OfficeHour OfficeHour { get; set; }

        public Semester Semester { get; set; }

        public List<OfficeHourSlotViewModel> CurrentSlots { get; private set; }

        public OrganiserMember Host { get; set; }

        public ActivityDate Date { get; set; }
    }

    public class OfficeHourDateSlotViewModel
    {
        public OfficeHourDateSlotViewModel()
        {
            Slots = new List<SingleSlotViewModel>();
        }

        public OfficeHour OfficeHour { get; set; }

        public Semester Semester { get; set; }


        public ActivityDate CurrentDate { get; set; }

        public ActivityDate PreviousDate { get; set; }

        public ActivityDate NextDate { get; set; }

        public OccurrenceStateModel State { get; set; }

        public List<SingleSlotViewModel> Slots { get; set; }

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

        public bool HasSubscriptions
        {
            get { return Slots.Any(s => s.Member.Any()); }
        }
    }

    public class SingleSlotViewModel
    {
        public SingleSlotViewModel()
        {
            Member = new List<CourseMemberModel>();
        }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public Occurrence Occurrence { get; set; }

        public List<CourseMemberModel> Member { get; set; }
    }


    /// <summary>
    /// Darstellug eines Sprechstundentermins
    /// </summary>
    public class OfficeHourDatePreviewModel
    {
        public OfficeHourDatePreviewModel()
        {
            CurrentSlots = new List<OfficeHourSlotViewModel>();
        }

        public OfficeHour OfficeHour { get; set; }

        public ActivityDate Date { get; set; }

        public List<OfficeHourSlotViewModel> CurrentSlots { get; private set; }
    }



    public class OfficeHourViewModel
    {
        public OfficeHourViewModel()
        {
            SlotStates = new List<SlotViewModel>();
            Member = new List<CourseMemberModel>();
        }

        public OfficeHour OfficeHour { get; set; }

        public OrganiserMember Lecturer { get; set; }

        public ActivityDate NextOccurrence { get; set; }

        public CourseDateStateModel Date { get; set; }

        /// <summary>
        /// Status des Termins
        /// </summary>
        public OccurrenceStateModel State { get; set;  }

        public List<SlotViewModel> SlotStates { get; private set; }

        public List<CourseMemberModel> Member { get; set; }
    }

    public class SlotViewModel
    {
        public SlotViewModel()
        {
            Member = new List<CourseMemberModel>();
        }


        public ActivitySlot Slot { get; set; }
        public OccurrenceStateModel State { get; set; }

        public CourseDateStateModel Date { get; set; }

        public List<CourseMemberModel> Member { get; set; }
    }

    public class OfficeHourCreateModel
    {
        public Guid OfficeHourId { get; set; }

        [Display(Name="Kurzname Dozent")]
        public string DozId { get; set; }

        public string NewDate { get; set; }

        public string Semester { get; set; }

        [Display(Name = "Wochentag")]
        public int DayOfWeek { get; set; }

        [Display(Name = "Beginn")]
        public string StartTime { get; set; }

        [Display(Name = "Ende")]
        public string EndTime { get; set; }

        [Display(Name = "Anzahl Plätze je Termin bzw. Slot")]
        public int Capacity { get; set; }

        [Display(Name = "Slotlänge")]
        public int SlotDuration { get; set; }

        [Display(Name = "Anzahl Reserveslots")]
        public int SpareSlots { get; set; }

        [Display(Name = "Eintragung zeitlich beschränken")]
        public int SubscriptionLimit { get; set; }


        [Display(Name = "Anlegen von Terminen")]
        public int DateOption { get; set; }

        [Display(Name = "Anzeigetext")]
        public string Text { get; set; }

        public int Type { get; set; }

        public bool IsWeekly { get; set; }


        [Display(Name = "Wochentag")]
        public int DayOfWeek2 { get; set; }

        [Display(Name = "Beginn")]
        public string StartTime2 { get; set; }

        [Display(Name = "Ende")]
        public string EndTime2 { get; set; }

        [Display(Name = "Eintragung zeitlich beschränken")]
        public int SubscriptionLimit2 { get; set; }
    }




    public class OfficeHourSlotViewModel
    {
        public DateTime Date { get; set; }

        public TimeSpan From { get; set; }

        public TimeSpan Until { get; set; }

        public CourseMemberModel Member { get; set; }

        public Guid DateOccurrenceId { get; set; }

        public int RowNo { get; set; }

        public int RowCount { get; set; }

        public OccurrenceStateModel State { get; set; }

        public int SubscriptionNo { get; set; }

        public int SubscriptionCount { get; set; }

        public Occurrence Occurrence { get; set; }

        public ActivityDate ActivityDate { get; set; }

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

        public bool IsHistory { get; set; }
    }

    public class OfficeHourDateViewModel
    {
        public OfficeHour OfficeHour { get; set; }

        public OrganiserMember Lecturer { get; set; }

        public ActivityDate Date { get; set; }

        public ActivitySlot Slot { get; set; }

        public int RowNo { get; set; }

        public int RowCount { get; set; }

        public OccurrenceStateModel State { get; set; }
    }

    public class OfficeHourMoveDateModel
    {
        public OfficeHour OfficeHour { get; set; }
        public ActivityDate Date { get; set; }


        public Guid ActivityId { get; set; }

        public Guid ActivityDateId { get; set; }

        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        [Display(Name = "Ende")]
        public string NewEnd { get; set; }


        public bool AdjustSlotCount { get; set; }
    }


    public class OfficeHourSubscriptionViewModel
    {
        public OfficeHourSubscriptionViewModel()
        {
            Slots = new List<OfficeHourAvailableSlotViewModel>();
        }

        public OfficeHour OfficeHour { get; set; }

        public Semester Semester { get; set; }

        public OrganiserMember Host { get; set; }

        public List<OfficeHourAvailableSlotViewModel> Slots { get; set; }

    }

    public class OfficeHourAvailableSlotViewModel
    {
        public DateTime Date { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public Occurrence Occurrence { get; set; }

        public OccurrenceStateModel State { get; set; }

        public string Remark { get; set; }

        public bool IsSubscribed { get; set; }
    }


}

