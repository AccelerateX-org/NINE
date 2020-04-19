using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class CopyDayViewModel
    {
        public ActivityOrganiser Organiser { get; set; }

        public Semester Semester { get; set; }

        [Display(Name="Studiengang")]
        public Guid CurrId { get; set; }

        [Display(Name = "Tag der kopiert werden soll")]
        public string SourceDate { get; set; }

        [Display(Name = "Tag, der die Termine erhalten soll")]
        public string TargetDate { get; set; }

        [Display(Name = "Verschieben, statt kopieren")]
        public bool Move { get; set; }
    }

    public class CopyDayReport
    {
        public Curriculum Curriculum { get; set; }

        public DateTime SourceDay { get; set; }

        public DateTime TargetDay { get; set; }

        public List<CopyDayCourseReport> CourseReports { get; set; }
    }

    public class CopyDayCourseReport
    {
        public Course Course { get; set; }

        public ActivityDate SourceDate { get; set; }

        public ActivityDate TargetDate { get; set; }

        public bool IsNew { get; set; }

        public bool IsMove { get; set; }
    }
}