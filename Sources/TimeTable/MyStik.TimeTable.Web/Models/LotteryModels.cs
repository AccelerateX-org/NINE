using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class LotteryCreateModel
    {
        public Guid LotteryId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name ="Kurname")]
        public string JobId { get; set; }

        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        [Display(Name = "Max. Teilnahme", Description = "Maxmimal Anzahl an Kursen, an denen ein Student teilnehmen kann")]
        public int MaxConfirm { get; set; }

        [Display(Name = "Datum der ersten Ziehung")]
        public string FirstDrawing { get; set; }

        [Display(Name = "Datum der letzten Ziehung")]
        public string LastDrawing { get; set; }

        [Display(Name = "Zeitpunkt der täglichen Ziehung")]
        public string DrawingTime { get; set; }

    }

    public class LotteryLotPotModel
    {
        public LotteryLotPotModel()
        {
            LotPotActivities = new List<IActivitySummary>();
        }

        public Guid LotteryId { get; set; }

        public Lottery Lottery { get; set; }

        public ICollection<IActivitySummary> LotPotActivities { get; set; }
    }

}