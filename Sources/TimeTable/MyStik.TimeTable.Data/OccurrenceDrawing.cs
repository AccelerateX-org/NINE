using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class OccurrenceDrawing
    {
        public OccurrenceDrawing()
        {
            SubscriptionDrawings = new HashSet<SubscriptionDrawing>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual LotteryDrawing LotteryDrawing { get; set; }

        public virtual Occurrence Occurrence { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public virtual ICollection<SubscriptionDrawing> SubscriptionDrawings { get; set; }
    }
}
