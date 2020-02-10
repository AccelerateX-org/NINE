using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class AdvertisementCreateModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Titel")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }


        [Display(Name = "Praktisches Studiensemester")]
        public bool ForInternship { get; set; }

        [Display(Name = "Abschlussarbeit")]
        public bool ForThesis { get; set; }

        [Display(Name = "Auslandsaufenthalt")]
        public bool ForStayAbroad { get; set; }

        [Display(Name = "Stipendien")]
        public bool ForAdvancement { get; set; }


        [Display(Name = "SHK, Tutoren")]
        public bool ForTutor { get; set; }


        [Display(Name = "Werkstudententätigkeit")]
        public bool ForWorkingStudent { get; set; }


        [Display(Name = "Studentische Wettbewerbe")]
        public bool ForCompetition { get; set; }


        [Display(Name = "Sichtbar")]
        public string ExpiryDate { get; set; }

        public HttpPostedFileBase Attachment1 { get; set; }
    }


    public class AdvertisementNewModel
    {
        public List<Advertisement> Internships { get; set; }
        public List<Advertisement> Theses { get; set; }
        public List<Advertisement> StayAbroads { get; set; }
        public List<Advertisement> Advancements { get; set; }
        public List<Advertisement> WorkingStudents { get; set; }
        public List<Advertisement> Tutors { get; set; }
        public List<Advertisement> Competitions { get; set; }
    }
}