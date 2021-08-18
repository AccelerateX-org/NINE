using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public string Description { get; set; }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        /// <summary>
        /// Datum der letzten Durchführung der Bestellung
        /// </summary>
        public DateTime? LastProcessed { get; set; }

        public virtual ICollection<OrderBasket> Baskets { get; set; }
    }

    public class OrderBasket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public virtual OrderPeriod OrderPeriod { get; set; }

        /// <summary>
        /// Bestellnummer
        /// </summary>
        public int? OrderNumber { get; set; }
    }

    public class ScriptOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual OrderBasket OrderBasket { get; set; }


        public virtual ScriptDocument ScriptDocument { get; set; }


    }



}
