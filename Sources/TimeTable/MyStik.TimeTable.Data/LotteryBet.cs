using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class LotteryBet
    {
        /// <summary>
        /// Schlüssel
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der Betrag des Einsatzes
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Darf das Budget nutzen (Nachweis erbracht)
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Einsatz verbraucht - unabhängig vom Betrag
        /// </summary>
        public bool IsConsumed { get; set; }

        /// <summary>
        /// Das verbrauchte Budget
        /// </summary>
        public int AmountConsumed { get; set; }

        /// <summary>
        /// Das zugehörige Budget
        /// </summary>
        public virtual LotteryBudget Budget { get; set; }

        /// <summary>
        /// Die zugehörige Eintragung
        /// </summary>
        public virtual OccurrenceSubscription Subscription { get; set; }

    }
}
