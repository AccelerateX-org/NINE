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
            //ModuleAccreditations = new HashSet<ModuleAccreditation>();
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
        /// Gewichtung für Durchschnittswert, i.d.R. 100%, aber auch 25%, 50%
        /// Ein Wert von 0 bedeutet "nur TN"
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Das Volumen ausgedrückt in ECTS
        /// Das ist der Soll-Wert, der sich aus dem Curriculum ergibt
        /// Der Ist-Wert ergibt sich aus der Summe der akkreditierten Fächer
        /// Die Anzahl der zu belegenden Kurse ergibt sich aus der Summe der ECTS der akkreditierten Fächer
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


        //public virtual ICollection<ModuleAccreditation> ModuleAccreditations { get; set; }


        /// <summary>
        /// deprecated: wird in die Chips verschoben
        /// </summary>
        public virtual ICollection<SubjectAccreditation> SubjectAccreditations { get; set; }

        public virtual ICollection<SlotLoading> Loadings { get; set; }



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

    /// <summary>
    /// Bestückung eines Slots mit Chips
    /// Ein Chip ist dabei wieder ein Platzhalter für Fächer
    /// Damit ergibt sich dann letztlich die Auswahl als Grundlage für Wahlverfahren
    /// </summary>
    public class SlotLoading
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public virtual CurriculumSlot Slot { get; set; }
    }

    public class SlotLoadingChip
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Teilmenge, die Summe aller Chips muss die Slotgröße ergeben
        /// </summary>
        public int ECTS { get; set; }

        public virtual SlotLoading Loading { get; set; }

        public virtual ICollection<SubjectAccreditation> SubjectAccreditations { get; set; }
    }

}
