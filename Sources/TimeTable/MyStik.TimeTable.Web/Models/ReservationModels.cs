using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ReservationViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Reservation Reservation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser Owner { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class ReservationCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICollection<Room> Rooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]

        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

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
        public bool IsWeekly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDaily { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende täglich")]
        public string DailyEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Ende wöchentlich")]
        public string WeeklyEnd { get; set; }
    }
}