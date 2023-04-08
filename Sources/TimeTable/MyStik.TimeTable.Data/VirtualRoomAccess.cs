using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class VirtualRoomAccess
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Übernimmt EInstellungen des Raumes
        /// </summary>
        public bool isDefault { get; set; }

        /// <summary>
        /// Der Zugangscode für den einen Termin
        /// </summary>
        public string Token { get; set; }


        public virtual VirtualRoom Room { get; set; }

        public virtual ActivityDate Date { get; set; }

    }
}
