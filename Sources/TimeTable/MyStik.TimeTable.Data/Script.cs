using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ScriptDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Publisher
        /// </summary>
        public string UserId { get; set; }


        /// <summary>
        /// Datum der Veröffentlichung
        /// </summary>
        public DateTime Created { get; set; }

        public string Title { get; set; }

        public string Version { get; set; }

        public virtual BinaryStorage Storage { get; set; }

        public virtual ICollection<ScriptOrder> Orders { get; set; }

        public virtual  ICollection<ScriptPublishing> Publishings { get; set; }
    }


    public class ScriptPublishing
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime Published { get; set; }

        public virtual ScriptDocument ScriptDocument { get; set; }

        public virtual Course Course { get; set; }

    }




    public class OrderPeriod
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Titel 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Semesterbezug, damit werden die Lehrveranstaltungen ausgewählt
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Bezug zum Veranstalter
        /// Feingranulatioger wäre dann nur noch der Studiengang
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }


        /// <summary>
        /// Beginn des Bestellzeitraums
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// Ende des Bestellzeitraums
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Datum der letzten Durchführung der Bestellung
        /// Idee: nur eine Bestellung pro Bestellperiode sinnvoll
        /// </summary>
        public DateTime? LastProcessed { get; set; }

        /// <summary>
        /// Liste der Bestellungen
        /// </summary>
        public virtual ICollection<OrderBasket> Baskets { get; set; }
    }

    public class OrderBasket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public virtual OrderPeriod OrderPeriod { get; set; }

        /// <summary>
        /// Bestellnummer, wird bei der Bestellung generiert
        /// </summary>
        public int? OrderNumber { get; set; }

        /// <summary>
        /// Abgeholt / geliefert
        /// </summary>
        public bool? Delivered { get; set; }


        public virtual ICollection<ScriptOrder> Orders { get; set; }
    }

    public class ScriptOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual OrderBasket OrderBasket { get; set; }


        public virtual ScriptDocument ScriptDocument { get; set; }


        public DateTime OrderedAt { get; set; }
    }



}
