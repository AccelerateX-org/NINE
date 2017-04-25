using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.GpUntis.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class SemesterDateViewModel
    {
        public Guid SemesterId { get; set; }

        public string Description { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public bool HasCourses { get; set; }
    }

    public class SemesterViewModel
    {
        public Semester Semester { get; set; }
        
    }

    public class SemesterCreateViewModel
    {
        public string Name { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string StartCourses { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public string EndCourses { get; set; }
    }

    public class SemesterEditViewModel
    {
        public Guid SemesterId { get; set; }

        public string Name { get; set; }

        public string StartCourses { get; set; }

        public string EndCourses { get; set; }
    }

    public class SemesteGroupInitModel
    {
        public Guid TargetSemesterId { get; set; }

        public string SourceSemesterName { get; set; }
    }


    public class SemesterImportModel
    {
        [Display(Name = "Fakultät")]
        public Guid OrganiserId { get; set; }


        [Display(Name = "Semester")]
        public Guid SemesterId { get; set; }


        public string Message { get; set; }

        public IEnumerable<string> FileNames { get; set; }

        public int Existing { get; set; }

        public Semester Semester { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public ICollection<HttpPostedFileBase> Attachments { get; set; }

    }

}