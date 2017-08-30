using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AgendaViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public AgendaViewModel()
        {
            Days = new List<AgendaDayViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AgendaDayViewModel> Days { get; private set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AgendaDayViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public AgendaDayViewModel()
        {
            Activities = new List<AgendaActivityViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AgendaActivityViewModel> Activities { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get
            {
                if (Day.Date == GlobalSettings.Today)
                {
                    return "Heute";
                }
                else if (Day.Date == GlobalSettings.Today.AddDays(1))
                {
                    return "Morgen";
                }
                else
                {
                    return Day.ToShortDateString();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AgendaActivityViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivityDate Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivitySlot Slot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Begin
        {
            get
            {
                if (Slot != null)
                {
                    return Slot.Begin;
                }
                else
                {
                    return Date.Begin;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime End
        {
            get
            {
                if (Slot != null)
                {
                    return Slot.End;
                }
                else
                {
                    return Date.End;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AgendaStyle
        {
            get
            {
                if (Date.Activity is Course)
                    return "note-course";
                if (Date.Activity is OfficeHour)
                    return "note-officehour";
                if (Date.Activity is Event)
                    return "note-event";
                return "note-hm";
            }
            
        }

    }

}