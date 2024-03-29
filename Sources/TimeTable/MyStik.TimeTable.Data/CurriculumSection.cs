﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CurriculumSection
    {
        public CurriculumSection()
        {
            Slots = new HashSet<CurriculumSlot>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Zeitliche oder inhaltliche Abfolge innerhalb des Curriculums
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Name, Bezeichnung
        /// </summary>
        public  string Name { get; set; }

        public virtual Curriculum Curriculum { get; set; }

        public virtual ICollection<CurriculumSlot> Slots { get; set; }

    }


    public class CurriculumSlot
    {
        public CurriculumSlot()
        {
            ModuleAccreditations = new HashSet<ModuleAccreditation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Anordnung innerhalb der Section
        /// keine inhaltliche Bedeutung, nur für Generierung "gewohnter" Anzeigen
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Ettikett, innherlab eines Curriculums eindeutig
        /// </summary>
        public string Tag { get; set; }


        public string Name { get; set; }


        /// <summary>
        /// Das Volumen ausgedrückt in ECTS
        /// </summary>
        public double ECTS { get; set; }


        public virtual ItemLabelSet LabelSet { get; set; }


        public virtual CurriculumSection CurriculumSection { get; set; }



        public virtual ICollection<ModuleAccreditation> ModuleAccreditations { get; set; }

    }
}
