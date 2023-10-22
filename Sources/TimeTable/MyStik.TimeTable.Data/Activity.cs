using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Activity
    {
        public Activity()
        {
            this.Dates = new HashSet<ActivityDate>();
            this.Ressources = new HashSet<BinaryStorage>();
            this.SemesterGroups = new HashSet<SemesterGroup>();
            this.SemesterTopics = new HashSet<SemesterTopic>();
            this.Owners = new HashSet<ActivityOwner>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Langname der Aktivität
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Kurname (optional)
        /// </summary>
        [Display(Name = "Kurzbezeichnung")]
        public string ShortName { get; set; }

        /// <summary>
        /// Beschreibung (optional), HTML encoded
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// wird unterschiedlich verwendet
        /// Course: "gesperrt"
        /// Event: "veröffentlichen"
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// Status: ist geplant
        /// default: false
        /// </summary>
        public bool IsProjected { get; set; }

        /// <summary>
        /// Kommaseparierte Liste mit Länder / Sprachkürzeln
        /// z.B. de,en
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Name des externen Systems, wenn importiert wurde
        /// </summary>
        public string ExternalSource { get; set; }

        /// <summary>
        /// Eine Nummer für Import aus anderen Systemen
        /// </summary>
        public string ExternalId { get; set; }


        /// <summary>
        /// Über die Semestergoup wird geregelt, in welchen Stundenplänen
        /// die Activity auftaucht
        /// </summary>
        public virtual ICollection<SemesterGroup> SemesterGroups { get; set; }

        /// <summary>
        /// Über die Semestertopics wird geregelt, in welchen Stundenplänen
        /// die Activity auftaucht
        /// </summary>
        public virtual ICollection<SemesterTopic> SemesterTopics { get; set; }

        /// <summary>
        /// Für die Verortung, bisher nur bei Sprechstunde und Exam
        /// Vermeintliche Doppelung zu den Semestergruppen
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Zuordnung zu einem Segment
        /// Angabe des semesters damit redundent, aber wer wei0 wozu es gut ist
        /// </summary>
        public virtual SemesterDate Segment { get; set; }


        public virtual Occurrence Occurrence { get; set; }

        public virtual ItemLabelSet LabelSet { get; set; }


        public virtual ICollection<BinaryStorage> Ressources { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual ICollection<ActivityDate> Dates { get; set; }

        public virtual ICollection<ActivityOwner> Owners { get; set; }

        public virtual ICollection<ContentChannel> Channels { get; set; }

    }
}
