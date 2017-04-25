using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class AgendaViewModel
    {
        public AgendaViewModel()
        {
            Days = new List<AgendaDayViewModel>();
        }

        public List<AgendaDayViewModel> Days { get; private set; }

    }


    public class AgendaDayViewModel
    {
        public AgendaDayViewModel()
        {
            Activities = new List<AgendaActivityViewModel>();
        }

        public DateTime Day { get; set; }

        public List<AgendaActivityViewModel> Activities { get; private set; }


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

    public class AgendaActivityViewModel
    {
        public ActivityDate Date { get; set; }

        public ActivitySlot Slot { get; set; }

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