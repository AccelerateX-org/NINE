using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Infoscreen
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<ActivityOrganiser> Organisers { get; set; }

        public ICollection<InfoAnnouncement> Announcements { get; set; }

        public ICollection<InfoText> InfoTexts { get; set; }
    }
}
