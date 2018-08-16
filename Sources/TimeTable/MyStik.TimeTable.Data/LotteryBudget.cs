using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class LotteryBudget
    {
        public LotteryBudget()
        {
            Bets = new List<LotteryBet>();
        }

        /// <summary>
        /// Schlüssel
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung (HTML formatiert)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Beschränkung: steht nur auf Antrag zur Verfügung
        /// </summary>
        public bool IsRestricted { get; set; }

        /// <summary>
        /// Das Volumen
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Der Abklingfaktor
        /// </summary>
        public double Fraction { get; set; }

        /// <summary>
        /// Die zugehörige Verlosung
        /// </summary>
        public virtual Lottery Lottery { get; set; }

        /// <summary>
        /// Alle Einsätze
        /// </summary>
        public virtual ICollection<LotteryBet> Bets { get; set; }

    }
}
