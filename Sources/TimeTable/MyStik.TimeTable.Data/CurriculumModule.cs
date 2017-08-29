using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CurriculumModule
    {
        public CurriculumModule ()
        {
            ModuleCourses = new HashSet<ModuleCourse>();
            ModuleExams = new HashSet<ModuleExam>();
            Groups = new HashSet<CurriculumGroup>();
            Accreditations = new HashSet<ModuleAccreditation>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fachlicher Schlüssel (keine Beschränkung auf Identität)
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kurzname des Moduls
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Anzahl der Credits für das Modul
        /// </summary>
        public int ECTS { get; set; }

        /// <summary>
        /// Inhaltsbeschreibung (Elemente, beliebig strukturiert, HTML formatiert)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Zugehörige Studiengruppe
        /// Überflüssig!
        /// </summary>
        public virtual ICollection<CurriculumGroup> Groups { get; set; }


        /// <summary>
        /// Modulverantwortlicher
        /// </summary>
        public virtual OrganiserMember MV { get; set; }
        

        /// <summary>
        /// Dozent
        /// </summary>
        public int Lecturer { get; set; }

        /// <summary>
        /// Sprache der Veranstaltung
        /// </summary>
        public int Language { get; set; }

        /// <summary>
        /// Lehrform
        /// </summary>
        public int SWS { get; set; }

        /// <summary>
        /// Arbeitsaufwand
        /// </summary>
        public int Work { get; set; }

        /// <summary>
        /// Voraussetzungen für Modul
        /// </summary>
        public int Requirements { get; set; }

        /// <summary>
        /// Lernziele/Kompetenzen
        /// </summary>
        public int Skills { get; set; }

        /// <summary>
        /// Verbindliche Lehrinhalte
        /// </summary>
        public int Topic { get; set; }

        /// <summary>
        /// Studien-/ Prüfungsleistungen
        /// </summary>
        public int Leistung { get; set; }

        /// <summary>
        /// Literatur
        /// </summary>
        public int Books { get; set; }

        /// <summary>
        /// Liste der akkreditierten Module
        /// </summary>
        public virtual ICollection<ModuleCourse> ModuleCourses { get; set; }

        /// <summary>
        /// Liste aller Modulprüfungen
        /// </summary>
        public virtual ICollection<ModuleExam> ModuleExams { get; set; }

        /// <summary>
        /// Alle Akkreditierungen des Moduls
        /// </summary>
       public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }
    }
}
