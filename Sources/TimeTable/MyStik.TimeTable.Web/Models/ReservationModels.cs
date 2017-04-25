using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ReservationViewModel
    {
        public Reservation Reservation { get; set; }

        public ApplicationUser Owner { get; set; }


    }

    public class ReservationCreateModel
    {
        public ICollection<Room> Rooms { get; set; }

        [Display(Name = "Name")]

        public string Name { get; set; }

        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Display(Name = "Datum")]
        public string NewDate { get; set; }

        [Display(Name = "Beginn")]
        public string NewBegin { get; set; }

        [Display(Name = "Ende")]
        public string NewEnd { get; set; }

        public ICollection<Guid> RoomIds { get; set; }


        public bool IsWeekly { get; set; }
        public bool IsDaily { get; set; }

        [Display(Name = "Ende täglich")]
        public string DailyEnd { get; set; }

        [Display(Name = "Ende wöchentlich")]
        public string WeeklyEnd { get; set; }
    }
}