using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class AdvertisementInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Advertisement Advertisement { get; set; }

        public string Description { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
