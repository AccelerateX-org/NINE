using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class CurriculumViewModel
    {
        public Curriculum Curriculum { get; set; }

        public IEnumerable<SemesterGroup> SemesterGroups { get; set; }
        public IEnumerable<Curriculum> Curricula { get; set; }
    }

    public class GroupSelectionViewModel
    {
        [Display(Name = "Fakultät")]
        public String Faculty { get; set; }

        [Display(Name = "Studienprogramm")]
        public string Curriculum { get; set; }

        [Display(Name = "Semestergruppe")]
        public string Group { get; set; }

        [Display(Name="Semester")]
        public string Semester { get; set; }
    }

    public class CurriculumCreateAliasModel
    {
        public Guid CurriculumId { get; set; }

        [Display(Name = "Aliasname")]
        public string AliasName { get; set; }

        [Display(Name = "Name Curriculumsgruppe")]
        public string CurrGroupName { get; set; }

        [Display(Name = "Zusatz Semestergruppe")]
        public string SemGroupName { get; set; }
    }
}