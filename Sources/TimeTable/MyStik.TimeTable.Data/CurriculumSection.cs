using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        /// Maskiert, je nach Weg - mal sehen, ob das im LINQ klappt
        /// </summary>
        public int Semester
        {
            get => Position;
            set => Position = value;
        }

        /// <summary>
        /// Ettikett, innherlab eines Curriculums eindeutig
        /// </summary>
        public string Tag { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }


        /// <summary>
        /// Das Volumen ausgedrückt in ECTS
        /// </summary>
        public double ECTS { get; set; }


        public virtual ItemLabelSet LabelSet { get; set; }


        /// <summary>
        /// der eine Weg
        /// </summary>
        public virtual CurriculumSection CurriculumSection { get; set; }

        /// <summary>
        /// der andere Weg
        /// </summary>
        public virtual AreaOption AreaOption { get; set; }


        public virtual ICollection<ModuleAccreditation> ModuleAccreditations { get; set; }

        public virtual ICollection<SubjectAccreditation> SubjectAccreditations { get; set; }


        public string FullTag
        {
            get
            {
                if (CurriculumSection == null && AreaOption != null)
                {
                    return $"{AreaOption.Area.Tag}#{AreaOption.Tag}#{Tag}";
                }
                else if (CurriculumSection != null && AreaOption == null)
                {
                    if (LabelSet != null && LabelSet.ItemLabels.Any())
                    {
                        var label = LabelSet.ItemLabels.First();
                        return $"{label.Name}#{Tag}";
                    }
                    else
                    {
                        return Tag;
                    }
                }
                else
                {
                    return "#INVALID#";
                }
            }
        }
    }
}
