using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LotteryCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid LotteryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name ="Kurzname")]
        public string JobId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl Plätze für Teilnahme", Description = "Maxmimal Anzahl an Kursen, an denen ein Student teilnehmen kann")]
        public int MaxConfirm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum der ersten Ziehung")]
        public string FirstDrawing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum der letzten Ziehung")]
        public string LastDrawing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Zeitpunkt der täglichen Ziehung")]
        public string DrawingTime { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class LotteryLotPotModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LotteryLotPotModel()
        {
            PotElements = new List<LotteryLotPotCourseModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid LotteryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<LotteryLotPotCourseModel> PotElements { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LotteryLotPotCourseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary ActivitySummary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseSummaryModel CourseSummary { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LotterySummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalSubscriptionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalSubscriberCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double AvgSubscriptionCount { get; set; }

    }

}