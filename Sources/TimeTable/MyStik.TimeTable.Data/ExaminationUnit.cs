﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyStik.TimeTable.Data
{
    /*
    public class ExaminationUnit
    {
        public ExaminationUnit()
        {
            ExaminationAids = new HashSet<ExaminationAid>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Prüfungsform
        /// </summary>
        public ExaminationForm Form { get; set; }

        /// <summary>
        /// Prüfungsdauer in Minuten
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Gewichtung der Prüfungsleistung
        /// </summary>
        public double Weight { get; set; }


        // die Hilfsmittel

        /// <summary>
        /// Das zugehörige Module
        /// veraltet
        /// </summary>
        // public virtual TeachingBuildingBlock Module { get; set; }


        //public virtual TeachingAssessment Assessment { get; set; }

        public virtual ModuleDescription ModuleDescription { get; set; }


        public virtual ICollection<ExaminationAid> ExaminationAids { get; set; }
    }
    */


    public class ExaminationOption
    {
        public ExaminationOption()
        {
            Fractions = new List<ExaminationFraction>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ExaminationFraction> Fractions { get; set; }

        public virtual CurriculumModule Module { get; set; }

        public string FullName
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0}: ", Name);
                
                foreach(var fraction in Fractions)
                { 
                    sb.AppendFormat("{0} {1:P}. ", fraction.Form.ShortName, fraction.Weight);
                }

                return sb.ToString();
            }
        }

        public string OptionName
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var fraction in Fractions)
                {
                    sb.AppendFormat("{0} {1:P}. ", fraction.Form.ShortName, fraction.Weight);
                }

                return sb.ToString();
            }
        }

    }

    public class ExaminationFraction
    {
        public ExaminationFraction()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Prüfungsform
        /// </summary>
        public virtual ExaminationForm Form { get; set; }

        /// <summary>
        /// Prüfungsdauer in Minuten
        /// </summary>
        public int? MinDuration { get; set; }

        public int? MaxDuration { get; set; }

        /// <summary>
        /// Gewichtung der Prüfungsleistung
        /// </summary>
        public double Weight { get; set; }

        public virtual ExaminationOption ExaminationOption { get; set; }
    }
}
