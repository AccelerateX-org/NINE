using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Infoscreen
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Das ist der Teil der URL
        /// </summary>
        public string Tag { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PublicTransporrtInfo { get; set; }

        public string PublicTransporrtUrl { get; set; }

        public virtual ICollection<ActivityOrganiser> Organisers { get; set; }
       
        public virtual ICollection<InfoscreenPage> Pages { get; set; }

    }

    public enum InfoscreeenPageType
    {
        Playing,    // Jetzt läuft (mit danach kommt)
        Idle,       // Freie Räume
        Ad,         // Werbung => nur das Image
        Event       // Das Activity Date und ggf. das Image
    }

    public class InfoscreenPage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Korrespondiert zur Reihenfolge der Seiten
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Wenn gesetzt, dann wird das als Untertitel angezeigt
        /// </summary>
        public string Name { get; set; }

        public InfoscreeenPageType Type { get; set; }

        public virtual Infoscreen Infoscreen { get; set; }

        public virtual RoomAllocationGroup RoomAllocationGroup { get; set; }

        public virtual BinaryStorage Image { get; set; }

    }

}
