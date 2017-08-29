using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class LotteryDrawing
    {
        public LotteryDrawing()
        {
            OccurrenceDrawings = new HashSet<OccurrenceDrawing>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Lottery Lottery { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Message { get; set; }

        public string JobId { get; set; }

        public virtual BinaryStorage Report { get; set; }

        public virtual ICollection<OccurrenceDrawing> OccurrenceDrawings { get; set; }

    }
}
