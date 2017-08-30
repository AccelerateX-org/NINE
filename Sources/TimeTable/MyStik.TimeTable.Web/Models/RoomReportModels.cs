using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;
using System.ComponentModel.DataAnnotations;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomReportRequestModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string RoomNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage ="Bitte Zeitraum wählen")]
        public int Days { get; set; }

        /// <summary>
        /// wird benötigt für Datumseingabe aus dem Kalender
        /// </summary>
        public string fromCalendar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string untilCalendar { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class StringListModel
        {
        /// <summary>
        /// 
        /// </summary>
            public string DataPoint { get; set; }
        }

    /// <summary>
    /// 
    /// </summary>
    public class RoomReportResponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomReportResponseModel()
        {
            Statistics = new List<RoomStatisticsModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime from { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime until { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<RoomStatisticsModel> Statistics { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public StringListModel StringList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DayAmountValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string fromCalendarTransport { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string untilCalendarTransport { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomStatisticsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string EndOfDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DateCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalMinutes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Wochentag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Auslastung { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Werktag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Day { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class InputValue
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Input { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FlotSeriesModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<List<string>> data { get; set; }
    }


}