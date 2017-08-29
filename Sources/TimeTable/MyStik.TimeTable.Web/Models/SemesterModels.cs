using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.GpUntis.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterDateViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid DateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasCourses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Wichtig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Eintaegig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Terminstehtnochnichtfest { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesterViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        
    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesterCreateViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StartCourses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EndCourses { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesterEditViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StartCourses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EndCourses { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesteGroupInitModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid TargetSemesterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SourceSemesterName { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SemesterImportModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Fakultät")]
        public Guid OrganiserId { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Display(Name = "Semester")]
        public Guid SemesterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Erster Vorlesungstag")]
        public string FirstDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Letzter Vorlesungstag")]
        public string LastDate { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Länge einer Einheit")]
        public int SlotLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Länge der Pausen")]
        public int BreakLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl Einheiten ohne Pause")]
        public int BreakFrequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Beginn der ersten Stunde")]
        public string BeginFirstSlot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> FileNames { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Existing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GroupGeneration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<HttpPostedFileBase> Attachments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase AttachmentDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase AttachmentHours { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase AttachmentGroups { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BaseImportContext Context { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesterCourseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

    }
}