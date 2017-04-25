using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace WIQuest.Web.Data
{
    public enum GenderType
    {
        [Display(Name = "männlich")]
        Male,
        
        [Display(Name = "weiblich")]
        Female
    }

    public enum AgeCohortType
    {
        [Display(Name = "jünger als 16 Jahre")]
        Age15OrYounger,

        [Display(Name = "16 Jahre")]
        Age16,

        [Display(Name = "17 Jahre")]
        Age17,

        [Display(Name = "18 Jahre")]
        Age18,

        [Display(Name = "19 Jahre")]
        Age19,

        [Display(Name = "20 Jahre")]
        Age20,

        [Display(Name = "21 bis 25 Jahre")]
        Age21To25,
        
        [Display(Name = "26 bis 30 Jahre")]
        Age26To30,

        [Display(Name = "älter als 30 Jahre")]
        Age31OrOlder,
    }

    public enum QualificationType
    {
        [Display(Name = "Abitur")]
        Abitur,

        [Display(Name = "Fachoberschule (FOS)")]
        FOS,

        [Display(Name = "Berufsoberschule (BOS)")]
        BOS,

        [Display(Name = "Meister")]
        Meister
    }


    public class User
    {
        /// <summary>
        /// Id des Benutzers
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Geschlecht
        /// </summary>
        [DisplayName("Ihr Geschlecht")]
        public GenderType Geschlecht { get; set; }


        /// <summary>
        /// Altergruppe
        /// </summary>
        [DisplayName("Ihr Alter")]
      //AlterSgruppe!!
        public AgeCohortType Altersgruppe { get; set; }


        /// <summary>
        /// Hochschulzugangsberechtigung
        /// </summary>
        [DisplayName("Ihre (angestrebte) Hochschulzugangsberechtigung")]
        public QualificationType Hochschulzugangsberechtigung { get; set; }



        /// <summary>
        /// Liste aller Antworten des Benutzers
        /// </summary>
        public virtual ICollection<QuestLog> Logs { get; set; }

      
    }
}

