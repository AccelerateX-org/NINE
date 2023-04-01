using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyStik.Gym.Data
{
    public class Label
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string BackgroundColor { get; set; }

        public virtual ICollection<QuestionLabel> QuestionLabels { get; set; }

    }

    public class QuestionLabel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public virtual Label Label { get; set; }

        public virtual Question Question { get; set; }

    }
}
