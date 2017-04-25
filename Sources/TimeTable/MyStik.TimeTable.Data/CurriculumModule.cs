using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CurriculumModule
    {
        public CurriculumModule ()
        {
            ModuleCourses = new HashSet<ModuleCourse>();
            ModuleExams = new HashSet<ModuleExam>();
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
        /// </summary>
        public virtual CurriculumGroup Group { get; set; }


        /// <summary>
        /// Modulverantwortlicher
        /// </summary>
        public virtual OrganiserMember MV { get; set; }
        
        /// <summary>
        /// Liste der Lehrveranstaltungen zu diesem Modul
        /// </summary>
        public virtual ICollection<ModuleCourse> ModuleCourses { get; set; }

        /// <summary>
        /// Liste aller Modulprüfungen
        /// </summary>
        public virtual ICollection<ModuleExam> ModuleExams { get; set; }
    }
}
