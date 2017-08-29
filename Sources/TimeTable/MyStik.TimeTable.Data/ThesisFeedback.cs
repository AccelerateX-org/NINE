using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ThesisFeedback
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int Earning { get; set; }

        public  virtual ThesisWorkflow Workflow { get; set; }

        public virtual ThesisProvider Provider { get; set; }

    }
}
