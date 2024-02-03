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

        public ICollection<ActivityOrganiser> Organisers { get; set; }

        //public ICollection<InfoAnnouncement> Announcements { get; set; }

        //public ICollection<InfoText> InfoTexts { get; set; }
        
        public ICollection<InfoscreenPage> Pages { get; set; }

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

        public string Name { get; set; }

        public InfoscreeenPageType Type { get; set; }

        public virtual Infoscreen Infoscreen { get; set; }

        public virtual RoomAllocationGroup RoomAllocationGroup { get; set; }

        public virtual BinaryStorage Image { get; set; }

    }

}
