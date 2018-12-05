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
    public class ModuleCatalogViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumModule Module { get; set; }


    }

    public class ModuleCreateViewModel
    {
        public Course Course { get; set; }

        public Curriculum Curriculum { get; set; }

        public OrganiserMember MV { get; set; }

        [Display(Name = "Bezeichnung")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kurzbezeichnung")]
        [Required]
        public string ShortName { get; set; }

        [Display(Name="Kennung nach Studienplan / SPO")]
        [Required]
        public string CatalogId { get; set; }

        [AllowHtml]
        [Display(Name="Beschreibung / Inhalt / Litertaur etc.")]
        public string Description { get; set; }

        [Display(Name="ECTS")]
        public int Ects { get; set; }

        [Display(Name="SWS")]
        public int Sws { get; set; }

        [Display(Name="US Credits")]
        public int UsCredits { get; set; }


    }


    public class ModuleSelectViewModel
    {
        public Course Course { get; set; }

        public Curriculum Curriculum { get; set; }


        public Guid PackageId { get; set; }

        public Guid OptionId { get; set; }

        public Guid ModuleId { get; set; }

    }

    public class ModuleAssignViewModel
    {
        public ModuleAssignViewModel()
        {
            Modules = new List<TeachingBuildingBlock>();
        }

        public ModuleAccreditation Accreditation { get; set; }

        public List<TeachingBuildingBlock> Modules { get; set; }
    }

}