using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class LotteryDrawing
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Lottery Lottery { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Message { get; set; }

        public string JobId { get; set; }

        public virtual BinaryStorage Report { get; set; }

    }
}
