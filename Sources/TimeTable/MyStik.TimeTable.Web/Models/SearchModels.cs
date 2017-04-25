using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class SearchViewModel
    {
        public string SearchText { get; set; }

        public Semester Semester { get; set; }

        public ICollection<CourseSummaryModel> Courses { get; set; }

        public ICollection<LecturerViewModel> Lecturers { get; set; }
    }
}