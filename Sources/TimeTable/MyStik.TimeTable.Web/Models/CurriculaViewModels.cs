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
    public class CurriculaCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OrganiserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ModuleViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Shortcut { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MV { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Dozent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Assignment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SWS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Work { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Requirements { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Skills { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Leistungen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Books { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumModuleCreateModel
    {
        /// <summary>
        /// 
        /// </summary>

        [Display(Name = "Fachlicher Schlüssel")]
        public string ModuleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Modulbezeichnung")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Stundenplankürzel")]
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [AllowHtml]
        [Display(Name = "Modulverantwortlicher")]
        public string MV { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Dozent")]
        public string Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Sprache")]
        public string Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Zuordnung zum Curriculum")]
        public string Groups { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Lehrform/SWS")]
        public string SWS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Arbeitsaufwand")]
        public string Work { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Kreditpunkte")]
        public string ECTS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Voraussetzungen")]
        public string Requirements { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Lernziele/Kompetenzen")]
        public string Skills { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Studien-/Prüfungsleistung")]
        public string Leistung { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Literatur")]
        public string Books { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumGroupCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl der Kapazitätsgruppen")]
        public int CapacityGroupCount { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Als Studiengruppe für Studierende")]
        public bool IsSubscribable { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CurriculumGroupEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumGroup CurriculumGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Als Studiengruppe für Studierende")]
        public bool IsSubscribable { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CapacityGroupCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CurriculumGroup CurriculumGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wird im WS angeboten")]
        public bool InWS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wird im SS angeboten")]
        public bool InSS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Externe IDs (Komma getrennt)")]
        public string AliasList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CapacityGroupEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CapacityGroup CapacityGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wird im WS angeboten")]
        public bool InWS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wird im SS angeboten")]
        public bool InSS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Externe IDs (Komma getrennt)")]
        public string AliasList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SelectModuleViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CurriculumCriteria Criteria { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CurriculumModule> Modules { get; set; }
    }
}   